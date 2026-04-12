using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LubanGui.Infrastructure;
using LubanGui.LubanAdapter.Dtos;
using LubanGui.LubanAdapter.Interfaces;
using LubanGui.Models;
using Microsoft.Extensions.Logging;

namespace LubanGui.Services;

/// <summary>
/// 实现 <see cref="ISchemaService"/>，通过 <see cref="ILubanSchemaReader"/> 读取 Luban Schema，
/// 通过 <see cref="ExcelWriter"/> 写入元数据 xlsx 文件。
/// </summary>
public class SchemaService : ISchemaService
{
    private readonly ILubanSchemaReader _schemaReader;
    private readonly ILogger<SchemaService> _logger;

    public SchemaService(ILubanSchemaReader schemaReader, ILogger<SchemaService> logger)
    {
        _schemaReader = schemaReader;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<List<TableMeta>> LoadTablesAsync(string projectPath)
    {
        var confPath = Path.Combine(projectPath, "luban.conf");

        if (!File.Exists(confPath))
        {
            _logger.LogWarning("luban.conf 不存在，跳过 Schema 读取：{Path}", confPath);
            return [];
        }

        try
        {
            var tableDtos = await _schemaReader.ReadTablesAsync(confPath);
            var tables = tableDtos.Select(ToTableMeta).ToList();

            _logger.LogInformation("加载了 {Count} 张表格 from {Path}", tables.Count, confPath);
            return tables;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "读取 Schema 失败：{Path}", confPath);
            throw;
        }
    }

    /// <summary>将 <see cref="TableSchemaDto"/> 映射为 <see cref="TableMeta"/>。</summary>
    private static TableMeta ToTableMeta(TableSchemaDto dto) => new()
    {
        FullName = dto.FullName,
        ValueType = dto.ValueType,
        Index = dto.Index,
        Mode = dto.Mode,
        Group = string.Join(",", dto.Groups),
        Comment = dto.Comment,
        ReadSchemaFromFile = dto.ReadSchemaFromFile,
        Input = dto.InputFiles.Count > 0 ? dto.InputFiles[0] : string.Empty,
        Output = dto.OutputFile,
        Tags = string.Empty,
    };

    /// <inheritdoc/>
    public async Task<TableMeta> CreateTableAsync(
        string projectPath,
        string fullName,
        string indexField,
        IReadOnlyList<FieldDefinition> fields)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("表格全名不能为空", nameof(fullName));
        }

        // 推导相对路径：将 "cfg.Item" → "cfg/Item.xlsx"
        var relativePath = fullName.Replace('.', '/') + ".xlsx";
        var xlsxAbsPath = Path.Combine(projectPath, "Datas", relativePath);

        _logger.LogInformation("创建表格 {ValueType} → {Path}", fullName, xlsxAbsPath);

        // 1. 创建数据 xlsx
        ExcelWriter.CreateDataXlsx(xlsxAbsPath, fields);

        // 2. 构建 TableMeta
        // Luban 规范：当 read_schema_from_file=true 时，full_name（table 容器类型）
        // 与 value_type（记录 bean 类型）必须不同，否则 DefAssembly.AddType 会报 duplicate。
        // 约定：full_name = "Tb" + 首字母大写(shortName)，value_type = 用户输入的原始名称。
        var tableFullName = BuildTableFullName(fullName);
        var table = new TableMeta
        {
            FullName = tableFullName,
            ValueType = fullName,
            Index = indexField,
            Mode = string.Empty,
            Group = string.Empty,
            Comment = string.Empty,
            ReadSchemaFromFile = true,
            Input = relativePath,
            Output = string.Empty,
            Tags = string.Empty,
        };

        // 3. 追加到 __tables__.xlsx（写入前先清理已有重复条目）
        var tablesXlsx = Path.Combine(projectPath, "Datas", "__tables__.xlsx");
        await Task.Run(() =>
        {
            int removed = ExcelWriter.RemoveDuplicateTableEntries(tablesXlsx);
            if (removed > 0)
                _logger.LogWarning("__tables__.xlsx 中发现并清除了 {Count} 条重复记录。", removed);

            try
            {
                ExcelWriter.AppendTableEntry(tablesXlsx, table);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "跳过写入：{FullName}", table.FullName);
            }
        });

        return table;
    }

    /// <inheritdoc/>
    public async Task<TableMeta> ImportTableAsync(
        string projectPath,
        string xlsxAbsPath,
        string fullName,
        string indexField)
    {
        if (!File.Exists(xlsxAbsPath))
        {
            throw new FileNotFoundException($"文件不存在：{xlsxAbsPath}");
        }

        var datasDir = Path.Combine(projectPath, "Datas");

        // 计算相对于 Datas/ 的路径
        string relativePath;
        if (xlsxAbsPath.StartsWith(datasDir, StringComparison.OrdinalIgnoreCase))
        {
            relativePath = Path.GetRelativePath(datasDir, xlsxAbsPath).Replace('\\', '/');
        }
        else
        {
            // 不在 Datas/ 目录内，只取文件名
            relativePath = Path.GetFileName(xlsxAbsPath);
        }

        var table = new TableMeta
        {
            FullName = BuildTableFullName(fullName),
            ValueType = fullName,
            Index = indexField,
            Mode = string.Empty,
            Group = string.Empty,
            Comment = string.Empty,
            ReadSchemaFromFile = true,
            Input = relativePath,
            Output = string.Empty,
            Tags = string.Empty,
        };

        var tablesXlsx = Path.Combine(datasDir, "__tables__.xlsx");
        await Task.Run(() =>
        {
            int removed = ExcelWriter.RemoveDuplicateTableEntries(tablesXlsx);
            if (removed > 0)
                _logger.LogWarning("__tables__.xlsx 中发现并清除了 {Count} 条重复记录。", removed);

            try
            {
                ExcelWriter.AppendTableEntry(tablesXlsx, table);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "跳过写入：{FullName}", table.FullName);
            }
        });

        _logger.LogInformation("导入表格 {FullName} → {RelPath}", fullName, relativePath);
        return table;
    }

    /// <inheritdoc/>
    public Task CreateEnumAsync(
        string projectPath,
        string fullName,
        IReadOnlyList<EnumItemDefinition> items)
    {
        // MVP：创建独立的枚举记录 xlsx（用于人工参考，未集成到 __enums__.xlsx）
        _logger.LogInformation("创建枚举 {FullName}（{Count} 个值）", fullName, items.Count);

        var fields = new List<FieldDefinition>
        {
            new() { Name = "name",    Type = "string", Comment = "枚举名" },
            new() { Name = "alias",   Type = "string", Comment = "别名" },
            new() { Name = "value",   Type = "string", Comment = "枚举值" },
            new() { Name = "comment", Type = "string", Comment = "说明" },
        };

        var relativePath = fullName.Replace('.', '/') + "_enum.xlsx";
        var xlsxPath = Path.Combine(projectPath, "Datas", relativePath);

        ExcelWriter.CreateDataXlsx(xlsxPath, fields);

        // TODO（未来）：集成到 __enums__.xlsx
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task CreateBeanAsync(
        string projectPath,
        string fullName,
        IReadOnlyList<FieldDefinition> fields)
    {
        // MVP：创建独立的 Bean 记录 xlsx（用于人工参考，未集成到 __beans__.xlsx）
        _logger.LogInformation("创建 Bean {FullName}（{Count} 个字段）", fullName, fields.Count);

        var relativePath = fullName.Replace('.', '/') + "_bean.xlsx";
        var xlsxPath = Path.Combine(projectPath, "Datas", relativePath);

        ExcelWriter.CreateDataXlsx(xlsxPath, fields);

        // TODO（未来）：集成到 __beans__.xlsx
        return Task.CompletedTask;
    }

    /// <summary>
    /// 按照 Luban 约定将用户输入的记录类型名转为 table 容器类型名（加 Tb 前缀）。
    /// 例：itemConfig → TbItemConfig，cfg.itemConfig → cfg.TbItemConfig
    /// </summary>
    private static string BuildTableFullName(string recordFullName)
    {
        var lastDot = recordFullName.LastIndexOf('.');
        if (lastDot >= 0)
        {
            var ns = recordFullName[..lastDot];
            var name = recordFullName[(lastDot + 1)..];
            var tbName = "Tb" + char.ToUpper(name[0]) + name[1..];
            return ns + "." + tbName;
        }
        else
        {
            return "Tb" + char.ToUpper(recordFullName[0]) + recordFullName[1..];
        }
    }
}

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

        // 1. 校验字段类型（前置拦截，避免写入非法结构）
        foreach (var field in fields)
        {
            if (string.IsNullOrWhiteSpace(field.Name)) continue;
            var typeError = ContainerTypeValidator.Validate(field.Type);
            if (typeError != null)
                throw new ArgumentException(
                    $"字段 '{field.Name}' 的类型不合法：{typeError}", nameof(fields));
        }

        // 2. 创建数据 xlsx
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
    public async Task CreateEnumAsync(
        string projectPath,
        string fullName,
        bool isFlags,
        bool isUnique,
        IReadOnlyList<EnumItemDefinition> items)
    {
        ValidateFullName(fullName, "枚举");

        var enumsXlsx = Path.Combine(projectPath, "Datas", "__enums__.xlsx");
        _logger.LogInformation("开始创建枚举 {FullName}（flags={IsFlags}, unique={IsUnique}, {Count} 项）",
            fullName, isFlags, isUnique, items.Count);

        await Task.Run(() =>
        {
            if (!File.Exists(enumsXlsx))
            {
                _logger.LogInformation("__enums__.xlsx 不存在，自动创建");
                ExcelWriter.CreateEnumsMetaXlsx(enumsXlsx);
            }

            try
            {
                ExcelWriter.AppendEnumEntry(enumsXlsx, fullName, isFlags, isUnique,
                    group: string.Empty, comment: string.Empty, items);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("枚举已存在，跳过写入：{FullName}", fullName);
                throw new InvalidOperationException($"枚举 '{fullName}' 已存在，请使用不同的名称。", ex);
            }
        });

        _logger.LogInformation("枚举 {FullName} 已成功创建", fullName);
    }

    /// <inheritdoc/>
    public async Task CreateBeanAsync(
        string projectPath,
        string fullName,
        IReadOnlyList<FieldDefinition> fields)
    {
        ValidateFullName(fullName, "Bean");

        if (fields.Count == 0)
            throw new ArgumentException("Bean 至少需要一个字段。", nameof(fields));

        // 校验字段类型
        foreach (var field in fields)
        {
            if (string.IsNullOrWhiteSpace(field.Name)) continue;
            var typeError = ContainerTypeValidator.Validate(field.Type);
            if (typeError != null)
                throw new ArgumentException(
                    $"字段 '{field.Name}' 的类型不合法：{typeError}", nameof(fields));
        }

        var beansXlsx = Path.Combine(projectPath, "Datas", "__beans__.xlsx");
        _logger.LogInformation("开始创建 Bean {FullName}（{Count} 个字段）", fullName, fields.Count);

        await Task.Run(() =>
        {
            if (!File.Exists(beansXlsx))
            {
                _logger.LogInformation("__beans__.xlsx 不存在，自动创建");
                ExcelWriter.CreateBeansMetaXlsx(beansXlsx);
            }

            try
            {
                ExcelWriter.AppendBeanEntry(beansXlsx, fullName,
                    parent: string.Empty, sep: string.Empty,
                    group: string.Empty, comment: string.Empty, fields);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Bean 已存在，跳过写入：{FullName}", fullName);
                throw new InvalidOperationException($"Bean '{fullName}' 已存在，请使用不同的名称。", ex);
            }
        });

        _logger.LogInformation("Bean {FullName} 已成功创建", fullName);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<string>> GetAvailableTypeNamesAsync(string projectPath)
    {
        var confPath = Path.Combine(projectPath, "luban.conf");
        if (!File.Exists(confPath))
            return Array.Empty<string>();

        var result = new List<string>();

        try
        {
            var enums = await _schemaReader.ReadEnumsAsync(confPath);
            foreach (var e in enums) result.Add(e.FullName);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "读取枚举类型列表失败，跳过");
        }

        try
        {
            var beans = await _schemaReader.ReadBeansAsync(confPath);
            foreach (var b in beans) result.Add(b.FullName);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "读取 Bean 类型列表失败，跳过");
        }

        return ContainerTypeValidator.BuildTypeSuggestions(result);
    }

    /// <summary>
    /// 校验类型全名合法性：每段由字母开头，仅含字母/数字/下划线，段之间用 '.' 分隔。
    /// </summary>
    private static void ValidateFullName(string fullName, string typeName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException($"{typeName}全名不能为空。", nameof(fullName));

        var parts = fullName.Trim().Split('.');
        foreach (var part in parts)
        {
            if (string.IsNullOrEmpty(part))
                throw new ArgumentException(
                    $"{typeName}名称中包含连续的 '.' 或首尾有 '.'：'{fullName}'", nameof(fullName));

            if (!char.IsLetter(part[0]))
                throw new ArgumentException(
                    $"{typeName}名称每段必须以字母开头，非法名称：'{fullName}'", nameof(fullName));

            foreach (var c in part)
            {
                if (!char.IsLetterOrDigit(c) && c != '_')
                    throw new ArgumentException(
                        $"{typeName}名称包含非法字符 '{c}'：'{fullName}'", nameof(fullName));
            }
        }
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

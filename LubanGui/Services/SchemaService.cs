using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ClosedXML.Excel;
using LubanGui.Infrastructure;
using LubanGui.Models;
using Microsoft.Extensions.Logging;

namespace LubanGui.Services;

/// <summary>
/// 实现 <see cref="ISchemaService"/>，使用 ClosedXML 读写 Luban 元数据 xlsx 文件。
/// </summary>
public class SchemaService : ISchemaService
{
    private readonly ILogger<SchemaService> _logger;

    public SchemaService(ILogger<SchemaService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public Task<List<TableMeta>> LoadTablesAsync(string projectPath)
    {
        var tables = new List<TableMeta>();
        var tablesXlsx = Path.Combine(projectPath, "Datas", "__tables__.xlsx");

        if (!File.Exists(tablesXlsx))
        {
            _logger.LogWarning("__tables__.xlsx 不存在：{Path}", tablesXlsx);
            return Task.FromResult(tables);
        }

        try
        {
            using var workbook = new XLWorkbook(tablesXlsx);
            var sheet = workbook.Worksheet(1);

            // 检测格式：
            //   "##"（纯元行）→ 字段名在第 2 行，数据从第 3 行起
            //   "##var"/"##type" 等 → 第 1 行本身就是字段名行，数据从第 2 行起
            //   其他（旧格式）→ 第 1 行是字段名，数据从第 2 行起
            var a1 = sheet.Cell(1, 1).GetString().Trim();
            int headerRow, dataStartRow;

            if (string.Equals(a1, "##", StringComparison.Ordinal))
            {
                // 纯 meta 行，字段名在第 2 行
                headerRow = 2;
                dataStartRow = 3;
            }
            else
            {
                // "##var" 行本身含字段名，或旧格式无 ## 行
                headerRow = 1;
                dataStartRow = 2;
            }

            // 构建 列号 → 字段名 映射（从第2列开始，第1列是 ##var 标记）
            var colMap = BuildColumnMap(sheet, headerRow);

            // 读取数据行
            var lastRow = sheet.LastRowUsed()?.RowNumber() ?? 0;
            for (int row = dataStartRow; row <= lastRow; row++)
            {
                var aVal = sheet.Cell(row, 1).GetString().Trim();

                // 跳过 ## 开头的行（元数据行）
                if (aVal.StartsWith("##", StringComparison.Ordinal))
                {
                    continue;
                }

                // 读取 full_name（必填）
                var fullName = GetCol(sheet, row, colMap, "full_name").Trim();
                if (string.IsNullOrWhiteSpace(fullName))
                {
                    continue;
                }

                var table = new TableMeta
                {
                    FullName = fullName,
                    ValueType = GetCol(sheet, row, colMap, "value_type"),
                    Index = GetCol(sheet, row, colMap, "index"),
                    Mode = GetCol(sheet, row, colMap, "mode"),
                    Group = GetCol(sheet, row, colMap, "group"),
                    Comment = GetCol(sheet, row, colMap, "comment"),
                    ReadSchemaFromFile = GetCol(sheet, row, colMap, "read_schema_from_file")
                        .Equals("true", StringComparison.OrdinalIgnoreCase),
                    Input = GetCol(sheet, row, colMap, "input"),
                    Output = GetCol(sheet, row, colMap, "output"),
                    Tags = GetCol(sheet, row, colMap, "tags"),
                };

                tables.Add(table);
            }

            _logger.LogInformation("加载了 {Count} 张表格 from {Path}", tables.Count, tablesXlsx);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "读取 __tables__.xlsx 失败：{Path}", tablesXlsx);
            throw;
        }

        return Task.FromResult(tables);
    }

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

        _logger.LogInformation("创建表格 {FullName} → {Path}", fullName, xlsxAbsPath);

        // 1. 创建数据 xlsx
        ExcelWriter.CreateDataXlsx(xlsxAbsPath, fields);

        // 2. 构建 TableMeta
        var table = new TableMeta
        {
            FullName = fullName,
            ValueType = fullName,           // read_schema_from_file=true 时 value_type 与 full_name 相同
            Index = indexField,
            Mode = string.Empty,
            Group = string.Empty,
            Comment = string.Empty,
            ReadSchemaFromFile = true,
            Input = relativePath,
            Output = string.Empty,
            Tags = string.Empty,
        };

        // 3. 追加到 __tables__.xlsx
        var tablesXlsx = Path.Combine(projectPath, "Datas", "__tables__.xlsx");
        await Task.Run(() => ExcelWriter.AppendTableEntry(tablesXlsx, table));

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
            FullName = fullName,
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
        await Task.Run(() => ExcelWriter.AppendTableEntry(tablesXlsx, table));

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

    // ── 私有辅助 ──────────────────────────────────────────────────────────────

    /// <summary>从指定行构建 "字段名 → 列号（1-based）" 的映射。</summary>
    private static Dictionary<string, int> BuildColumnMap(IXLWorksheet sheet, int headerRow)
    {
        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var lastCol = sheet.Row(headerRow).LastCellUsed()?.Address.ColumnNumber ?? 0;

        for (int col = 1; col <= lastCol; col++)
        {
            var name = sheet.Cell(headerRow, col).GetString().Trim();
            if (!string.IsNullOrEmpty(name) && !name.StartsWith("##", StringComparison.Ordinal))
            {
                map.TryAdd(name, col);
            }
        }

        return map;
    }

    private static string GetCol(IXLWorksheet sheet, int row, Dictionary<string, int> colMap, string fieldName)
    {
        if (!colMap.TryGetValue(fieldName, out int col))
        {
            return string.Empty;
        }

        return sheet.Cell(row, col).GetString().Trim();
    }
}

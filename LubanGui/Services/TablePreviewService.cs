using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using LubanGui.LubanAdapter.Dtos;
using LubanGui.LubanAdapter.Interfaces;
using LubanGui.Models;
using Microsoft.Extensions.Logging;

namespace LubanGui.Services;

/// <summary>
/// 实现 <see cref="ITablePreviewService"/>，读取 xlsx 数据行。
/// 列名优先通过 <see cref="ILubanSchemaReader"/> 从 Bean 定义获取（需要传入 confPath）；
/// 当 confPath 未提供或 Schema 中无匹配 Bean 时，回退到解析 xlsx 的 ##var 行。
/// </summary>
public class TablePreviewService : ITablePreviewService
{
    private const int MaxPreviewRows = 500;

    private readonly ILubanSchemaReader _schemaReader;
    private readonly ILogger<TablePreviewService> _logger;

    public TablePreviewService(ILubanSchemaReader schemaReader, ILogger<TablePreviewService> logger)
    {
        _schemaReader = schemaReader;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<TablePreviewData> LoadPreviewAsync(string xlsxAbsPath, string confPath = "")
    {
        var result = new TablePreviewData { FilePath = xlsxAbsPath };

        if (!File.Exists(xlsxAbsPath))
        {
            _logger.LogWarning("预览文件不存在：{Path}", xlsxAbsPath);
            return result;
        }

        // 1. 尝试通过 Schema Reader 从 Bean 定义获取列名
        if (!string.IsNullOrEmpty(confPath))
        {
            var schemaColumns = await TryGetColumnsFromSchemaAsync(xlsxAbsPath, confPath);
            result.Columns.AddRange(schemaColumns);
        }

        // 2. 读取数据行（使用 ClosedXML），列名不足时从 ##var 行补充
        try
        {
            using var workbook = new XLWorkbook(xlsxAbsPath);
            var sheet = workbook.Worksheet(1);

            var (varRow, _) = FindMetaRows(sheet);
            int dataStartRow;

            if (result.Columns.Count == 0)
            {
                // 未能从 Schema 获取列名，回退到解析 ##var 行
                if (varRow < 0)
                {
                    _logger.LogWarning("无法在 {Path} 中找到 ##var 行", xlsxAbsPath);
                    return result;
                }

                var lastCol = sheet.Row(varRow).LastCellUsed()?.Address.ColumnNumber ?? 0;
                for (int col = 2; col <= lastCol; col++)
                {
                    var name = sheet.Cell(varRow, col).GetString().Trim();
                    if (!string.IsNullOrEmpty(name))
                    {
                        result.Columns.Add(name);
                    }
                }
            }

            dataStartRow = varRow >= 0 ? varRow + 1 : 1;

            if (result.Columns.Count == 0)
            {
                return result;
            }

            // 取实际使用的最大列号（##var 行或首数据行）
            var lastUsedCol = varRow >= 0
                ? (sheet.Row(varRow).LastCellUsed()?.Address.ColumnNumber ?? 1)
                : result.Columns.Count + 1;

            // 读取数据行（跳过 ## 开头的行和完全空行）
            var lastRow = sheet.LastRowUsed()?.RowNumber() ?? 0;
            int rowCount = 0;

            for (int row = dataStartRow; row <= lastRow && rowCount < MaxPreviewRows; row++)
            {
                var aVal = sheet.Cell(row, 1).GetString().Trim();

                if (aVal.StartsWith("##", StringComparison.Ordinal))
                {
                    continue;
                }

                bool isEmpty = true;
                for (int col = 1; col <= lastUsedCol && isEmpty; col++)
                {
                    if (!sheet.Cell(row, col).IsEmpty())
                    {
                        isEmpty = false;
                    }
                }

                if (isEmpty)
                {
                    continue;
                }

                var dataRow = new List<string>();
                for (int col = 2; col <= lastUsedCol; col++)
                {
                    int colIdx = col - 2;
                    if (colIdx >= result.Columns.Count)
                    {
                        break;
                    }

                    dataRow.Add(sheet.Cell(row, col).GetString());
                }

                while (dataRow.Count < result.Columns.Count)
                {
                    dataRow.Add(string.Empty);
                }

                result.Rows.Add(dataRow);
                rowCount++;
            }

            _logger.LogDebug("预览 {Path}：{Cols} 列，{Rows} 行", xlsxAbsPath, result.Columns.Count, result.Rows.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "读取预览失败：{Path}", xlsxAbsPath);
        }

        return result;
    }

    /// <inheritdoc/>
    public int GetFieldColumnIndex(string xlsxAbsPath, string fieldName)
    {
        if (!File.Exists(xlsxAbsPath))
        {
            return -1;
        }

        try
        {
            using var workbook = new XLWorkbook(xlsxAbsPath);
            var sheet = workbook.Worksheet(1);

            var (varRow, _) = FindMetaRows(sheet);
            if (varRow < 0)
            {
                return -1;
            }

            var lastCol = sheet.Row(varRow).LastCellUsed()?.Address.ColumnNumber ?? 0;
            for (int col = 2; col <= lastCol; col++)
            {
                var name = sheet.Cell(varRow, col).GetString().Trim();
                if (string.Equals(name, fieldName, StringComparison.OrdinalIgnoreCase))
                {
                    return col; // 1-based Excel column
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取字段列号失败：{Path} / {Field}", xlsxAbsPath, fieldName);
        }

        return -1;
    }

    // ── 私有辅助 ──────────────────────────────────────────────────────────────

    /// <summary>
    /// 通过 <see cref="ILubanSchemaReader"/> 查找与 xlsx 匹配的表格，并返回其 Bean 的字段名列表。
    /// 仅对 <c>ReadSchemaFromFile = false</c> 的表有效（字段由 __beans__.xlsx 定义）。
    /// 失败或无匹配时返回空列表，交由调用方回退到 ##var 行解析。
    /// </summary>
    private async Task<List<string>> TryGetColumnsFromSchemaAsync(string xlsxAbsPath, string confPath)
    {
        try
        {
            var tables = await _schemaReader.ReadTablesAsync(confPath);
            var confDir = Path.GetDirectoryName(confPath) ?? string.Empty;

            // 找到 InputFiles 中有路径匹配当前 xlsx 的表
            TableSchemaDto? matchedTable = null;
            foreach (var table in tables)
            {
                if (table.ReadSchemaFromFile)
                {
                    // Schema 来自 xlsx 本身，直接解析 ##var 行更准确
                    continue;
                }

                foreach (var inputFile in table.InputFiles)
                {
                    if (IsInputFileMatch(xlsxAbsPath, confDir, inputFile))
                    {
                        matchedTable = table;
                        break;
                    }
                }

                if (matchedTable != null)
                {
                    break;
                }
            }

            if (matchedTable == null)
            {
                return [];
            }

            // 从 Bean 定义中获取字段名
            var beans = await _schemaReader.ReadBeansAsync(confPath);
            var bean = beans.FirstOrDefault(b =>
                string.Equals(b.FullName, matchedTable.ValueType, StringComparison.OrdinalIgnoreCase));

            if (bean == null || bean.Fields.Count == 0)
            {
                return [];
            }

            _logger.LogDebug("通过 Schema 获取 {Table} 列名（{Count} 个字段）", matchedTable.FullName, bean.Fields.Count);
            return bean.Fields.Select(f => f.Name).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Schema reader 获取列名失败，将回退到 ##var 行解析");
            return [];
        }
    }

    /// <summary>
    /// 判断 <paramref name="inputFile"/>（相对路径）是否对应 <paramref name="xlsxAbsPath"/>。
    /// 尝试两种解析基准：confDir 和 confDir/Datas。
    /// </summary>
    private static bool IsInputFileMatch(string xlsxAbsPath, string confDir, string inputFile)
    {
        var candidate1 = Path.GetFullPath(Path.Combine(confDir, inputFile));
        var candidate2 = Path.GetFullPath(Path.Combine(confDir, "Datas", inputFile));
        return string.Equals(candidate1, xlsxAbsPath, StringComparison.OrdinalIgnoreCase)
            || string.Equals(candidate2, xlsxAbsPath, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 查找 xlsx 中的 ##var 行（标题行）和 ##type 行的行号（1-based）。
    /// 若找不到返回 (-1, -1)。
    /// </summary>
    private static (int varRow, int typeRow) FindMetaRows(IXLWorksheet sheet)
    {
        int varRow = -1;
        int typeRow = -1;

        var lastRow = sheet.LastRowUsed()?.RowNumber() ?? 0;

        for (int row = 1; row <= Math.Min(lastRow, 10); row++)
        {
            var a = sheet.Cell(row, 1).GetString().Trim().ToLowerInvariant();

            if (varRow < 0 && (a == "##var" || a == "##+"))
            {
                varRow = row;
            }
            else if (varRow < 0 && a == "##" && row == 1)
            {
                // 兼容历史格式：第一行 A1 = "##"，可能第一行本身就是 var 行（B 列起有字段名）
                var b1 = sheet.Cell(1, 2).GetString().Trim();
                if (!string.IsNullOrEmpty(b1))
                {
                    varRow = 1;
                }
            }
            else if (typeRow < 0 && a == "##type")
            {
                typeRow = row;
            }

            if (varRow > 0 && !a.StartsWith("##", StringComparison.Ordinal))
            {
                break;
            }
        }

        return (varRow, typeRow);
    }
}

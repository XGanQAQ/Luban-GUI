using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ClosedXML.Excel;
using LubanGui.Models;
using Microsoft.Extensions.Logging;

namespace LubanGui.Services;

/// <summary>
/// 实现 <see cref="ITablePreviewService"/>，使用 ClosedXML 读取 xlsx 数据行。
/// </summary>
public class TablePreviewService : ITablePreviewService
{
    private const int MaxPreviewRows = 500;

    private readonly ILogger<TablePreviewService> _logger;

    public TablePreviewService(ILogger<TablePreviewService> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public Task<TablePreviewData> LoadPreviewAsync(string xlsxAbsPath)
    {
        var result = new TablePreviewData { FilePath = xlsxAbsPath };

        if (!File.Exists(xlsxAbsPath))
        {
            _logger.LogWarning("预览文件不存在：{Path}", xlsxAbsPath);
            return Task.FromResult(result);
        }

        try
        {
            using var workbook = new XLWorkbook(xlsxAbsPath);
            var sheet = workbook.Worksheet(1);

            var (varRow, _) = FindMetaRows(sheet);
            if (varRow < 0)
            {
                _logger.LogWarning("无法在 {Path} 中找到 ##var 行", xlsxAbsPath);
                return Task.FromResult(result);
            }

            // 读取列名（跳过 A 列的 ##var 标记）
            var lastCol = sheet.Row(varRow).LastCellUsed()?.Address.ColumnNumber ?? 0;
            for (int col = 2; col <= lastCol; col++)
            {
                var name = sheet.Cell(varRow, col).GetString().Trim();
                if (!string.IsNullOrEmpty(name))
                {
                    result.Columns.Add(name);
                }
            }

            if (result.Columns.Count == 0)
            {
                return Task.FromResult(result);
            }

            // 读取数据行（跳过所有 ## 开头的行）
            var lastRow = sheet.LastRowUsed()?.RowNumber() ?? 0;
            int rowCount = 0;

            for (int row = varRow + 1; row <= lastRow && rowCount < MaxPreviewRows; row++)
            {
                var aVal = sheet.Cell(row, 1).GetString().Trim();

                // 跳过元数据行（##type, ##group, ##comment 等）
                if (aVal.StartsWith("##", StringComparison.Ordinal))
                {
                    continue;
                }

                // 检查该行是否完全为空
                bool isEmpty = true;
                for (int col = 1; col <= lastCol && isEmpty; col++)
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
                for (int col = 2; col <= lastCol; col++)
                {
                    // 只读取有对应列名的列
                    int colIdx = col - 2;
                    if (colIdx >= result.Columns.Count)
                    {
                        break;
                    }

                    dataRow.Add(sheet.Cell(row, col).GetString());
                }

                // 补足短行
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

        return Task.FromResult(result);
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
                // 尝试检测 B1 是否有非空字段名
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

            // 如果找到 var 行且当前行不是 ## 开头，就停止搜索
            if (varRow > 0 && !a.StartsWith("##", StringComparison.Ordinal))
            {
                break;
            }
        }

        return (varRow, typeRow);
    }
}

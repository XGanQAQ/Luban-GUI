using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using LubanGui.Models;

namespace LubanGui.Infrastructure;

/// <summary>
/// 负责生成和修改符合 Luban 规范的 xlsx 文件。
/// Luban 要求：第一行 A1 = "##var"（var row，同时作为 meta 行），
/// 数据行从后续行开始（A 列留空，数据从 B 列起）。
/// </summary>
public static class ExcelWriter
{
    // ── __tables__.xlsx 字段（与 Luban ExcelSchemaLoader 对应） ─────────────

    private static readonly string[] s_tableFields =
    {
        "full_name", "value_type", "index", "mode",
        "group", "comment", "read_schema_from_file", "input", "output", "tags",
    };

    // ── __enums__.xlsx：简化格式（MVP - 仅标题，不含 list 子字段） ──────────

    private static readonly string[] s_enumFields =
    {
        "full_name", "comment", "flags", "group", "tags", "unique",
    };

    // ── __beans__.xlsx：简化格式（MVP - 仅标题，不含 list 子字段） ──────────

    private static readonly string[] s_beanFields =
    {
        "full_name", "parent", "valueType", "sep", "alias", "comment", "tags", "group",
    };

    // ── 公共 API ──────────────────────────────────────────────────────────────

    /// <summary>创建空的 __tables__.xlsx（含 ## 行和 ##var 标题行）。</summary>
    public static void CreateTablesMetaXlsx(string path)
        => CreateMetaXlsx(path, s_tableFields);

    /// <summary>创建空的 __enums__.xlsx。</summary>
    public static void CreateEnumsMetaXlsx(string path)
        => CreateMetaXlsx(path, s_enumFields);

    /// <summary>创建空的 __beans__.xlsx。</summary>
    public static void CreateBeansMetaXlsx(string path)
        => CreateMetaXlsx(path, s_beanFields);

    /// <summary>
    /// 向已有的 __tables__.xlsx 中追加一条表格记录。
    /// </summary>
    public static void AppendTableEntry(string path, TableMeta table)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"__tables__.xlsx 不存在：{path}");
        }

        using var workbook = new XLWorkbook(path);
        var sheet = workbook.Worksheet(1);

        int nextRow = FindNextDataRow(sheet);

        // A 列留空；从 B 列写 full_name, C 列写 value_type …
        sheet.Cell(nextRow, 2).Value = table.FullName;
        sheet.Cell(nextRow, 3).Value = table.ValueType;
        sheet.Cell(nextRow, 4).Value = table.Index;
        sheet.Cell(nextRow, 5).Value = table.Mode;
        sheet.Cell(nextRow, 6).Value = table.Group;
        sheet.Cell(nextRow, 7).Value = table.Comment;
        sheet.Cell(nextRow, 8).Value = table.ReadSchemaFromFile ? "true" : "false";
        sheet.Cell(nextRow, 9).Value = table.Input;
        sheet.Cell(nextRow, 10).Value = table.Output;
        sheet.Cell(nextRow, 11).Value = table.Tags;

        workbook.Save();
    }

    /// <summary>
    /// 创建一个符合 Luban 规范的 xlsx（含 ## 行、##var 行和 ##type 行）。
    /// </summary>
    /// <param name="path">文件路径（含文件名）。</param>
    /// <param name="fields">字段定义列表（Name + Type）。</param>
    public static void CreateDataXlsx(string path, IReadOnlyList<FieldDefinition> fields)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
        {
            Directory.CreateDirectory(dir);
        }

        using var workbook = new XLWorkbook();
        var sheet = workbook.AddWorksheet("Sheet1");

        // Row 1：var row（同时作为 meta 行）- A1 = "##var"，B1+ = 字段名
        // 规范要求 ##var 必须是第一行，否则整个 Sheet 被 Luban 忽略
        sheet.Cell(1, 1).Value = "##var";
        for (int i = 0; i < fields.Count; i++)
        {
            sheet.Cell(1, i + 2).Value = fields[i].Name;
        }

        // Row 2：type row - A2 = "##type"，B2+ = 字段类型
        sheet.Cell(2, 1).Value = "##type";
        for (int i = 0; i < fields.Count; i++)
        {
            sheet.Cell(2, i + 2).Value = fields[i].Type;
        }

        // Row 3（可选）：comment row - A3 = "##"，B3+ = 字段注释（人类可读，不参与解析）
        bool hasComments = false;
        foreach (var f in fields)
        {
            if (!string.IsNullOrWhiteSpace(f.Comment))
            {
                hasComments = true;
                break;
            }
        }

        if (hasComments)
        {
            sheet.Cell(3, 1).Value = "##";
            for (int i = 0; i < fields.Count; i++)
            {
                sheet.Cell(3, i + 2).Value = fields[i].Comment;
            }
        }

        sheet.Columns().AdjustToContents();
        workbook.SaveAs(path);
    }

    // ── 私有辅助 ──────────────────────────────────────────────────────────────

    /// <summary>创建带 Luban 标准 ## 行和 ##var 标题行的空元数据 xlsx。</summary>
    private static void CreateMetaXlsx(string path, string[] fieldNames)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.AddWorksheet("Sheet1");

        // Row 1：var row（同时作为 meta 行）- A1 = "##var"，B1+ = 字段名
        // 规范要求 ##var 必须是第一行，否则整个 Sheet 被 Luban 忽略
        sheet.Cell(1, 1).Value = "##var";
        for (int i = 0; i < fieldNames.Length; i++)
        {
            sheet.Cell(1, i + 2).Value = fieldNames[i];
        }

        sheet.Columns().AdjustToContents();
        workbook.SaveAs(path);
    }

    /// <summary>返回下一个可写入数据的行号（跳过 ## 行和 ##var 行，以及已有数据行）。</summary>
    private static int FindNextDataRow(IXLWorksheet sheet)
    {
        int row = 1;
        while (true)
        {
            var cell = sheet.Cell(row, 1);
            if (cell.IsEmpty() && sheet.Cell(row, 2).IsEmpty())
            {
                // 找到第一个完全为空的行（且 A 列也为空），说明数据已结束
                return row;
            }

            string a = cell.GetString().Trim();
            if (!a.StartsWith("##") && !string.IsNullOrEmpty(a))
            {
                // A 列有非 ## 数据，说明是已有数据行，继续往下
                row++;
                continue;
            }

            // A 列是空或 ## 开头
            if (a.StartsWith("##"))
            {
                row++;
                continue;
            }

            // A 列为空，B 列有数据（已有数据行）
            if (!sheet.Cell(row, 2).IsEmpty())
            {
                row++;
                continue;
            }

            // A 列为空，B 列也为空：找到空行
            return row;
        }
    }
}

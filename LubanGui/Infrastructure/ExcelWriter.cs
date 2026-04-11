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
    // ── __tables__.xlsx 字段（列顺序与官方模板一致，B 列起） ─────────────────
    // B=full_name  C=value_type  D=read_schema_from_file  E=input
    // F=index  G=mode  H=group  I=comment  J=tags  K=output

    private static readonly string[] s_tableFields =
    {
        "full_name", "value_type", "read_schema_from_file", "input",
        "index", "mode", "group", "comment", "tags", "output",
    };

    // ── __enums__.xlsx 顶层字段（B~G 列，H~L 列为合并 *items 子字段） ────────
    // B=full_name  C=flags  D=unique  E=group  F=comment  G=tags

    private static readonly string[] s_enumTopFields =
    {
        "full_name", "flags", "unique", "group", "comment", "tags",
    };

    // *items 子字段（H~L，5 列）
    private static readonly string[] s_enumItemSubFields =
    {
        "name", "alias", "value", "comment", "tags",
    };

    // ── __beans__.xlsx 顶层字段（B~I 列，J~P 列为合并 *fields 子字段） ───────
    // B=full_name  C=parent  D=valueType  E=sep  F=alias  G=comment  H=group  I=tags

    private static readonly string[] s_beanTopFields =
    {
        "full_name", "parent", "valueType", "sep", "alias", "comment", "group", "tags",
    };

    // *fields 子字段（J~P，7 列）
    private static readonly string[] s_beanFieldSubFields =
    {
        "name", "alias", "type", "group", "comment", "tags", "variants",
    };

    // ── 公共 API ──────────────────────────────────────────────────────────────

    /// <summary>创建空的 __tables__.xlsx（含 ##var 标题行和两行 ## 注释行）。</summary>
    public static void CreateTablesMetaXlsx(string path)
        => CreateSimpleMetaXlsx(path, s_tableFields);

    /// <summary>
    /// 创建空的 __enums__.xlsx。
    /// 结构：双行 ##var（第二行定义 *items 子字段），H1:L1 合并单元格。
    /// </summary>
    public static void CreateEnumsMetaXlsx(string path)
        => CreateEnumsXlsx(path);

    /// <summary>
    /// 创建空的 __beans__.xlsx。
    /// 结构：双行 ##var（第二行定义 *fields 子字段），J1:P1 合并单元格。
    /// </summary>
    public static void CreateBeansMetaXlsx(string path)
        => CreateBeansXlsx(path);

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

        // A 列留空；从 B 列起按模板列顺序写入
        // B=full_name  C=value_type  D=read_schema_from_file  E=input
        // F=index  G=mode  H=group  I=comment  J=tags  K=output
        sheet.Cell(nextRow, 2).Value = table.FullName;
        sheet.Cell(nextRow, 3).Value = table.ValueType;
        sheet.Cell(nextRow, 4).Value = table.ReadSchemaFromFile ? "true" : "false";
        sheet.Cell(nextRow, 5).Value = table.Input;
        sheet.Cell(nextRow, 6).Value = table.Index;
        sheet.Cell(nextRow, 7).Value = table.Mode;
        sheet.Cell(nextRow, 8).Value = table.Group;
        sheet.Cell(nextRow, 9).Value = table.Comment;
        sheet.Cell(nextRow, 10).Value = table.Tags;
        sheet.Cell(nextRow, 11).Value = table.Output;

        workbook.Save();
    }

    /// <summary>
    /// 创建一个符合 Luban 规范的 xlsx（含 ## 行、##var 行和 ##type 行）。
    /// </summary>
    /// <param name="path">文件路径（含文件名）。</param>
    /// <param name="fields">字段定义列表（Name + Type）。</param>
    public static void CreateDataXlsx(string path, IReadOnlyList<FieldDefinition> fields)
    {
        EnsureDirectory(path);

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

    /// <summary>
    /// 创建单行 ##var 标题的简单元数据 xlsx（用于 __tables__.xlsx）。
    /// </summary>
    private static void CreateSimpleMetaXlsx(string path, string[] fieldNames)
    {
        EnsureDirectory(path);
        using var workbook = new XLWorkbook();
        var sheet = workbook.AddWorksheet("Sheet1");

        // Row 1：##var + 字段名（第一行即为 meta 行，Luban 规范要求）
        sheet.Cell(1, 1).Value = "##var";
        for (int i = 0; i < fieldNames.Length; i++)
        {
            sheet.Cell(1, i + 2).Value = fieldNames[i];
        }

        sheet.Columns().AdjustToContents();
        workbook.SaveAs(path);
    }

    /// <summary>
    /// 创建 __enums__.xlsx，包含双行 ##var 和 H1:L1 合并的 *items 容器字段。
    /// </summary>
    private static void CreateEnumsXlsx(string path)
    {
        EnsureDirectory(path);
        using var workbook = new XLWorkbook();
        var sheet = workbook.AddWorksheet("Sheet1");

        // 顶层字段数量（B~G）
        int topCount = s_enumTopFields.Length;          // 6
        int subCount = s_enumItemSubFields.Length;       // 5
        int subStartCol = topCount + 2;                  // H = col 8

        // Row 1：##var + 顶层字段名 + *items（合并 subStartCol ~ subStartCol+subCount-1）
        sheet.Cell(1, 1).Value = "##var";
        for (int i = 0; i < topCount; i++)
        {
            sheet.Cell(1, i + 2).Value = s_enumTopFields[i];
        }
        sheet.Cell(1, subStartCol).Value = "*items";
        sheet.Range(1, subStartCol, 1, subStartCol + subCount - 1).Merge();

        // Row 2：##var + 子字段名（顶层列留空）
        sheet.Cell(2, 1).Value = "##var";
        for (int i = 0; i < subCount; i++)
        {
            sheet.Cell(2, subStartCol + i).Value = s_enumItemSubFields[i];
        }

        sheet.Columns().AdjustToContents();
        workbook.SaveAs(path);
    }

    /// <summary>
    /// 创建 __beans__.xlsx，包含双行 ##var 和 J1:P1 合并的 *fields 容器字段。
    /// </summary>
    private static void CreateBeansXlsx(string path)
    {
        EnsureDirectory(path);
        using var workbook = new XLWorkbook();
        var sheet = workbook.AddWorksheet("Sheet1");

        int topCount = s_beanTopFields.Length;           // 8
        int subCount = s_beanFieldSubFields.Length;      // 7
        int subStartCol = topCount + 2;                  // J = col 10

        // Row 1：##var + 顶层字段名 + *fields（合并）
        sheet.Cell(1, 1).Value = "##var";
        for (int i = 0; i < topCount; i++)
        {
            sheet.Cell(1, i + 2).Value = s_beanTopFields[i];
        }
        sheet.Cell(1, subStartCol).Value = "*fields";
        sheet.Range(1, subStartCol, 1, subStartCol + subCount - 1).Merge();

        // Row 2：##var + 子字段名（顶层列留空）
        sheet.Cell(2, 1).Value = "##var";
        for (int i = 0; i < subCount; i++)
        {
            sheet.Cell(2, subStartCol + i).Value = s_beanFieldSubFields[i];
        }

        sheet.Columns().AdjustToContents();
        workbook.SaveAs(path);
    }

    private static void EnsureDirectory(string path)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
        {
            Directory.CreateDirectory(dir);
        }
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

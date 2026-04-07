using System.Collections.Generic;

namespace LubanGui.Models;

/// <summary>
/// 表格预览数据：从 xlsx 中读取的列名和数据行。
/// </summary>
public class TablePreviewData
{
    /// <summary>源 xlsx 文件的绝对路径。</summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>列名列表（字段名），顺序与 Rows 中的元素对应。</summary>
    public List<string> Columns { get; set; } = new();

    /// <summary>数据行，每行是一个字符串列表，与 Columns 顺序对应。</summary>
    public List<List<string>> Rows { get; set; } = new();
}

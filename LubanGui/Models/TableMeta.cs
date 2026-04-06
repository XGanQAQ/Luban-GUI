namespace LubanGui.Models;

/// <summary>
/// 从 __tables__.xlsx 中读取的单张表格元数据。
/// </summary>
public class TableMeta
{
    /// <summary>表格全限定名，如 "item.ItemConfig"。</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// 对应 Excel 文件的相对路径（相对于项目 Datas/ 目录），如 "item/ItemConfig.xlsx"。
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>值类型，如 "map:int,item.ItemConfig"。</summary>
    public string ValueType { get; set; } = string.Empty;

    /// <summary>索引字段名，如 "id"。</summary>
    public string Index { get; set; } = string.Empty;

    /// <summary>所属分组，如 "c,s"。</summary>
    public string Group { get; set; } = string.Empty;

    /// <summary>简短的表名（FullName 最后一段），用于 UI 展示。</summary>
    public string DisplayName => FullName.Contains('.') ? FullName[(FullName.LastIndexOf('.') + 1)..] : FullName;

    /// <summary>上次导表是否成功（用于状态图标显示）。</summary>
    public bool? LastExportSuccess { get; set; }
}

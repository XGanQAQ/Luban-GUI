namespace LubanGui.Models;

/// <summary>
/// 「数据类型列表」窗口中的单条类型信息。
/// </summary>
public sealed class DataTypeListItem
{
    /// <summary>类型分类：内置类型 / 枚举 / Bean。</summary>
    public required string Category { get; init; }

    /// <summary>类型名称（如 int、cfg.EItemType、cfg.Item）。</summary>
    public required string Name { get; init; }

    /// <summary>补充说明（如字段数、flags、继承关系等）。</summary>
    public string Description { get; init; } = string.Empty;
}
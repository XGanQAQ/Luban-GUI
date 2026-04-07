namespace LubanGui.Models;

/// <summary>
/// 表格字段定义（供 GUI 对话框使用）。
/// </summary>
public class FieldDefinition
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "string";
    public string Comment { get; set; } = string.Empty;
}

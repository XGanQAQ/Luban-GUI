namespace LubanGui.Models;

/// <summary>
/// 「删除表格」确认对话框的返回结果。
/// </summary>
public class DeleteTableConfirmResult
{
    /// <summary>true = 同时删除物理 xlsx 数据文件；false = 仅移除 __tables__.xlsx 注册条目。</summary>
    public bool DeletePhysicalFile { get; init; }

    /// <summary>true = 将当前选择保存为下次默认策略。</summary>
    public bool SaveAsDefault { get; init; }
}

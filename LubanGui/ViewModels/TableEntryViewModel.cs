using CommunityToolkit.Mvvm.ComponentModel;

namespace LubanGui.ViewModels;

public enum TableExportStatus
{
    Unknown,
    Success,
    Failed,
}

/// <summary>
/// 表示一个表格条目的 ViewModel，包含表格名称、导出状态等信息。
/// </summary>
public partial class TableEntryViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private TableExportStatus _exportStatus = TableExportStatus.Unknown;

    public string StatusIcon => ExportStatus switch
    {
        TableExportStatus.Success => "✓",
        TableExportStatus.Failed  => "✗",
        _                         => "—",
    };

    partial void OnExportStatusChanged(TableExportStatus value) =>
        OnPropertyChanged(nameof(StatusIcon));
}

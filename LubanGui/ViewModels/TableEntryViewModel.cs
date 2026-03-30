using CommunityToolkit.Mvvm.ComponentModel;

namespace LubanGui.ViewModels;

public enum TableExportStatus
{
    Unknown,
    Success,
    Failed,
}

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

using CommunityToolkit.Mvvm.ComponentModel;
using LubanGui.Models;

namespace LubanGui.ViewModels;

public partial class DeleteTableConfirmDialogViewModel : ViewModelBase
{
    /// <summary>表格的显示名称（full_name）。</summary>
    [ObservableProperty]
    private string _tableName = string.Empty;

    /// <summary>是否同时删除物理 xlsx 数据文件（true）还是仅移除注册（false）。</summary>
    [ObservableProperty]
    private bool _deletePhysicalFile;

    /// <summary>是否将当前选择保存为默认策略。</summary>
    [ObservableProperty]
    private bool _saveAsDefault;

    public DeleteTableConfirmResult BuildResult() => new()
    {
        DeletePhysicalFile = DeletePhysicalFile,
        SaveAsDefault = SaveAsDefault,
    };
}

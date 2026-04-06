using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LubanGui.ViewModels;

/// <summary>对话框的返回结果。</summary>
public record NewProjectResult(string Name, string WorkspacePath);

/// <summary>「新建项目」对话框的 ViewModel。</summary>
public partial class NewProjectDialogViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanCreate))]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    private string _projectName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanCreate))]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    private string _workspacePath = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public bool CanCreate =>
        !string.IsNullOrWhiteSpace(ProjectName) &&
        !string.IsNullOrWhiteSpace(WorkspacePath);

    /// <summary>关闭请求事件；参数为 NewProjectResult（确认）或 null（取消）。</summary>
    public event EventHandler<NewProjectResult?>? CloseRequested;

    /// <summary>打开文件夹选择对话框（由 View 传入 TopLevel）。</summary>
    public Func<Task<string?>>? BrowseHandler { get; set; }

    [RelayCommand]
    private async Task Browse()
    {
        if (BrowseHandler != null)
        {
            var path = await BrowseHandler();
            if (!string.IsNullOrEmpty(path))
            {
                WorkspacePath = path;
            }
        }
    }

    [RelayCommand(CanExecute = nameof(CanCreate))]
    private void Create()
    {
        ErrorMessage = string.Empty;

        var name = ProjectName.Trim();

        // 简单验证：名称不含非法字符
        if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            ErrorMessage = "项目名称包含非法字符";
            return;
        }

        var targetDir = Path.Combine(WorkspacePath, name);
        if (Directory.Exists(targetDir))
        {
            ErrorMessage = $"目录已存在：{targetDir}";
            return;
        }

        CloseRequested?.Invoke(this, new NewProjectResult(name, WorkspacePath));
    }

    [RelayCommand]
    private void Cancel() => CloseRequested?.Invoke(this, null);
}

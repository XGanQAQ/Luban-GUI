using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using LubanGui.Models;
using LubanGui.Services;
using LubanGui.ViewModels;
using LubanGui.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;

namespace LubanGui.Views;

public partial class MainWindow : Window
{
    private LogWindow? _logWindow;

    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is MainWindowViewModel vm)
        {
            vm.OpenLogWindowRequested += OnOpenLogWindowRequested;
            vm.OpenExportSettingsRequested += OnOpenExportSettingsRequested;
            vm.OpenAboutRequested += OnOpenAboutRequested;
            vm.NewProjectRequested += OnNewProjectRequested;
            vm.OpenProjectRequested += OnOpenProjectRequested;
        }
    }

    private void OnOpenLogWindowRequested(object? sender, EventArgs e)
    {
        if (_logWindow == null || !_logWindow.IsVisible)
        {
            _logWindow = new LogWindow { DataContext = DataContext };
            _logWindow.Closed += (_, _) => _logWindow = null;
            _logWindow.Show(this);
        }
        else
        {
            _logWindow.Activate();
        }
    }

    private async void OnOpenExportSettingsRequested(object? sender, EventArgs e)
    {
        var dialog = new ExportSettingsWindow { DataContext = DataContext };
        await dialog.ShowDialog(this);
    }

    private async void OnOpenAboutRequested(object? sender, EventArgs e)
    {
        var dialog = new AboutWindow { DataContext = DataContext };
        await dialog.ShowDialog(this);
    }

    private async void OnNewProjectRequested(object? sender, EventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm)
        {
            return;
        }

        var dialogVm = new NewProjectDialogViewModel();
        dialogVm.BrowseHandler = async () => await PickFolderAsync();

        var dialog = new NewProjectDialog(dialogVm);
        var result = await dialog.ShowDialog<NewProjectResult?>(this);

        if (result == null)
        {
            return;
        }

        var projectManager = GetProjectManager();
        if (projectManager == null)
        {
            return;
        }

        try
        {
            await projectManager.CreateProjectAsync(result.Name, result.WorkspacePath);
            vm.SyncProjectsFromManager();
        }
        catch (Exception ex)
        {
            vm.AddLog(LogEntryLevel.Error, $"创建项目失败：{ex.Message}");
        }
    }

    private async void OnOpenProjectRequested(object? sender, EventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm)
        {
            return;
        }

        var path = await PickFolderAsync();
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        var projectManager = GetProjectManager();
        if (projectManager == null)
        {
            return;
        }

        try
        {
            await projectManager.OpenProjectAsync(path);
            vm.SyncProjectsFromManager();
        }
        catch (Exception ex)
        {
            vm.AddLog(LogEntryLevel.Error, $"打开项目失败：{ex.Message}");
        }
    }

    private async Task<string?> PickFolderAsync()
    {
        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "选择文件夹",
            AllowMultiple = false,
        });

        return folders.Count > 0 ? folders[0].TryGetLocalPath() : null;
    }

    private IProjectManager? GetProjectManager() =>
        (App.Current as App)?._serviceProvider?.GetService<IProjectManager>();
}

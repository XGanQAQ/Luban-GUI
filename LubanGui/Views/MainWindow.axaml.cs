using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
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
            vm.NewTableRequested += OnNewTableRequested;
            vm.NewEnumRequested += OnNewEnumRequested;
            vm.NewBeanRequested += OnNewBeanRequested;
            vm.ImportFileRequested += OnImportFileRequested;
        }

        // 绑定 DataGrid 列标题双击事件（在 PreviewGrid 的列头上双击打开文件）
        var grid = this.FindControl<DataGrid>("PreviewGrid");
        if (grid != null)
        {
            grid.DoubleTapped += OnPreviewGridDoubleTapped;
        }
    }

    private void OnPreviewGridDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm)
        {
            return;
        }

        // 找到被双击的列头
        if (e.Source is not Control source)
        {
            return;
        }

        // 查找祖先中的 DataGridColumnHeader
        var element = source;
        while (element != null)
        {
            if (element.GetType().Name == "DataGridColumnHeader")
            {
                // 取列头文本（DataGrid AutoGenerateColumns 时列头就是列名）
                var headerContent = element.GetValue(ContentControl.ContentProperty);
                if (headerContent is string fieldName)
                {
                    vm.OpenFileAtField(fieldName);
                }

                break;
            }

            element = element.Parent as Control;
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

        var projectManager = GetService<IProjectManager>();
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

        var projectManager = GetService<IProjectManager>();
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

    private async void OnNewTableRequested(object? sender, EventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm || vm.CurrentProject == null)
        {
            return;
        }

        var schemaService = GetService<ISchemaService>();
        if (schemaService == null)
        {
            return;
        }

        var dialogVm = new NewTableDialogViewModel();
        var dialog = new NewTableDialog(dialogVm);
        var result = await dialog.ShowDialog<NewTableResult?>(this);

        if (result == null)
        {
            return;
        }

        try
        {
            var meta = await schemaService.CreateTableAsync(
                vm.CurrentProject.ProjectPath,
                result.FullName,
                result.IndexField,
                result.Fields);

            vm.AddLog(LogEntryLevel.Success, $"已创建表格：{meta.FullName} → Datas/{meta.Input}");
            await vm.RefreshTablesCommand.ExecuteAsync(null);
        }
        catch (Exception ex)
        {
            vm.AddLog(LogEntryLevel.Error, $"创建表格失败：{ex.Message}");
        }
    }

    private async void OnNewEnumRequested(object? sender, EventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm || vm.CurrentProject == null)
        {
            return;
        }

        var schemaService = GetService<ISchemaService>();
        if (schemaService == null)
        {
            return;
        }

        var dialogVm = new NewEnumDialogViewModel();
        var dialog = new NewEnumDialog(dialogVm);
        var result = await dialog.ShowDialog<NewEnumResult?>(this);

        if (result == null)
        {
            return;
        }

        try
        {
            await schemaService.CreateEnumAsync(vm.CurrentProject.ProjectPath, result.FullName, result.Items);
            vm.AddLog(LogEntryLevel.Success, $"已创建枚举：{result.FullName}");
        }
        catch (Exception ex)
        {
            vm.AddLog(LogEntryLevel.Error, $"创建枚举失败：{ex.Message}");
        }
    }

    private async void OnNewBeanRequested(object? sender, EventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm || vm.CurrentProject == null)
        {
            return;
        }

        var schemaService = GetService<ISchemaService>();
        if (schemaService == null)
        {
            return;
        }

        var dialogVm = new NewBeanDialogViewModel();
        var dialog = new NewBeanDialog(dialogVm);
        var result = await dialog.ShowDialog<NewBeanResult?>(this);

        if (result == null)
        {
            return;
        }

        try
        {
            await schemaService.CreateBeanAsync(vm.CurrentProject.ProjectPath, result.FullName, result.Fields);
            vm.AddLog(LogEntryLevel.Success, $"已创建 Bean：{result.FullName}");
        }
        catch (Exception ex)
        {
            vm.AddLog(LogEntryLevel.Error, $"创建 Bean 失败：{ex.Message}");
        }
    }

    private async void OnImportFileRequested(object? sender, EventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm || vm.CurrentProject == null)
        {
            return;
        }

        var schemaService = GetService<ISchemaService>();
        if (schemaService == null)
        {
            return;
        }

        // 选择 xlsx 文件
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "选择要导入的 xlsx 文件",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Excel 文件") { Patterns = new[] { "*.xlsx" } },
            },
        });

        if (files.Count == 0)
        {
            return;
        }

        var xlsxPath = files[0].TryGetLocalPath();
        if (string.IsNullOrEmpty(xlsxPath))
        {
            return;
        }

        // 询问全名
        var tableName = Path.GetFileNameWithoutExtension(xlsxPath);
        var fullName = $"cfg.{tableName}";   // 默认全名

        try
        {
            var meta = await schemaService.ImportTableAsync(
                vm.CurrentProject.ProjectPath,
                xlsxPath,
                fullName,
                "id");

            vm.AddLog(LogEntryLevel.Success, $"已导入表格：{meta.FullName}");
            await vm.RefreshTablesCommand.ExecuteAsync(null);
        }
        catch (Exception ex)
        {
            vm.AddLog(LogEntryLevel.Error, $"导入文件失败：{ex.Message}");
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

    private T? GetService<T>() where T : class =>
        (App.Current as App)?._serviceProvider?.GetService<T>();
}

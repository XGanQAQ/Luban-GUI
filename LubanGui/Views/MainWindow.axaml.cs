using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
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
    private DataTypeListWindow? _dataTypeListWindow;

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
            vm.OpenDataTypeListRequested += OnOpenDataTypeListRequested;
            vm.NewProjectRequested += OnNewProjectRequested;
            vm.OpenProjectRequested += OnOpenProjectRequested;
            vm.NewTableRequested += OnNewTableRequested;
            vm.NewEnumRequested += OnNewEnumRequested;
            vm.NewBeanRequested += OnNewBeanRequested;
            vm.ImportFileRequested += OnImportFileRequested;
            vm.PropertyChanged += OnViewModelPropertyChanged;
        }

        // 双击表格条目打开对应 xlsx 文件
        var listBox = this.FindControl<ListBox>("TableListBox");
        if (listBox != null)
        {
            listBox.DoubleTapped += OnTableListBoxDoubleTapped;
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainWindowViewModel.PreviewColumnNames))
        {
            RebuildPreviewColumns();
        }
    }

    private void OnTableListBoxDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm) return;
        vm.OpenTableFileCommand.Execute(vm.SelectedTable);
    }

    /// <summary>
    /// 根据 ViewModel 的 PreviewColumnNames 重建 DataGrid 列。
    /// 每列使用 DataGridTemplateColumn，单元格携带"定位到 Excel 单元格"右键菜单。
    /// </summary>
    private void RebuildPreviewColumns()
    {
        var grid = this.FindControl<DataGrid>("PreviewGrid");
        if (grid == null) return;

        grid.Columns.Clear();

        if (DataContext is not MainWindowViewModel vm || vm.PreviewColumnNames == null) return;

        var columns = vm.PreviewColumnNames;
        for (int i = 0; i < columns.Count; i++)
        {
            int colIndex = i; // capture for closure
            grid.Columns.Add(new DataGridTemplateColumn
            {
                Header = columns[i],
                IsReadOnly = true,
                CellTemplate = new FuncDataTemplate<IList<string>>((rowData, _) =>
                {
                    var tb = new TextBlock
                    {
                        Text = rowData != null && colIndex < rowData.Count ? rowData[colIndex] : string.Empty,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Padding = new Thickness(4, 0),
                    };

                    // 用 Panel 填满整个单元格区域，确保任意位置右键都能命中 ContextMenu
                    var panel = new Panel
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        MinHeight = 22,
                    };
                    panel.Children.Add(tb);

                    var menuItem = new MenuItem { Header = "定位到 Excel 单元格" };
                    menuItem.Click += (_, _) =>
                    {
                        if (DataContext is not MainWindowViewModel vmCtx) return;
                        var rowIdx = vmCtx.PreviewRows.IndexOf(rowData!);
                        if (rowIdx >= 0)
                            vmCtx.OpenCellInExcel(rowIdx, colIndex);
                    };

                    panel.ContextMenu = new ContextMenu { Items = { menuItem } };
                    return panel;
                }),
            });
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
        var exportVm = (DataContext as MainWindowViewModel)?.ExportConfig;
        var dialog = new ExportSettingsWindow { DataContext = exportVm };
        await dialog.ShowDialog(this);
    }

    private async void OnOpenAboutRequested(object? sender, EventArgs e)
    {
        var dialog = new AboutWindow { DataContext = DataContext };
        await dialog.ShowDialog(this);
    }

    private void OnOpenDataTypeListRequested(object? sender, EventArgs e)
    {
        if (_dataTypeListWindow == null || !_dataTypeListWindow.IsVisible)
        {
            _dataTypeListWindow = new DataTypeListWindow { DataContext = DataContext };
            _dataTypeListWindow.Closed += (_, _) => _dataTypeListWindow = null;
            _dataTypeListWindow.Show(this);
        }
        else
        {
            _dataTypeListWindow.Activate();
        }
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

        // 注入类型建议（内置基础类型 + 容器模板 + 自定义类型），供字段类型自动补全使用
        try
        {
            var suggestions = await schemaService.GetAvailableTypeNamesAsync(vm.CurrentProject.ProjectPath);
            dialogVm.SetAvailableTypes(suggestions);
        }
        catch { /* 获取失败时不影响对话框打开 */ }

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
            await schemaService.CreateEnumAsync(
                vm.CurrentProject.ProjectPath,
                result.FullName,
                result.IsFlags,
                result.IsUnique,
                result.Items);
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

        // 注入当前已知的自定义类型，供字段类型自动补全使用
        try
        {
            var availableTypes = await schemaService.GetAvailableTypeNamesAsync(vm.CurrentProject.ProjectPath);
            dialogVm.SetAvailableTypes(availableTypes);
        }
        catch { /* 获取失败时不影响对话框打开 */ }

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

using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using LubanGui.ViewModels;

namespace LubanGui.Views;

public partial class ExportSettingsWindow : Window
{
    public ExportSettingsWindow()
    {
        InitializeComponent();

        var closeButton = this.FindControl<Button>("CloseButton");
        if (closeButton != null)
        {
            closeButton.Click += CloseButton_Click;
        }
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is MainWindowViewModel vm)
        {
            vm.BrowseLubanPathRequested += OnBrowseLubanPathRequested;
            vm.BrowseConfFileRequested  += OnBrowseConfFileRequested;
        }
    }

    private async void OnBrowseLubanPathRequested(object? sender, EventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "选择 Luban 可执行文件",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("可执行文件") { Patterns = new[] { "*.exe", "*.dll" } },
                new FilePickerFileType("所有文件")   { Patterns = new[] { "*" } },
            },
        });

        if (files.Count > 0 && DataContext is MainWindowViewModel vm)
        {
            var path = files[0].TryGetLocalPath();
            if (!string.IsNullOrEmpty(path))
                vm.LubanPath = path;
        }
    }

    private async void OnBrowseConfFileRequested(object? sender, EventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "选择 luban.conf 配置文件",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("配置文件") { Patterns = new[] { "*.conf", "*.json" } },
                new FilePickerFileType("所有文件") { Patterns = new[] { "*" } },
            },
        });

        if (files.Count > 0 && DataContext is MainWindowViewModel vm)
        {
            var path = files[0].TryGetLocalPath();
            if (!string.IsNullOrEmpty(path))
                vm.ConfFile = path;
        }
    }

    private async void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            await vm.SaveExportConfigAsync();
        }
        Close();
    }
}

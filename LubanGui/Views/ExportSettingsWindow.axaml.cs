using System;
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

        var cancelButton = this.FindControl<Button>("CancelButton");
        if (cancelButton != null)
            cancelButton.Click += CancelButton_Click;

        var saveButton = this.FindControl<Button>("SaveButton");
        if (saveButton != null)
            saveButton.Click += SaveButton_Click;
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is ExportConfigViewModel vm)
        {
            vm.BrowseDataOutputPathRequested += OnBrowseDataOutputPathRequested;
            vm.BrowseCodeOutputPathRequested += OnBrowseCodeOutputPathRequested;
        }
    }

    private async void OnBrowseDataOutputPathRequested(object? sender, EventArgs e)
    {
        var folder = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "选择数据导出目录",
            AllowMultiple = false,
        });

        if (folder.Count > 0 && DataContext is ExportConfigViewModel vm)
        {
            var path = folder[0].TryGetLocalPath();
            if (!string.IsNullOrEmpty(path))
                vm.DataOutputPath = path;
        }
    }

    private async void OnBrowseCodeOutputPathRequested(object? sender, EventArgs e)
    {
        var folder = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "选择代码导出目录",
            AllowMultiple = false,
        });

        if (folder.Count > 0 && DataContext is ExportConfigViewModel vm)
        {
            var path = folder[0].TryGetLocalPath();
            if (!string.IsNullOrEmpty(path))
                vm.CodeOutputPath = path;
        }
    }

    private void CancelButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ExportConfigViewModel vm)
            await vm.SaveAsync();
        Close();
    }
}

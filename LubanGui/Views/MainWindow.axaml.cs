using System;
using Avalonia.Controls;
using LubanGui.ViewModels;

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
}
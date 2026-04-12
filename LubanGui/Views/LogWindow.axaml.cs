using System;
using System.Collections.Specialized;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using LubanGui.Models;
using LubanGui.ViewModels;

namespace LubanGui.Views;

public partial class LogWindow : Window
{
    public LogWindow()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is MainWindowViewModel vm)
        {
            vm.LogEntries.CollectionChanged += LogEntries_CollectionChanged;
        }
    }

    private void LogEntries_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action != NotifyCollectionChangedAction.Add)
        {
            return;
        }

        var listBox = this.FindControl<ListBox>("LogListBox");
        if (listBox?.ItemCount > 0)
        {
            listBox.ScrollIntoView(listBox.ItemCount - 1);
        }
    }

    private async void LogListBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key != Key.C || (e.KeyModifiers & KeyModifiers.Control) == 0)
            return;

        var listBox = sender as ListBox;
        if (listBox?.SelectedItems is null || listBox.SelectedItems.Count == 0)
            return;

        var text = string.Join(Environment.NewLine,
            listBox.SelectedItems.OfType<LogEntry>().Select(entry => entry.FormattedMessage));

        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is not null)
            await clipboard.SetTextAsync(text);

        e.Handled = true;
    }
}

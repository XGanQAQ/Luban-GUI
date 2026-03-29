using System;
using System.Collections.Specialized;
using Avalonia.Controls;
using LubanGui.ViewModels;

namespace LubanGui.Views;

public partial class MainWindow : Window
{
    public MainWindow()
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
}
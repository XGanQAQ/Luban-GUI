using Avalonia.Controls;
using Avalonia.Interactivity;
using LubanGui.Models;
using LubanGui.ViewModels;

namespace LubanGui.Views;

public partial class DataTypeListWindow : Window
{
    public DataTypeListWindow()
    {
        InitializeComponent();
    }

    public void SetViewModel(DataTypeListViewModel viewModel)
    {
        DataContext = viewModel;
    }

    private void OnModifyEnumClick(object? sender, RoutedEventArgs e)
    {
        if (sender is MenuItem { DataContext: DataTypeListItem { Category: "枚举" } item }
            && DataContext is DataTypeListViewModel vm)
        {
            vm.ModifyEnumCommand.Execute(item);
        }
    }
}

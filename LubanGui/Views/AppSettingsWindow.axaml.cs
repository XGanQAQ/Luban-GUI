using Avalonia.Controls;
using LubanGui.ViewModels;

namespace LubanGui.Views;

public partial class AppSettingsWindow : Window
{
    public AppSettingsWindow()
    {
        InitializeComponent();
    }

    public AppSettingsWindow(AppSettingsViewModel viewModel) : this()
    {
        DataContext = viewModel;
        viewModel.CloseRequested += (_, result) => Close(result);
    }
}

using Avalonia.Controls;
using LubanGui.ViewModels;

namespace LubanGui.Views.Dialogs;

public partial class NewProjectDialog : Window
{
    public NewProjectDialog()
    {
        InitializeComponent();
    }

    public NewProjectDialog(NewProjectDialogViewModel viewModel) : this()
    {
        DataContext = viewModel;
        viewModel.CloseRequested += (_, result) => Close(result);
    }
}

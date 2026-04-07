using Avalonia.Controls;
using LubanGui.ViewModels;

namespace LubanGui.Views.Dialogs;

public partial class NewBeanDialog : Window
{
    public NewBeanDialog() : this(new NewBeanDialogViewModel()) { }

    public NewBeanDialog(NewBeanDialogViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        this.FindControl<Button>("CreateButton")!.Click += (_, _) =>
        {
            if (DataContext is NewBeanDialogViewModel vm)
            {
                Close(vm.BuildResult());
            }
        };

        this.FindControl<Button>("CancelButton")!.Click += (_, _) => Close(null);
    }
}

using Avalonia.Controls;
using Avalonia.Interactivity;
using LubanGui.ViewModels;

namespace LubanGui.Views.Dialogs;

public partial class NewTableDialog : Window
{
    public NewTableDialog() : this(new NewTableDialogViewModel()) { }

    public NewTableDialog(NewTableDialogViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        var createBtn = this.FindControl<Button>("CreateButton")!;
        var cancelBtn = this.FindControl<Button>("CancelButton")!;

        createBtn.Click += OnCreate;
        cancelBtn.Click += OnCancel;
    }

    private void OnCreate(object? sender, RoutedEventArgs e)
    {
        if (DataContext is NewTableDialogViewModel vm)
        {
            Close(vm.BuildResult());
        }
    }

    private void OnCancel(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }
}

using Avalonia.Controls;
using Avalonia.Interactivity;
using LubanGui.ViewModels;

namespace LubanGui.Views.Dialogs;

public partial class ModifyEnumDialog : Window
{
    public ModifyEnumDialog()
    {
        InitializeComponent();
    }

    public ModifyEnumDialog(ModifyEnumDialogViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        this.FindControl<Button>("SaveButton")!.Click += OnSave;
        this.FindControl<Button>("CancelButton")!.Click += OnCancel;
    }

    private void OnSave(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ModifyEnumDialogViewModel vm)
        {
            Close(vm.BuildResult());
        }
    }

    private void OnCancel(object? sender, RoutedEventArgs e) => Close(null);
}

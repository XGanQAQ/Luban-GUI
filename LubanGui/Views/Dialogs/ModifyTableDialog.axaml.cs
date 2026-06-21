using Avalonia.Controls;
using Avalonia.Interactivity;
using LubanGui.ViewModels;

namespace LubanGui.Views.Dialogs;

public partial class ModifyTableDialog : Window
{
    public ModifyTableDialog() : this(new ModifyTableDialogViewModel()) { }

    public ModifyTableDialog(ModifyTableDialogViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        var saveBtn = this.FindControl<Button>("SaveButton")!;
        var cancelBtn = this.FindControl<Button>("CancelButton")!;

        saveBtn.Click += OnSave;
        cancelBtn.Click += OnCancel;
    }

    private void OnSave(object? sender, RoutedEventArgs e)
    {
        if (DataContext is ModifyTableDialogViewModel vm)
        {
            Close(vm.BuildResult());
        }
    }

    private void OnCancel(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }
}

using Avalonia.Controls;
using Avalonia.Interactivity;
using LubanGui.Models;
using LubanGui.ViewModels;

namespace LubanGui.Views.Dialogs;

public partial class DeleteTableConfirmDialog : Window
{
    public DeleteTableConfirmDialog() : this(new DeleteTableConfirmDialogViewModel()) { }

    public DeleteTableConfirmDialog(DeleteTableConfirmDialogViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        var confirmBtn = this.FindControl<Button>("ConfirmButton")!;
        var cancelBtn = this.FindControl<Button>("CancelButton")!;

        confirmBtn.Click += OnConfirm;
        cancelBtn.Click += OnCancel;
    }

    private void OnConfirm(object? sender, RoutedEventArgs e)
    {
        if (DataContext is DeleteTableConfirmDialogViewModel vm)
        {
            Close(vm.BuildResult());
        }
    }

    private void OnCancel(object? sender, RoutedEventArgs e)
    {
        Close(null);
    }
}

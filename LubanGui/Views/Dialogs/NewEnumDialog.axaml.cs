using Avalonia.Controls;
using Avalonia.Interactivity;
using LubanGui.ViewModels;

namespace LubanGui.Views.Dialogs;

public partial class NewEnumDialog : Window
{
    public NewEnumDialog() : this(new NewEnumDialogViewModel()) { }

    public NewEnumDialog(NewEnumDialogViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        this.FindControl<Button>("CreateButton")!.Click += OnCreate;
        this.FindControl<Button>("CancelButton")!.Click += OnCancel;
    }

    private void OnCreate(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is NewEnumDialogViewModel vm)
        {
            Close(vm.BuildResult());
        }
    }

    private void OnCancel(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Close(null);
}

using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LubanGui.Views;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();

        var okButton = this.FindControl<Button>("OkButton");
        if (okButton != null)
        {
            okButton.Click += (_, _) => Close();
        }
    }
}

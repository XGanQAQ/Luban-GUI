using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LubanGui.Models;

namespace LubanGui.ViewModels;

/// <summary>
/// 对话框中可编辑的字段定义行，带移除命令。
/// </summary>
public partial class FieldDefinitionViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _type = "string";

    [ObservableProperty]
    private string _comment = string.Empty;

    public IRelayCommand RemoveCommand { get; }

    public FieldDefinitionViewModel(Action<FieldDefinitionViewModel> removeAction)
    {
        RemoveCommand = new RelayCommand(() => removeAction(this));
    }

    public FieldDefinition ToModel() => new()
    {
        Name = Name,
        Type = Type,
        Comment = Comment,
    };
}

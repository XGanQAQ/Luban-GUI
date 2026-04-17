using System;
using System.Collections.Generic;
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

    /// <summary>
    /// 可选的类型候选列表，供前端 AutoCompleteBox 提供补全建议。
    /// 由父级 ViewModel 在创建时注入（若未注入则为空列表，仍可手动输入）。
    /// </summary>
    public IReadOnlyList<string> AvailableTypes { get; set; } = Array.Empty<string>();

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

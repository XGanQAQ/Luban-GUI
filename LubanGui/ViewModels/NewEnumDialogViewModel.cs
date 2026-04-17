using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LubanGui.Models;

namespace LubanGui.ViewModels;

/// <summary>枚举值可编辑行，带移除命令。</summary>
public partial class EnumItemViewModel : ObservableObject
{
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string _alias = string.Empty;
    [ObservableProperty] private string _value = string.Empty;
    [ObservableProperty] private string _comment = string.Empty;

    public IRelayCommand RemoveCommand { get; }

    public EnumItemViewModel(Action<EnumItemViewModel> removeAction)
    {
        RemoveCommand = new RelayCommand(() => removeAction(this));
    }

    public EnumItemDefinition ToModel() => new()
    {
        Name    = Name,
        Alias   = Alias,
        Value   = Value,
        Comment = Comment,
    };
}

/// <summary>结果对象，由对话框关闭时返回给调用方。</summary>
public record NewEnumResult(
    string FullName,
    bool IsFlags,
    bool IsUnique,
    IReadOnlyList<EnumItemDefinition> Items);

/// <summary>「新建枚举」对话框的 ViewModel。</summary>
public partial class NewEnumDialogViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ErrorMessage))]
    [NotifyPropertyChangedFor(nameof(CanCreate))]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    private string _enumFullName = string.Empty;

    [ObservableProperty] private bool _isFlags = false;
    [ObservableProperty] private bool _isUnique = true;

    public ObservableCollection<EnumItemViewModel> Items { get; } = new();

    /// <summary>当前校验错误信息；空字符串表示无错误。</summary>
    public string ErrorMessage => ValidateInputs();

    /// <summary>所有输入合法且至少有一枚举项时为 true。</summary>
    public bool CanCreate => string.IsNullOrEmpty(ErrorMessage) && Items.Count > 0;

    [RelayCommand]
    private void AddItem()
    {
        Items.Add(new EnumItemViewModel(i =>
        {
            Items.Remove(i);
            NotifyValidation();
        }));
        NotifyValidation();
    }

    private bool CanExecuteCreate() => CanCreate;

    [RelayCommand(CanExecute = nameof(CanExecuteCreate))]
    private void Create() { }

    [RelayCommand]
    private void Cancel() { }

    public NewEnumResult BuildResult() => new(
        EnumFullName.Trim(),
        IsFlags,
        IsUnique,
        Items.Select(i => i.ToModel()).Where(i => !string.IsNullOrWhiteSpace(i.Name)).ToList());

    // ──────────────────────────────────────────────────────────
    // 内部辅助
    // ──────────────────────────────────────────────────────────

    private void NotifyValidation()
    {
        OnPropertyChanged(nameof(ErrorMessage));
        OnPropertyChanged(nameof(CanCreate));
        CreateCommand.NotifyCanExecuteChanged();
    }

    private string ValidateInputs()
    {
        var name = EnumFullName?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(name))
            return "枚举全名不能为空";

        var parts = name.Split('.');
        foreach (var part in parts)
        {
            if (string.IsNullOrEmpty(part))
                return "名称中包含连续的 '.' 或首尾有 '.'";
            if (!char.IsLetter(part[0]))
                return "名称每段必须以字母开头";
            foreach (var c in part)
                if (!char.IsLetterOrDigit(c) && c != '_')
                    return $"名称包含非法字符 '{c}'";
        }

        // 检查重复枚举项名
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in Items)
        {
            if (!string.IsNullOrWhiteSpace(item.Name) && !seen.Add(item.Name))
                return $"枚举项名称重复：'{item.Name}'";
        }

        return string.Empty;
    }
}

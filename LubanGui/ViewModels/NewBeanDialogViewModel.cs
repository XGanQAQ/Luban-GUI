using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LubanGui.Models;

namespace LubanGui.ViewModels;

/// <summary>结果对象，由对话框关闭时返回给调用方。</summary>
public record NewBeanResult(
    string FullName,
    IReadOnlyList<FieldDefinition> Fields);

/// <summary>「新建 Bean」对话框的 ViewModel。</summary>
public partial class NewBeanDialogViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ErrorMessage))]
    [NotifyPropertyChangedFor(nameof(CanCreate))]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    private string _beanFullName = string.Empty;

    public ObservableCollection<FieldDefinitionViewModel> Fields { get; } = new();

    /// <summary>
    /// 当前校验错误信息；空字符串表示无错误。
    /// </summary>
    public string ErrorMessage => ValidateInputs();

    /// <summary>所有输入合法且至少有一字段时为 true。</summary>
    public bool CanCreate => string.IsNullOrEmpty(ErrorMessage) && Fields.Count > 0;

    /// <summary>
    /// 可用类型列表（内置类型 + 当前项目的枚举/Bean）。
    /// 由 MainWindow 在打开对话框前通过 <see cref="SetAvailableTypes"/> 注入。
    /// </summary>
    public IReadOnlyList<string> AvailableTypes { get; private set; } = Array.Empty<string>();

    /// <summary>注入可用类型列表，并更新已有字段行的候选列表。</summary>
    public void SetAvailableTypes(IReadOnlyList<string> types)
    {
        AvailableTypes = types;
        foreach (var f in Fields)
            f.AvailableTypes = types;
    }

    [RelayCommand]
    private void AddField()
    {
        var vm = new FieldDefinitionViewModel(f => { Fields.Remove(f); NotifyValidation(); })
        {
            AvailableTypes = AvailableTypes,
        };
        Fields.Add(vm);
        NotifyValidation();
    }

    private bool CanExecuteCreate() => CanCreate;

    [RelayCommand(CanExecute = nameof(CanExecuteCreate))]
    private void Create() { }

    [RelayCommand]
    private void Cancel() { }

    public NewBeanResult BuildResult() => new(
        BeanFullName.Trim(),
        Fields.Select(f => f.ToModel()).Where(f => !string.IsNullOrWhiteSpace(f.Name)).ToList());

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
        var name = BeanFullName?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(name))
            return "Bean 全名不能为空";

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

        // 检查字段名是否重复
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var field in Fields)
        {
            if (!string.IsNullOrWhiteSpace(field.Name) && !seen.Add(field.Name))
                return $"字段名重复：'{field.Name}'";
        }

        return string.Empty;
    }
}

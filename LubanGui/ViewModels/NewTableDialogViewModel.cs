using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LubanGui.Models;
using LubanGui.Services;

namespace LubanGui.ViewModels;

/// <summary>结果对象，由对话框关闭时返回给调用方。</summary>
public record NewTableResult(
    string FullName,
    string IndexField,
    IReadOnlyList<FieldDefinition> Fields);

/// <summary>「新建表格」对话框的 ViewModel。</summary>
public partial class NewTableDialogViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ErrorMessage))]
    [NotifyPropertyChangedFor(nameof(CanCreate))]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    private string _tableFullName = string.Empty;

    [ObservableProperty]
    private string _indexField = "id";

    public ObservableCollection<FieldDefinitionViewModel> Fields { get; } = new();

    /// <summary>当前校验错误信息；空字符串表示无错误。</summary>
    public string ErrorMessage => ValidateInputs();

    /// <summary>所有输入合法且至少有一字段时为 true。</summary>
    public bool CanCreate => string.IsNullOrEmpty(ErrorMessage) && Fields.Count > 0;

    /// <summary>
    /// 可用类型建议列表（内置基础类型 + 容器模板 + 当前项目自定义类型）。
    /// 由 MainWindow 在打开对话框前通过 <see cref="SetAvailableTypes"/> 注入。
    /// </summary>
    public IReadOnlyList<string> AvailableTypes { get; private set; } = Array.Empty<string>();

    /// <summary>注入类型建议列表，并更新已有字段行的候选列表。</summary>
    public void SetAvailableTypes(IReadOnlyList<string> types)
    {
        AvailableTypes = types;
        foreach (var f in Fields)
        {
            f.AvailableTypes = types;
            f.ValidationRequested = NotifyValidation;
        }
    }

    [RelayCommand]
    private void AddField()
    {
        var vm = new FieldDefinitionViewModel(f => { Fields.Remove(f); NotifyValidation(); })
        {
            AvailableTypes = AvailableTypes,
            ValidationRequested = NotifyValidation,
        };
        Fields.Add(vm);
        NotifyValidation();
    }

    private bool CanExecuteCreate() => CanCreate;

    [RelayCommand(CanExecute = nameof(CanExecuteCreate))]
    private void Create() { /* handled by View */ }

    [RelayCommand]
    private void Cancel() { /* handled by View */ }

    public NewTableResult BuildResult() => new(
        TableFullName.Trim(),
        IndexField.Trim(),
        Fields.Select(f => f.ToModel()).Where(f => !string.IsNullOrWhiteSpace(f.Name)).ToList());

    public NewTableDialogViewModel()
    {
        AddDefaultFields();
    }

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
        var name = TableFullName?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(name))
            return "表格全名不能为空";

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

        // 校验字段类型
        foreach (var field in Fields)
        {
            if (string.IsNullOrWhiteSpace(field.Name)) continue;
            var typeError = ContainerTypeValidator.Validate(field.Type);
            if (typeError != null)
                return $"字段 '{field.Name}' 的类型不合法：{typeError}";
        }

        return string.Empty;
    }

    private void AddDefaultFields()
    {
        AddNamedField("id",   "int",    "唯一 ID");
        AddNamedField("name", "string", "名称");
    }

    private void AddNamedField(string name, string type, string comment)
    {
        var vm = new FieldDefinitionViewModel(f => { Fields.Remove(f); NotifyValidation(); })
        {
            Name    = name,
            Type    = type,
            Comment = comment,
            AvailableTypes    = AvailableTypes,
            ValidationRequested = NotifyValidation,
        };
        Fields.Add(vm);
    }
}

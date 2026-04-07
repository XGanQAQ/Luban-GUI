using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LubanGui.Models;

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
    [NotifyPropertyChangedFor(nameof(CanCreate))]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    private string _tableFullName = string.Empty;

    [ObservableProperty]
    private string _indexField = "id";

    public ObservableCollection<FieldDefinitionViewModel> Fields { get; } = new();

    public bool CanCreate =>
        !string.IsNullOrWhiteSpace(TableFullName) && Fields.Count > 0;

    [RelayCommand]
    private void AddField()
    {
        Fields.Add(new FieldDefinitionViewModel(f => { Fields.Remove(f); OnPropertyChanged(nameof(CanCreate)); }));
        OnPropertyChanged(nameof(CanCreate));
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
        // 默认添加 id + name 两个字段
        AddDefaultFields();
    }

    private void AddDefaultFields()
    {
        var idField = new FieldDefinitionViewModel(f => { Fields.Remove(f); OnPropertyChanged(nameof(CanCreate)); })
        {
            Name = "id",
            Type = "int",
            Comment = "唯一 ID",
        };
        Fields.Add(idField);

        var nameField = new FieldDefinitionViewModel(f => { Fields.Remove(f); OnPropertyChanged(nameof(CanCreate)); })
        {
            Name = "name",
            Type = "string",
            Comment = "名称",
        };
        Fields.Add(nameField);
    }
}

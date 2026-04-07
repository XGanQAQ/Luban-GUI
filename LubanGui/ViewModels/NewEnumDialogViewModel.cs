using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LubanGui.Models;

namespace LubanGui.ViewModels;

/// <summary>枚举值可编辑行，带移除命令。</summary>
public partial class EnumItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _value = string.Empty;

    [ObservableProperty]
    private string _comment = string.Empty;

    public IRelayCommand RemoveCommand { get; }

    public EnumItemViewModel(Action<EnumItemViewModel> removeAction)
    {
        RemoveCommand = new RelayCommand(() => removeAction(this));
    }

    public EnumItemDefinition ToModel() => new()
    {
        Name = Name,
        Value = Value,
        Comment = Comment,
    };
}

/// <summary>结果对象，由对话框关闭时返回给调用方。</summary>
public record NewEnumResult(
    string FullName,
    System.Collections.Generic.IReadOnlyList<EnumItemDefinition> Items);

/// <summary>「新建枚举」对话框的 ViewModel。</summary>
public partial class NewEnumDialogViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanCreate))]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    private string _enumFullName = string.Empty;

    public ObservableCollection<EnumItemViewModel> Items { get; } = new();

    public bool CanCreate =>
        !string.IsNullOrWhiteSpace(EnumFullName) && Items.Count > 0;

    [RelayCommand]
    private void AddItem()
    {
        Items.Add(new EnumItemViewModel(i => { Items.Remove(i); OnPropertyChanged(nameof(CanCreate)); }));
        OnPropertyChanged(nameof(CanCreate));
    }

    private bool CanExecuteCreate() => CanCreate;

    [RelayCommand(CanExecute = nameof(CanExecuteCreate))]
    private void Create() { }

    [RelayCommand]
    private void Cancel() { }

    public NewEnumResult BuildResult() => new(
        EnumFullName.Trim(),
        Items.Select(i => i.ToModel()).Where(i => !string.IsNullOrWhiteSpace(i.Name)).ToList());
}

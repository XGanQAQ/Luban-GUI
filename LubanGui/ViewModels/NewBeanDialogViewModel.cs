using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LubanGui.Models;

namespace LubanGui.ViewModels;

/// <summary>结果对象，由对话框关闭时返回给调用方。</summary>
public record NewBeanResult(
    string FullName,
    System.Collections.Generic.IReadOnlyList<FieldDefinition> Fields);

/// <summary>「新建 Bean」对话框的 ViewModel。</summary>
public partial class NewBeanDialogViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanCreate))]
    [NotifyCanExecuteChangedFor(nameof(CreateCommand))]
    private string _beanFullName = string.Empty;

    public ObservableCollection<FieldDefinitionViewModel> Fields { get; } = new();

    public bool CanCreate =>
        !string.IsNullOrWhiteSpace(BeanFullName) && Fields.Count > 0;

    [RelayCommand]
    private void AddField()
    {
        Fields.Add(new FieldDefinitionViewModel(f => { Fields.Remove(f); OnPropertyChanged(nameof(CanCreate)); }));
        OnPropertyChanged(nameof(CanCreate));
    }

    private bool CanExecuteCreate() => CanCreate;

    [RelayCommand(CanExecute = nameof(CanExecuteCreate))]
    private void Create() { }

    [RelayCommand]
    private void Cancel() { }

    public NewBeanResult BuildResult() => new(
        BeanFullName.Trim(),
        Fields.Select(f => f.ToModel()).Where(f => !string.IsNullOrWhiteSpace(f.Name)).ToList());
}

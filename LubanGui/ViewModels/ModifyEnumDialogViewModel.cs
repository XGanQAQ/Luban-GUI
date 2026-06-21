using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LubanGui.Models;

namespace LubanGui.ViewModels;

public record ModifyEnumResult(
    bool IsFlags,
    bool IsUnique,
    IReadOnlyList<EnumItemDefinition> Items
);

public partial class ModifyEnumDialogViewModel : ObservableObject
{
    [ObservableProperty]
    private string _enumFullName = string.Empty;

    [ObservableProperty]
    private bool _isFlags;

    [ObservableProperty]
    private bool _isUnique;

    public ObservableCollection<EnumItemViewModel> Items { get; } = new();

    public string ErrorMessage => ValidateInputs();

    public bool CanSave => string.IsNullOrEmpty(ErrorMessage) && Items.Count > 0;

    public void LoadFrom(EnumInfoDto dto)
    {
        EnumFullName = dto.FullName;
        IsFlags = dto.IsFlags;
        IsUnique = dto.IsUnique;

        Items.Clear();
        foreach (var item in dto.Items)
        {
            var vm = new EnumItemViewModel(i =>
            {
                Items.Remove(i);
                NotifyValidation();
            });
            vm.Name = item.Name;
            vm.Alias = item.Alias;
            vm.Value = item.Value;
            vm.Comment = item.Comment;
            Items.Add(vm);
        }
    }

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

    [RelayCommand]
    private void Save() { }

    [RelayCommand]
    private void Cancel() { }

    public ModifyEnumResult BuildResult() => new(
        IsFlags,
        IsUnique,
        Items.Select(i => i.ToModel()).Where(i => !string.IsNullOrWhiteSpace(i.Name)).ToList());

    private void NotifyValidation()
    {
        OnPropertyChanged(nameof(ErrorMessage));
        OnPropertyChanged(nameof(CanSave));
        SaveCommand.NotifyCanExecuteChanged();
    }

    private string ValidateInputs()
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in Items)
        {
            if (!string.IsNullOrWhiteSpace(item.Name) && !seen.Add(item.Name))
                return $"枚举项名称重复：'{item.Name}'";
        }

        return string.Empty;
    }
}

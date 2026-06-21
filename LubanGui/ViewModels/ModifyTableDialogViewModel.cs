using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LubanGui.Models;
using LubanGui.Services;

namespace LubanGui.ViewModels;

public record ModifyTableResult(
    IReadOnlyList<FieldDefinition> Fields);

public partial class ModifyTableDialogViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ErrorMessage))]
    [NotifyPropertyChangedFor(nameof(CanSave))]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    private string _tableFullName = string.Empty;

    public ObservableCollection<FieldDefinitionViewModel> Fields { get; } = new();

    public string ErrorMessage => ValidateInputs();

    public bool CanSave => string.IsNullOrEmpty(ErrorMessage) && Fields.Count > 0;

    public IReadOnlyList<string> AvailableTypes { get; private set; } = Array.Empty<string>();

    public void SetAvailableTypes(IReadOnlyList<string> types)
    {
        AvailableTypes = types;
        foreach (var f in Fields)
        {
            f.AvailableTypes = types;
            f.ValidationRequested = NotifyValidation;
        }
    }

    public void LoadFromExisting(string fullName, IReadOnlyList<FieldDefinition> existingFields)
    {
        TableFullName = fullName;
        Fields.Clear();
        foreach (var f in existingFields)
        {
            var vm = new FieldDefinitionViewModel(
                fd => { Fields.Remove(fd); NotifyValidation(); },
                MoveFieldUp,
                MoveFieldDown)
            {
                Name = f.Name,
                Type = f.Type,
                Comment = f.Comment,
                AvailableTypes = AvailableTypes,
                ValidationRequested = NotifyValidation,
            };
            Fields.Add(vm);
        }
        NotifyValidation();
    }

    [RelayCommand]
    private void AddField()
    {
        var vm = new FieldDefinitionViewModel(
            f => { Fields.Remove(f); NotifyValidation(); },
            MoveFieldUp,
            MoveFieldDown)
        {
            AvailableTypes = AvailableTypes,
            ValidationRequested = NotifyValidation,
        };
        Fields.Add(vm);
        NotifyValidation();
    }

    private void MoveFieldUp(FieldDefinitionViewModel vm)
    {
        var idx = Fields.IndexOf(vm);
        if (idx > 0)
        {
            Fields.Move(idx, idx - 1);
            NotifyValidation();
        }
    }

    private void MoveFieldDown(FieldDefinitionViewModel vm)
    {
        var idx = Fields.IndexOf(vm);
        if (idx < Fields.Count - 1)
        {
            Fields.Move(idx, idx + 1);
            NotifyValidation();
        }
    }

    private bool CanExecuteSave() => CanSave;

    [RelayCommand(CanExecute = nameof(CanExecuteSave))]
    private void Save() { }

    [RelayCommand]
    private void Cancel() { }

    public ModifyTableResult BuildResult() => new(
        Fields.Select(f => f.ToModel()).Where(f => !string.IsNullOrWhiteSpace(f.Name)).ToList());

    public ModifyTableDialogViewModel() { }

    private void NotifyValidation()
    {
        OnPropertyChanged(nameof(ErrorMessage));
        OnPropertyChanged(nameof(CanSave));
        SaveCommand.NotifyCanExecuteChanged();
    }

    private string ValidateInputs()
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var field in Fields)
        {
            if (!string.IsNullOrWhiteSpace(field.Name) && !seen.Add(field.Name))
                return $"字段名重复：'{field.Name}'";
        }

        foreach (var field in Fields)
        {
            if (string.IsNullOrWhiteSpace(field.Name)) continue;
            var typeError = ContainerTypeValidator.Validate(field.Type, AvailableTypes);
            if (typeError != null)
                return $"字段 '{field.Name}' 的类型不合法：{typeError}";
        }

        return string.Empty;
    }
}

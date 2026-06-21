using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LubanGui.Models;
using LubanGui.Services;

namespace LubanGui.ViewModels;

public partial class DataTypeListViewModel : ObservableObject
{
    private readonly ISchemaService? _schemaService;

    public DataTypeListViewModel() { }

    public DataTypeListViewModel(ISchemaService? schemaService)
    {
        _schemaService = schemaService;
    }

    public ObservableCollection<DataTypeListItem> DataTypes { get; } = new();

    public ObservableCollection<DataTypeListItem> FilteredDataTypes { get; } = new();

    [ObservableProperty]
    private bool _isDataTypesLoading;

    [ObservableProperty]
    private int _builtinTypeCount;

    [ObservableProperty]
    private int _enumTypeCount;

    [ObservableProperty]
    private int _beanTypeCount;

    public int TotalTypeCount => BuiltinTypeCount + EnumTypeCount + BeanTypeCount;

    [ObservableProperty]
    private bool _showBuiltinTypes;

    partial void OnShowBuiltinTypesChanged(bool value) => ApplyFilter();

    [ObservableProperty]
    private string _typeFilter = string.Empty;

    partial void OnTypeFilterChanged(string value) => ApplyFilter();

    public event EventHandler<string>? ModifyEnumRequested;
    public event EventHandler? RefreshRequested;

    public void Clear()
    {
        DataTypes.Clear();
        FilteredDataTypes.Clear();
        BuiltinTypeCount = 0;
        EnumTypeCount = 0;
        BeanTypeCount = 0;
        OnPropertyChanged(nameof(TotalTypeCount));
    }

    public void LoadFrom(IReadOnlyList<DataTypeListItem> items)
    {
        DataTypes.Clear();
        foreach (var item in items)
        {
            DataTypes.Add(item);
        }

        BuiltinTypeCount = items.Count(i => i.Category == "内置");
        EnumTypeCount = items.Count(i => i.Category == "枚举");
        BeanTypeCount = items.Count(i => i.Category == "Bean");
        OnPropertyChanged(nameof(TotalTypeCount));

        ApplyFilter();
    }

    public void ApplyFilter()
    {
        FilteredDataTypes.Clear();
        var filter = TypeFilter?.Trim() ?? string.Empty;

        foreach (var item in DataTypes)
        {
            if (!ShowBuiltinTypes && item.Category == "内置")
                continue;

            if (!string.IsNullOrEmpty(filter)
                && !item.Name.Contains(filter, StringComparison.OrdinalIgnoreCase)
                && !item.Category.Contains(filter, StringComparison.OrdinalIgnoreCase))
                continue;

            FilteredDataTypes.Add(item);
        }
    }

    [RelayCommand]
    private void ModifyEnum(DataTypeListItem? item)
    {
        if (item?.Category != "枚举") return;
        ModifyEnumRequested?.Invoke(this, item.Name);
    }

    [RelayCommand]
    private void Refresh()
    {
        RefreshRequested?.Invoke(this, EventArgs.Empty);
    }
}

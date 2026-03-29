using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LubanGui.ViewModels;

/// <summary>
/// 可编辑字符串列表项的 ViewModel，用于 codeTarget、dataTarget、xargs 等可重复参数的 UI 绑定。
/// </summary>
public partial class StringItemViewModel : ObservableObject
{
    private readonly Action<StringItemViewModel> _removeCallback;

    [ObservableProperty]
    private string _value = string.Empty;

    public StringItemViewModel(string value, Action<StringItemViewModel> removeCallback)
    {
        _value = value;
        _removeCallback = removeCallback;
    }

    [RelayCommand]
    private void Remove() => _removeCallback(this);
}

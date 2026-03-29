using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using LubanGui.Models;

namespace LubanGui.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ILogger<MainWindowViewModel> _logger;

    [ObservableProperty]
    private string _excelDirectory = string.Empty;

    [ObservableProperty]
    private string _outputDirectory = string.Empty;

    [ObservableProperty]
    private string _lubanPath = string.Empty;

    [ObservableProperty]
    private string _statusText = "就绪";

    [ObservableProperty]
    private bool _isExporting = false;

    public ObservableCollection<LogEntry> LogEntries { get; } = new();

    // Design-time / unit-test constructor (no real logger needed)
    public MainWindowViewModel() : this(NullLogger<MainWindowViewModel>.Instance)
    {
    }

    public MainWindowViewModel(ILogger<MainWindowViewModel> logger)
    {
        _logger = logger;
        _logger.LogInformation("MainWindowViewModel 初始化完成");
        AddLog(LogEntryLevel.Info, "Luban 导表工具已就绪");
    }

    [RelayCommand]
    private void BrowseExcelDirectory()
    {
        AddLog(LogEntryLevel.Info, "文件浏览对话框将在 Week 2 实现");
    }

    [RelayCommand]
    private void BrowseOutputDirectory()
    {
        AddLog(LogEntryLevel.Info, "文件浏览对话框将在 Week 2 实现");
    }

    [RelayCommand]
    private void BrowseLubanPath()
    {
        AddLog(LogEntryLevel.Info, "文件浏览对话框将在 Week 2 实现");
    }

    [RelayCommand]
    private void ValidateConfig()
    {
        _logger.LogInformation("ValidateConfig 命令被调用");
        AddLog(LogEntryLevel.Info, "配置校验功能将在 Week 2 实现");
    }

    [RelayCommand]
    private void Export()
    {
        _logger.LogInformation("Export 命令被调用");
        AddLog(LogEntryLevel.Info, "全量导表功能将在 Week 2 实现");
    }

    [RelayCommand(CanExecute = nameof(IsExporting))]
    private void Cancel()
    {
        _logger.LogWarning("Cancel 命令被调用");
        AddLog(LogEntryLevel.Warning, "取消功能将在 Week 2 实现");
    }

    public void AddLog(LogEntryLevel level, string message)
    {
        LogEntries.Add(new LogEntry
        {
            Level = level,
            Message = message,
            Timestamp = DateTime.Now,
        });
    }
}


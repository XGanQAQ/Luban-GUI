using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using LubanGui.Models;

namespace LubanGui.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ILogger<MainWindowViewModel> _logger;

    // ── 必填参数 ──────────────────────────────────────────────────────────────

    [ObservableProperty]
    private string _lubanPath = string.Empty;

    [ObservableProperty]
    private string _confFile = string.Empty;

    [ObservableProperty]
    private string _target = string.Empty;

    // ── 常用可选：列表字段 ────────────────────────────────────────────────────

    public ObservableCollection<StringItemViewModel> CodeTargets { get; } = new();
    public ObservableCollection<StringItemViewModel> DataTargets { get; } = new();
    public ObservableCollection<StringItemViewModel> Xargs { get; } = new();

    // ── 高级可选：标量字段 ────────────────────────────────────────────────────

    [ObservableProperty]
    private string _schemaCollector = "default";

    [ObservableProperty]
    private string _pipeline = "default";

    [ObservableProperty]
    private string _timeZone = string.Empty;

    [ObservableProperty]
    private string _logConfig = "nlog.xml";

    [ObservableProperty]
    private bool _validationFailAsError;

    [ObservableProperty]
    private bool _forceLoadTableDatas;

    [ObservableProperty]
    private bool _verbose;

    // ── 高级可选：列表字段 ────────────────────────────────────────────────────

    public ObservableCollection<StringItemViewModel> OutputTables { get; } = new();
    public ObservableCollection<StringItemViewModel> IncludeTags { get; } = new();
    public ObservableCollection<StringItemViewModel> ExcludeTags { get; } = new();
    public ObservableCollection<StringItemViewModel> Variants { get; } = new();
    public ObservableCollection<StringItemViewModel> CustomTemplateDirs { get; } = new();
    public ObservableCollection<StringItemViewModel> WatchDirs { get; } = new();

    // ── UI 状态 ───────────────────────────────────────────────────────────────

    [ObservableProperty]
    private string _statusText = "就绪";

    [ObservableProperty]
    private bool _isExporting;

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

    partial void OnIsExportingChanged(bool value)
    {
        CancelCommand.NotifyCanExecuteChanged();
        ExportCommand.NotifyCanExecuteChanged();
        ValidateConfigCommand.NotifyCanExecuteChanged();
    }

    // ── 列表项操作命令 ────────────────────────────────────────────────────────

    [RelayCommand]
    private void AddCodeTarget() => CodeTargets.Add(new StringItemViewModel(string.Empty, item => CodeTargets.Remove(item)));

    [RelayCommand]
    private void AddDataTarget() => DataTargets.Add(new StringItemViewModel(string.Empty, item => DataTargets.Remove(item)));

    [RelayCommand]
    private void AddXargs() => Xargs.Add(new StringItemViewModel(string.Empty, item => Xargs.Remove(item)));

    [RelayCommand]
    private void AddOutputTable() => OutputTables.Add(new StringItemViewModel(string.Empty, item => OutputTables.Remove(item)));

    [RelayCommand]
    private void AddIncludeTag() => IncludeTags.Add(new StringItemViewModel(string.Empty, item => IncludeTags.Remove(item)));

    [RelayCommand]
    private void AddExcludeTag() => ExcludeTags.Add(new StringItemViewModel(string.Empty, item => ExcludeTags.Remove(item)));

    [RelayCommand]
    private void AddVariant() => Variants.Add(new StringItemViewModel(string.Empty, item => Variants.Remove(item)));

    [RelayCommand]
    private void AddCustomTemplateDir() => CustomTemplateDirs.Add(new StringItemViewModel(string.Empty, item => CustomTemplateDirs.Remove(item)));

    [RelayCommand]
    private void AddWatchDir() => WatchDirs.Add(new StringItemViewModel(string.Empty, item => WatchDirs.Remove(item)));

    // ── 文件/目录浏览命令（Week 2 实现） ─────────────────────────────────────

    [RelayCommand]
    private void BrowseLubanPath()
    {
        AddLog(LogEntryLevel.Info, "文件浏览对话框将在 Week 2 实现");
    }

    [RelayCommand]
    private void BrowseConfFile()
    {
        AddLog(LogEntryLevel.Info, "文件浏览对话框将在 Week 2 实现");
    }

    // ── 主操作命令 ────────────────────────────────────────────────────────────

    private bool CanValidateConfig() => !IsExporting;

    [RelayCommand(CanExecute = nameof(CanValidateConfig))]
    private void ValidateConfig()
    {
        _logger.LogInformation("ValidateConfig 命令被调用");
        AddLog(LogEntryLevel.Info, "配置校验功能将在 Week 2 实现");
    }

    private bool CanExport() => !IsExporting;

    [RelayCommand(CanExecute = nameof(CanExport))]
    private void Export()
    {
        _logger.LogInformation("Export 命令被调用");
        AddLog(LogEntryLevel.Info, "全量导表功能将在 Week 2 实现");
    }

    private bool CanCancel() => IsExporting;

    [RelayCommand(CanExecute = nameof(CanCancel))]
    private void Cancel()
    {
        _logger.LogWarning("Cancel 命令被调用");
        AddLog(LogEntryLevel.Warning, "取消功能将在 Week 2 实现");
    }

    // ── 辅助方法 ──────────────────────────────────────────────────────────────

    /// <summary>
    /// 根据当前 ViewModel 状态构建 <see cref="ExportConfig"/>，供服务层调用。
    /// </summary>
    public ExportConfig BuildConfig() => new()
    {
        LubanPath = LubanPath,
        ConfFile = ConfFile,
        Target = Target,
        CodeTargets = CodeTargets.Select(x => x.Value).Where(v => !string.IsNullOrWhiteSpace(v)).ToList(),
        DataTargets = DataTargets.Select(x => x.Value).Where(v => !string.IsNullOrWhiteSpace(v)).ToList(),
        Xargs = Xargs.Select(x => x.Value).Where(v => !string.IsNullOrWhiteSpace(v)).ToList(),
        SchemaCollector = SchemaCollector,
        Pipeline = Pipeline,
        OutputTables = OutputTables.Select(x => x.Value).Where(v => !string.IsNullOrWhiteSpace(v)).ToList(),
        IncludeTags = IncludeTags.Select(x => x.Value).Where(v => !string.IsNullOrWhiteSpace(v)).ToList(),
        ExcludeTags = ExcludeTags.Select(x => x.Value).Where(v => !string.IsNullOrWhiteSpace(v)).ToList(),
        Variants = Variants.Select(x => x.Value).Where(v => !string.IsNullOrWhiteSpace(v)).ToList(),
        TimeZone = TimeZone,
        CustomTemplateDirs = CustomTemplateDirs.Select(x => x.Value).Where(v => !string.IsNullOrWhiteSpace(v)).ToList(),
        ValidationFailAsError = ValidationFailAsError,
        LogConfig = LogConfig,
        WatchDirs = WatchDirs.Select(x => x.Value).Where(v => !string.IsNullOrWhiteSpace(v)).ToList(),
        ForceLoadTableDatas = ForceLoadTableDatas,
        Verbose = Verbose,
    };

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


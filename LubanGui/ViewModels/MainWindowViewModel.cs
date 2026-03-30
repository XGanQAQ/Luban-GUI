using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using LubanGui.Models;

namespace LubanGui.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ILogger<MainWindowViewModel> _logger;

    // ── 导出配置：GUI 专有 ────────────────────────────────────────────────────

    [ObservableProperty]
    private string _profileName = "Default";

    // ── 导出配置：必填参数 ────────────────────────────────────────────────────

    [ObservableProperty]
    private string _lubanPath = string.Empty;

    [ObservableProperty]
    private string _confFile = string.Empty;

    [ObservableProperty]
    private string _target = string.Empty;

    // ── 导出配置：常用可选列表 ────────────────────────────────────────────────

    public ObservableCollection<StringItemViewModel> CodeTargets { get; } = new();
    public ObservableCollection<StringItemViewModel> DataTargets { get; } = new();
    public ObservableCollection<StringItemViewModel> Xargs { get; } = new();

    // ── 导出配置：高级可选标量 ────────────────────────────────────────────────

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

    // ── 导出配置：高级可选列表 ────────────────────────────────────────────────

    public ObservableCollection<StringItemViewModel> OutputTables { get; } = new();
    public ObservableCollection<StringItemViewModel> IncludeTags { get; } = new();
    public ObservableCollection<StringItemViewModel> ExcludeTags { get; } = new();
    public ObservableCollection<StringItemViewModel> Variants { get; } = new();
    public ObservableCollection<StringItemViewModel> CustomTemplateDirs { get; } = new();
    public ObservableCollection<StringItemViewModel> WatchDirs { get; } = new();

    // ── 表格列表 ──────────────────────────────────────────────────────────────

    public ObservableCollection<TableEntryViewModel> Tables { get; } = new();

    [ObservableProperty]
    private TableEntryViewModel? _selectedTable;

    [ObservableProperty]
    private string _tableFilter = string.Empty;

    // ── UI 状态 ───────────────────────────────────────────────────────────────

    [ObservableProperty]
    private string _statusText = "● 就绪";

    [ObservableProperty]
    private ExportStatus _exportStatus = ExportStatus.Idle;

    [ObservableProperty]
    private bool _isExporting;

    [ObservableProperty]
    private string _lastExportStatusText = "上次导表: —";

    /// <summary>应用版本号，从程序集版本读取。</summary>
    public string AppVersion { get; } =
        "v" + (Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.0");

    public ObservableCollection<LogEntry> LogEntries { get; } = new();

    // ── 二级窗口事件（由 View 层订阅，负责实际打开窗口） ──────────────────────

    public event EventHandler? OpenLogWindowRequested;
    public event EventHandler? OpenExportSettingsRequested;
    public event EventHandler? OpenAboutRequested;

    // ── 构造函数 ──────────────────────────────────────────────────────────────

    public MainWindowViewModel() : this(NullLogger<MainWindowViewModel>.Instance) { }

    public MainWindowViewModel(ILogger<MainWindowViewModel> logger)
    {
        _logger = logger;
        _logger.LogInformation("MainWindowViewModel 初始化完成");
        AddLog(LogEntryLevel.Info, "Luban 导表工具已就绪");
    }

    // ── 属性变更联动 ──────────────────────────────────────────────────────────

    partial void OnIsExportingChanged(bool value)
    {
        ExportStatus = value ? ExportStatus.Exporting : ExportStatus.Idle;
        CancelCommand.NotifyCanExecuteChanged();
        ExportCommand.NotifyCanExecuteChanged();
        ValidateConfigCommand.NotifyCanExecuteChanged();
    }

    partial void OnExportStatusChanged(ExportStatus value)
    {
        StatusText = value switch
        {
            ExportStatus.Exporting => "⟳ 导表中…",
            ExportStatus.Success   => "✓ 成功",
            ExportStatus.Failed    => "✗ 失败",
            ExportStatus.Cancelled => "✗ 已取消",
            _                      => "● 就绪",
        };
    }

    // ── 文件菜单命令 ──────────────────────────────────────────────────────────

    [RelayCommand]
    private void NewConfig()
    {
        ProfileName = "Default";
        LubanPath = string.Empty;
        ConfFile = string.Empty;
        Target = string.Empty;
        CodeTargets.Clear();
        DataTargets.Clear();
        Xargs.Clear();
        AddLog(LogEntryLevel.Info, "已创建新配置");
    }

    [RelayCommand]
    private void OpenConfig()
    {
        AddLog(LogEntryLevel.Info, "打开配置文件功能将在后续版本实现");
    }

    [RelayCommand]
    private void SaveConfig()
    {
        AddLog(LogEntryLevel.Info, "保存配置功能将在后续版本实现");
    }

    [RelayCommand]
    private void Exit()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }

    // ── 操作菜单命令 ──────────────────────────────────────────────────────────

    private bool CanExport() => !IsExporting;

    [RelayCommand(CanExecute = nameof(CanExport))]
    private void Export()
    {
        _logger.LogInformation("Export 命令被调用");
        OpenLogWindowRequested?.Invoke(this, EventArgs.Empty);
        AddLog(LogEntryLevel.Info, "全量导表功能将在后续版本实现");
    }

    private bool CanValidateConfig() => !IsExporting;

    [RelayCommand(CanExecute = nameof(CanValidateConfig))]
    private void ValidateConfig()
    {
        _logger.LogInformation("ValidateConfig 命令被调用");
        OpenLogWindowRequested?.Invoke(this, EventArgs.Empty);
        AddLog(LogEntryLevel.Info, "配置校验功能将在后续版本实现");
    }

    private bool CanCancel() => IsExporting;

    [RelayCommand(CanExecute = nameof(CanCancel))]
    private void Cancel()
    {
        _logger.LogWarning("Cancel 命令被调用");
        AddLog(LogEntryLevel.Warning, "取消功能将在后续版本实现");
    }

    [RelayCommand]
    private void OpenExportSettings() =>
        OpenExportSettingsRequested?.Invoke(this, EventArgs.Empty);

    // ── 视图菜单命令 ──────────────────────────────────────────────────────────

    [RelayCommand]
    private void OpenLogWindow() =>
        OpenLogWindowRequested?.Invoke(this, EventArgs.Empty);

    [RelayCommand]
    private void RefreshTables()
    {
        AddLog(LogEntryLevel.Info, "表格列表刷新功能将在后续版本实现");
    }

    // ── 帮助菜单命令 ──────────────────────────────────────────────────────────

    [RelayCommand]
    private void ShowAbout() =>
        OpenAboutRequested?.Invoke(this, EventArgs.Empty);

    // ── 列表项操作命令（导出配置用） ──────────────────────────────────────────

    [RelayCommand]
    private void AddCodeTarget() =>
        CodeTargets.Add(new StringItemViewModel(string.Empty, item => CodeTargets.Remove(item)));

    [RelayCommand]
    private void AddDataTarget() =>
        DataTargets.Add(new StringItemViewModel(string.Empty, item => DataTargets.Remove(item)));

    [RelayCommand]
    private void AddXargs() =>
        Xargs.Add(new StringItemViewModel(string.Empty, item => Xargs.Remove(item)));

    [RelayCommand]
    private void AddOutputTable() =>
        OutputTables.Add(new StringItemViewModel(string.Empty, item => OutputTables.Remove(item)));

    [RelayCommand]
    private void AddIncludeTag() =>
        IncludeTags.Add(new StringItemViewModel(string.Empty, item => IncludeTags.Remove(item)));

    [RelayCommand]
    private void AddExcludeTag() =>
        ExcludeTags.Add(new StringItemViewModel(string.Empty, item => ExcludeTags.Remove(item)));

    [RelayCommand]
    private void AddVariant() =>
        Variants.Add(new StringItemViewModel(string.Empty, item => Variants.Remove(item)));

    [RelayCommand]
    private void AddCustomTemplateDir() =>
        CustomTemplateDirs.Add(new StringItemViewModel(string.Empty, item => CustomTemplateDirs.Remove(item)));

    [RelayCommand]
    private void AddWatchDir() =>
        WatchDirs.Add(new StringItemViewModel(string.Empty, item => WatchDirs.Remove(item)));

    // ── 文件浏览命令 ──────────────────────────────────────────────────────────

    [RelayCommand]
    private void BrowseLubanPath()
    {
        AddLog(LogEntryLevel.Info, "文件浏览对话框将在后续版本实现");
    }

    [RelayCommand]
    private void BrowseConfFile()
    {
        AddLog(LogEntryLevel.Info, "文件浏览对话框将在后续版本实现");
    }

    // ── 日志操作命令 ──────────────────────────────────────────────────────────

    [RelayCommand]
    private void ClearLogs()
    {
        LogEntries.Clear();
        _logger.LogInformation("日志已清空");
    }

    // ── 辅助方法 ──────────────────────────────────────────────────────────────

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


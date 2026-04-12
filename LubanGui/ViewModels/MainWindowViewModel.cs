using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using LubanGui.Infrastructure;
using LubanGui.Models;
using LubanGui.Services;

namespace LubanGui.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly IProjectManager? _projectManager;
    private readonly ISchemaService? _schemaService;
    private readonly ITablePreviewService? _previewService;
    private readonly FileOpenService? _fileOpenService;

    // ── 项目管理 ──────────────────────────────────────────────────────────────

    /// <summary>所有已注册的项目列表（绑定到项目切换栏下拉框）。</summary>
    public ObservableCollection<ProjectInfo> Projects { get; } = new();

    private bool _isSyncingProject = false;

    [ObservableProperty]
    private ProjectInfo? _currentProject;

    partial void OnCurrentProjectChanged(ProjectInfo? value)
    {
        OnPropertyChanged(nameof(HasCurrentProject));
        OpenProjectFolderCommand.NotifyCanExecuteChanged();
        Tables.Clear();
        FilteredTables.Clear();
        PreviewDataView = null;
        SelectedTableMeta = null;

        // 当用户通过 ComboBox 切换项目时，通知 ProjectManager（避免重入）
        if (!_isSyncingProject && _projectManager != null && value != null
            && !string.Equals(value.Name, _projectManager.CurrentProject?.Name, StringComparison.Ordinal))
        {
            _ = _projectManager.SwitchProjectAsync(value.Name);
        }

        AddLog(LogEntryLevel.Info, value != null
            ? $"已切换到项目：{value.Name}"
            : "当前无打开的项目");

        if (value != null)
        {
            _ = RefreshTablesInternalAsync(value.ProjectPath);
        }
    }

    /// <summary>是否有当前打开的项目（用于 UI 绑定启用状态）。</summary>
    public bool HasCurrentProject => CurrentProject != null;

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

    partial void OnTableFilterChanged(string value) => ApplyTableFilter();

    /// <summary>过滤后显示的表格列表（绑定到左侧 ListBox）。</summary>
    public ObservableCollection<TableEntryViewModel> FilteredTables { get; } = new();

    // ── 表格内容预览 ───────────────────────────────────────────────────────────

    /// <summary>当前预览的表格数据（DataGrid ItemsSource）。</summary>
    [ObservableProperty]
    private DataView? _previewDataView;

    /// <summary>当前预览的表格元数据（用于双击列标题打开文件）。</summary>
    [ObservableProperty]
    private TableMeta? _selectedTableMeta;

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
    public event EventHandler? NewProjectRequested;
    public event EventHandler? OpenProjectRequested;
    public event EventHandler? NewTableRequested;
    public event EventHandler? NewEnumRequested;
    public event EventHandler? NewBeanRequested;
    public event EventHandler? ImportFileRequested;

    // ── 构造函数 ──────────────────────────────────────────────────────────────

    public MainWindowViewModel() : this(NullLogger<MainWindowViewModel>.Instance, null, null, null, null) { }

    public MainWindowViewModel(
        ILogger<MainWindowViewModel> logger,
        IProjectManager? projectManager,
        ISchemaService? schemaService,
        ITablePreviewService? previewService,
        FileOpenService? fileOpenService)
    {
        _logger = logger;
        _projectManager = projectManager;
        _schemaService = schemaService;
        _previewService = previewService;
        _fileOpenService = fileOpenService;

        if (_projectManager != null)
        {
            _projectManager.CurrentProjectChanged += OnCurrentProjectChanged;
            SyncProjectList();
        }

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
        NewProjectRequested?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private void OpenConfig()
    {
        OpenProjectRequested?.Invoke(this, EventArgs.Empty);
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

    // ── 项目管理命令 ──────────────────────────────────────────────────────────

    [RelayCommand]
    private void NewProject() => NewProjectRequested?.Invoke(this, EventArgs.Empty);

    [RelayCommand]
    private void OpenProject() => OpenProjectRequested?.Invoke(this, EventArgs.Empty);

    [RelayCommand]
    private async Task SwitchProject(ProjectInfo? project)
    {
        if (project == null || _projectManager == null)
        {
            return;
        }

        try
        {
            await _projectManager.SwitchProjectAsync(project.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "切换项目失败");
            AddLog(LogEntryLevel.Error, $"切换项目失败：{ex.Message}");
        }
    }

    private bool CanOpenProjectFolder() => HasCurrentProject;

    [RelayCommand(CanExecute = nameof(CanOpenProjectFolder))]
    private void OpenProjectFolder()
    {
        if (CurrentProject == null)
        {
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = CurrentProject.ProjectPath,
                UseShellExecute = true,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "打开项目文件夹失败");
            AddLog(LogEntryLevel.Error, $"打开文件夹失败：{ex.Message}");
        }
    }

    // ── 项目切换响应 ──────────────────────────────────────────────────────────

    private void OnCurrentProjectChanged(object? sender, ProjectInfo? project)
    {
        _isSyncingProject = true;
        CurrentProject = project;
        _isSyncingProject = false;
        SyncProjectList();
    }

    private void SyncProjectList()
    {
        Projects.Clear();
        if (_projectManager == null)
        {
            return;
        }

        foreach (var p in _projectManager.Projects)
        {
            Projects.Add(p);
        }

        if (CurrentProject == null && _projectManager.CurrentProject != null)
        {
            CurrentProject = _projectManager.CurrentProject;
        }
    }

    /// <summary>供外部（App 启动时）在 ProjectManager 初始化完成后刷新项目列表。</summary>
    public void SyncProjectsFromManager() => SyncProjectList();

    // ── 表格选中/预览 ──────────────────────────────────────────────────────────

    partial void OnSelectedTableChanged(TableEntryViewModel? value)
    {
        if (value == null || CurrentProject == null || _previewService == null)
        {
            PreviewDataView = null;
            SelectedTableMeta = null;
            return;
        }

        var meta = GetMetaForEntry(value);
        SelectedTableMeta = meta;

        if (meta == null)
        {
            PreviewDataView = null;
            return;
        }

        _ = LoadPreviewAsync(meta);
    }

    private async Task LoadPreviewAsync(TableMeta meta)
    {
        if (_previewService == null || CurrentProject == null)
        {
            return;
        }

        var absPath = Path.Combine(CurrentProject.ProjectPath, "Datas", meta.Input);
        var confPath = Path.Combine(CurrentProject.ProjectPath, "luban.conf");
        try
        {
            var data = await _previewService.LoadPreviewAsync(absPath, confPath);
            var dt = new DataTable();
            foreach (var col in data.Columns)
            {
                dt.Columns.Add(col, typeof(string));
            }

            foreach (var row in data.Rows)
            {
                var r = dt.NewRow();
                for (int i = 0; i < row.Count && i < dt.Columns.Count; i++)
                {
                    r[i] = row[i];
                }
                dt.Rows.Add(r);
            }

            PreviewDataView = dt.DefaultView;
            AddLog(LogEntryLevel.Info, $"已加载预览：{meta.DisplayName}（{data.Rows.Count} 行，{data.Columns.Count} 列）");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加载预览失败：{Path}", absPath);
            AddLog(LogEntryLevel.Error, $"预览加载失败：{ex.Message}");
        }
    }

    // ── 表格列表刷新 ──────────────────────────────────────────────────────────

    private async Task RefreshTablesInternalAsync(string projectPath)
    {
        if (_schemaService == null)
        {
            return;
        }

        try
        {
            var metas = await _schemaService.LoadTablesAsync(projectPath);

            Tables.Clear();
            foreach (var meta in metas)
            {
                Tables.Add(new TableEntryViewModel { Name = meta.FullName });
            }

            ApplyTableFilter();
            AddLog(LogEntryLevel.Info, $"已加载 {metas.Count} 张表格");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刷新表格列表失败");
            AddLog(LogEntryLevel.Error, $"刷新表格列表失败：{ex.Message}");
        }
    }

    private void ApplyTableFilter()
    {
        FilteredTables.Clear();
        var filter = TableFilter?.Trim() ?? string.Empty;

        foreach (var t in Tables)
        {
            if (string.IsNullOrEmpty(filter)
                || t.Name.Contains(filter, StringComparison.OrdinalIgnoreCase))
            {
                FilteredTables.Add(t);
            }
        }
    }

    /// <summary>将 TableEntryViewModel 映射到对应的 TableMeta（通过 FullName 匹配）。</summary>
    private TableMeta? GetMetaForEntry(TableEntryViewModel entry)
    {
        if (CurrentProject == null || _schemaService == null)
        {
            return null;
        }

        // 简单实现：重新从内存中搜索（TODO：缓存 metas）
        // 目前通过 Tables 集合顺序反推 index，用 FullName 匹配
        var tablesXlsx = Path.Combine(CurrentProject.ProjectPath, "Datas", "__tables__.xlsx");
        if (!File.Exists(tablesXlsx))
        {
            return null;
        }

        // 同步加载（仅用于单次映射，不频繁调用）
        try
        {
            var metas = _schemaService.LoadTablesAsync(CurrentProject.ProjectPath).GetAwaiter().GetResult();
            return metas.FirstOrDefault(m =>
                string.Equals(m.FullName, entry.Name, StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            return null;
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
    private async Task RefreshTables()
    {
        if (CurrentProject == null)
        {
            return;
        }

        await RefreshTablesInternalAsync(CurrentProject.ProjectPath);
    }

    // ── 快捷操作工具栏命令 ────────────────────────────────────────────────────

    [RelayCommand]
    private void NewTable() => NewTableRequested?.Invoke(this, EventArgs.Empty);

    [RelayCommand]
    private void NewEnum() => NewEnumRequested?.Invoke(this, EventArgs.Empty);

    [RelayCommand]
    private void NewBean() => NewBeanRequested?.Invoke(this, EventArgs.Empty);

    [RelayCommand]
    private void ImportFile() => ImportFileRequested?.Invoke(this, EventArgs.Empty);

    // ── 表格列表上下文命令 ────────────────────────────────────────────────────

    [RelayCommand]
    private void OpenTableFile(TableEntryViewModel? entry)
    {
        if (entry == null || CurrentProject == null || _fileOpenService == null)
        {
            return;
        }

        var meta = GetMetaForEntry(entry);
        if (meta == null)
        {
            return;
        }

        var absPath = Path.Combine(CurrentProject.ProjectPath, "Datas", meta.Input);
        try
        {
            _fileOpenService.OpenFile(absPath);
        }
        catch (Exception ex)
        {
            AddLog(LogEntryLevel.Error, $"打开文件失败：{ex.Message}");
        }
    }

    [RelayCommand]
    private void ShowTableInExplorer(TableEntryViewModel? entry)
    {
        if (entry == null || CurrentProject == null || _fileOpenService == null)
        {
            return;
        }

        var meta = GetMetaForEntry(entry);
        if (meta == null)
        {
            return;
        }

        var absPath = Path.Combine(CurrentProject.ProjectPath, "Datas", meta.Input);
        try
        {
            _fileOpenService.OpenInExplorer(absPath);
        }
        catch (Exception ex)
        {
            AddLog(LogEntryLevel.Error, $"在资源管理器中显示失败：{ex.Message}");
        }
    }

    [RelayCommand]
    private void RemoveTable(TableEntryViewModel? entry)
    {
        if (entry == null)
        {
            return;
        }

        Tables.Remove(entry);
        ApplyTableFilter();
        AddLog(LogEntryLevel.Info, $"已从列表中移除：{entry.Name}（文件未被删除）");
    }

    /// <summary>当用户在预览面板中双击列标题时调用（由 View 层传入列名）。</summary>
    public void OpenFileAtField(string fieldName)
    {
        if (SelectedTableMeta == null || CurrentProject == null || _fileOpenService == null)
        {
            return;
        }

        var absPath = Path.Combine(CurrentProject.ProjectPath, "Datas", SelectedTableMeta.Input);
        try
        {
            _fileOpenService.OpenFile(absPath);
            AddLog(LogEntryLevel.Info, $"已打开文件（字段：{fieldName}）");
        }
        catch (Exception ex)
        {
            AddLog(LogEntryLevel.Error, $"打开文件失败：{ex.Message}");
        }
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


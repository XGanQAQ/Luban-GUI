using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using LubanGui.Infrastructure;
using LubanGui.LubanAdapter.Interfaces;
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
    private readonly IExportService? _exportService;
    private readonly ProjectConfigManager? _configManager;
    private readonly ILubanConfAdapter? _confAdapter;

    /// <summary>当前正在进行的导表操作的取消令牌源。</summary>
    private CancellationTokenSource? _exportCts;

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
        _cachedTableMetas.Clear();
        ClearPreview();
        ClearDataTypes();

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
            _ = LoadExportConfigAsync(value.ProjectPath);
        }
    }

    /// <summary>是否有当前打开的项目（用于 UI 绑定启用状态）。</summary>
    public bool HasCurrentProject => CurrentProject != null;

    // ── 导出配置 ──────────────────────────────────────────────────────────────

    /// <summary>导出配置 ViewModel，绑定到「导出配置」窗口。</summary>
    public ExportConfigViewModel ExportConfig { get; }

    // ── 表格列表 ──────────────────────────────────────────────────────────────

    /// <summary>最近一次 RefreshTablesInternalAsync 加载的元数据缓存，供 GetMetaForEntry 同步查找使用。</summary>
    private List<TableMeta> _cachedTableMetas = new();

    public ObservableCollection<TableEntryViewModel> Tables { get; } = new();

    [ObservableProperty]
    private TableEntryViewModel? _selectedTable;

    [ObservableProperty]
    private string _tableFilter = string.Empty;

    partial void OnTableFilterChanged(string value) => ApplyTableFilter();

    /// <summary>过滤后显示的表格列表（绑定到左侧 ListBox）。</summary>
    public ObservableCollection<TableEntryViewModel> FilteredTables { get; } = new();

    // ── 表格内容预览 ───────────────────────────────────────────────────────────

    /// <summary>当前预览表格的列名列表（null 表示无预览）；变更时由 View 层重建 DataGrid 列。</summary>
    [ObservableProperty]
    private IList<string>? _previewColumnNames;

    /// <summary>当前预览表格的数据行，每行是与 <see cref="PreviewColumnNames"/> 等长的字符串列表。</summary>
    public ObservableCollection<IList<string>> PreviewRows { get; } = new();

    /// <summary>当前预览的表格元数据（用于双击列标题打开文件）。</summary>
    [ObservableProperty]
    private TableMeta? _selectedTableMeta;

    // ── 数据类型列表 ───────────────────────────────────────────────────────────

    /// <summary>统一类型列表（内置类型 + 枚举 + Bean），绑定到「数据类型列表」窗口。</summary>
    public ObservableCollection<DataTypeListItem> DataTypes { get; } = new();

    [ObservableProperty]
    private bool _isDataTypesLoading;

    [ObservableProperty]
    private int _builtinTypeCount;

    [ObservableProperty]
    private int _enumTypeCount;

    [ObservableProperty]
    private int _beanTypeCount;

    public int TotalTypeCount => BuiltinTypeCount + EnumTypeCount + BeanTypeCount;

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
    public event EventHandler? OpenDataTypeListRequested;
    public event EventHandler? NewProjectRequested;
    public event EventHandler? OpenProjectRequested;
    public event EventHandler? NewTableRequested;
    public event EventHandler? NewEnumRequested;
    public event EventHandler? NewBeanRequested;
    public event EventHandler? ImportFileRequested;

    /// <summary>请求删除指定表格（由 View 层处理确认对话框与实际删除）。</summary>
    public event EventHandler<TableEntryViewModel?>? DeleteTableRequested;

    // ── 构造函数 ──────────────────────────────────────────────────────────────

    public MainWindowViewModel() : this(NullLogger<MainWindowViewModel>.Instance, null, null, null, null, null, null, null) { }

    public MainWindowViewModel(
        ILogger<MainWindowViewModel> logger,
        IProjectManager? projectManager,
        ISchemaService? schemaService,
        ITablePreviewService? previewService,
        FileOpenService? fileOpenService,
        IExportService? exportService,
        ProjectConfigManager? configManager,
        ILubanConfAdapter? confAdapter)
    {
        _logger = logger;
        _projectManager = projectManager;
        _schemaService = schemaService;
        _previewService = previewService;
        _fileOpenService = fileOpenService;
        _exportService = exportService;
        _configManager = configManager;
        _confAdapter = confAdapter;

        ExportConfig = new ExportConfigViewModel(SaveProjectConfigAsync);

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
            ClearPreview();
            return;
        }

        var meta = GetMetaForEntry(value);
        SelectedTableMeta = meta;

        if (meta == null)
        {
            ClearPreview();
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

            // Clear rows before changing columns to avoid index-out-of-range during rebind
            PreviewRows.Clear();
            PreviewColumnNames = data.Columns.Count > 0 ? data.Columns : null;

            foreach (var row in data.Rows)
            {
                PreviewRows.Add(row);
            }

            AddLog(LogEntryLevel.Info, $"已加载预览：{meta.DisplayName}（{data.Rows.Count} 行，{data.Columns.Count} 列）");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加载预览失败：{Path}", absPath);
            AddLog(LogEntryLevel.Error, $"预览加载失败：{ex.Message}");
        }
    }

    /// <summary>清空预览区域的列和行。</summary>
    private void ClearPreview()
    {
        PreviewRows.Clear();
        PreviewColumnNames = null;
        SelectedTableMeta = null;
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

            _cachedTableMetas = metas;
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
        return _cachedTableMetas.FirstOrDefault(m =>
            string.Equals(m.FullName, entry.Name, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>公开版本的 GetMetaForEntry，供 View 层在处理对话框时访问。</summary>
    public TableMeta? GetMetaForEntryPublic(TableEntryViewModel entry) => GetMetaForEntry(entry);

    // ── 操作菜单命令 ──────────────────────────────────────────────────────────

    private bool CanExport() => !IsExporting;

    [RelayCommand(CanExecute = nameof(CanExport))]
    private async Task Export()
    {
        if (_exportService == null)
        {
            AddLog(LogEntryLevel.Error, "导出服务未初始化");
            return;
        }

        OpenLogWindowRequested?.Invoke(this, EventArgs.Empty);

        var config = ExportConfig.ToProjectConfig();

        // 校验
        var errors = _exportService.ValidateConfig(config);
        if (errors.Count > 0)
        {
            foreach (var err in errors)
                AddLog(LogEntryLevel.Error, err);
            return;
        }

        IsExporting = true;
        ExportStatus = ExportStatus.Exporting;
        AddLog(LogEntryLevel.Info, "开始全量导表…");

        _exportCts = new CancellationTokenSource();
        var ct = _exportCts.Token;

        // IProgress<string> 会自动将回调切换到创建它的线程（UI 线程）
        var progress = new Progress<string>(line =>
        {
            var (level, msg) = ParseProgressLine(line);
            AddLog(level, msg);
        });

        try
        {
            var result = await Task.Run(() => _exportService.ExportAsync(config, progress, ct), ct);

            if (result.Success)
            {
                ExportStatus = ExportStatus.Success;
                LastExportStatusText = $"上次导表: 成功（{result.Duration.TotalSeconds:F1}s）";
                AddLog(LogEntryLevel.Success, $"导表成功！耗时 {result.Duration.TotalSeconds:F1}s");

                // 成功后 3 秒恢复就绪状态
                _ = Task.Delay(3000).ContinueWith(_ =>
                {
                    if (ExportStatus == ExportStatus.Success)
                        ExportStatus = ExportStatus.Idle;
                }, TaskScheduler.Current);
            }
            else if (result.ExitCode == -1 && result.ErrorMessage == "导表已取消")
            {
                ExportStatus = ExportStatus.Cancelled;
                LastExportStatusText = $"上次导表: 已取消";
                AddLog(LogEntryLevel.Warning, "导表已取消");
            }
            else
            {
                ExportStatus = ExportStatus.Failed;
                LastExportStatusText = $"上次导表: 失败";
                AddLog(LogEntryLevel.Error, $"导表失败：{result.ErrorMessage}");
            }
        }
        catch (Exception ex)
        {
            ExportStatus = ExportStatus.Failed;
            LastExportStatusText = "上次导表: 失败";
            AddLog(LogEntryLevel.Error, $"导表异常：{ex.Message}");
            _logger.LogError(ex, "导表过程中发生未处理异常");
        }
        finally
        {
            IsExporting = false;
            _exportCts?.Dispose();
            _exportCts = null;
        }
    }

    /// <summary>将进度行的前缀解析为日志级别。</summary>
    private static (LogEntryLevel level, string msg) ParseProgressLine(string line)
    {
        if (line.StartsWith("[ERR] ", StringComparison.Ordinal))
            return (LogEntryLevel.ProcessError, line[6..]);
        if (line.StartsWith("[ERROR] ", StringComparison.Ordinal))
            return (LogEntryLevel.Error, line[8..]);
        if (line.StartsWith("[WARN] ", StringComparison.Ordinal))
            return (LogEntryLevel.Warning, line[7..]);
        if (line.StartsWith("[OUT] ", StringComparison.Ordinal))
            return (LogEntryLevel.Output, line[6..]);
        return (LogEntryLevel.Output, line);
    }

    private bool CanValidateConfig() => !IsExporting;

    [RelayCommand(CanExecute = nameof(CanValidateConfig))]
    private void ValidateConfig()
    {
        if (_exportService == null)
        {
            AddLog(LogEntryLevel.Error, "导出服务未初始化");
            return;
        }

        OpenLogWindowRequested?.Invoke(this, EventArgs.Empty);
        var config = ExportConfig.ToProjectConfig();
        var errors = _exportService.ValidateConfig(config);

        if (errors.Count == 0)
        {
            AddLog(LogEntryLevel.Success, "配置校验通过 ✓");
        }
        else
        {
            AddLog(LogEntryLevel.Warning, $"配置校验发现 {errors.Count} 个问题：");
            foreach (var err in errors)
                AddLog(LogEntryLevel.Error, "  " + err);
        }
    }

    private bool CanCancel() => IsExporting;

    [RelayCommand(CanExecute = nameof(CanCancel))]
    private void Cancel()
    {
        if (_exportCts == null || _exportCts.IsCancellationRequested)
        {
            return;
        }

        _logger.LogWarning("用户请求取消导表");
        _exportCts.Cancel();
        AddLog(LogEntryLevel.Warning, "正在取消导表…");
    }

    [RelayCommand]
    private void OpenExportSettings() =>
        OpenExportSettingsRequested?.Invoke(this, EventArgs.Empty);

    // ── 视图菜单命令 ──────────────────────────────────────────────────────────

    [RelayCommand]
    private void OpenLogWindow() =>
        OpenLogWindowRequested?.Invoke(this, EventArgs.Empty);

    [RelayCommand]
    private async Task OpenDataTypeList()
    {
        if (CurrentProject == null)
        {
            AddLog(LogEntryLevel.Warning, "请先打开项目，再查看数据类型列表");
            return;
        }

        await ReloadDataTypesAsync(CurrentProject.ProjectPath);
        OpenDataTypeListRequested?.Invoke(this, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task RefreshTables()
    {
        if (CurrentProject == null)
        {
            return;
        }

        await RefreshTablesInternalAsync(CurrentProject.ProjectPath);
    }

    [RelayCommand]
    private async Task RefreshDataTypes()
    {
        if (CurrentProject == null)
        {
            return;
        }

        await ReloadDataTypesAsync(CurrentProject.ProjectPath);
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

    [RelayCommand]
    private void DeleteTable(TableEntryViewModel? entry)
    {
        if (entry == null)
        {
            return;
        }

        DeleteTableRequested?.Invoke(this, entry);
    }

    /// <summary>
    /// 在 UI 中移除指定条目并清空预览（由 View 层在服务端删除成功后调用）。
    /// </summary>
    public void ApplyTableDeletion(TableEntryViewModel entry)
    {
        bool wasSelected = ReferenceEquals(SelectedTable, entry);

        Tables.Remove(entry);
        _cachedTableMetas.RemoveAll(m =>
            string.Equals(m.FullName, entry.Name, StringComparison.OrdinalIgnoreCase));
        ApplyTableFilter();

        if (wasSelected)
        {
            SelectedTable = null;
            ClearPreview();
        }
    }

    /// <summary>
    /// 在 Excel 中定位到预览表格的指定单元格（由单元格右键菜单触发）。
    /// </summary>
    /// <param name="previewRowIndex">预览行的 0-based 索引。</param>
    /// <param name="previewColIndex">预览列的 0-based 索引。</param>
    public void OpenCellInExcel(int previewRowIndex, int previewColIndex)
    {
        if (SelectedTableMeta == null || CurrentProject == null || _fileOpenService == null)
        {
            return;
        }

        var absPath = Path.Combine(CurrentProject.ProjectPath, "Datas", SelectedTableMeta.Input);
        try
        {
            // Luban xlsx：前三行为 ##var / ##type / ## 注释，数据从第 4 行起；A 列留空，数据从 B 列（2）起
            int excelRow = previewRowIndex + 4;
            int excelCol = previewColIndex + 2;
            _fileOpenService.OpenFileAtCell(absPath, excelRow, excelCol);

            var colName = PreviewColumnNames != null && previewColIndex < PreviewColumnNames.Count
                ? PreviewColumnNames[previewColIndex]
                : $"列{previewColIndex + 1}";
            AddLog(LogEntryLevel.Info, $"已定位到 Excel 单元格：第 {excelRow} 行，第 {excelCol} 列（{colName}）");
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

    // ── 日志操作命令 ──────────────────────────────────────────────────────────

    [RelayCommand]
    private void ClearLogs()
    {
        LogEntries.Clear();
        _logger.LogInformation("日志已清空");
    }

    private void ClearDataTypes()
    {
        DataTypes.Clear();
        BuiltinTypeCount = 0;
        EnumTypeCount = 0;
        BeanTypeCount = 0;
        OnPropertyChanged(nameof(TotalTypeCount));
    }

    private async Task ReloadDataTypesAsync(string projectPath)
    {
        if (_schemaService == null)
        {
            return;
        }

        IsDataTypesLoading = true;
        try
        {
            var items = await _schemaService.GetUnifiedTypeListAsync(projectPath);

            DataTypes.Clear();
            foreach (var item in items)
            {
                DataTypes.Add(item);
            }

            BuiltinTypeCount = items.Count(i => i.Category == "内置");
            EnumTypeCount = items.Count(i => i.Category == "枚举");
            BeanTypeCount = items.Count(i => i.Category == "Bean");
            OnPropertyChanged(nameof(TotalTypeCount));

            AddLog(LogEntryLevel.Info,
                $"已加载数据类型：共 {TotalTypeCount} 项（内置 {BuiltinTypeCount} / 枚举 {EnumTypeCount} / Bean {BeanTypeCount}）");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "加载数据类型列表失败");
            AddLog(LogEntryLevel.Error, $"加载数据类型列表失败：{ex.Message}");
        }
        finally
        {
            IsDataTypesLoading = false;
        }
    }

    // ── 导出配置持久化 ────────────────────────────────────────────────────────

    /// <summary>
    /// 将 <paramref name="config"/> 持久化到当前项目目录下的 projectConfig.json。
    /// 由 <see cref="ExportConfigViewModel"/> 的保存委托调用。
    /// </summary>
    private async Task SaveProjectConfigAsync(ProjectConfig config)
    {
        if (_configManager == null || CurrentProject == null) return;
        try
        {
            await _configManager.SaveAsync(CurrentProject.ProjectPath, config);
            AddLog(LogEntryLevel.Success, "导出配置已保存");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存导出配置失败");
            AddLog(LogEntryLevel.Error, $"保存配置失败：{ex.Message}");
        }
    }

    /// <summary>从项目目录加载 ProjectConfig 并应用到 ExportConfigViewModel。</summary>
    private async Task LoadExportConfigAsync(string projectDir)
    {
        if (_configManager == null) return;
        try
        {
            var config = await _configManager.LoadAsync(projectDir);
            ExportConfig.LoadFromConfig(config);

            // 从 luban.conf 读取可用的导出目标列表
            if (_confAdapter != null)
            {
                var confPath = Path.Combine(projectDir, "luban.conf");
                if (File.Exists(confPath))
                {
                    try
                    {
                        var confDto = await _confAdapter.ReadAsync(confPath);
                        if (confDto.TargetNames.Count > 0)
                            ExportConfig.UpdateAvailableTargets(confDto.TargetNames);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "读取 luban.conf 目标列表失败，使用默认值");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "加载导出配置失败");
        }
    }

    // ── 日志辅助 ──────────────────────────────────────────────────────────────

    public void AddLog(LogEntryLevel level, string message)
    {
        const int MaxLogEntries = 10_000;
        const int TrimCount = 500;

        // 超过上限时移除最旧的若干条
        if (LogEntries.Count >= MaxLogEntries)
        {
            for (int i = 0; i < TrimCount && LogEntries.Count > 0; i++)
                LogEntries.RemoveAt(0);
        }

        LogEntries.Add(new LogEntry
        {
            Level = level,
            Message = message,
            Timestamp = DateTime.Now,
        });
    }
}

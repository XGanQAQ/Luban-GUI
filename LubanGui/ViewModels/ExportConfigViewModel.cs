using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LubanGui.Models;

namespace LubanGui.ViewModels;

/// <summary>
/// 管理「导出配置」窗口的状态，绑定简化的用户配置字段，并在保存时通过委托持久化。
/// </summary>
public partial class ExportConfigViewModel : ViewModelBase
{
    private readonly Func<ProjectConfig, Task> _saveDelegate;

    // ── 下拉框可选值（静态，来自 Luban 文档支持列表） ──────────────────────────

    /// <summary>支持的数据导出格式（data target）。</summary>
    public static IReadOnlyList<string> DataOutputTypes { get; } = new[]
    {
        "bin", "json", "json2", "lua", "xml", "yaml",
        "msgpack", "bson", "protobuf2-bin", "protobuf3-bin",
        "flatbuffers-json", "text-list",
    };

    /// <summary>支持的代码导出类型（code target）。</summary>
    public static IReadOnlyList<string> CodeOutputTypes { get; } = new[]
    {
        "cs-bin", "cs-simple-json", "cs-dotnet-json", "cs-newtonsoft-json",
        "cs-editor-json", "cs-protobuf2", "cs-protobuf3",
        "typescript-json", "typescript-bin", "typescript-protobuf",
        "javascript-json", "javascript-bin",
        "java-bin", "java-json",
        "go-bin", "go-json",
        "lua-bin", "lua-lua",
        "python-json",
        "rust-bin", "rust-json",
        "php-json", "dart-json", "gdscript-json",
        "cpp-sharedptr-bin", "cpp-rawptr-bin",
        "protobuf2", "protobuf3", "flatbuffers",
    };

    /// <summary>从当前项目 luban.conf 动态读取的导出目标列表。</summary>
    public ObservableCollection<string> AvailableTargets { get; } = new() { "all" };

    // ── 数据导出 ──────────────────────────────────────────────────────────────

    [ObservableProperty]
    private string _dataOutputType = "bin";

    [ObservableProperty]
    private string _dataOutputPath = string.Empty;

    // ── 代码导出 ──────────────────────────────────────────────────────────────

    [ObservableProperty]
    private bool _codeOutputEnabled;

    [ObservableProperty]
    private string _codeOutputType = "cs-bin";

    [ObservableProperty]
    private string _codeOutputPath = string.Empty;

    [ObservableProperty]
    private string _codeOutputTopModule = "cfg";

    // ── 导出目标 ──────────────────────────────────────────────────────────────

    [ObservableProperty]
    private string _target = "all";

    // ── 浏览路径事件（由 ExportSettingsWindow 订阅） ──────────────────────────

    /// <summary>用户点击「浏览」按钮选择数据输出目录。</summary>
    public event EventHandler? BrowseDataOutputPathRequested;

    /// <summary>用户点击「浏览」按钮选择代码输出目录。</summary>
    public event EventHandler? BrowseCodeOutputPathRequested;

    [RelayCommand]
    private void BrowseDataOutputPath() =>
        BrowseDataOutputPathRequested?.Invoke(this, EventArgs.Empty);

    [RelayCommand]
    private void BrowseCodeOutputPath() =>
        BrowseCodeOutputPathRequested?.Invoke(this, EventArgs.Empty);

    // ── 构造函数 ──────────────────────────────────────────────────────────────

    public ExportConfigViewModel(Func<ProjectConfig, Task> saveDelegate)
    {
        _saveDelegate = saveDelegate;
    }

    // ── 数据转换 ──────────────────────────────────────────────────────────────

    /// <summary>将当前 ViewModel 状态转换为 <see cref="ProjectConfig"/>。</summary>
    public ProjectConfig ToProjectConfig() => new()
    {
        DataOutput = new DataOutputConfig
        {
            Type       = DataOutputType,
            OutputPath = DataOutputPath,
        },
        CodeOutput = new CodeOutputConfig
        {
            Enabled   = CodeOutputEnabled,
            Type      = CodeOutputType,
            OutputPath = CodeOutputPath,
            TopModule = CodeOutputTopModule,
        },
        Target = Target,
    };

    /// <summary>将 <see cref="ProjectConfig"/> 的值填充到 ViewModel 属性。</summary>
    public void LoadFromConfig(ProjectConfig config)
    {
        DataOutputType    = config.DataOutput.Type;
        DataOutputPath    = config.DataOutput.OutputPath;
        CodeOutputEnabled = config.CodeOutput.Enabled;
        CodeOutputType    = config.CodeOutput.Type;
        CodeOutputPath    = config.CodeOutput.OutputPath;
        CodeOutputTopModule = config.CodeOutput.TopModule;
        Target            = config.Target;
    }

    /// <summary>
    /// 用 luban.conf 中读取的目标名称列表更新 <see cref="AvailableTargets"/>。
    /// 若当前选中的 Target 不在新列表中，自动切换到第一项。
    /// </summary>
    public void UpdateAvailableTargets(IEnumerable<string> targets)
    {
        AvailableTargets.Clear();
        foreach (var t in targets)
            AvailableTargets.Add(t);

        if (AvailableTargets.Count > 0 && !AvailableTargets.Contains(Target))
            Target = AvailableTargets[0];
    }

    // ── 保存 ──────────────────────────────────────────────────────────────────

    /// <summary>将当前配置通过注入的委托持久化。</summary>
    public Task SaveAsync() => _saveDelegate(ToProjectConfig());
}

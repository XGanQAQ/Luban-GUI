using System.Collections.Generic;

namespace LubanGui.Models;

/// <summary>
/// 导表配置，字段与 Luban CLI 参数一一对应。
/// </summary>
public class ExportConfig
{
    // ── GUI 专有 ──────────────────────────────────────────────────────────────

    /// <summary>配置名称，仅用于 GUI 内部区分，不传给 Luban CLI。</summary>
    public string ProfileName { get; set; } = "Default";

    /// <summary>Luban 可执行文件路径，例: C:\Tools\Luban\luban.exe</summary>
    public string LubanPath { get; set; } = string.Empty;

    // ── 必填参数 ──────────────────────────────────────────────────────────────

    /// <summary>Luban 配置文件路径 (--conf)，例: D:\GameData\luban.conf</summary>
    public string ConfFile { get; set; } = string.Empty;

    /// <summary>生成目标名称 (--target / -t)，例: client, server, all</summary>
    public string Target { get; set; } = string.Empty;

    // ── 常用可选参数 ──────────────────────────────────────────────────────────

    /// <summary>代码输出目标列表 (--codeTarget / -c)，例: ["cs-bin", "typescript-json"]</summary>
    public List<string> CodeTargets { get; set; } = new();

    /// <summary>数据输出目标列表 (--dataTarget / -d)，例: ["bin", "json"]</summary>
    public List<string> DataTargets { get; set; } = new();

    /// <summary>扩展参数列表 (--xargs / -x)，每项格式为 key=value，例: ["outputCodeDir=D:\\Gen"]</summary>
    public List<string> Xargs { get; set; } = new();

    // ── 高级可选参数 ──────────────────────────────────────────────────────────

    /// <summary>Schema 收集器名称 (--schemaCollector / -s)，默认 "default"</summary>
    public string SchemaCollector { get; set; } = "default";

    /// <summary>Pipeline 名称 (--pipeline / -p)，默认 "default"</summary>
    public string Pipeline { get; set; } = "default";

    /// <summary>只输出指定表 (--outputTable / -o)，可多个</summary>
    public List<string> OutputTables { get; set; } = new();

    /// <summary>包含标签过滤 (--includeTag / -i)，可多个</summary>
    public List<string> IncludeTags { get; set; } = new();

    /// <summary>排除标签过滤 (--excludeTag / -e)，可多个</summary>
    public List<string> ExcludeTags { get; set; } = new();

    /// <summary>字段变体 (--variant)，每项格式为 key=value，可多个</summary>
    public List<string> Variants { get; set; } = new();

    /// <summary>时区 (--timeZone)，例: "Asia/Shanghai"</summary>
    public string TimeZone { get; set; } = string.Empty;

    /// <summary>自定义模板目录列表 (--customTemplateDir)，可多个</summary>
    public List<string> CustomTemplateDirs { get; set; } = new();

    /// <summary>验证失败时作为错误退出 (--validationFailAsError)</summary>
    public bool ValidationFailAsError { get; set; }

    /// <summary>NLog 配置文件路径 (--logConfig / -l)，默认 "nlog.xml"</summary>
    public string LogConfig { get; set; } = "nlog.xml";

    /// <summary>监听目录列表，变更时自动重新生成 (--watchDir / -w)，可多个</summary>
    public List<string> WatchDirs { get; set; } = new();

    /// <summary>无 dataTarget 时强制加载表数据 (--forceLoadTableDatas / -f)</summary>
    public bool ForceLoadTableDatas { get; set; }

    /// <summary>详细输出模式 (--verbose / -v)</summary>
    public bool Verbose { get; set; }

    /// <summary>验证所有必填项是否有效。</summary>
    public bool IsValid() =>
        !string.IsNullOrWhiteSpace(LubanPath) &&
        !string.IsNullOrWhiteSpace(ConfFile) &&
        !string.IsNullOrWhiteSpace(Target);
}

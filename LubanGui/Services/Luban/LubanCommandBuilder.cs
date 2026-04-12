using System.Collections.Generic;
using LubanGui.Models;

namespace LubanGui.Services.Luban;

/// <summary>
/// 将 <see cref="ExportConfig"/> 转换为 Luban CLI 命令行参数数组。
/// 纯函数，无副作用，不依赖任何 Service 或 I/O。
/// </summary>
public static class LubanCommandBuilder
{
    /// <summary>
    /// 根据 <paramref name="config"/> 构建 Luban CLI 参数列表。
    /// 返回的字符串数组可直接填入 <see cref="System.Diagnostics.ProcessStartInfo.ArgumentList"/>。
    /// </summary>
    public static string[] BuildArgs(ExportConfig config)
    {
        var args = new List<string>();

        // ── 必填参数 ──────────────────────────────────────────────────────────
        args.Add("--conf");
        args.Add(config.ConfFile);

        args.Add("--target");
        args.Add(config.Target);

        // ── 常用可选：代码/数据/扩展参数 ─────────────────────────────────────
        foreach (var t in config.CodeTargets)
        {
            args.Add("--codeTarget");
            args.Add(t);
        }

        foreach (var t in config.DataTargets)
        {
            args.Add("--dataTarget");
            args.Add(t);
        }

        foreach (var x in config.Xargs)
        {
            args.Add("--xargs");
            args.Add(x);
        }

        // ── 高级可选：标量 ───────────────────────────────────────────────────
        if (!string.IsNullOrWhiteSpace(config.SchemaCollector) && config.SchemaCollector != "default")
        {
            args.Add("--schemaCollector");
            args.Add(config.SchemaCollector);
        }

        if (!string.IsNullOrWhiteSpace(config.Pipeline) && config.Pipeline != "default")
        {
            args.Add("--pipeline");
            args.Add(config.Pipeline);
        }

        if (!string.IsNullOrWhiteSpace(config.TimeZone))
        {
            args.Add("--timeZone");
            args.Add(config.TimeZone);
        }

        if (!string.IsNullOrWhiteSpace(config.LogConfig) && config.LogConfig != "nlog.xml")
        {
            args.Add("--logConfig");
            args.Add(config.LogConfig);
        }

        // ── 高级可选：列表 ───────────────────────────────────────────────────
        foreach (var t in config.OutputTables)
        {
            args.Add("--outputTable");
            args.Add(t);
        }

        foreach (var tag in config.IncludeTags)
        {
            args.Add("--includeTag");
            args.Add(tag);
        }

        foreach (var tag in config.ExcludeTags)
        {
            args.Add("--excludeTag");
            args.Add(tag);
        }

        foreach (var v in config.Variants)
        {
            args.Add("--variant");
            args.Add(v);
        }

        foreach (var dir in config.CustomTemplateDirs)
        {
            args.Add("--customTemplateDir");
            args.Add(dir);
        }

        foreach (var dir in config.WatchDirs)
        {
            args.Add("--watchDir");
            args.Add(dir);
        }

        // ── 高级可选：布尔开关 ────────────────────────────────────────────────
        if (config.ValidationFailAsError)
            args.Add("--validationFailAsError");

        if (config.ForceLoadTableDatas)
            args.Add("--forceLoadTableDatas");

        if (config.Verbose)
            args.Add("--verbose");

        return args.ToArray();
    }
}

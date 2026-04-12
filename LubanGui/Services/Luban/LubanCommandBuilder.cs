using System.Collections.Generic;
using LubanGui.Models;

namespace LubanGui.Services.Luban;

/// <summary>
/// 将 <see cref="ProjectConfig"/> 转换为 Luban CLI 命令行参数数组。
/// 纯函数，无副作用，不依赖任何 Service 或 I/O。
/// </summary>
public static class LubanCommandBuilder
{
    /// <summary>
    /// 根据 <paramref name="config"/> 和 <paramref name="confFilePath"/> 构建 Luban CLI 参数列表。
    /// 返回的字符串数组可直接填入 <see cref="System.Diagnostics.ProcessStartInfo.ArgumentList"/>。
    /// </summary>
    public static string[] BuildArgs(ProjectConfig config, string confFilePath)
    {
        var args = new List<string>();

        // ── 必填参数 ──────────────────────────────────────────────────────────
        args.Add("--conf");
        args.Add(confFilePath);

        args.Add("--target");
        args.Add(config.Target);

        // ── 数据导出 ──────────────────────────────────────────────────────────
        args.Add("--dataTarget");
        args.Add(config.DataOutput.Type);

        args.Add("--xargs");
        args.Add($"outputDataDir={config.DataOutput.OutputPath}");

        // ── 代码导出（可选）──────────────────────────────────────────────────
        if (config.CodeOutput.Enabled)
        {
            args.Add("--codeTarget");
            args.Add(config.CodeOutput.Type);

            args.Add("--xargs");
            args.Add($"outputCodeDir={config.CodeOutput.OutputPath}");

            if (!string.IsNullOrWhiteSpace(config.CodeOutput.TopModule))
            {
                args.Add("--xargs");
                args.Add($"topModule={config.CodeOutput.TopModule}");
            }
        }

        return args.ToArray();
    }
}

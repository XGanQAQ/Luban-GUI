using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LubanGui.Models;

namespace LubanGui.Services;

/// <summary>
/// 驱动完整导表流程：校验配置 → 构建 CLI 命令 → 执行 → 汇总结果。
/// </summary>
public interface IExportService
{
    /// <summary>
    /// 执行完整导表流程。
    /// </summary>
    /// <param name="config">导出配置。</param>
    /// <param name="progress">实时进度回调，每行 stdout/stderr 输出触发一次。</param>
    /// <param name="ct">取消令牌。</param>
    Task<ExportResult> ExportAsync(
        ProjectConfig config,
        IProgress<string> progress,
        CancellationToken ct);

    /// <summary>
    /// 校验导出配置的合法性，返回错误消息列表（空列表表示合法）。
    /// </summary>
    IReadOnlyList<string> ValidateConfig(ProjectConfig config);
}

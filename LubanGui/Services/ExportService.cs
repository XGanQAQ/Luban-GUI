using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LubanGui.Models;
using LubanGui.Services.Luban;
using Microsoft.Extensions.Logging;

namespace LubanGui.Services;

/// <summary>
/// 实现完整导表流程：校验 → 构建参数 → 调用 Luban CLI → 汇总结果。
/// </summary>
public class ExportService : IExportService
{
    private readonly ILubanExecutor _executor;
    private readonly ILogger<ExportService> _logger;

    public ExportService(ILubanExecutor executor, ILogger<ExportService> logger)
    {
        _executor = executor;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<ExportResult> ExportAsync(
        ExportConfig config,
        IProgress<string> progress,
        CancellationToken ct)
    {
        // 1. 校验配置
        var errors = ValidateConfig(config);
        if (errors.Count > 0)
        {
            var errMsg = string.Join("\n", errors);
            _logger.LogWarning("导出配置校验失败：{Errors}", errMsg);
            progress.Report("[ERROR] 配置校验失败：");
            foreach (var e in errors)
                progress.Report("[ERROR]   " + e);
            return new ExportResult { Success = false, ExitCode = -1, ErrorMessage = errMsg };
        }

        // 2. 构建 CLI 参数
        var args = LubanCommandBuilder.BuildArgs(config);
        _logger.LogInformation("开始导表，Luban 路径：{Path}", config.LubanPath);

        var sw = Stopwatch.StartNew();
        try
        {
            // 3. 执行 Luban CLI
            var exitCode = await _executor.RunAsync(config.LubanPath, args, progress, ct);
            sw.Stop();

            var success = exitCode == 0;
            _logger.LogInformation("导表完成，退出码={Code}，耗时={Duration}", exitCode, sw.Elapsed);

            return new ExportResult
            {
                Success      = success,
                ExitCode     = exitCode,
                ErrorMessage = success ? string.Empty : $"Luban 返回非零退出码: {exitCode}",
                Duration     = sw.Elapsed,
            };
        }
        catch (OperationCanceledException)
        {
            sw.Stop();
            _logger.LogInformation("导表已取消，耗时={Duration}", sw.Elapsed);
            progress.Report("[WARN] 导表已取消");
            return new ExportResult
            {
                Success      = false,
                ExitCode     = -1,
                ErrorMessage = "导表已取消",
                Duration     = sw.Elapsed,
            };
        }
        catch (Win32Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "启动 Luban 进程失败");
            var msg = $"无法启动 Luban 进程：{ex.Message}";
            progress.Report("[ERROR] " + msg);
            return new ExportResult
            {
                Success      = false,
                ExitCode     = -1,
                ErrorMessage = msg,
                Duration     = sw.Elapsed,
            };
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "导表过程中发生未预期异常");
            throw;
        }
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> ValidateConfig(ExportConfig config)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(config.LubanPath))
        {
            errors.Add("Luban 路径不能为空");
        }
        else if (!File.Exists(config.LubanPath))
        {
            errors.Add($"Luban 可执行文件不存在：{config.LubanPath}");
        }

        if (string.IsNullOrWhiteSpace(config.ConfFile))
        {
            errors.Add("配置文件路径（--conf）不能为空");
        }
        else if (!File.Exists(config.ConfFile))
        {
            errors.Add($"配置文件不存在：{config.ConfFile}");
        }

        if (string.IsNullOrWhiteSpace(config.Target))
        {
            errors.Add("导出目标（--target）不能为空");
        }

        return errors;
    }
}

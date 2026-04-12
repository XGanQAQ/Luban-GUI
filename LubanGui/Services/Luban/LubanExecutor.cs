using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LubanGui.Services.Luban;

/// <summary>
/// 启动 Luban CLI 子进程，并行收集 stdout/stderr，支持 <see cref="CancellationToken"/> 取消。
/// </summary>
public class LubanExecutor : ILubanExecutor
{
    private readonly ILogger<LubanExecutor> _logger;

    public LubanExecutor(ILogger<LubanExecutor> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<int> RunAsync(
        string lubanExePath,
        string[] args,
        IProgress<string> progress,
        CancellationToken ct)
    {
        var psi = BuildProcessStartInfo(lubanExePath, args);

        _logger.LogInformation("启动 Luban：{File} {Args}",
            psi.FileName, string.Join(" ", psi.ArgumentList));

        using var process = new Process { StartInfo = psi };
        process.Start();

        // 并行读取 stdout 和 stderr，防止缓冲区满导致子进程挂起
        var stdoutTask = CollectStreamAsync(process.StandardOutput, progress, "[OUT] ");
        var stderrTask = CollectStreamAsync(process.StandardError, progress, "[ERR] ");

        using var cancelReg = ct.Register(() =>
        {
            try
            {
                if (!process.HasExited)
                {
                    _logger.LogWarning("收到取消信号，正在终止 Luban 进程 (PID={Pid})", process.Id);
                    process.Kill(entireProcessTree: true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "终止 Luban 进程时出错");
            }
        });

        try
        {
            await Task.WhenAll(stdoutTask, stderrTask);
            await process.WaitForExitAsync(CancellationToken.None); // 等待进程真正退出
        }
        catch (OperationCanceledException)
        {
            // 确保流读取完毕后再抛出
            try { await Task.WhenAll(stdoutTask, stderrTask); } catch { /* ignore */ }
            await process.WaitForExitAsync(CancellationToken.None);
            throw;
        }

        _logger.LogInformation("Luban 进程退出，退出码 = {Code}", process.ExitCode);
        return process.ExitCode;
    }

    /// <summary>
    /// 构造 <see cref="ProcessStartInfo"/>。
    /// 若 <paramref name="lubanExePath"/> 为 .dll，自动使用 <c>dotnet</c> 运行。
    /// </summary>
    private static ProcessStartInfo BuildProcessStartInfo(string lubanExePath, string[] args)
    {
        var psi = new ProcessStartInfo
        {
            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            UseShellExecute        = false,
            CreateNoWindow         = true,
        };

        if (lubanExePath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
        {
            psi.FileName = "dotnet";
            psi.ArgumentList.Add(lubanExePath);
        }
        else
        {
            psi.FileName = lubanExePath;
        }

        foreach (var arg in args)
        {
            psi.ArgumentList.Add(arg);
        }

        return psi;
    }

    /// <summary>逐行读取流，每行通过 <paramref name="progress"/> 上报。</summary>
    private static async Task CollectStreamAsync(
        StreamReader reader,
        IProgress<string> progress,
        string prefix)
    {
        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            progress.Report(prefix + line);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LubanGui.Infrastructure;
using LubanGui.Models;
using LubanGui.Services.Luban;
using Microsoft.Extensions.Logging;

namespace LubanGui.Services;

/// <summary>
/// 实现完整导表流程：校验 → 构建参数 → 调用 Luban CLI → 汇总结果。
/// Luban DLL 路径优先取 <see cref="AppConfigManager"/> 中的自定义路径，
/// 未配置时自动回退到应用程序目录下的内置 <c>luban\Luban.dll</c>。
/// </summary>
public class ExportService : IExportService
{
    private readonly ILubanExecutor _executor;
    private readonly IProjectManager _projectManager;
    private readonly AppConfigManager _appConfigManager;
    private readonly ILogger<ExportService> _logger;

    /// <summary>内置 Luban.dll 的默认路径（相对于应用程序目录）。</summary>
    private static string DefaultLubanDllPath =>
        Path.Combine(AppContext.BaseDirectory, "luban", "Luban.dll");

    public ExportService(
        ILubanExecutor executor,
        IProjectManager projectManager,
        AppConfigManager appConfigManager,
        ILogger<ExportService> logger)
    {
        _executor = executor;
        _projectManager = projectManager;
        _appConfigManager = appConfigManager;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<ExportResult> ExportAsync(
        ProjectConfig config,
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

        // 2. 导表前修复 __tables__.xlsx 中的旧格式数据并清理重复记录
        var projectPath = _projectManager.CurrentProject?.ProjectPath;
        if (!string.IsNullOrEmpty(projectPath))
        {
            var tablesXlsx = Path.Combine(projectPath, "Datas", "__tables__.xlsx");
            try
            {
                int migrated = await Task.Run(() => ExcelWriter.MigrateToTbPrefixTableNames(tablesXlsx));
                if (migrated > 0)
                {
                    _logger.LogWarning("导表前已将 __tables__.xlsx 中 {Count} 条记录的 full_name 修正为 Tb 前缀格式。", migrated);
                    progress.Report($"[WARN] 已自动修正 {migrated} 条表格命名（full_name 添加 Tb 前缀）");
                }

                int removed = await Task.Run(() => ExcelWriter.RemoveDuplicateTableEntries(tablesXlsx));
                if (removed > 0)
                {
                    _logger.LogWarning("导表前在 __tables__.xlsx 中发现并清除了 {Count} 条重复记录。", removed);
                    progress.Report($"[WARN] 已自动清除 {removed} 条重复表格记录");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "清理 __tables__.xlsx 重复记录时发生异常，跳过清理继续导表。");
            }
        }

        // 3. 解析路径
        var lubanDllPath = ResolveLubanDllPath();
        var confFilePath = GetConfFilePath();

        // 4. 构建 CLI 参数
        var args = LubanCommandBuilder.BuildArgs(config, confFilePath);
        _logger.LogInformation("开始导表，Luban 路径：{Path}", lubanDllPath);

        var sw = Stopwatch.StartNew();
        try
        {
            // 5. 执行 Luban CLI
            var exitCode = await _executor.RunAsync(lubanDllPath, args, progress, ct);
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
    public IReadOnlyList<string> ValidateConfig(ProjectConfig config)
    {
        var errors = new List<string>();

        // 校验 Luban DLL 存在
        var lubanPath = ResolveLubanDllPath();
        if (!File.Exists(lubanPath))
            errors.Add($"Luban 可执行文件不存在：{lubanPath}");

        // 校验项目已打开且 luban.conf 存在
        var projectPath = _projectManager.CurrentProject?.ProjectPath;
        if (string.IsNullOrEmpty(projectPath))
        {
            errors.Add("当前无已打开的项目");
        }
        else
        {
            var confFile = Path.Combine(projectPath, "luban.conf");
            if (!File.Exists(confFile))
                errors.Add($"项目配置文件不存在：{confFile}");
        }

        // 校验导出目标
        if (string.IsNullOrWhiteSpace(config.Target))
            errors.Add("导出目标不能为空");

        // 校验数据输出路径
        if (string.IsNullOrWhiteSpace(config.DataOutput.OutputPath))
            errors.Add("数据输出路径不能为空");

        // 校验代码输出（可选）
        if (config.CodeOutput.Enabled && string.IsNullOrWhiteSpace(config.CodeOutput.OutputPath))
            errors.Add("代码输出路径不能为空（已勾选生成代码）");

        return errors;
    }

    /// <summary>
    /// 解析实际使用的 Luban DLL 路径：
    /// 优先使用 <see cref="AppConfig.LubanDllPath"/> 中的自定义路径，
    /// 未设置则回退到内置的 <see cref="DefaultLubanDllPath"/>。
    /// </summary>
    private string ResolveLubanDllPath()
    {
        var configured = _appConfigManager.GetLubanDllPath();
        return !string.IsNullOrWhiteSpace(configured) ? configured : DefaultLubanDllPath;
    }

    /// <summary>返回当前项目的 luban.conf 完整路径。</summary>
    private string GetConfFilePath()
    {
        var projectPath = _projectManager.CurrentProject?.ProjectPath ?? string.Empty;
        return Path.Combine(projectPath, "luban.conf");
    }
}

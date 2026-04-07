using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using LubanGui.Infrastructure;
using LubanGui.Models;
using Microsoft.Extensions.Logging;

namespace LubanGui.Services;

/// <summary>
/// 管理多个 Luban 项目的创建、切换与加载。
/// </summary>
public class ProjectManager : IProjectManager
{
    private readonly AppConfigManager _appConfigManager;
    private readonly ProjectConfigManager _projectConfigManager;
    private readonly ILogger<ProjectManager> _logger;

    private readonly List<ProjectInfo> _projects = new();

    public IReadOnlyList<ProjectInfo> Projects => _projects.AsReadOnly();
    public ProjectInfo? CurrentProject { get; private set; }

    /// <summary>当前项目切换后触发（新项目信息，或 null 表示无项目）。</summary>
    public event EventHandler<ProjectInfo?>? CurrentProjectChanged;

    public ProjectManager(
        AppConfigManager appConfigManager,
        ProjectConfigManager projectConfigManager,
        ILogger<ProjectManager> logger)
    {
        _appConfigManager = appConfigManager;
        _projectConfigManager = projectConfigManager;
        _logger = logger;
    }

    /// <summary>加载持久化的配置，并自动恢复上次打开的项目。</summary>
    public async Task InitializeAsync()
    {
        var appConfig = await _appConfigManager.LoadAsync();

        _projects.Clear();
        foreach (var info in appConfig.Projects)
        {
            // 跳过目录已不存在的项目
            if (Directory.Exists(info.ProjectPath))
            {
                _projects.Add(info);
            }
            else
            {
                _logger.LogWarning("项目目录不存在，已跳过：{Path}", info.ProjectPath);
            }
        }

        // 恢复上次打开的项目
        var lastName = appConfig.LastOpenedProjectName;
        if (!string.IsNullOrEmpty(lastName))
        {
            var last = _projects.Find(p =>
                string.Equals(p.Name, lastName, StringComparison.OrdinalIgnoreCase));

            if (last != null)
            {
                CurrentProject = last;
                _logger.LogInformation("已恢复上次项目：{Name}", last.Name);
            }
        }
    }

    /// <inheritdoc/>
    public async Task<ProjectInfo> CreateProjectAsync(string name, string workspacePath)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("项目名称不能为空", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(workspacePath))
        {
            throw new ArgumentException("工作区路径不能为空", nameof(workspacePath));
        }

        var projectDir = Path.Combine(workspacePath, name);

        if (Directory.Exists(projectDir))
        {
            throw new InvalidOperationException($"目录已存在：{projectDir}");
        }

        _logger.LogInformation("创建新项目 '{Name}' 在 {Dir}", name, projectDir);

        // 1. 创建目录结构
        Directory.CreateDirectory(projectDir);
        var datasDir = Path.Combine(projectDir, "Datas");
        Directory.CreateDirectory(datasDir);
        Directory.CreateDirectory(Path.Combine(projectDir, "Defines"));

        // 2. 生成初始 luban.conf
        var lubanConf = BuildInitialLubanConf(name);
        await File.WriteAllTextAsync(Path.Combine(projectDir, "luban.conf"), lubanConf, Encoding.UTF8);

        // 3. 生成三个元数据 xlsx（使用 Luban 标准格式）
        ExcelWriter.CreateTablesMetaXlsx(Path.Combine(datasDir, "__tables__.xlsx"));
        ExcelWriter.CreateEnumsMetaXlsx(Path.Combine(datasDir, "__enums__.xlsx"));
        ExcelWriter.CreateBeansMetaXlsx(Path.Combine(datasDir, "__beans__.xlsx"));

        // 4. 生成初始 projectConfig.json
        await _projectConfigManager.SaveAsync(projectDir, new ProjectConfig());

        // 5. 注册到全局配置
        var info = new ProjectInfo
        {
            Name = name,
            WorkspaceRoot = workspacePath,
            LastOpenedAt = DateTime.Now,
        };

        _projects.Add(info);
        await _appConfigManager.AddOrUpdateProjectAsync(info);
        await SwitchToAsync(info);

        _logger.LogInformation("项目 '{Name}' 创建成功", name);
        return info;
    }

    /// <inheritdoc/>
    public async Task<ProjectInfo> OpenProjectAsync(string projectPath)
    {
        if (!Directory.Exists(projectPath))
        {
            throw new DirectoryNotFoundException($"项目目录不存在：{projectPath}");
        }

        var name = Path.GetFileName(projectPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        var workspaceRoot = Path.GetDirectoryName(projectPath)
            ?? throw new InvalidOperationException("无法获取工作区根目录");

        var info = new ProjectInfo
        {
            Name = name,
            WorkspaceRoot = workspaceRoot,
            LastOpenedAt = DateTime.Now,
        };

        // 注册或更新
        var existing = _projects.FindIndex(p =>
            string.Equals(p.ProjectPath, projectPath, StringComparison.OrdinalIgnoreCase));

        if (existing >= 0)
        {
            _projects[existing] = info;
        }
        else
        {
            _projects.Add(info);
        }

        await _appConfigManager.AddOrUpdateProjectAsync(info);
        await SwitchToAsync(info);

        _logger.LogInformation("已打开项目：{Name}", name);
        return info;
    }

    /// <inheritdoc/>
    public async Task SwitchProjectAsync(string projectName)
    {
        var info = _projects.Find(p =>
            string.Equals(p.Name, projectName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"找不到项目：{projectName}");

        await SwitchToAsync(info);
    }

    /// <inheritdoc/>
    public async Task RemoveProjectAsync(string projectName)
    {
        _projects.RemoveAll(p =>
            string.Equals(p.Name, projectName, StringComparison.OrdinalIgnoreCase));
        await _appConfigManager.RemoveProjectAsync(projectName);

        if (string.Equals(CurrentProject?.Name, projectName, StringComparison.OrdinalIgnoreCase))
        {
            CurrentProject = null;
            CurrentProjectChanged?.Invoke(this, null);
        }
    }

    // ── 私有辅助 ──────────────────────────────────────────────────────────────

    private async Task SwitchToAsync(ProjectInfo info)
    {
        CurrentProject = info;
        info.LastOpenedAt = DateTime.Now;
        await _appConfigManager.SetLastOpenedAsync(info.Name);
        await _appConfigManager.AddOrUpdateProjectAsync(info);
        CurrentProjectChanged?.Invoke(this, info);
        _logger.LogInformation("已切换到项目：{Name}", info.Name);
    }

    /// <summary>生成最小可用的初始 luban.conf（JSON 格式）。</summary>
    private static string BuildInitialLubanConf(string projectName)
    {
        return $$"""
{
  "groups": [
    { "names": ["c"], "default": true },
    { "names": ["s"], "default": true }
  ],
  "schemaFiles": [
    { "fileName": "Datas/__tables__.xlsx", "type": "table" },
    { "fileName": "Datas/__beans__.xlsx",  "type": "bean" },
    { "fileName": "Datas/__enums__.xlsx",  "type": "enum" }
  ],
  "dataDir": "Datas",
  "targets": [
    {
      "name": "all",
      "manager": "Tables",
      "groups": ["c", "s"],
      "topModule": "cfg"
    }
  ]
}
""";
    }
}

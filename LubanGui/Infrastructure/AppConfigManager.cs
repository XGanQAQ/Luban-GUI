using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LubanGui.Models;
using Microsoft.Extensions.Logging;

namespace LubanGui.Infrastructure;

/// <summary>
/// 管理全局应用配置（已注册项目列表、上次打开的项目）。
/// 存储位置：%LOCALAPPDATA%\LubanGui\appConfig.json
/// </summary>
public class AppConfigManager
{
    private static readonly string ConfigDir =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LubanGui");

    private static readonly string ConfigPath = Path.Combine(ConfigDir, "appConfig.json");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly ILogger<AppConfigManager> _logger;
    private AppConfig _cache = new();

    public AppConfigManager(ILogger<AppConfigManager> logger)
    {
        _logger = logger;
    }

    /// <summary>从磁盘加载配置；若文件不存在则返回默认值。</summary>
    public async Task<AppConfig> LoadAsync()
    {
        if (!File.Exists(ConfigPath))
        {
            _logger.LogInformation("appConfig.json 不存在，使用默认配置");
            _cache = new AppConfig();
            return _cache;
        }

        try
        {
            await using var stream = File.OpenRead(ConfigPath);
            _cache = await JsonSerializer.DeserializeAsync<AppConfig>(stream, JsonOptions) ?? new AppConfig();
            _logger.LogInformation("已加载 appConfig.json，共 {Count} 个项目", _cache.Projects.Count);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "读取 appConfig.json 失败，使用默认配置");
            _cache = new AppConfig();
        }

        return _cache;
    }

    /// <summary>将当前配置缓存持久化到磁盘。</summary>
    public async Task SaveAsync()
    {
        try
        {
            Directory.CreateDirectory(ConfigDir);
            await using var stream = File.Create(ConfigPath);
            await JsonSerializer.SerializeAsync(stream, _cache, JsonOptions);
            _logger.LogDebug("appConfig.json 已保存");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存 appConfig.json 失败");
        }
    }

    /// <summary>获取已注册的所有项目。</summary>
    public IReadOnlyList<ProjectInfo> GetProjects() => _cache.Projects.AsReadOnly();

    /// <summary>获取上次打开的项目名称（可能为空字符串）。</summary>
    public string GetLastOpenedProjectName() => _cache.LastOpenedProjectName;

    /// <summary>添加或更新一个项目记录，并保存。</summary>
    public async Task AddOrUpdateProjectAsync(ProjectInfo info)
    {
        var existing = _cache.Projects.FindIndex(p =>
            string.Equals(p.Name, info.Name, StringComparison.OrdinalIgnoreCase));

        if (existing >= 0)
        {
            _cache.Projects[existing] = info;
        }
        else
        {
            _cache.Projects.Add(info);
        }

        await SaveAsync();
    }

    /// <summary>从列表中移除指定项目记录，并保存。</summary>
    public async Task RemoveProjectAsync(string projectName)
    {
        _cache.Projects.RemoveAll(p =>
            string.Equals(p.Name, projectName, StringComparison.OrdinalIgnoreCase));
        await SaveAsync();
    }

    /// <summary>记录上次打开的项目，并保存。</summary>
    public async Task SetLastOpenedAsync(string projectName)
    {
        _cache.LastOpenedProjectName = projectName;
        await SaveAsync();
    }

    /// <summary>获取自定义的 Luban DLL 路径（空字符串表示使用内置默认）。</summary>
    public string GetLubanDllPath() => _cache.LubanDllPath;

    /// <summary>获取"删除表格时是否默认同时删除物理文件"的配置。</summary>
    public bool GetDeleteTablePhysicalFileByDefault() => _cache.DeleteTablePhysicalFileByDefault;

    /// <summary>保存"删除表格默认策略"设置。</summary>
    public async Task SetDeleteTablePhysicalFileByDefaultAsync(bool value)
    {
        _cache.DeleteTablePhysicalFileByDefault = value;
        await SaveAsync();
    }
}

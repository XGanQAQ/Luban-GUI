using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LubanGui.Models;
using Microsoft.Extensions.Logging;

namespace LubanGui.Infrastructure;

/// <summary>
/// 管理单个项目的 projectConfig.json 读写。
/// 存储位置：&lt;ProjectPath&gt;/projectConfig.json
/// </summary>
public class ProjectConfigManager
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly ILogger<ProjectConfigManager> _logger;

    public ProjectConfigManager(ILogger<ProjectConfigManager> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 从指定项目目录加载 projectConfig.json；若不存在则返回默认配置。
    /// </summary>
    public async Task<ProjectConfig> LoadAsync(string projectDir)
    {
        var path = Path.Combine(projectDir, "projectConfig.json");
        if (!File.Exists(path))
        {
            _logger.LogInformation("项目 {Dir} 的 projectConfig.json 不存在，返回默认配置", projectDir);
            return new ProjectConfig();
        }

        try
        {
            await using var stream = File.OpenRead(path);
            var config = await JsonSerializer.DeserializeAsync<ProjectConfig>(stream, JsonOptions);
            return config ?? new ProjectConfig();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "读取 {Path} 失败，返回默认配置", path);
            return new ProjectConfig();
        }
    }

    /// <summary>将 ProjectConfig 持久化到指定项目目录下的 projectConfig.json。</summary>
    public async Task SaveAsync(string projectDir, ProjectConfig config)
    {
        var path = Path.Combine(projectDir, "projectConfig.json");
        try
        {
            Directory.CreateDirectory(projectDir);
            await using var stream = File.Create(path);
            await JsonSerializer.SerializeAsync(stream, config, JsonOptions);
            _logger.LogDebug("projectConfig.json 已保存到 {Path}", path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存 {Path} 失败", path);
            throw;
        }
    }
}

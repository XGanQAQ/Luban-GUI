using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LubanGui.LubanAdapter.Interfaces;
using Microsoft.Extensions.Logging;

namespace LubanGui.LubanAdapter;

/// <summary>
/// 读写 luban.conf（JSON with comments / trailing commas 格式）。
/// </summary>
public class LubanConfAdapter : ILubanConfAdapter
{
    private readonly ILogger<LubanConfAdapter> _logger;

    public LubanConfAdapter(ILogger<LubanConfAdapter> logger)
    {
        _logger = logger;
    }

    public Task<LubanConfDto> ReadAsync(string confPath)
    {
        return Task.Run(() =>
        {
            if (!File.Exists(confPath))
                throw new FileNotFoundException($"luban.conf 不存在：{confPath}");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
            };
            var raw = JsonSerializer.Deserialize<RawConf>(
                File.ReadAllText(confPath, Encoding.UTF8), options)
                ?? throw new InvalidOperationException("解析 luban.conf 返回 null");

            // 从 targets 列表提取所有目标名（如 all、client、server）
            var targetNames = raw.Targets?
                .Where(t => !string.IsNullOrEmpty(t.Name))
                .Select(t => t.Name!)
                .ToList() ?? [];

            // 从 targets 列表提取 codeTargets / dataTargets / topModule
            var codeTargets = raw.Targets?
                .Where(t => !string.IsNullOrEmpty(t.Manager) && t.Manager != "Tables")
                .Select(t => t.Name ?? string.Empty).ToList() ?? [];
            var dataTargets = raw.Targets?
                .Where(t => t.Manager == "Tables")
                .Select(t => t.Name ?? string.Empty).ToList() ?? [];
            string topModule = raw.Targets?.FirstOrDefault()?.TopModule ?? string.Empty;

            // 从 groups 列表提取分组名
            var groups = raw.Groups?
                .Where(g => g.Names?.Count > 0)
                .ToDictionary(g => g.Names![0], g => string.Join(",", g.Names!))
                ?? new Dictionary<string, string>();

            return new LubanConfDto(
                Target: raw.Targets?.FirstOrDefault()?.Name ?? string.Empty,
                CodeTargets: codeTargets,
                DataTargets: dataTargets,
                TopModule: topModule,
                InputDataDirs: string.IsNullOrEmpty(raw.DataDir) ? [] : [raw.DataDir],
                Groups: groups,
                TargetNames: targetNames
            );
        });
    }

    public Task WriteAsync(string confPath, LubanConfDto dto)
    {
        _logger.LogInformation("写入 luban.conf：{Path}", confPath);
        // TODO: 实现写回（目前 MVP 阶段仅支持读取）
        return Task.CompletedTask;
    }

    public Task CreateDefaultAsync(string confPath)
    {
        return Task.Run(() =>
        {
            string dir = Path.GetDirectoryName(confPath) ?? ".";
            Directory.CreateDirectory(dir);

            const string template = """
{
    "groups": [
        { "names": ["c"], "default": true },
        { "names": ["s"], "default": true }
    ],
    "schemaFiles": [
        { "fileName": "Datas/__tables__.xlsx", "type": "table" },
        { "fileName": "Datas/__enums__.xlsx",  "type": "enum" },
        { "fileName": "Datas/__beans__.xlsx",  "type": "bean" }
    ],
    "dataDir": "Datas",
    "targets": [
        { "name": "server", "manager": "Tables", "groups": ["s"], "topModule": "" }
    ]
}
""";
            File.WriteAllText(confPath, template, Encoding.UTF8);
            _logger.LogInformation("已生成默认 luban.conf：{Path}", confPath);
        });
    }

    // ── 内部 JSON 模型 ─────────────────────────────────────────────────────

    private class RawConf
    {
        [JsonPropertyName("groups")]
        public List<RawGroup>? Groups { get; set; }

        [JsonPropertyName("schemaFiles")]
        public List<RawSchemaFile>? SchemaFiles { get; set; }

        [JsonPropertyName("dataDir")]
        public string? DataDir { get; set; }

        [JsonPropertyName("targets")]
        public List<RawTarget>? Targets { get; set; }
    }

    private class RawGroup
    {
        [JsonPropertyName("names")]
        public List<string>? Names { get; set; }

        [JsonPropertyName("default")]
        public bool Default { get; set; }
    }

    private class RawSchemaFile
    {
        [JsonPropertyName("fileName")]
        public string? FileName { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }

    private class RawTarget
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("manager")]
        public string? Manager { get; set; }

        [JsonPropertyName("groups")]
        public List<string>? Groups { get; set; }

        [JsonPropertyName("topModule")]
        public string? TopModule { get; set; }
    }
}

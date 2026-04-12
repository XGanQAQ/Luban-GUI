using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luban;
using Luban.RawDefs;
using Luban.Schema;
using LubanGui.LubanAdapter.Dtos;
using LubanGui.LubanAdapter.Interfaces;
using Microsoft.Extensions.Logging;

namespace LubanGui.LubanAdapter;

/// <summary>
/// 使用 Luban 原生解析管线读取 Schema。
/// 通过 GlobalConfigLoader + DefaultSchemaCollector 读取 luban.conf 及全部 Schema 文件。
/// </summary>
public class LubanSchemaReader : ILubanSchemaReader
{
    private readonly ILogger<LubanSchemaReader> _logger;

    // 保护 EnvManager.Current 与 GenerationContext.GlobalConf 两个全局静态状态
    private static readonly object _syncLock = new();

    public LubanSchemaReader(ILogger<LubanSchemaReader> logger)
    {
        _logger = logger;
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public async Task<IReadOnlyList<TableSchemaDto>> ReadTablesAsync(string confPath)
    {
        var asm = await LoadRawAssemblyAsync(confPath);
        return asm?.Tables.Select(ToDto).ToList() ?? [];
    }

    public async Task<IReadOnlyList<EnumSchemaDto>> ReadEnumsAsync(string confPath)
    {
        var asm = await LoadRawAssemblyAsync(confPath);
        return asm?.Enums.Select(ToDto).ToList() ?? [];
    }

    public async Task<IReadOnlyList<BeanSchemaDto>> ReadBeansAsync(string confPath)
    {
        var asm = await LoadRawAssemblyAsync(confPath);
        return asm?.Beans.Select(ToDto).ToList() ?? [];
    }

    // ── 核心：加载 RawAssembly ────────────────────────────────────────────

    private Task<RawAssembly?> LoadRawAssemblyAsync(string confPath)
    {
        return Task.Run(() =>
        {
            if (!System.IO.File.Exists(confPath))
            {
                _logger.LogWarning("luban.conf 不存在，跳过 Schema 读取：{Path}", confPath);
                return null;
            }

            try
            {
                lock (_syncLock)
                {
                    var configLoader = new GlobalConfigLoader();
                    var config = configLoader.Load(confPath);

                    // DefaultSchemaCollector.LoadTableValueTypeSchemasFromFile()
                    // 会调用 GenerationContext.GetInputDataPath()，
                    // 该方法读取 GenerationContext.GlobalConf.InputDataDir。
                    GenerationContext.GlobalConf = config;
                    EnvManager.Current = new EnvManager(new Dictionary<string, string>());

                    ISchemaCollector collector = SchemaManager.Ins.CreateSchemaCollector("default");
                    collector.Load(config);
                    return collector.CreateRawAssembly();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "读取 Luban Schema 失败：{Path}", confPath);
                return null;
            }
        });
    }

    // ── DTO 映射 ──────────────────────────────────────────────────────────

    private static TableSchemaDto ToDto(RawTable t) => new(
        FullName: string.IsNullOrEmpty(t.Namespace) ? t.Name : $"{t.Namespace}.{t.Name}",
        Index: t.Index ?? string.Empty,
        ValueType: t.ValueType ?? string.Empty,
        Mode: t.Mode.ToString(),
        Comment: t.Comment ?? string.Empty,
        InputFiles: t.InputFiles.AsReadOnly(),
        OutputFile: t.OutputFile ?? string.Empty,
        Groups: t.Groups.AsReadOnly(),
        ReadSchemaFromFile: t.ReadSchemaFromFile
    );

    private static EnumSchemaDto ToDto(RawEnum e) => new(
        FullName: e.FullName,
        IsFlags: e.IsFlags,
        Comment: e.Comment ?? string.Empty,
        Items: e.Items.Select(i => new EnumItemDto(
            i.Name,
            i.Alias ?? string.Empty,
            i.Value ?? string.Empty,
            i.Comment ?? string.Empty
        )).ToList()
    );

    private static BeanSchemaDto ToDto(RawBean b) => new(
        FullName: b.FullName,
        Parent: b.Parent ?? string.Empty,
        IsValueType: b.IsValueType,
        Comment: b.Comment ?? string.Empty,
        Sep: b.Sep ?? string.Empty,
        Fields: (b.Fields ?? []).Select(f => new FieldDto(
            f.Name,
            f.Alias ?? string.Empty,
            f.Type ?? string.Empty,
            f.Comment ?? string.Empty,
            (f.Groups ?? []).AsReadOnly(),
            (f.Tags ?? new Dictionary<string, string>()).AsReadOnly()
        )).ToList()
    );
}

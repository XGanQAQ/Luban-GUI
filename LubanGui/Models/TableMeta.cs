namespace LubanGui.Models;

/// <summary>
/// 从 __tables__.xlsx 中读取的单张表格元数据，字段名与 Luban ExcelSchemaLoader 保持一致。
/// </summary>
public class TableMeta
{
    /// <summary>表格全限定名，如 "cfg.ItemConfig"。</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>值类型（Bean 类型名），如 "cfg.ItemRecord"。read_schema_from_file=true 时可与 FullName 相同。</summary>
    public string ValueType { get; set; } = string.Empty;

    /// <summary>索引字段名，如 "id"。</summary>
    public string Index { get; set; } = string.Empty;

    /// <summary>表格模式（rows / map / list / one），通常为空表示默认。</summary>
    public string Mode { get; set; } = string.Empty;

    /// <summary>所属分组，如 "c,s"。</summary>
    public string Group { get; set; } = string.Empty;

    /// <summary>注释说明。</summary>
    public string Comment { get; set; } = string.Empty;

    /// <summary>是否从数据文件自动读取字段 Schema（true 时无需在 __beans__.xlsx 中定义 Bean）。</summary>
    public bool ReadSchemaFromFile { get; set; }

    /// <summary>
    /// 对应 Excel 文件的相对路径（相对于项目 Datas/ 目录），如 "cfg/ItemConfig.xlsx"。
    /// 对应 Luban __tables__.xlsx 中的 input 列。
    /// </summary>
    public string Input { get; set; } = string.Empty;

    /// <summary>输出路径（通常为空，由导出配置决定）。</summary>
    public string Output { get; set; } = string.Empty;

    /// <summary>标签，如 "tag1,tag2"。</summary>
    public string Tags { get; set; } = string.Empty;

    // ── 兼容旧代码 ──────────────────────────────────────────────────────────

    /// <summary>等同于 Input，保留供旧代码引用。</summary>
    public string FilePath
    {
        get => Input;
        set => Input = value;
    }

    /// <summary>简短的表名（FullName 最后一段），用于 UI 展示。</summary>
    public string DisplayName => FullName.Contains('.') ? FullName[(FullName.LastIndexOf('.') + 1)..] : FullName;

    /// <summary>上次导表是否成功（用于状态图标显示）。</summary>
    public bool? LastExportSuccess { get; set; }
}

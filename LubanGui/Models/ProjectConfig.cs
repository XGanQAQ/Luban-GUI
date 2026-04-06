namespace LubanGui.Models;

/// <summary>
/// 用户通过导出配置窗口填写的导出配置，保存在项目目录下的 projectConfig.json。
/// </summary>
public class ProjectConfig
{
    /// <summary>数据导出配置。</summary>
    public DataOutputConfig DataOutput { get; set; } = new();

    /// <summary>代码导出配置（可选）。</summary>
    public CodeOutputConfig CodeOutput { get; set; } = new();

    /// <summary>
    /// 导出目标，对应 luban.conf 中定义的 targets，如 "all"、"client"、"server"。
    /// </summary>
    public string Target { get; set; } = "all";
}

/// <summary>数据序列化导出配置。</summary>
public class DataOutputConfig
{
    /// <summary>数据序列化格式，如 "bin"、"json"、"lua"。</summary>
    public string Type { get; set; } = "bin";

    /// <summary>导出数据文件的目标目录。</summary>
    public string OutputPath { get; set; } = string.Empty;
}

/// <summary>代码生成导出配置。</summary>
public class CodeOutputConfig
{
    /// <summary>是否生成代码文件。</summary>
    public bool Enabled { get; set; } = false;

    /// <summary>代码目标，如 "cs-bin"、"typescript-json"、"java-bin"。</summary>
    public string Type { get; set; } = "cs-bin";

    /// <summary>生成代码文件的目标目录。</summary>
    public string OutputPath { get; set; } = string.Empty;

    /// <summary>生成代码的顶层命名空间，如 "cfg"。</summary>
    public string TopModule { get; set; } = "cfg";
}

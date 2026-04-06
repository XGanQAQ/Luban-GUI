using System;
using System.IO;

namespace LubanGui.Models;

/// <summary>
/// 项目元信息，保存在全局 appConfig.json 的项目列表中。
/// </summary>
public class ProjectInfo
{
    /// <summary>项目名称（同时也是工作区下的子目录名）。</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>工作区根目录（包含项目子目录的父目录）。</summary>
    public string WorkspaceRoot { get; set; } = string.Empty;

    /// <summary>完整的项目目录路径 = WorkspaceRoot / Name。</summary>
    public string ProjectPath => Path.Combine(WorkspaceRoot, Name);

    /// <summary>最后打开时间（用于排序）。</summary>
    public DateTime LastOpenedAt { get; set; }
}

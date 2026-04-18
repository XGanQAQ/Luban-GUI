using System.Collections.Generic;

namespace LubanGui.Models;

/// <summary>
/// 全局应用配置，保存在 %LOCALAPPDATA%\LubanGui\appConfig.json。
/// </summary>
public class AppConfig
{
    /// <summary>已注册的所有项目信息列表。</summary>
    public List<ProjectInfo> Projects { get; set; } = new();

    /// <summary>上次打开的项目名称；空字符串表示无上次记录。</summary>
    public string LastOpenedProjectName { get; set; } = string.Empty;

    /// <summary>
    /// Luban.dll 的自定义路径。
    /// 为空时自动使用 GUI 内置的 <c>luban\Luban.dll</c>（位于应用程序目录下）。
    /// </summary>
    public string LubanDllPath { get; set; } = string.Empty;

    /// <summary>
    /// 删除表格时的默认策略：true = 同时删除物理 xlsx 文件；false = 仅移除注册（保留文件）。
    /// </summary>
    public bool DeleteTablePhysicalFileByDefault { get; set; } = false;
}

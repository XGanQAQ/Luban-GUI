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
}

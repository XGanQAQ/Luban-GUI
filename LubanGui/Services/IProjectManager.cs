using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LubanGui.Models;

namespace LubanGui.Services;

/// <summary>
/// 管理多个 Luban 项目的创建、切换与加载。
/// </summary>
public interface IProjectManager
{
    /// <summary>所有已注册的项目列表（按最近打开时间降序）。</summary>
    IReadOnlyList<ProjectInfo> Projects { get; }

    /// <summary>当前打开的项目；若未打开任何项目则为 null。</summary>
    ProjectInfo? CurrentProject { get; }

    /// <summary>当前项目切换后触发（新项目信息，或 null 表示无项目）。</summary>
    event EventHandler<ProjectInfo?> CurrentProjectChanged;

    /// <summary>
    /// 在 <paramref name="workspacePath"/> 下创建名为 <paramref name="name"/> 的新项目，
    /// 并初始化完整的工作区文件结构。
    /// </summary>
    Task<ProjectInfo> CreateProjectAsync(string name, string workspacePath);

    /// <summary>
    /// 打开 <paramref name="projectPath"/> 指向的已有项目目录，
    /// 注册到项目列表，并设为当前项目。
    /// </summary>
    Task<ProjectInfo> OpenProjectAsync(string projectPath);

    /// <summary>切换当前项目，通知 UI 刷新。</summary>
    Task SwitchProjectAsync(string projectName);

    /// <summary>从列表中移除项目记录（不删除磁盘文件）。</summary>
    Task RemoveProjectAsync(string projectName);

    /// <summary>加载持久化的配置，并自动恢复上次打开的项目。</summary>
    Task InitializeAsync();
}

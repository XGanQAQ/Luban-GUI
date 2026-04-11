# ProjectManager

**所属层次**: 业务逻辑层 (Service)

---

## 职责

管理多项目的生命周期：新建、打开、切换、删除项目，维护当前激活项目状态，协调 `AppConfigManager` 和 `ProjectConfigManager` 完成项目元数据持久化。

---

## 接口定义

```csharp
public interface IProjectManager
{
    IReadOnlyList<ProjectInfo> Projects { get; }
    ProjectInfo?               CurrentProject { get; }

    Task<ProjectInfo> CreateProjectAsync(string name, string rootPath);
    Task<ProjectInfo> OpenProjectAsync(string rootPath);
    Task             SwitchProjectAsync(ProjectInfo project);
    void             RemoveProject(ProjectInfo project);
}
```

---

## 工作区初始化流程

1. 读取 `AppConfig`（通过 `AppConfigManager.LoadAsync`）。
2. 将 `AppConfig.RecentProjects` 反序列化为 `ProjectInfo` 列表。
3. 按最近使用时间排序，更新 `Projects` 属性。
4. 若列表非空，自动激活第一个项目（`SwitchProjectAsync`）。
5. 等待激活完成后通知表现层刷新视图。

---

## 层间约定

- 向下依赖 `AppConfigManager`（基础设施层）和 `ProjectConfigManager`（基础设施层）。
- 只暴露 `IProjectManager` 接口，不允许上层直接构造 `ProjectManager` 实例。
- 所有 `Task` 方法需保证在任意线程调用时线程安全。

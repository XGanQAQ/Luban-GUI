# ExportService

**所属层次**: 业务逻辑层 (Service)

---

## 职责

驱动完整导表流程：校验配置 → 构建 CLI 命令 → 调用 `LubanExecutor` 执行 → 汇总结果，支持进度上报和取消操作。

---

## 接口定义

```csharp
public interface IExportService
{
    Task<ExportResult> ExportAsync(
        ProjectConfig       config,
        IProgress<string>   progress,
        CancellationToken   ct);

    IReadOnlyList<string> ValidateConfig(ProjectConfig config);
}
```

---

## 导表流程

1. 调用 `ValidateConfig` 检查配置合法性；若有错则直接返回失败的 `ExportResult`。
2. 通过 `LubanCommandBuilder` 将 `ProjectConfig` 转换为 CLI 参数列表。
3. 调用 `LubanExecutor.RunAsync`，传入 `progress` 和 `ct`。
4. 实时将 stdout / stderr 行推入 `LogWindowViewModel` 的日志队列。
5. 等待进程退出，根据退出码构建 `ExportResult`。
6. 若 `ct` 被取消，强制终止子进程并返回取消状态。

---

## 异常处理

| 异常类型 | 处理方式 |
|----------|----------|
| `OperationCanceledException` | 终止子进程，返回 `ExportStatus.Cancelled` |
| 配置校验失败 | 返回 `ExportStatus.Failed` + 错误消息列表 |
| 进程启动失败 | 捕获 `Win32Exception`，返回 `ExportStatus.Failed` |
| 其他异常 | 记录日志后重新抛出 |

---

## 层间约定

- 向下依赖 `LubanExecutor` 和 `LubanCommandBuilder`（工具层）。
- 向下依赖 `ProjectConfigManager`（基础设施层）读取当前配置。
- 只暴露 `IExportService` 接口。
- `ExportAsync` 必须在非 UI 线程中执行，进度回调由 `IProgress<T>` 自动切换线程。

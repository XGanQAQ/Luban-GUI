# Logger

**所属层次**: 基础设施层 (Infrastructure)

---

## 职责

为整个应用提供结构化日志能力，通过 Serilog 将日志同时输出到控制台（调试用）和滚动文件（生产用），并将日志条目推送到 `LogWindowViewModel` 供界面展示。

---

## 初始化代码

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File(
        path:            Path.Combine(logDir, "luban-gui-.log"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7)
    .WriteTo.Sink(new LogWindowSink(logWindowViewModel))
    .CreateLogger();
```

---

## Sink 说明

| Sink | 输出位置 | 用途 |
|------|----------|------|
| `Console` | 标准输出 | 开发调试 |
| `File` | `%LOCALAPPDATA%\LubanGui\logs\luban-gui-<date>.log` | 问题排查，保留 7 天 |
| `LogWindowSink` | `LogWindowViewModel.LogItems` | 实时界面展示 |

---

## 日志级别约定

| 级别 | 使用场景 |
|------|----------|
| `Debug` | 内部调试信息，发布版可关闭 |
| `Information` | 正常流程里程碑（项目加载、导表开始/完成） |
| `Warning` | 可恢复的异常状态（配置文件不存在、使用默认值） |
| `Error` | 导致功能失败的错误，需要用户感知 |

---

## 层间约定

- Serilog 作为唯一日志框架，通过 `ILogger` 接口（Microsoft.Extensions.Logging）注入，隔离框架耦合。
- `LogWindowSink` 推送日志到 ViewModel 前，需切换到 Avalonia UI 线程。
- 应用退出时调用 `Log.CloseAndFlush()` 确保日志落盘。

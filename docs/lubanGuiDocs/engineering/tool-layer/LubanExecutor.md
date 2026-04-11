# LubanExecutor

**所属层次**: 工具层 (Tool)

---

## 职责

启动和管理 Luban CLI 子进程，实时收集 stdout / stderr 输出，并安全终止进程（支持 `CancellationToken`）。

---

## 进程启动配置

```csharp
var psi = new ProcessStartInfo
{
    FileName               = lubanExePath,           // bin/luban/Luban.dll 或 Luban.exe
    Arguments              = args,
    RedirectStandardOutput = true,
    RedirectStandardError  = true,
    UseShellExecute        = false,
    CreateNoWindow         = true,
};
```

---

## 死锁防护

stdout 和 stderr 必须**并发**读取，避免其中一路缓冲区满导致子进程挂起：

```csharp
var stdoutTask = CollectStreamAsync(process.StandardOutput);
var stderrTask = CollectStreamAsync(process.StandardError);
await Task.WhenAll(stdoutTask, stderrTask);
```

---

## 输出推送流程

```
子进程输出行
    ↓ ReadLineAsync（异步）
IProgress<string>.Report(line)
    ↓ 切换到 UI 线程
LogWindowViewModel.LogItems.Add(entry)
```

---

## 层间约定

- 只依赖 `System.Diagnostics.Process`，不引用任何 Service 接口。
- `RunAsync` 接受 `IProgress<string>` 和 `CancellationToken`，不直接操作 UI。
- 当 `CancellationToken` 触发时，调用 `process.Kill(entireProcessTree: true)` 后再 `await process.WaitForExitAsync()`。

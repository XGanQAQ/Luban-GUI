# Luban-GUI — AGENTS.md

## 仓库结构

两个独立代码库共存于一个仓库：
- `LubanGui/` — Avalonia 桌面前端（C# .NET 8, AXAML + MVVM）
- `lubanSrc/` — 完整 Luban CLI 工具链源码（**不要修改**，只读引用）

GUI 通过 LubanAdapter 层（`LubanGui/LubanAdapter/`）在进程内调用 `Luban.Core`、`Luban.Schema.Builtin`、`Luban.DataLoader.Builtin`，同时 MSBuild 构建时自动编译 `lubanSrc/Luban/Luban.csproj` 并将 CLI 产物复制到 `LubanGui/bin/.../luban/` 作为子进程调用。

## 关键命令

```powershell
# 构建（含内嵌 Luban CLI）
dotnet build .\LubanGui.slnx

# 运行 GUI
dotnet run --project .\LubanGui\LubanGui.csproj

# 发布 Windows x64 自包含
dotnet publish .\LubanGui\LubanGui.csproj -c Release -r win-x64 --self-contained true -o .\publish\win-x64

# 仅构建 Luban 工具链
dotnet build .\lubanSrc\Luban.sln

# 直接运行 Luban CLI（调试用）
dotnet run --project .\lubanSrc\Luban\Luban.csproj -- --conf <路径> --target <目标>
```

无测试项目，无 lint/typecheck 命令。

## 架构（五层，单向向下依赖）

```
Views (AXAML) → ViewModels → Services → Infrastructure + LubanAdapter
```

DI 注册在 `App.axaml.cs:ConfigureServices`，所有服务均为 `AddSingleton`。

- `App.axaml.cs:OnFrameworkInitializationCompleted` 中 `MainWindow.Opened` 事件里异步调用 `projectManager.InitializeAsync()` 恢复上次项目。
- `LubanAdapterInitializer.Initialize()` 必须在任何 Luban 类型使用前调用（已在 `App.axaml.cs:Initialize()` 中执行）。

## 关键注意事项

- `lubanSrc/` 不改代码。所有与 Luban 的交互通过 `LubanGui/LubanAdapter/` 中的适配器隔离。
- 设计文档在 `docs/lubanGuiDocs/engineering/`，采用文档驱动开发，**代码与文档冲突时以文档为准**。
- `LubanGui/luban/` 是构建产物（运行时快照），不要提交到 git。
- 构建有 NU1902/NU1903/NU1904 警告（`lubanSrc` 依赖 Scriban 5.12.0），不影响运行，发布时需在 release note 标注。
- 代码风格：Allman 大括号，强制大括号，最大行长 180（见 `lubanSrc/.editorconfig`）。
- 项目配置存储：全局注册表 `%LOCALAPPDATA%/LubanGui/appConfig.json`，项目配置 `<项目路径>/projectConfig.json`。
- 日志文件：`logs/luban-gui-<日期>.txt`（Serilog 滚动写入）。

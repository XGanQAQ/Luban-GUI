# Luban-GUI Copilot 指引

## 构建与运行

- 在仓库根目录，用 `dotnet build .\LubanGui.slnx` 同时构建 GUI 和内嵌的 Luban CLI。
- 仅构建 Luban 工具链：`dotnet build .\lubanSrc\Luban.sln`。
- 运行 Avalonia GUI：`dotnet run --project .\LubanGui\LubanGui.csproj`。
- 直接运行 Luban CLI：`dotnet run --project .\lubanSrc\Luban\Luban.csproj -- --conf <配置文件> --target <目标> [--codeTarget ...] [--dataTarget ...]`。
- 发布 GUI：`dotnet publish .\LubanGui\LubanGui.csproj -c Release -o .\publish`。
- 仓库中目前没有提交任何测试项目或专用 lint 命令，暂无单测运行命令。
- 构建可以成功，但 `dotnet build .\LubanGui.slnx` 和 `dotnet build .\lubanSrc\Luban.sln` 会因 `lubanSrc\Luban.Core` 依赖 `Scriban 5.12.0` 而产生 `NU1902`/`NU1903`/`NU1904` 警告。

## 整体架构

- 本仓库包含两个独立代码库：
  - `LubanGui\` —— Avalonia 桌面前端。
  - `lubanSrc\` —— 完整的 Luban CLI / 工具链源码。
- 根解决方案 `LubanGui.slnx` 只包含 `LubanGui\LubanGui.csproj`。GUI 不直接引用 Luban 项目；`LubanGui.csproj` 通过 MSBuild Target 在 GUI 构建前先构建 `lubanSrc\Luban\Luban.csproj`，再将 CLI 输出复制到 `LubanGui\bin\<Configuration>\net8.0\luban\`。
- `lubanSrc\Luban\Luban.csproj` 是 CLI 入口，引用了各语言/格式的生成器、Schema、加载器、校验器等 `Luban.*` 项目。入口 `lubanSrc\Luban\Program.cs` 负责加载全局配置、创建 Pipeline 并执行代码生成。监听模式实现于 `lubanSrc\Luban\Utils\DirectoryWatcher.cs`。
- `LubanGui\luban\` 是打包进去的 Luban 运行时快照，应视为构建产物/运行时负载，`lubanSrc\` 才是可编辑的源码。
- `docs\` 中的文档描述了 GUI 的目标架构与路线图（含 `ExportService`、`ConfigManager`、`LubanExecutor`、`ProcessManager` 等服务），是重要的设计背景，但其中大部分结构在 `LubanGui\` 中尚未实现。

## 关键约定

- 当实际行为与设计文档冲突时，以 Luban CLI 源码为准。文档中仍描述了简化参数如 `--input_data_dir`、`--output_dir`、`--codeFormat`，但 `lubanSrc\Luban\Program.cs` 实际暴露的是 `--conf`、`--target`、`--codeTarget`、`--dataTarget`、`--watchDir` 等选项。
- 将 `docs\01-软件设计文档.md`、`docs\02-技术方案文档.md`、`docs\03-开发计划文档.md` 视为规划/设计参考，而非已实现功能的证明。
- 当前 GUI 遵循默认的 Avalonia + CommunityToolkit.Mvvm 模式：
  - `LubanGui\ViewModels\ViewModelBase.cs` 继承 `ObservableObject`。
  - `LubanGui\App.axaml.cs` 在运行时设置 `DataContext`。
  - `LubanGui\Views\*.axaml` 使用 `x:DataType` 进行编译绑定，并设置独立的设计时 `DataContext`。
  - `LubanGui\ViewLocator.cs` 按命名约定将 `*ViewModel` 映射到对应的 `*View`。
- 保留 `LubanGui\App.axaml.cs` 中的校验器设置：它移除了 Avalonia 的 `DataAnnotationsValidationPlugin`，以避免与 CommunityToolkit 产生重复校验。
- GUI 目前仍处于模板级别（`MainWindowViewModel` 仅暴露 `Greeting`），请勿假设 `docs\` 中描述的 Service / DI / 日志等类型已存在于代码中。
- `lubanSrc\.editorconfig` 是 Luban 工具链的主要代码风格来源：Allman 大括号风格、强制使用大括号（`IDE0011 = error`）、最大行长 180。

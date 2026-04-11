# Luban-GUI Copilot 指引

## 项目背景简述

Luban-GUI 是一个基于 Avalonia 的跨平台桌面应用，旨在为 Luban CLI 提供图形化界面，简化配置和执行流程。项目采用 C# 和 .NET 10.0 开发，包含两个独立代码库：`LubanGui\`（GUI 前端）和 `lubanSrc\`（完整的 Luban CLI 源码）。设计文档位于 `docs\lubanGuiDocs\engineering\`，详细描述了架构设计、接口定义、模块职责等。

此项目大部分代码由 Copilot 生成，开发者主要负责设计文档撰写、代码审查和测试验证。Copilot 生成的代码可能不完整或存在逻辑错误，请务必仔细审查并根据设计文档进行修改。

## 文档是第一公民

- 此项目采用文档驱动设计，所有架构设计、接口定义、模块职责等均以 `docs\lubanGuiDocs\engineering\` 中的 Markdown 文档为准。**请勿直接修改代码来改变设计**，而应修改对应的设计文档，确保代码实现与文档保持一致。
- 设计文档中描述的类型、接口、方法等在代码中可能尚未实现，或实现细节可能与文档略有出入。**当实际代码与设计文档冲突时，以设计文档为准**，并更新代码以符合设计。


## 文档目录结构

`docs\` 分为两大部分：

```
docs/
├── lubanDocs/                          # Luban CLI 官方文档（只读参考）
│   ├── intro.md
│   ├── basic/
│   ├── beginner/
│   ├── manual/
│   └── other/
└── lubanGuiDocs/                       # Luban-GUI 项目设计文档
    ├── product-design/
    │   └── 01-产品需求文档.md           # 产品定位、用户故事、功能列表、版本路线图
    ├── engineering/
    │   ├── 01-技术方案.md               # 技术选型、架构说明、接口定义、NuGet 清单
    │   ├── 02-层次划分.md               # 五层架构索引，指向各层文件夹
    │   ├── 03-前端界面设计.md           # 主窗口布局、各对话框 UI 规范
    │   ├── presentation-layer/         # 表现层模块
    │   │   ├── MainWindowViewModel.md
    │   │   ├── TableListViewModel.md
    │   │   ├── TablePreviewViewModel.md
    │   │   ├── ExportConfigViewModel.md
    │   │   └── LogWindowViewModel.md
    │   ├── service-layer/              # 业务逻辑层模块
    │   │   ├── ProjectManager.md
    │   │   ├── SchemaService.md
    │   │   ├── ExportService.md
    │   │   └── TablePreviewService.md
    │   ├── tool-layer/                 # 工具层模块
    │   │   ├── LubanExecutor.md
    │   │   ├── LubanCommandBuilder.md
    │   │   ├── ExcelWriter.md
    │   │   └── FileOpenService.md
    │   ├── infrastructure-layer/       # 基础设施层模块
    │   │   ├── AppConfigManager.md
    │   │   ├── ProjectConfigManager.md
    │   │   └── Logger.md
    │   ├── luban-adapter-layer/        # Luban 源适配层模块（唯一与 lubanSrc 接触点）
    │   │   ├── LubanSchemaReader.md
    │   │   ├── LubanConfAdapter.md
    │   │   └── LubanTypeMapper.md
    │   └── specifications/             # 数据格式规范
    │       ├── 01-Excel数据文件规范.md
    │       └── 02-元数据表规范.md
    └── project-management/
        └── 01-开发计划.md              # 版本规划、里程碑、风险表
```

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
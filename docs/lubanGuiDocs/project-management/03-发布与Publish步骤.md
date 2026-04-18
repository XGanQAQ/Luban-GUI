# Luban-GUI 发布与 Publish 步骤

版本: 1.0  
日期: 2026年4月  
适用范围: Luban-GUI Windows 发布

---

## 一、目标

本手册用于规范 Luban-GUI 的发布流程，确保每次发布都满足以下要求:

1. 可重复构建，命令固定。
2. 可执行文件可独立运行。
3. 内置 Luban 运行时目录完整可用。
4. 发布说明与版本信息一致。

---

## 二、前置条件

1. 已安装 .NET SDK 8 或更高版本。
2. 在仓库根目录执行命令。
3. 代码已完成主流程验证:
- 新建项目
- 创建表格
- 配置导出
- 一键导表
4. 发布前已确认文档状态:
- README 已更新
- CHANGELOG 已更新
- 发布版本号和对外口径已统一

---

## 三、标准发布命令

### 1. 先做完整构建检查

dotnet build .\LubanGui.slnx

### 2. 生成 Windows x64 自包含发布产物

dotnet publish .\LubanGui\LubanGui.csproj -c Release -r win-x64 --self-contained true -o .\publish\win-x64

### 3. 可选: 生成单文件版本

dotnet publish .\LubanGui\LubanGui.csproj -c Release -r win-x64 --self-contained true -o .\publish\win-x64-single /p:PublishSingleFile=true /p:DebugType=None /p:DebugSymbols=false

说明:

1. 当前工程会在 GUI 构建前自动构建 lubanSrc\Luban。
2. 构建后会自动把 Luban 输出复制到发布目录中的 luban 子目录。
3. 已支持 RuntimeIdentifier 透传，发布命令中带 -r 时可正确构建对应 RID 的 Luban 产物。

---

## 四、发布流程清单

### 步骤 1: 版本与文档冻结

1. 确认发布版本号。
2. 确认 README、CHANGELOG、发布说明一致。
3. 确认已知问题描述真实，不夸大已完成功能。

### 步骤 2: 执行 Publish

1. 执行标准发布命令。
2. 记录发布日志和耗时。
3. 若失败，优先处理阻塞错误，再重试。

### 步骤 3: 产物完整性检查

在 publish\win-x64 下确认:

1. LubanGui.exe 存在。
2. luban\Luban.dll 存在。
3. luban\Templates 目录存在。
4. luban\Luban.runtimeconfig.json 存在。
5. luban\Luban.deps.json 存在。

### 步骤 4: 冒烟验证

1. 双击 LubanGui.exe 启动应用。
2. 执行一次最小导表流程:
- 打开项目
- 检查导出配置
- 触发全量导表
3. 检查日志窗口输出是否正常。
4. 确认导表成功并有输出文件。

### 步骤 5: 打包与发布

1. 将 publish\win-x64 目录打包为 zip。
2. 命名建议:
- luban-gui-vX.Y.Z-windows-x64.zip
3. 在 GitHub Releases 上传 zip 并填写发布说明。
4. 发布后执行一次远端下载回归:
- 下载
- 解压
- 启动
- 最小导表验证

---

## 五、常见问题与处理

### 问题 1: NETSDK1047

现象:

发布时出现提示，project.assets.json 没有 net8.0 或 net8.0/win-x64 目标。

原因:

Luban 子项目在发布场景缺少 RID 维度还原或构建。

处理:

1. 确认发布命令包含 -r win-x64。
2. 确认 LubanGui.csproj 的 BuildLuban 目标传递了 RuntimeIdentifier 和 SelfContained。
3. 确认 BuildLuban 使用 Restore;Build。

### 问题 2: NU1902 或 NU1903 或 NU1904 警告

现象:

构建输出中提示 Scriban 5.12.0 存在安全漏洞告警。

说明:

1. 该问题来自 lubanSrc\Luban.Core 的上游依赖。
2. 不一定阻塞发布，但需要在发布说明标注为已知风险。
3. 后续可单独评估升级 Scriban 并做兼容性回归。

### 问题 3: 发布目录异常增量

现象:

LubanGui\luban 或其他目录出现大量运行时 DLL 变化。

处理建议:

1. 区分源码与构建产物。
2. 常规发布不提交自动生成的运行时二进制变化。
3. 仅在明确升级内置运行时快照时再提交该类文件。

---

## 六、发布验收标准

满足以下全部条件方可发布:

1. Publish 命令成功执行。
2. 发布目录关键文件完整。
3. 应用可在目标机器独立启动。
4. 最小导表流程成功。
5. README 与 CHANGELOG 与 Release Note 一致。
6. 已知风险已在发布说明中披露。

---

## 七、推荐发布节奏

1. Preview 阶段: 小步快发，保持每周可用版本。
2. 稳定版阶段: 发布前至少完成一次完整冒烟和回归。
3. 版本节奏建议:
- 开发分支日常构建
- 候选版本做一次发布彩排
- 正式版本仅从稳定提交发布

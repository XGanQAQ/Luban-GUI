# ProjectConfigManager

**所属层次**: 基础设施层 (Infrastructure)

---

## 职责

管理项目级配置（Luban 配置文件路径、导出目标、输出路径等），以 JSON 文件形式存储在项目根目录下。

---

## 存储路径

```
<ProjectRootPath>/projectConfig.json
```

---

## `ProjectConfig` JSON 示例

```json
{
  "confFile":     "Defines/luban.conf",
  "target":       "cs_bin",
  "codeTargets":  ["cs-simple-json"],
  "dataTargets":   ["json"],
  "codeOutputDir": "output/code",
  "dataOutputDir": "output/data",
  "extraArgs":    {}
}
```

---

## 读写实现

```csharp
// 读取
var json = await File.ReadAllTextAsync(configPath);
var config = JsonSerializer.Deserialize<ProjectConfig>(json, _options);

// 写入
var json = JsonSerializer.Serialize(config, _options);
await File.WriteAllTextAsync(configPath, json);
```

---

## 层间约定

- 使用 `System.Text.Json` 进行序列化，启用 `WriteIndented = true` 保证可读性。
- 配置文件路径由 `ProjectManager` 传入，`ProjectConfigManager` 不自行维护路径状态。
- 读写均为异步操作。

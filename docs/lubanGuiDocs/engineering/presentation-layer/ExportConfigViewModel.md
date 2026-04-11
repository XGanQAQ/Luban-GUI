# ExportConfigViewModel

**所属层次**: 表现层 (Avalonia UI)

---

## 职责

管理导出配置窗口（`ExportSettingsWindow`）的状态，让用户配置数据/代码的类型目标和输出路径，并在保存前执行配置校验。

---

## 关键属性

| 属性 | 类型 | 说明 |
|------|------|------|
| `Config` | `ProjectConfig` | 当前导出配置（双向绑定） |
| `ValidationErrors` | `IReadOnlyList<string>` | 校验失败时的错误列表 |
| `IsValid` | `bool` | 当前配置是否通过校验 |

---

## 关键命令

| 命令 | 触发场景 |
|------|----------|
| `SaveConfigCommand` | 用户点击「保存」按钮 |
| `BrowseOutputPathCommand` | 用户点击输出路径浏览按钮，弹出文件夹选择对话框 |
| `BrowseCodeOutputPathCommand` | 用户点击代码输出路径浏览按钮 |

---

## 层间约定

- 只调用**业务逻辑层 (Service)** 接口，不直接访问工具层或基础设施层。
- 通过构造函数注入 Service 接口，便于单元测试 Mock。
- 路径选择通过 Avalonia Storage Provider API 实现，不直接调用 `System.IO.Directory`。

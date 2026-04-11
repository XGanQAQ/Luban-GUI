# MainWindowViewModel

**所属层次**: 表现层 (Avalonia UI)

---

## 职责

统筹项目切换、表格列表、表格预览、导表状态，是主窗口的核心 ViewModel。

---

## 关键属性

| 属性 | 类型 | 说明 |
|------|------|------|
| `CurrentProject` | `ProjectInfo?` | 当前激活的项目 |
| `Projects` | `ObservableCollection<ProjectInfo>` | 所有已注册项目 |
| `IsExporting` | `bool` | 是否正在导表 |
| `ExportStatusMessage` | `string` | 导表状态提示文字 |

---

## 关键命令

| 命令 | 触发场景 |
|------|----------|
| `SwitchProjectCommand` | 用户点击项目切换栏下拉框 |
| `StartExportCommand` | 用户点击「全量导表」按钮 |
| `CancelExportCommand` | 用户点击「取消」按钮 |
| `OpenLogWindowCommand` | 用户点击「日志窗口」菜单项 |

---

## 层间约定

- 只调用**业务逻辑层 (Service)** 接口，不直接访问工具层或基础设施层。
- 通过构造函数注入 Service 接口，便于单元测试 Mock。
- 所有 UI 更新在 Avalonia UI 线程执行（通过 `IProgress<T>` 或 `Dispatcher.UIThread.Post`）。

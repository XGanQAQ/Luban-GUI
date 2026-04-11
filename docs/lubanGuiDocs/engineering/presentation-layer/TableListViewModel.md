# TableListViewModel

**所属层次**: 表现层 (Avalonia UI)

---

## 职责

管理左侧表格列表面板，响应快捷操作命令（新建/导入/移除表格）。

---

## 关键属性

| 属性 | 类型 | 说明 |
|------|------|------|
| `Tables` | `ObservableCollection<TableMeta>` | 当前项目下的所有表格 |
| `SelectedTable` | `TableMeta?` | 当前选中的表格 |
| `FilterText` | `string` | 过滤框输入文本，实时筛选表格列表 |

---

## 关键命令

| 命令 | 触发场景 |
|------|----------|
| `CreateTableCommand` | 用户点击「+ 新建表格」按钮 |
| `CreateEnumCommand` | 用户点击「+ 新建枚举」按钮 |
| `CreateBeanCommand` | 用户点击「+ 新建 Bean」按钮 |
| `ImportTableCommand` | 用户点击「📥 导入文件」按钮 |
| `RemoveTableCommand` | 用户右键 → 从项目中移除 |
| `SelectTableCommand` | 用户单击列表中某一表格 |
| `OpenInExplorerCommand` | 用户右键 → 在文件管理器中显示 |

---

## 层间约定

- 只调用**业务逻辑层 (Service)** 接口，不直接访问工具层或基础设施层。
- 通过构造函数注入 Service 接口，便于单元测试 Mock。
- 所有 UI 更新在 Avalonia UI 线程执行。

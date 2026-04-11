# TablePreviewViewModel

**所属层次**: 表现层 (Avalonia UI)

---

## 职责

管理右侧表格内容面板，展示选中表格的数据预览，响应双击列标题定位到 Excel 文件的命令。

---

## 关键属性

| 属性 | 类型 | 说明 |
|------|------|------|
| `PreviewData` | `TablePreviewData?` | 当前预览的表格数据（列名 + 数据行） |
| `IsLoading` | `bool` | 是否正在加载预览数据 |
| `ErrorMessage` | `string?` | 读取失败时的错误提示文本 |

---

## 关键命令

| 命令 | 触发场景 |
|------|----------|
| `OpenFieldInExcelCommand` | 用户双击列标题，定位到 Excel 文件中对应字段 |
| `OpenInEditorCommand` | 用户点击右上角「在编辑器中打开」按钮 |

---

## 层间约定

- 只调用**业务逻辑层 (Service)** 接口，不直接访问工具层或基础设施层。
- 通过构造函数注入 Service 接口，便于单元测试 Mock。
- 所有 UI 更新在 Avalonia UI 线程执行。

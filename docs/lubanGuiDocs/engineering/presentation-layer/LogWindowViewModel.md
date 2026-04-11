# LogWindowViewModel

**所属层次**: 表现层 (Avalonia UI)

---

## 职责

管理日志浮动窗口（`LogWindow`）的数据，展示运行时日志记录，支持清空和自动滚动到底部。

---

## 关键属性

| 属性 | 类型 | 说明 |
|------|------|------|
| `LogItems` | `ObservableCollection<LogEntry>` | 日志记录列表，最多保留 10 000 条 |
| `AutoScroll` | `bool` | 是否自动滚动到最新日志 |

---

## 关键命令

| 命令 | 触发场景 |
|------|----------|
| `ClearLogCommand` | 用户点击「清空」按钮 |

---

## 过容量策略

当 `LogItems` 超过 10 000 条时，每次追加前移除最早的若干条记录，防止内存无限增长。

---

## 层间约定

- 只调用**业务逻辑层 (Service)** 接口，不直接访问工具层或基础设施层。
- 通过构造函数注入 Service 接口，便于单元测试 Mock。
- 所有 UI 更新在 Avalonia UI 线程执行。

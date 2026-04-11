# TablePreviewService

**所属层次**: 业务逻辑层 (Service)

---

## 职责

为表格预览面板提供数据：读取指定 `.xlsx` 数据文件的前 N 行（默认 100 行），并支持获取某列在 Excel 文件中的精确单元格位置（用于双击定位功能）。

---

## 接口定义

```csharp
public interface ITablePreviewService
{
    Task<TablePreviewData> LoadPreviewAsync(
        TableMeta meta,
        int       maxRows = 100);

    (string FilePath, int Row, int Col) GetFieldLocation(
        TableMeta meta,
        string    fieldName);
}
```

---

## 数据结构

```csharp
public record TablePreviewData(
    IReadOnlyList<string>              ColumnNames,
    IReadOnlyList<IReadOnlyList<string>> Rows
);
```

---

## 层间约定

- 向下依赖 `ExcelWriter`（工具层）的只读方法读取 `.xlsx` 文件。
- 只暴露 `ITablePreviewService` 接口。
- `LoadPreviewAsync` 应在后台线程执行，不得阻塞 UI 线程。

# ExcelWriter

**所属层次**: 工具层 (Tool)

---

## 职责

封装 ClosedXML 库，提供对 `.xlsx` 文件的读写操作，供 `SchemaService` 操作元数据文件、供 `TablePreviewService` 读取数据行使用。

---

## 依赖

```xml
<PackageReference Include="ClosedXML" Version="0.100.*" />
```

---

## 操作类型

| 方法 | 说明 |
|------|------|
| `ReadRowsAsync(path, sheetName, maxRows)` | 读取指定 Sheet 的行数据，返回 `IReadOnlyList<IReadOnlyList<string>>` |
| `AppendRowAsync(path, sheetName, values)` | 在 Sheet 末尾追加一行 |
| `UpdateRowAsync(path, sheetName, rowIndex, values)` | 更新指定行（1-based） |
| `DeleteRowAsync(path, sheetName, rowIndex)` | 删除指定行 |

---

## xlsx 格式约定

- 第一行固定为表头（列名）。
- 所有单元格读写均以 `string` 类型处理，类型转换由上层负责。
- 若目标文件不存在，`AppendRowAsync` 自动创建文件和 Sheet。

---

## 层间约定

- 只依赖 ClosedXML，不引用任何 Service 接口或 ViewModel。
- 所有文件 I/O 操作均为异步（`Task`），避免阻塞 UI 线程。
- 不缓存文件句柄，每次操作后立即关闭 workbook。

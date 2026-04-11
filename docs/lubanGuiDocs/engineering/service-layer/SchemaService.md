# SchemaService

**所属层次**: 业务逻辑层 (Service)

---

## 职责

对当前项目的表格 Schema 进行 CRUD 操作：读取、新建、导入、删除表 / 枚举 / 结构体，并通过 `ExcelWriter` 将变更写入 `.xlsx` 元数据文件。

---

## 接口定义

```csharp
public interface ISchemaService
{
    Task<IReadOnlyList<TableMeta>> GetTablesAsync();
    Task<TableMeta>                CreateTableAsync(TableMeta meta);
    Task<TableMeta>                ImportTableAsync(string excelPath);
    Task<EnumItemDefinition>       CreateEnumAsync(string enumName);
    Task<FieldDefinition>          CreateBeanAsync(string beanName);
    Task                           RemoveTableAsync(TableMeta meta);
}
```

---

## Excel 元数据文件格式

元数据文件位于 `<ProjectPath>/Defines/__tables.xlsx`，每行一张表，格式如下：

| full_name | value_type | mode | define_from_file |
|-----------|------------|------|-----------------|
| item.ItemCfg | Item | map | item/item.xlsx |
| task.TaskCfg | Task | list | task/task.xlsx |

---

## 层间约定

- 向下依赖 `ExcelWriter`（工具层）读写元数据 `.xlsx` 文件。
- 向下依赖 `LubanSchemaReader`（Luban 适配层）校验表名与类型是否合法。
- 只暴露 `ISchemaService` 接口。

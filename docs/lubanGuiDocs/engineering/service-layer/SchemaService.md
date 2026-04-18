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
    Task<IReadOnlyList<DataTypeListItem>> GetUnifiedTypeListAsync(string projectPath);
}
```

`GetUnifiedTypeListAsync` 用于「数据类型列表」二级窗口，统一返回三类类型：

- 内置默认类型（如 `int`、`string`）
- 自定义枚举类型（enum）
- 自定义结构类型（bean）

### 字段类型来源约束

- 字段类型中的**自定义类型**仅允许来自 `__enums__.xlsx` 与 `__beans__.xlsx`。
- 普通配置表（`__tables__.xlsx`）是数据容器定义，不属于字段可引用的类型来源。

### 类型视图来源约束

- 「数据类型列表」中的枚举/Bean条目以 `__enums__.xlsx` 和 `__beans__.xlsx` 的显式声明为准。
- 即使 Luban 运行时可从普通表推导记录类型，这类推导类型也不在 GUI 的 Bean 类型列表中展示。

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

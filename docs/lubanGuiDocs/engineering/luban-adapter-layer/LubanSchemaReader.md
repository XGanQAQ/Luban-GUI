# LubanSchemaReader

**所属层次**: Luban 源适配层 (LubanAdapter)

---

## 职责

从 Luban 的 `luban.conf` 及其引用的 Schema 文件中读取表、枚举、结构体的 Schema 定义，转换为 GUI 自有的 DTO 类型，隔离 GUI 对 Luban 源码类型的直接依赖。

---

## 接口定义

```csharp
public interface ILubanSchemaReader
{
    Task<IReadOnlyList<TableSchemaDto>>  ReadTablesAsync(string confPath);
    Task<IReadOnlyList<EnumSchemaDto>>   ReadEnumsAsync(string confPath);
    Task<IReadOnlyList<BeanSchemaDto>>   ReadBeansAsync(string confPath);
}
```

---

## DTO 定义

```csharp
public record TableSchemaDto(
    string FullName,
    string ValueType,
    string Mode,
    string DefineFromFile
);

public record EnumSchemaDto(
    string FullName,
    IReadOnlyList<string> Items
);

public record BeanSchemaDto(
    string FullName,
    IReadOnlyList<string> Fields
);
```

---

## 层间约定

- 本层是 GUI 与 `lubanSrc\` 之间的**唯一接触点**，上方各层不得直接引用 Luban 源码中的类型。
- 若 Luban Schema 文件不存在，返回空列表，不抛出异常。
- 所有方法均为异步，读取 I/O 在后台线程完成。

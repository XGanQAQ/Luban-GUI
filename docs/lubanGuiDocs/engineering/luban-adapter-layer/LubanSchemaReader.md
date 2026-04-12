# LubanSchemaReader

**所属层次**: Luban 源适配层 (LubanAdapter)

---

## 职责

从 Luban 的 `luban.conf` 及其引用的 Schema 文件中读取表、枚举、结构体的 Schema 定义，转换为 GUI 自有的 DTO 类型，隔离 GUI 对 Luban 源码类型的直接依赖。

读取流程直接复用 Luban 的真实解析管线（`GlobalConfigLoader` → `DefaultSchemaCollector`），而非自行解析 Excel/XML。

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

### FieldDto — 字段定义

```csharp
public record FieldDto(
    string                             Name,
    string                             Alias,
    string                             Type,       // Luban 原始类型字符串，如 "int", "list,string"
    string                             Comment,
    IReadOnlyList<string>              Groups,
    IReadOnlyDictionary<string, string> Tags
);
```

### TableSchemaDto — 表格 Schema

对应 Luban `RawTable`。

```csharp
public record TableSchemaDto(
    string                FullName,           // Namespace.Name，如 "game.tables.TbItem"
    string                Index,              // 主键字段名，如 "id"
    string                ValueType,          // 行值类型，如 "game.ItemRecord"
    string                Mode,               // "ONE" / "MAP" / "LIST"
    string                Comment,
    IReadOnlyList<string> InputFiles,         // 相对于 dataDir 的数据文件路径列表
    string                OutputFile,
    IReadOnlyList<string> Groups,
    bool                  ReadSchemaFromFile  // true 时字段 Schema 内嵌于数据文件
);
```

### EnumItemDto — 枚举项

```csharp
public record EnumItemDto(
    string Name,
    string Alias,
    string Value,
    string Comment
);
```

### EnumSchemaDto — 枚举 Schema

对应 Luban `RawEnum`。

```csharp
public record EnumSchemaDto(
    string                    FullName,
    bool                      IsFlags,
    string                    Comment,
    IReadOnlyList<EnumItemDto> Items
);
```

### BeanSchemaDto — 结构体 Schema

对应 Luban `RawBean`。

```csharp
public record BeanSchemaDto(
    string                FullName,
    string                Parent,       // 父类全名，无继承时为空
    bool                  IsValueType,
    string                Comment,
    string                Sep,          // 分隔符（用于内联序列化）
    IReadOnlyList<FieldDto> Fields
);
```

---

## 实现方案

### 依赖

- `LubanAdapterInitializer`：应用启动时已完成一次性初始化（注册 Luban 各 Manager）。
- `Luban.GlobalConfigLoader`：解析 `luban.conf` → `LubanConfig`。
- `Luban.Schema.SchemaManager`：创建 `DefaultSchemaCollector`。
- `Luban.GenerationContext`：提供 `InputDataDir` 给 `DefaultSchemaCollector`（通过静态属性 `GlobalConf`）。

### 内部执行序列

```
ReadTablesAsync(confPath) / ReadEnumsAsync / ReadBeansAsync
  └─ 共用私有方法 LoadRawAssemblyAsync(confPath)
       1. await Task.Run(() => { ... }) // 后台线程
       2. lock(_syncLock)               // 串行化（保护全局静态状态）
       3. GlobalConfigLoader.Load(confPath)  → LubanConfig
       4. GenerationContext.GlobalConf = config
       5. EnvManager.Current = new EnvManager({})
       6. SchemaManager.Ins.CreateSchemaCollector("default") → ISchemaCollector
       7. collector.Load(config)
       8. return collector.CreateRawAssembly()
  └─ 将 RawAssembly.Tables / .Enums / .Beans 映射为 DTO 列表返回
```

### 异常处理

- 若 `confPath` 不存在或格式错误，捕获异常并记录日志，返回空列表。
- Schema 文件（`__tables__.xlsx` 等）缺失时，Luban 内部会跳过，不影响其他 schema。

---

## 层间约定

- 本层是 GUI 与 `lubanSrc\` 之间的**唯一接触点**，上方各层不得直接引用 Luban 源码中的类型。
- 若 Luban Schema 文件不存在，返回空列表，不抛出异常。
- 所有方法均为异步，读取 I/O 在后台线程完成。
- `_syncLock` 保护 `EnvManager.Current` 与 `GenerationContext.GlobalConf` 这两个全局静态状态，禁止并发 Schema 读取。

# Luban 源适配层模块设计

**所属层次**: Luban 源适配层 (LubanAdapter)  
**版本**: 2.0

---

## 职责

作为 GUI 代码库与 `lubanSrc\` 源码之间的**唯一隔离边界**。

- GUI 各层所需的 Luban 内部数据（Schema 定义、配置格式、类型系统）全部通过此层接口获取。
- 上方任何层（包括基础设施层）**不得直接引用** `lubanSrc\` 中的命名空间或类型。
- 此层的接口语义以 GUI 需求为中心，返回 GUI 友好的 DTO，屏蔽 Luban 内部复杂结构。

---

## 存在意义

| 问题 | 解决方式 |
|------|----------|
| Luban 源码 API 随版本迭代变化 | 改动只在此层修改，上层代码不受影响 |
| Luban 内部结构复杂（多态 Bean 树、加载器链等） | 此层将其扁平化为 GUI 需要的简单 DTO |
| 上层测试依赖 Luban 源码 | 通过 Mock 此层接口，可独立测试上层逻辑 |
| Schema 读取逻辑散布在多处 | 统一收口，方便排查格式兼容问题 |

---

## LubanSchemaReader

**职责**: 从元数据文件（`__tables__.xlsx`、`__enums__.xlsx`、`__beans__.xlsx`）读取 Schema 定义，返回 GUI 友好的 DTO，屏蔽 Luban 内部 Schema 解析细节。

**接口**:
```csharp
public interface ILubanSchemaReader
{
    // 读取所有表格定义
    Task<IReadOnlyList<TableSchemaDto>> ReadTablesAsync(string dataDir);

    // 读取所有枚举定义
    Task<IReadOnlyList<EnumSchemaDto>> ReadEnumsAsync(string dataDir);

    // 读取所有 Bean 定义
    Task<IReadOnlyList<BeanSchemaDto>> ReadBeansAsync(string dataDir);
}
```

**返回 DTO 示例**:
```csharp
public record TableSchemaDto(
    string Name,          // 表格名，如 "TbItem"
    string Module,        // 所属模块，如 "item"
    string ValueType,     // 值类型，如 "Item"
    string Mode,          // 表类型：map / list / singleton
    string InputFile      // 对应 xlsx 路径
);

public record EnumSchemaDto(
    string Name,
    bool IsFlags,
    IReadOnlyList<EnumItemDto> Items
);

public record BeanSchemaDto(
    string Name,
    string? Parent,       // 父 Bean 名（多态继承）
    IReadOnlyList<FieldDto> Fields
);
```

**实现说明**:
- 实现类直接调用 Luban 源码中的 xlsx 解析逻辑（如 `Luban.Core` 里的 SchemaLoader），但将结果转换为上述 DTO 后返回。
- 若 Luban 内部解析 API 发生变化，仅需修改此实现类，接口与上层代码不变。

---

## LubanConfAdapter

**职责**: 解析和生成 `luban.conf`，屏蔽 Luban 配置文件格式细节

**接口**:
```csharp
public interface ILubanConfAdapter
{
    // 从磁盘读取 luban.conf，返回 GUI 可理解的配置 DTO
    Task<LubanConfDto> ReadAsync(string confPath);

    // 根据 GUI 配置 DTO 生成/覆写 luban.conf
    Task WriteAsync(string confPath, LubanConfDto conf);

    // 生成新项目所需的初始 luban.conf 内容
    LubanConfDto CreateDefault(string projectName, string dataDir);
}
```

**配置 DTO 示例**:
```csharp
public record LubanConfDto(
    IReadOnlyList<string> SchemaFiles,   // 元数据表路径列表
    IReadOnlyList<GroupDto> Groups,      // 分组定义
    IReadOnlyList<TargetDto> Targets     // 导出目标定义
);
```

**实现说明**:
- `luban.conf` 是 JSON 格式，由 Luban 源码定义其 Schema。此类封装序列化/反序列化逻辑。
- 若 Luban 升级后 `luban.conf` 格式变更，仅需更新此类的读写逻辑。

---

## LubanTypeMapper

**职责**: 将 Luban 内置类型标识符映射为 GUI 可显示的类型描述，以及将 GUI 的类型选择反向映射为 Luban 类型字符串

**接口**:
```csharp
public interface ILubanTypeMapper
{
    // 获取所有 Luban 内置基础类型（用于新建字段时的类型下拉列表）
    IReadOnlyList<LubanTypeDescriptor> GetBuiltinTypes();

    // 将 Luban 类型字符串转换为 GUI 友好的显示名
    // 例如: "map,int,string" → "map<int, string>"
    string ToDisplayName(string lubanType);

    // 将 GUI 选择的类型转换回 Luban 类型字符串
    string ToLubanType(string displayName);
}

public record LubanTypeDescriptor(
    string LubanType,     // Luban 内部类型字符串，如 "int", "string", "list,int"
    string DisplayName,   // GUI 显示名称，如 "整数", "字符串", "列表<整数>"
    string Category       // 分类：基础类型 / 容器类型 / 自定义类型
);
```

---

## 层间约定

- 此层是 GUI 代码库对 `lubanSrc\` 的**唯一依赖点**，其他所有层均不得直接引用 `lubanSrc\` 中的类型。
- 此层只做"转译"，不包含 GUI 业务逻辑（如"是否允许重名"等判断由业务逻辑层负责）。
- 所有文件 IO 操作使用异步方法（`Task` / `Task<T>`）。
- 接口以 `I` 前缀命名，实现类放在 `LubanGui/LubanAdapter/` 目录下。

---

## 目录结构

```
LubanGui/
└── LubanAdapter/
    ├── ILubanSchemaReader.cs     # 接口定义
    ├── ILubanConfAdapter.cs
    ├── ILubanTypeMapper.cs
    ├── LubanSchemaReader.cs      # 依赖 lubanSrc\Luban.Core 的实现
    ├── LubanConfAdapter.cs
    ├── LubanTypeMapper.cs
    └── Dto/
        ├── TableSchemaDto.cs
        ├── EnumSchemaDto.cs
        ├── BeanSchemaDto.cs
        ├── LubanConfDto.cs
        └── LubanTypeDescriptor.cs
```

# LubanTypeMapper

**所属层次**: Luban 源适配层 (LubanAdapter)

---

## 职责

提供 Luban 内置类型系统到 GUI 显示名称的双向映射，让 GUI 可以在下拉列表中展示友好的类型名，同时在写入 Schema 时转换回 Luban 原始类型字符串。

---

## 接口定义

```csharp
public interface ILubanTypeMapper
{
    IReadOnlyList<LubanTypeDescriptor> GetBuiltinTypes();
    string                             ToDisplayName(string lubanType);
    string                             ToLubanType(string displayName);
}
```

---

## `LubanTypeDescriptor` 定义

```csharp
public record LubanTypeDescriptor(
    string LubanType,    // Luban 原始类型字符串，如 "int", "string", "bool"
    string DisplayName,  // 界面显示名，如 "整数 (int)", "字符串 (string)"
    string Category      // 分组标签，如 "基础类型", "集合类型", "引用类型"
);
```

---

## 层间约定

- 本层是 GUI 与 `lubanSrc\` 之间的**唯一接触点**，上方各层不得直接引用 Luban 源码中的类型。
- `GetBuiltinTypes` 返回的列表应与 Luban 当前版本支持的内置类型保持同步，版本升级时需验证。
- 若传入未知类型，`ToDisplayName` 和 `ToLubanType` 直接返回原始输入，不抛出异常。

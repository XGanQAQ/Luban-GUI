# LubanConfAdapter

**所属层次**: Luban 源适配层 (LubanAdapter)

---

## 职责

读写 Luban 的主配置文件（`luban.conf`），将其内容映射为 GUI 可直接使用的 `LubanConfDto`，并支持在项目初始化时生成默认配置文件。

---

## 接口定义

```csharp
public interface ILubanConfAdapter
{
    Task<LubanConfDto> ReadAsync(string confPath);
    Task               WriteAsync(string confPath, LubanConfDto dto);
    Task               CreateDefaultAsync(string confPath);
}
```

---

## `LubanConfDto` 定义

```csharp
public record LubanConfDto(
    string                           Target,
    IReadOnlyList<string>            CodeTargets,
    IReadOnlyList<string>            DataTargets,
    string                           TopModule,
    IReadOnlyList<string>            InputDataDirs,
    IReadOnlyDictionary<string, string> Groups
);
```

---

## 层间约定

- 本层是 GUI 与 `lubanSrc\` 之间的**唯一接触点**，上方各层不得直接引用 Luban 源码中的类型。
- `CreateDefaultAsync` 生成的默认配置需与 Luban CLI 当前版本兼容，版本升级时需同步验证。
- 读写均为异步操作。

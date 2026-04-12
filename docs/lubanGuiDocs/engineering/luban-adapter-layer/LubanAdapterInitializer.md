# LubanAdapterInitializer

**所属层次**: Luban 源适配层 (LubanAdapter)

---

## 职责

在应用启动时**一次性**初始化 Luban 的各个单例 Manager，注册全部内置行为（Schema Loader、Data Loader、Table Importer 等），使后续适配层各组件可以直接调用 Luban 的解析 API。

---

## 接口定义

```csharp
public static class LubanAdapterInitializer
{
    /// <summary>
    /// 初始化 Luban 内部所有 Manager 并注册内置行为。
    /// 幂等：多次调用只执行一次。
    /// 应在 DI 容器构建完成、首次访问任何适配层服务前调用。
    /// </summary>
    public static void Initialize();
}
```

---

## 实现说明

内部调用 `Luban.SimpleLauncher.Start(new Dictionary<string, string>())`，该方法完成：

1. 创建空 `EnvManager`（后续每次读取时按需替换）。
2. 初始化各 Manager：`SchemaManager`、`DataLoaderManager`、`CustomBehaviourManager` 等。
3. 扫描当前应用目录下所有带 `[assembly: RegisterBehaviour]` 的 Luban DLL，注册其中的：
   - `ISchemaCollector`（如 `DefaultSchemaCollector`）
   - `ISchemaLoader`（如 `ExcelSchemaLoader`、`XmlSchemaLoader`）
   - `IDataLoader`（如 Excel、JSON、YAML、CSV 加载器）
   - `ITableImporter`、`IBeanSchemaLoader` 等

由于 `LubanGui.csproj` 直接引用了 `Luban.Schema.Builtin` 和 `Luban.DataLoader.Builtin`，上述 DLL 在编译时已复制到输出目录，因此 `SimpleLauncher` 可正确发现并注册。

---

## 调用位置

在 `App.axaml.cs` 的 `ConfigureServices` 末尾调用：

```csharp
// Luban 适配层一次性初始化
LubanAdapterInitializer.Initialize();
```

---

## 层间约定

- 本模块允许直接引用 `lubanSrc\` 中的 `SimpleLauncher`，但不对外暴露任何 Luban 内部类型。
- `Initialize()` 设计为幂等，重复调用无副作用。
- 若初始化失败（如 DLL 缺失），抛出异常并由应用启动流程捕获，GUI 应展示错误提示后退出。

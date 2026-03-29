# 使用指南
> Source: https://www.datable.cn/docs/basic

## 使用指南

- [与classic版本差异](../manual/migrate.md) — 当前版本相比于classic版本极大简化了代码实现，更方便定制。
- [设计哲学](../manual/architecture.md) — 在大多数人的印象中，配置工具的实现应该是非常简单的。
- [特性](../manual/traits.md) — Luban内置了丰富的特性。
- [luban.conf](../manual/luban.conf.md) — 定义了luban所需要的全局配置。
- [schema 逻辑结构](../manual/schema.md) — Luban的核心为完备的类型系统，而DPP管线则是强大的扩展能力的基础。
- [配置定义](../manual/defaultschemacollector.md) — Luban有一套独立于具体实现的Schema逻辑结构实现。
- [自动导入table](../manual/importtable.md) — v3.0.0版本起支持自动导入table。
- [命令行工具](../manual/commandtools.md) — 跨平台
- [层级参数机制](../manual/cascadingoption.md) — Luban的大多数内置模板都使用了层级参数(Cascading Option)机制。
- [类型系统](../manual/types.md) — Luban有完备的类型系统，尤其是bean支持类型继承和多态。
- [类型映射](../manual/typemapper.md) — 有时候你希望生成的代码中能直接使用现成的结构类型。
- [excel格式（初级）](../manual/excel.md) — 基础规则
- [excel格式（高级）](../manual/exceladvanced.md) — 示例中用到的结构
- [Excel 紧凑格式](../manual/excelcompactformat.md) — 介绍
- [非excel数据源](../manual/otherdatasource.md) — 并不是所有配置数据都以excel格式保存。
- [代码与数据生成](../manual/generatecodedata.md) — 支持主流游戏引擎及平台
- [代码风格](../manual/codestyle.md) — Luban默认为某个语言生成符合该语言推荐风格的代码。
- [加载配置](../manual/loadconfigatruntime.md) — 安装Luban.Runtime
- [数据校验器](../manual/validator.md) — Luban.DataValidtor.Builtin模块中实现多种常见的数据校验器。
- [自定义模板](../manual/template.md) — luban使用scriban 模板引擎来生成代码。
- [数据tag](../manual/tag.md) — luban支持记录级别的tag标记。
- [字段变体（Variants）](../manual/variants.md) — 有时候同一个字段可能有多个配置。
- [本地化](../manual/l10n.md) — 支持多种本地化机制，它们可以同时使用。
- [最佳实践](../manual/bestpractices.md) — 命名约定
- [扩展Luban](../manual/extendluban.md) — 在权衡灵活性和简便性后，luban没有使用插件机制，而是在源码工程中新增一些扩展项目来实现扩展。

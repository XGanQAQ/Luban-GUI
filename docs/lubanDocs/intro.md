# 介绍
> Source: https://www.datable.cn/docs/intro

## 介绍

![icon](/assets/images/logo-4bc800312bd07ef2f4d6e4568b9b5758.png)

[![license](http://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://opensource.org/licenses/MIT) ![star](https://img.shields.io/github/stars/focus-creative-games/luban?style=flat-square)

luban是一个强大、易用、优雅、稳定的游戏配置解决方案。它设计目标为满足从小型到超大型游戏项目的简单到复杂的游戏配置工作流需求。

luban可以处理丰富的文件类型，支持主流的语言，可以生成多种导出格式，支持丰富的数据检验功能，具有良好的跨平台能力，并且生成极快。 luban有清晰优雅的生成管线设计，支持良好的模块化和插件化，方便开发者进行二次开发。开发者很容易就能将luban适配到自己的配置格式，定制出满足项目要求的强大的配置工具。

luban标准化了游戏配置开发工作流，可以极大提升策划和程序的工作效率。

## 核心特性

*   丰富的源数据格式。支持excel族(csv,xls,xlsx,xlsm)、json、xml、yaml、lua等
*   丰富的导出格式。 支持生成binary、json、bson、xml、lua、yaml等格式数据
*   增强的excel格式。可以简洁地配置出像简单列表、子结构、结构列表，以及任意复杂的深层次的嵌套结构
*   完备的类型系统。不仅能表达常见的规范行列表，由于**支持OOP类型继承**，能灵活优雅表达行为树、技能、剧情、副本之类复杂GamePlay数据
*   支持多种的语言。支持生成c#、java、go、cpp、lua、python、typescript 等语言代码
*   支持主流的消息方案。 protobuf(schema + binary + json)、flatbuffers(schema + json)、msgpack(binary)
*   强大的数据校验能力。ref引用检查、path资源路径、range范围检查等等
*   完善的本地化支持
*   支持所有主流的游戏引擎和平台。支持Unity、Unreal、Cocos、Godot、Laya、微信小游戏等
*   良好的跨平台能力。能在Win,Linux,Mac平台良好运行。
*   支持所有主流的热更新方案。hybridclr、ilruntime、{x,t,s}lua、puerts等
*   清晰优雅的生成管线，很容易在luban基础上进行二次开发，定制出适合自己项目风格的配置工具。

## Excel格式概览

基础数据格式

![primitive_type](/assets/images/primitive_type-d85cfb51a19f153b0fdf9ac299b4a5e1.jpg)

enum 数据格式

![enum](/assets/images/enum-dee044226803effc6032313e7c4981e7.jpg)

bean数据格式

![bean](/assets/images/bean-85ba1ecb5030e30e47c4487ec0c261d2.jpg)

多态bean数据格式

![bean](/assets/images/bean2-04651442a9b2d1cb2c12f18f23cb9bcf.jpg)

容器

![collection](/assets/images/collection-5416a057bd788208fb64a6b5420663ef.jpg)

可空类型

![nullable](/assets/images/nullable-8a3a3a221c9def07e16e04ccf86a9b84.jpg)

无主键表

![table_list_not_key](/assets/images/table_list_not_key-082f29e3fc26a5cc33d982f34a4c1e60.jpg)

多主键表（联合索引）

![table_list_union_key](/assets/images/table_list_union_key-27d9231b4a48f42aa5f79cf80e2ffd81.jpg)

多主键表（独立索引）

![table_list_indep_key](/assets/images/table_list_indep_key-3d2f4e268f41d88d0312c350bdf075e4.jpg)

单例表

有一些配置全局只有一份，比如 公会模块的开启等级，背包初始大小，背包上限。此时使用单例表来配置这些数据比较合适。

![singleton](/assets/images/singleton2-b46d4b2c6cccbabd69296a59222fe9d4.jpg)

纵表

大多数表都是横表，即一行一个记录。有些表，比如单例表，如果纵着填，一行一个字段，会比较舒服。A1为`##column`或`##vertical`表示使用纵表模式。 上面的单例表，以纵表模式填如下。

![singleton](/assets/images/singleton-9b7d41bf32c0c214d2baac6cbbd5cea8.jpg)

## 代码使用预览

这儿只简略展示c#、typescript、go、c++ 语言在开发中的用法，更多语言以及更详细的使用范例和代码见[示例项目](https://gitee.com/focus-creative-games/luban_examples)。

*   C# 使用示例

```csharp
// 一行代码可以加载所有配置。 cfg.Tables 包含所有表的一个实例字段。
var tables = new cfg.Tables(file => return new ByteBuf(File.ReadAllBytes($"{gameConfDir}/{file}.bytes")));
// 访问一个单例表
Console.WriteLine(tables.TbGlobal.Name);
// 访问普通的 key-value 表
Console.WriteLine(tables.TbItem.Get(12).Name);
// 支持 operator []用法
Console.WriteLine(tables.TbMail[1001].Desc);
```

*   typescript 使用示例

```typescript
// 一行代码可以加载所有配置。 cfg.Tables 包含所有表的一个实例字段。
let tables = new cfg.Tables(f => JsHelpers.LoadFromFile(gameConfDir, f))
// 访问一个单例表
console.log(tables.TbGlobal.name)
// 访问普通的 key-value 表
console.log(tables.TbItem.get(12).Name)
```

*   go 使用示例

```go
// 一行代码可以加载所有配置。 cfg.Tables 包含所有表的一个实例字段。
if tables , err := cfg.NewTables(loader) ; err != nil { println(err.Error()) return}
// 访问一个单例表
println(tables.TbGlobal.Name)
// 访问普通的 key-value 表
println(tables.TbItem.Get(12).Name)
```

*   c++ 使用示例

```cpp
    cfg::Tables tables;
    if (!tables.load([](ByteBuf& buf, const std::string& s) { return buf.loadFromFile("../GenerateDatas/bytes/" + s + ".bytes"); }))
    {
        std::cout << "== load fail == " << std::endl;
        return;
    }
    std::cout << tables.TbGlobal->name << std::endl;
    std::cout << tables.TbItem.get(12)->name << std::endl;
```

## license

Luban is licensed under the [MIT](https://github.com/focus-creative-games/luban/blob/main/LICENSE) license

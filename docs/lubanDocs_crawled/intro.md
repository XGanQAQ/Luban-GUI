> Source: https://www.datable.cn/docs/intro

Title: 介绍 | Luban

URL Source: https://www.datable.cn/docs/intro

Published Time: Wed, 25 Mar 2026 12:09:02 GMT

Markdown Content:
## 介绍

![Image 1: icon](https://www.datable.cn/assets/images/logo-4bc800312bd07ef2f4d6e4568b9b5758.png)

[![Image 2: license](http://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://opensource.org/licenses/MIT)![Image 3: star](https://img.shields.io/github/stars/focus-creative-games/luban?style=flat-square)

luban是一个强大、易用、优雅、稳定的游戏配置解决方案。它设计目标为满足从小型到超大型游戏项目的简单到复杂的游戏配置工作流需求。

luban可以处理丰富的文件类型，支持主流的语言，可以生成多种导出格式，支持丰富的数据检验功能，具有良好的跨平台能力，并且生成极快。 luban有清晰优雅的生成管线设计，支持良好的模块化和插件化，方便开发者进行二次开发。开发者很容易就能将luban适配到自己的配置格式，定制出满足项目要求的强大的配置工具。

luban标准化了游戏配置开发工作流，可以极大提升策划和程序的工作效率。

## 核心特性[​](https://www.datable.cn/docs/intro#%E6%A0%B8%E5%BF%83%E7%89%B9%E6%80%A7 "核心特性的直接链接")

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

## Excel格式概览[​](https://www.datable.cn/docs/intro#excel%E6%A0%BC%E5%BC%8F%E6%A6%82%E8%A7%88 "Excel格式概览的直接链接")

基础数据格式

![Image 4: primitive_type](https://www.datable.cn/assets/images/primitive_type-d85cfb51a19f153b0fdf9ac299b4a5e1.jpg)

enum 数据格式

![Image 5: enum](https://www.datable.cn/assets/images/enum-dee044226803effc6032313e7c4981e7.jpg)

bean数据格式

![Image 6: bean](https://www.datable.cn/assets/images/bean-85ba1ecb5030e30e47c4487ec0c261d2.jpg)

多态bean数据格式

![Image 7: bean](https://www.datable.cn/assets/images/bean2-04651442a9b2d1cb2c12f18f23cb9bcf.jpg)

容器

![Image 8: collection](https://www.datable.cn/assets/images/collection-5416a057bd788208fb64a6b5420663ef.jpg)

可空类型

![Image 9: nullable](https://www.datable.cn/assets/images/nullable-8a3a3a221c9def07e16e04ccf86a9b84.jpg)

无主键表

![Image 10: table_list_not_key](https://www.datable.cn/assets/images/table_list_not_key-082f29e3fc26a5cc33d982f34a4c1e60.jpg)

多主键表（联合索引）

![Image 11: table_list_union_key](https://www.datable.cn/assets/images/table_list_union_key-27d9231b4a48f42aa5f79cf80e2ffd81.jpg)

多主键表（独立索引）

![Image 12: table_list_indep_key](https://www.datable.cn/assets/images/table_list_indep_key-3d2f4e268f41d88d0312c350bdf075e4.jpg)

单例表

有一些配置全局只有一份，比如 公会模块的开启等级，背包初始大小，背包上限。此时使用单例表来配置这些数据比较合适。

![Image 13: singleton](https://www.datable.cn/assets/images/singleton2-b46d4b2c6cccbabd69296a59222fe9d4.jpg)

纵表

大多数表都是横表，即一行一个记录。有些表，比如单例表，如果纵着填，一行一个字段，会比较舒服。A1为`##column`或`##vertical`表示使用纵表模式。 上面的单例表，以纵表模式填如下。

![Image 14: singleton](https://www.datable.cn/assets/images/singleton-9b7d41bf32c0c214d2baac6cbbd5cea8.jpg)

使用sep读入bean及嵌套bean。

![Image 15: sep_bean](https://www.datable.cn/assets/images/sep_bean-82bc281e78eff8ae7fb8e4b9c0110457.jpg)

使用sep读取普通容器。

![Image 16: sep_bean](https://www.datable.cn/assets/images/sep_container1-4a8bd3a370e5707614dae98f3fcf51e2.jpg)

使用sep读取结构容器。

![Image 17: sep_bean](https://www.datable.cn/assets/images/sep_container2-c8fa336d283b22df2d9e7b70742e8558.jpg)

多级标题头

![Image 18: colloumlimit](https://www.datable.cn/assets/images/multileveltitle-3e1e45452ed00a0da5f65d40f557e62c.jpg)

限定列格式

![Image 19: titlelimit](https://www.datable.cn/assets/images/titlelimit-602bb9196f754dd4a3c55d766a6d301c.jpg)

枚举的列限定格式

![Image 20: titlle_enum](https://www.datable.cn/assets/images/title_enum-5c96663bfbbb1992cd6e2713cec78d1b.jpg)

多态bean列限定格式

![Image 21: title_dynamic_bean](https://www.datable.cn/assets/images/title_dynamic_bean-482422aabcccdac7d7fbd6d369cdbe4c.jpg)

map的列限定格式

![Image 22: title_map](https://www.datable.cn/assets/images/title_map-13d12e479c22398dc7b73af7e44c3232.jpg)

多行字段

![Image 23: map](https://www.datable.cn/assets/images/multiline-d4bd4a85c32fa4b9978c22cd9d0adaa9.jpg)

数据标签过滤

![Image 24: tag](https://www.datable.cn/assets/images/tag-e58c3cc27b698633de18a8f060eb96a3.jpg)

## 其他格式概览[​](https://www.datable.cn/docs/intro#%E5%85%B6%E4%BB%96%E6%A0%BC%E5%BC%8F%E6%A6%82%E8%A7%88 "其他格式概览的直接链接")

以行为树为例，展示json格式下如何配置行为树配置。xml、lua、yaml等格式请参见 [详细文档](http://localhost:3000/docs/intro)。

`{  "id": 10002,  "name": "random move",  "desc": "demo behaviour tree",  "executor": "SERVER",  "blackboard_id": "demo",  "root": {    "$type": "Sequence",    "id": 1,    "node_name": "test",    "desc": "root",    "services": [],    "decorators": [      {        "$type": "UeLoop",        "id": 3,        "node_name": "",        "flow_abort_mode": "SELF",        "num_loops": 0,        "infinite_loop": true,        "infinite_loop_timeout_time": -1      }    ],    "children": [      {        "$type": "UeWait",        "id": 30,        "node_name": "",        "ignore_restart_self": false,        "wait_time": 1,        "random_deviation": 0.5,        "services": [],        "decorators": []      },      {        "$type": "MoveToRandomLocation",        "id": 75,        "node_name": "",        "ignore_restart_self": false,        "origin_position_key": "x5",        "radius": 30,        "services": [],        "decorators": []      }    ]  }}`

## 代码使用预览[​](https://www.datable.cn/docs/intro#%E4%BB%A3%E7%A0%81%E4%BD%BF%E7%94%A8%E9%A2%84%E8%A7%88 "代码使用预览的直接链接")

这儿只简略展示c#、typescript、go、c++ 语言在开发中的用法，更多语言以及更详细的使用范例和代码见[示例项目](https://gitee.com/focus-creative-games/luban_examples)。

*   C# 使用示例

`// 一行代码可以加载所有配置。 cfg.Tables 包含所有表的一个实例字段。var tables = new cfg.Tables(file => return new ByteBuf(File.ReadAllBytes($"{gameConfDir}/{file}.bytes")));// 访问一个单例表Console.WriteLine(tables.TbGlobal.Name);// 访问普通的 key-value 表Console.WriteLine(tables.TbItem.Get(12).Name);// 支持 operator []用法Console.WriteLine(tables.TbMail[1001].Desc);`

*   typescript 使用示例

`// 一行代码可以加载所有配置。 cfg.Tables 包含所有表的一个实例字段。let tables = new cfg.Tables(f => JsHelpers.LoadFromFile(gameConfDir, f))// 访问一个单例表console.log(tables.TbGlobal.name)// 访问普通的 key-value 表console.log(tables.TbItem.get(12).Name)`

*   go 使用示例

`// 一行代码可以加载所有配置。 cfg.Tables 包含所有表的一个实例字段。if tables , err := cfg.NewTables(loader) ; err != nil { println(err.Error()) return}// 访问一个单例表println(tables.TbGlobal.Name)// 访问普通的 key-value 表println(tables.TbItem.Get(12).Name)`

*   c++ 使用示例

`cfg::Tables tables;    if (!tables.load([](ByteBuf& buf, const std::string& s) { return buf.loadFromFile("../GenerateDatas/bytes/" + s + ".bytes"); }))    {        std::cout << "== load fail == " << std::endl;        return;    }    std::cout << tables.TbGlobal->name << std::endl;    std::cout << tables.TbItem.get(12)->name << std::endl;`

## license[​](https://www.datable.cn/docs/intro#license "license的直接链接")

Luban is licensed under the [MIT](https://github.com/focus-creative-games/luban/blob/main/LICENSE) license

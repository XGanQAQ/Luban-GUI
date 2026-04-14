> Source: https://www.datable.cn/docs/manual/traits

# 特性 | Luban

特性 | Luban 

 跳到主要内容 

 Luban 文档 

 GitHub 
 4.x 

 4.x 

 3.x 

 1.x 

 中文 

 中文 

 English 

 搜索 

 介绍 

 新手教程 

 快速上手 

 生成代码和数据 

 集成到项目 

 运行时加载配置 

 使用自定义类型 

 使用容器类型 

 使用列限定与紧凑格式 

 使用数据校验器 

 使用多态类型 

 自动导入table 

 使用指南 

 与classic版本差异 

 设计哲学 

 特性 

 luban.conf 

 schema 逻辑结构 

 配置定义 

 自动导入table 

 命令行工具 

 层级参数机制 

 类型系统 

 类型映射 

 excel格式（初级） 

 excel格式（高级） 

 Excel 紧凑格式 

 非excel数据源 

 代码与数据生成 

 代码风格 

 加载配置 

 数据校验器 

 自定义模板 

 数据tag 

 字段变体（Variants） 

 本地化 

 最佳实践 

 扩展Luban实现 

 FAQ 

 其他 

 使用指南 

 特性 

 版本：4.x 
 本页总览 

 特性 

 Luban内置了丰富的特性。 

 完备的类型系统 ​ 

 基础内置类型 bool,byte,short,int,long,float,double,string,text,datetime 

 容器类型 array,list,set,map 

 自定义枚举和结构，其中结构支持无限层次的类型继承和多态，轻松表达复杂的数据结构 

 可空类型。除了容器以外的类型都支持定义相应的可空类型 

 支持增强的excel格式 ​ 

 支持 true、false、1、0表示bool值 

 用枚举名及别名表示枚举常量。比如用 白绿红之类表示品质，而不是1,2,3这样的magic number 

 支持可空变量。例如对于int?类型，用5表示有效值，用null表示空 

 支持 datetime 数据类型 

 支持结构类型字段。即可以占据多个单元格，也可以一个单元格，甚至是更复杂的配置中的一部分 

 支持拆分单元格。在一个单元格里填写vector3。 

 支持多态。可以方便填写多态数据，比如 Circle,5 或者 Rectangle,3,4 

 支持多行填写结构列表 

 支持多级标题头。方便对应一些比较深的数据。比如 a.b.c 这种。 

 支持丰富的源文件类型 ​ 

 支持excel族文件。 csv、xls、xlsx、xlsm等格式 

 支持从指定excel里的某个单元薄读入。 

 支持json。 每个文件当作一个或多个记录读入 

 支持lua。 每个文件当作一个或多个记录读入 

 支持xml。 每个文件当作一个或多个记录读入 

 支持yml。 每个文件当作一个或多个记录读入 

 支持目录。 递归目录下的所有文件，每个文件当作一个记录读入。允许不同类型的文件混合，比如目录下可以同时有json,lua,xml,excel之类的格式。 

 每个表允许指定多个数据源，可以使用以上所有的组合。 

 多对一。比如可以在一个excel里用不同页签配置不同的表。比如 装备升级表和进阶表都在 装备表.xlsx中 

 一对多。比如任务表可以来 任务1.xlsx，任务2.xlsx 等等多个表 

 多对多。还可以是以上组合，不过实际中很少见） 

 多种导出数据格式支持 ​ 

 导出格式与原始数据解耦 。无论源数据是 excel、lua、xml、json 或者它们的混合, 最终都被以 统一的格式 导出，极大简化了生成代码的复杂性。 目前支持以下几种导出格式： 

 bin格式。与pb格式类似。所占空间最小，加载最快 

 json 格式 

 lua 格式 

 xml 格式 

 yaml 格式 

 protobuf (json + bin) 格式 

 msgpack 

 flatbuffers json格式 

 如果不能满足需求，扩展新的格式也很容易。 

 支持表与字段级别分组 ​ 

 支持自定义分组类型。既支持按分组选择性导出一部分表，也支持选择性导出表中的一部分字段。比如为前后端分别导出他们所用的数据。 

 支持数据标签 ​ 

 支持记录标签。可以为每个记录加标签。比如标签为 "test"，则只在测试导出情况下才导出，适合用于配置一些开发用的测试数据，但对外发布时不导出它们。 

 强大的数据校验能力 ​ 

 完整的数据内建约束检查 

 ref 校验器。检查表引用合法性。比如 Monster表中的dropId必须是合法的 TbDrop表的key 

 path 校验器。检查资源引用合法性。比如 Monster表的icon必须是合法的图标资源。对于防止策划填错极其有用，不再需要运行时才发现资源配置错误了 

 range 校验器。检查数值范围 

 size 校验器。要求容器size必须为指定大小或者范围 

 set 校验器。要求数据必须为集合中一个 

 regex 校验器。要求字符串必须符合某个正则规则 

 not default检验器。 要求值为不能为默认值。 

 index校验器。要求结构列表按照结构某个字段的索引唯一 

 text检验器。要求key为合法的本地化key。 

 扩展新的校验器也非常容易。 

 多种数据表模式 ​ 

 one。即单例表模式 

 map。普通key-value表 

 list。普通的list表，但支持多主键联合索引和多主键独立索引 

 本地化支持 ​ 

 支持 本地化时间 。 配置中的 datetime会根据指定的timezone及localtime转化为正确的utc时间，方便程序处理 

 支持text类型。可以校验本地化key的合法性 

 支持主流的游戏开发语言 ​ 

 由于类型系统支持命令空间，对于大多数语言都生成良好模块化的代码。扩展新的语言支持也非常容易。 

 c++ (11+) 

 c# (.net framework 2+. dotnet core 2+) 

 java (1.6+) 

 go (1.10+) 

 lua (5.1+) 

 typescript (3.0+) 

 python (2.7+ 及 3.0+) 

 gdscript (4.0+) 

 php 

 dart 

 支持主流引擎、热更新方案和游戏平台 ​ 

 unity + c# 

 unity + hybridclr 

 unity + tolua,xlua 

 unity + ILRuntime 

 unreal + c++ 

 unreal + unlua 

 unreal + sluaunreal 

 unreal + puerts 

 cocos2d-x + lua 

 cocos2d-x + ts 

 godot + gdscript 

 微信小程序平台 

 其他家基于js的小程序平台 

 其他所有支持lua的引擎和平台 

 其他所有支持js的引擎和平台 

 生成过程非常快速 ​ 

 即使是大型项目，也能秒级生成。 

 跨平台运行 ​ 

 基于.net 开发，可以高效运行在Windows、Linux、macOS这些主流操作系统。 

 上一页 

 设计哲学 

 下一页 

 luban.conf 

 完备的类型系统 

 支持增强的excel格式 

 支持丰富的源文件类型 

 多种导出数据格式支持 

 支持表与字段级别分组 

 支持数据标签 

 强大的数据校验能力 

 多种数据表模式 

 本地化支持 

 支持主流的游戏开发语言 

 支持主流引擎、热更新方案和游戏平台 

 生成过程非常快速 

 跨平台运行 

 Docs 

 文档 

 Repository 

 luban_examples 

 Excel2TextDiff 

 hybridclr 

 More 

 GitHub 

 Gitee 

 Copyright © 2026 Code Philosophy . All Rights Reserved.

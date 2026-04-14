> Source: https://www.datable.cn/docs/manual/types

# 类型系统 | Luban

类型系统 | Luban 

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

 类型系统 

 版本：4.x 
 本页总览 

 类型系统 

 Luban有完备的类型系统，尤其是 bean支持类型继承和多态 ，使得Luban可以轻松表达任意复杂的数据结构。 

 基本类型 ​ 

 类型 描述 

 bool bool类型， 
 true、false、0、1 
 都能被识别，大小写不敏感，如 
 True、TRUE 
 也是有效值 

 byte 对应c#的byte（uint8_t） 

 short 对应c#的short（int16_t） 

 int 对应c#的int （int32_t） 

 long 对应c#的long （int64_t） 

 float 对应c#的float 

 double 对应c#的double 

 string 对应c#的string 

 text text是一个语法糖类型，而不是独立的类型。等价于 
 string#text=1 
 ，即包含tag 
 text=1 
 的string类型。luban会对该类型数据校验本地化key的合法性 

 datetime 类型为c#里的long，值为自UTC 1970-01-01 00:00:00以来的秒数 

 自定义类型 ​ 

 查看 schema逻辑结构 了解自定义结构的详细设计。 

 类型 描述 

 enum 枚举类，对应c#的enum 

 bean 复合类型，对应c#的class或struct。bean支持 类型继承和多态 

 容器类型 ​ 

 类型 描述 

 array 对应c#的数组，定义方式为 
 array,<eleType> 
 ，eleType不能为可空类型 

 list 对应c#的List，定义方式为 
 list,<eleType> 
 ，eleType不能为可空类型 

 set 对应c#的HashSet，定义方式为 
 set,<eleType> 
 ，eleType不能为可空类型 

 map 对应c#的Dictionary，定义方式为 
 map,<keyType>,<valueType> 
 。keyType只能为基本类型或enum类型，keyType与valueType都不能为可空类型 

 可空类型 ​ 

 基本类型和自定义类型都支持可空类型，容器类型不支持可空，容器的key或value类型也不支持可空。定义方式为 
 <类型>? 
 （如 
 int? 
 , 
 Color? 
 ，与c#的语法相同。 

 类型映射 ​ 

 支持将自定义类型映射到外部现成的类型，例如将MyAccessMode枚举类映射到 System.IO.AccessMode类型；将MyVec3类型映射到UnityEngine.Vector3类型。
生成的代码中所有自定义类型都会映射到外部类型，使用更方便。 

 特殊类型 table ​ 

 为大多数语言生成代码时会为每个table生成一个类，管理这个表的所有数据。 

 特殊类型 tables ​ 

 为大多数语言生成代码时会包含一个所有表的管理类，类名在 
 luban.conf 
 中的 
 targets[xxx].manager 
 字段定义，一般取名为Tables。 

 上一页 

 层级参数机制 

 下一页 

 类型映射 

 基本类型 

 自定义类型 

 容器类型 

 可空类型 

 类型映射 

 特殊类型 table 

 特殊类型 tables 

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

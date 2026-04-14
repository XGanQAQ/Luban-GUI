> Source: https://www.datable.cn/docs/manual/typemapper

# 类型映射 | Luban

类型映射 | Luban 

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

 类型映射 

 版本：4.x 
 本页总览 

 类型映射 

 有时候你希望生成的代码中能直接使用现成的结构类型，而不是使用生成的类型代码。例如vector3是非常常见的类型，你在配置中定义了vector3后，可能希望生成的C#代码中涉及到
vector3类型的地方能直接使用UnityEngine.Vector3，而不是生成的vector3类。Luban支持这种外部类型映射机制，可以将配置类映射到外部现成的enum或者class类型。 

 类型映射的定义方式请阅读文档 schema逻辑结构 及 配置定义 。
由于类型映射影响了代码生成，目前 只有C#代码(cs-bin、cs-xxx-json之类) 支持类型映射。如果其他语言也需要类型，请仿照修改即可。 

 builtin.xml 中提供极好的类型映射的示例。 

 enum类型映射 ​ 

 以AudioType为例，以下配置将它映射到UnityEngine.AudiotType。 

 < enum name = " AudioType " > 
 < var name = " UNKNOWN " value = " 0 " /> 
 < var name = " ACC " value = " 1 " /> 
 < var name = " AIFF " value = " 2 " /> 
 < mapper target = " client " codeTarget = " cs-bin " > 
 < option name = " type " value = " UnityEngine.AudioType " /> 
 </ mapper > 
 </ enum > 

 name为'type'的option配置的value字段指定了类型映射的目标C#类型。注意，必须保存枚举项的值与映射的枚举类型的枚举项的值完全一致，因为enum的类型映射的
实现方式为先读取出配置AudioType，再类型强转为UnityEngine.AudioType。 

 bean映射 ​ 

 以 vector2、vector3、vector4为例，以下配置将它们映射到UnityEngine.Vector{2,3,4}。 

 < bean name = " vector2 " valueType = " 1 " sep = " , " > 
 < var name = " x " type = " float " /> 
 < var name = " y " type = " float " /> 
 < mapper target = " client " codeTarget = " cs-bin " > 
 < option name = " type " value = " UnityEngine.Vector2 " /> 
 < option name = " constructor " value = " ExternalTypeUtil.NewVector2 " /> 
 </ mapper > 
 </ bean > 
 < bean name = " vector3 " valueType = " 1 " sep = " , " > 
 < var name = " x " type = " float " /> 
 < var name = " y " type = " float " /> 
 < var name = " z " type = " float " /> 
 < mapper target = " client " codeTarget = " cs-bin " > 
 < option name = " type " value = " UnityEngine.Vector3 " /> 
 < option name = " constructor " value = " ExternalTypeUtil.NewVector3 " /> 
 </ mapper > 
 </ bean > 
 < bean name = " vector4 " valueType = " 1 " sep = " , " > 
 < var name = " x " type = " float " /> 
 < var name = " y " type = " float " /> 
 < var name = " z " type = " float " /> 
 < var name = " w " type = " float " /> 
 < mapper target = " client " codeTarget = " cs-bin " > 
 < option name = " type " value = " UnityEngine.Vector4 " /> 
 < option name = " constructor " value = " ExternalTypeUtil.NewVector4 " /> 
 </ mapper > 
 </ bean > 

 name为'type'的option项与enum完全相同，配置了目标类型。由于bean并不能类型强转，因此相比enum需要提供一个自定义的强转（或者构造）函数，将
配置类型转换为映射类型。由'constructor'配置项提供这个参数。 

 生成类型映射后的代码 ​ 

 对于不同的target，即使是同一种语言，前后端不一定使用相同的类型映射。例如对于vector3，前端可能想映射到UnityEngine.Vector3，而
后端期望直接使用默认生成的类型或者映射到System.Numerics.Vector3。因此需要有机制可以区分这种情况。 

 目前使用 mapper的target和codeTarget参数组合来表达映射需求。当 命令行的 
 -t $target 
 参数与 
 -c $codeTarget 
 参数分别与mapper的target
及codeTarget的值匹配时，表示需要执行当前mapper指定的映射。 

 提示 

 mapper的target和codeTarget参数都可以是多个值，如target="client,server,all"，codeTarget="cs-bin,cs-dotnet-json"。 
 -t 
 和 
 -c 
 参数
只需要是其中一个即可满足匹配。 

 上一页 

 类型系统 

 下一页 

 excel格式（初级） 

 enum类型映射 

 bean映射 

 生成类型映射后的代码 

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

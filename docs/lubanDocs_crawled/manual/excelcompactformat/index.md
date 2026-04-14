> Source: https://www.datable.cn/docs/manual/excelcompactformat

# Excel 紧凑格式 | Luban

Excel 紧凑格式 | Luban 

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

 Excel 紧凑格式 

 版本：4.x 
 本页总览 

 Excel 紧凑格式 

 介绍 ​ 

 如果某个数据是非原子数据（如bean或容器），并且它被限定到某些单元格列范围或者是sep分割的数据的一部分，则它的解析方式为紧凑格式。 

 自luban v4.0.0版本之前，只支持流式格式这一种紧凑格式。自v4.0.0起，支持以下紧凑格式： 

 流式格式 

 lite 简洁格式 

 json格式 

 lua格式 

 指定excel数据使用的紧凑格式 ​ 

 危险 

 注意是在字段名上定义format属性，而不是在type上！ 

 在字段标题头上配置format属性来指定解析紧凑数据的格式，如 
 position#format=lite 
 。如果没有为字段指定format则默认紧凑格式为stream流式格式。 

 紧凑格式有以下类型： 

 stream （流式格式） 

 lite （简洁格式） 

 json 

 lua 

 在填写不复杂的复合数据时使用流式格式最为简洁。当复合数据包含复杂的深层次嵌套数据时，流式格式由于没有显式的边界而变得难以理解。此时lite格式更清晰易读，也更容易掌握使用。
json格式和lua格式由于需要填写字段名，数据比较啰嗦，建议只在特殊场合下使用。 

 流式格式 ​ 

 流式格式是默认的紧凑格式，即未在标题头指定 
 format 
 属性时，使用流式格式解析数据。在luban v4.0.0以前，只支持流式格式。 

 数据格式 ​ 

 流式格式的读取数据规则如下： 

 bool false,true 

 int,float 之类 有效整数值 

 string 用""表示长度为0的字符串，用其他非值表示值本身 

 enum 非空有效值 

 bean 用流式格式按顺序读入每个字段 

 多态bean类型 先读入一个字符串，可以是具体的子类名或者子类别名，然后再根据子类名，流式读入该类型的每个字段。 

 可空bean类型 先读入一个字符串，如果是bean的类型名或者'{}'，则流格式读入该类型的所有字段；如果为null，则表示空，结束读取；其他情况则当作非空bean，从刚才读入的字符串开始，读入该类型的字段。
以vec3类型为例， 
 1,2,3 
 、 
 null 
 、 
 {},1,2,3 
 、 
 vec3,1,2,3 
 都是有效数据 

 array,list,set 如果流结束或者下一个读入的为'}'，则读取结束，否则用流格式读入element_type，如此循环。 

 map 如果流结束或者下一个读入的为'}'，则读取结束，否则递归读入key_type和value_type，如此循环。 

 由于流式格式无法区分 
 默认单元格 
 和 
 空白单元格 
 ，因此不支持 默认值 语义，会忽略所有空白单元格。对于默认值必须填上有效值默认值来表示数据，而不能留空来表达。 

 示例如下。 

 图中标红两行，试图用空白单元格表达bool和string的默认值，由于流式格式下这些单元格或者sep分割出的长度为0的字符串会被忽略，导致出现
数据不足的解析错误。 

 sep分割机制 ​ 

 我们包含多个原子数据元素的数据结构称之为复合结构。对于复合结构，如果每个原子数据都占据一个单元格，则可能列数太多而不便阅读。
使用sep分割机制可以达到在一个单元格内填写复杂数据的目的。 

 写法为 
 sep=<char1><char2><char3>... 
 。sep可以是一个字符，或者多个字符。当sep为多个字符时，表示其中任意一个都是分割符，
而不是将多个字符整体当作分割符。 

 提示 

 由于 
 # 
 和 
 & 
 字符被用作attrs和tags的分割符，如果希望用 
 # 
 或 
 & 
 作为分割符，必须在前面添加转义字符 
 \ 
 ，例如 
 (list#sep=\#),int 
 。 

 sep可以出现在以下位置： 

 在excel的字段名上。如 
 x#sep=, 

 bean的tags上，如 
 <bean name="Item" tags="sep=,"> 

 在field的type字段的tag上，如 
 type="Vec#sep=," 
 ， 
 type="(list#sep=|),int" 
 、 
 type="list,(Vec#sep=,)" 
 、 
 type="(list#sep=|),(Vec#sep=,)" 

 当出现在excel字段名上时，会用该sep自动拆分该字段名的列范围内的每个单元格，形成数据流，再使用流式格式（下面小节会介绍）依次读取这些数据。 

 当出现在bean的tags上时，表示所有读取该bean的地方，都以字符串形式提供整个bean的数据，用分割符拆分后流式读取 

 当出现在type的tag上时，会将下一个读到的数据，当作该type对应的整体数据，接着使用sep拆分这个数据，再用流式格式读取type对应的数据。 

 当type为容器时，在自身(如list)或者元素类型上都可以施加sep属性。如果在自身上施加sep属性，则表示将下一个读入的字符串当作整个容器的数据，
同时用sep去分割这个字符串，再用流式格式依次读取容器的内容。如果在元素类型上施加sep属性，则将每个读入的内容当作一个元素的数据，再用sep
去分割这个数据，以流式格式读取元素类型对应的数据，重复这个操作，直到没有数据或者遇到容器结束符 
 } 
 为止。 

 示例：使用sep读入bean及嵌套bean。 

 示例：使用sep读取普通容器。 

 示例：使用sep读取结构容器。 

 lite格式 ​ 

 lite是luban独有的数据格式，适合表达非常复杂嵌套的数据结构。lite格式是无字段名的格式，因此相比json和lua格式填写更为简洁，而且解析更高效。实践中建议
简单的复合数据使用流式格式，复杂的复合数据使用lite格式，仅在必要的情况下使用json和lua格式。详细文档见 lite格式 。 

 在标题头上指定format属性值为lite时使用该格式解析数据，如position#format=lite。 

 格式举例： 

 vec3数据 (1.0,2.0,3.0) 填法为 
 {1.0, 2.0, 3.0} 
 。 

 class User{ int id; string name; vec3 pos;} 
 填法为 
 {1, xxxx, {1,2,3}} 
 。 

 json 格式 ​ 

 以json格式填写数据。详细文档见 json格式 。 

 在标题头上指定format属性值为json时使用该格式解析数据，如 
 position#format=json 
 。 

 格式举例： 

 vec3数据 
 (1.0,2.0,3.0) 
 填法为 
 {"x":1.0, "y":2.0, "z":3.0} 
 。 

 class User{ int id; string name; vec3 pos;} 
 填法为 
 {"id":1, "name":"xxxx", "pos":{"x":1, "y":2, "z":3}} 
 。 

 lua 格式 ​ 

 以lua格式填写数据。详细文档见 lua格式 。 

 在标题头上指定format属性值为lua时使用该格式解析数据，如 
 position#format=lua 
 。 

 格式举例： 

 vec3数据 
 (1.0,2.0,3.0) 
 以lua格式填法为 
 {x=1.0, y=2.0, z=3.0} 
 。 

 class User{ int id; string name; vec3 pos;} 
 填法为 
 {id=1, name="xxxx", pos={x=1, y=2, z=3}} 
 。 

 上一页 

 excel格式（高级） 

 下一页 

 非excel数据源 

 介绍 

 指定excel数据使用的紧凑格式 

 流式格式 

 数据格式 

 sep分割机制 

 lite格式 

 json 格式 

 lua 格式 

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

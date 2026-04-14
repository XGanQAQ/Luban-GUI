> Source: https://www.datable.cn/docs/other/changelog

Title: change log | Luban

URL Source: https://www.datable.cn/docs/other/changelog

Markdown Content:
版本：4.x

## 2022[​](https://www.datable.cn/docs/other/changelog#2022 "2022的直接链接")

### 2022.7.26[​](https://www.datable.cn/docs/other/changelog#2022726 "2022.7.26的直接链接")

*   新增cpp的联合多主键代码生成支持

### 2022.07.19[​](https://www.datable.cn/docs/other/changelog#20220719 "2022.07.19的直接链接")

*   新增bidx，即binary格式的索引文件输出。方便优化按需加载

### 2022.07.18[​](https://www.datable.cn/docs/other/changelog#20220718 "2022.07.18的直接链接")

*   新增bson导出格式
*   新增嵌套容器支持（目前只支持c#语言）

### 2022.06.09[​](https://www.datable.cn/docs/other/changelog#20220609 "2022.06.09的直接链接")

*   新增gdscript语言的导出支持

### 2022.03.12[​](https://www.datable.cn/docs/other/changelog#20220312 "2022.03.12的直接链接")

*   新增data_yaml yaml导出格式支持

### 2022.02.16[​](https://www.datable.cn/docs/other/changelog#20220216 "2022.02.16的直接链接")

*   支持vector2,3,4及datetime类型的externaltype映射

### 2022.02.12[​](https://www.datable.cn/docs/other/changelog#20220212 "2022.02.12的直接链接")

*   excel、json、xml等数据格式使用$type之类的字段名来读取多态类型名

### 2022.02.11[​](https://www.datable.cn/docs/other/changelog#20220211 "2022.02.11的直接链接")

*   对容器元素类型的ref也生成相应的ref变量及resolve代码，目前只支持c#

### 2022.02.10[​](https://www.datable.cn/docs/other/changelog#20220210 "2022.02.10的直接链接")

*   删除对简单原生数据列表类型"list,int"之类的自动sep
*   统一#与&的用法，不再允许因为兼容而混乱的用法了
*   调整externaltype设计

### 2022.01.29[​](https://www.datable.cn/docs/other/changelog#20220129 "2022.01.29的直接链接")

*   允许bean的跨模块继承！较大改动的特性！

### 2022.01.28[​](https://www.datable.cn/docs/other/changelog#20220128 "2022.01.28的直接链接")

*   excel格式中支持定义多态类型

### 2022.01.26[​](https://www.datable.cn/docs/other/changelog#20220126 "2022.01.26的直接链接")

*   root.xml新增配置项 option
*   gen_types 新增 code_cs_unity_editor_json
*   enum 支持空单元格取默认值，但要求必须包含值为0的项

### 2022.01.06[​](https://www.datable.cn/docs/other/changelog#20220106 "2022.01.06的直接链接")

*   path校验支持子资源文件格式，如 Assets/Icons/GoodsIcon.spriteatlas[10000]

## 2021[​](https://www.datable.cn/docs/other/changelog#2021 "2021的直接链接")

### 2021.12.22[​](https://www.datable.cn/docs/other/changelog#20211222 "2021.12.22的直接链接")

*   新增 python27支持

### 2021.12.18[​](https://www.datable.cn/docs/other/changelog#20211218 "2021.12.18的直接链接")

*   新增Unity asset格式支持，可以从asset中解析出目标数据
*   允许为目录数据源指定参数，这些参数会作用于所有子数据文件
*   新增 --output:tables, --output:include_tables, --output:exclude_tables 用于指定包含或者排除某些表
*   新增convert模板支持

### 2021.12.09[​](https://www.datable.cn/docs/other/changelog#20211209 "2021.12.09的直接链接")

*   新增 [xxx, xxx] 这种匹配的格式来标识一个多列字段，方便csv这种不支持单元格合并的格式标识多列字段

### 2021.12.08[​](https://www.datable.cn/docs/other/changelog#20211208 "2021.12.08的直接链接")

*   新增 protobuf3 schema生成及对应的json格式导出

### 2021.12.06[​](https://www.datable.cn/docs/other/changelog#20211206 "2021.12.06的直接链接")

*   新增msgpack导出格式支持
*   新增flatbuffers定义生成及相应json导出格式支持(binary格式暂时不支持，但特地生成了转换命令文件，可以使用flat将json格式转成binary)

### 2021.12.05[​](https://www.datable.cn/docs/other/changelog#20211205 "2021.12.05的直接链接")

*   新增refgroup支持，方便很多字段都引用同一组表的情形
*   调整sep设计，从列名移到type，更加灵活并且统一

### 2021.12.04[​](https://www.datable.cn/docs/other/changelog#20211204 "2021.12.04的直接链接")

*   新增gen_types类型 code_template，可以新增自定义模板类型了
*   新增与code_template配套的参数 --template:data:dir
*   不再生成空的模板生成结果文件

### 2021.12.02[​](https://www.datable.cn/docs/other/changelog#20211202 "2021.12.02的直接链接")

*   新增外部 enum与bean支持。重大特性！

### 2021.12.01[​](https://www.datable.cn/docs/other/changelog#20211201 "2021.12.01的直接链接")

*   新增table.mode='list'类型， 此类型表支持 无主键，联合多主键，独立多主键 三种模式。
*   ref相应调整，适合新增的list类型表模式

### 2021.11.30[​](https://www.datable.cn/docs/other/changelog#20211130 "2021.11.30的直接链接")

*   新增protobuf 定义及数据格式导出。优秀！！！
*   Luban.Server新增 --disable_cache 模式，禁用生成缓存，方便调试模板
*   excel格式支持对于列表水平展开时，每个元素单独指定多级标题头，方便填写

### 2021.11.22[​](https://www.datable.cn/docs/other/changelog#20211122 "2021.11.22的直接链接")

*   新增 validator: set 。检查值是否在某一集合内。

### 2021.11.20[​](https://www.datable.cn/docs/other/changelog#20211120 "2021.11.20的直接链接")

*   新增"unchecked"Tag，用于指示校验器不检查记录
*   移除"no"Tag

### 2021.11.18[​](https://www.datable.cn/docs/other/changelog#20211118 "2021.11.18的直接链接")

*   新增size校验器

### 2021.11.16[​](https://www.datable.cn/docs/other/changelog#20211116 "2021.11.16的直接链接")

*   考虑到##field容易写错，新增 ##var和##var 作为子字段行的标签
*   lua、yaml都支持json那样从子字段读入数据

### 2021.11.15[​](https://www.datable.cn/docs/other/changelog#20211115 "2021.11.15的直接链接")

*   新增datetime的别名time
*   修复因为.net 6对TimeZone相关调整导致datetime计算错误的bug
*   Luban.Server和Luban.ClientServer新增 --timezone 参数用于指定默认时区
*   json数据源支持从json子字段读入bean或者list,bean（即多个表的数据可以配到一个json文件中，虽然实际中罕见这么做），支持用*@xxx.json形式将json当作一个记录列表读入 list,bean形式的数据
*   类型多态id字段名由ID改名为**ID**,避免与常见的字段名冲突而产生编译错误

### 2021.11.09[​](https://www.datable.cn/docs/other/changelog#20211109 "2021.11.09的直接链接")

框架升级到.net 6

### 2021.11.05[​](https://www.datable.cn/docs/other/changelog#20211105 "2021.11.05的直接链接")

*   使用ClosedXml替换SpreedSheet库，解决mac下convert_xlsx失败的问题
*   新增 --input_convert_data_dir,用转换数据convert_xxxx生成文件代替table.input中指定的文件为数据源
*   convert_xlsx 生成的文件中，记录按key排序

### 2021.11.2[​](https://www.datable.cn/docs/other/changelog#2021112 "2021.11.2的直接链接")

*   新增 --generateonly 选项，生成但不下载结果，用于单纯的配置合法性检查

### 2021.1.29[​](https://www.datable.cn/docs/other/changelog#2021129 "2021.1.29的直接链接")

*   放松对enum的格式要求，可以填枚举值对应的整数了

### 2021.10.28[​](https://www.datable.cn/docs/other/changelog#20211028 "2021.10.28的直接链接")

*   将Excel2TextDiff拆成独立项目 [Excel2TextDiff](https://github.com/focus-creative-games/Excel2TextDiff)

### 2021.10.27[​](https://www.datable.cn/docs/other/changelog#20211027 "2021.10.27的直接链接")

*   重构excel数据源格式。 更加清晰优雅美观

### 2021.10.25[​](https://www.datable.cn/docs/other/changelog#20211025 "2021.10.25的直接链接")

*   新增 --naming_convention_xxxx 相关选项，可以为生成的代码指定命名风格了（camelCase,PascalCase,under_scores等等）
*   优化 解析excel格式出错时的错误信息，给出准确的错误位置

### 2021.10.22[​](https://www.datable.cn/docs/other/changelog#20211022 "2021.10.22的直接链接")

*   新增 --output_compact_json， 生成紧凑的json数据
*   大幅优化了LubanAssistant 的加载和保存速度。（慢如牛到快如闪电)

### 2021.10.20[​](https://www.datable.cn/docs/other/changelog#20211020 "2021.10.20的直接链接")

*   LubanAssistant 支持 多行列表

### 2021.10.18[​](https://www.datable.cn/docs/other/changelog#20211018 "2021.10.18的直接链接")

*   table新增output属性，可以指定生成文件名

### 2021.10.13[​](https://www.datable.cn/docs/other/changelog#20211013 "2021.10.13的直接链接")

*   新增LubanAssistant插件

### 2021.10.11[​](https://www.datable.cn/docs/other/changelog#20211011 "2021.10.11的直接链接")

*   移除 --export_test_data ，新增 --export_exclude_tags 选项，允许更加灵活地过滤导出数据

### 2021.9.1[​](https://www.datable.cn/docs/other/changelog#202191 "2021.9.1的直接链接")

*   新增 --data_file_extension选项，允许指定生成的数据文件的后缀

### 2021.8.28[​](https://www.datable.cn/docs/other/changelog#2021828 "2021.8.28的直接链接")

*   新增 erlang 导出数据格式

### 2021.8.27[​](https://www.datable.cn/docs/other/changelog#2021827 "2021.8.27的直接链接")

*   支持数据模板生成，新增 gen_types类型 data_template及相应配套选项--template_name
*   新增excel字段的default选项，允许为空白单元格指定默认址

### 2021.8.26[​](https://www.datable.cn/docs/other/changelog#2021826 "2021.8.26的直接链接")

*   为enum,bean,table等大多数对象添加attrs属性，方便代码模板中自定义处理
*   Luban.Server的 命令行参数-t 语义调整为额外的模板搜索路径。如果指定，优先搜索此路径，再搜索默认的Templates路径。
*   支持动态本地化

### 2021.8.25[​](https://www.datable.cn/docs/other/changelog#2021825 "2021.8.25的直接链接")

*   新增 Luban.ClientServer一体化程序，可以不必部属Luban.Server，一个程序就能完成生成。

### 2021.8.24[​](https://www.datable.cn/docs/other/changelog#2021824 "2021.8.24的直接链接")

*   支持代码模板

### 2021.8.13[​](https://www.datable.cn/docs/other/changelog#2021813 "2021.8.13的直接链接")

*   新增data_json2格式。 与data_json区别在于 data_json的map导出成[[k1,v1], ...] 而data_json2的map导出格式为 { k1:v1, ... }

### 2021.8.12[​](https://www.datable.cn/docs/other/changelog#2021812 "2021.8.12的直接链接")

*   新增 yaml 数据源支持

### 2021.8.9[​](https://www.datable.cn/docs/other/changelog#202189 "2021.8.9的直接链接")

*   在win下自动禁用console的快速编辑模式

### 2021.8.4[​](https://www.datable.cn/docs/other/changelog#202184 "2021.8.4的直接链接")

*   支持在excel中定义bean
*   支持在excel定义enum类型
*   优化cfg日志，不再打印吓人的堆栈信息

### 2021.8.3[​](https://www.datable.cn/docs/other/changelog#202183 "2021.8.3的直接链接")

*   新增 --cs:use_unity_vector命令行参数，允许使用 UnityEngine.Vector{2,3,4} 而不是 System.Numerics.Vector{2,3,4}

### 2021.8.2[​](https://www.datable.cn/docs/other/changelog#202182 "2021.8.2的直接链接")

*   解决不同平台下生成json和lua数据文件（换行符）不同的问题， 统一为 \n
*   支持从excel中读入Table列表定义。这样大多数情况下，只通过修改excel文件就能完成添加新表。

### 2021.7.30[​](https://www.datable.cn/docs/other/changelog#2021730 "2021.7.30的直接链接")

*   支持从excel文件的标题头里直接读取定义，定义和数据一体了。简化写记录定义的麻烦

### 2021.7.29[​](https://www.datable.cn/docs/other/changelog#2021729 "2021.7.29的直接链接")

*   解决Luban.Server和Luban.Client在非大陆地区运行错误日志乱码的问题

### 2021.7.8[​](https://www.datable.cn/docs/other/changelog#202178 "2021.7.8的直接链接")

*   新增go语言json数据加载支持(code_go_json)
*   修复 go对可空变量的支持

### 2021.6.19[​](https://www.datable.cn/docs/other/changelog#2021619 "2021.6.19的直接链接")

*   新增Excel2TextDiff工具项目，用于将excel转换为text或者直接将两个excel文件转换为text后再diff
*   excel、lua、xml、json 支持标签过滤

### 2021.6.17[​](https://www.datable.cn/docs/other/changelog#2021617 "2021.6.17的直接链接")

*   支持 主版本+分支版本的 多分支数据合并，用于发布多地区版本极其有用
*   支持 静态本地化

### 2021.5.8[​](https://www.datable.cn/docs/other/changelog#202158 "2021.5.8的直接链接")

*   Luban.Client支持watch机制，监测相关定义或数据改变后自动重新生成。节省迭代时间。

### 2021.4.12[​](https://www.datable.cn/docs/other/changelog#2021412 "2021.4.12的直接链接")

*   优化导出的json数据文件。unicode字符以原字符方式导出而不是\uxxxx这种格式。

### 2021.4.8[​](https://www.datable.cn/docs/other/changelog#202148 "2021.4.8的直接链接")

*   新增gen_type类型 data_json_monolithic:将所有json文件生成到一个文件

*   [2022](https://www.datable.cn/docs/other/changelog#2022)
    *   [2022.7.26](https://www.datable.cn/docs/other/changelog#2022726)
    *   [2022.07.19](https://www.datable.cn/docs/other/changelog#20220719)
    *   [2022.07.18](https://www.datable.cn/docs/other/changelog#20220718)
    *   [2022.06.09](https://www.datable.cn/docs/other/changelog#20220609)
    *   [2022.03.12](https://www.datable.cn/docs/other/changelog#20220312)
    *   [2022.02.16](https://www.datable.cn/docs/other/changelog#20220216)
    *   [2022.02.12](https://www.datable.cn/docs/other/changelog#20220212)
    *   [2022.02.11](https://www.datable.cn/docs/other/changelog#20220211)
    *   [2022.02.10](https://www.datable.cn/docs/other/changelog#20220210)
    *   [2022.01.29](https://www.datable.cn/docs/other/changelog#20220129)
    *   [2022.01.28](https://www.datable.cn/docs/other/changelog#20220128)
    *   [2022.01.26](https://www.datable.cn/docs/other/changelog#20220126)
    *   [2022.01.06](https://www.datable.cn/docs/other/changelog#20220106)

*   [2021](https://www.datable.cn/docs/other/changelog#2021)
    *   [2021.12.22](https://www.datable.cn/docs/other/changelog#20211222)
    *   [2021.12.18](https://www.datable.cn/docs/other/changelog#20211218)
    *   [2021.12.09](https://www.datable.cn/docs/other/changelog#20211209)
    *   [2021.12.08](https://www.datable.cn/docs/other/changelog#20211208)
    *   [2021.12.06](https://www.datable.cn/docs/other/changelog#20211206)
    *   [2021.12.05](https://www.datable.cn/docs/other/changelog#20211205)
    *   [2021.12.04](https://www.datable.cn/docs/other/changelog#20211204)
    *   [2021.12.02](https://www.datable.cn/docs/other/changelog#20211202)
    *   [2021.12.01](https://www.datable.cn/docs/other/changelog#20211201)
    *   [2021.11.30](https://www.datable.cn/docs/other/changelog#20211130)
    *   [2021.11.22](https://www.datable.cn/docs/other/changelog#20211122)
    *   [2021.11.20](https://www.datable.cn/docs/other/changelog#20211120)
    *   [2021.11.18](https://www.datable.cn/docs/other/changelog#20211118)
    *   [2021.11.16](https://www.datable.cn/docs/other/changelog#20211116)
    *   [2021.11.15](https://www.datable.cn/docs/other/changelog#20211115)
    *   [2021.11.09](https://www.datable.cn/docs/other/changelog#20211109)
    *   [2021.11.05](https://www.datable.cn/docs/other/changelog#20211105)
    *   [2021.11.2](https://www.datable.cn/docs/other/changelog#2021112)
    *   [2021.1.29](https://www.datable.cn/docs/other/changelog#2021129)
    *   [2021.10.28](https://www.datable.cn/docs/other/changelog#20211028)
    *   [2021.10.27](https://www.datable.cn/docs/other/changelog#20211027)
    *   [2021.10.25](https://www.datable.cn/docs/other/changelog#20211025)
    *   [2021.10.22](https://www.datable.cn/docs/other/changelog#20211022)
    *   [2021.10.20](https://www.datable.cn/docs/other/changelog#20211020)
    *   [2021.10.18](https://www.datable.cn/docs/other/changelog#20211018)
    *   [2021.10.13](https://www.datable.cn/docs/other/changelog#20211013)
    *   [2021.10.11](https://www.datable.cn/docs/other/changelog#20211011)
    *   [2021.9.1](https://www.datable.cn/docs/other/changelog#202191)
    *   [2021.8.28](https://www.datable.cn/docs/other/changelog#2021828)
    *   [2021.8.27](https://www.datable.cn/docs/other/changelog#2021827)
    *   [2021.8.26](https://www.datable.cn/docs/other/changelog#2021826)
    *   [2021.8.25](https://www.datable.cn/docs/other/changelog#2021825)
    *   [2021.8.24](https://www.datable.cn/docs/other/changelog#2021824)
    *   [2021.8.13](https://www.datable.cn/docs/other/changelog#2021813)
    *   [2021.8.12](https://www.datable.cn/docs/other/changelog#2021812)
    *   [2021.8.9](https://www.datable.cn/docs/other/changelog#202189)
    *   [2021.8.4](https://www.datable.cn/docs/other/changelog#202184)
    *   [2021.8.3](https://www.datable.cn/docs/other/changelog#202183)
    *   [2021.8.2](https://www.datable.cn/docs/other/changelog#202182)
    *   [2021.7.30](https://www.datable.cn/docs/other/changelog#2021730)
    *   [2021.7.29](https://www.datable.cn/docs/other/changelog#2021729)
    *   [2021.7.8](https://www.datable.cn/docs/other/changelog#202178)
    *   [2021.6.19](https://www.datable.cn/docs/other/changelog#2021619)
    *   [2021.6.17](https://www.datable.cn/docs/other/changelog#2021617)
    *   [2021.5.8](https://www.datable.cn/docs/other/changelog#202158)
    *   [2021.4.12](https://www.datable.cn/docs/other/changelog#2021412)
    *   [2021.4.8](https://www.datable.cn/docs/other/changelog#202148)

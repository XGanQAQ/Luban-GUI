> Source: https://www.datable.cn/docs/manual/defaultschemacollector

Title: 配置定义 | Luban

URL Source: https://www.datable.cn/docs/manual/defaultschemacollector

Markdown Content:
## 配置定义

Luban有一套独立于具体实现的[Schema逻辑结构](https://www.datable.cn/docs/manual/schema)实现。对怎么定义配置没有要求，只要最终的定义能被 加载解析为schema逻辑结构，就能被统一处理。Luban.Schema.Builtin模块提供了一个默认的SchemaCollector 实现。如果不能满足要求，开发者可以实现自定义的SchemaCollector，适应自己的项目需要。

本文档介绍DefaultSchemaCollector实现的配置定义格式。由于DefaultSchemaCollector是[Schema逻辑结构](https://www.datable.cn/docs/manual/schema) 的实现，**大多数配置项是一一对应的**。因此除了特殊情况，不再介绍这些字段的具体含义，请直接查阅该文档。

## excel定义文件[​](https://www.datable.cn/docs/manual/defaultschemacollector#excel%E5%AE%9A%E4%B9%89%E6%96%87%E4%BB%B6 "excel定义文件的直接链接")

由于excel表格适用于表达相似的定义，因此enum、bean、table需要分别在不同的单元薄或者文件中定义。

### enum类型定义文件[​](https://www.datable.cn/docs/manual/defaultschemacollector#enum%E7%B1%BB%E5%9E%8B%E5%AE%9A%E4%B9%89%E6%96%87%E4%BB%B6 "enum类型定义文件的直接链接")

一般来说，定义文件都放到Defines文件下，但为了方便策划编写，excel类型定义文件也可以放到Datas目录。

典型的enum文件格式如下：

![Image 1: excel_enum](https://www.datable.cn/assets/images/excel_enum-d695422ca6af65443ffcbb25be85d24b.jpg)

表字段说明：

| 字段 | 可空 | 默认值 | 说明 |
| --- | --- | --- | --- |
| full_name | 否 |  | 类型全名，即可以是不包含命名空间，如 Hello，也可以包含命名空间如 item.Item |
| flags | 是 | false | 等价schema逻辑结构中isFlags字段 |
| unique | 是 | false | 当前enum内的所有枚举值必须唯一，等价schema逻辑结构中isUniqueItemId字段 |
| comment | 是 |  |  |
| tags | 是 |  | 填写方式为 key1=value1#key2=value2... |
| items | 否 |  | 枚举项列表 |

枚举项字段说明：

| 字段 | 可空 | 默认值 | 说明 |
| --- | --- | --- | --- |
| name | 否 |  | 枚举项名 |
| alias | 是 |  | 别名 |
| value | 是 |  | 枚举值，可以为10进制或者16进制整数，如123、0x2A。也可以为其他枚举项的或值，如 A|B|C |
| comment | 是 |  |  |
| tags | 是 |  | 同其他tags写法 |

### bean类型定义文件[​](https://www.datable.cn/docs/manual/defaultschemacollector#bean%E7%B1%BB%E5%9E%8B%E5%AE%9A%E4%B9%89%E6%96%87%E4%BB%B6 "bean类型定义文件的直接链接")

典型的bean文件格式如下：

![Image 2: excel_bean](https://www.datable.cn/assets/images/excel_bean-063b68900f015d7b522d3cdfc06540fc.jpg)

表字段说明：

| 字段 | 可空 | 默认值 | 说明 |
| --- | --- | --- | --- |
| full_name | 否 |  | 类型全名，即可以是不包含命名空间，如 Hello，也可以包含命名空间如 item.Item |
| parent | 是 |  | 父类名，如果名字不包含命名空间，会优先从当前命名空间找，再从全局命名空间找 |
| valueType | 是 | false | 对应schema逻辑结构中isValueType字段 |
| sep | 是 |  |  |
| alias | 是 | false |  |
| comment | 是 |  |  |
| group | 是 |  |  |
| tags | 是 |  |  |
| fields | 否 |  | 字段列表 |

字段定义的字段说明：

| 字段 | 可空 | 默认值 | 说明 |
| --- | --- | --- | --- |
| name | 否 |  |  |
| type | 否 |  |  |
| group | 是 |  |  |
| comment | 是 |  |  |
| tags | 是 |  |  |

### table类型定义文件[​](https://www.datable.cn/docs/manual/defaultschemacollector#table%E7%B1%BB%E5%9E%8B%E5%AE%9A%E4%B9%89%E6%96%87%E4%BB%B6 "table类型定义文件的直接链接")

典型的table文件格式如下：

![Image 3: excel_table](https://www.datable.cn/assets/images/excel_table-0c1f600c7a46f5db89edc0c2912a2999.jpg)

表字段说明：

| 字段 | 可空 | 默认值 | 说明 |
| --- | --- | --- | --- |
| full_name | 否 |  | 类型全名，即可以是不包含命名空间，如 Hello，也可以包含命名空间如 item.Item |
| value_type | 否 |  | 表记录类型 |
| read_schema_from_file | 是 | false | 是否从input的excel文件的标题头行读取value_type定义。此时不能再定义value_type对应的bean，否则会出现定义重复的错误 |
| input | 否 |  | 对应schema逻辑结构中inputFiles字段 |
| index | 是 |  |  |
| mode | 是 |  |  |
| comment | 是 |  |  |
| group | 是 |  |  |
| tags | 是 |  |  |
| output | 否 |  | 对应schema逻辑结构中outputFileName字段 |

## xml定义文件[​](https://www.datable.cn/docs/manual/defaultschemacollector#xml%E5%AE%9A%E4%B9%89%E6%96%87%E4%BB%B6 "xml定义文件的直接链接")

由于xml天然可以填写不同结构的数据，因此xml文件不需要像excel文件那样区分类型，要求import的type属性未定义或者为空。

典型的xml定义文件如下：

`<module name="item"><enum name="Quality">  <var name="WHITE" alias="白"/>  <var name="RED" alias="红"/>  <var name="GREEN" alias="绿"/></enum><bean name="Item">  <var name="id" type="int"/>  <var name="count" type="int"/></bean><bean name="Item2">  <var name="id" type="int"/>  <var name="count" type="int"/>  <var name="desc" type="string"/></bean><table name="TbItem" value="Item" input="item.xlsx"/><table name="TbItem2" value="Item2" input="item2.xlsx"/><refgroup name="item.TbItem,item.TbItem2"/><enum name="AudioType">    <var name="UNKNOWN" value="0"/>    <var name="ACC" value="1"/>    <var name="AIFF" value="2"/>    <mapper target="client" codeTarget="cs-bin">        <option name="type" value="UnityEngine.AudioType"/>    </mapper></enum><bean name="vector2" valueType="1" sep=",">    <var name="x" type="float"/>    <var name="y" type="float"/>    <mapper target="client" codeTarget="cs-bin">        <option name="type" value="UnityEngine.Vector2"/>        <option name="constructor" value="ExternalTypeUtil.NewVector2"/>    </mapper></bean><module name="subModule">  <bean name="SubModuleType">    <var name="id" type="int"/>    <var name="count" type="int"/>  </bean>  <table name="TbSubItem" value="SubModuleType" input="submodule.xlsx"/></module></module>`

### module定义[​](https://www.datable.cn/docs/manual/defaultschemacollector#module%E5%AE%9A%E4%B9%89 "module定义的直接链接")

| 字段名 | 可选 | 默认值 | 描述 |
| --- | --- | --- | --- |
| name | 是 |  | 命名空间名，可以为空。可以是多级命名空间，如a.b |

module中可以再定义子module，支持无限嵌套层级。

### enum定义[​](https://www.datable.cn/docs/manual/defaultschemacollector#enum%E5%AE%9A%E4%B9%89 "enum定义的直接链接")

| 字段名 | 可选 | 默认值 | 描述 |
| --- | --- | --- | --- |
| name | 否 |  | 类型名，不能包含命名空间 |
| flags | 是 | false | 等价schema逻辑结构中isFlags字段 |
| unique | 是 | false | 当前enum内的所有枚举值必须唯一，等价schema逻辑结构中isUniqueItemId字段 |
| comment | 是 |  |  |
| tags | 是 |  | 填写方式为 key1=value1#key2=value2... |

enum支持两种子元素：var和mapper。

var定义了枚举项的信息，格式如下：

| 字段 | 可空 | 默认值 | 说明 |
| --- | --- | --- | --- |
| name | 否 |  | 枚举项名 |
| alias | 是 |  | 别名 |
| value | 是 |  | 枚举值，可以为10进制或者16进制整数，如123、0x2A。也可以为其他枚举项的或值，如 A|B|C |
| comment | 是 |  |  |
| tags | 是 |  | 同其他tags写法 |

mapper定义了外部类型映射相关信息，具体含义请参照schema逻辑结构中文档，字段如下:

| 字段 | 可空 | 默认值 | 说明 |
| --- | --- | --- | --- |
| target | 否 |  | 为1-n个，以逗号','分割 |
| codeTarget | 否 | 1-n个，以逗号','分割 |  |

mapper可包含0-n个option子元素，option格式如下：

| 字段 | 可空 | 默认值 | 说明 |
| --- | --- | --- | --- |
| name | 否 |  |  |
| value | 否 |  |  |

### bean定义[​](https://www.datable.cn/docs/manual/defaultschemacollector#bean%E5%AE%9A%E4%B9%89 "bean定义的直接链接")

| 字段 | 可空 | 默认值 | 说明 |
| --- | --- | --- | --- |
| name | 否 |  | 类型名，不能包含命名空间 |
| parent | 是 |  | 父类名，如果名字不包含命名空间，会优先从当前命名空间找，再从全局命名空间找 |
| valueType | 是 | false | 对应schema逻辑结构中isValueType字段 |
| sep | 是 |  |  |
| alias | 是 | false |  |
| comment | 是 |  |  |
| group | 是 |  |  |
| tags | 是 |  |  |

bean支持三种子元素：var、bean和mapper。

var定义了成员字段，定义如下：

| 字段 | 可空 | 默认值 | 说明 |
| --- | --- | --- | --- |
| name | 否 |  |  |
| type | 否 |  |  |
| group | 是 |  |  |
| comment | 是 |  |  |
| tags | 是 |  |  |

bean定义了子结构，子结构的parent为上级bean，自动包含父类的所有字段，此时该子结构的parent字段会被自动忽略，子bean的定义方式与bean完全相同。 这种包含类型继承的结构为**多态结构**，尤其适合在技能、副本、AI中表达复杂GamePlay数据结构。

mapper定义了外部类型映射相关信息，与enum中mapper的定义方式完全相同，不再赘述。

### table定义[​](https://www.datable.cn/docs/manual/defaultschemacollector#table%E5%AE%9A%E4%B9%89 "table定义的直接链接")

| 字段 | 可空 | 默认值 | 说明 |
| --- | --- | --- | --- |
| name | 否 |  | 类型名，不能包含命名空间 |
| value | 否 |  | 表记录类型 |
| readSchemaFromFile | 是 | false | 是否从input的excel文件标题头行中读取value定义。此时不能再定义value对应的bean，否则会出现定义重复的错误 |
| input | 否 |  | 可以为多个，以逗号','分割，对应schema逻辑结构中inputFiles字段 |
| index | 是 |  | 可以为多个，如果是联合主键，则以'+'分割，如果是独立主键，则以','分割 |
| mode | 是 |  | 取one(或singleton)、map、list之一 |
| comment | 是 |  |  |
| group | 是 |  |  |
| tags | 是 |  |  |
| output | 否 |  | 对应schema逻辑结构中outputFileName字段 |

### constalias[​](https://www.datable.cn/docs/manual/defaultschemacollector#constalias "constalias的直接链接")

策划填写数据的时候，有时候希望用一个字符串来代表某个整数以方便阅读，同时也不容易出错。

常量别名没有命名空间的概念，不受module名影响。

提示

从v4.3.0版本起支持constalias。

| 字段 | 类型 | 可空 | 描述 |
| --- | --- | --- | --- |
| name | string | 否 | 别名 |
| value | string | 否 | 值 |

### refgroup[​](https://www.datable.cn/docs/manual/defaultschemacollector#refgroup "refgroup的直接链接")

refgroup为DefaultSchemaCollector的语法糖，用于表示一组被ref引用的table，在schema逻辑结构中没有有对应的对象。格式如下：

| 字段 | 可空 | 默认值 | 说明 |
| --- | --- | --- | --- |
| name | 否 |  | 可以为1-n个值，以逗号','分割，每个值必须为table全名（如item.TbItem,item.TbItem2） |

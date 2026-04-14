> Source: https://www.datable.cn/docs/manual/otherdatasource

Title: 非excel数据源 | Luban

URL Source: https://www.datable.cn/docs/manual/otherdatasource

Markdown Content:
并不是所有配置数据都以excel格式保存。实际项目中有一些比较复杂的配置通过编辑器生成，它们一般保存为json或者xml之类的格式。luban目前 支持以下数据源：

*   json
*   lua
*   xml
*   yaml
*   **lite**。 luban独有的一种简洁的无字段名的文本数据格式，适合表达非常复杂的数据结构

## 演示所用的类型[​](https://www.datable.cn/docs/manual/otherdatasource#%E6%BC%94%E7%A4%BA%E6%89%80%E7%94%A8%E7%9A%84%E7%B1%BB%E5%9E%8B "演示所用的类型的直接链接")

以下面的DemoType2为例，展示如何在其他文件格式中填写该类型对应的数据。

`<bean name="DemoType2" >  <var name="x4" type="int"/>  <var name="x1" type="bool"/>  <var name="x5" type="long"/>  <var name="x6" type="float"/>  <var name="x7" type="double"/>  <var name="x10" type="string"/>  <var name="x12" type="DemoType1"/>  <var name="x13" type="DemoEnum"/>  <var name="x14" type="DemoDynamic#sep=,"/>  <var name="t1" type="datetime"/>  <var name="k1" type="array,int"/>  <var name="k2" type="list,int"/>  <var name="k8" type="map,int,int"/>  <var name="k9" type="(list#sep=,#index=y1),DemoE2"/>  <var name="k15" type="(array#sep=,),DemoDynamic"/> </bean><table name="TbDataFromSingle" value="DemoType2" input="test/datas"/>`

## 数据目录[​](https://www.datable.cn/docs/manual/otherdatasource#%E6%95%B0%E6%8D%AE%E7%9B%AE%E5%BD%95 "数据目录的直接链接")

如果table.inputFiles指向目录，则会自动遍历整个目录树，忽略以`.~_`字符开头的文件，将剩下的文件当作数据文件输入。 这些数据文件格式则对应下面的单记录格式。注意，如果遇到excel文件，仍然默认试图从一个文件中读取多个记录。

## 单记录格式[​](https://www.datable.cn/docs/manual/otherdatasource#%E5%8D%95%E8%AE%B0%E5%BD%95%E6%A0%BC%E5%BC%8F "单记录格式的直接链接")

### json格式[​](https://www.datable.cn/docs/manual/otherdatasource#json%E6%A0%BC%E5%BC%8F "json格式的直接链接")

json格式的一些特殊点：

*   set类型。填法为 `[v1,v2,...]`
*   map类型。由于json只支持string类型的key，因此map格式填法为 `[[k1,v1],[k2,v2]...]`
*   多态bean类型。需要`$type`属性来指定具体类型名

`{  "x1":true,  "x2":3,  "x3":128,  "x4":1,  "x5":11223344,  "x6":1.2,  "x7":1.23432,  "x10":"hq",  "x12": { "x1":10},  "x13":"B",  "x14":{"$type": "DemoD2", "x1":1, "x2":2},  "t1":"1970-01-01 00:00:00",  "k1":[1,2],  "k2":[2,3],  "k7":[2,3],  "k8":[[2,2],[4,10]],  "k9":[{"y1":1, "y2":true},{"y1":2, "y2":false}],  "k15":[{"$type": "DemoD2", "x1":1, "x2":2}]}`

### lua格式[​](https://www.datable.cn/docs/manual/otherdatasource#lua%E6%A0%BC%E5%BC%8F "lua格式的直接链接")

Lua数据源特点：

*   数据前有一个return，这是因为 lua 数据是当作 lua 文件加载的，每个加载后的结果当作一个记录读入
*   set 的格式为 `{v1, v2, ...}`
*   与json不同，lua 的table的key支持任意格式，所以lua的map可以直接 `{[key1] = value1, [key2] = value2, ,,,}`
*   多态bean类型。需要 `_type_`属性来指定具体类型名

`return {  x1 = false,  x2 = 2,  x3 = 128,  x4 = 1122,  x5 = 112233445566,  x6 = 1.3,  x7 = 1122,  x10 = "yf",  x12 = {x1=1},  x13 = "D",  x14 = { _type_="DemoD2", x1 = 1, x2=3},  t1 = "1970-01-01 00:00:00",  k1 = {1,2},  k2 = {2,3},  k8 = {[2]=10,[3]=12},  k9 = { {y1=1,y2=true}, {y1=10,y2=false} },  k15 = { { _type_="DemoD2", x1 = 1, x2=3} },}`

### xml格式[​](https://www.datable.cn/docs/manual/otherdatasource#xml%E6%A0%BC%E5%BC%8F "xml格式的直接链接")

`<data>  <x1>true</x1>  <x2>4</x2>  <x3>128</x3>  <x4>1</x4>  <x5>112233445566</x5>  <x6>1.3</x6>  <x7>1112232.43123</x7>  <x10>yf</x10>  <x12> <x1>1</x1> </x12>  <x13>C</x13>  <x14 type="DemoD2">  <x1>1</x1>  <x2>2</x2> </x14>  <k1> <item>1</item> <item>2</item> </k1>  <k2> <item>1</item> <item>2</item> </k2>  <k8>      <item> <key>2</key><value>10</value></item>      <item> <key>3</key><value>30</value></item>  </k8>  <k9>      <item> <y1>1</y1> <y2>true</y2> </item>      <item> <y1>2</y1> <y2>false</y2> </item>  </k9>  <k15>      <item type="DemoD2"> <x1>1</x1> <x2>2</x2> </item>  </k15></data>`

### yaml格式[​](https://www.datable.cn/docs/manual/otherdatasource#yaml%E6%A0%BC%E5%BC%8F "yaml格式的直接链接")

yaml格式的特性性与json相似：

*   set类型。填法对应json列表 `[v1,v2,...]`
*   map类型。由于json只支持string类型的key，因此map格式填法对于json中kv列表 `[[k1,v1],[k2,v2]...]`
*   多态bean类型。需要 `$type`属性来指定具体类型名

提示

`yaml`和`yml`后缀的文件都会被识别为yaml文件。

`x1: truex2: 3x3: 128x4: 40x5: 11223344x6: 1.2x7: 1.23432x10: hqx12:  x1: 10x13: Bx14:  $type: DemoD2  x1: 1  x2: 2t1: '1970-01-01 00:00:00'k1:- 1- 2k2:- 2- 3k8:- - 2  - 2- - 4  - 10k9:- y1: 1  y2: true- y1: 2  y2: falsek15:- $type: DemoD2  x1: 1  x2: 2`

## lite格式[​](https://www.datable.cn/docs/manual/otherdatasource#lite%E6%A0%BC%E5%BC%8F "lite格式的直接链接")

Lite格式是luban独有的一种文本型数据格式。相比于json之类的格式，它不需要字段名，因此更加简洁。

提示

Lite格式文件的后缀为`lit`，如`abc.lit`。

| 数据类型 | 配置格式 | 补充说明 |
| --- | --- | --- |
| bool | true、false、1、0 | 大小写无关 |
| byte | 1 |  |
| short | 1 |  |
| int | 1 |  |
| long | 1 |  |
| float | 1、1.0 |  |
| double | 1、1.0 |  |
| string | abc、'abc'、"abc" |  |
| datetime | 1970-01-01 00:00:00 |  |
| array | {1,2,3} |  |
| list | {1,2,3} |  |
| set | {1,2,3} |  |
| map | {{1,2},{3,4},{5,6}} |  |
| enum | A |  |
| bean | {1,2,3} | 以'{'开始，按顺序填写每个字段，以'}'结束。如果是可空的多态或者普通bean，可以直接用`null`表示空，而不能写成`{null}`。非空多态bean的格式为`{多态类型,字段1,字段2,...}`，即第一个字段为多态类型 |

补充说明：

*   配置中出现的换行和字段前后的空白值会被自动剔除，类似html解析规则
*   不支持默认字段值，空白数据会被跳过。例如`{1,,,2}`等价于`{1,2}`
*   string 可以不必像json格式那样以`'`或`"`将字符串内容围起来。由于解析数据时会自动剔除掉字符串前后的空白字符，如果想要在字符串中包含空白字段，需要添加`'`或`"`，例如 `'   a    '`。 如果字符串中遇到`'`或`"`或`,`或`{`或`}`或`\n`之类字符，需要使用`\`字符转义，例如`'abc\{\}\'\"'\t\n\\`。
*   容器和bean（值为null的bean除外）需要以 `{`开始，以`}`结束。

示例配置的填法如下：

单行填法：

`{ 1122, false, 2, 128, 112233445566, 1.3, 1122, yf, {1}, D, {DemoD2,1,3}, 1970-01-01 00:00:00, {1,2}, {2,3}, {{2,10},{3,12}}, {{1,true},{2,false}}, {{DemoD2,1,3}}}`

阅读性更佳的多行填法：

`{  1122,  false,  2,  128,  112233445566,  1.3,  1122,  yf,  {1},  D,  {DemoD2,1,3},  1970-01-01 00:00:00,  {1,2},  {2,3},  {{2,10},{3,12}},  {{1,true},{2,false}},  {{DemoD2,1,3}}}`

## 复合文件格式[​](https://www.datable.cn/docs/manual/otherdatasource#%E5%A4%8D%E5%90%88%E6%96%87%E4%BB%B6%E6%A0%BC%E5%BC%8F "复合文件格式的直接链接")

如果在一个文件中包含多个记录，则需要在文件名前加上`*@`，表示该文件包含一个记录列表，而不是一个记录。 这里以json格式为例。table的inputFiles，有以下几种格式：

*   `xxx.json`，把xxx.json当作一个记录读入。
*   `*@xxx.json`，把xxx.json当作记录列表读入。
*   `field@xxx.json`，把 xxx.json中的field字段当作一个记录读入。field可以是深层次字段，比如 a.b.c。
*   `*field@xxx.json`，把xxx.json中的field字段当作记录列表读入。field可以是深层次字段。

比较有趣的是，与xlsx数据源相似，支持将多个表放到同一个json中，不过实践中极少这么做。

如下列示例：

*   TbCompositeJsonTable1 从 composite_tables.json的table1字段中读入记录列表，从composite_tables2.json中读入记录列表，从one_record.json中读入一个记录
*   TbCompositeJsonTable2 从 composite_tables.json的table2字段中读入记录列表
*   TbCompositeJsonTable3 从 composite_tables.json的table3字段中读入一个记录

`<bean name="CompositeJsonTable1">    <var name="id" type="int"/>    <var name="x" type="string"/></bean><bean name="CompositeJsonTable2">    <var name="id" type="int"/>    <var name="y" type="int"/></bean><bean name="CompositeJsonTable3">    <var name="a" type="int"/>    <var name="b" type="int"/></bean><table name="TbCompositeJsonTable1" value="CompositeJsonTable1" input="*table1@composite_tables.json,*@composite_tables2.json,one_record.json"/><table name="TbCompositeJsonTable2" value="CompositeJsonTable2" input="*table2@composite_tables.json"/><table name="TbCompositeJsonTable3" value="CompositeJsonTable3" mode="one" input="table3@composite_tables.json"/>`

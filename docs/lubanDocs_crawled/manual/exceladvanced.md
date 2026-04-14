> Source: https://www.datable.cn/docs/manual/exceladvanced

Title: excel格式（高级） | Luban

URL Source: https://www.datable.cn/docs/manual/exceladvanced

Markdown Content:
## 示例中用到的结构[​](https://www.datable.cn/docs/manual/exceladvanced#%E7%A4%BA%E4%BE%8B%E4%B8%AD%E7%94%A8%E5%88%B0%E7%9A%84%E7%BB%93%E6%9E%84 "示例中用到的结构的直接链接")

以下是示例中要用于的bean类型定义。

`<bean name="Type1">  <var name="a" type="int"/>  <var name="b" type="string"/>  <var name="c" type="bool"/></bean><bean name="Type2">  <var name="a" type="int"/>  <var name="b" type="bool"/>  <var name="c" type="Type1"/></bean><bean name="Vec3" sep=",">  <var name="x" type="float"/>  <var name="y" type="float"/>  <var name="z" type="float"/></bean><bean name="Type3">  <var name="a" type="int"/>  <var name="b" type="bool"/>  <var name="c" type="Type1#sep=,"/></bean><bean name="Type4">  <var name="a" type="string"/>  <var name="c" type="Vec3"/></bean><bean name="Title0">  <var name="a" type="int"/>  <var name="b" type="bool"/>  <var name="c" type="Title1"/></bean><bean name="Title1">  <var name="a" type="int"/>  <var name="b" type="string"/>  <var name="c" type="Title2"/></bean><bean name="Title2">  <var name="a" type="int"/>  <var name="b" type="int"/></bean>`

## 常量别名[​](https://www.datable.cn/docs/manual/exceladvanced#%E5%B8%B8%E9%87%8F%E5%88%AB%E5%90%8D "常量别名的直接链接")

策划填写数据的时候，有时候希望用一个字符串来代表某个整数以方便阅读，同时也不容易出错。

在xml schema文件中定义`constalias`常量别名，在填写数据时使用它。

注意！常量别名仅能用于`byte、short、int、long、float、double`类型的数据，并且仅在excel族(xls、xlsx、csv等)、lite类型源数据类型中生效。

常量别名没有命名空间的概念，**不受module名影响**。

`<module name="test">  <constalias name="ITEM0" value="1001"/>  <constalias name="ITEM1" value="1002"/>  <constalias name="FLOAT1" value="1.5"/>  <constalias name="FLOAT2" value="2.5"/></module>`

![Image 1: constalias](blob:http://localhost/fb773706b57e322c2501e609099e8e76)

## 限定列格式[​](https://www.datable.cn/docs/manual/exceladvanced#%E9%99%90%E5%AE%9A%E5%88%97%E6%A0%BC%E5%BC%8F "限定列格式的直接链接")

通过标题行及多级标题行，可以精确限定某个数据在某些列范围内。

对于只有一个原子值的简单类型数据，限定列格式下，由于能够非常清晰知道它的值必然来自某一单元格，所以它支持**默认值**语义，即如果单元格为空，值取默认值，例如 int类型默认值为0，int?默认值为null。

限定列格式下，多态bean类型需要用 $type 列来指定具体类型名，可空bean类型也需要用$type列来指示是有效bean还是空bean。

如果最低层的限定列的类型为容器或者bean，由于限定列只限定了该数据整体范围，但**未限定**子数据的范围，因此读取子数据的格式为**流式格式**，即按顺序读入每个子数据。

![Image 2: titlelimit](https://www.datable.cn/assets/images/titlelimit-602bb9196f754dd4a3c55d766a6d301c.jpg)

### `flags=1` 的 enum 类型支持列限定模式。[​](https://www.datable.cn/docs/manual/exceladvanced#flags1-%E7%9A%84-enum-%E7%B1%BB%E5%9E%8B%E6%94%AF%E6%8C%81%E5%88%97%E9%99%90%E5%AE%9A%E6%A8%A1%E5%BC%8F "flags1-的-enum-类型支持列限定模式的直接链接")

用枚举项作为列名，最终值为所有非0或空的枚举项的**或值**。

![Image 3: titlle_enum](https://www.datable.cn/assets/images/title_enum-5c96663bfbbb1992cd6e2713cec78d1b.jpg)

### 多态bean支持 $type与$value 分别配置的列限定或流式格式的混合填写方式[​](https://www.datable.cn/docs/manual/exceladvanced#%E5%A4%9A%E6%80%81bean%E6%94%AF%E6%8C%81-type%E4%B8%8Evalue-%E5%88%86%E5%88%AB%E9%85%8D%E7%BD%AE%E7%9A%84%E5%88%97%E9%99%90%E5%AE%9A%E6%88%96%E6%B5%81%E5%BC%8F%E6%A0%BC%E5%BC%8F%E7%9A%84%E6%B7%B7%E5%90%88%E5%A1%AB%E5%86%99%E6%96%B9%E5%BC%8F "多态bean支持 $type与$value 分别配置的列限定或流式格式的混合填写方式的直接链接")

即用$type列为限定类型，用$value列来限定bean的实际字段，并且$value中以流式填写bean的所有字段。

![Image 4: title_dynamic_bean](https://www.datable.cn/assets/images/title_dynamic_bean-482422aabcccdac7d7fbd6d369cdbe4c.jpg)

### map的列限定格式[​](https://www.datable.cn/docs/manual/exceladvanced#map%E7%9A%84%E5%88%97%E9%99%90%E5%AE%9A%E6%A0%BC%E5%BC%8F "map的列限定格式的直接链接")

有两种填法：

*   多行填法。此时要求 `$key`子列对应key字段，剩余列对应value的子字段。如下图y1字段所示
*   非多行填法。可以将key作为子字段名，如果对应的单元不为空，则对应key-value的键值对存在。例如下图中id=1的记录， 它的y2字段最终值为`{{"aaa", 1}, {"ccc":2}}`；id=2的记录，它的y2字段最终值为`{{"bbb", 10}, {"ccc", 20}, {"ddd", 30}}`。 如下图y2字段所示

![Image 5: title_map](https://www.datable.cn/assets/images/title_map-13d12e479c22398dc7b73af7e44c3232.jpg)

提示

以上仅是map的列限定格式下的填法。map还有额外两种流式格式下的填法。

## 多级标题头[​](https://www.datable.cn/docs/manual/exceladvanced#%E5%A4%9A%E7%BA%A7%E6%A0%87%E9%A2%98%E5%A4%B4 "多级标题头的直接链接")

有时候，某些字段是复合结构，如bean或者结构列表之类的类型，按顺序填写时，由于流式格式中空白单元格会被自动跳过， 导致实践中容易写错。另外，流式格式不支持空白单元格表示默认值，也无法直观地限定某个子字段在某一铺，带来一些不便。 多级标题可以用于限定bean或容器的子字段，提高了可读性，避免流式格式的意外错误。

通过在某个`##var`行下新增一行`##var`，为添加子字段名，则可以为子字段设置标题头。可以有任意级别的子标题头。 下图中，x1只有1级子标题头，y1有2级，y2只有1级，z1有3级。

![Image 6: colloumlimit](https://www.datable.cn/assets/images/multileveltitle-3e1e45452ed00a0da5f65d40f557e62c.jpg)

## 多行结构列表[​](https://www.datable.cn/docs/manual/exceladvanced#%E5%A4%9A%E8%A1%8C%E7%BB%93%E6%9E%84%E5%88%97%E8%A1%A8 "多行结构列表的直接链接")

有时候列表结构的每个结构字段较多，如果水平展开则占据太多列，不方便编辑，如果拆表，无论程序还是策划都不方便，此时可以使用多行模式。

将字段名标记为`*<name>`即可表达要将这个数据多行读入。支持任意层次的多行结构列表（也即多行结构中的每个元素也可以是多行）。 对于`array,bean`、`list,bean`这样的结构容器类型，还可以配合限定列格式，限定元素中每个子字段的列，如字段x2所示。

![Image 7: map](https://www.datable.cn/assets/images/multiline-d4bd4a85c32fa4b9978c22cd9d0adaa9.jpg)

## 紧凑格式[​](https://www.datable.cn/docs/manual/exceladvanced#%E7%B4%A7%E5%87%91%E6%A0%BC%E5%BC%8F "紧凑格式的直接链接")

如果某个数据是非原子数据（如bean或容器），并且它被限定到某些单元格列范围或者是sep分割的数据的一部分，则它的解析方式为**紧凑格式**。

![Image 8: image](https://www.datable.cn/assets/images/compact-6388075acdb892d9a2987e3fe87e6485.jpg)

由于紧凑格式比较复杂，单独用一篇文档介绍它。详细见[Excel紧凑格式](https://www.datable.cn/docs/manual/excelcompactformat)。

## 数据标签过滤[​](https://www.datable.cn/docs/manual/exceladvanced#%E6%95%B0%E6%8D%AE%E6%A0%87%E7%AD%BE%E8%BF%87%E6%BB%A4 "数据标签过滤的直接链接")

开发期经常会制作一些仅供开发使用的配置，比如测试道具，比如自动化测试使用的配置，开发者希望在正式发布时不导出这些数据。 可以通过给记录加上tag，再配合命令行参数 --excludeTag实现这个目的 。`##`是一个特殊的tag，表示这个数据被永久注释，任何情况下都不会被导出。 详细文档请阅读 [数据 tag](https://www.datable.cn/docs/manual/tag)。

如下图，id=3和id=4的记录，在命令行添加 `--excludeTag dev` 参数后，导出时不会包含这两个dev记录。

![Image 9: tag](https://www.datable.cn/assets/images/tag-e58c3cc27b698633de18a8f060eb96a3.jpg)

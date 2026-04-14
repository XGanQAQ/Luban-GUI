> Source: https://www.datable.cn/docs/manual/excel

Title: excel格式（初级） | Luban

URL Source: https://www.datable.cn/docs/manual/excel

Markdown Content:
## 基础规则[​](https://www.datable.cn/docs/manual/excel#%E5%9F%BA%E7%A1%80%E8%A7%84%E5%88%99 "基础规则的直接链接")

### 支持的excel文件族[​](https://www.datable.cn/docs/manual/excel#%E6%94%AF%E6%8C%81%E7%9A%84excel%E6%96%87%E4%BB%B6%E6%97%8F "支持的excel文件族的直接链接")

支持 xls、 xlsx、 xlm、 xlmx、csv 。基本上excel能打开的都可以读取。

### excel文件 读取规则[​](https://www.datable.cn/docs/manual/excel#excel%E6%96%87%E4%BB%B6-%E8%AF%BB%E5%8F%96%E8%A7%84%E5%88%99 "excel文件 读取规则的直接链接")

*   如果未指定sheet，则默认会读取所有sheet
*   可以用 [sheet@xxx.xlsx](mailto:sheet@xxx.xlsx) 指定只读入这个sheet数据
*   如果A1单元格数据不以##开头，则会被当作非数据sheet，被忽略

### 读取除GKB和UTF8以外编码的csv文件[​](https://www.datable.cn/docs/manual/excel#%E8%AF%BB%E5%8F%96%E9%99%A4gkb%E5%92%8Cutf8%E4%BB%A5%E5%A4%96%E7%BC%96%E7%A0%81%E7%9A%84csv%E6%96%87%E4%BB%B6 "读取除GKB和UTF8以外编码的csv文件的直接链接")

luban会智能猜测出它的编码，正确处理。

### 灵活的文件组织形式[​](https://www.datable.cn/docs/manual/excel#%E7%81%B5%E6%B4%BB%E7%9A%84%E6%96%87%E4%BB%B6%E7%BB%84%E7%BB%87%E5%BD%A2%E5%BC%8F "灵活的文件组织形式的直接链接")

*   可以几个表都放到一个xlsx中，每个表占一个sheet。 只需要为每个表的input指定为该单元薄即可，如`xxx@item/test/abs.xlsx`
*   可以一个表拆分为几个xlsx。 如 `item/a.xlsx,bag/b.xlsx,c.xlsx`
*   可以一个读入一个目录下的所有xlsx。 如 `xlsx_files` 。

## 标题头行格式[​](https://www.datable.cn/docs/manual/excel#%E6%A0%87%E9%A2%98%E5%A4%B4%E8%A1%8C%E6%A0%BC%E5%BC%8F "标题头行格式的直接链接")

一个典型的配置表示例：

![Image 1: excel](https://www.datable.cn/assets/images/simple1-c95bc3b88dc62a5ffff773f4c68b4a92.jpg)

*   第1列单元格为 `##var` 表示这行是字段定义行
*   第1列单元格为 `##type` 表示这行是 类型定义行
*   第1列单元格为 `##group` 表示这行是 导出分组行。**此行可选**。另外，单元格留空表示对所有分组导出。
*   第1列单元格以##**开头** 表示这是注释行，如果有多个##行，默认以第一个行作为代码中字段的注释，你可以通过##comment 显式指定某行为代码注释行。
*   填写多级字段名行时，以##var表示这是次级字段行
*   你可以随意调整以`##<name>`开头的行的顺序。例如`##var`行与`##group`行调换顺序，完全不影响最终结果

## 注释行或列[​](https://www.datable.cn/docs/manual/excel#%E6%B3%A8%E9%87%8A%E8%A1%8C%E6%88%96%E5%88%97 "注释行或列的直接链接")

当标题行字段名为空或者以'#'开头时，这个列会被当作注释列而忽略。当数据行的第一列以##开头时，这一行会被当作注释行而被忽略。

![Image 2: excel](https://www.datable.cn/assets/images/ignorefield-59cfc9f1e6c25bc26576eda614647145.jpg)

以上示例中，D列和E列被注释而忽略，第7行由于以##开头，也被注释而不会导出。

## 基础数据格式[​](https://www.datable.cn/docs/manual/excel#%E5%9F%BA%E7%A1%80%E6%95%B0%E6%8D%AE%E6%A0%BC%E5%BC%8F "基础数据格式的直接链接")

如下图所示，数据填法基本与常识相符。

![Image 3: primitive_type](https://www.datable.cn/assets/images/primitive_type-d85cfb51a19f153b0fdf9ac299b4a5e1.jpg)

特殊说明：

*   bool： `true、false、0、1、是、否` 都是有效值。另外大小写不敏感，如True也是合法bool值。填其他值如abc、4则会发生解析错误
*   string：单元格留空即为长度为0的字符串。但读取流式格式时，空白单元格也会被当作无用单元格而被忽略，此时可以用`""`表示长度为0的字符串。string默认不处理转义， 如果希望将字符串中`\n`替换为换行符，则需要添加上`escape=1`tag，如`string#escape=1`。
*   datetime支持以下几种格式
    *   excel中的内置日期格式
    *   yyyy-mm-dd hh:mm:ss 字符串格式
    *   yyyy-mm-dd hh:mm 字符串格式。此时秒自动取0
    *   yyyy-mm-dd hh 字符串格式。此时分与秒取0
    *   yyyy-mm-dd 字符串格式。此时时分秒都取0

**除了datetime以外的基础数据格式都可以留空**，自动取默认值，如第10行所示。

## enum 数据格式[​](https://www.datable.cn/docs/manual/excel#enum-%E6%95%B0%E6%8D%AE%E6%A0%BC%E5%BC%8F "enum 数据格式的直接链接")

可以填写枚举项的变量名、别名或者对应的整数。如果是flags类型枚举，还可以填'A|B'这样的或形式枚举。flags类型enum还支持列限定模式， 让每个枚举值占一列，然后将包含的标志位列标为1，表示最终的枚举值包含此项。

![Image 4: enum](https://www.datable.cn/assets/images/enum-dee044226803effc6032313e7c4981e7.jpg)

如果enum中有值为0的枚举项，则可以留空，自动取该枚举项，否则会解析出错。

对于flags类型的enum，允许填多个枚举值来表示组合，默认使用'|'来分割多个枚举值。可以通过设置enum的sep属性来指定其他分割符。如`sep=","`时，使用`A,B`表示两个枚举值的或组合。

## bean数据格式[​](https://www.datable.cn/docs/manual/excel#bean%E6%95%B0%E6%8D%AE%E6%A0%BC%E5%BC%8F "bean数据格式的直接链接")

假设 Item是包含`int id; int count; string desc` 这三个字段的bean，item字段类型为Item。合并C1-E1为一个单元格，作为item字段范围。在item字段的列范围内， 按顺序填写Item结构的每个字段。如下图所示。

![Image 5: bean](https://www.datable.cn/assets/images/bean-85ba1ecb5030e30e47c4487ec0c261d2.jpg)

如果该bean字段为多态类型，则须先填写多态类型名，再顺序填写多态类型的字段。多态类型名可以填bean的别名。如下图。

![Image 6: bean](https://www.datable.cn/assets/images/bean2-04651442a9b2d1cb2c12f18f23cb9bcf.jpg)

## 容器类型[​](https://www.datable.cn/docs/manual/excel#%E5%AE%B9%E5%99%A8%E7%B1%BB%E5%9E%8B "容器类型的直接链接")

与bean类型，通过合并单元格作为字段的列范围，在范围内填写数据即可。**空白**单元格会被忽略。

![Image 7: collection](https://www.datable.cn/assets/images/collection-5416a057bd788208fb64a6b5420663ef.jpg)

map以key、value为键值对，依次填写。

## 可空类型[​](https://www.datable.cn/docs/manual/excel#%E5%8F%AF%E7%A9%BA%E7%B1%BB%E5%9E%8B "可空类型的直接链接")

除了容器以外的类型都可以是可空类型。所有可空类型都可以用null表达空值。

*   对于只包含一个数据的原子数据类型（如int），单元格留空也表达null
*   对于string?类型，单元格留空表达null而不是长度为0的字符串。如果你想表达空白字符串请用`""`
*   对于非多态的bean类型的可空类型，如果非空，需要以`{}`开头表示非空，接着顺序填写bean的值
*   对于多态bean，填法不变

![Image 8: nullable](https://www.datable.cn/assets/images/nullable-8a3a3a221c9def07e16e04ccf86a9b84.jpg)

## 无主键表[​](https://www.datable.cn/docs/manual/excel#%E6%97%A0%E4%B8%BB%E9%94%AE%E8%A1%A8 "无主键表的直接链接")

有时候只想得到一个记录列表，无主键。mode="list"并且index为空，表示无主键表。

定义表

`<table name="TbNotKeyList" value="NotKeyList" mode="list" input="not_key_list.xlsx"/>`

![Image 9: table_list_not_key](https://www.datable.cn/assets/images/table_list_not_key-082f29e3fc26a5cc33d982f34a4c1e60.jpg)

## 多主键表（联合索引）[​](https://www.datable.cn/docs/manual/excel#%E5%A4%9A%E4%B8%BB%E9%94%AE%E8%A1%A8%E8%81%94%E5%90%88%E7%B4%A2%E5%BC%95 "多主键表（联合索引）的直接链接")

多个key构成联合唯一主键。使用"+"分割key，表示联合关系。

定义表

`<table name="TbUnionMultiKey" value="UnionMultiKey" index="key1+key2+key3" input="union_multi_key.xlsx"/>`

![Image 10: table_list_union_key](https://www.datable.cn/assets/images/table_list_union_key-27d9231b4a48f42aa5f79cf80e2ffd81.jpg)

## 多主键表（独立索引）[​](https://www.datable.cn/docs/manual/excel#%E5%A4%9A%E4%B8%BB%E9%94%AE%E8%A1%A8%E7%8B%AC%E7%AB%8B%E7%B4%A2%E5%BC%95 "多主键表（独立索引）的直接链接")

多个key，各自独立唯一索引。与联合索引写法区别在于使用 ","来划分key，表示独立关系。

定义表

`<table name="TbMultiKey" value="MultiKey" index="key1,key2,key3" input="multi_key.xlsx"/>`

![Image 11: table_list_indep_key](https://www.datable.cn/assets/images/table_list_indep_key-3d2f4e268f41d88d0312c350bdf075e4.jpg)

## 单例表[​](https://www.datable.cn/docs/manual/excel#%E5%8D%95%E4%BE%8B%E8%A1%A8 "单例表的直接链接")

有一些配置全局只有一份，比如 公会模块的开启等级，背包初始大小，背包上限。此时使用单例表来配置这些数据比较合适。

![Image 12: singleton](https://www.datable.cn/assets/images/singleton2-b46d4b2c6cccbabd69296a59222fe9d4.jpg)

## 纵表[​](https://www.datable.cn/docs/manual/excel#%E7%BA%B5%E8%A1%A8 "纵表的直接链接")

大多数表都是横表，即一行一个记录。有些表，比如单例表，如果纵着填，一行一个字段，会比较舒服。A1为`##column`或`##vertical`表示使用纵表模式。 上面的单例表，以纵表模式填如下。

![Image 13: singleton](https://www.datable.cn/assets/images/singleton-9b7d41bf32c0c214d2baac6cbbd5cea8.jpg)

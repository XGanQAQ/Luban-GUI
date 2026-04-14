> Source: https://www.datable.cn/docs/help/faq

Title: FAQ | Luban

URL Source: https://www.datable.cn/docs/help/faq

Markdown Content:
## 如何指定主键[​](https://www.datable.cn/docs/help/faq#%E5%A6%82%E4%BD%95%E6%8C%87%E5%AE%9A%E4%B8%BB%E9%94%AE "如何指定主键的直接链接")

table的index字段指定主键列表。 详细请参见 [配置相关定义](https://www.datable.cn/docs/manual/schema) 中关于table的mode和index的相关文档。

map及list表支持主键概念，未指定mode和index的情况下，自动为mode=map模式，并记录bean的第一个字段作为主键。

假设 TbTest表的记录为Test类型，你想用Test的my_index字段作为key，则：

*   如果在xml里定义表，则在table的index属性中指定主键字段名，如下：

`<table name="TbTest" value="Test" index="my_index"/>`

*   如果在 table.xlsx里定义表，则在index列指定主键名，如下

| ##var | full_name | value_type | define_from_excel | input | index | ... |
| --- | --- | --- | --- | --- | --- | --- |
|  | TbTest | Test | true | equip.xlsx | my_index |  |

## 支持多主键吗？[​](https://www.datable.cn/docs/help/faq#%E6%94%AF%E6%8C%81%E5%A4%9A%E4%B8%BB%E9%94%AE%E5%90%97 "支持多主键吗？的直接链接")

支持。 table mode=list时，支持联合多主键模式和独立多主键模式。详细文档参见 [配置相关定义](https://www.datable.cn/docs/manual/schema) 中关于table的mode的相关文档。

## 支持按client和server导出不同表及不同字段吗？[​](https://www.datable.cn/docs/help/faq#%E6%94%AF%E6%8C%81%E6%8C%89client%E5%92%8Cserver%E5%AF%BC%E5%87%BA%E4%B8%8D%E5%90%8C%E8%A1%A8%E5%8F%8A%E4%B8%8D%E5%90%8C%E5%AD%97%E6%AE%B5%E5%90%97 "支持按client和server导出不同表及不同字段吗？的直接链接")

支持。[配置相关定义](https://www.datable.cn/docs/manual/schema) 中关于 分级定义及分组导出相关文档。

## 支持哪些源数据文件类型[​](https://www.datable.cn/docs/help/faq#%E6%94%AF%E6%8C%81%E5%93%AA%E4%BA%9B%E6%BA%90%E6%95%B0%E6%8D%AE%E6%96%87%E4%BB%B6%E7%B1%BB%E5%9E%8B "支持哪些源数据文件类型的直接链接")

*   excel族。 csv、xls、xlm、xlsx、xlsm 等等
*   json
*   xml
*   lua
*   yaml

## 配置表的数据可以来源于多个文件吗？[​](https://www.datable.cn/docs/help/faq#%E9%85%8D%E7%BD%AE%E8%A1%A8%E7%9A%84%E6%95%B0%E6%8D%AE%E5%8F%AF%E4%BB%A5%E6%9D%A5%E6%BA%90%E4%BA%8E%E5%A4%9A%E4%B8%AA%E6%96%87%E4%BB%B6%E5%90%97 "配置表的数据可以来源于多个文件吗？的直接链接")

可以。 参见 [配置相关定义](https://www.datable.cn/docs/manual/schema) 中关于 table.input的文档。

## 可以将多个表放到同一个excel文件吗？[​](https://www.datable.cn/docs/help/faq#%E5%8F%AF%E4%BB%A5%E5%B0%86%E5%A4%9A%E4%B8%AA%E8%A1%A8%E6%94%BE%E5%88%B0%E5%90%8C%E4%B8%80%E4%B8%AAexcel%E6%96%87%E4%BB%B6%E5%90%97 "可以将多个表放到同一个excel文件吗？的直接链接")

可以。 参见 [配置相关定义](https://www.datable.cn/docs/manual/schema) 中关于 table.input的文档。

## 当数据文件为xlsx文件时，luban会读入第一个sheet还是所有sheet？[​](https://www.datable.cn/docs/help/faq#%E5%BD%93%E6%95%B0%E6%8D%AE%E6%96%87%E4%BB%B6%E4%B8%BAxlsx%E6%96%87%E4%BB%B6%E6%97%B6luban%E4%BC%9A%E8%AF%BB%E5%85%A5%E7%AC%AC%E4%B8%80%E4%B8%AAsheet%E8%BF%98%E6%98%AF%E6%89%80%E6%9C%89sheet "当数据文件为xlsx文件时，luban会读入第一个sheet还是所有sheet？的直接链接")

读入所有sheet，但是会忽略那些A1单元格内容不是##开头的sheet。

## 策划想在xlsx中有一个非数据的sheet，该怎么做呢[​](https://www.datable.cn/docs/help/faq#%E7%AD%96%E5%88%92%E6%83%B3%E5%9C%A8xlsx%E4%B8%AD%E6%9C%89%E4%B8%80%E4%B8%AA%E9%9D%9E%E6%95%B0%E6%8D%AE%E7%9A%84sheet%E8%AF%A5%E6%80%8E%E4%B9%88%E5%81%9A%E5%91%A2 "策划想在xlsx中有一个非数据的sheet，该怎么做呢的直接链接")

只要该sheet的A1单元格不以##开头即可。

## 想注释掉某一列，该如何做[​](https://www.datable.cn/docs/help/faq#%E6%83%B3%E6%B3%A8%E9%87%8A%E6%8E%89%E6%9F%90%E4%B8%80%E5%88%97%E8%AF%A5%E5%A6%82%E4%BD%95%E5%81%9A "想注释掉某一列，该如何做的直接链接")

将列名取空，或者 #xxxx这样的名称。

## 想注释掉某一行记录，该如何做[​](https://www.datable.cn/docs/help/faq#%E6%83%B3%E6%B3%A8%E9%87%8A%E6%8E%89%E6%9F%90%E4%B8%80%E8%A1%8C%E8%AE%B0%E5%BD%95%E8%AF%A5%E5%A6%82%E4%BD%95%E5%81%9A "想注释掉某一行记录，该如何做的直接链接")

将该行第一个单元格填以##即可。

## 有些配置只想开发期内部测试用，正式发布时不导出，该如何做？[​](https://www.datable.cn/docs/help/faq#%E6%9C%89%E4%BA%9B%E9%85%8D%E7%BD%AE%E5%8F%AA%E6%83%B3%E5%BC%80%E5%8F%91%E6%9C%9F%E5%86%85%E9%83%A8%E6%B5%8B%E8%AF%95%E7%94%A8%E6%AD%A3%E5%BC%8F%E5%8F%91%E5%B8%83%E6%97%B6%E4%B8%8D%E5%AF%BC%E5%87%BA%E8%AF%A5%E5%A6%82%E4%BD%95%E5%81%9A "有些配置只想开发期内部测试用，正式发布时不导出，该如何做？的直接链接")

luban支持数据tag的概念。 excel第一列为tag。

*   当tag为##时忽略这个行
*   当tag为xxx时，如果Luban.Client 命令行中使用 --export_exclude_tags xxx，则不会导出该记录

## 我想每个json保存一个记录，文件太多，在input中指定很麻烦，怎么解决？[​](https://www.datable.cn/docs/help/faq#%E6%88%91%E6%83%B3%E6%AF%8F%E4%B8%AAjson%E4%BF%9D%E5%AD%98%E4%B8%80%E4%B8%AA%E8%AE%B0%E5%BD%95%E6%96%87%E4%BB%B6%E5%A4%AA%E5%A4%9A%E5%9C%A8input%E4%B8%AD%E6%8C%87%E5%AE%9A%E5%BE%88%E9%BA%BB%E7%83%A6%E6%80%8E%E4%B9%88%E8%A7%A3%E5%86%B3 "我想每个json保存一个记录，文件太多，在input中指定很麻烦，怎么解决？的直接链接")

使用 目录数据源。 把所有json文件放到一个目录下（可以是目录树），将input设为该目录。luban会自动遍历整个目录树，将每个文件当作 一个记录读入。 详细参见[其它数据源-json](https://www.datable.cn/docs/manual/otherdatasource)

## 一个json文件可以包含多个记录吗？[​](https://www.datable.cn/docs/help/faq#%E4%B8%80%E4%B8%AAjson%E6%96%87%E4%BB%B6%E5%8F%AF%E4%BB%A5%E5%8C%85%E5%90%AB%E5%A4%9A%E4%B8%AA%E8%AE%B0%E5%BD%95%E5%90%97 "一个json文件可以包含多个记录吗？的直接链接")

可以。但必须在数据源中以 *@xxx.json形式指定。详细参见[其它数据源-json](https://www.datable.cn/docs/manual/otherdatasource)

## 记录可以来自json文件的某个深层次字段吗？[​](https://www.datable.cn/docs/help/faq#%E8%AE%B0%E5%BD%95%E5%8F%AF%E4%BB%A5%E6%9D%A5%E8%87%AAjson%E6%96%87%E4%BB%B6%E7%9A%84%E6%9F%90%E4%B8%AA%E6%B7%B1%E5%B1%82%E6%AC%A1%E5%AD%97%E6%AE%B5%E5%90%97 "记录可以来自json文件的某个深层次字段吗？的直接链接")

可以。分两种情况：

*   从字段中读入一个记录，则以 [a.b.c@xx.json](mailto:a.b.c@xx.json)的形式指定
*   从字段中读入记录列表，则以 *[a.b.c@xx.json](mailto:a.b.c@xx.json)的形式指定

详细参见[其它数据源-json](https://www.datable.cn/docs/manual/otherdatasource)

## 可以像xlsx那样，将多个表的数据都放到一个json文件中吗？[​](https://www.datable.cn/docs/help/faq#%E5%8F%AF%E4%BB%A5%E5%83%8Fxlsx%E9%82%A3%E6%A0%B7%E5%B0%86%E5%A4%9A%E4%B8%AA%E8%A1%A8%E7%9A%84%E6%95%B0%E6%8D%AE%E9%83%BD%E6%94%BE%E5%88%B0%E4%B8%80%E4%B8%AAjson%E6%96%87%E4%BB%B6%E4%B8%AD%E5%90%97 "可以像xlsx那样，将多个表的数据都放到一个json文件中吗？的直接链接")

可以。 与excel数据源类似，只要每个表用 [field@xx.json](mailto:field@xx.json)或者 *[field@xx.json](mailto:field@xx.json)的形式指定即可。 详细参见[其它数据源-json](https://www.datable.cn/docs/manual/otherdatasource)

## 支持异步加载配置表吗？[​](https://www.datable.cn/docs/help/faq#%E6%94%AF%E6%8C%81%E5%BC%82%E6%AD%A5%E5%8A%A0%E8%BD%BD%E9%85%8D%E7%BD%AE%E8%A1%A8%E5%90%97 "支持异步加载配置表吗？的直接链接")

不直接支持。但你可以通过自定义模板方式实现异步加载。

## 可以引用现有的枚举和结构吗？比如生成的代码中想使用UnityEngine.AudioType和UnityEngine.Color[​](https://www.datable.cn/docs/help/faq#%E5%8F%AF%E4%BB%A5%E5%BC%95%E7%94%A8%E7%8E%B0%E6%9C%89%E7%9A%84%E6%9E%9A%E4%B8%BE%E5%92%8C%E7%BB%93%E6%9E%84%E5%90%97%E6%AF%94%E5%A6%82%E7%94%9F%E6%88%90%E7%9A%84%E4%BB%A3%E7%A0%81%E4%B8%AD%E6%83%B3%E4%BD%BF%E7%94%A8unityengineaudiotype%E5%92%8Cunityenginecolor "可以引用现有的枚举和结构吗？比如生成的代码中想使用UnityEngine.AudioType和UnityEngine.Color的直接链接")

可以，支持external类型的枚举和结构，但目前只支持c#语言。 详细文档参见 [配置定义介绍](https://www.datable.cn/docs/manual/schema) 中的type mapper相关文档。

> Source: https://www.datable.cn/docs/beginner/streamandcolumnformat

Title: 使用列限定与紧凑格式 | Luban

URL Source: https://www.datable.cn/docs/beginner/streamandcolumnformat

Markdown Content:
## 使用列限定与紧凑格式

像结构与容器都是包含多个元素的数据类型，Luban提供了多种方式（**不限于以下这几种**）读取这种复合数据类型：

*   限定列格式，多个单元格，显式指定每个字段所占的列，然后读取
*   流式格式（紧凑格式），多个单元格，按顺序读取
*   流式格式（紧凑格式），一个单元格，使用分割符分割后按顺序读取
*   lite格式（紧凑格式），一个单元格
*   json格式（紧凑格式），一个单元格
*   多行读取。此方式只对容器类型有效。 读取元素时可以使用以上三种方式读取

详细数据文档见[Excel格式（高级）](https://www.datable.cn/docs/manual/exceladvanced)。

## 预备工作[​](https://www.datable.cn/docs/beginner/streamandcolumnformat#%E9%A2%84%E5%A4%87%E5%B7%A5%E4%BD%9C "预备工作的直接链接")

假设Item结构包含两个字段： `int item_id`和`int count`。

## 限定列格式，多个单元格，显式指定每个字段所占的列，然后读取[​](https://www.datable.cn/docs/beginner/streamandcolumnformat#%E9%99%90%E5%AE%9A%E5%88%97%E6%A0%BC%E5%BC%8F%E5%A4%9A%E4%B8%AA%E5%8D%95%E5%85%83%E6%A0%BC%E6%98%BE%E5%BC%8F%E6%8C%87%E5%AE%9A%E6%AF%8F%E4%B8%AA%E5%AD%97%E6%AE%B5%E6%89%80%E5%8D%A0%E7%9A%84%E5%88%97%E7%84%B6%E5%90%8E%E8%AF%BB%E5%8F%96 "限定列格式，多个单元格，显式指定每个字段所占的列，然后读取的直接链接")

通过新增子标题头行来为结构指定子字段的列。子标题头行第一列必须是'##var'。

![Image 1: item](blob:http://localhost/9bb6bc85aec62e2fea2e92e3a96a6a8c)

提示

限定列格式可以有多层*：即如果结构的某个子字段也是结构，仍然可以新增一行子标题头行，为子字段的子字段指定列。

## 流式格式，多个单元格，按顺序读取[​](https://www.datable.cn/docs/beginner/streamandcolumnformat#%E6%B5%81%E5%BC%8F%E6%A0%BC%E5%BC%8F%E5%A4%9A%E4%B8%AA%E5%8D%95%E5%85%83%E6%A0%BC%E6%8C%89%E9%A1%BA%E5%BA%8F%E8%AF%BB%E5%8F%96 "流式格式，多个单元格，按顺序读取的直接链接")

流式格式有个特点：它按顺序读取复合数据的每个元素（字段）。由于它无法限定每个字段的范围，它会跳过读到的所有空白单元格。 当字段不多时，这个并不是问题，可当字段变多时，很容易因为漏填了一个字段导致后续字段读取错误。

如以下item字段的填法，无论是在开始、中间、最后插入一个空单元格，都不影响数据读取。

![Image 2: item](https://www.datable.cn/assets/images/use_stream1-fe7587cd5c7a87d6bdb1081c6e5af5c3.jpg)

## 流式格式，一个单元格，使用分割符分割后按顺序读取[​](https://www.datable.cn/docs/beginner/streamandcolumnformat#%E6%B5%81%E5%BC%8F%E6%A0%BC%E5%BC%8F%E4%B8%80%E4%B8%AA%E5%8D%95%E5%85%83%E6%A0%BC%E4%BD%BF%E7%94%A8%E5%88%86%E5%89%B2%E7%AC%A6%E5%88%86%E5%89%B2%E5%90%8E%E6%8C%89%E9%A1%BA%E5%BA%8F%E8%AF%BB%E5%8F%96 "流式格式，一个单元格，使用分割符分割后按顺序读取的直接链接")

使用sep分割后，将每个数据当作一个单元格，它的填法与第一种方法相同。

![Image 3: item](https://www.datable.cn/assets/images/use_stream2-cf5224bace4ffe4e8e7392f10bdea178.jpg)

## lite格式[​](https://www.datable.cn/docs/beginner/streamandcolumnformat#lite%E6%A0%BC%E5%BC%8F "lite格式的直接链接")

在标题头字段添加`#format=lite`表示使用lite格式。lite格式是luban独有的数据格式，适合复杂的嵌套数据结构。 它的配置数据中不含字段名，因此比json和lua格式更简洁。详细文档见[lite格式](https://www.datable.cn/docs/manual/otherdatasource#lite%E6%A0%BC%E5%BC%8F)。

![Image 4: Item](https://www.datable.cn/assets/images/use_lite-a9c1037772bc7d7662c6c3829d445c13.jpg)

## json格式[​](https://www.datable.cn/docs/beginner/streamandcolumnformat#json%E6%A0%BC%E5%BC%8F "json格式的直接链接")

在标题头字段添加`#format=json`表示使用json格式。详细文档见[json格式](https://www.datable.cn/docs/manual/otherdatasource#json%E6%A0%BC%E5%BC%8F)。

![Image 5: Item](https://www.datable.cn/assets/images/use_json-98d65fa405ff2165e9f1f9d3cd520e47.jpg)

## lua格式[​](https://www.datable.cn/docs/beginner/streamandcolumnformat#lua%E6%A0%BC%E5%BC%8F "lua格式的直接链接")

在标题头字段添加`#format=lua`表示使用lua格式。详细文档见[lua格式](https://www.datable.cn/docs/manual/otherdatasource#lua%E6%A0%BC%E5%BC%8F)。

![Image 6: Item](https://www.datable.cn/assets/images/use_lua-c1de92183a66853221cae839d3dfadd1.jpg)

## 多行读取[​](https://www.datable.cn/docs/beginner/streamandcolumnformat#%E5%A4%9A%E8%A1%8C%E8%AF%BB%E5%8F%96 "多行读取的直接链接")

只有容器类型才能使用多行读取方式。在字段名前加'*'，表示此字段以多行方式读取。在读取每行数据时，既支持流式格式，也支持列限定格式。

![Image 7: item](https://www.datable.cn/assets/images/use_rows-374577d37cf23454b822b4e81856648c.jpg)

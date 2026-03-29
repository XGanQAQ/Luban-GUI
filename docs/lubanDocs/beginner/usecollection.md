# 使用容器类型
> Source: https://www.datable.cn/docs/beginner/usecollection

## 使用容器类型

容器是也是最常用的类型。Luban支持丰富的容器数据格式，可以按照需求选择最适合的类型和填写。

详细定义文档见[schema逻辑结构](/docs/manual/schema)和[类型系统](/docs/manual/types)，详细数据文档见[Excel格式（初级）](/docs/manual/excel)和[Excel格式（高级）](/docs/manual/exceladvanced)。

## 容器定义

目前支持4种基础容器类型，详细见 [容器类型](/docs/manual/types#容器类型)：

| 容器类型 | 描述 | 示例 |
|---------|------|------|
| array | 数组类型，与list相似，但生成的代码对应原生数组类型，如C#语言中对应 `xxx[]` | `array,int`、`array,string`、`array,Color`，`array,Item` |
| list | 列表类型，与array相似，但生成的代码对应List容器类型，如C#语言中对应 `List<xx>` | `list,int`、`list,string`、`list,Color`，`list,Item` |
| set | 集合类型，要求元素值唯一，生成的代码对应Set容器类型，如C#语言中对应 `HashSet<xx>` | `set,int`、`set,string`、`set,Color` |
| map | 字典类型，要求键唯一，生成的代码对应Map类型，如C#语言中对应`Dictionary<xx,yy>` | `map,int,int`、`map,string,string`, `map,Color,int`，`map,int,Item` |

## 填写array、list、set数据

常见有两种填法：

*   占多个单元格，每个单元格一个元素
*   占一个单元格，每个元素用分割符分割

占多个单元格时，字段名需要占多列，此时需要合并单元格让该字段占多列. 如果在使用csv这种不支持合并单元格的文件格式，可以使用`[{字段名}`和`{字段名}]`放在起始和结束列，表示这个字段占多列。

占一个单元格时，需要使用`sep=x`来指定分割符，分割符可以是','、'|'、';'、'&'、'\_'、'-' 等常见字符，如`sep=,`。

![item](/assets/images/use_list-9c0c728dc4e403625f1b62c04dd9d651.jpg)

元素类型也可以为枚举或者结构类型，下图只展示了array，list与set类型的填法类似。

![item](/assets/images/use_list2-825e82024dedf61964386e2f433ec1eb.jpg)

甚至可以多行填写，只要字段名前加'\*'，则表示以多行方式填写数据，每行一个元素，详细见[Excel格式（高级）](/docs/manual/exceladvanced)。

![item](/assets/images/use_list3-8b7ed9a5f3ed0834204c44af6d6d0e5b.jpg)

## 填写map数据

字典类型包含key和value，因此比array之类要复杂一些。

同样的，map也可以在一个单元格内填写，为了更好区别 key和value，以及划分每个key-value对，一般来说map的sep会填2个。示例如下：

![item](/assets/images/use_map-fa7e9e195425fd8511b92dfea680beab.jpg)

map的value类型也可以为结构类型，类似这样：

![item](/assets/images/use_map2-bce4de0106a053c50478ccb3a730bb27.jpg)

map也可以多行填写，如下：

![item](/assets/images/use_map3-bf848e769a2351bcc5171426e4aba165.jpg)

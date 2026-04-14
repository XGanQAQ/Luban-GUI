> Source: https://www.datable.cn/docs/manual/template

Title: 自定义模板 | Luban

URL Source: https://www.datable.cn/docs/manual/template

Markdown Content:
## 自定义模板

luban使用[scriban](https://github.com/scriban/scriban) 模板引擎来生成代码，也使用这个模板来生成自定义的文本型数据文件。

## 源项目中模板位置[​](https://www.datable.cn/docs/manual/template#%E6%BA%90%E9%A1%B9%E7%9B%AE%E4%B8%AD%E6%A8%A1%E6%9D%BF%E4%BD%8D%E7%BD%AE "源项目中模板位置的直接链接")

由于模块化，一般是每个子项目有独立的模板目录，而不是统一放到一个目录下。位置为`{proj}/Templates`，例如 `Luban.Csharp/Templates`。 为了最终发布时这些模块文件也会被复现到发布目录，对于每个模板文件，需要`右键->属性`，复制选项设置为`Copy Always`或者`Copy if newer`。

## 发布后自定义模板搜索路径[​](https://www.datable.cn/docs/manual/template#%E5%8F%91%E5%B8%83%E5%90%8E%E8%87%AA%E5%AE%9A%E4%B9%89%E6%A8%A1%E6%9D%BF%E6%90%9C%E7%B4%A2%E8%B7%AF%E5%BE%84 "发布后自定义模板搜索路径的直接链接")

发布后，所有模板文件都会被统一复制到输出目录的Templates目录。如果需要自定义模板，尽管可以直接修改Templates目录下的模板文件， 但每次更新Luban会覆盖自己的实现，不是很方便。你可以使用命令行参数"--customTemplateDir ${templatir}" 用于指定优先搜索路径。

### 代码模板环境变量[​](https://www.datable.cn/docs/manual/template#%E4%BB%A3%E7%A0%81%E6%A8%A1%E6%9D%BF%E7%8E%AF%E5%A2%83%E5%8F%98%E9%87%8F "代码模板环境变量的直接链接")

对于像cs这样的需要enum、bean、table、tables分开生成不同代码文件的语言，为每类对象的生成模板提供了默认机制。

enum

| 变量名 | 描述 |
| --- | --- |
| __ctx | 当前GenerationContext变量 |
| __name | 枚举名 |
| __namespace | 枚举的命名空间 |
| __top_module | 顶层命名空间，即target.TopModule |
| __namespace_with_top_module | 包含topModule的命名空间 |
| __full_name_with_top_module | 包含topModule的全名 |
| __enum | 当前枚举定义对象 |
| __this | 同__enum |
| __code_style | 当前代码风格 |

bean

| 变量名 | 描述 |
| --- | --- |
| __ctx | 当前GenerationContext变量 |
| __manager_name | target.manager值 |
| __manager_name_with_top_module | 包含topModule的target.manager |
| __name | 结构名 |
| __namespace | 命名空间 |
| __top_module | 顶层命名空间，即target.TopModule |
| __namespace_with_top_module | 包含topModule的命名空间 |
| __full_name_with_top_module | 包含topModule的全名 |
| __bean | 当前bean定义对象 |
| __this | 同__bean |
| __code_style | 当前代码风格 |

table

| 变量名 | 描述 |
| --- | --- |
| __ctx | 当前GenerationContext变量 |
| __manager_name | target.manager值 |
| __manager_name_with_top_module | 包含topModule的target.manager |
| __name | 结构名 |
| __namespace | 命名空间 |
| __top_module | 顶层命名空间，即target.TopModule |
| __namespace_with_top_module | 包含topModule的命名空间 |
| __full_name_with_top_module | 包含topModule的全名 |
| __table | 当前table定义对象 |
| __this | 同__table |
| __code_style | 当前代码风格 |
| __key_type | table的key类型 |
| __value_type | table的value类型 |

tables

| 变量名 | 描述 |
| --- | --- |
| __ctx | 当前GenerationContext变量 |
| __name | target.manager值 |
| __namespace | target.topModule |
| __tables | 当前导出的tables列表 |
| __this | 同__table |
| __code_style | 当前代码风格 |

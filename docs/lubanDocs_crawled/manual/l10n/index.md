> Source: https://www.datable.cn/docs/manual/l10n

Title: 本地化 | Luban

URL Source: https://www.datable.cn/docs/manual/l10n

Markdown Content:
## 本地化

支持多种本地化机制，它们可以同时使用。

## 特性[​](https://www.datable.cn/docs/manual/l10n#%E7%89%B9%E6%80%A7 "特性的直接链接")

支持以下几类本地化：

*   时间本地化。即将配置中的datetime类型数据，按照指定地区转换为UTC秒数
*   text类型。专用的表达本地化字符串key的类型，可以校验key的有效性
*   静态本地化。导出时将本地化key字符串转换为对应的实际语言值，避免运行时再转换的麻烦
*   [字段变体](https://www.datable.cn/docs/manual/variants)。为字段的每个本地化目标配置一个变体字段，导出时只导出指定变体的数据

## 基础概念[​](https://www.datable.cn/docs/manual/l10n#%E5%9F%BA%E7%A1%80%E6%A6%82%E5%BF%B5 "基础概念的直接链接")

### ITextProvider[​](https://www.datable.cn/docs/manual/l10n#itextprovider "ITextProvider的直接链接")

提示

如果未提供 `-x l10n.provider=xxx`参数，会自动禁用text有效性检查及静态本地化。

ITextProvider用于提供有效的text映射表，这样TextValidator可以检查本地化key的有效性。使用`-x l10n.provider=xxx`指定你使用ITextProvider， `default`对应DefaultTextProvider。你也可以实现自己的TextProvider。

### DefaultTextProvider[​](https://www.datable.cn/docs/manual/l10n#defaulttextprovider "DefaultTextProvider的直接链接")

使用 `-x l10n.textFile.path=xxx` 指定本地化key-value的文件。DefaultTextProvider使用builtin DataLoader读取本地化文件内数据。 DefaultTextProvider假定表格式如下：

`<bean name="TextInfo">  <var name="{key}" type="string"/>  <var name="{language1}" type="string"/>  <var name="{language2}" type="string"/>  ...</bean><table name="TbTextInfo" value="TextInfo" input="xxx"/>`

`{key}`和`{languege_xxx}`为自定义值，例如 `{key}`取 key, `{language_x}`取`zh`、`en`之类。

如果只作text校验，可以不需要`{langauge_xx}`之类的字段，luban只使用`{key}`字段校验key的有效性。

本地化数据文件可以是excel格式，也可以是json、xml之类的格式，只要它符合luban的builtin DataLoader的格式标准（见[excel格式](https://www.datable.cn/docs/manual/excel)和[非excel格式](https://www.datable.cn/docs/manual/otherdatasource)文档）即可。 当使用json格式时，由于不可能在一个json文件中只放一个text数据，实践中会让json文件中包含text列表，可以用复合数据文件格式（见[非excel格式](https://www.datable.cn/docs/manual/otherdatasource)） 填。以下为示例json格式的本地化文本表，通过`-x l10n.textFile.path=*@your_text_file.json` 指定读入text列表。

危险

本地化字符串文件为json之类的格式时，由于一个文件中包含多个本地化数据，本地化文件前一定要加上`*@`，例如`*@your_text_file.json`，否则加载出错！

![Image 1: text](https://www.datable.cn/assets/images/l10n_text-cfb7e64cd9dc44ecda12fce66b489f72.jpg)

## 时间本地化[​](https://www.datable.cn/docs/manual/l10n#%E6%97%B6%E9%97%B4%E6%9C%AC%E5%9C%B0%E5%8C%96 "时间本地化的直接链接")

datetime类型数据会根据目标时区，输出相应时刻的UTC秒数，方便程序使用。默认使用的时区为本地时区，可以通过命令行 参数 `--timeZone`或`-t`来指定目标时区。

该时区为linux或win下的时区名，例如: -t "Asia/Shanghai" 或者 -t "China Standard Time" 指定目标时区为 亚洲上海（也即北京时间）

## text类型[​](https://www.datable.cn/docs/manual/l10n#text%E7%B1%BB%E5%9E%8B "text类型的直接链接")

由于本地化需求的广泛性，特地提供text类型支持对应需求。text是一个特殊的语法糖，并不是一个独立的类型。它等价`string#text=1`，即包含`text=1` tag的string 类型。

text语义上对应本地化字符串的key，Luban在数据校验器中会检查这个key的有效性。

必须设置指定`l10n.provider`、`l10n.textFile.path`、`l10n.textFile.keyFieldName`这几个参数才会开启text校验。示例如下：

`-x l10n.provider=default ^  -x "l10n.textFile.path=*@D:\workspace2\luban_examples\DataTables\Datas\l10n\texts.json" ^  -x l10n.textFile.keyFieldName=key`

## 静态本地化[​](https://www.datable.cn/docs/manual/l10n#%E9%9D%99%E6%80%81%E6%9C%AC%E5%9C%B0%E5%8C%96 "静态本地化的直接链接")

静态本地化会在导出过程中将text类型由key转换为实际的文本值，适用于已经固定确定本地化语言的场合。使用者不需要再由key转换为实际的文档，使用较为方便。

必须设置了`l10n.provider`、`l10n.textFile.path`、`l10n.textFile.keyFieldName`、`l10n.textFile.languageFieldName`、`l10n.convertTextKeyToValue` 这几个参数才会开启静态本地化，示例如下：

`-x l10n.provider=default ^  -x "l10n.textFile.path=*@D:\workspace2\luban_examples\DataTables\Datas\l10n\texts.json" ^  -x l10n.textFile.keyFieldName=key ^  -x l10n.textFile.languageFieldName=zh ^  -x l10n.convertTextKeyToValue=1`

## 字段变体[​](https://www.datable.cn/docs/manual/l10n#%E5%AD%97%E6%AE%B5%E5%8F%98%E4%BD%93 "字段变体的直接链接")

详见[字段变体](https://www.datable.cn/docs/manual/variants)。

## 收集配置表中所有text的列表[​](https://www.datable.cn/docs/manual/l10n#%E6%94%B6%E9%9B%86%E9%85%8D%E7%BD%AE%E8%A1%A8%E4%B8%AD%E6%89%80%E6%9C%89text%E7%9A%84%E5%88%97%E8%A1%A8 "收集配置表中所有text的列表的直接链接")

有时候你想收集配置中所有的本地化text的key，DataTarget text-list 正用于此目的。text-list会收集配置表中出现的所有text数据，按从小到大排序后，输出文件。 通过`l10n.textListFile`选项指定输出文件，详细见[命令行工具](https://www.datable.cn/docs/manual/commandtools)文档。

一个示例命令如下：

`dotnet Luban.dll -t all -d text-list ^--conf D:\workspace2\luban_examples\DataTables\luban.conf ^--validationFailAsError ^-x outputDataDir=D:\workspace2\luban_examples\Projects\GenerateDatas\text ^-x l10n.textListFile=texts.txt`

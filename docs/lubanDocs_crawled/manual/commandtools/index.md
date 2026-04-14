> Source: https://www.datable.cn/docs/manual/commandtools

Title: 命令行工具 | Luban

URL Source: https://www.datable.cn/docs/manual/commandtools

Markdown Content:
## 命令行工具

## 跨平台[​](https://www.datable.cn/docs/manual/commandtools#%E8%B7%A8%E5%B9%B3%E5%8F%B0 "跨平台的直接链接")

得益于.net的跨平台能力，Luban可以在主流的Win、Linux及macOS操作系统上运行。

## 命令格式[​](https://www.datable.cn/docs/manual/commandtools#%E5%91%BD%E4%BB%A4%E6%A0%BC%E5%BC%8F "命令格式的直接链接")

`dotnet <path_of_luban.dll> [args]  -s, --schemaCollector        schema collector name  --conf                       Required. luban conf file  -t, --target                 Required. target name  -c, --codeTarget             code target name  -d, --dataTarget             data target name  -p, --pipeline               pipeline name  -f, --forceLoadTableDatas    force load table datas when not any dataTarget  -i, --includeTag             include tag  -e, --excludeTag             exclude tag  --variant                    field variants  -o, --outputTable            output table  --timeZone                   time zone  --customTemplateDir          custom template dirs  --validationFailAsError      validation fail as error  -x, --xargs                  args like -x a=1 -x b=2  -l, --logConfig              (Default: nlog.xml) nlog config file  -w, --watchDir               watch dir and regererate when dir changes  -v, --verbose                verbose  --help                       Display this help screen.  --version                    Display version information.`

| 参数 | 必选 | 默认值 | 描述 |
| --- | --- | --- | --- |
| -s, --schemaCollector | 否 | default | schema根收集器 |
| --conf | 是 |  | luban配置项 |
| -t， --target | 是 |  | 生成目标，取schema全局参数target中的一个 |
| -c, --codeTarget | 否 |  | 生成的代码目标。可以有0-n个。如 `-c cs-bin -c java-json` |
| -d, --dataTarget | 否 |  | 生成的数据目标。可以有0-n个。如 `-d bin -d json` |
| -f, --forceLoadTableDatas | 否 | false | 即使没有指定任何dataTarget也要强行加载配置数据，适用于在配置表提交前检查配置合法性 |
| -p, --pipeline | 否 | default | 生成管线。默认为内置的DefaultPipeline |
| -i, --includeTag | 否 |  | tag为空或者为该tag的记录会被输出到数据目标，其他tag数据会被忽略。 --includeTag与--excludeTag不能同时存在 |
| -e, --excludeTag | 否 |  | 包含该tag的记录不会被输出到数据目标。 --includeTag与--excludeTag不能同时存在 |
| --variant | 否 |  | 指定使用的字段变体，格式为 `--variant {variantKey}={variantName}`，其中`{variantKey}`为`{beanFullName}.{fieldName}`（如 `test.TestVariant.value`）。 可以有多个`--variant`用于为不同的字段指定变体。详细文档见[变量变体](https://www.datable.cn/docs/manual/variants) |
| -o, --outputTable | 否 |  | 指定要生成的table，可以有多个，例如`-o item.tbItem -o bag.TbBag`。如果未指定此参数，则按照group规则计算导出的table列表 |
| --timeZone | 否 |  | 指定当前时区，默认取本地时区。此参数会影响datetime类型。该参数为linux或win下的时区名，例如 `Asia/Shanghai` 或 `China Standard Time` |
| --customTemplateDir | 否 |  | 自定义template搜索路径，优先级搜索此路径，再搜索默认的Templates路径 |
| --validationFailAsError | 否 | false | 如果有任何校验器未通过，则生成失败。此参数一般在正式发布时使用 |
| -x, --xargs | 否 |  | 指定一些特殊参数。具体需要哪些参数由生成管线运行过程中涉及的模块决定 |

## SchemaCollector[​](https://www.datable.cn/docs/manual/commandtools#schemacollector "SchemaCollector的直接链接")

Luban.SchemaCollector.Builtin项目实现了DefaultSchemaCollector，它支持与旧版本Luban相似的定义格式。该schemaCollector名为default。

## Code Target[​](https://www.datable.cn/docs/manual/commandtools#code-target "Code Target的直接链接")

目前内置支持以下code target：

| code target | 描述 |
| --- | --- |
| cs-bin | C#，读取bin格式文件 |
| cs-simple-json | C#，使用SimpleJSON读取json文件，推荐用于Unity客户端 |
| cs-dotnet-json | C#，使用System.Text.Json库读取json文件，推荐用于dotnet core服务器 |
| cs-newtonsoft-json | C#，使用Newtonsoft.Json库读取json文件 |
| cs-editor-json | C#，读取与保存记录为单个json文件，适用于自定义编辑器保存与加载原始配置文件 |
| cs-protobuf2 | 生成加载所有protobuf bin和json格式数据的代码，仅含Tables类 |
| cs-protobuf3 | 生成加载所有protobuf bin和json格式数据的代码，仅含Tables类 |
| lua-lua | lua，读取lua格式的文件 |
| lua-bin | lua，读取bin格式文件 |
| java-bin | java，读取bin格式文件 |
| java-json | java，使用gson库读取json格式文件 |
| ~~cpp-bin~~ | cpp，读取bin格式文件。自v2.3.0版本起移除 |
| cpp-sharedptr-bin | cpp，使用智能指针保存动态分配的对象，读取bin格式文件 |
| cpp-rawptr-bin | cpp，使用裸指针保存动态分配的对象，读取bin格式文件 |
| go-bin | go，读取bin格式文件 |
| go-json | go，读取json格式文件 |
| python-json | python，读取json格式文件 |
| gdscript-json | gdscript，读取json格式文件。注意，如果你使用C#语言开发，推荐使用更高效的cs-bin格式 |
| javascript-bin | javascript，读取bin格式文件 |
| javascript-json | javascript，读取json格式文件 |
| typescript-bin | typescript，读取bin格式文件 |
| typescript-json | typescript，读取json格式文件 |
| typescript-protobuf | typescript，生成读取protobuf格式数据的代码，仅含Tables类 |
| rust-bin | 生成rust代码，读取bin格式文件 |
| rust-json | 生成rust代码，读取json格式文件 |
| php-json | php，读取json格式文件 |
| dart-json | 生成dart代码，读取json格式文件 |
| protobuf2 | 生成proto2语法的schema文件 |
| protobuf3 | 生成proto3语法的schema文件 |
| flatbuffers | 生成flatbuffers的schema文件 |

警告

code target必须与data target匹配，否则会加载失败。

一次生成多个code target时，必须为每个code target单独指定输出目录，否则会相互覆盖。实践中也不可能让不同的代码输出到同一个目录下。

Luban的大多数内置模板都使用了[层级参数(Cascading Option)](https://www.datable.cn/docs/manual/cascadingoption)机制，你只需要用`<code target name>.outputCodeDir`参数 分别为每个target指定输出目录参数即可。

## Data Target[​](https://www.datable.cn/docs/manual/commandtools#data-target "Data Target的直接链接")

内置支持以下 data target：

| data target | 描述 |
| --- | --- |
| bin | Luban独有的binary格式，紧凑、高效，推荐用于正式发布 |
| bin-offset | 记录以bin格式导出的数据文件中每个记录的索引位置，可以用于以记录为粒度的lazy加载 |
| json | json格式，map输出成[[key, value]]格式 |
| json2 | 与json格式类似，但map输出成{"key":"value"}格式 |
| lua | lua格式 |
| xml | xml格式 |
| yaml | yaml格式 |
| bson | bson格式 |
| msgpack | msgpack的二进制格式 |
| protobuf2-bin | protobuf2的二进制格式 |
| protobuf3-bin | protobuf3的二进制格式 |
| protobuf2-json | protobuf2支持的json格式 |
| protobuf3-json | protobuf3起支持的json格式 |
| flatbuffers-json | flatbuffers支持的json格式 |
| text-list | 输出配置出现的所有text key，按从小到大排序 |

如果要一次输出多个target，解决办法与code target类似。只需要用`<data target name>.outputDataDir`参数 分别为每个target指定输出目录参数即可。

每个code target只能读取一种相匹配的数据格式。例如cs-bin生成的c#代码只能读取bin格式数据， java-json生成的java代码只能读取json格式。

## Pipeline[​](https://www.datable.cn/docs/manual/commandtools#pipeline "Pipeline的直接链接")

Luban.Core中实现一个默认管线DefaultPipeline，名为default。使用者可以实现自己的Pipeline。

## xargs[​](https://www.datable.cn/docs/manual/commandtools#xargs "xargs的直接链接")

还有大量的命令行参数，由于它们是Pipeline中各个可定制模块独有的参数，因而这些参数没放到标准命令行参数中，统一用`-x --xargs`参数指令。

内置模块用到的参数有：

| 参数 | 描述 | 可用值 | 示例 |
| --- | --- | --- | --- |
| {codeTarget}.outputCodeDir | 代码目标的输出目录 |  | -x outputCodeDir=/my/output/dir |
| {dataTarget}.outputDataDir | 数据目标的输出目录 |  | -x outputDataDir=/my/output/dir |
| codeStyle | 代码目标的命名风格，内置实现的Code Target都会自动设置与目标语言相匹配的codeStyle，不需要显式指定 | none、csharp-default、java-default、go-default、lua-default、typescript-default、cpp-default、python-default、dart-default | -x codeStyle=csharp-default |
| namingConvention.{codeTarget}.{location} | codeTarget为`--codeTarget`参数中指定的target名。location为风格位置，可选值为namespace、type、method、property、field、enumItem，详见[代码风格](https://www.datable.cn/docs/manual/codestyle)。该参数为层级选项，如果不指定{codeTarget}，则对所有target生效 | none、pascal、camel、upper、snake | -x namingConvention.cs-bin.field=pascal |
| dataExporter | 数据导出器 | null、default | -x dataExporter=default |
| codePostprocess | 代码后处理器，可以为多个 | 未实现任何内置postprocess | -x codePostProcess=a,b,c |
| dataPostprocess | 数据后处理器，可以为多个 | 未实现任何内置postprocess | -x dataPostProcess=a,b |
| outputSaver | 数据保存器，默认为local，即输出到本地目录，如果不想输出任何文件，可以用null | null、local | -x outputSaver=local |
| outputSaver.{codeTarget|dataTarget}.cleanUpOutputDir | 输出文件前是否清除outputCodeDir或者outputDataDir目录中多余的文件，默认为true |  | -x outputSaver.cs-bin.cleanUpOutputDir=0 |
| l10n.provider | 本地化文本Provider，如果未设置此参数则不进行任何本地化相关操作，包括text校验和静态转换 | default | -x l10n.provider=default |
| l10n.textFile.path | 本地化文本数据文件，当设置了l10.provider时必填此值 |  | -x l10n.textFile.path=xxxx |
| l10n.textFile.keyFieldName | 本地化文本数据文件中数据项key字段的字段名 |  | -x l10n.textFile.keyFieldName=key |
| l10n.textFile.languageFieldName | 本地化文本数据文件中数据项对应语言的文本值字段的字段名 |  | -x l10n.languageFieldName=en |
| l10n.convertTextKeyToValue | 执行静态本地化，将key替换为对应语言的文本值 |  | -x l10n.convertTextKeyToValue=1 |
| l10n.textListFile | 输出的配置中所有text key的列表的文件，配合 DataTarget text-list使用 |  |  |
| pathValidator.rootDir | path校验器搜索文件所用的根目录 |  | -x pathValidator.rootDir=/xx/yy |
| {code|data}.lineEnding | 生成的代码和数据文件的行尾符。该选项只对文本类型的输出文件有效，对于二进制格式如bin、bson之类的格式无效 | 可为CR、LF、CRLF，如果未指定则取Environment.NewLine为行尾符 | -x lineEnding=LF, -x code.LineEnding=LF、-x data.lineEnding=LF |
| json.compact | 是否输出紧凑无缩进的json数据，与json或json2 dataTarget配合使用，默认为0 | 0、1、true、false | -x compact=1 |
| {dataTarget}.fileExt | 输出数据文件的文件名后缀 |  | -x bin.fileExt=bin |
| ~~{dataTarget}.outputDataExtension~~ | 输出数据文件的文件名后缀。v2.12.0版本移除，改名为fileExt |  | -x bin.outputDataExtension=bin |
| {codeTarget|dataTarget}.fileEncoding | 输出文件的字符编码 | -x lua.fileEncoding=gb2313 |  |

## OutputSaver[​](https://www.datable.cn/docs/manual/commandtools#outputsaver "OutputSaver的直接链接")

最终生成的数据的保存器。当前实现了两个保存器local和null。

local将文件保存到本地目录。null则不执行任何操作。local是默认使用的保存器，一般生成任务使用local。对于只想校验配置表，不想生成任何数据，使用null 保存器可以达到这个目标。

## 示例[​](https://www.datable.cn/docs/manual/commandtools#%E7%A4%BA%E4%BE%8B "示例的直接链接")

下面展示了一些常见的生成命令示例，更多示例请参考 [luban_examples/Projects](https://gitee.com/focus-creative-games/luban_examples/tree/main/Projects)。

### unity + c# + bin[​](https://www.datable.cn/docs/manual/commandtools#unity--c--bin "unity + c# + bin的直接链接")

示例项目[Csharp_Unity_bin](https://gitee.com/focus-creative-games/luban_examples/tree/main/Projects/Csharp_Unity_bin)。

`set WORKSPACE=..\..set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dllset CONF_ROOT=%WORKSPACE%\DataTablesdotnet %LUBAN_DLL% ^    -t all ^    -c cs-bin ^    -d bin  ^    --conf %CONF_ROOT%\luban.conf ^    -x outputCodeDir=Assets/Gen ^    -x outputDataDir=..\GenerateDatas\bytes ^    -x pathValidator.rootDir=%WORKSPACE%\Projects\Csharp_Unity_bin ^    -x l10n.provider=*@%WORKSPACE%\DataTables\Datas\l10n\texts.json`

### unity + c# + json[​](https://www.datable.cn/docs/manual/commandtools#unity--c--json "unity + c# + json的直接链接")

示例项目[Csharp_Unity_json](https://gitee.com/focus-creative-games/luban_examples/tree/main/Projects/Csharp_Unity_json)。

`set WORKSPACE=..\..set GEN_CLIENT=%WORKSPACE%\Tools\Luban\Luban.dllset CONF_ROOT=%WORKSPACE%\DataTablesdotnet %GEN_CLIENT% ^    -t all ^    -c cs-simple-json ^    -d json  ^    --conf %CONF_ROOT%\luban.conf ^    -x outputCodeDir=Assets/Gen ^    -x outputDataDir=..\GenerateDatas\json ^    -x pathValidator.rootDir=D:\workspace2\luban_examples\Projects\Csharp_Unity_bin ^    -x l10n.provider=*@D:\workspace2\luban_examples\DataTables\Datas\l10n\texts.json`

### dotnet + c# + bin[​](https://www.datable.cn/docs/manual/commandtools#dotnet--c--bin "dotnet + c# + bin的直接链接")

示例项目[Csharp_DotNet_bin](https://gitee.com/focus-creative-games/luban_examples/tree/main/Projects/Csharp_DotNet_bin)。

`set WORKSPACE=..\..set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dllset CONF_ROOT=%WORKSPACE%\DataTablesdotnet %LUBAN_DLL% ^    -t all ^    -c cs-bin ^    -d bin  ^    --conf %CONF_ROOT%\luban.conf ^    -x outputCodeDir=Gen ^    -x outputDataDir=..\GenerateDatas\bytes ^    -x pathValidator.rootDir=%WORKSPACE%\Projects\Csharp_Unity_bin ^    -x l10n.provider=*@%WORKSPACE%\DataTables\Datas\l10n\texts.json`

### go + bin[​](https://www.datable.cn/docs/manual/commandtools#go--bin "go + bin的直接链接")

示例项目 [Go_bin](https://gitee.com/focus-creative-games/luban_examples/tree/main/Projects/Go_bin)。

`set WORKSPACE=..\..set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dllset CONF_ROOT=%WORKSPACE%\DataTablesdotnet %LUBAN_DLL% ^    -t all ^    -c go-bin ^    -d bin  ^    --conf %CONF_ROOT%\luban.conf ^    -x outputCodeDir=gen ^    -x outputDataDir=..\GenerateDatas\bytes ^    -x pathValidator.rootDir=%WORKSPACE%\Projects\Csharp_Unity_bin ^    -x l10n.provider=*@%WORKSPACE%\DataTables\Datas\l10n\texts.json ^    -x lubanGoModule=demo/luban`

### java + bin[​](https://www.datable.cn/docs/manual/commandtools#java--bin "java + bin的直接链接")

示例项目 [Java_bin](https://gitee.com/focus-creative-games/luban_examples/tree/main/Projects/Java_bin)

`set WORKSPACE=..\..set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dllset CONF_ROOT=%WORKSPACE%\DataTablesdotnet %LUBAN_DLL% ^    -t all ^    -c java-bin ^    -d bin  ^    --conf %CONF_ROOT%\luban.conf ^    -x outputCodeDir=src/main/gen ^    -x outputDataDir=..\GenerateDatas\bytes ^    -x pathValidator.rootDir=%WORKSPACE%\Projects\Csharp_Unity_bin ^    -x l10n.provider=*@%WORKSPACE%\DataTables\Datas\l10n\texts.json`

### 用于策划检查配置，不生成任何代码和文件[​](https://www.datable.cn/docs/manual/commandtools#%E7%94%A8%E4%BA%8E%E7%AD%96%E5%88%92%E6%A3%80%E6%9F%A5%E9%85%8D%E7%BD%AE%E4%B8%8D%E7%94%9F%E6%88%90%E4%BB%BB%E4%BD%95%E4%BB%A3%E7%A0%81%E5%92%8C%E6%96%87%E4%BB%B6 "用于策划检查配置，不生成任何代码和文件的直接链接")

示例项目 [ConfigCheck](https://gitee.com/focus-creative-games/luban_examples/tree/main/DataTables)。

`set WORKSPACE=..set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dllset CONF_ROOT=%WORKSPACE%\DataTablesdotnet %LUBAN_DLL% ^    -t all ^    --conf %CONF_ROOT%\luban.conf ^    -x pathValidator.rootDir=%WORKSPACE%\Projects\Csharp_Unity_bin ^    -x l10n.provider=*@%WORKSPACE%\DataTables\Datas\l10n\texts.json ^    -x forceLoadDatas=1`

### 同时生成 cs-bin和java-bin代码[​](https://www.datable.cn/docs/manual/commandtools#%E5%90%8C%E6%97%B6%E7%94%9F%E6%88%90-cs-bin%E5%92%8Cjava-bin%E4%BB%A3%E7%A0%81 "同时生成 cs-bin和java-bin代码的直接链接")

`set WORKSPACE=..\..set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dllset CONF_ROOT=%WORKSPACE%\DataTablesdotnet %LUBAN_DLL% ^    -t all ^    -c cs-bin ^    -c java-bin ^    -d bin  ^    --conf %CONF_ROOT%\luban.conf ^    -x cs-bin.outputCodeDir=cs_output_path ^    -x java-bin.outputCodeDir=java_output_path ^    -x outputDataDir=..\GenerateDatas\bytes ^    -x pathValidator.rootDir=%WORKSPACE%\Projects\Csharp_Unity_bin ^    -x l10n.provider=*@%WORKSPACE%\DataTables\Datas\l10n\texts.json`

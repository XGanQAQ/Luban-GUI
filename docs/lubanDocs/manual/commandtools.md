# 命令行工具
> Source: https://www.datable.cn/docs/manual/commandtools

## 命令行工具

## 跨平台

得益于.net的跨平台能力，Luban可以在主流的Win、Linux及macOS操作系统上运行。

## 命令格式

```
dotnet <path_of_luban.dll> [args]
  -s, --schemaCollector        schema collector name
  --conf                       Required. luban conf file
  -t, --target                 Required. target name
  -c, --codeTarget             code target name
  -d, --dataTarget             data target name
  -p, --pipeline               pipeline name
  -f, --forceLoadTableDatas    force load table datas when not any dataTarget
  -i, --includeTag             include tag
  -e, --excludeTag             exclude tag
  --variant                    field variants
  -o, --outputTable            output table
  --timeZone                   time zone
  --customTemplateDir          custom template dirs
  --validationFailAsError      validation fail as error
  -x, --xargs                  args like -x a=1 -x b=2
  -l, --logConfig              (Default: nlog.xml) nlog config file
  -w, --watchDir               watch dir and regererate when dir changes
  -v, --verbose                verbose
  --help                       Display this help screen.
  --version                    Display version information.
```

| 参数 | 必选 | 默认值 | 描述 |
|------|------|--------|------|
| -s, --schemaCollector | 否 | default | schema根收集器 |
| --conf | 是 | | luban配置项 |
| -t， --target | 是 | | 生成目标，取schema全局参数target中的一个 |
| -c, --codeTarget | 否 | | 生成的代码目标。可以有0-n个。如 `-c cs-bin -c java-json` |
| -d, --dataTarget | 否 | | 生成的数据目标。可以有0-n个。如 `-d bin -d json` |
| -f, --forceLoadTableDatas | 否 | false | 即使没有指定任何dataTarget也要强行加载配置数据，适用于在配置表提交前检查配置合法性 |
| -p, --pipeline | 否 | default | 生成管线。默认为内置的DefaultPipeline |
| -i, --includeTag | 否 | | tag为空或者为该tag的记录会被输出到数据目标，其他tag数据会被忽略。 --includeTag与--excludeTag不能同时存在 |
| -e, --excludeTag | 否 | | 包含该tag的记录不会被输出到数据目标。 --includeTag与--excludeTag不能同时存在 |
| --variant | 否 | | 指定使用的字段变体，格式为 `--variant {variantKey}={variantName}` |
| -o, --outputTable | 否 | | 指定要生成的table，可以有多个 |
| --timeZone | 否 | | 指定当前时区，默认取本地时区 |
| --customTemplateDir | 否 | | 自定义template搜索路径 |
| --validationFailAsError | 否 | false | 如果有任何校验器未通过，则生成失败 |
| -x, --xargs | 否 | | 指定一些特殊参数 |

## SchemaCollector

Luban.SchemaCollector.Builtin项目实现了DefaultSchemaCollector，它支持与旧版本Luban相似的定义格式。该schemaCollector名为default。

## Code Target

目前内置支持以下code target：

| code target | 描述 |
|-------------|------|
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
| cpp-sharedptr-bin | cpp，使用智能指针保存动态分配的对象，读取bin格式文件 |
| cpp-rawptr-bin | cpp，使用裸指针保存动态分配的对象，读取bin格式文件 |
| go-bin | go，读取bin格式文件 |
| go-json | go，读取json格式文件 |
| python-json | python，读取json格式文件 |
| gdscript-json | gdscript，读取json格式文件 |
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

> **警告**
> code target必须与data target匹配，否则会加载失败。
> 一次生成多个code target时，必须为每个code target单独指定输出目录，否则会相互覆盖。

## Data Target

内置支持以下 data target：

| data target | 描述 |
|-------------|------|
| bin | Luban独有的binary格式，紧凑、高效，推荐用于正式发布 |
| bin-offset | 记录以bin格式导出的数据文件中每个记录的索引位置，可以用于以记录为粒度的lazy加载 |
| json | json格式，map输出成\[\[key, value\]\]格式 |
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

## Pipeline

Luban.Core中实现一个默认管线DefaultPipeline，名为default。使用者可以实现自己的Pipeline。

## xargs

还有大量的命令行参数，由于它们是Pipeline中各个可定制模块独有的参数，因而这些参数没放到标准命令行参数中，统一用`-x --xargs`参数指令。

内置模块用到的参数有：

| 参数 | 描述 | 可用值 | 示例 |
|------|------|--------|------|
| {codeTarget}.outputCodeDir | 代码目标的输出目录 | | `-x outputCodeDir=/my/output/dir` |
| {dataTarget}.outputDataDir | 数据目标的输出目录 | | `-x outputDataDir=/my/output/dir` |
| codeStyle | 代码目标的命名风格 | none、csharp-default、java-default等 | `-x codeStyle=csharp-default` |
| namingConvention.{codeTarget}.{location} | 命名风格位置 | none、pascal、camel、upper、snake | `-x namingConvention.cs-bin.field=pascal` |
| dataExporter | 数据导出器 | null、default | `-x dataExporter=default` |
| outputSaver | 数据保存器 | null、local | `-x outputSaver=local` |
| l10n.provider | 本地化文本Provider | default | `-x l10n.provider=default` |
| l10n.textFile.path | 本地化文本数据文件 | | `-x l10n.textFile.path=xxxx` |
| l10n.convertTextKeyToValue | 执行静态本地化 | | `-x l10n.convertTextKeyToValue=1` |
| pathValidator.rootDir | path校验器搜索文件所用的根目录 | | `-x pathValidator.rootDir=/xx/yy` |
| json.compact | 是否输出紧凑无缩进的json数据 | 0、1、true、false | `-x compact=1` |
| {dataTarget}.fileExt | 输出数据文件的文件名后缀 | | `-x bin.fileExt=bin` |

## OutputSaver

最终生成的数据的保存器。当前实现了两个保存器local和null。

local将文件保存到本地目录。null则不执行任何操作。local是默认使用的保存器，一般生成任务使用local。对于只想校验配置表，不想生成任何数据，使用null 保存器可以达到这个目标。

## 示例

下面展示了一些常见的生成命令示例，更多示例请参考 [luban\_examples/Projects](https://gitee.com/focus-creative-games/luban_examples/tree/main/Projects)。

### unity + c# + bin

```batch
set WORKSPACE=..\..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=%WORKSPACE%\DataTables
dotnet %LUBAN_DLL% ^
    -t all ^
    -c cs-bin ^
    -d bin  ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputCodeDir=Assets/Gen ^
    -x outputDataDir=..\GenerateDatas\bytes ^
    -x pathValidator.rootDir=%WORKSPACE%\Projects\Csharp_Unity_bin ^
    -x l10n.provider=*@%WORKSPACE%\DataTables\Datas\l10n\texts.json
```

### unity + c# + json

```batch
set WORKSPACE=..\..
set GEN_CLIENT=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=%WORKSPACE%\DataTables
dotnet %GEN_CLIENT% ^
    -t all ^
    -c cs-simple-json ^
    -d json  ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputCodeDir=Assets/Gen ^
    -x outputDataDir=..\GenerateDatas\json
```

### dotnet + c# + bin

```batch
set WORKSPACE=..\..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=%WORKSPACE%\DataTables
dotnet %LUBAN_DLL% ^
    -t all ^
    -c cs-bin ^
    -d bin  ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputCodeDir=Gen ^
    -x outputDataDir=..\GenerateDatas\bytes
```

### go + bin

```batch
set WORKSPACE=..\..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=%WORKSPACE%\DataTables
dotnet %LUBAN_DLL% ^
    -t all ^
    -c go-bin ^
    -d bin  ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputCodeDir=gen ^
    -x outputDataDir=..\GenerateDatas\bytes ^
    -x lubanGoModule=demo/luban
```

### java + bin

```batch
set WORKSPACE=..\..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=%WORKSPACE%\DataTables
dotnet %LUBAN_DLL% ^
    -t all ^
    -c java-bin ^
    -d bin  ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputCodeDir=src/main/gen ^
    -x outputDataDir=..\GenerateDatas\bytes
```

### 用于策划检查配置，不生成任何代码和文件

```batch
set WORKSPACE=..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=%WORKSPACE%\DataTables
dotnet %LUBAN_DLL% ^
    -t all ^
    --conf %CONF_ROOT%\luban.conf ^
    -x forceLoadDatas=1
```

### 同时生成 cs-bin和java-bin代码

```batch
set WORKSPACE=..\..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=%WORKSPACE%\DataTables
dotnet %LUBAN_DLL% ^
    -t all ^
    -c cs-bin ^
    -c java-bin ^
    -d bin  ^
    --conf %CONF_ROOT%\luban.conf ^
    -x cs-bin.outputCodeDir=cs_output_path ^
    -x java-bin.outputCodeDir=java_output_path ^
    -x outputDataDir=..\GenerateDatas\bytes
```

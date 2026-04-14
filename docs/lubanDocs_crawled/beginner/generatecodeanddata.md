> Source: https://www.datable.cn/docs/beginner/generatecodeanddata

Title: 生成代码和数据 | Luban

URL Source: https://www.datable.cn/docs/beginner/generatecodeanddata

Markdown Content:
## 生成代码和数据

Luban不仅支持导出配置数据，也内置支持生成多种语言的代码，用于运行时加载配置。同时Luban也支持protobuf、flatbuffers、msgpack等流行的消息框架。 你使用的语言即使不在默认支持的语言列表，也可以通过这些消息框架来支持。

## 生成数据[​](https://www.datable.cn/docs/beginner/generatecodeanddata#%E7%94%9F%E6%88%90%E6%95%B0%E6%8D%AE "生成数据的直接链接")

命令行参数 `-d {dataTarget}`用于指定生成的数据类型，参数`-x outputDataDir={dataOutputDir}`用于指定数据输出目录。假设我们要生成json数据，示例如下：

`set GEN_CLIENT={Luban.dll的路径}set CONF_ROOT={DataTables目录的路径}dotnet %GEN_CLIENT% ^    -t client ^    -d json ^    --conf %CONF_ROOT%\luban.conf ^    -x outputDataDir=..\GenerateDatas\jsonpause`

更多的dataTarget可见 [DataTarget列表](https://www.datable.cn/docs/manual/commandtools#data-target)。

## 生成代码[​](https://www.datable.cn/docs/beginner/generatecodeanddata#%E7%94%9F%E6%88%90%E4%BB%A3%E7%A0%81 "生成代码的直接链接")

命令行参数 `-c {codeTarget}`用于指定生成的代码类型，参数 `-x outputCodeDir={codeOutputDir}`用于指定代码输出目录。假设我们要生成加载json数据的c#代码，示例如下：

`set GEN_CLIENT={Luban.dll的路径}set CONF_ROOT={DataTables目录的路径}dotnet %GEN_CLIENT% ^    -t client ^    -c cs-simple-json ^    --conf %CONF_ROOT%\luban.conf ^    -x outputCodeDir=Assets/Genpause`

codeTarget `cs-simple-json`中`cs`表示生成c#语言代码，`simple`表示使用SimpleJson加载json文件，`json`表示加载json数据。所有的codeTarget都遵循这个命名规则。

对于同一种数据格式（如json）,为所有语言生成的数据文件都是一样的。也就是导出数据不受`-c`参数影响。

更多的codeTarget可见 [CodeTarget列表](https://www.datable.cn/docs/manual/commandtools#code-target)。

危险

请不将`outputCodeDir`和`outputDataDir`指向相同目录，它们会互相覆盖！

## 同时生成代码和数据[​](https://www.datable.cn/docs/beginner/generatecodeanddata#%E5%90%8C%E6%97%B6%E7%94%9F%E6%88%90%E4%BB%A3%E7%A0%81%E5%92%8C%E6%95%B0%E6%8D%AE "同时生成代码和数据的直接链接")

运行两次命令分别生成代码和数据不仅麻烦，也增加了生成时间。Luban支持一次生成代码和数据，示例如下：

`set GEN_CLIENT={Luban.dll的路径}set CONF_ROOT={DataTables目录的路径}dotnet %GEN_CLIENT% ^    -t client ^    -c cs-simple-json ^    -d json ^    --conf %CONF_ROOT%\luban.conf ^    -x outputCodeDir=Assets/Gen ^    -x outputDataDir=..\GenerateDatas\json`

一次性生成多种语言和数据格式也是可以的，只要指定多个`-c`和`-d`参数即可，不过要求它们必须是相同的分组（即-t 参数相同）。很显然，它们不应该被输出到相同目录。Luban支持[层级参数机制](https://www.datable.cn/docs/manual/cascadingoption)，当有多个codeTarget或者dataTarget 时，可以通过`{codeTarget}.outputCodeDir=xxx`和`{dataTarget}.outputDataDir=xxx`来分别指定它们的输出目录。示例如下：

`set GEN_CLIENT={Luban.dll的路径}set CONF_ROOT={DataTables目录的路径}dotnet %GEN_CLIENT% ^    -t client ^    -c cs-simple-json ^    -c cs-bin ^    -d json ^    -d bin ^    --conf %CONF_ROOT%\luban.conf ^    -x cs-simple-json.outputCodeDir=Assets/Gen/Json ^    -x cs-bin.outputCodeDir=Assets/Gen/Bin ^    -x json.outputDataDir=..\GenerateDatas\json ^    -x bin.outputDataDir=..\GenerateDatas\bin`

危险

请不将多个`outputCodeDir`指向相同目录，也不要将多个`outputDataDir`指向相同目录，它们会互相覆盖！

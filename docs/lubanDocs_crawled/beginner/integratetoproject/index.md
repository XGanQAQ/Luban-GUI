> Source: https://www.datable.cn/docs/beginner/integratetoproject

Title: 集成到项目 | Luban

URL Source: https://www.datable.cn/docs/beginner/integratetoproject

Markdown Content:
## 集成到项目

## 安装 dotnet SDK[​](https://www.datable.cn/docs/beginner/integratetoproject#%E5%AE%89%E8%A3%85-dotnet-sdk "安装 dotnet SDK的直接链接")

按[快速上手](https://www.datable.cn/docs/beginner/quickstart)要求，安装[dotnet sdk 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)。

## 下载[luban_examples项目](https://gitee.com/focus-creative-games/luban_examples)[​](https://www.datable.cn/docs/beginner/integratetoproject#%E4%B8%8B%E8%BD%BDluban_examples%E9%A1%B9%E7%9B%AE "下载luban_examples项目的直接链接")

使用git clone或者下载zip的方式下载luban_examples项目。

一般来说`luban_examples/Tools/Luban`目录已经包含了最新的Luban二进制代码。如果想自己编译，按以下步骤：

*   将[Luban](https://gitee.com/focus-creative-games/luban) Clone到luban_examples同级目录（即luban和luban_examples在同一个目录下），目录名必须为luban
*   运行 `luban_examples/Tools/build-luban.bat`

如果一切顺利，`luban_examples/Tools/Luban`目录会替换为Luban最新版本的二进制代码。

## 复制Luban工具到你的项目[​](https://www.datable.cn/docs/beginner/integratetoproject#%E5%A4%8D%E5%88%B6luban%E5%B7%A5%E5%85%B7%E5%88%B0%E4%BD%A0%E7%9A%84%E9%A1%B9%E7%9B%AE "复制Luban工具到你的项目的直接链接")

大多数项目一般有一个专用目录放这些第三方的工具，比如说 `{proj}/Tools`，将`luban_examples/Tools/Luban`复制到任意合适的目录即可。

## 创建策划配置目录[​](https://www.datable.cn/docs/beginner/integratetoproject#%E5%88%9B%E5%BB%BA%E7%AD%96%E5%88%92%E9%85%8D%E7%BD%AE%E7%9B%AE%E5%BD%95 "创建策划配置目录的直接链接")

将`luban_examples/MiniTemplate`复制到项目的合适位置，如`{proj}`。建议将MiniTemplate改名为DataTables或者别的名字，MiniTemplates下的子目录建议保持原名。

## 修改luban.conf[​](https://www.datable.cn/docs/beginner/integratetoproject#%E4%BF%AE%E6%94%B9lubanconf "修改luban.conf的直接链接")

每个项目的Luban工具的位置不同，需要修改`gen.bat`（或gen.sh）命令中Luban.dll的路径。假设你将MiniTemplate目录复制到了你项目的`{DataTables}`目录。 打开`{DataTables}/gen.bat`，将`set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll`中`%WORKSPACE%\Tools\Luban\Luban.dll`替换成实际的Luban.dll的目录。 Luban.dll在Luban工具目录下。

此时运行 `{DataTables}/gen.bat`，确保可以正确运行。

## 新增客户端和服务器的gen.bat脚本[​](https://www.datable.cn/docs/beginner/integratetoproject#%E6%96%B0%E5%A2%9E%E5%AE%A2%E6%88%B7%E7%AB%AF%E5%92%8C%E6%9C%8D%E5%8A%A1%E5%99%A8%E7%9A%84genbat%E8%84%9A%E6%9C%AC "新增客户端和服务器的gen.bat脚本的直接链接")

在客户端的合适位置创建 gen_client.bat脚本，内容大致如下：

`set GEN_CLIENT={Luban.dll的路径}set CONF_ROOT={DataTables目录的路径}dotnet %GEN_CLIENT% ^    -t client ^    -c cs-simple-json ^    -d json ^    --conf %CONF_ROOT%\luban.conf ^    -x outputCodeDir={生成的代码的路径} ^    -x outputDataDir={生成的数据的路径}`

请根据实际所用的语言和导出数据格式替换 `-c cs-simple-json`和`-d json`。

危险

Luban生成时会删除outputCodeDir目录下的所有其他文件，请为它提供一个单独的目录，千万不要指向`Assets/Scripts`目录，它会删除掉其他代码文件！outputDataDir同理。

在服务器的合适位置创建 gen_server.bat脚本，内容大致如下：

`set GEN_CLIENT={Luban.dll的路径}set CONF_ROOT={DataTables目录的路径}dotnet %GEN_CLIENT% ^    -t server ^    -c cs-dotnet-json ^    -d json ^    --conf %CONF_ROOT%\luban.conf ^    -x outputCodeDir={生成的代码的路径} ^    -x outputDataDir={生成的数据的路径}`

请根据实际所用的语言和导出数据格式替换 `-c cs-dotnet-json`和`-d json`。

## 运行时加载配置[​](https://www.datable.cn/docs/beginner/integratetoproject#%E8%BF%90%E8%A1%8C%E6%97%B6%E5%8A%A0%E8%BD%BD%E9%85%8D%E7%BD%AE "运行时加载配置的直接链接")

请看下一节[运行时加载配置](https://www.datable.cn/docs/beginner/loadinruntime)。

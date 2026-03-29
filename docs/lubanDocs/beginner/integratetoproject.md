# 集成到项目
> Source: https://www.datable.cn/docs/beginner/integratetoproject

## 集成到项目

## 安装 dotnet SDK

按[快速上手](/docs/beginner/quickstart)要求，安装[dotnet sdk 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)。

## 下载[luban\_examples项目](https://gitee.com/focus-creative-games/luban_examples)

使用git clone或者下载zip的方式下载luban\_examples项目。

一般来说`luban_examples/Tools/Luban`目录已经包含了最新的Luban二进制代码。如果想自己编译，按以下步骤：

*   将[Luban](https://gitee.com/focus-creative-games/luban) Clone到luban\_examples同级目录（即luban和luban\_examples在同一个目录下），目录名必须为luban
*   运行 `luban_examples/Tools/build-luban.bat`

如果一切顺利，`luban_examples/Tools/Luban`目录会替换为Luban最新版本的二进制代码。

## 复制Luban工具到你的项目

大多数项目一般有一个专用目录放这些第三方的工具，比如说 `{proj}/Tools`，将`luban_examples/Tools/Luban`复制到任意合适的目录即可。

## 创建策划配置目录

将`luban_examples/MiniTemplate`复制到项目的合适位置，如`{proj}`。建议将MiniTemplate改名为DataTables或者别的名字，MiniTemplates下的子目录建议保持原名。

## 修改luban.conf

每个项目的Luban工具的位置不同，需要修改`gen.bat`（或gen.sh）命令中Luban.dll的路径。假设你将MiniTemplate目录复制到了你项目的`{DataTables}`目录。 打开`{DataTables}/gen.bat`，将`set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll`中`%WORKSPACE%\Tools\Luban\Luban.dll`替换成实际的Luban.dll的目录。 Luban.dll在Luban工具目录下。

此时运行 `{DataTables}/gen.bat`，确保可以正确运行。

## 新增客户端和服务器的gen.bat脚本

在客户端的合适位置创建 gen\_client.bat脚本，内容大致如下：

```batch
set GEN_CLIENT={Luban.dll的路径}
set CONF_ROOT={DataTables目录的路径}
dotnet %GEN_CLIENT% ^
    -t client ^
    -c cs-simple-json ^
    -d json ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputCodeDir={生成的代码的路径} ^
    -x outputDataDir={生成的数据的路径}
```

请根据实际所用的语言和导出数据格式替换 `-c cs-simple-json`和`-d json`。

> **危险**
> Luban生成时会删除outputCodeDir目录下的所有其他文件，请为它提供一个单独的目录，千万不要指向`Assets/Scripts`目录，它会删除掉其他代码文件！outputDataDir同理。

在服务器的合适位置创建 gen\_server.bat脚本，内容大致如下：

```batch
set GEN_CLIENT={Luban.dll的路径}
set CONF_ROOT={DataTables目录的路径}
dotnet %GEN_CLIENT% ^
    -t server ^
    -c cs-dotnet-json ^
    -d json ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputCodeDir={生成的代码的路径} ^
    -x outputDataDir={生成的数据的路径}
```

请根据实际所用的语言和导出数据格式替换 `-c cs-dotnet-json`和`-d json`。

## 运行时加载配置

请看下一节[运行时加载配置](/docs/beginner/loadinruntime)。

# 快速上手
> Source: https://www.datable.cn/docs/beginner/quickstart

## 快速上手

## 安装

1.  安装[dotnet sdk 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)或更高版本sdk
2.  下载[luban\_examples项目](https://gitee.com/focus-creative-games/luban_examples)。该项目中包含测试配置以及大量的示例项目。为方便起见，后续提及到的文件，默认都指这个项目中的文件

> **提示**
> `luban_examples/Tools/Luban`目录下的Luban可能不是最新版本。开发者既可以从[release](https://github.com/focus-creative-games/luban/releases)直接下载Luban的最新版本，也可以自己从Luban源码编译。

## 准备配置工程

直接使用luban\_examples项目中的MiniTemplate，后续操作在此基础上修改。

## 创建Reward表

在`MiniTemplate/Datas`目录下创建`reward.xlsx`文件，内容如下：

![reward](/assets/images/quickstart_reward-19dc3f1fbc3b8d0ae5863ea2a9451422.jpg)

简略介绍一下文件格式：

*   第1行是字段名行。单元格 A1 必须以##开头。表示这是一个有效数据表。
*   第2行是字段类型行。第1个单元格必须为 ##type。
*   第3行是分组行。`c`表示字段属于客户端，`s`表示属于属于服务器,`c,s`表示同时属于所有，留空也表示属于所有。
*   第4行是注释行。 以##开头。 可以有0-N个注释行，而且可以出现在任何位置
*   第5行起是数据行。

luban并没有限制标题头行的位置和数量。像`##xxx`之类的行可以任意调整顺序和位置，而`##group`和`##`行也是可选的。

推荐使用`xx_yy_zz`风格的字段名，因为luban会根据生成的语言，自动转化为 该语言推荐的命名风格的名称。你也可以手动指定你期望的命令风格。

在Datas目录下的`__tables__.xlsx`添加reward表声明，如下图：

![reward](/assets/images/quickstart_table-b88ebd57fde1248746a8b2eea85e0838.jpg)

至此完成reward表的创建工作！

## 生成配置数据

直接运行`MiniTemplate/gen.bat`（Win平台）或`MinTemplate/gen.sh`(MacOS或者Linux平台)。

如果运行成功，命令行界面会类似这样，以一个`bye~`结束。

![gen](/assets/images/gen-214c20fa1373df6e3d119c1544d1cc92.jpg)

在`MiniTemplate/output`目录下生成了json配置数据，可自行打开查看。

> Source: https://www.datable.cn/docs/manual/cascadingoption

Title: 层级参数机制 | Luban

URL Source: https://www.datable.cn/docs/manual/cascadingoption

Markdown Content:
# 层级参数机制 | Luban

[跳到主要内容](https://www.datable.cn/docs/manual/cascadingoption#__docusaurus_skipToContent_fallback)

[![Image 1: My Site Logo](https://www.datable.cn/img/logo.png) **Luban**](https://www.datable.cn/)[文档](https://www.datable.cn/docs/intro)

[GitHub](https://github.com/focus-creative-games/luban)

[4.x](https://www.datable.cn/docs/intro)
*   [4.x](https://www.datable.cn/docs/manual/cascadingoption)
*   [3.x](https://www.datable.cn/docs/3.x/manual/cascadingoption)
*   [1.x](https://www.datable.cn/docs/1.x/intro)

[中文](https://www.datable.cn/docs/manual/cascadingoption#)
*   [中文](https://www.datable.cn/docs/manual/cascadingoption)
*   [English](https://www.datable.cn/en/docs/manual/cascadingoption)

搜索 K

*   [介绍](https://www.datable.cn/docs/intro)
*   [新手教程](https://www.datable.cn/docs/beginner) 
    *   [快速上手](https://www.datable.cn/docs/beginner/quickstart)
    *   [生成代码和数据](https://www.datable.cn/docs/beginner/generatecodeanddata)
    *   [集成到项目](https://www.datable.cn/docs/beginner/integratetoproject)
    *   [运行时加载配置](https://www.datable.cn/docs/beginner/loadinruntime)
    *   [使用自定义类型](https://www.datable.cn/docs/beginner/usecustomtype)
    *   [使用容器类型](https://www.datable.cn/docs/beginner/usecollection)
    *   [使用列限定与紧凑格式](https://www.datable.cn/docs/beginner/streamandcolumnformat)
    *   [使用数据校验器](https://www.datable.cn/docs/beginner/usevalidator)
    *   [使用多态类型](https://www.datable.cn/docs/beginner/usepolymorphismtype)
    *   [自动导入table](https://www.datable.cn/docs/beginner/importtable)

*   [使用指南](https://www.datable.cn/docs/basic) 
    *   [与classic版本差异](https://www.datable.cn/docs/manual/migrate)
    *   [设计哲学](https://www.datable.cn/docs/manual/architecture)
    *   [特性](https://www.datable.cn/docs/manual/traits)
    *   [luban.conf](https://www.datable.cn/docs/manual/luban.conf)
    *   [schema 逻辑结构](https://www.datable.cn/docs/manual/schema)
    *   [配置定义](https://www.datable.cn/docs/manual/defaultschemacollector)
    *   [自动导入table](https://www.datable.cn/docs/manual/importtable)
    *   [命令行工具](https://www.datable.cn/docs/manual/commandtools)
    *   [层级参数机制](https://www.datable.cn/docs/manual/cascadingoption)
    *   [类型系统](https://www.datable.cn/docs/manual/types)
    *   [类型映射](https://www.datable.cn/docs/manual/typemapper)
    *   [excel格式（初级）](https://www.datable.cn/docs/manual/excel)
    *   [excel格式（高级）](https://www.datable.cn/docs/manual/exceladvanced)
    *   [Excel 紧凑格式](https://www.datable.cn/docs/manual/excelcompactformat)
    *   [非excel数据源](https://www.datable.cn/docs/manual/otherdatasource)
    *   [代码与数据生成](https://www.datable.cn/docs/manual/generatecodedata)
    *   [代码风格](https://www.datable.cn/docs/manual/codestyle)
    *   [加载配置](https://www.datable.cn/docs/manual/loadconfigatruntime)
    *   [数据校验器](https://www.datable.cn/docs/manual/validator)
    *   [自定义模板](https://www.datable.cn/docs/manual/template)
    *   [数据tag](https://www.datable.cn/docs/manual/tag)
    *   [字段变体（Variants）](https://www.datable.cn/docs/manual/variants)
    *   [本地化](https://www.datable.cn/docs/manual/l10n)
    *   [最佳实践](https://www.datable.cn/docs/manual/bestpractices)
    *   [扩展Luban实现](https://www.datable.cn/docs/manual/extendluban)

*   [FAQ](https://www.datable.cn/docs/help/faq)
*   [其他](https://www.datable.cn/docs/other) 

*   [](https://www.datable.cn/)
*   [使用指南](https://www.datable.cn/docs/basic)
*   层级参数机制

版本：4.x

本页总览

# 层级参数机制

Luban的大多数内置模板都使用了[层级参数(Cascading Option)](https://www.datable.cn/docs/manual/cascadingoption)机制，即逐级缩减模块名，直到查找到选项为止。

## 参数名规则[​](https://www.datable.cn/docs/manual/cascadingoption#%E5%8F%82%E6%95%B0%E5%90%8D%E8%A7%84%E5%88%99 "参数名规则的直接链接")

参数名支持命名空间，跟csharp代码的命名空间类似。以'a.b.c.key'参数为例，它的命名空间为'a.b.c'，基本名为'key'。

## 层级搜索规则[​](https://www.datable.cn/docs/manual/cascadingoption#%E5%B1%82%E7%BA%A7%E6%90%9C%E7%B4%A2%E8%A7%84%E5%88%99 "层级搜索规则的直接链接")

查找参数'{m1}.{m2}...{mk}.{n1}'，会先查找完整参数，如果再不到，再依次删减最下一层命名空间，直至找到为止。以'a.b.c.key'为例， 按照以下顺序查找选项值：

*   a.b.c.key
*   a.b.key
*   a.key
*   key

## 层级搜索规则的意义[​](https://www.datable.cn/docs/manual/cascadingoption#%E5%B1%82%E7%BA%A7%E6%90%9C%E7%B4%A2%E8%A7%84%E5%88%99%E7%9A%84%E6%84%8F%E4%B9%89 "层级搜索规则的意义的直接链接")

Luban的一部分参数支持多目标，如`--codeTarget`和`--dataTarget`，大多数情况下，命令行中只会包含1个这种目标，但有时候也有可能想 一次生成多个。如果只有一个公共的`outputCodeDir`和`outputDataDir`，生成将会相互覆盖。

层级参数较好地解决了这个问题。以code target为例，如果只有一个目标，简单使用`-x outputCodeDir=xxx`即可，更换目标时也不用修改选项key值。 如果有多个目标，如需要同时生成cs-bin和java-bin，则只需`-x cs-bin.outputCodeDir=cs_path`和`-x java-bin.outputCodeDir=java_path`， 即可为他们分别指定参数。

## 使用了层级参数的模块[​](https://www.datable.cn/docs/manual/cascadingoption#%E4%BD%BF%E7%94%A8%E4%BA%86%E5%B1%82%E7%BA%A7%E5%8F%82%E6%95%B0%E7%9A%84%E6%A8%A1%E5%9D%97 "使用了层级参数的模块的直接链接")

大多数可配置的模块都使用了层级参数的机制，如 code target、data target、output saver之类的模块。

[上一页 命令行工具](https://www.datable.cn/docs/manual/commandtools)[下一页 类型系统](https://www.datable.cn/docs/manual/types)

*   [参数名规则](https://www.datable.cn/docs/manual/cascadingoption#%E5%8F%82%E6%95%B0%E5%90%8D%E8%A7%84%E5%88%99)
*   [层级搜索规则](https://www.datable.cn/docs/manual/cascadingoption#%E5%B1%82%E7%BA%A7%E6%90%9C%E7%B4%A2%E8%A7%84%E5%88%99)
*   [层级搜索规则的意义](https://www.datable.cn/docs/manual/cascadingoption#%E5%B1%82%E7%BA%A7%E6%90%9C%E7%B4%A2%E8%A7%84%E5%88%99%E7%9A%84%E6%84%8F%E4%B9%89)
*   [使用了层级参数的模块](https://www.datable.cn/docs/manual/cascadingoption#%E4%BD%BF%E7%94%A8%E4%BA%86%E5%B1%82%E7%BA%A7%E5%8F%82%E6%95%B0%E7%9A%84%E6%A8%A1%E5%9D%97)

Docs

*   [文档](https://www.datable.cn/docs/intro)

Repository

*   [luban_examples](https://github.com/focus-creative-games/luban_examples)
*   [Excel2TextDiff](https://github.com/focus-creative-games/Excel2TextDiff)
*   [hybridclr](https://github.com/focus-creative-games/hybridclr)

More

*   [GitHub](https://github.com/focus-creative-games/luban)
*   [Gitee](https://gitee.com/focus-creative-games/luban)

Copyright © 2026 [Code Philosophy](https://code-philosophy.com/). All Rights Reserved.

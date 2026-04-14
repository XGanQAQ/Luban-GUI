> Source: https://www.datable.cn/docs/manual/extendluban

Title: 扩展Luban实现 | Luban

URL Source: https://www.datable.cn/docs/manual/extendluban

Markdown Content:
# 扩展Luban实现 | Luban

[跳到主要内容](https://www.datable.cn/docs/manual/extendluban#__docusaurus_skipToContent_fallback)

[![Image 1: My Site Logo](https://www.datable.cn/img/logo.png) **Luban**](https://www.datable.cn/)[文档](https://www.datable.cn/docs/intro)

[GitHub](https://github.com/focus-creative-games/luban)

[4.x](https://www.datable.cn/docs/intro)
*   [4.x](https://www.datable.cn/docs/manual/extendluban)
*   [3.x](https://www.datable.cn/docs/3.x/manual/extendluban)
*   [1.x](https://www.datable.cn/docs/1.x/intro)

[中文](https://www.datable.cn/docs/manual/extendluban#)
*   [中文](https://www.datable.cn/docs/manual/extendluban)
*   [English](https://www.datable.cn/en/docs/manual/extendluban)

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
*   扩展Luban实现

版本：4.x

本页总览

# 扩展Luban实现

在权衡灵活性和简便性后，luban没有使用插件机制，而是在源码工程中新增一些扩展项目来实现扩展。

## 创建扩展模块[​](https://www.datable.cn/docs/manual/extendluban#%E5%88%9B%E5%BB%BA%E6%89%A9%E5%B1%95%E6%A8%A1%E5%9D%97 "创建扩展模块的直接链接")

源码中除了`Luban.Core`和`Luban`以外的项目都是扩展项目，开发者可以参考它们给Luban添加扩展模块。 SimpleLauncher会自动搜索模块名中包含Luban的模块，因此**扩展模块名中最好都包含Luban**，否则需要 自己使用`SimpleLauncher.ScanResigerAssembly`注册自定义的扩展类。

以创建Luban.Demo模块为例，创建扩展模块的步骤如下：

*   创建项目 Luban.Demo
*   在Luban项目中引用Luban.Demo项目
*   Luban.Demo项目中新增对Luban.Core的引用
*   从Luban.CSharp项目中复制AssemblyInfo.cs到本目录

## 可扩展的部分[​](https://www.datable.cn/docs/manual/extendluban#%E5%8F%AF%E6%89%A9%E5%B1%95%E7%9A%84%E9%83%A8%E5%88%86 "可扩展的部分的直接链接")

*   Pipeline
*   Schema Collector
*   Data Loader
*   CodeTarget
*   DataTarget
*   DataValidator
*   CodeStyle
*   PostProcessor
*   OutputSaver
*   TextProvider

## 将Luban嵌入到其他C#工程中[​](https://www.datable.cn/docs/manual/extendluban#%E5%B0%86luban%E5%B5%8C%E5%85%A5%E5%88%B0%E5%85%B6%E4%BB%96c%E5%B7%A5%E7%A8%8B%E4%B8%AD "将Luban嵌入到其他C#工程中的直接链接")

有时候需要在其他工具中嵌入Luban，而不是直接使用Luban命令行工具。嵌入操作如下：

*   引用Luban.Core项目，强烈建议也引入那几个Luban.XXX.Builtin项目，因为它们包含了Luban所需要的核心默认实现
*   使用SimpleLauncher类初始化环境
*   使用DefaultPipeline或者自定义Pipeline运行生成管线

[上一页 最佳实践](https://www.datable.cn/docs/manual/bestpractices)[下一页 FAQ](https://www.datable.cn/docs/help/faq)

*   [创建扩展模块](https://www.datable.cn/docs/manual/extendluban#%E5%88%9B%E5%BB%BA%E6%89%A9%E5%B1%95%E6%A8%A1%E5%9D%97)
*   [可扩展的部分](https://www.datable.cn/docs/manual/extendluban#%E5%8F%AF%E6%89%A9%E5%B1%95%E7%9A%84%E9%83%A8%E5%88%86)
*   [将Luban嵌入到其他C#工程中](https://www.datable.cn/docs/manual/extendluban#%E5%B0%86luban%E5%B5%8C%E5%85%A5%E5%88%B0%E5%85%B6%E4%BB%96c%E5%B7%A5%E7%A8%8B%E4%B8%AD)

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

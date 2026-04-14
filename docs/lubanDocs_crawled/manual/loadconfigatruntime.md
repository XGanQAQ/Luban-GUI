> Source: https://www.datable.cn/docs/manual/loadconfigatruntime

Title: 加载配置 | Luban

URL Source: https://www.datable.cn/docs/manual/loadconfigatruntime

Markdown Content:
# 加载配置 | Luban

[跳到主要内容](https://www.datable.cn/docs/manual/loadconfigatruntime#__docusaurus_skipToContent_fallback)

[![Image 1: My Site Logo](https://www.datable.cn/img/logo.png) **Luban**](https://www.datable.cn/)[文档](https://www.datable.cn/docs/intro)

[GitHub](https://github.com/focus-creative-games/luban)

[4.x](https://www.datable.cn/docs/intro)
*   [4.x](https://www.datable.cn/docs/manual/loadconfigatruntime)
*   [3.x](https://www.datable.cn/docs/3.x/manual/loadconfigatruntime)
*   [1.x](https://www.datable.cn/docs/1.x/manual/loadconfigatruntime)

[中文](https://www.datable.cn/docs/manual/loadconfigatruntime#)
*   [中文](https://www.datable.cn/docs/manual/loadconfigatruntime)
*   [English](https://www.datable.cn/en/docs/manual/loadconfigatruntime)

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
*   加载配置

版本：4.x

本页总览

# 加载配置

## 安装Luban.Runtime[​](https://www.datable.cn/docs/manual/loadconfigatruntime#%E5%AE%89%E8%A3%85lubanruntime "安装Luban.Runtime的直接链接")

加载数据依赖一些Luban Runtime代码。对于Unity+C#，已经提供了`com.code-philosophy.luban`包。在Package Manager中安装com.code-philosophy.luban包，地址 `https://gitee.com/focus-creative-games/luban_unity.git`或`https://github.com/focus-creative-games/luban_unity.git`(或者从`https://github.com/focus-creative-games/luban_unity`下载)。对于其他语言请在 [示例项目](https://gitee.com/focus-creative-games/luban_examples/tree/main/Projects)中找到与你项目类型相符的项目，从该项目中复制Luban相关的Runtime代码。

## unity + c# + json[​](https://www.datable.cn/docs/manual/loadconfigatruntime#unity--c--json "unity + c# + json的直接链接")

先完成`安装Luban.Runtime`操作，然后使用如下代码加载配置。

`void Load()    {        // 一行代码可以加载所有配置。 cfg.Tables 包含所有表的一个实例字段。        var tables = new cfg.Tables(Loader);        // 访问一个单例表        Console.WriteLine(tables.TbGlobal.Name);        // 访问普通的 key-value 表        Console.WriteLine(tables.TbItem.Get(12).Name);        // 支持 operator []用法        Console.WriteLine(tables.TbMail[1001].Desc);    }    private static JSONNode LoadJson(string file)    {        return JSON.Parse(File.ReadAllText($"{your_json_dir}/{file}.json", System.Text.Encoding.UTF8));    }`

## unity项目中使用c#代码并自动判断加载bin或json配置[​](https://www.datable.cn/docs/manual/loadconfigatruntime#unity%E9%A1%B9%E7%9B%AE%E4%B8%AD%E4%BD%BF%E7%94%A8c%E4%BB%A3%E7%A0%81%E5%B9%B6%E8%87%AA%E5%8A%A8%E5%88%A4%E6%96%AD%E5%8A%A0%E8%BD%BDbin%E6%88%96json%E9%85%8D%E7%BD%AE "unity项目中使用c#代码并自动判断加载bin或json配置的直接链接")

先完成`安装Luban.Runtime`操作，然后使用如下代码加载配置。

开发期希望使用json导出格式，但在正式发布时为了节约导出文件大小以及提高加载性能，希望使用bin导出格式。通过反射创建cfg.Tables的方式，可以做到不改代码，自动适应这两种方式。

`void Start()    {        var tablesCtor = typeof(cfg.Tables).GetConstructors()[0];        var loaderReturnType = tablesCtor.GetParameters()[0].ParameterType.GetGenericArguments()[1];        // 根据cfg.Tables的构造函数的Loader的返回值类型决定使用json还是ByteBuf Loader        System.Delegate loader = loaderReturnType == typeof(ByteBuf) ?            new System.Func<string, ByteBuf>(LoadByteBuf)            : (System.Delegate)new System.Func<string, JSONNode>(LoadJson);        var tables = (cfg.Tables)tablesCtor.Invoke(new object[] {loader});        // 访问一个单例表        Console.WriteLine(tables.TbGlobal.Name);        // 访问普通的 key-value 表        Console.WriteLine(tables.TbItem.Get(12).Name);        // 支持 operator []用法        Console.WriteLine(tables.TbMail[1001].Desc);    }    private static JSONNode LoadJson(string file)    {        return JSON.Parse(File.ReadAllText($"{your_json_dir}/{file}.json", System.Text.Encoding.UTF8));    }    private static ByteBuf LoadByteBuf(string file)    {        return new ByteBuf(File.ReadAllBytes($"{your_json_dir}/{file}.bytes"));    }`

## 其他项目类型[​](https://www.datable.cn/docs/manual/loadconfigatruntime#%E5%85%B6%E4%BB%96%E9%A1%B9%E7%9B%AE%E7%B1%BB%E5%9E%8B "其他项目类型的直接链接")

请在[Projects](https://gitee.com/focus-creative-games/luban_examples/tree/main/Projects)中找到与你项目类型相符的示例项目，参考其加载 代码即可。

[上一页 代码风格](https://www.datable.cn/docs/manual/codestyle)[下一页 数据校验器](https://www.datable.cn/docs/manual/validator)

*   [安装Luban.Runtime](https://www.datable.cn/docs/manual/loadconfigatruntime#%E5%AE%89%E8%A3%85lubanruntime)
*   [unity + c# + json](https://www.datable.cn/docs/manual/loadconfigatruntime#unity--c--json)
*   [unity项目中使用c#代码并自动判断加载bin或json配置](https://www.datable.cn/docs/manual/loadconfigatruntime#unity%E9%A1%B9%E7%9B%AE%E4%B8%AD%E4%BD%BF%E7%94%A8c%E4%BB%A3%E7%A0%81%E5%B9%B6%E8%87%AA%E5%8A%A8%E5%88%A4%E6%96%AD%E5%8A%A0%E8%BD%BDbin%E6%88%96json%E9%85%8D%E7%BD%AE)
*   [其他项目类型](https://www.datable.cn/docs/manual/loadconfigatruntime#%E5%85%B6%E4%BB%96%E9%A1%B9%E7%9B%AE%E7%B1%BB%E5%9E%8B)

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

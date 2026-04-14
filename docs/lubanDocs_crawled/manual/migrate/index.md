> Source: https://www.datable.cn/docs/manual/migrate

Title: 与classic版本差异 | Luban

URL Source: https://www.datable.cn/docs/manual/migrate

Markdown Content:
# 与classic版本差异 | Luban

[跳到主要内容](https://www.datable.cn/docs/manual/migrate#__docusaurus_skipToContent_fallback)

[![Image 1: My Site Logo](https://www.datable.cn/img/logo.png) **Luban**](https://www.datable.cn/)[文档](https://www.datable.cn/docs/intro)

[GitHub](https://github.com/focus-creative-games/luban)

[4.x](https://www.datable.cn/docs/intro)
*   [4.x](https://www.datable.cn/docs/manual/migrate)
*   [3.x](https://www.datable.cn/docs/3.x/manual/migrate)
*   [1.x](https://www.datable.cn/docs/1.x/intro)

[中文](https://www.datable.cn/docs/manual/migrate#)
*   [中文](https://www.datable.cn/docs/manual/migrate)
*   [English](https://www.datable.cn/en/docs/manual/migrate)

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
*   与classic版本差异

版本：4.x

本页总览

# 与classic版本差异

当前版本相比于classic版本极大简化了代码实现，更方便定制。虽然代码调整极大，但使用差别不大。

当前版本的数据配置格式、生成的代码格式、生成的数据格式与classic版本基本相同，但在**本地化**方面实现差别较大。

## 移除了不必要的模块[​](https://www.datable.cn/docs/manual/migrate#%E7%A7%BB%E9%99%A4%E4%BA%86%E4%B8%8D%E5%BF%85%E8%A6%81%E7%9A%84%E6%A8%A1%E5%9D%97 "移除了不必要的模块的直接链接")

*   移除 Proto和DB生成，除去大量不必要的抽象
*   移除云生成，极大简化了代码

## excel格式调整[​](https://www.datable.cn/docs/manual/migrate#excel%E6%A0%BC%E5%BC%8F%E8%B0%83%E6%95%B4 "excel格式调整的直接链接")

*   excel A1单元以`##`开头则第一行会被当作注释行，而旧版本则当作是字段名定义行。

## 命令行参数调整[​](https://www.datable.cn/docs/manual/migrate#%E5%91%BD%E4%BB%A4%E8%A1%8C%E5%8F%82%E6%95%B0%E8%B0%83%E6%95%B4 "命令行参数调整的直接链接")

变化极大。为了方便定制，新版本支持自定义参数。

## 类型系统调整[​](https://www.datable.cn/docs/manual/migrate#%E7%B1%BB%E5%9E%8B%E7%B3%BB%E7%BB%9F%E8%B0%83%E6%95%B4 "类型系统调整的直接链接")

*   移除了vector2、vector3、vector4类型，改由开发者配合type mapper实现
*   text不再包含key和value两个字段，只包含key。旧版本中text是独立类型，新版本中text是`string#text=1`的语法糖

## 定义调整[​](https://www.datable.cn/docs/manual/migrate#%E5%AE%9A%E4%B9%89%E8%B0%83%E6%95%B4 "定义调整的直接链接")

*   enum、bean支持group参数
*   table的read_from_file属性调整为readSchemaFromFile，相应的excel格式中read_from_file改为read_schema_from_file
*   移除externaltype类型，改为typeMapper，并且直接在enum与bean的子元素中定义

## 支持真正意义的多代码或者数据target[​](https://www.datable.cn/docs/manual/migrate#%E6%94%AF%E6%8C%81%E7%9C%9F%E6%AD%A3%E6%84%8F%E4%B9%89%E7%9A%84%E5%A4%9A%E4%BB%A3%E7%A0%81%E6%88%96%E8%80%85%E6%95%B0%E6%8D%AEtarget "支持真正意义的多代码或者数据target的直接链接")

允许使用 `-c target1 -c target2 ...`或`-d target1 -d target2 ...` 一次生成多个代码和数据目标。因为新版本的层级参数机制， 使得可以为每个target指定输出目录。

## 移除了少量语言[​](https://www.datable.cn/docs/manual/migrate#%E7%A7%BB%E9%99%A4%E4%BA%86%E5%B0%91%E9%87%8F%E8%AF%AD%E8%A8%80 "移除了少量语言的直接链接")

不再内置提供erlang支持，由使用者自己实现。

## 更强大的管线及定制能力[​](https://www.datable.cn/docs/manual/migrate#%E6%9B%B4%E5%BC%BA%E5%A4%A7%E7%9A%84%E7%AE%A1%E7%BA%BF%E5%8F%8A%E5%AE%9A%E5%88%B6%E8%83%BD%E5%8A%9B "更强大的管线及定制能力的直接链接")

可以在不影响Luban原始代码的情况下对管线及几乎所有模块进行单独定制和调整。

[上一页 使用指南](https://www.datable.cn/docs/basic)[下一页 设计哲学](https://www.datable.cn/docs/manual/architecture)

*   [移除了不必要的模块](https://www.datable.cn/docs/manual/migrate#%E7%A7%BB%E9%99%A4%E4%BA%86%E4%B8%8D%E5%BF%85%E8%A6%81%E7%9A%84%E6%A8%A1%E5%9D%97)
*   [excel格式调整](https://www.datable.cn/docs/manual/migrate#excel%E6%A0%BC%E5%BC%8F%E8%B0%83%E6%95%B4)
*   [命令行参数调整](https://www.datable.cn/docs/manual/migrate#%E5%91%BD%E4%BB%A4%E8%A1%8C%E5%8F%82%E6%95%B0%E8%B0%83%E6%95%B4)
*   [类型系统调整](https://www.datable.cn/docs/manual/migrate#%E7%B1%BB%E5%9E%8B%E7%B3%BB%E7%BB%9F%E8%B0%83%E6%95%B4)
*   [定义调整](https://www.datable.cn/docs/manual/migrate#%E5%AE%9A%E4%B9%89%E8%B0%83%E6%95%B4)
*   [支持真正意义的多代码或者数据target](https://www.datable.cn/docs/manual/migrate#%E6%94%AF%E6%8C%81%E7%9C%9F%E6%AD%A3%E6%84%8F%E4%B9%89%E7%9A%84%E5%A4%9A%E4%BB%A3%E7%A0%81%E6%88%96%E8%80%85%E6%95%B0%E6%8D%AEtarget)
*   [移除了少量语言](https://www.datable.cn/docs/manual/migrate#%E7%A7%BB%E9%99%A4%E4%BA%86%E5%B0%91%E9%87%8F%E8%AF%AD%E8%A8%80)
*   [更强大的管线及定制能力](https://www.datable.cn/docs/manual/migrate#%E6%9B%B4%E5%BC%BA%E5%A4%A7%E7%9A%84%E7%AE%A1%E7%BA%BF%E5%8F%8A%E5%AE%9A%E5%88%B6%E8%83%BD%E5%8A%9B)

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

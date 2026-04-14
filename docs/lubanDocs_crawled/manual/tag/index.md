> Source: https://www.datable.cn/docs/manual/tag

Title: 数据tag | Luban

URL Source: https://www.datable.cn/docs/manual/tag

Markdown Content:
# 数据tag | Luban

[跳到主要内容](https://www.datable.cn/docs/manual/tag#__docusaurus_skipToContent_fallback)

[![Image 2: My Site Logo](https://www.datable.cn/img/logo.png) **Luban**](https://www.datable.cn/)[文档](https://www.datable.cn/docs/intro)

[GitHub](https://github.com/focus-creative-games/luban)

[4.x](https://www.datable.cn/docs/intro)
*   [4.x](https://www.datable.cn/docs/manual/tag)
*   [3.x](https://www.datable.cn/docs/3.x/manual/tag)
*   [1.x](https://www.datable.cn/docs/1.x/manual/tag)

[中文](https://www.datable.cn/docs/manual/tag#)
*   [中文](https://www.datable.cn/docs/manual/tag)
*   [English](https://www.datable.cn/en/docs/manual/tag)

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
*   数据tag

版本：4.x

本页总览

# 数据tag

luban支持记录级别的tag标记，每个数据可以有0到多个tag。 tag可用标识记录为注释，或者过滤导出，或者指示检验器不检查此记录。

## 格式介绍[​](https://www.datable.cn/docs/manual/tag#%E6%A0%BC%E5%BC%8F%E4%BB%8B%E7%BB%8D "格式介绍的直接链接")

不同文件格式下，记录tag的填写方式相似，也可以参考 [luban_examples/DataTables/Data/tag_datas](https://gitee.com/focus-creative-games/luban_examples/tree/main/DataTables/Datas/tag_datas)目录下的示例。

### excel格式[​](https://www.datable.cn/docs/manual/tag#excel%E6%A0%BC%E5%BC%8F "excel格式的直接链接")

在记录第一列填写tag。

![Image 3: tag](https://www.datable.cn/assets/images/tag2-97dd5819722449854c1016c031d28572.jpg)

### json格式[​](https://www.datable.cn/docs/manual/tag#json%E6%A0%BC%E5%BC%8F "json格式的直接链接")

`{  "__tag__": "dev",  "id":1,  "name":"xxx"}`

### lua格式[​](https://www.datable.cn/docs/manual/tag#lua%E6%A0%BC%E5%BC%8F "lua格式的直接链接")

`return {  __tag__ = "dev",  id = 1,  name = "xxx",}`

### xml格式[​](https://www.datable.cn/docs/manual/tag#xml%E6%A0%BC%E5%BC%8F "xml格式的直接链接")

`<data>  <__tag__>dev</__tag__>  <id>1</id>  <name>xxx</name></data>`

### yaml格式[​](https://www.datable.cn/docs/manual/tag#yaml%E6%A0%BC%E5%BC%8F "yaml格式的直接链接")

`__tag__ : devid : 1name : xxx`

## 特殊的tag名[​](https://www.datable.cn/docs/manual/tag#%E7%89%B9%E6%AE%8A%E7%9A%84tag%E5%90%8D "特殊的tag名的直接链接")

有一些特殊的tag名被用于特殊意义。

*   ##。 表示此记录被注释，永远不会导出
*   unchecked。 表示校验器不检查此记录

## 记录过滤导出[​](https://www.datable.cn/docs/manual/tag#%E8%AE%B0%E5%BD%95%E8%BF%87%E6%BB%A4%E5%AF%BC%E5%87%BA "记录过滤导出的直接链接")

有几种场合会用到过滤导出

*   有些记录仅用于内部测试，不希望对外正式发布时导出
*   有些记录希望测试和发布有不同版本
*   一些简单多版本管理，比如某个记录只在某个版本或者分支才导出

通过命令行参数 `--includeTag` 或 `--excludeTag` 来包含或者排除指定tag的数据，以下为使用示例。

| ##var | id | name |  |
| --- | --- | --- | --- |
| ##type | int | string |  |
| ## | id | desc1 | 注释 |
|  | 1 | item1 | 永远导出 |
| ## | 2 | item2 | 永远不导出 |
| test | 4 | item4 | --excludeTag test 时不导出 |
| TEST | 5 | item5 | --excludeTag test 时不导出 |
| dev | 6 | item6 | --excludeTag dev 时不导出 |
|  | 7 | item7 | 永远导出 |

[上一页 自定义模板](https://www.datable.cn/docs/manual/template)[下一页 字段变体（Variants）](https://www.datable.cn/docs/manual/variants)

*   [格式介绍](https://www.datable.cn/docs/manual/tag#%E6%A0%BC%E5%BC%8F%E4%BB%8B%E7%BB%8D)
    *   [excel格式](https://www.datable.cn/docs/manual/tag#excel%E6%A0%BC%E5%BC%8F)
    *   [json格式](https://www.datable.cn/docs/manual/tag#json%E6%A0%BC%E5%BC%8F)
    *   [lua格式](https://www.datable.cn/docs/manual/tag#lua%E6%A0%BC%E5%BC%8F)
    *   [xml格式](https://www.datable.cn/docs/manual/tag#xml%E6%A0%BC%E5%BC%8F)
    *   [yaml格式](https://www.datable.cn/docs/manual/tag#yaml%E6%A0%BC%E5%BC%8F)

*   [特殊的tag名](https://www.datable.cn/docs/manual/tag#%E7%89%B9%E6%AE%8A%E7%9A%84tag%E5%90%8D)
*   [记录过滤导出](https://www.datable.cn/docs/manual/tag#%E8%AE%B0%E5%BD%95%E8%BF%87%E6%BB%A4%E5%AF%BC%E5%87%BA)

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

> Source: https://www.datable.cn/docs/manual/generatecodedata

Title: 代码与数据生成 | Luban

URL Source: https://www.datable.cn/docs/manual/generatecodedata

Markdown Content:
# 代码与数据生成 | Luban

[跳到主要内容](https://www.datable.cn/docs/manual/generatecodedata#__docusaurus_skipToContent_fallback)

[![Image 1: My Site Logo](https://www.datable.cn/img/logo.png) **Luban**](https://www.datable.cn/)[文档](https://www.datable.cn/docs/intro)

[GitHub](https://github.com/focus-creative-games/luban)

[4.x](https://www.datable.cn/docs/intro)
*   [4.x](https://www.datable.cn/docs/manual/generatecodedata)
*   [3.x](https://www.datable.cn/docs/3.x/manual/generatecodedata)
*   [1.x](https://www.datable.cn/docs/1.x/manual/generatecodedata)

[中文](https://www.datable.cn/docs/manual/generatecodedata#)
*   [中文](https://www.datable.cn/docs/manual/generatecodedata)
*   [English](https://www.datable.cn/en/docs/manual/generatecodedata)

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
*   代码与数据生成

版本：4.x

本页总览

# 代码与数据生成

## 支持主流游戏引擎及平台[​](https://www.datable.cn/docs/manual/generatecodedata#%E6%94%AF%E6%8C%81%E4%B8%BB%E6%B5%81%E6%B8%B8%E6%88%8F%E5%BC%95%E6%93%8E%E5%8F%8A%E5%B9%B3%E5%8F%B0 "支持主流游戏引擎及平台的直接链接")

*   unity
*   unreal
*   cocos2d-x
*   godot
*   egret
*   微信小游戏平台
*   其他家支持js的小游戏平台

## 支持主流的热方案[​](https://www.datable.cn/docs/manual/generatecodedata#%E6%94%AF%E6%8C%81%E4%B8%BB%E6%B5%81%E7%9A%84%E7%83%AD%E6%96%B9%E6%A1%88 "支持主流的热方案的直接链接")

*   HybridCLR
*   {x,t,s...}lua
*   ILRuntime
*   puerts
*   其他

## 支持流行的游戏框架[​](https://www.datable.cn/docs/manual/generatecodedata#%E6%94%AF%E6%8C%81%E6%B5%81%E8%A1%8C%E7%9A%84%E6%B8%B8%E6%88%8F%E6%A1%86%E6%9E%B6 "支持流行的游戏框架的直接链接")

*   skynet
*   ET
*   GameFramework
*   QFramework
*   其他

## 支持主流的游戏开发语言[​](https://www.datable.cn/docs/manual/generatecodedata#%E6%94%AF%E6%8C%81%E4%B8%BB%E6%B5%81%E7%9A%84%E6%B8%B8%E6%88%8F%E5%BC%80%E5%8F%91%E8%AF%AD%E8%A8%80 "支持主流的游戏开发语言的直接链接")

*   c# (.net framework 4+. dotnet core 3+)
*   java (1.6+)
*   go (1.10+)
*   lua (5.1+)
*   typescript (3.0+)
*   python (3.0+)
*   c++ (11+)。
*   erlang (18+)。 classic 版本Luban支持，暂未迁移到当前版本。
*   rust (1.5+)。classic 版本Luban支持，暂未迁移到当前版本。
*   godot。 classic 版本Luban支持，暂未迁移到当前版本。

想自定义或者扩展支持新的语言非常容易。

## 支持的数据格式[​](https://www.datable.cn/docs/manual/generatecodedata#%E6%94%AF%E6%8C%81%E7%9A%84%E6%95%B0%E6%8D%AE%E6%A0%BC%E5%BC%8F "支持的数据格式的直接链接")

提示

同一种格式，为不同语言生成的数据是完全相同的

*   binary 格式。 格式紧凑，加载高效，但基本不具体可读性。推荐只用于正式发布。
*   bin-offset 格式。记录以bin格式导出的数据文件中每个记录的索引位置，可以用于以记录为粒度的lazy加载|
*   标准 json 格式
*   **protobuf** bin和json
*   flatbuffers json
*   **msgpack**
*   lua 
*   xml 
*   erlang
*   yaml

### bin-offset 格式[​](https://www.datable.cn/docs/manual/generatecodedata#bin-offset-%E6%A0%BC%E5%BC%8F "bin-offset 格式的直接链接")

有时候不想直接加载整个表，而是希望以记录为粒度，访问到哪个记录时再加载某个记录。bin-offset格式记录了bin格式下每个记录在 bin文件中的偏移，这样可以实现访问到某个记录时，如果未加载则直接从bin文件中读取相应偏移的数据。

bin-offset按记录顺序序列化每个record的key和offset信息。

| record_index 1 | record_index 2| ... | record_index n|

其中 record_index k的实现为序列化记录的所有key，再序列化记录的offset。

`buf.Write(key1);buf.Write(key2);...buf.Write(Key N);buf.WriteSize(offset);`

直接从bin-offset的源码能理解更清楚一些。

`// x 为 输出的bin-offset文件    private void WriteList(DefTable table, List<Record> datas, ByteBuf x)    {        // buf 对应输出的bin文件        ByteBuf buf = new ByteBuf();        buf.WriteSize(datas.Count);        foreach (var d in datas)        {            foreach (var indexInfo in table.IndexList)            {                DType keyData = d.Data.Fields[indexInfo.IndexFieldIdIndex];                // 序列化记录的每个key                keyData.Apply(BinaryDataVisitor.Ins, x);            }            // 序列化记录的offset            x.WriteSize(buf.Size);            d.Data.Apply(BinaryDataVisitor.Ins, buf);        }    }`

## 不同语言支持的格式如下：[​](https://www.datable.cn/docs/manual/generatecodedata#%E4%B8%8D%E5%90%8C%E8%AF%AD%E8%A8%80%E6%94%AF%E6%8C%81%E7%9A%84%E6%A0%BC%E5%BC%8F%E5%A6%82%E4%B8%8B "不同语言支持的格式如下：的直接链接")

同一个语言，需要为加载不同数据格式生成不同的代码。也就是code target与data target必须匹配。

| language | binary | json | lua |
| :--- | :---: | :---: | :---: |
| c# | ✔️ | ✔️ |  |
| java | ✔️ | ✔️ |  |
| go | ✔️ | ✔️ |  |
| lua | ✔️ |  | ✔️ |
| c++ | ✔️ |  |  |
| go | ✔️ | ✔️ |  |
| python |  | ✔️ |  |
| typescript | ✔️ | ✔️ |  |
| php |  | ✔️ |  |
| rust |  | ✔️ |  |
| godot |  | ✔️ |  |
| protobuf | ✔️ | ✔️ |  |

## 生成[​](https://www.datable.cn/docs/manual/generatecodedata#%E7%94%9F%E6%88%90 "生成的直接链接")

具体请见[命令行工具](https://www.datable.cn/docs/manual/commandtools)。

[上一页 非excel数据源](https://www.datable.cn/docs/manual/otherdatasource)[下一页 代码风格](https://www.datable.cn/docs/manual/codestyle)

*   [支持主流游戏引擎及平台](https://www.datable.cn/docs/manual/generatecodedata#%E6%94%AF%E6%8C%81%E4%B8%BB%E6%B5%81%E6%B8%B8%E6%88%8F%E5%BC%95%E6%93%8E%E5%8F%8A%E5%B9%B3%E5%8F%B0)
*   [支持主流的热方案](https://www.datable.cn/docs/manual/generatecodedata#%E6%94%AF%E6%8C%81%E4%B8%BB%E6%B5%81%E7%9A%84%E7%83%AD%E6%96%B9%E6%A1%88)
*   [支持流行的游戏框架](https://www.datable.cn/docs/manual/generatecodedata#%E6%94%AF%E6%8C%81%E6%B5%81%E8%A1%8C%E7%9A%84%E6%B8%B8%E6%88%8F%E6%A1%86%E6%9E%B6)
*   [支持主流的游戏开发语言](https://www.datable.cn/docs/manual/generatecodedata#%E6%94%AF%E6%8C%81%E4%B8%BB%E6%B5%81%E7%9A%84%E6%B8%B8%E6%88%8F%E5%BC%80%E5%8F%91%E8%AF%AD%E8%A8%80)
*   [支持的数据格式](https://www.datable.cn/docs/manual/generatecodedata#%E6%94%AF%E6%8C%81%E7%9A%84%E6%95%B0%E6%8D%AE%E6%A0%BC%E5%BC%8F)
    *   [bin-offset 格式](https://www.datable.cn/docs/manual/generatecodedata#bin-offset-%E6%A0%BC%E5%BC%8F)

*   [不同语言支持的格式如下：](https://www.datable.cn/docs/manual/generatecodedata#%E4%B8%8D%E5%90%8C%E8%AF%AD%E8%A8%80%E6%94%AF%E6%8C%81%E7%9A%84%E6%A0%BC%E5%BC%8F%E5%A6%82%E4%B8%8B)
*   [生成](https://www.datable.cn/docs/manual/generatecodedata#%E7%94%9F%E6%88%90)

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

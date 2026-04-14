> Source: https://www.datable.cn/docs/basic

Title: 使用指南 | Luban

URL Source: https://www.datable.cn/docs/basic

Published Time: Wed, 25 Mar 2026 12:09:02 GMT

Markdown Content:
[跳到主要内容](https://www.datable.cn/docs/basic#__docusaurus_skipToContent_fallback)

[![Image 1: My Site Logo](https://www.datable.cn/img/logo.png) **Luban**](https://www.datable.cn/)
[文档](https://www.datable.cn/docs/intro)

[GitHub](https://github.com/focus-creative-games/luban)

[4.x](https://www.datable.cn/docs/intro)

*   [4.x](https://www.datable.cn/docs/basic)
*   [3.x](https://www.datable.cn/docs/3.x/basic)
*   [1.x](https://www.datable.cn/docs/1.x/basic)

[中文](https://www.datable.cn/docs/basic#)
*   [中文](https://www.datable.cn/docs/basic)
*   [English](https://www.datable.cn/en/docs/basic)

*   [](https://www.datable.cn/)
*   使用指南

版本：4.x

## 使用指南

[## 📄️与classic版本差异 当前版本相比于classic版本极大简化了代码实现，更方便定制。虽然代码调整极大，但使用差别不大。](https://www.datable.cn/docs/manual/migrate)[## 📄️设计哲学 在大多数人的印象中，配置工具的实现应该是非常简单的。事实上，你确实可以用几百行代码就实现一个简单的配置工具，但它仅仅是能用，远远不足以满足实际游戏项目中配置工作流的各种要求。](https://www.datable.cn/docs/manual/architecture)[## 📄️特性 Luban内置了丰富的特性。](https://www.datable.cn/docs/manual/traits)[## 📄️luban.conf 定义了luban所需要的全局配置。](https://www.datable.cn/docs/manual/luban.conf)[## 📄️schema 逻辑结构 在设计哲学文档已经介绍了，Luban的核心为完备的类型系统，而DPP管线则是强大的扩展能力的基础。](https://www.datable.cn/docs/manual/schema)[## 📄️配置定义 Luban有一套独立于具体实现的Schema逻辑结构实现。对怎么定义配置没有要求，只要最终的定义能被](https://www.datable.cn/docs/manual/defaultschemacollector)[## 📄️自动导入table v3.0.0版本起支持自动导入table。](https://www.datable.cn/docs/manual/importtable)[## 📄️命令行工具 跨平台](https://www.datable.cn/docs/manual/commandtools)[## 📄️层级参数机制 Luban的大多数内置模板都使用了层级参数(Cascading Option)机制，即逐级缩减模块名，直到查找到选项为止。](https://www.datable.cn/docs/manual/cascadingoption)[## 📄️类型系统 Luban有完备的类型系统，尤其是bean支持类型继承和多态，使得Luban可以轻松表达任意复杂的数据结构。](https://www.datable.cn/docs/manual/types)[## 📄️类型映射 有时候你希望生成的代码中能直接使用现成的结构类型，而不是使用生成的类型代码。例如vector3是非常常见的类型，你在配置中定义了vector3后，可能希望生成的C#代码中涉及到](https://www.datable.cn/docs/manual/typemapper)[## 📄️excel格式（初级） 基础规则](https://www.datable.cn/docs/manual/excel)[## 📄️excel格式（高级） 示例中用到的结构](https://www.datable.cn/docs/manual/exceladvanced)[## 📄️Excel 紧凑格式 介绍](https://www.datable.cn/docs/manual/excelcompactformat)[## 📄️非excel数据源 并不是所有配置数据都以excel格式保存。实际项目中有一些比较复杂的配置通过编辑器生成，它们一般保存为json或者xml之类的格式。luban目前](https://www.datable.cn/docs/manual/otherdatasource)[## 📄️代码与数据生成 支持主流游戏引擎及平台](https://www.datable.cn/docs/manual/generatecodedata)[## 📄️代码风格 Luban默认为某个语言生成符合该语言推荐风格的代码，但有时候开发者想控制生成的代码风格，Luban](https://www.datable.cn/docs/manual/codestyle)[## 📄️加载配置 安装Luban.Runtime](https://www.datable.cn/docs/manual/loadconfigatruntime)[## 📄️数据校验器 Luban.DataValidtor.Builtin模块中实现多种常见的数据校验器。](https://www.datable.cn/docs/manual/validator)[## 📄️自定义模板 luban使用scriban 模板引擎来生成代码，也使用这个模板来生成自定义的文本型数据文件。](https://www.datable.cn/docs/manual/template)[## 📄️数据tag luban支持记录级别的tag标记，每个数据可以有0到多个tag。 tag可用标识记录为注释，或者过滤导出，或者指示检验器不检查此记录。](https://www.datable.cn/docs/manual/tag)[## 📄️字段变体（Variants） 有时候同一个字段可能有多个配置。一个非常常见的场景是制作本地化数据时，不同地区的某个初始道具有不同的值。](https://www.datable.cn/docs/manual/variants)[## 📄️本地化 支持多种本地化机制，它们可以同时使用。](https://www.datable.cn/docs/manual/l10n)[## 📄️最佳实践 命名约定](https://www.datable.cn/docs/manual/bestpractices)[在权衡灵活性和简便性后，luban没有使用插件机制，而是在源码工程中新增一些扩展项目来实现扩展。](https://www.datable.cn/docs/manual/extendluban)

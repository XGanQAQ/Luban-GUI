> Source: https://www.datable.cn/docs/beginner/usepolymorphismtype

Title: 使用多态类型 | Luban

URL Source: https://www.datable.cn/docs/beginner/usepolymorphismtype

Markdown Content:
# 使用多态类型 | Luban

[跳到主要内容](https://www.datable.cn/docs/beginner/usepolymorphismtype#__docusaurus_skipToContent_fallback)

[![Image 5: My Site Logo](https://www.datable.cn/img/logo.png) **Luban**](https://www.datable.cn/)[文档](https://www.datable.cn/docs/intro)

[GitHub](https://github.com/focus-creative-games/luban)

[4.x](https://www.datable.cn/docs/intro)
*   [4.x](https://www.datable.cn/docs/beginner/usepolymorphismtype)
*   [3.x](https://www.datable.cn/docs/3.x/beginner/usepolymorphismtype)
*   [1.x](https://www.datable.cn/docs/1.x/intro)

[中文](https://www.datable.cn/docs/beginner/usepolymorphismtype#)
*   [中文](https://www.datable.cn/docs/beginner/usepolymorphismtype)
*   [English](https://www.datable.cn/en/docs/beginner/usepolymorphismtype)

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
*   [FAQ](https://www.datable.cn/docs/help/faq)
*   [其他](https://www.datable.cn/docs/other) 

*   [](https://www.datable.cn/)
*   [新手教程](https://www.datable.cn/docs/beginner)
*   使用多态类型

版本：4.x

本页总览

# 使用多态类型

并不是所有数据结构都是规范一致的，在复杂的GamePlay玩法中，经常出现Buff效果有非常多种类型，每种类型的字段都不一样。 如果简单地取它们的并集，即取一个大结构包含所有类型的字段，配置将非常臃肿和复杂。多态类型完美解决了此问题。

## 定义[​](https://www.datable.cn/docs/beginner/usepolymorphismtype#%E5%AE%9A%E4%B9%89 "定义的直接链接")

我们以Shape为例，Shape有很多子类，如圆、三角形、矩形。定义如下：

![Image 6: item](https://www.datable.cn/assets/images/define_shape-95a8e5a8ee443a7826aa55570c2fc60f.jpg)

当一个类形有1个及以上子类时，称之为多态类型。每个子类型需要指定parent属性，如Circle的parent为Shape。

## 填写多态数据[​](https://www.datable.cn/docs/beginner/usepolymorphismtype#%E5%A1%AB%E5%86%99%E5%A4%9A%E6%80%81%E6%95%B0%E6%8D%AE "填写多态数据的直接链接")

多态结构如普通结构那样，支持流式与sep。填写多态数据时，第一个数据必须是多态类型，如Circle。填写多态类型时也支持别名，如`矩形`。

![Image 7: item](https://www.datable.cn/assets/images/use_shape1-8a284b753c692b370a01f29edf6b0b71.jpg)

支持列限定格式，此时需要用$type列来指定多态类型：

![Image 8: item](https://www.datable.cn/assets/images/use_shape2-1e1fd19702aa455d20677bc748b5474e.jpg)

支持多行数据：

![Image 9: item](https://www.datable.cn/assets/images/use_shape3-663cf51a766aa6fca89d2c05592dedc7.jpg)

[上一页 使用数据校验器](https://www.datable.cn/docs/beginner/usevalidator)[下一页 自动导入table](https://www.datable.cn/docs/beginner/importtable)

*   [定义](https://www.datable.cn/docs/beginner/usepolymorphismtype#%E5%AE%9A%E4%B9%89)
*   [填写多态数据](https://www.datable.cn/docs/beginner/usepolymorphismtype#%E5%A1%AB%E5%86%99%E5%A4%9A%E6%80%81%E6%95%B0%E6%8D%AE)

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

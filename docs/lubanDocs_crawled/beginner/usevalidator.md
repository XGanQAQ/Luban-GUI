> Source: https://www.datable.cn/docs/beginner/usevalidator

Title: 使用数据校验器 | Luban

URL Source: https://www.datable.cn/docs/beginner/usevalidator

Markdown Content:
## 使用数据校验器

实践中经常遇到某个字段需要校验合法性。例如item_id字段必须是有一个有效的item表的id。Luban支持完善的数据校验。

详细文档见[数据校验器](https://www.datable.cn/docs/manual/validator)。

## 常见的数据校验器[​](https://www.datable.cn/docs/beginner/usevalidator#%E5%B8%B8%E8%A7%81%E7%9A%84%E6%95%B0%E6%8D%AE%E6%A0%A1%E9%AA%8C%E5%99%A8 "常见的数据校验器的直接链接")

luban支持的数据校验器比较多，最常用有两种：

*   ref 引用校验
*   path 路径校验

## ref引用[​](https://www.datable.cn/docs/beginner/usevalidator#ref%E5%BC%95%E7%94%A8 "ref引用的直接链接")

格式： `{type}#ref={target}`。 target支持以下目标：

*   mode=map的表。此时target填表名，如 `ref=item.TbItem`
*   mode=list的表。要求此表至少有1个及以上主键。此时target必须指定是哪个主键，如`ref=index1@test.TbMultiKey`
*   mode=one的表。由于单例表只有一个记录，此时必须指定一个map类型的成员字段，如`ref=items@test.TbTestSingleton`

type可以出现在任意位置，但要求它必须是简单数据类型（即int、string、枚举之类）。例如以下皆合法：

*   `int#ref=item.TbItem` 要求这个int类型数据必须为有效的item.TbItem表id
*   `list,(int#ref=item.TbItem)` 要求list的每个元素都必须为有效的item.TbItem表id
*   `map,(int#ref=item.TbItem),(string#ref=test.TbString)` 要求map的每个key都必须为有效的item.TbItem表id，要求每个value都有有效的test.TbString表id

![Image 1: item](https://www.datable.cn/assets/images/use_ref-b29587a7cf3f397ba36f8770a55e13d0.jpg)

## path路径校验[​](https://www.datable.cn/docs/beginner/usevalidator#path%E8%B7%AF%E5%BE%84%E6%A0%A1%E9%AA%8C "path路径校验的直接链接")

自行查阅[数据校验器](https://www.datable.cn/docs/manual/validator)文档。

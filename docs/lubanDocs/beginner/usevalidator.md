# 使用数据校验器
> Source: https://www.datable.cn/docs/beginner/usevalidator

## 使用数据校验器

实践中经常遇到某个字段需要校验合法性。例如item\_id字段必须是有一个有效的item表的id。Luban支持完善的数据校验。

详细文档见[数据校验器](/docs/manual/validator)。

## 常见的数据校验器

luban支持的数据校验器比较多，最常用有两种：

*   ref 引用校验
*   path 路径校验

## ref引用

格式： `{type}#ref={target}`。 target支持以下目标：

*   mode=map的表。此时target填表名，如 `ref=item.TbItem`
*   mode=list的表。要求此表至少有1个及以上主键。此时target必须指定是哪个主键，如`ref=index1@test.TbMultiKey`
*   mode=one的表。由于单例表只有一个记录，此时必须指定一个map类型的成员字段，如`ref=items@test.TbTestSingleton`

type可以出现在任意位置，但要求它必须是简单数据类型（即int、string、枚举之类）。例如以下皆合法：

*   `int#ref=item.TbItem` 要求这个int类型数据必须为有效的item.TbItem表id
*   `list,(int#ref=item.TbItem)` 要求list的每个元素都必须为有效的item.TbItem表id
*   `map,(int#ref=item.TbItem),(string#ref=test.TbString)` 要求map的每个key都必须为有效的item.TbItem表id，要求每个value都有有效的test.TbString表id

![item](/assets/images/use_ref-b29587a7cf3f397ba36f8770a55e13d0.jpg)

## path路径校验

自行查阅[数据校验器](/docs/manual/validator)文档。

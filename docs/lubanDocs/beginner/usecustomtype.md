# 使用自定义类型
> Source: https://www.datable.cn/docs/beginner/usecustomtype

## 使用自定义类型

实践中经常遇到定义枚举和自定义结构的情况，Luban完美支持这个特性。

详细定义文档见[schema逻辑结构](/docs/manual/schema)，详细数据文档见[Excel格式（初级）](/docs/manual/excel)和[Excel格式（高级）](/docs/manual/exceladvanced)。

## 定义普通枚举

我们以定义Color枚举为例。

打开`__enums__.xlsx`（一般在Datas目录下），添加如下数据：

![color](/assets/images/define_color-9d8229fee2ca0814f9ddeee5188b8477.jpg)

字段解释详细见[enum逻辑结构](/docs/manual/schema#enum)。

建议flags=false、unique=true，即非标志枚举，枚举项唯一。

配置枚举类型的数据时，可以填枚举项名，枚举项别名，枚举项值，即RED、红、1这几个都对应RED这个枚举。

## 定义标志类型枚举

有的枚举类型的枚举项是标志位，使用时可以多个组合，我们以AccessFlag为例。

![color](/assets/images/define_accessflag-d3d17a9c9b6042223d0fa32a63d8f238.jpg)

flags=true表示这是标志类型枚举。

配置标志类型枚举时，即使可以像普通枚举类型那样只用一个枚举项，如`READ`；也可以用'|'来组合多个枚举项，如`READ|EXECUTE`、`写|执行`。

## 填写枚举数据

![enum data](/assets/images/enum-dee044226803effc6032313e7c4981e7.jpg)

## 定义普通结构类型

我们以常见的道具结构为例。打开`__beans__.xlsx`，添加以下数据：

![item](/assets/images/define_item-9f9e72ceb8af6dcbc0480a796fab3bd8.jpg)

## 填写结构类型数据

默认情况下，结构中每个字段都要占一个单元格，字段名需要占多个单元格，以表示这几个单元格内的数据对应结构的每个字段，此时需要合并单元格。

如果在使用csv这种不支持合并单元格的文件格式，可以使用`[{字段名}`和`{字段名}]`放在起始和结束列，表示这个字段占多列。

也可以使用sep在一个字段格内填写整个结构，`sep=,`表示使用','分割单元格数据，分割后的数据作为结构的字段。

当结构的字段很多时，连续填写字段容易出错，此时可以使用多级标题头格式，指定每个成员字段所在的列，文档详见[多级标题头](/docs/manual/exceladvanced#多级标题头)。

![item](/assets/images/use_item-513b175faefd91435357a469108d961c.jpg)

## 紧凑结构类型

如果某个结构总是以紧凑的方式在一个单元格内填写数据，可以直接结构定义时定义sep属性。我们以MyVec3为例：

![item](/assets/images/define_vec3-84c96a05193d45f68ef63cc0cc8fe5cb.jpg)

> **提示**
> `Defines/builtin.xml`中已经默认定义了vec2、vec3、vec4等常见的类型，不必再重新定义。

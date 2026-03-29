# 代码风格
> Source: https://www.datable.cn/docs/manual/codestyle

## 代码风格

Luban默认为某个语言生成符合该语言推荐风格的代码，但有时候开发者想控制生成的代码风格，Luban 对此有较完善的支持。

## 命名风格

Luban内建了以下几种命名风格：

| 风格名 | 描述 | 示例 |
|--------|------|------|
| none | 保持原样 | aa\_bb\_cc => aa\_bb\_cc |
| camel | 先按'\_'分割原始名，获得原子名列表，再使用Camel风格拼成最终名 | aa\_bb\_cc => aaBbCc |
| pascal | 先按'\_'分割原始名，获得原子名列表，再使用Pascal风格拼成最终名 | aa\_bb\_cc => AaBbCc |
| upper | 直接将原始名全大写 | aa\_bb\_cc => AA\_BB\_CC |
| snake | 下划线风格，等同于none风格 | aa\_bb\_cc => aa\_bb\_cc |

## 命名位置

Luban中可以控制以下几个位置的命名风格：

| 位置 | 描述 |
|------|------|
| namespace | 命名空间 |
| type | 类型，包括enum、bean、table及manager的类型名 |
| method | 函数，bean、table及manager中出现的函数名 |
| property | 属性，主要指bean、table中出现的属性名。不是所有语言都支持属性的概念，如c++ |
| field | 字段，bean中的字段名 |
| enumItem | 枚举项名，如AccessFlags枚举中的WRITE、READ |

## 代码风格

代码风格提供了一组命名风格的配置，指示了每个命名位置所用的命名风格。对于常见语言，Luban提供了默认的代码风格。

| 语言 | namespace | type | method | property | field | enumItem |
|------|-----------|------|--------|----------|-------|----------|
| none | none | none | none | none | none | none |
| c# | pascal | pascal | pascal | pascal | camel | none |
| java | pascal | pascal | pascal | camel | camel | none |
| go | snake | pascal | camel | camel | pascal | none |
| lua | snake | pascal | camel | camel | snake | none |
| typescript | pascal | pascal | camel | camel | camel | none |
| c++ | snake | pascal | pascal | pascal | camel | none |
| python | snake | pascal | snake | snake | snake | none |
| dart | pascal | pascal | camel | camel | camel | none |

如果不在这些语言列表，则取none代码风格，即使用原样名字。

## 相关命令行参数

具体参数介绍请见[命令行工具](/docs/manual/commandtools)。

| 参数 | 描述 |
|------|------|
| codeStyle | |
| namingConvention.{codeTarget}.{location} | 此参数为层级选项，如果不指定{codeTarget}则对所有code target生效 |

> Source: https://www.datable.cn/docs/manual/bestpractices

Title: 最佳实践 | Luban

URL Source: https://www.datable.cn/docs/manual/bestpractices

Markdown Content:
## 最佳实践

## 命名约定[​](https://www.datable.cn/docs/manual/bestpractices#%E5%91%BD%E5%90%8D%E7%BA%A6%E5%AE%9A "命名约定的直接链接")

*   table.name 推荐 TbXxxYyy 类风格，便于区别表与普通bean类型
*   bean.var.name 推荐 xx_yy_zz风格，生成时自动会根据目标语言，生成合适的变量名，如c#下为XxYyZz；java下为xxYyZz。

## 调整生成的代码的命名约定[​](https://www.datable.cn/docs/manual/bestpractices#%E8%B0%83%E6%95%B4%E7%94%9F%E6%88%90%E7%9A%84%E4%BB%A3%E7%A0%81%E7%9A%84%E5%91%BD%E5%90%8D%E7%BA%A6%E5%AE%9A "调整生成的代码的命名约定的直接链接")

默认是按照每个语言的推荐风格生成名称，例如 xxxx_yyyy在c#下是XxxxYyyy。如果你想调整命名风格，请参阅[代码风格](https://www.datable.cn/docs/manual/codestyle)文档。

## 灵活选择xml与excel定义[​](https://www.datable.cn/docs/manual/bestpractices#%E7%81%B5%E6%B4%BB%E9%80%89%E6%8B%A9xml%E4%B8%8Eexcel%E5%AE%9A%E4%B9%89 "灵活选择xml与excel定义的直接链接")

*   审美要求高的，习惯像protobuf那样手写表定义的，可以完全在xml里完成表定义
*   实用主义，方便策划使用或编辑，可以完全在excel中完成表定义
*   可以适当混用以上两者

如果使用xml定义，建议每个模块对应一个xml文件，并且有独立的模块名，便于管理和查找。

## 模块化[​](https://www.datable.cn/docs/manual/bestpractices#%E6%A8%A1%E5%9D%97%E5%8C%96 "模块化的直接链接")

强烈建议按模块管理配置，每个模块一个目录，将该模块的所有配置放到该目录下。

定义表与结构时，也推荐加上合适的模块名，如 item.TbItem, item.ItemInfo，而不是空module。

## 导出格式[​](https://www.datable.cn/docs/manual/bestpractices#%E5%AF%BC%E5%87%BA%E6%A0%BC%E5%BC%8F "导出格式的直接链接")

开发期推荐使用相应语言的json版本，这样不会因为配置格式变动而经常重新发布服务器或者客户端

## 优雅地在excel中配置复杂结构的数据[​](https://www.datable.cn/docs/manual/bestpractices#%E4%BC%98%E9%9B%85%E5%9C%B0%E5%9C%A8excel%E4%B8%AD%E9%85%8D%E7%BD%AE%E5%A4%8D%E6%9D%82%E7%BB%93%E6%9E%84%E7%9A%84%E6%95%B0%E6%8D%AE "优雅地在excel中配置复杂结构的数据的直接链接")

配合 多行记录 + 多级字段列名 + sep机制(字段sep，及type的sep机制)，灵活选择 列限定模式和流式模式， 简洁地配置出复杂数据。 有困难可以在群里咨询。

## 使用OOP类型继承来定义游戏中复杂的GamePlay数据[​](https://www.datable.cn/docs/manual/bestpractices#%E4%BD%BF%E7%94%A8oop%E7%B1%BB%E5%9E%8B%E7%BB%A7%E6%89%BF%E6%9D%A5%E5%AE%9A%E4%B9%89%E6%B8%B8%E6%88%8F%E4%B8%AD%E5%A4%8D%E6%9D%82%E7%9A%84gameplay%E6%95%B0%E6%8D%AE "使用OOP类型继承来定义游戏中复杂的GamePlay数据的直接链接")

灵活使用OOP类型继承来定义技能、BUFF、AI、副本等等复杂的GamePlay数据。视情况选择excel或json数据来填写 这些复杂数据。**千万不要**再用传统的 type + param1,param2,param3这种方式来组合表达复杂数据结构，对策划和程序不友好，而且难以检查错误。

## 使用githooks，在策划提交策划配置前检查数据合法性[​](https://www.datable.cn/docs/manual/bestpractices#%E4%BD%BF%E7%94%A8githooks%E5%9C%A8%E7%AD%96%E5%88%92%E6%8F%90%E4%BA%A4%E7%AD%96%E5%88%92%E9%85%8D%E7%BD%AE%E5%89%8D%E6%A3%80%E6%9F%A5%E6%95%B0%E6%8D%AE%E5%90%88%E6%B3%95%E6%80%A7 "使用githooks，在策划提交策划配置前检查数据合法性的直接链接")

参考 [githooks-demo](https://gitee.com/focus-creative-games/luban_examples/tree/main/githooks-demo)

## 策划检查配置脚本可以不指定codeTarget和dataTarget[​](https://www.datable.cn/docs/manual/bestpractices#%E7%AD%96%E5%88%92%E6%A3%80%E6%9F%A5%E9%85%8D%E7%BD%AE%E8%84%9A%E6%9C%AC%E5%8F%AF%E4%BB%A5%E4%B8%8D%E6%8C%87%E5%AE%9Acodetarget%E5%92%8Cdatatarget "策划检查配置脚本可以不指定codeTarget和dataTarget的直接链接")

由于策划往往只检查配置有效性而不想生成代码或者数据，可以不提供任何codeTarget和dataTarget。但如果没有任何dataTarget， 默认不会加载数据，也不会校验数据，此时可以通过`-f`参数强迫没有任何dataTarget的情况下也加载配置数据，类似如下：

`dotnet %LUBAN_DLL% ^    -t all ^    -f ^    --conf %CONF_ROOT%\luban.conf ^    ...`

## refgroup[​](https://www.datable.cn/docs/manual/bestpractices#refgroup "refgroup的直接链接")

如果很多字段都ref了相同一批表，可以使用refgroup方便引用。

## 编辑器生成的数据使用json数据格式[​](https://www.datable.cn/docs/manual/bestpractices#%E7%BC%96%E8%BE%91%E5%99%A8%E7%94%9F%E6%88%90%E7%9A%84%E6%95%B0%E6%8D%AE%E4%BD%BF%E7%94%A8json%E6%95%B0%E6%8D%AE%E6%A0%BC%E5%BC%8F "编辑器生成的数据使用json数据格式的直接链接")

编辑器生成的复杂配置数据建议以json数据保存，每个记录点一个文件，放到目录下。将table.input设置为该目录。 luban支持生成记录从json加载和保存的代码，不要自己手写这个序列化！

## 使用tag来标识测试和开发期数据[​](https://www.datable.cn/docs/manual/bestpractices#%E4%BD%BF%E7%94%A8tag%E6%9D%A5%E6%A0%87%E8%AF%86%E6%B5%8B%E8%AF%95%E5%92%8C%E5%BC%80%E5%8F%91%E6%9C%9F%E6%95%B0%E6%8D%AE "使用tag来标识测试和开发期数据的直接链接")

使用tag来标记那些测试和开发期数据，正式发布时使用 --output:exclude_tags tag1,tag2,... 来过滤这些数据， 不要自己去改它！

## 使用tag unchecked 来标识不校验记录[​](https://www.datable.cn/docs/manual/bestpractices#%E4%BD%BF%E7%94%A8tag-unchecked-%E6%9D%A5%E6%A0%87%E8%AF%86%E4%B8%8D%E6%A0%A1%E9%AA%8C%E8%AE%B0%E5%BD%95 "使用tag unchecked 来标识不校验记录的直接链接")

有些数据批量临时制作，很多引用值都不合法，但暂时未被程序使用，生成时因为ref失败而打印大量警告。可以为这些记录加上 unchecked 标签，luban就不会检查这些数据了。

## 使用datetime来表示时间[​](https://www.datable.cn/docs/manual/bestpractices#%E4%BD%BF%E7%94%A8datetime%E6%9D%A5%E8%A1%A8%E7%A4%BA%E6%97%B6%E9%97%B4 "使用datetime来表示时间的直接链接")

使用datetime来标识时间，注意配合时区参数使用。

## 多态类型使用场合[​](https://www.datable.cn/docs/manual/bestpractices#%E5%A4%9A%E6%80%81%E7%B1%BB%E5%9E%8B%E4%BD%BF%E7%94%A8%E5%9C%BA%E5%90%88 "多态类型使用场合的直接链接")

*   推荐用于 类型多变的场合，尤其是 GamePlay数据，比如技能、AI、任务、副本等等
*   简单的可以在excel配置，更复杂，尤其是技能这种需要独立技能编辑器中编辑的，推荐以json格式保存数据

## 代码中使用多态类型[​](https://www.datable.cn/docs/manual/bestpractices#%E4%BB%A3%E7%A0%81%E4%B8%AD%E4%BD%BF%E7%94%A8%E5%A4%9A%E6%80%81%E7%B1%BB%E5%9E%8B "代码中使用多态类型的直接链接")

假设是如下多态类型：

`public abstract class Shape : BeanBase{    // xxxx}public class Triangle : Shape{    float a;    float b;    float c;}public class Circle : Shape{    float radius;}public class Rectangle : Shape{    float width;    float height;}`

假设配置中 有个Shape字段shape。实际逻辑代码中要根据它的实际类型来不同处理。 有三种常见写法。当类型数量很少时，这三种方法都可以，按个人喜好选择。当类型数量较多时，推荐按照方法3的办法，更为高效。

### 方法1[​](https://www.datable.cn/docs/manual/bestpractices#%E6%96%B9%E6%B3%951 "方法1的直接链接")

`if (shape is Circle c)    {        // xxx    }    else if(shape is Triangle t)    {        // xxx    }    else if(shape is Rectangle r)    {        // xxx    }`

### 方法2[​](https://www.datable.cn/docs/manual/bestpractices#%E6%96%B9%E6%B3%952 "方法2的直接链接")

`switch(shape){    case Circle c:    {        // xxx;        break;    }    case Triangle t:    {        // xxx        break;    }    case Rectangle r:    {        // xxx;        break;    }}`

### 方法3[​](https://www.datable.cn/docs/manual/bestpractices#%E6%96%B9%E6%B3%953 "方法3的直接链接")

`switch(shape.GetTypeId()){    case Circle::__ID__:    {        Circle c = (Circle)shape;        // xxx;        break;    }    case Triangle::__ID__:    {        Triangle t = (Triangle)shape;        // xxx        break;    }    case Rectangle::__ID__:    {        Rectangle r = (Rectangle)shape;        // xxx;        break;    }}`

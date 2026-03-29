# 最佳实践
> Source: https://www.datable.cn/docs/manual/bestpractices

## 最佳实践

## 命名约定

*   table.name 推荐 TbXxxYyy 类风格，便于区别表与普通bean类型
*   bean.var.name 推荐 xx\_yy\_zz风格，生成时自动会根据目标语言，生成合适的变量名，如c#下为XxYyZz；java下为xxYyZz。

## 调整生成的代码的命名约定

默认是按照每个语言的推荐风格生成名称，例如 xxxx\_yyyy在c#下是XxxxYyyy。如果你想调整命名风格，请参阅[代码风格](/docs/manual/codestyle)文档。

## 灵活选择xml与excel定义

*   审美要求高的，习惯像protobuf那样手写表定义的，可以完全在xml里完成表定义
*   实用主义，方便策划使用或编辑，可以完全在excel中完成表定义
*   可以适当混用以上两者

如果使用xml定义，建议每个模块对应一个xml文件，并且有独立的模块名，便于管理和查找。

## 模块化

强烈建议按模块管理配置，每个模块一个目录，将该模块的所有配置放到该目录下。

定义表与结构时，也推荐加上合适的模块名，如 item.TbItem, item.ItemInfo，而不是空module。

## 导出格式

开发期推荐使用相应语言的json版本，这样不会因为配置格式变动而经常重新发布服务器或者客户端

## 优雅地在excel中配置复杂结构的数据

配合 多行记录 + 多级字段列名 + sep机制(字段sep，及type的sep机制)，灵活选择 列限定模式和流式模式， 简洁地配置出复杂数据。 有困难可以在群里咨询。

## 使用OOP类型继承来定义游戏中复杂的GamePlay数据

灵活使用OOP类型继承来定义技能、BUFF、AI、副本等等复杂的GamePlay数据。视情况选择excel或json数据来填写 这些复杂数据。**千万不要**再用传统的 type + param1,param2,param3这种方式来组合表达复杂数据结构，对策划和程序不友好，而且难以检查错误。

## 使用githooks，在策划提交策划配置前检查数据合法性

参考 [githooks-demo](https://gitee.com/focus-creative-games/luban_examples/tree/main/githooks-demo)

## 策划检查配置脚本可以不指定codeTarget和dataTarget

由于策划往往只检查配置有效性而不想生成代码或者数据，可以不提供任何codeTarget和dataTarget。但如果没有任何dataTarget， 默认不会加载数据，也不会校验数据，此时可以通过`-f` 参数强迫没有任何dataTarget的情况下也加载配置数据，类似如下：

```batch
dotnet %LUBAN_DLL% ^
    -t all ^
    -f ^
    --conf %CONF_ROOT%\luban.conf ^
    ...
```

## refgroup

如果很多字段都ref了相同一批表，可以使用refgroup方便引用。

## 编辑器生成的数据使用json数据格式

编辑器生成的复杂配置数据建议以json数据保存，每个记录点一个文件，放到目录下。将table.input设置为该目录。 luban支持生成记录从json加载和保存的代码，不要自己手写这个序列化！

## 使用tag来标识测试和开发期数据

使用tag来标记那些测试和开发期数据，正式发布时使用 --output:exclude\_tags tag1,tag2,... 来过滤这些数据， 不要自己去改它！

## 使用tag unchecked 来标识不校验记录

有些数据批量临时制作，很多引用值都不合法，但暂时未被程序使用，生成时因为ref失败而打印大量警告。可以为这些记录加上 unchecked 标签，luban就不会检查这些数据了。

## 使用datetime来表示时间

使用datetime来标识时间，注意配合时区参数使用。

## 多态类型使用场合

*   推荐用于 类型多变的场合，尤其是 GamePlay数据，比如技能、AI、任务、副本等等
*   简单的可以在excel配置，更复杂，尤其是技能这种需要独立技能编辑器中编辑的，推荐以json格式保存数据

## 代码中使用多态类型

假设是如下多态类型：

```csharp
public abstract class Shape : BeanBase
{
    // xxxx
}
public class Triangle : Shape
{
    float a;
    float b;
    float c;
}
public class Circle : Shape
{
    float radius;
}
public class Rectangle : Shape
{
    float width;
    float height;
}
```

假设配置中 有个Shape字段shape。实际逻辑代码中要根据它的实际类型来不同处理。 有三种常见写法。当类型数量很少时，这三种方法都可以，按个人喜好选择。当类型数量较多时，推荐按照方法3的办法，更为高效。

### 方法1

```csharp
    if (shape is Circle c)
    {
        // xxx
    }
    else if(shape is Triangle t)
    {
        // xxx
    }
    else if(shape is Rectangle r)
    {
        // xxx
    }
```

### 方法2

```csharp
switch(shape)
{
    case Circle c:
    {
        // xxx;
        break;
    }
    case Triangle t:
    {
        // xxx
        break;
    }
    case Rectangle r:
    {
        // xxx;
        break;
    }
}
```

### 方法3

```csharp
switch(shape.GetTypeId())
{
    case Circle::__ID__:
    {
        Circle c = (Circle)shape;
        // xxx;
        break;
    }
    case Triangle::__ID__:
    {
        Triangle t = (Triangle)shape;
        // xxx
        break;
    }
    case Rectangle::__ID__:
    {
        Rectangle r = (Rectangle)shape;
        // xxx;
        break;
    }
}
```

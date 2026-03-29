# 运行时加载配置
> Source: https://www.datable.cn/docs/beginner/loadinruntime

## 运行时加载配置

我们已经在[Projects](https://gitee.com/focus-creative-games/luban_examples/tree/main/Projects)下提供了大量示例项目。

我们以最常见的 Unity + c# + json 为例，示例项目为 [Csharp\_Unity\_Json](https://gitee.com/focus-creative-games/luban_examples/tree/main/Projects/Csharp_Unity_json)， 其他类型请参考 Projects目录下的相应项目。

## 安装 com.code-philosophy.luban

在Package Manager中安装com.code-philosophy.luban包，地址 `https://gitee.com/focus-creative-games/luban_unity.git`或`https://github.com/focus-creative-games/luban_unity.git`。

## 生成代码和配置

请根据[接入Luban到项目](/docs/beginner/integratetoproject)文档接入Luban，同时准备好相应的生成脚本gen.bat。如果有疑惑，可以参考 `Csharp_Unity_Json`项目的`gen.bat`文件。

运行该脚本，如果一切正常，会产生一系列日志，最终一行是 `bye~`。

> **危险**
> Luban生成时会删除outputCodeDir目录下的所有其他文件，请为它提供一个单独的目录，千万不要指向`Assets/Scripts`目录，它会删除掉其他代码文件！outputDataDir同理。

## 加载配置

只需一行代码即可加载所有配置表。整个游戏运行期间只加载一次（除非要运行中重新加载配置）。实践中在创建tables后将它保存起来，以便后续使用。

```csharp
string gameConfDir = "<outputDataDir>"; // 替换为gen.bat中outputDataDir指向的目录
var tables = new cfg.Tables(file => JSON.Parse(File.ReadAllText($"{gameConfDir}/{file}.json")));
```

> **提示**
> 默认生成的代码不支持异步加载，而在android之类的平台不能直接读取StreamingAssets目录，因此需要自己做一些特殊处理，比如先将所有配置数据文件加载到内存后再使用`new Tables`加载。
> 你可以通过修改[代码模板](/docs/manual/template)来支持异步加载。

## 使用配置

cfg.Tables 里包含所有配置表的一个实例字段。加载完 cfg.Tables 后，用 `tables.<表名>` 获得那个表实例，接着对该表做后续操作。 例如我们要打印Reward表id = 1001 的那个奖励信息，代码如下：

```csharp
cfg.demo.Reward reward = tables.TbReward.Get(1001);
Console.WriteLine("reward:{0}", reward);
```

你可能会注意到，reward表字段名 id,name,desc的首字母被大写了。这是因为工具会根据输出的语言，自动作相应代码风格的字段名转换，也即 boo\_bar 会被转换为 BooBar 这样的名字。 因此推荐配置中字段名时统一使用 xx\_yy\_zz 的风格。

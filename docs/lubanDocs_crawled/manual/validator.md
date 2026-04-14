> Source: https://www.datable.cn/docs/manual/validator

Title: 数据校验器 | Luban

URL Source: https://www.datable.cn/docs/manual/validator

Markdown Content:
## 数据校验器

Luban.DataValidtor.Builtin模块中实现多种常见的数据校验器。

## notDefaultValue 校验器[​](https://www.datable.cn/docs/manual/validator#notdefaultvalue-%E6%A0%A1%E9%AA%8C%E5%99%A8 "notDefaultValue 校验器的直接链接")

要求数据不能为默认值。例如int类型字段不能为0，int?类型字段不能null。

### 格式[​](https://www.datable.cn/docs/manual/validator#%E6%A0%BC%E5%BC%8F "格式的直接链接")

在类型名后面加上!字符，例如：

*   `type="int!"` 要求该字段为不能为0
*   `type=int?!` 要求该字段不能为null
*   `type="list,int!` 要求列表的元素值不能为0
*   `type="map,int!,string!` 要求map的key不能为0，value不能为长度为0的字符串

## ref 校验器[​](https://www.datable.cn/docs/manual/validator#ref-%E6%A0%A1%E9%AA%8C%E5%99%A8 "ref 校验器的直接链接")

检查某字段是否为某表的合法key，在游戏中非常常见，比如要求礼包表中item_id字段必须是合法的item.TbItem表的key。 ref可以用于任何可以当作key的数据类型上，也可以是容器的元素类型。

### 格式[​](https://www.datable.cn/docs/manual/validator#%E6%A0%BC%E5%BC%8F-1 "格式的直接链接")

根据被引用表的mode,引用格式略有区别

*   普通 map表。 ref="table{?}" 。 由于map表只有一个主键，因此只需写上被引用的**表全名**即可。
*   list表。 ref="key@table{?}"。 由于list表有多个主键，因此需要用 key@table来表明是被引用的表的哪个主键。
*   单例表。 ref="key@table{?}"。 由于单例表没有主键，因此需要用key@table来表明引用哪个字段。被引用字段必须为map类型，而且key类型必须匹配。

### 常规引用[​](https://www.datable.cn/docs/manual/validator#%E5%B8%B8%E8%A7%84%E5%BC%95%E7%94%A8 "常规引用的直接链接")

假设 item.TbItem 是道具表, key字段类型为int；假设 ui.TbWidget为组件表，key字段类型为string。

`<bean name="TestRef">   <var name="x1" type="int#ref=item.TbItem"/> x1必须是item.TbItem表的合法id   <var name="x2" type="list,int#ref=item.TbItem"/> x2列表中每个元素都必须是合法id   <var name="x2_2" type="list,(int#ref=item.TbItem)"/> 为了清晰，加上括号限定   <var name="x3" type="map,int#ref=item.TbItem,int"/> x3的key必须是 item.TbItem合法id   <var name="x4" type="map,int,int#ref=item.TbItem"/> x4的value必须是 item.TbItem合法id   <var name="x5" type="map,int#ref=item.TbItem,int#item.TbItem"/> x5的key和value都必须是合法id   <var name="x5" type="map,(int#ref=item.TbItem),(int#item.TbItem)"/> 为了清晰，加上括号      <var name="y1" type="string#ref=ui.TbWidget"/> y1 必须是 ui.TbWidget表的合法id   <var name="y2" tppe="list,string#ref=ui.TbWidget"/> y2列表中每个元素都必须是合法id   <var name="y3" type="map,string#ref=ui.TbWidget,int"/> y3 中 key必须是合法id   <var name="z1" type="map,(int#ref=item.TbItem),(string#ref=ui.TbWidget)"/> z1的key必须是item.TbItem合法id, value必须是ui.TbWidget合法id</bean>`

### 忽略空白值引用[​](https://www.datable.cn/docs/manual/validator#%E5%BF%BD%E7%95%A5%E7%A9%BA%E7%99%BD%E5%80%BC%E5%BC%95%E7%94%A8 "忽略空白值引用的直接链接")

有时候，需要字段值为0或者""时，忽略检查，使用 "ref=xxx?" 即可。如下：

`<bean name="TestEmptyRef">   <var name="x" type="int#ref=item.TbItem?"/> x 值不为0时，必须是合法id，为0时忽略检查</bean>`

### 可空变量的引用检查[​](https://www.datable.cn/docs/manual/validator#%E5%8F%AF%E7%A9%BA%E5%8F%98%E9%87%8F%E7%9A%84%E5%BC%95%E7%94%A8%E6%A3%80%E6%9F%A5 "可空变量的引用检查的直接链接")

对于可空变量，值为null时，忽略检查，如下：

`<bean name="TestNull">   <var name="x" type="int?#ref=item.TbItem"/>  当x!= null时，必须为合法id,为null忽略   <var name="y" type="int?#ref=item.TbItem?"/> 当x != null且x != 0时，必须为合法id,否则忽略</bean>`

### 多表引用[​](https://www.datable.cn/docs/manual/validator#%E5%A4%9A%E8%A1%A8%E5%BC%95%E7%94%A8 "多表引用的直接链接")

有时候，希望id是必须在几个表中某个表存在，ref支持多个表引用，写法如下：

`<bean name="TestMultiRef">   <var name="x" type="int#(ref=item.TbItem,item.TbEquip)"/>    <var name="y" type="int" ref="item.TbItem,item.TbEquip"/></bean>`

### 引用组[​](https://www.datable.cn/docs/manual/validator#%E5%BC%95%E7%94%A8%E7%BB%84 "引用组的直接链接")

如果很多字段都引用了相同的一组表。使用引用组会比较方便。

`<refgroup name="test_ref_group" ref="item.TbItem,item.TbEquip"/><bean name="TestMultiRef">   <var name="x" type="int#ref=test_ref_group"/>    <var name="x2" type="int#ref=test_ref_group?"/>    <var name="x3" type="int#ref=test_ref_group,hero.TbHero"/> </bean>`

### 代码生成中对ref的特殊处理[​](https://www.datable.cn/docs/manual/validator#%E4%BB%A3%E7%A0%81%E7%94%9F%E6%88%90%E4%B8%AD%E5%AF%B9ref%E7%9A%84%E7%89%B9%E6%AE%8A%E5%A4%84%E7%90%86 "代码生成中对ref的特殊处理的直接链接")

一些语言如 c#, typescript的生成代码中，如果某字段 xx 字段了ref，则为会它生成 xx_Ref 字段，类型为引用的表的记录类型， 在加载后会自动设置这个引用值，方便程序使用。注意，如果**ref了多个表**，则不会生成代码。

示例c#代码如下

`public int X1 { get; private set; }public test.TestBeRef X1_Ref { get; private set; }public int X12 { get; private set; }public int X2 { get; private set; }public int X3 { get; private set; }public int[] A1 { get; private set; }public test.TestBeRef[] A1_Ref { get; private set; }public int[] A2 { get; private set; }public test.TestBeRef[] A2_Ref { get; private set; }public System.Collections.Generic.List<int> B1 { get; private set; }public System.Collections.Generic.List<test.TestBeRef> B1_Ref { get; private set; }public System.Collections.Generic.List<int> B2 { get; private set; }public System.Collections.Generic.List<test.TestBeRef> B2_Ref { get; private set; }public System.Collections.Generic.HashSet<int> C1 { get; private set; }public System.Collections.Generic.HashSet<test.TestBeRef> C1_Ref { get; private set; }public System.Collections.Generic.HashSet<int> C2 { get; private set; }public System.Collections.Generic.HashSet<test.TestBeRef> C2_Ref { get; private set; }public System.Collections.Generic.Dictionary<int, int> D1 { get; private set; }public System.Collections.Generic.Dictionary<int, test.TestBeRef> D1_Ref { get; private set; }public System.Collections.Generic.Dictionary<int, int> D2 { get; private set; }public System.Collections.Generic.Dictionary<int, test.TestBeRef> D2_Ref { get; private set; }`

## path 校验器[​](https://www.datable.cn/docs/manual/validator#path-%E6%A0%A1%E9%AA%8C%E5%99%A8 "path 校验器的直接链接")

与ref的定义方法相似，但path只能作用于string类型数据。path校验器有几种子类型，参数有细微不同。path校验器的工作原理为根据字段值，产生一个路径， 然后再检查这个路径是否存在，因此使用path检验器要求命令行中使用 `-x pathValidator.rootDir=xxx` 指定检查路径的根目录。

提示

如果未设置`pathValidator.rootDir`选项，则会自动禁用path检查，同时打印警告日志

### normal 检验器[​](https://www.datable.cn/docs/manual/validator#normal-%E6%A3%80%E9%AA%8C%E5%99%A8 "normal 检验器的直接链接")

normal检验器需要参数，格式为 path=normal;<pattern>。 pattern中出现的*会被字段值替换，形成最终值，再检查validate_root_dir目录下是否存在相应文件。

`<bean name="TestPath">  <var name="x" type="string#path=normal;UI/*.text"/> 检查完整路径${pathValidator.rootDir}/UI/${x}.text 对应的资源是否存在  <var name="x2" type="list,string#path=normal;Prefabs/*.prefab"/> 检查x2的每个元素e ，${pathValidator.rootDir}/Prefabs/${e}.prefab 对应的资源是否存在</bean>`

### unity 校验器[​](https://www.datable.cn/docs/manual/validator#unity-%E6%A0%A1%E9%AA%8C%E5%99%A8 "unity 校验器的直接链接")

直接检查是否存在 `${pathValidator.rootDir}/{path}`文件。一个典型的使用场景是与与unity引擎的Addressable配合工作,将`pathValidator.rootDir`指向项目的根目录。

`<bean name="TestPath">  <var name="x" type="string#path=unity"/> 检查完整路径${pathValidator.rootDir}/{x} 对应的资源是否存在  <var name="x2" type="list,string#path=unity"/> 检查x2的每个元素e ，${pathValidator.rootDir}/{e} 对应的资源是否存在</bean>`

### ue 校验器[​](https://www.datable.cn/docs/manual/validator#ue-%E6%A0%A1%E9%AA%8C%E5%99%A8 "ue 校验器的直接链接")

检查是否有字段同名的资源，特别针对UE4的资源系统优化，`pathValidator.rootDir` 必须指向项目的Content目录。 与unity不同，unity中的资源值必须包含文件后缀，ue中不包含文件后缀，ue检验器，会自动检查${x}.uasset 或者 ${x}.umap 对应的资源是否存在。 如果资源值中还带有前缀如 "blueprint'/character/Mouse"，则会自动剔除 blueprint前缀后再去查找相应的资源。

`<bean name="TestPath">  <var name="x" type="string#path=ue"/> 检查 ${x} 对应的资源是否存在  <var name="x2" type="list,string#path=ue"/> 检查x2的每个元素e ，${e} 对应的资源是否存在</bean>`

### godot 校验器[​](https://www.datable.cn/docs/manual/validator#godot-%E6%A0%A1%E9%AA%8C%E5%99%A8 "godot 校验器的直接链接")

检查是否存在 `${pathValidator.rootDir}/{path.SubString(6)}`文件，即移除path路径的`res://`前缀后，检查剩余相对路径在rootDir下是否存在。

`<bean name="TestPath">  <var name="x" type="string#path=godot"/>  <var name="x2" type="list,string#path=godot"/></bean>`

## index 校验器[​](https://www.datable.cn/docs/manual/validator#index-%E6%A0%A1%E9%AA%8C%E5%99%A8 "index 校验器的直接链接")

对于`list,Bean`、`array,Bean`类型，有时候你希望按照Bean的某个字段唯一。`index`检验器满足这个需求。

`<bean name="Foo">  <var name="id" type="int"/>  <var name="name" type="string"/></bean><bean name="Bar">  <var name="foos" type="(list#index=id),Foo"/>要求列表内id字段唯一</bean>`

### 代码生成中对index的特殊处理[​](https://www.datable.cn/docs/manual/validator#%E4%BB%A3%E7%A0%81%E7%94%9F%E6%88%90%E4%B8%AD%E5%AF%B9index%E7%9A%84%E7%89%B9%E6%AE%8A%E5%A4%84%E7%90%86 "代码生成中对index的特殊处理的直接链接")

只用c#等少数语言的代码对index作了特殊处理，额外生成一个 map类型的 `xxx_{index}`，key为index字段的类型，value为 列表的元素类型。大致如下。

`List<Foo> Foos;  Dictionary<int, Foo> Foos_id;`

## range 校验器[​](https://www.datable.cn/docs/manual/validator#range-%E6%A0%A1%E9%AA%8C%E5%99%A8 "range 校验器的直接链接")

支持固定大小或者区间段的写法。 其中区间段支持开、闭区间，以及半开，半闭区间，示例如下

`<bean name="TestRange">  <var name="x0" type="int#range=10"/>  x0必须为10，不过实践中应该不会有人这么用  <var name="x1" type="int#range=[1,10]"/>  x1必须在 [1,10]之间  <var name="x2" type="int#range=(1,10)"/> 必须在 (1,10]之间，注意左开区间，不包含1  <var name="x3" type="int#range=[1,10)"/> 必须在 [1,10)之间，注意右开区间，不包含10  <var name="x4" type="int#range=(1,10]"/>  <var name="x5" type="int#range=[1,]"/> 必须 [1,无穷]  <var name="x6" type="int#range=[,100]"/> 必须 [-无穷,100] 之间  <var name="x7" type="int#range=(1,)"/> 必须 (1,无穷], 不包含1  <var name="x8" type="int#range=(,100)"/> 必须 (-无穷,100)， 不包含100</bean>`

## size 校验器[​](https://www.datable.cn/docs/manual/validator#size-%E6%A0%A1%E9%AA%8C%E5%99%A8 "size 校验器的直接链接")

只能作用于容器，用于检查容器元素个数是否符合要求。由于size作用器，作用于容器类型本身，定义时必须 (容器类型#size=xx),元素类型， 而不能 容器类型,元素类型#size=xxx，这会导致 size作用于元素数据上而出错！

size支持固定大小或者区间段的写法。

`<bean name="TestSize">  <var name="x" type="(list#size=4),int"/> 要求x元素个数必须为4  <var name="y" type="(map#size=[5,10]),int,int"/> 要求y的元素个数必须为5-10个</bean>`

## set 检验器[​](https://www.datable.cn/docs/manual/validator#set-%E6%A3%80%E9%AA%8C%E5%99%A8 "set 检验器的直接链接")

检查值是否在指定的集合内，目前支持int,long,string,enum类型，相应的容器类型也可以，如 list,int; map,int,string。

语法为 set=xx1{sep}xx2 ... 。 其中sep可以是,或者;。 推荐用分号';'，比较不容易出错。

`<bean name="TestSet">    <var name="id" type="int"/>    <var name="a1" type="int#(set=1,2,3)"/> 可以用','分割，但必须用()将set定义包围起来，不然'int#set=1'会被识别为容器，导致解析出错    <var name="a2" type="long#set=2;3"/>    <var name="a3" type="string#(set=ab,cd)"/>    <var name="a4" type="DemoEnum#set=B;C"/>    <var name="x1" type="list,int#set=1,2,3,4,5"/>    <var name="x2" type="list,long#set=2,3,4,5"/>    <var name="x3" type="list,string#set=ab,cd"/>    <var name="x4" type="list,DemoEnum#set=A,B"/>    <var name="x5" type="map,(int#set=1,2,3),(string#set=ab,cd)"/></bean>`

## 其他复杂的自定义校验[​](https://www.datable.cn/docs/manual/validator#%E5%85%B6%E4%BB%96%E5%A4%8D%E6%9D%82%E7%9A%84%E8%87%AA%E5%AE%9A%E4%B9%89%E6%A0%A1%E9%AA%8C "其他复杂的自定义校验的直接链接")

显然自带的校验器不可能支持所有校验需求，对于一些特殊的校验需求，使用单独的校验脚本会更适合。一个较优的办法是创建一个单独的校验项目， 进行必须的校验，如果有错误，则打印错误并且返回失败。使用C#的测试框架来构建测试工程是一个非常合适的解决办法，具体可以 参考 [CfgValidator](https://gitee.com/focus-creative-games/luban_examples/tree/main/Projects/CfgValidator)项目。

luban.conf
定义了luban所需要的全局配置。

格式
一个典型的luban.conf的配置内容如下。

{
    "groups":
    [
        {"names":["c"], "default":true},
        {"names":["s"], "default":true},
        {"names":["e"], "default":true}
    ],
    "schemaFiles":
    [
        {"fileName":"Defines", "type":""},
        {"fileName":"Datas/__tables__.xlsx", "type":"table"},
        {"fileName":"Datas/__beans__.xlsx", "type":"bean"},
        {"fileName":"Datas/__enums__.xlsx", "type":"enum"}
    ],
    "dataDir": "Datas",
    "targets":
    [
        {"name":"server", "manager":"Tables", "groups":["s"], "topModule":"cfg"},
        {"name":"client", "manager":"Tables", "groups":["c"], "topModule":"cfg"},
        {"name":"all", "manager":"Tables", "groups":["c","s","e"], "topModule":"cfg"}
    ]
}



dataDir
指定数据根目录，此项目配置不可为空。

schemaFiles
定义了需要收集的schema子定义文件，可以为多个，也可以为目录，此时会递归收集目录树（包含子目录）下所有子文件。

字段	类型	可空	描述
fileName	string	否	需要收集的schema子定义文件，可以为文件或目录，为目录时会递归收集目录树（包含子目录）下所有子文件，子文件的type值与目录相同
type	string	否	子定义文件类型。如果是xml格式定义文件，不需要type。如果为excel族，由于每个文件只适合包含一类定义，需要用type指定包含哪种类型的定义，有效值为 空白字符串、enum、bean、table
groups
groups定义了配置内可用的分组。实践中前后端需要的数据表和表内字段经常是不一样的，这种根据输出目标对输出内容的分类，对应luban内的概念为group。

字段	类型	可空	描述
names	list,string	否	分组名，包含1-n个值
default	bool	否	是否为table的默认导出目标，即如果table的group为空，自动属于此分组
group的name字段可以为任意值，但不要重复出现。取client这样的name也是可以的，但实践为了方便填写，一般是c、s这样的简单的单字符。

group的default字段对于enum、bean及field（bean的成员字段）不生效。

Luban的生成代码时，会从导出table出发，递归收集所有相关的enum及bean定义。 当enum和bean的group为空时，只有被导出表直接或者间接引用，才会标记它们为导出类型，为他们生成代码。如果手动设置了它们的group，不属于当前 导出目标，但它们被导出表间接引用时，会忽略group而强行导出。

如果field的group为空，则默认属于所有分组，不可配置。这是因为大多数field会属于所有分组，我们希望让field的 group都为空就能表示导出给所有目标，而不是啰嗦地设置它的分组为所有分组。

group有多个，一般项目至少包含c（即client）和s（即server）分组。如果项目有自定义配置编辑器，一般还会有一个额外的目标e（即editor）。 group的个数和命名没有限制，只不过约定俗成会这么设计。

targets
定义了导出目标。一般来说，对于客户端或服务器的导出，需要一些独有的配置信息，对应luban的概念为target。

字段	类型	可空	描述
name	string	否	导出目标名
manager	string	否	生成的管理所有导出Table的管理类的名称，一般取Tables
groups	list,string	否	该输出目标包含哪些分组，可以为1-n个
topModule	string	是	类型额外的顶层命名空间，可以为空。当你想让生成的所有配置代码都在cfg命名空间下，不需要给每个enum、bean、table之类的命名空间都加上cfg前缀，让topModule为cfg即可
# Luban Excel 数据文件格式规范

> 本文档基于 `LubanGui/Tmplate/Datas/#demo.item.xlsx` 实际模板文件及 `lubanSrc/Luban.DataLoader.Builtin/Excel/` 源码分析整理，供后续开发"新建 Excel"功能参考。

---

## 一、支持的文件格式

| 扩展名 | 说明 |
|--------|------|
| `.xlsx` | Excel 2007+ 格式（推荐） |
| `.xls` | 旧版 Excel 格式 |
| `.xlsm` | 含宏的 Excel 格式 |
| `.xlm` | 早期宏 Excel 格式 |
| `.csv` | CSV 文本格式（自动检测编码） |

底层使用 [ExcelDataReader](https://github.com/ExcelDataReader/ExcelDataReader) 库读取文件，CSV 使用 [Ude](https://github.com/errepi/ude) 进行编码检测。

---

## 二、标准 Sheet 结构

以模板文件 `#demo.item.xlsx` 为基准，一个标准数据表 Sheet 如下所示：

| A（列标识） | B | C | D | E |
|------------|---|---|---|---|
| `##var` | `id` | `name` | `desc` | `count` |
| `##type` | `int` | `string` | `string` | `int` |
| `##group` | *(空)* | `c,s` | `c` | *(空)* |
| `##` | `id` | `名称` | `描述` | `个数` |
| *(空)* | `1001` | `道具1` | `描述1` | `10` |
| *(空)* | `1002` | `道具2` | `描述2` | `100` |

**每行的含义：**

| A 列值 | 行类型 | 必填 | 说明 |
|--------|--------|------|------|
| `##var` | **字段名行** | ✅ 必须有且为第一行 | 同时作为 Meta 行和标题行，B 列起依次填写字段名 |
| `##type` | **类型行** | 可选 | B 列起对应字段的类型，供 `read_schema_from_file=true` 时读取 Schema |
| `##group` | **分组行** | 可选 | B 列起对应字段所属分组（`c`=客户端，`s`=服务端，`e`=编辑器），空=全分组 |
| `##` | **注释行** | 可选 | 可重复添加多行，人类可读的字段说明，不参与解析 |
| *(空)* | **数据行** | — | A 列为空或不以 `##` 开头即为数据行 |

> **核心规则**：`##var`（或 `##field`/`##+`）必须是 Sheet 的**第一行**，Luban 以此作为 Meta 行并同时解析字段名。若第一行不以 `##` 开头，整个 Sheet 被忽略。

---

## 三、字段名行（`##var` 行）详解

`##var` 行的 A0 是固定关键字，B 列起每个单元格为一个字段名。

### 3.1 字段名前缀修饰

| 语法 | 说明 |
|------|------|
| `fieldName` | 普通字段 |
| `*fieldName` | **多行模式**字段，容器类型（list/array/set/map）跨多个 Excel 行填写时使用 |
| `!fieldName` | **非空约束**，单元格为空时报错（等同 `non_empty=1`） |
| `[fieldName` … `fieldName]` | **多列区间**字段，起止列分别写 `[name` 和 `name]` |

### 3.2 字段名后缀标签（用 `#` 分隔）

```
fieldName#sep=;#non_empty=1
```

| 标签名 | 示例值 | 说明 |
|--------|--------|------|
| `sep` | `;` | 单元格内多值分隔符（如 `1;2;3` 表示一个 list） |
| `non_empty` | `1` | 字段不允许为空 |
| `multi_rows` | `1` 或 `true` | 启用多行模式（等同 `*` 前缀） |
| `default` | 任意字符串 | 单元格为空时的替换值 |
| `format` | `stream`/`json`/`lua`/`lite` | 单元格内容的解析格式（见第六节） |

> **忽略列**：字段名为空或以 `#` 开头时，该列被忽略，不参与数据解析。

---

## 四、层级标题（多级字段）

当字段本身是一个 Bean 或多列容器时，需要用层级标题来声明子字段。有两种方式：

### 4.1 合并单元格（推荐）

将父字段名所在单元格横向合并，覆盖子字段所在的列范围，然后再加一行 `##var` 定义子字段名。

`__enums__.xlsx` 中的示例（`*items` 字段合并了 H~L 列）：

| A | B | C | D | E | F | G | H~L（合并） |
|---|---|---|---|---|---|---|-------------|
| `##var` | `full_name` | `flags` | `unique` | `group` | `comment` | `tags` | `*items` |
| `##var` | *(空)* | *(空)* | *(空)* | *(空)* | *(空)* | *(空)* | `name` \| `alias` \| `value` \| `comment` \| `tags` |

`*items` 合并了多列，其子字段 `name`、`alias`、`value`、`comment`、`tags` 定义在第二行 `##var` 对应列。

### 4.2 不合并单元格的多级标题行

父字段列写字段名，在下一行 `##var` 中对应位置写子字段名（父字段名单元格留空）。  
如 `__beans__.xlsx`（`*fields` 字段跨 J~P 列）：

| A | … | J | K | L | M | N | O | P |
|---|---|---|---|---|---|---|---|---|
| `##var` | … | `*fields` | *(空)* | *(空)* | *(空)* | *(空)* | *(空)* | *(空)* |
| `##var` | … | `name` | `alias` | `type` | `group` | `comment` | `tags` | `variants` |

---

## 五、数据行规则

- **A 列为空**或 **A 列不以 `##` 开头** → 数据行
- **空行跳过**：一行内（从第一字段列到最后字段列）所有单元格均为空时，该行跳过
- **连续 300 行空行**会触发性能告警（建议不要在数据区保留大量空行）

### 5.1 数据行的行级标签

A 列可填写标签字符串（不以 `##` 开头），例如：

| A 列值 | 说明 |
|--------|------|
| *(空)* | 普通数据行 |
| `ignore` | 该行被忽略，不导入 |

---

## 六、特殊字段名

以下字段名具有固定语义，用于多态 Bean 和 Map 场景：

| 字段名 | 用途 |
|--------|------|
| `$type` | 多态 Bean 的子类型名；或可空 Bean 是否为空的标识 |
| `$value` | 多态 Bean 的行内字段值（与 `$type` 配合） |
| `$key` | 多行+列限定模式下 Map 的 key 列 |
| `__type__` | `$type` 的备用字段名（兼容旧格式） |

#### `$type` 的合法值

| 值 | 含义 |
|----|------|
| `null` | 该记录为 null（仅可空类型） |
| `{}` | 该记录非 null，使用声明类型 |
| 具体子类名 | 使用该子类填充字段 |

---

## 七、单元格内容格式（`format` 标签）

默认情况下单元格内容按"流式"规则读取，可通过 `format` 标签指定其他解析器：

| format 值 | 说明 |
|-----------|------|
| `stream`（默认） | 按列顺序依次读取单元格，Luban 原生流式格式 |
| `json` | 单元格内容解析为 JSON，如 `{"x":1,"y":2}` |
| `lua` | 单元格内容解析为 Lua 表，如 `{x=1,y=2}` |
| `lite` | Luban 自定义轻量级格式 |

---

## 八、数据布局方向

### 行方向（默认）

字段名在第一行，每条记录占一行：

```
A0 = ##var  →  数据向下排列
```

### 列方向（`##column` / `##vertical`）

字段名在第一列，每条记录占一列。A0 写 `##column`，Luban 内部自动**转置**后统一处理：

```
A0 = ##column  →  数据向右排列（列方向）
```

---

## 九、多行模式（`*fieldName`）

容器类型（list/array/set/map）的数据量较多时，可让同一条记录的容器字段跨越多个 Excel 行。字段名前加 `*` 即可启用：

```
A       B       C
##var   id      *skills
##type  int     int
(空)    1001    100
(空)    (空)    200
(空)    (空)    300
(空)    1002    50
```

- 行 2~4：id=1001 的记录，`skills` 有三个元素 `[100, 200, 300]`
- 行 5：id=1002 的新记录（`id` 字段为 `non_empty`，出现新值则开始新记录）

---

## 十、完整标准示例

以 `#demo.item.xlsx` 为准，新建一个道具表的最小合法格式如下：

```
列:   A         B      C       D       E
行0:  ##var     id     name    desc    count
行1:  ##type    int    string  string  int
行2:  ##group          c,s     c
行3:  ##        id     名称    描述    个数
行4:            1001   道具1   描述1   10
行5:            1002   道具2   描述2   100
```

对应 Schema（`read_schema_from_file=false` 时外部定义）：
```xml
<bean name="Item">
  <var name="id"    type="int"/>
  <var name="name"  type="string"/>
  <var name="desc"  type="string"/>
  <var name="count" type="int"/>
</bean>
```

---

## 十一、文件 URL 与 Sheet 指定

在 `__tables__.xlsx` 的 `input` 列中填写文件路径，支持以下写法：

| 写法 | 说明 |
|------|------|
| `item/items.xlsx` | 读取该文件内所有有效 Sheet |
| `Sheet1@item/items.xlsx` | 仅读取名为 `Sheet1` 的 Sheet |
| `*field@items.xlsx` | subAsset 以 `*` 开头时，整个文件作为多记录来源 |

---

## 十二、数据加载流程（代码路径）

```
DataLoaderManager.LoadTableFile()
  └── CreateDataLoader(ext)                       // 按扩展名选择 Loader
  └── ExcelRowColumnDataSource.Load()             // xlsx/xls/csv
       └── SheetLoadUtil.LoadRawSheets()
            └── TryParseMeta(reader)              // 读第一行，验证 ## 前缀，判断方向
            └── ParseRawSheetContent()            // 读取全部单元格（列方向则转置）
            └── ValidateTitles()                  // 校验 ## 标签合法性
            └── ParseTitle() → ParseSubTitles()   // 递归构建字段树 (Title)
            └── RemoveAll(IsNotDataRow)            // 过滤所有 ## 行，仅保留数据行
  └── RowColumnSheet.Load(rawSheet)               // 将数据行映射为 TitleRow 列表
  └── SheetDataCreator.Accept(type, sheet, row)   // 访问者模式，按字段类型逐列读取值
```

---

## 十三、新建 Excel 功能开发要点

生成一个合法的 Luban Excel 数据文件需满足：

1. **第一行（A0）必须是 `##var`**（或 `##field`/`##+`）。若第一行不以 `##` 开头，整个 Sheet 将被静默忽略。
2. **`##var` 行同时是字段名行**，B 列起按顺序填写字段名，无需额外再加一行标题。
3. `##type`、`##group`、`##` 等辅助行**可选**，顺序无严格要求，但均须在数据行之前。
4. **数据行 A 列留空**，填写数据从 B 列起对应字段。
5. 字段名不能以 `#` 开头（否则该列被忽略）。
6. 容器类型（list/array/set/map）若要在单格内表达，需在字段名后追加 `#sep=分隔符`。
7. 多态 Bean 必须包含 `$type` 列（以及可选的 `$value` 列）。
8. 建议第一个字段（通常是 `id`）保持 `non_empty`，Luban 会自动对首字段加此约束。
9. 不要在数据区留超过 300 行连续空行，会触发性能告警。

---

## 附录 A：Title 对象属性说明

| 属性 | 来源 | 说明 |
|------|------|------|
| `Name` | `##var` 行单元格文本 | 字段名（去除前缀后） |
| `FromIndex` | 列索引（0 起） | 字段起始列 |
| `ToIndex` | 列索引 | 字段结束列（单列 = FromIndex） |
| `Sep` | tag `sep=` | 单元格内多值分隔符 |
| `NonEmpty` | tag `non_empty=1` 或 `!` 前缀 | 不允许为空 |
| `SelfMultiRows` | tag `multi_rows=1` 或 `*` 前缀 | 该字段启用多行模式 |
| `HierarchyMultiRows` | 自身或任一子字段含 multi_rows | 整条记录需要多行处理 |
| `Default` | tag `default=value` | 单元格为空时的替换值 |
| `SubTitleList` | 合并单元格或多级 `##var` 行 | 子字段列表（按列索引排序） |

---

## 附录 B：Meta 行合法属性

A0 中 `##` 后可用 `#` 分隔追加以下属性：

| 属性 | 说明 |
|------|------|
| `var` / `field` / `+` | 声明本行同时为字段名行（标题行） |
| `column` / `vertical` | 切换为列方向（数据横向排列） |
| `type` | 声明本行为类型行 |
| `comment` | 声明本行为注释行 |
| `group` | 声明本行为分组行 |

> **不再支持 `&` 作为分隔符**（旧版），请统一使用 `#`。

> Source: https://www.datable.cn/docs/beginner/importtable

Title: 自动导入table | Luban

URL Source: https://www.datable.cn/docs/beginner/importtable

Markdown Content:
提示

v3.0.0版本起支持自动导入table。

每新增一个表都在**tables**.xlsx中添加一项，这个工作令人烦琐。大多数情况下，每个excel对应一个表，让工具自动添加表定义是可能的。

luban支持按照指定的规则扫描excel文件，自动导入对应的table。注意，并不会将该表的信息添加到`__tables__.xlsx`文件中。具体请查阅文档[自动导入table](https://www.datable.cn/docs/manual/importtable)。

## 创建自动导入的table[​](https://www.datable.cn/docs/beginner/importtable#%E5%88%9B%E5%BB%BA%E8%87%AA%E5%8A%A8%E5%AF%BC%E5%85%A5%E7%9A%84table "创建自动导入的table的直接链接")

危险

v4.4.1版本起才支持表注释写法`#<TableName>-<TableComment>`，更早版本只支持无表注释的写法`#<TableName>`。

将[快速上手](https://www.datable.cn/docs/beginner/quickstart)中创建的reward.xlsx复制为`#Reward2-奖励表.xlsx`文件，**不需要**修改 _ _ tables _ _.xlsx。重新生成后会发现新增了TbReward2表，表记录类型的Reward2。 表名后的`-xxxx`注释是可选的，如果存在，会自动被当作表注释。

对于常见情况，这种加表方式非常方便。

## 默认导入规则[​](https://www.datable.cn/docs/beginner/importtable#%E9%BB%98%E8%AE%A4%E5%AF%BC%E5%85%A5%E8%A7%84%E5%88%99 "默认导入规则的直接链接")

默认将扫描配置目录（luban.conf中dataDir字段）下（包含子目录）所有文件名以#开头的excel族（xls、xlsx、xlm、csv）文件，如 `#Item.xlsx`、`reward/#Reward.xlsx`。 以文件名除去开头的'#'字符及文件后缀后的字符串作为表的value_type，在value_type名上新增Tb作为表的full_name，如果excel文件在子目录下，则会将子目录作为命名空间。举例如下：

*   `#Item.xlsx` 生成 full_name为TbItem，value_type为Item，mode=map的表
*   `reward/#Reward.xlsx`生成full_name为reward.TbReward，value_type为reward.Reward，mode=map的表
*   `item/equip/#Equip.csv`生成full_name为item.equip.TbEquip，value_type为item.equip.Equip，mode=map的表
*   `#item.Item.xlsx`生成 full_name为 item.TbItem，value_type为item.Item，mode=map的表

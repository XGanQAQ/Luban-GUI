# LubanCommandBuilder

**所属层次**: 工具层 (Tool)

---

## 职责

将 `ProjectConfig` 对象转换为 Luban CLI 的命令行参数字符串，隔离参数拼接逻辑与执行逻辑。

---

## `ProjectConfig` → CLI 参数映射

| `ProjectConfig` 字段 | CLI 参数 | 说明 |
|----------------------|----------|------|
| `ConfFile` | `--conf <path>` | Luban 主配置文件路径 |
| `Target` | `--target <target>` | 生成目标（如 `cs_bin`） |
| `CodeTargets` | `--codeTarget <t1> --codeTarget <t2>` | 可多个 |
| `DataTargets` | `--dataTarget <t1>` | 可多个 |
| `ExtraArgs` | `--xargs key=value ...` | 透传的额外参数 |

---

## 示例输出

```
--conf "D:/proj/luban.conf"
--target cs_bin
--codeTarget cs-simple-json
--dataTarget json
```

---

## 层间约定

- 纯函数，无副作用，不依赖任何 Service 或 I/O。
- 输出结果直接传给 `LubanExecutor`。
- 路径中若包含空格，必须用双引号包裹。

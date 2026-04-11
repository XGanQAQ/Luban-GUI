# AppConfigManager

**所属层次**: 基础设施层 (Infrastructure)

---

## 职责

管理应用级全局配置（最近打开项目列表、主题偏好等），以 JSON 文件形式持久化到用户本地数据目录。

---

## 存储路径

```
%LOCALAPPDATA%\LubanGui\appConfig.json
```

---

## JSON 示例

```json
{
  "recentProjects": [
    {
      "name": "MyGame",
      "rootPath": "D:/projects/MyGame",
      "lastOpened": "2024-06-01T10:00:00Z"
    }
  ],
  "theme": "Light"
}
```

---

## 操作接口

| 方法 | 说明 |
|------|------|
| `LoadAsync()` | 读取配置文件，若不存在则返回默认配置 |
| `SaveAsync(config)` | 将当前配置序列化写入文件 |
| `AddProject(projectInfo)` | 添加项目到最近列表（去重 + 更新时间戳） |
| `RemoveProject(projectInfo)` | 从最近列表移除指定项目 |

---

## 层间约定

- 使用 `System.Text.Json` 进行序列化，不引入第三方 JSON 库。
- 读写均为异步，防止阻塞 UI 线程。
- 若配置文件损坏（JSON 解析异常），静默重置为默认配置，不抛出给上层。

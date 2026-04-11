# FileOpenService

**所属层次**: 工具层 (Tool)

---

## 职责

以平台原生方式在外部程序中打开文件，支持打开文件（资源管理器定位）和打开 Excel 文件并定位到指定单元格。

---

## 接口定义

```csharp
public interface IFileOpenService
{
    void OpenFile(string filePath);
    void OpenFileAtCell(string filePath, int row, int col);
}
```

---

## 平台兼容性

| 平台 | `OpenFile` | `OpenFileAtCell` |
|------|-----------|-----------------|
| Windows | `explorer.exe /select,<path>` | 通过 COM Automation 定位 Excel 单元格 |
| macOS | `open -R <path>` | AppleScript 定位 |
| Linux | `xdg-open <dir>` | 仅打开目录，不定位单元格 |

当前版本仅保证 **Windows** 上的完整功能；macOS / Linux 降级为打开文件所在目录。

---

## 层间约定

- 只依赖 `System.Diagnostics.Process`，不引用任何 Service 接口。
- `OpenFileAtCell` 在 Excel 未安装时，降级为 `OpenFile`（静默处理异常）。
- 调用方无需关心平台差异，统一通过 `IFileOpenService` 调用。

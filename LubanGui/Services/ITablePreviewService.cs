using System.Threading.Tasks;
using LubanGui.Models;

namespace LubanGui.Services;

/// <summary>
/// 负责读取 xlsx 文件并提供数据预览。
/// </summary>
public interface ITablePreviewService
{
    /// <summary>
    /// 读取 xlsx 文件，跳过 ## 开头的元数据行，返回预览数据（列名 + 数据行）。
    /// </summary>
    Task<TablePreviewData> LoadPreviewAsync(string xlsxAbsPath);

    /// <summary>
    /// 返回指定字段在 xlsx 中的列号（1-based），用于双击定位。
    /// 若字段不存在则返回 -1。
    /// </summary>
    int GetFieldColumnIndex(string xlsxAbsPath, string fieldName);
}

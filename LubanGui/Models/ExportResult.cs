using System;

namespace LubanGui.Models;

/// <summary>
/// 一次导表操作的结果。
/// </summary>
public class ExportResult
{
    /// <summary>是否成功导表。</summary>
    public bool Success { get; set; }

    /// <summary>如果失败，记录错误信息。</summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>完整的标准输出（stdout）。</summary>
    public string StandardOutput { get; set; } = string.Empty;

    /// <summary>完整的标准错误（stderr）。</summary>
    public string StandardError { get; set; } = string.Empty;

    /// <summary>Luban 进程的退出码。0 = 成功，其他 = 失败。</summary>
    public int ExitCode { get; set; }

    /// <summary>导表耗时。</summary>
    public TimeSpan Duration { get; set; }
}

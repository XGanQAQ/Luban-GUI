using System;

namespace LubanGui.Models;

public enum LogEntryLevel
{
    Info,
    Success,
    Warning,
    Error,
    Output,
    ProcessError,
}

public class LogEntry
{
    public LogEntryLevel Level { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;

    public string Prefix => Level switch
    {
        LogEntryLevel.Success => "[SUCCESS]",
        LogEntryLevel.Warning => "[WARN]",
        LogEntryLevel.Error => "[ERROR]",
        LogEntryLevel.Output => "[OUT]",
        LogEntryLevel.ProcessError => "[ERR]",
        _ => "[INFO]",
    };

    public string FormattedMessage => $"{Timestamp:HH:mm:ss} {Prefix} {Message}";
}

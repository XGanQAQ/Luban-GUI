using System;
using System.Diagnostics;
using System.IO;

namespace LubanGui.Infrastructure;

/// <summary>
/// 负责打开文件或在文件管理器中定位文件。
/// </summary>
public class FileOpenService
{
    /// <summary>
    /// 用系统默认程序打开指定文件。
    /// </summary>
    public void OpenFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"文件不存在：{filePath}");
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = filePath,
            UseShellExecute = true,
        });
    }

    /// <summary>
    /// 在 Windows 资源管理器中打开文件所在目录，并选中该文件。
    /// </summary>
    public void OpenInExplorer(string filePath)
    {
        if (!File.Exists(filePath) && !Directory.Exists(filePath))
        {
            throw new FileNotFoundException($"路径不存在：{filePath}");
        }

        if (File.Exists(filePath))
        {
            // 在资源管理器中选中文件
            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = $"/select,\"{filePath}\"",
                UseShellExecute = true,
            });
        }
        else
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true,
            });
        }
    }

    /// <summary>
    /// 打开文件夹。
    /// </summary>
    public void OpenDirectory(string dirPath)
    {
        if (!Directory.Exists(dirPath))
        {
            throw new DirectoryNotFoundException($"目录不存在：{dirPath}");
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = dirPath,
            UseShellExecute = true,
        });
    }
}

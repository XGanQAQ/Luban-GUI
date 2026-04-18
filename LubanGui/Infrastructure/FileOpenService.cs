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

    /// <summary>
    /// 用 Excel COM 自动化打开指定文件，并将光标定位到指定单元格（行、列均为 1-based）。
    /// 若 Excel 未安装、COM 操作失败或当前平台非 Windows，自动降级为 <see cref="OpenFile"/>。
    /// </summary>
    /// <param name="filePath">Excel 文件绝对路径。</param>
    /// <param name="row">目标行（1-based）。</param>
    /// <param name="col">目标列（1-based，1=A, 2=B …）。</param>
    public void OpenFileAtCell(string filePath, int row, int col)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"文件不存在：{filePath}");
        }

        if (!OperatingSystem.IsWindows())
        {
            OpenFile(filePath);
            return;
        }

        try
        {
            var excelType = Type.GetTypeFromProgID("Excel.Application");
            if (excelType == null)
            {
                OpenFile(filePath);
                return;
            }

            dynamic excel = Activator.CreateInstance(excelType)!;
            try
            {
                excel.Visible = false;
                dynamic workbook = excel.Workbooks.Open(filePath);
                dynamic sheet = workbook.Worksheets[1];
                dynamic cell = sheet.Cells[row, col];
                excel.Visible = true;
                excel.Goto(cell, true);
            }
            catch
            {
                // 导航失败时至少保证 Excel 可见
                try { excel.Visible = true; } catch { /* ignore */ }
            }
        }
        catch
        {
            // COM 自动化失败，退化为直接打开文件
            OpenFile(filePath);
        }
    }
}

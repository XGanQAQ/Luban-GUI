using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using LubanGui.Models;

namespace LubanGui.Converters;

/// <summary>
/// 将 <see cref="ExportStatus"/> 枚举值转换为对应的 UI 颜色画笔。
/// </summary>
public class ExportStatusToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ExportStatus status)
        {
            return status switch
            {
                ExportStatus.Exporting => new SolidColorBrush(Color.FromRgb(33, 150, 243)),   // #2196F3 蓝色
                ExportStatus.Success   => new SolidColorBrush(Color.FromRgb(76, 175, 80)),    // #4CAF50 绿色
                ExportStatus.Failed    => new SolidColorBrush(Color.FromRgb(244, 67, 54)),    // #F44336 红色
                ExportStatus.Cancelled => new SolidColorBrush(Color.FromRgb(255, 152, 0)),    // #FF9800 橙色
                _                      => new SolidColorBrush(Color.FromRgb(158, 158, 158)),  // #9E9E9E 灰色（Idle）
            };
        }

        return new SolidColorBrush(Colors.Gray);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

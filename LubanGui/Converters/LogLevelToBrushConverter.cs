using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using LubanGui.Models;

namespace LubanGui.Converters;

public class LogLevelToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is LogEntryLevel level)
        {
            return level switch
            {
                LogEntryLevel.Success => new SolidColorBrush(Color.FromRgb(144, 238, 144)),   // LightGreen
                LogEntryLevel.Warning => new SolidColorBrush(Color.FromRgb(255, 165, 0)),     // Orange
                LogEntryLevel.Error => new SolidColorBrush(Color.FromRgb(255, 99, 71)),       // Tomato
                LogEntryLevel.ProcessError => new SolidColorBrush(Color.FromRgb(255, 99, 71)),
                LogEntryLevel.Output => new SolidColorBrush(Color.FromRgb(220, 220, 220)),    // Light gray
                _ => new SolidColorBrush(Color.FromRgb(160, 160, 160)),                       // Info = gray
            };
        }

        return new SolidColorBrush(Colors.White);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

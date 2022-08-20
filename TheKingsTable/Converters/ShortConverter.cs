using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace TheKingsTable.Converters
{
    internal class ShortConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new NotSupportedException();

            var val = "";
            if (value is short s)
                val = s.ToString();
            return val;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

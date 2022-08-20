using System;
using System.Collections.Generic;
using CaveStoryModdingFramework.Stages;
using Avalonia.Data.Converters;
using System.Globalization;
using Avalonia.Controls;

namespace TheKingsTable.Converters
{
    class StageEntryListConverter : IMultiValueConverter
    {
        public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new NotSupportedException();
            
            var val = "";
            if (values[0] is StageTableEntry entry)
            {    
                if(values[1] is IList<StageTableEntry> ic)
                {
                    var index = ic.IndexOf(entry);
                    if (index != -1)
                        val += $"{index} - ";
                }
                val += $"{entry.MapName} ({entry.Filename})";
            }
            return val;
        }
    }
}

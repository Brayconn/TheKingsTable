using Avalonia.Data.Converters;
using CaveStoryModdingFramework.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheKingsTable.Converters
{
    internal class EntityListConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (targetType != typeof(string))
                throw new NotSupportedException();

            var val = "";
            if (values[0] is KeyValuePair<int, EntityInfo> entity)
            {
                /*
                if (values[1] is Dictionary<int, EntityInfo> entities)
                {
                    //TODO this can probably be cached
                    var index = entities.ContainsValue(entity) ? entities.First(x => x.Value == entity).Key : -1;
                    if (index != -1)
                        val += $"{index} - ";
                }
                val += entity.Name;
                */
                val = $"{entity.Key} - {entity.Value.Name}";
            }
            return val;
        }
    }
}

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CaveStoryModdingFramework
{
    public static class SmokeSizes
    {
        public readonly static ReadOnlyDictionary<int, string> SmokeSizeList
            = new ReadOnlyDictionary<int, string>(new Dictionary<int, string>()
        {
            {0, SmokeSizeNames.None },
            {1, SmokeSizeNames.Small },
            {2, SmokeSizeNames.Medium },
            {3, SmokeSizeNames.Large }
        });
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaveStoryModdingFramework
{
    public static class SmokeSizes
    {
        public readonly static Dictionary<int, string> SmokeSizeList = new Dictionary<int, string>()
        {
            {0, "None" },
            {1, "Small" },
            {2, "Medium" },
            {3, "Large" }
        };
    }
}

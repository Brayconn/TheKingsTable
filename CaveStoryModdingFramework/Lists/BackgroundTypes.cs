using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CaveStoryModdingFramework
{
    public static class BackgroundTypes
    {
        //The fact that this is <int,string> and not <long,string> theoretically causes an issue, but I think we'll be fine
        public static readonly ReadOnlyDictionary<int, string> BackgroundTypeList
            = new ReadOnlyDictionary<int, string>(new Dictionary<int, string>()
        {
            {0, BackgroundTypeNames.FixedToCamera },
            {1, BackgroundTypeNames.FollowSlowly },
            {2, BackgroundTypeNames.FixedToForeground },
            {3, BackgroundTypeNames.Water },
            {4, BackgroundTypeNames.NoDraw },
            {5, BackgroundTypeNames.ScrollItems },
            {6, BackgroundTypeNames.ParallaxItems },
            {7, BackgroundTypeNames.Parallax }
        });
    }
}

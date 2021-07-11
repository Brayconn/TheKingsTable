using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CaveStoryModdingFramework
{
    public static class Bosses
    {
        //The fact that this is <int,string> and not <long,string> theoretically causes an issue, but I think we'll be fine
        public static readonly ReadOnlyDictionary<int, string> BossNameList
            = new ReadOnlyDictionary<int, string>(new Dictionary<int, string>()
        {
            {0, BossNames.None },
            {1, BossNames.Omega },
            {2, BossNames.Balfrog },
            {3, BossNames.MonsterX },
            {4, BossNames.Core },
            {5, BossNames.Ironhead },
            {6, BossNames.Sisters },
            {7, BossNames.UndeadCore },
            {8, BossNames.HeavyPress },
            {9, BossNames.BallosBall }
        });
    }
}

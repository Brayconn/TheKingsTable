using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CaveStoryModdingFramework
{
    public static class Bosses
    {
        public static readonly ReadOnlyDictionary<long, string> BossNameList
            = new ReadOnlyDictionary<long, string>(new Dictionary<long, string>()
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

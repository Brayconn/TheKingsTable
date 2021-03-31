using System.Collections.Generic;
using System.Drawing;
using System.IO;
using CaveStoryModdingFramework.Entities;

namespace CaveStoryModdingFramework
{
    public static class EntityInfoTXT
    {
        public static Dictionary<int, EntityInfo> Load(string path, int divisor = 2)
        {
            var output = new Dictionary<int, EntityInfo>();
            using(var sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine().Split('\t');
                    if (line[0].StartsWith("//"))
                        continue;
                    //Substring(1) to igore the #
                    var num = int.Parse(line[0].Substring(1));
                    var short1 = line[1];
                    var short2 = line[2];
                    var longStr = line[3];
                    var rectPoints = line[4].Split(':');
                    var rect = Rectangle.FromLTRB(int.Parse(rectPoints[0]) / divisor, int.Parse(rectPoints[1]) / divisor, int.Parse(rectPoints[2]) / divisor, int.Parse(rectPoints[3]) / divisor);
                    var desc = line[5];
                    var category = line[6];

                    output.Add(num, new EntityInfo(longStr, rect, category));
                }
            }
            return output;
        }
    }
}

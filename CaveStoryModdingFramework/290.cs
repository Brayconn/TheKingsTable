using System;
using System.IO;

namespace CaveStoryModdingFramework
{
    public static class _290DotRec
    {
        public static bool TryLoad(string path, out int time)
        {
            int[] counter = new int[4];
            byte[] random;
            
            using(var br = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
            {
                for(int i = 0; i < counter.Length; i++)
                    counter[i] = br.ReadInt32();
                random = br.ReadBytes(4);
            }

            for(int i = 0; i < counter.Length; i++)
            {
                var work = BitConverter.GetBytes(counter[i]);
                work[0] -= random[i];
                work[1] -= random[i];
                work[2] -= random[i];
                work[3] -= (byte)(random[i] / 2);
                counter[i] = BitConverter.ToInt32(work,0);
            }
            
            bool valid = counter[0] == counter[1] && counter[0] == counter[2];
            time = valid ? counter[0] : 0;
            return valid;
        }

        public static void Save(string path, int time)
        {
            Save(path, time, (int)DateTimeOffset.Now.ToUnixTimeSeconds());
        }
        public static void Save(string path, int time, int seed)
        {
            Random rand = new Random(seed);

            int[] counter = new int[4];
            byte[] random = new byte[4];

            for(int i = 0; i < 4; i++)
            {
                random[i] = (byte)(rand.Next(0, 250) + i);

                var work = BitConverter.GetBytes(time);
                work[0] += random[i];
                work[1] += random[i];
                work[2] += random[i];
                work[3] += (byte)(random[i] / 2);
                counter[i] = BitConverter.ToInt32(work, 0);
            }

            using(var bw = new BinaryWriter(new FileStream(path, FileMode.Create, FileAccess.Write)))
            {
                foreach (var count in counter)
                    bw.Write(count);
                bw.Write(random);
            }
        }
    }
}

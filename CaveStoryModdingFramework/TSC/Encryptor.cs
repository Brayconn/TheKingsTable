using System.Collections.Generic;
using System.IO;

namespace CaveStoryModdingFramework.TSC
{
    public static class Encryptor
    {
        public const byte DefaultKey = 7;


        public static int LoadFromFile<T>(string path, T output, int offset, byte defaultKey = DefaultKey) where T : IList<byte>
        {
            using(var file = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                int middle = (int)(file.Length / 2);
                file.Position = middle;
                var key = (byte)file.ReadByte();
                if (key == 0)
                    key = defaultKey;

                file.Position = 0;
                for (int i = 0; i < file.Length; i++)
                    output[offset + i] = (byte)(file.ReadByte() - key);
                output[middle] = key;

                return (int)file.Length;
            }
        }

        public static void DecryptInPlace<T>(T input, byte defaultKey = DefaultKey) where T : IList<byte>
        {
            var key = input[input.Count / 2];
            if (key == 0)
                key = defaultKey;
            for (int i = 0; i < input.Count; i++)
                input[i] -= key;
            input[input.Count / 2] = key;
        }
        public static void EncryptInPlace<T>(T input, byte defaultKey = DefaultKey) where T : IList<byte>
        {
            var key = input[input.Count / 2];
            if (key == 0)
                key = defaultKey;
            for (int i = 0; i < input.Count; i++)
                input[i] += key;
            input[input.Count / 2] = key;
        }

        public static T Decrypt<T>(T input, byte defaultKey = DefaultKey) where T : IList<byte>
        {
             return Crypt(input, 0, input.Count, true, defaultKey);
        }
        public static T Encrypt<T>(T input, byte defaultKey = DefaultKey) where T : IList<byte>
        {
            return Crypt(input, 0, input.Count, false, defaultKey);
        }
        public static T Crypt<T>(T input, int offset, int length, bool decrypt, byte defaultKey = DefaultKey) where T : IList<byte>
        {
            var middle = offset + (length / 2);
            byte key = input[middle];
            if (key == 0)
                key = defaultKey;
            for(int i = offset; i < length; i++)
            {
                if (decrypt)
                    input[i] -= key;
                else
                    input[i] += key;
            }
            input[middle] = key;
            return input;
        }
    }
}

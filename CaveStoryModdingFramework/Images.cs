using System.IO;
using System.Text;
using System.Threading;

namespace CaveStoryModdingFramework
{
    public static class Images
    {
        public const string DefaultImageExtension = "pbm";
        public const string DefaultCopyrightString = "(C)Pixel";
        public static bool IsCopyrighted(string path, string input = DefaultCopyrightString)
        {
            bool result;
            using(var fs = new FileStream(path,FileMode.Open,FileAccess.Read))
            {
                result = IsCopyrighted(fs, input);
            }
            return result;
        }
        public static bool IsCopyrighted(FileStream file, string input = DefaultCopyrightString)
        {
            return IsCopyrighted(file, Encoding.ASCII.GetBytes(input));
        }
        public static bool IsCopyrighted(FileStream file, byte[] expected)
        {
            byte[] actual = new byte[expected.Length];

            file.Seek(-expected.Length, SeekOrigin.End);
            file.Read(actual, 0, expected.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                if (expected[i] != actual[i])
                {
                    return false;
                }
            }
            return true;
        }

        const int MaxAttempts = 5;
        public static bool UpdateCopyright(string path, string input = DefaultCopyrightString)
        {
            int attempts = 0;
            while (attempts < MaxAttempts)
            {
                Thread.Sleep(attempts * 1000);
                try
                {
                    using (var fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
                    {
                        return UpdateCopyright(fs, input);
                    }
                }
                catch (IOException)
                {
                    attempts++;
                }
            }
            throw new IOException("Unable to open file!");
        }
        public static bool UpdateCopyright(FileStream file, string input = DefaultCopyrightString)
        {
            return UpdateCopyright(file, Encoding.ASCII.GetBytes(input));
        }
        public static bool UpdateCopyright(FileStream file, byte[] expected)
        {
            if (!IsCopyrighted(file, expected))
            {
                //this is probably redundant, but just to be sure...
                file.Seek(0, SeekOrigin.End);
                file.Write(expected, 0, expected.Length);
                return true;
            }
            return false;
        }
    }
}

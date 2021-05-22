using System.IO;

namespace CaveStoryModdingFramework.Compatability
{
    public static class ScriptSource
    {
        public const string ScriptSource_Name = "ScriptSource";
        public const string ScriptSource_Extension = "txt";
        public static string GetScriptSourcePath(string path)
        {
            return Path.Combine(Path.GetDirectoryName(path), ScriptSource_Name, Path.ChangeExtension(Path.GetFileName(path),ScriptSource_Extension));
        }
        public static string GetScriptSourceDirectory(string path)
        {
            return Path.Combine(Path.GetDirectoryName(path), ScriptSource_Name);
        }
        public static string GetScriptPath(string path, string extension)
        {
            return Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(path)), Path.ChangeExtension(Path.GetFileNameWithoutExtension(path), extension));
        }
    }
}

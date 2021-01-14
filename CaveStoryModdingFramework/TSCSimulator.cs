using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CaveStoryModdingFramework.Entities;
using CaveStoryModdingFramework.TSC;
using CaveStoryModdingFramework.Maps;

namespace CaveStoryModdingFramework
{
    class CharacterPrintedEventArgs : EventArgs
    {
        public char Character { get; set; }
        public CharacterPrintedEventArgs(char character)
        {
            Character = character;
        }
    }
    interface ITSCInstruction
    {
        bool IsInstruction(ref string buffer, int index);

        void Run(ref string buffer, ref int? index, Map tiles, List<Entity> entities);
    }
    class TSCSimulator
    {
        public const int DefaultTSCBufferSize = 0x5000;

        Map Tiles;
        List<Entity> Entities;
        
        int BufferSize;
        byte[] scriptBuffer;
        int headSize;
        int index = 0;
        List<ITSCInstruction> instructions;
        byte DefaultKey;
        
        public TSCSimulator(Map tiles, List<Entity> entities, int bufferSize = DefaultTSCBufferSize, byte defaultKey = Encryptor.DefaultKey)
        {
            Tiles = tiles;
            Entities = entities;
            BufferSize = bufferSize;
            DefaultKey = defaultKey;
        }
        public void ClearBuffer()
        {
            scriptBuffer = new byte[BufferSize];
        }
        public void LoadHead(string headPath)
        {
            ClearBuffer();
            headSize = Encryptor.LoadFromFile(headPath, scriptBuffer, 0, DefaultKey);
        }
        public void LoadFiles(params string[] stagePaths)
        {
            foreach (var file in stagePaths)
            {
                //I don't think I'm brave enough to try removing len and using the call directly
                var len = Encryptor.LoadFromFile(file, scriptBuffer, headSize, DefaultKey);
                scriptBuffer[headSize + len] = 0;
            }
        }
        public void Load(string headPath, params string[] stagePaths)
        {
            LoadHead(headPath);
            LoadFiles(stagePaths);
        }

        public event EventHandler<CharacterPrintedEventArgs> CharacterPrinted;
        /// <summary>
        /// runs the given script directly
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        public void Run(byte[] script, int eventNum)
        {

        }
        public void Run(int eventNum)
        {
            Run(scriptBuffer, eventNum);
        }
        public void Run(string eventNum)
        {
            Run(scriptBuffer, FlagConverter.FlagToRealValue(eventNum));
        }
    }
}

using CaveStoryModdingFramework;
using CaveStoryModdingFramework.Compatability;
using CaveStoryModdingFramework.Stages;
using CaveStoryModdingFramework.TSC;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CaveStoryEditor
{
    public partial class FormScriptEditor : Form
    {
        bool unsavedChanges = false;
        public bool UnsavedChanges
        {
            get => unsavedChanges;
            private set
            { 
                if(value != unsavedChanges)
                {
                    if (value)
                    {
                        this.Text += "*";
                    }
                    else
                    {
                        this.Text = this.Text.Substring(0, this.Text.Length - 1);
                    }
                    unsavedChanges = value;
                }
            }
        }
        readonly Mod parentMod;
        public StageEntry stageEntry { get; private set; } = null;
        public string Fullpath { get; private set; } = "";

        bool UseScriptSource;
        bool Encrypted;



        bool ReloadNeeded = false;
        private void FormScriptEditor_Activated(object sender, EventArgs e)
        {
            if (ReloadNeeded)
            {
                if(MessageBox.Show("Hey the stage entry's filename was changed, want to reload the new value?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Reload();
                }
            }
        }

        #region loading

        string LoadScript(string path, bool encrypted)
        {
            byte[] input = File.ReadAllBytes(path);
            if (encrypted)
                Encryptor.DecryptInPlace(input, parentMod.DefaultKey);
            return parentMod.TSCEncoding.GetString(input);
        }
        void Reload()
        {
            //if the stageEntry is set, that means this is a linked editor, so we need to update all settings
            if (stageEntry != null)
            {
                Encrypted = parentMod.TSCEncrypted;
                UseScriptSource = parentMod.UseScriptSource;
                Fullpath = parentMod.FolderPaths.GetFile(SearchLocations.Stage, stageEntry.Filename, Extension.Script);
                //if we're in scriptsource mode, we have to get the actual scriptsource path
                if (UseScriptSource)
                {
                    Fullpath = ScriptSource.GetScriptSourcePath(Fullpath);
                }
            }

            this.Text = Path.GetFileName(Fullpath);
            if (UseScriptSource)
                this.Text += $" ({ScriptSource.ScriptSource_Name})";

            string altPath;
            //check the main path
            if (File.Exists(Fullpath))
            {
                //The only way for this to be encrypted is if we AREN'T using ScriptSource, and it's set as encrypted
                mainScintilla.Text = LoadScript(Fullpath, !UseScriptSource && Encrypted);
            }
            //if that file didn't exist, but we're using ScriptSource, check the original script path
            else if (UseScriptSource && File.Exists(altPath = ScriptSource.GetScriptPath(Fullpath, parentMod.TSCExtension)))
            {
                mainScintilla.Text = LoadScript(altPath, Encrypted);
            }
            //if both those fail, this must be a new file
            else
            {
                mainScintilla.Text = "";
            }
            UnsavedChanges = false;
        }

        #endregion

        private FormScriptEditor(Mod m)
        {
            parentMod = m;

            InitializeComponent();
        }
        public FormScriptEditor(Mod m, StageEntry entry) : this(m)
        {
            Encrypted = parentMod.TSCEncrypted;
            UseScriptSource = parentMod.UseScriptSource;
            stageEntry = entry;

            Reload();
            AutoDetectParserMode();
        }
        public FormScriptEditor(Mod m, string path, bool encrypted, bool useScriptSource) : this(m)
        {
            Encrypted = encrypted;
            UseScriptSource = useScriptSource;
            Fullpath = UseScriptSource ? ScriptSource.GetScriptSourcePath(path) : path;

            Reload();
            AutoDetectParserMode();
        }

        void AutoDetectParserMode()
        {
            //TODO this is a bad check for credits tbh
            if (Path.GetFileNameWithoutExtension(Fullpath).Contains("Credit"))
                ParserMode = ParserModes.Credits;
            else if (parentMod.UseScriptSource)
                ParserMode = ParserModes.Scriptsource;
            else
                ParserMode = ParserModes.Default;
        }

        #region saving

        public event EventHandler ScriptSaved;

        void Save(string path)
        {
            var bytes = parentMod.TSCEncoding.GetBytes(mainScintilla.Text);
            if (parentMod.UseScriptSource)
            {
                string ssdir = Path.GetDirectoryName(path);
                if (!Directory.Exists(ssdir))
                    Directory.CreateDirectory(ssdir);
                File.WriteAllBytes(path, bytes);
                path = ScriptSource.GetScriptPath(path, parentMod.TSCExtension);
            }
            if (parentMod.TSCEncrypted)
                Encryptor.EncryptInPlace(bytes, parentMod.DefaultKey);

            File.WriteAllBytes(path, bytes);
            
            UnsavedChanges = false;
            ScriptSaved?.Invoke(this, new EventArgs());
        }
        void Save(string path, bool encrypt)
        {
            var bytes = parentMod.TSCEncoding.GetBytes(mainScintilla.Text);
            if(encrypt)
                Encryptor.EncryptInPlace(bytes, parentMod.DefaultKey);

            File.WriteAllBytes(path, bytes);

            UnsavedChanges = false;
            ScriptSaved?.Invoke(this, new EventArgs());
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save(Fullpath);
        }

        private void saveAsTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog()
            {
                Filter = string.Join("|", "Text Files (*.txt)|*.txt", FormMain.AllFilesFilter)
            })
            {
                if(sfd.ShowDialog() == DialogResult.OK)
                {
                    Save(sfd.FileName, false);
                }
            }            
        }
        private void saveAsencryptedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ScriptFilter = Path.ChangeExtension("*", parentMod.TSCExtension);
            using (var sfd = new SaveFileDialog()
            {
                Filter = string.Join("|", $"TSC Files ({ScriptFilter})|{ScriptFilter}", FormMain.AllFilesFilter)
            })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Save(sfd.FileName, true);
                }
            }
        }

        #endregion

        private void mainScintilla_TextChanged(object sender, EventArgs e)
        {
            UnsavedChanges = true;
        }

        enum ParserModes
        {
            None,
            Default,
            Credits,
            Scriptsource
        }
        ParserModes parserMode = ParserModes.None;
        ParserModes ParserMode
        {
            get => parserMode;
            set
            {
                if (parserMode != value)
                {
                    parserMode = value;
                    defaultToolStripMenuItem.Checked = ParserMode == ParserModes.Default;
                    creditsToolStripMenuItem.Checked = ParserMode == ParserModes.Credits;
                    InitParserStyles(ParserMode, mainScintilla);
                }
            }
        }

        #region styling

        private void InitParserStyles(ParserModes mode, ScintillaNET.Scintilla scintilla)
        {
            scintilla.ClearDocumentStyle();
            switch (mode)
            {
                case ParserModes.Default:
                case ParserModes.Scriptsource:
                    for(int i = 0; i <= 6; i++)
                        scintilla.Styles[i].Font = "Consolas";

                    scintilla.Styles[TSCStyleValue].ForeColor = Color.FromArgb(0xC42F63);

                    scintilla.Styles[TSCStyleSeparator].ForeColor = Color.Gray;

                    scintilla.Styles[TSCStyleCommand].ForeColor = Color.Blue;

                    scintilla.Styles[TSCStyleEvent].ForeColor = Color.Black;
                    scintilla.Styles[TSCStyleEvent].Bold = true;

                    scintilla.Styles[TSCStyleError].ForeColor = Color.Red;
                    scintilla.Styles[TSCStyleError].BackColor = Color.Gray;

                    scintilla.Styles[TSCStyleComment].ForeColor = Color.FromArgb(0x367A2A);
                    scintilla.Styles[TSCStyleComment].Italic = true;
                    break;
                case ParserModes.Credits:
                    for (int i = 0; i <= 10; i++)
                        scintilla.Styles[i].Font = "Consolas";

                    scintilla.Styles[CreditStyleValue].ForeColor = Color.FromArgb(0xC42F63);

                    scintilla.Styles[CreditStyleSeparater].ForeColor = Color.Gray;

                    scintilla.Styles[StyleLabel].ForeColor = Color.Black;
                    scintilla.Styles[StyleLabel].Bold = true;

                    scintilla.Styles[StyleJump].ForeColor = Color.Blue;
                    //scintilla.Styles[StyleJump].Bold = true;

                    scintilla.Styles[StyleOffset].ForeColor = Color.Blue;
                    //scintilla.Styles[StyleOffset].Bold = true;

                    scintilla.Styles[StyleBrackets].ForeColor = Color.Blue;

                    scintilla.Styles[StyleFade].ForeColor = Color.Blue;
                    //scintilla.Styles[StyleFade].Bold = true;

                    scintilla.Styles[StyleEnd].ForeColor = Color.Blue;
                    //scintilla.Styles[StyleEnd].Bold = true;

                    scintilla.Styles[StyleMusic].ForeColor = Color.Blue;
                    //scintilla.Styles[StyleMusic].Bold = true;

                    scintilla.Styles[CreditStyleComment].ForeColor = Color.FromArgb(0x367A2A);
                    scintilla.Styles[CreditStyleComment].Italic = true;
                    break;
            }
            scintilla.StartStyling(0);
        }

        private void mainScintilla_StyleNeeded(object sender, ScintillaNET.StyleNeededEventArgs e)
        {
            var s = (ScintillaNET.Scintilla)sender;
            switch(ParserMode)
            {
                case ParserModes.Scriptsource:
                case ParserModes.Default:
                    StyleTSC(s, s.GetEndStyled(), e.Position);
                    break;
                case ParserModes.Credits:
                    StyleCredits(s, s.GetEndStyled(), e.Position);
                    break;
            }            
        }

        private static void LookBackUntil(ScintillaNET.Scintilla scintilla, ref int startPos, List<string> stopStrings)
        {
            if (startPos > 0)
                startPos--;
            while (startPos > 0)
            {
                string currentString = scintilla.GetTextRange(startPos, stopStrings[0].Length);
                foreach (var ss in stopStrings)
                {
                    if(currentString.Length != ss.Length)
                        currentString = scintilla.GetTextRange(startPos, ss.Length);
                    if (ss == currentString)
                        return;
                }
                startPos--;
            }                
        }

        private static int LookBackUntil(ScintillaNET.Scintilla scintilla, int startPos, char stopChar)
        {
            if (startPos > 0)
                startPos--;
            while (startPos > 0 && (char)scintilla.GetCharAt(startPos) != stopChar)
                startPos--;
            return startPos;
        }

        /*
        private static void LookBackUntil(ScintillaNET.Scintilla scintilla, ref int startPos, params char[] stopChars)
        {
            var table = new HashSet<char>(stopChars);
            if (startPos > 0)
                startPos--;
            while (startPos > 0 && !table.Contains((char)scintilla.GetCharAt(startPos)))
                startPos--;
        }
        */

        const int TSCStyleEvent = 1;
        const int TSCStyleCommand = 2;
        const int TSCStyleValue = 3;
        const int TSCStyleSeparator = 4;
        const int TSCStyleComment = 5;
        const int TSCStyleError = 6;
        

        const int TEMPHEADLASTFLAG = 49;
        private void StyleTSC(ScintillaNET.Scintilla scintilla, int startPos, int endPos)
        {
            if ((parentMod.Commands?.Count ?? 0) <= 0)
                return;
            HashSet<Command> stopCommands = new HashSet<Command>();
            List<string> stopStrings = new List<string>() { "#" };
            foreach (var cmd in parentMod.Commands)
            {
                if ((cmd.Properties & CommandProperties.EndsEvent) != 0)
                {
                    stopCommands.Add(cmd);
                    //stopStrings.Add(cmd.ShortName); //TODO this is disabled to reduce the chances of an edge case happening
                }
            }
            

            //find the first command or event number (or start of the text box)
            LookBackUntil(scintilla, ref startPos, stopStrings);
                        
            scintilla.StartStyling(startPos);
            //TODO if we reached 0 we need to check head
            var firstChar = (char)scintilla.GetCharAt(startPos);
            bool inEvent =  firstChar == '#' || firstChar == '<';
            while (startPos < endPos)
            {
                var currChar = (char)scintilla.GetCharAt(startPos);
                switch (currChar)
                {
                    case '#':
                        inEvent = true;

                        var eventValue = FlagConverter.FlagToRealValue(scintilla.GetTextRange(startPos + 1, 4));

                        var prevEventIndex = LookBackUntil(scintilla, startPos, '#');
                        int prevEventValue = (prevEventIndex == startPos || (char)scintilla.GetCharAt(prevEventIndex) != '#')
                            ? TEMPHEADLASTFLAG
                            : FlagConverter.FlagToRealValue(scintilla.GetTextRange(prevEventIndex + 1, 4));

                        scintilla.Style(5, eventValue > prevEventValue ? TSCStyleEvent : TSCStyleError);
                        startPos += 5;
                        break;
                    case '<' when inEvent:
                        string cmdText = scintilla.GetTextRange(startPos, parentMod.Commands[0].ShortName.Length);
                        Command foundCommand = null;
                        foreach(var cmd in parentMod.Commands)
                        {
                            if (cmdText.Length != cmd.ShortName.Length)
                                cmdText = scintilla.GetTextRange(startPos, cmd.ShortName.Length);
                            if (cmdText == cmd.ShortName)
                            {
                                foundCommand = cmd;
                                break;
                            }
                        }
                        if (foundCommand == null)
                        {
                            scintilla.Style(1, TSCStyleError);
                            startPos++;
                            break;
                        }

                        scintilla.Style(foundCommand.ShortName.Length, TSCStyleCommand);
                        startPos += foundCommand.ShortName.Length;

                        List<Tuple<int, int>> flattenedArgs = null;
                        if (foundCommand.UsesRepeats)
                            flattenedArgs = new List<Tuple<int, int>>();

                        void StyleArgs(List<object> args, bool forceEndingSeparator = false)
                        {
                            for (int i = 0; i < args.Count; i++)
                            {
                                if (args[i] is Argument a)
                                {
                                    //fixed length arguments
                                    if (a.Length > 0)
                                    {
                                        flattenedArgs?.Add(new Tuple<int, int>(startPos, a.Length));
                                        
                                        scintilla.Style(a.Length, TSCStyleValue);
                                        startPos += a.Length;
                                        if (a.Separator.Length > 0 && (forceEndingSeparator || i < args.Count - 1))
                                        {
                                            scintilla.Style(a.Separator.Length, TSCStyleSeparator);
                                            startPos += a.Separator.Length;
                                        }
                                    }
                                    //variable length arguments
                                    else
                                    {
                                        int argLen = -1;
                                        string maybeSep;
                                        do
                                        {
                                            maybeSep = scintilla.GetTextRange(startPos + (++argLen), a.Separator.Length);
                                        } while (maybeSep != a.Separator && startPos + argLen < scintilla.TextLength);

                                        flattenedArgs?.Add(new Tuple<int, int>(startPos, argLen));

                                        scintilla.Style(argLen, TSCStyleValue);
                                        scintilla.Style(a.Separator.Length, TSCStyleSeparator);

                                        startPos += argLen + a.Separator.Length;
                                    }
                                }
                                else if (args[i] is RepeatStructure r)
                                {
                                    switch (r.RepeatType)
                                    {
                                        case RepeatTypes.GlobalIndex:
                                            var repeatCount = FlagConverter.FlagToRealValue(scintilla.GetTextRange(flattenedArgs[r.Value].Item1, flattenedArgs[r.Value].Item2));
                                            for (int j = 0; j < repeatCount; j++)
                                                StyleArgs(r.Arguments, forceEndingSeparator || i < args.Count - 1 || j < repeatCount - 1);
                                            break;
                                        //TODO local index?
                                    }
                                }
                            }
                        } 
                        StyleArgs(foundCommand.Arguments);

                        if (stopCommands.Contains(foundCommand))
                            inEvent = false;
                        break;
                    default:
                        scintilla.Style(1, inEvent ? 0 : TSCStyleComment);
                        startPos++;
                        break;
                }
            }
        }

        const int CreditStyleValue = 1;
        const int CreditStyleSeparater = 2;
        const int StyleLabel = 3;
        const int StyleJump = 4;
        const int StyleOffset = 5;
        const int StyleBrackets = 6;
        const int StyleFade = 7;
        const int StyleEnd = 8;
        const int StyleMusic = 9;
        const int CreditStyleComment = 10;
        
        private void StyleCredits(ScintillaNET.Scintilla scintilla, int startPos, int endPos)
        {
            bool IsValidArg(int offset)
            {
                return char.IsDigit((char)scintilla.GetCharAt(offset))
                            && char.IsDigit((char)scintilla.GetCharAt(offset + 1))
                            && char.IsDigit((char)scintilla.GetCharAt(offset + 2))
                            && char.IsDigit((char)scintilla.GetCharAt(offset + 3));
            }

            //scan backwards for the first [, or the start of the file
            startPos = LookBackUntil(scintilla, startPos, '[');
            
            scintilla.StartStyling(startPos);
            while(startPos < endPos)
            {
                switch((char)scintilla.GetCharAt(startPos))
                {
                    case '[':
                        int textLen = 0;
                        char c;
                        //scan for the matching brace, or the end of the doc
                        for (c = (char)scintilla.GetCharAt(++startPos);
                            startPos < scintilla.TextLength && c != ']';
                            c = (char)scintilla.GetCharAt(++startPos))
                        {
                            textLen++;
                        }
                        //style the inner text if we found a brace
                        if (c == ']')
                        {
                            scintilla.Style(1, StyleBrackets);
                            scintilla.Style(textLen, 0);
                            goto case ']';
                        }
                        //otherwise it's just a comment I guess?
                        else
                        {
                            startPos -= textLen;
                            goto default;
                        }
                        break;
                    case ']':
                        scintilla.Style(1, StyleBrackets);
                        scintilla.Style(4, CreditStyleValue);
                        startPos += 5;
                        break;

                    case 'l':
                        //char prevChar = (char)scintilla.GetCharAt(startPos - 1);
                        if (IsValidArg(startPos + 1))
                        {
                            scintilla.Style(1, StyleLabel);
                            //scintilla.SetStyling(4, StyleValue);
                            scintilla.Style(4, StyleLabel);
                            startPos += 5;
                        }
                        else
                            goto default;
                        break;

                    case 'f':
                        scintilla.Style(1, StyleJump);
                        scintilla.Style(4, CreditStyleValue);
                        scintilla.Style(1, CreditStyleSeparater);
                        scintilla.Style(4, CreditStyleValue);
                        startPos += 10;
                        break;
                    case 'j':
                        scintilla.Style(1, StyleJump);
                        scintilla.Style(4, CreditStyleValue);
                        startPos += 5;
                        break;

                    case '-':
                    case '+':
                        scintilla.Style(1, StyleOffset);
                        scintilla.Style(4, CreditStyleValue);
                        startPos += 5;
                        break;

                    case '!':
                        scintilla.Style(1, StyleMusic);
                        scintilla.Style(4, CreditStyleValue);
                        startPos += 5;
                        break;
                    case '~':
                        scintilla.Style(1, StyleFade);
                        startPos++;
                        break;
                    case '/':
                        scintilla.Style(1, StyleEnd);
                        startPos++;
                        break;
                    default:
                        scintilla.Style(1, CreditStyleComment);
                        startPos++;
                        break;
                }
            }
        }

        #endregion

        private void defaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ParserMode = ParserModes.Default;
        }

        private void creditsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ParserMode = ParserModes.Credits;
        }
    }
}

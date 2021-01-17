using System;
using System.Collections.Generic;
using System.IO;
using CaveStoryModdingFramework.Stages;
using CaveStoryModdingFramework;
using System.Windows.Forms;

namespace CaveStoryEditor
{
    public class EditorManager
    {
        readonly Mod parentMod;
        readonly SpriteCache cache;
        
        readonly List<FormStageEditor> TileEditors;
        readonly List<FormScriptEditor> ScriptEditors;
        readonly List<FormAttributeEditor> AttributeEditors;
        
        public EditorManager(Mod parent, SpriteCache c)
        {
            parentMod = parent;
            cache = c;

            TileEditors = new List<FormStageEditor>();
            ScriptEditors = new List<FormScriptEditor>();
            AttributeEditors = new List<FormAttributeEditor>();
        }

        void RemoveEditor(object sender, EventArgs e)
        {
            if(sender is FormStageEditor fte)
            {
                TileEditors.Remove(fte);
            }
            else if(sender is FormScriptEditor fse)
            {
                ScriptEditors.Remove(fse);
            }
            else if(sender is FormAttributeEditor fae)
            {
                AttributeEditors.Remove(fae);
            }

            if (sender is IDisposable d)
                d.Dispose();
        }

        public void OpenTileEditor(StageEntry entry)
        {
            FormStageEditor editor = null;
            foreach(var e in TileEditors)
            {
                if(e.stageEntry == entry)
                {
                    editor = e;
                    break;
                }
            }
            if (editor == null)
            {
                //HACK(?) todictionary()
                editor = new FormStageEditor(parentMod, cache, Keybinds.Default.StageEditor.ToDictionary(), entry);
                editor.FormClosed += RemoveEditor;
                TileEditors.Add(editor);
            }
            editor.Show();
            editor.Focus();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Full path of the tsc file to edit</param>
        public void OpenScriptEditor(string path)
        {
            FormScriptEditor editor = null;
            foreach(var e in ScriptEditors)
            {
                if(e.Fullpath == path)
                {
                    editor = e;
                    break;
                }
            }
            if (editor == null)
            {
                editor = new FormScriptEditor(parentMod, path);
                editor.FormClosed += RemoveEditor;
                editor.ScriptSaved += Editor_ScriptSaved;
                ScriptEditors.Add(editor);
            }
            editor.Show();
            editor.Focus();
        }
        public void OpenAttributeFile(string path)
        {
            FormAttributeEditor editor = null;
            foreach(var e in AttributeEditors)
            {
                if(e.AttributeFilename == path)
                {
                    editor = e;
                    break;
                }
            }
            if (editor == null)
            {
                editor = new FormAttributeEditor(parentMod, path, Editor.Default.TileTypePath, Keybinds.Default.StageEditor.ToDictionary());
                editor.FormClosed += RemoveEditor;
                AttributeEditors.Add(editor);
            }
            editor.Show();
            editor.Focus();
        }

        private void Editor_ScriptSaved(object sender, EventArgs e)
        {
            /*
            var file = ((FormScriptEditor)sender).Fullpath;
            foreach(var editor in TileEditors)
            {
                if(editor.Filename == file)
                {
                    editor.NotifyMapStateRefreshNeeded();
                }
            }
            */
        }

        public void onScriptChanged(object sender, FileSystemEventArgs e)
        {
            /*
            //tell the tile editors to refresh their map states (or maybe put up a prompt?
            //tell the script editors to refresh their script
            foreach(var editor in TileEditors)
            {
                
            }
            */
        }
    }
}

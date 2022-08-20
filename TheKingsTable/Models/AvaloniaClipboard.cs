using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Platform;
using CaveStoryModdingFramework.Maps;
using CaveStoryModdingFramework.Editors;
using System;
using System.Linq;
using System.Collections.Generic;
using CaveStoryModdingFramework.Entities;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace TheKingsTable.Models
{
    class StageEditorClipboard
    {
        IClipboard clipboard;
        const string TILE_BINARY = nameof(TILE_BINARY);
        const string TILE_CURSOR_POS = nameof(TILE_CURSOR_POS);
        const string ENTITY_BINARY = nameof(ENTITY_BINARY);
        public StageEditorClipboard()
        {
            if (Application.Current!.Clipboard == null)
                throw new ArgumentNullException();
            clipboard = Application.Current.Clipboard;
        }
        static string MapToString(Map m)
        {
            var sb = new StringBuilder(((4 + m.Tiles.Count) * 3) - 1);
            sb.AppendJoin(' ', (m.Width & 0xFF).ToString("X2"),  (m.Width >> 8).ToString("X2"),
                               (m.Height & 0xFF).ToString("X2"), (m.Height >> 8).ToString("X2")
                         );
            for (int i = 0; i < m.Tiles.Count; i++)
                sb.Append(' ').Append(m.Tiles[i]?.ToString("X2") ?? "__");
            return sb.ToString();
        }
        static Map? StringToMap(string s)
        {
            try
            {
                var bs = s.Split(' ');
                var width  = (short)(Convert.ToByte(bs[0], 16) | (Convert.ToByte(bs[1], 16) << 8));
                var height = (short)(Convert.ToByte(bs[2], 16) | (Convert.ToByte(bs[3], 16) << 8));
                var m = new Map(width, height);

                //new apple product
                const int iStart = 4;
                //no cheating those dimensions... 
                if (m.Tiles.Count != bs.Length - iStart)
                    return null;

                for(int i = iStart; i < bs.Length; i++)
                {
                    m.Tiles[i - iStart] = bs[i] == "__" ? null : Convert.ToByte(bs[i], 16);
                }
                return m;
            }
            catch (OverflowException)
            {
                return null;
            }
            catch (FormatException)
            {
                return null;
            }
        }
        public async Task SetTiles(TileSelection value)
        {
            var d = new DataObject();
            //d.Set(TILE_BINARY, value);
            d.Set(TILE_CURSOR_POS, Encoding.ASCII.GetBytes($"{value.CursorX},{value.CursorY}"));
            d.Set(DataFormats.Text, MapToString(value.Contents));
            await clipboard.SetDataObjectAsync(d);
        }
        public async Task<TileSelection?> GetTiles()
        {
            var formats = new HashSet<string>(await clipboard.GetFormatsAsync());
            if (formats.Contains(TILE_BINARY))
            {
                var sel = await clipboard.GetDataAsync(TILE_BINARY);
                return sel as TileSelection;
            }
            else if (formats.Contains(DataFormats.Text))
            {
                var m = StringToMap((string)await clipboard.GetDataAsync(DataFormats.Text));
                if (m != null)
                {
                    var sel = new TileSelection(0, 0, m);
                    if (formats.Contains(TILE_CURSOR_POS))
                    {
                        //despite the fact that I'm storing a string, trying to cast directly back to string causes an exception
                        var coords = Encoding.ASCII.GetString((byte[])await clipboard.GetDataAsync(TILE_CURSOR_POS));
                        var cs = coords.Split(',');
                        if (cs.Length == 2 && int.TryParse(cs[0], out var p1) && int.TryParse(cs[1], out var p2))
                        {
                            sel.CursorX = p1;
                            sel.CursorY = p2;
                        }
                    }
                    return sel;
                }
            }
            return null;
        }

        public async Task SetEntities(HashSet<Entity> entities)
        {
            var left = short.MaxValue;
            var top = short.MaxValue;
            foreach(var e in entities)
            {
                if (e.X < left)
                    left = e.X;
                if (e.Y < top)
                    top = e.Y;
            }
            var l = new List<Entity>(entities.Count);
            foreach(var e in entities)
            {
                var n = new Entity(e);
                n.X -= left;
                n.Y -= top;
                l.Add(n);
            }
            var d = new DataObject();
            d.Set(ENTITY_BINARY, l);
            await clipboard.SetDataObjectAsync(d);
        }
        public async Task<List<Entity>?> GetEntities()
        {
            var formats = new HashSet<string>(await clipboard.GetFormatsAsync());
            if (formats.Contains(ENTITY_BINARY))
            {
                var ents = await clipboard.GetDataAsync(ENTITY_BINARY);
                return ents as List<Entity>;
            }
            else
            {
                return null;
            }
        }
    }
}

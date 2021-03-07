using CaveStoryModdingFramework;
using CaveStoryModdingFramework.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace CaveStoryEditor
{
    public class SpriteCache
    {
        readonly Mod parentMod;
        /// <summary>
        /// Sprites for files that are global
        /// </summary>
        readonly Dictionary<int, Image> GlobalImages = new Dictionary<int, Image>();
        /// <summary>
        /// Sprites for files that depend on the map
        /// </summary>
        readonly Dictionary<int, Dictionary<SurfaceSourceFile, Image>> LocalImages = new Dictionary<int, Dictionary<SurfaceSourceFile, Image>>();
        public SpriteCache(Mod m)
        {
            parentMod = m;
            GenerateGlobal();
        }
        bool TryResolveSurfacePath(SurfaceSourceFile surfaceSource, out string path)
        {
            path = ResolveSurfacePath(surfaceSource);
            return File.Exists(path);
        }
        string ResolveSurfacePath(SurfaceSourceFile surfaceSource)
        {
            string path = parentMod.DataFolderPath;
            switch (surfaceSource.Folder)
            {
                case Folders.Npc:
                    path = Path.Combine(path, parentMod.NpcFolderPath);
                    break;
                case Folders.Stage:
                    path = Path.Combine(path, parentMod.StageFolderPath);
                    break;
            }
            string name;
            switch (surfaceSource.Prefix)
            {
                case Prefixes.Spritesheet:
                    name = parentMod.SpritesheetPrefix;
                    break;
                case Prefixes.Tileset:
                    name = parentMod.TilesetPrefix;
                    break;
                default:
                    name = "";
                    break;
            }
            name += surfaceSource.Filename + "." + parentMod.ImageExtension;
            return Path.Combine(path, name);
        }

        bool TryGetSurfaceSource<T>(int num, out T surface) where T : class, ISurfaceSource
        {
            //check that the npc number has a valid rect
            if (parentMod.EntityInfos.TryGetValue(num, out var entInf) && !entInf.SpriteLocation.IsEmpty
                //and that it appears in the npc table
                && num < parentMod.NPCTable.Count)
            {
                var tblEntry = parentMod.NPCTable[num];
                //check if the npc table entry's surface number matches the type we expect
                if (parentMod.SurfaceDescriptors.TryGetValue(tblEntry.SpriteSurface, out var s) && s is T surf)
                {
                    surface = surf;
                    return true;
                }
            }
            surface = default;
            return false;
        }
        /*
        ISurfaceSource GetSurfaceSource(int num)
        {
           
        }
        */

        bool TryGenerateSpriteImage(Bitmap source, Rectangle location, out Bitmap img)
        {
            GraphicsUnit g = GraphicsUnit.Pixel;
            if (!location.IsEmpty && source.GetBounds(ref g).Contains(location))
            {
                try
                {
                    img = source.Clone(location, PixelFormat.DontCare);
                    img.MakeTransparent(parentMod.TransparentColor);
                    return true;
                }
                //Clone() can also throw this in the event of an invalid rect I think...?
                catch (ArgumentException) { }
                //Clone() throws this in the case of an invalid location
                catch (OutOfMemoryException) { }
            }
            img = null;
            return false;
        }

        public void GenerateGlobal(bool clear = false)
        {
            if(clear)
                GlobalImages.Clear();
            var parsedSurfaces = new Dictionary<SurfaceSourceFile, Bitmap>();

            //looping through every EntityInfo looking for the global files
            foreach(var entityNum in parentMod.EntityInfos.Keys)
            {
                if (TryGetSurfaceSource(entityNum, out SurfaceSourceFile surfaceSource))
                {
                    if (!parsedSurfaces.ContainsKey(surfaceSource))
                    {
                        if (!TryResolveSurfacePath(surfaceSource, out var path))
                            continue;
                        parsedSurfaces.Add(surfaceSource, new Bitmap(path));
                    }
                    var imgSrc = parsedSurfaces[surfaceSource];
                    
                    if(TryGenerateSpriteImage(imgSrc, parentMod.EntityInfos[entityNum].SpriteLocation, out Bitmap entityIcon))
                        GlobalImages.Add(entityNum, entityIcon);
                }
            }
        }
        public void GenerateLocal(SurfaceSourceFile spritesheet, int index, bool clobber = false)
        {
            if (!TryResolveSurfacePath(spritesheet, out var path))
                return;
            var img = new Bitmap(path);
            for(int i = 0; i < parentMod.NPCTable.Count; i++)
            {
                if(parentMod.NPCTable[i].SpriteSurface == index && parentMod.EntityInfos.TryGetValue(i, out EntityInfo entInf))
                {
                    if (!LocalImages.ContainsKey(i))
                        LocalImages[i] = new Dictionary<SurfaceSourceFile, Image>();
                    if (TryGenerateSpriteImage(img, entInf.SpriteLocation, out Bitmap icon))
                    {
                        if (!LocalImages[i].ContainsKey(spritesheet))
                            LocalImages[i].Add(spritesheet, icon);
                        else if (clobber)
                            LocalImages[i][spritesheet] = icon;
                    }
                }
            }
        }

        public bool TryGetGlobalSprite(int num, out Image img)
        {
            return GlobalImages.TryGetValue(num, out img);
        }
        public Image GetGlobalSprite(int num)
        {
            return GlobalImages[num];
        }
        public bool TryGetLocalSprite(int num, SurfaceSourceFile spritesheet, out Image img)
        {
            img = null;
            return LocalImages.TryGetValue(num, out var val) && val.TryGetValue(spritesheet, out img);
        }
        public Image GetLocalSprite(int num, SurfaceSourceFile spritesheet)
        {
            return LocalImages[num][spritesheet];
        }
        public bool TryGetSprite(int num, out Image img, params SurfaceSourceFile[] spritesheets)
        {
            if(TryGetGlobalSprite(num,out img) ||
                (TryGetSurfaceSource(num, out SurfaceSourceIndex index) && TryGetLocalSprite(num, spritesheets[index.Index], out img)))
            {
                return true;
            }
            img = null;
            return false;
        }

        public Image[] GenerateList(out int[] indexTransfer, params SurfaceSourceFile[] spritesheets)
        {
            var imgs = new List<Image>();
            var indexes = new int[parentMod.NPCTable.Count];
            for (int i = 0; i < parentMod.NPCTable.Count; i++)
            {
                if(TryGetSprite(i, out var img, spritesheets))
                {
                    imgs.Add(img);
                    indexes[i] = imgs.Count - 1;
                }
                else
                {
                    indexes[i] = -1;
                }
            }
            indexTransfer = indexes;
            return imgs.ToArray();
        }

        public Image[] GetImageList(params SurfaceSourceFile[] spritesheets)
        {
            var imgs = new Image[parentMod.NPCTable.Count];
            for(int i = 0; i < imgs.Length; i++)
            {
                TryGetSprite(i, out imgs[i], spritesheets);
            }
            return imgs;            
        }
    }
}

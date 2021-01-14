using CaveStoryModdingFramework.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace CaveStoryEditor
{
    interface IUndo { }

    class EntitiesMoved : IUndo
    {
        public class MovedEntity
        {
            public Point OldLocation { get; }

            public Point NewLocation { get; set; }

            public MovedEntity(Point old, Point @new)
            {
                OldLocation = old;
                NewLocation = @new;
            }
        }

        public Dictionary<Entity, MovedEntity> Entities { get; } = new Dictionary<Entity, MovedEntity>();
    }

    class EntityPropertiesChanged : IUndo
    {
        public class EntityPropertyChanged
        {
            public int OldValue { get; }
            public int NewValue { get; set; }
            public EntityPropertyChanged(int old, int @new)
            {
                OldValue = old;
                NewValue = @new;
            }
        }
        public string Property { get; }

        public Dictionary<Entity, EntityPropertyChanged> Entities { get; } = new Dictionary<Entity, EntityPropertyChanged>();

        public EntityPropertiesChanged(string prop)
        {
            Property = prop;
        }

    }

    class EntityListChanged : IUndo
    {
        public Entity[] OldEntities { get; }
        public Entity[] NewEntities { get; set; }

        public EntityListChanged(Entity[] old)
        {
            OldEntities = old;
            NewEntities = Array.Empty<Entity>();
        }
    }

    class TilesPlaced : IUndo
    {
        public class TileChanged<T>
        {
            public T OldValue { get; }
            public T NewValue { get; set; }

            public TileChanged(T prev, T @new)
            {
                OldValue = prev;
                NewValue = @new;
            }
        }

        public Dictionary<int, TileChanged<byte?>> Tiles { get; } = new Dictionary<int, TileChanged<byte?>>();
    }

    class MapResized<S,T> : IUndo
    {
        public S OldWidth { get; }
        public S OldHeight { get; }
        public T[] OldTiles { get; }

        public S NewWidth { get; set; }
        public S NewHeight { get; set; }
        public T[] NewTiles { get; set; }

        public MapResized(S oldWidth, S oldHeight, T[] old)
        {
            OldWidth = oldWidth;
            OldHeight = oldHeight;
            OldTiles = old;

            NewTiles = Array.Empty<T>();
        }
    }
}

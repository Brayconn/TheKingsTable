using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CaveStoryModdingFramework
{
    public class BulletInfo : IXmlSerializable
    {
        public string Name { get; set; }
        public Rectangle SpriteLocation { get; set; }

        private BulletInfo() { }
        public BulletInfo(string name, Rectangle spriteLocation)
        {
            Name = name;
            SpriteLocation = spriteLocation;
        }

        public XmlSchema GetSchema() => null;

        public void ReadXml(XmlReader reader)
        {
            Name = reader.GetAttribute(nameof(Name));
            SpriteLocation = RectExtensions.RectFromString(reader.GetAttribute("Rect"));
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString(nameof(Name), Name);
            writer.WriteAttributeString("Rect", RectExtensions.RectToString(SpriteLocation));
        }
    }
    public static class BulletList
    {
        public static readonly ReadOnlyDictionary<int, BulletInfo> BulletInfos = new ReadOnlyDictionary<int, BulletInfo>(new Dictionary<int, BulletInfo>()
        {
            {0,new BulletInfo("Null", new Rectangle(0,0,0,0))},
            {1,new BulletInfo("Snake Lv. 1", new Rectangle(0,0,0,0))},
            {2,new BulletInfo("Snake Lv. 2", new Rectangle(0,0,0,0))},
            {3,new BulletInfo("Snake Lv. 3", new Rectangle(0,0,0,0))},
            {4,new BulletInfo("Polar Star Lv. 1", new Rectangle(0,0,0,0))},
            {5,new BulletInfo("Polar Star Lv. 2", new Rectangle(0,0,0,0))},
            {6,new BulletInfo("Polar Star Lv. 3", new Rectangle(0,0,0,0))},
            {7,new BulletInfo("Fireball Lv. 1", new Rectangle(0,0,0,0))},
            {8,new BulletInfo("Fireball Lv. 2", new Rectangle(0,0,0,0))},
            {9,new BulletInfo("Fireball Lv. 3", new Rectangle(0,0,0,0))},
            {10,new BulletInfo("Machine Gun Lv. 1", new Rectangle(0,0,0,0))},
            {11,new BulletInfo("Machine Gun Lv. 2", new Rectangle(0,0,0,0))},
            {12,new BulletInfo("Machine Gun Lv. 3", new Rectangle(0,0,0,0))},
            {13,new BulletInfo("Missile Launcher Lv. 1", new Rectangle(0,0,0,0))},
            {14,new BulletInfo("Missile Launcher Lv. 2", new Rectangle(0,0,0,0))},
            {15,new BulletInfo("Missile Launcher Lv. 3", new Rectangle(0,0,0,0))},
            {16,new BulletInfo("Missile Launcher (Explosion) Lv. 1", new Rectangle(0,0,0,0))},
            {17,new BulletInfo("Missile Launcher (Explosion) Lv. 2", new Rectangle(0,0,0,0))},
            {18,new BulletInfo("Missile Launcher (Explosion) Lv. 3", new Rectangle(0,0,0,0))},
            {19,new BulletInfo("Bubbler Lv. 1", new Rectangle(0,0,0,0))},
            {20,new BulletInfo("Bubbler Lv. 2", new Rectangle(0,0,0,0))},
            {21,new BulletInfo("Bubbler Lv. 3", new Rectangle(0,0,0,0))},
            {22,new BulletInfo("Bubbler (Spines) Lv. 3", new Rectangle(0,0,0,0))},
            {23,new BulletInfo("Blade (Slashes) Lv. 3", new Rectangle(0,0,0,0))},
            {24,new BulletInfo("Egg Corridor? Falling Spike", new Rectangle(0,0,0,0))},
            {25,new BulletInfo("Blade Lv. 1", new Rectangle(0,0,0,0))},
            {26,new BulletInfo("Blade Lv. 2", new Rectangle(0,0,0,0))},
            {27,new BulletInfo("Blade Lv. 3", new Rectangle(0,0,0,0))},
            {28,new BulletInfo("Super Missile Launcher Lv. 1", new Rectangle(0,0,0,0))},
            {29,new BulletInfo("Super Missile Launcher Lv. 2", new Rectangle(0,0,0,0))},
            {30,new BulletInfo("Super Missile Launcher Lv. 3", new Rectangle(0,0,0,0))},
            {31,new BulletInfo("Super Missile Launcher (Explosion) Lv. 1", new Rectangle(0,0,0,0))},
            {32,new BulletInfo("Super Missile Launcher (Explosion) Lv. 2", new Rectangle(0,0,0,0))},
            {33,new BulletInfo("Super Missile Launcher (Explosion) Lv. 3", new Rectangle(0,0,0,0))},
            {34,new BulletInfo("Nemesis Lv. 1", new Rectangle(0,0,0,0))},
            {35,new BulletInfo("Nemesis Lv. 2", new Rectangle(0,0,0,0))},
            {36,new BulletInfo("Nemesis Lv. 3", new Rectangle(0,0,0,0))},
            {37,new BulletInfo("Spur Lv. 1", new Rectangle(0,0,0,0))},
            {38,new BulletInfo("Spur Lv. 2", new Rectangle(0,0,0,0))},
            {39,new BulletInfo("Spur Lv. 3", new Rectangle(0,0,0,0))},
            {40,new BulletInfo("Spur (Trail Tail) Lv. 3", new Rectangle(0,0,0,0))},
            {41,new BulletInfo("Spur (Trail Body) Lv. 3", new Rectangle(0,0,0,0))},
            {42,new BulletInfo("Spur (Trail Head) Lv. 3", new Rectangle(0,0,0,0))},
            {43,new BulletInfo("Curly's Nemesis", new Rectangle(0,0,0,0))},
            {44,new BulletInfo("Debug Kill-all Bullet", new Rectangle(0,0,0,0))},
            {45,new BulletInfo("Whimsical Star", new Rectangle(0,0,0,0))},
        });
    }
}

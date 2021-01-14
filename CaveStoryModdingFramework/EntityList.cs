using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using LocalizeableComponentModel;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CaveStoryModdingFramework.Entities
{
    #region Common

    public class MultiEntityShell : INotifyPropertyChanging, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        private void NotifyPropertyChanging(string propertyName)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        readonly Entity[] hosts;
        private T GetProperty<T>(Entity e, PropertyInfo property)
        {
            return (T)property.GetValue(e);
        }
        private T? GetProperty<T>([CallerMemberName] string propertyName = "") where T : struct
        {
            var property = typeof(Entity).GetProperty(propertyName);
            T? val = GetProperty<T?>(hosts[0], property);
            for (int i = 1; i < hosts.Length; i++)
                if (!val.Equals(GetProperty<T>(hosts[i], property)))
                    return null;
            return val;
        }

        private void SetProperty<T>(T value, [CallerMemberName] string propertyName = "")
        {
            if (value == null)
                return;
            NotifyPropertyChanging(propertyName);
            var property = typeof(Entity).GetProperty(propertyName);
            foreach (var entity in hosts)
            {
                property.SetValue(entity, value);
            }
            NotifyPropertyChanged(propertyName);
        }

        [LocalizeableDescription(nameof(Dialog.XDescription), typeof(Dialog))]
        public short? X { get => GetProperty<short>(); set => SetProperty(value); }
        [LocalizeableDescription(nameof(Dialog.YDescription), typeof(Dialog))]
        public short? Y { get => GetProperty<short>(); set => SetProperty(value); }
        [LocalizeableDescription(nameof(Dialog.FlagDescription), typeof(Dialog))]
        public short? Flag { get => GetProperty<short>(); set => SetProperty(value); }
        [LocalizeableDescription(nameof(Dialog.EventDescription), typeof(Dialog))]
        public short? Event { get => GetProperty<short>(); set => SetProperty(value); }
        [LocalizeableDescription(nameof(Dialog.TypeDescription), typeof(Dialog))]
        public short? Type { get => GetProperty<short>(); set => SetProperty(value); }
        [LocalizeableDescription(nameof(Dialog.BitsDescription), typeof(Dialog))]
        public EntityFlags? Bits { get => GetProperty<EntityFlags>(); set => SetProperty(value); }


        public MultiEntityShell(params Entity[] ents)
        {
            if (ents.Length < 1)
                throw new ArgumentException("You must supply at least one entity.", nameof(ents));
            hosts = ents;
        }
    }

    /// <summary>
    /// Shell over an entity to allow for adding entity specific interfaces
    /// </summary>
    public abstract class EntityShell
    {
        readonly Entity host;


        [LocalizeableDescription(nameof(Dialog.XDescription), typeof(Dialog))]
        public short X { get => host.X; set => host.X = value; }

        [LocalizeableDescription(nameof(Dialog.YDescription), typeof(Dialog))]
        public short Y { get => host.Y; set => host.Y = value; }
        [LocalizeableDescription(nameof(Dialog.FlagDescription), typeof(Dialog))]
        public short Flag { get => host.Flag; set => host.Flag = value; }
        [LocalizeableDescription(nameof(Dialog.EventDescription), typeof(Dialog))]
        public short Event { get => host.Event; set => host.Event = value; }

        [LocalizeableDescription(nameof(Dialog.TypeDescription), typeof(Dialog))]
        public short Type { get => host.Type; set => host.Type = value; }

        [LocalizeableDescription(nameof(Dialog.BitsDescription), typeof(Dialog))]
        public EntityFlags Bits { get => host.Bits; set => host.Bits = value; }

        protected EntityShell(Entity e)
        {
            host = e;
        }
    }

    /*
    abstract class StringTypeConverter<T> : TypeConverter where T : struct
    {
        readonly List<string> text;
        readonly StandardValuesCollection svc;
        readonly string errorText;
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => true;
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return svc;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string s)
            {
                var t = typeof(T);
                if (t == typeof(int))
                    return text.IndexOf(s);
                else if (t == typeof(bool))
                    return text.IndexOf(s) == 0;
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(int) || destinationType == typeof(bool) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return value switch
            {
                int i => (0 <= i && i < text.Count) ? text[i] : errorText,
                bool b => text[b ? 1 : 0],
                _ => base.ConvertTo(context, culture, value, destinationType)
            };
        }

        public StringTypeConverter(string e, params string[] t)
        {
            errorText = e;
            text = new List<string>(t);

            //bool mode
            if (typeof(T) == typeof(bool))
            {
                //either provide just the two as seperate params
                if (!string.IsNullOrWhiteSpace(e) && t.Length == 1)
                    text.Add(errorText);
                //or leave e blank and provide two values to t
                else if (!(string.IsNullOrWhiteSpace(e) && t.Length == 2))
                    throw new ArgumentException("You must provide only 2 options for bool mode.", nameof(e) + ", " + nameof(t));
            }
            //if it's not bool, it better be int, otherwise...
            else if (typeof(T) != typeof(int))
                throw new NotSupportedException("Supplied type is not supported. Please use bool or int.");

            svc = new StandardValuesCollection(text);
        }
    }
    */
    #endregion

    public class EntityInfo
    {
        public string Name { get; set; }

        public string Category { get; set; }

        [ReadOnly(true)]
        public Type CustomType { get; set; }

        public Rectangle SpriteLocation { get; set; }

        public EntityInfo(string name, Rectangle spriteLocation, string category = "")
        {
            Name = name;
            Category = category;
            SpriteLocation = spriteLocation;
        }
        public EntityInfo(string name, Type type, Rectangle spriteLocation, string category = "") : this(name, spriteLocation, category)
        {
            CustomType = type;
        }
    }

    public static class EntityList
    {
        public static readonly ReadOnlyDictionary<int, EntityInfo> EntityInfos = new ReadOnlyDictionary<int, EntityInfo>(new Dictionary<int, EntityInfo>()
        {
            {0,new EntityInfo("Null",new Rectangle(0,0,0,0))},
            {1,new EntityInfo("EXP",new Rectangle(0,16,16,16))},
            {2,new EntityInfo("Behemoth (enemy)",new Rectangle(0,0,32,24))},
            {3,new EntityInfo("Null, spawned upon NPC death, disappears",new Rectangle(0,0,0,0))},
            {4,new EntityInfo("Smoke",new Rectangle(64,0,16,16))},
            {5,new EntityInfo("Critter, Hopping Green (enemy)",new Rectangle(0,47,16,15))},
            {6,new EntityInfo("Beetle, Horizontal Green (enemy)",new Rectangle(0,80,16,16))},
            {7,new EntityInfo("Basil (enemy)",new Rectangle(256,64,24,19))},
            {8,new EntityInfo("Beetle, Follow Green (enemy)",new Rectangle(80,80,16,16))},
            {9,new EntityInfo("Balrog, falling",new Rectangle(0,0,40,24))},
            {10,new EntityInfo("Balrog, Shooting (boss)",new Rectangle(0,0,40,24))},
            {11,new EntityInfo("Balrog energy ball invincible (projectile)",new Rectangle(240,80,16,16))},
            {12,new EntityInfo("Balrog, standing",new Rectangle(0,0,40,24))},
            {13,new EntityInfo("Forcefield",new Rectangle(128,0,21,16))},
            {14,new EntityInfo("Santa's Key",new Rectangle(225,0,15,16))},
            {15,new EntityInfo("Treasure Chest, closed",new Rectangle(240,0,16,16))},
            {16,new EntityInfo("Save Point",new Rectangle(96,16,16,16))},
            {17,new EntityInfo("Health/Ammo Refill",new Rectangle(288,0,16,16))},
            {18,new EntityInfo("Door",new Rectangle(224,16,16,24))},
            {19,new EntityInfo("Balrog, busts in",new Rectangle(0,0,40,24))},
            {20,new EntityInfo("Computer",new Rectangle(288,16,32,24))},
            {21,new EntityInfo("Treasure Chest, open",new Rectangle(224,40,16,8))},
            {22,new EntityInfo("Teleporter",new Rectangle(240,16,24,32))},
            {23,new EntityInfo("Teleporter lights",new Rectangle(264,32,24,4))},
            {24,new EntityInfo("Power Critter (enemy)",new Rectangle(0,0,24,24))},
            {25,new EntityInfo("Lift Platform / Elevator",new Rectangle(256,64,34,16))},
            {26,new EntityInfo("Bat, Black Circling (enemy)",new Rectangle(32,80,16,16))},
            {27,new EntityInfo("Death Spikes",new Rectangle(96,64,32,24))},
            {28,new EntityInfo("Critter, Flying Green (enemy)",new Rectangle(48,48,16,16))},
            {29,new EntityInfo("Cthulhu",new Rectangle(0,192,16,24))},
            {30,new EntityInfo("Hermit Gunsmith",new Rectangle(0,32,16,16))},
            {31,new EntityInfo("Bat, Black Hanging (enemy)",new Rectangle(0,79,16,16))},
            {32,new EntityInfo("Life Capsule",new Rectangle(32,96,16,16))},
            {33,new EntityInfo("Balrog energy ball bouncing (projectile)",new Rectangle(240,80,16,16))},
            {34,new EntityInfo("Bed",new Rectangle(192,48,32,16))},
            {35,new EntityInfo("Mannan (enemy)",new Rectangle(96,64,24,32))},
            {36,new EntityInfo("Balrog, Flying (boss)",new Rectangle(0,0,40,24))},
            {37,new EntityInfo("Signpost",new Rectangle(192,64,16,16))},
            {38,new EntityInfo("Fireplace",new Rectangle(128,64,16,16))},
            {39,new EntityInfo("Cocktail Sign",new Rectangle(224,63,16,17))},
            {40,new EntityInfo("Santa",new Rectangle(0,32,16,16))},
            {41,new EntityInfo("Busted doorway",new Rectangle(0,0,0,0))},
            {42,new EntityInfo("Sue",new Rectangle(0,0,16,16))},
            {43,new EntityInfo("Blackboard/Table",new Rectangle(128,80,40,32))},
            {44,new EntityInfo("Polish (enemy)",new Rectangle(0,0,32,32))},
            {45,new EntityInfo("Baby (enemy)",new Rectangle(16,32,16,16))},
            {46,new EntityInfo("H/V Trigger",new Rectangle(0,0,0,0))},
            {47,new EntityInfo("Sandcroc, Green (enemy)",new Rectangle(153,47,35,33))},
            {48,new EntityInfo("Omega mudball (projectile)",new Rectangle(287,87,16,16))},
            {49,new EntityInfo("Skullhead (enemy)",new Rectangle(32,80,32,22))},
            {50,new EntityInfo("Bone (projectile)",new Rectangle(48,32,16,16))},
            {51,new EntityInfo("Crow & Skullhead (enemy)",new Rectangle(64,80,62,22))},
            {52,new EntityInfo("Blue Robot",new Rectangle(256,200,16,16))},
            {53,new EntityInfo("Skullstep Leg (enemy)",new Rectangle(0,0,0,0))},
            {54,new EntityInfo("Skullstep (enemy)",new Rectangle(32,80,32,22))},
            {55,new EntityInfo("Kazuma",new Rectangle(192,192,16,24))},
            {56,new EntityInfo("Beetle, Horizontal Brown (enemy)",new Rectangle(0,142,16,16))},
            {57,new EntityInfo("Crow (enemy)",new Rectangle(96,80,32,32))},
            {58,new EntityInfo("Basu, 1 (enemy)",new Rectangle(192,0,24,24))},
            {59,new EntityInfo("Evil Door (enemy)",new Rectangle(208,80,16,24))},
            {60,new EntityInfo("Toroko",new Rectangle(0,64,16,16))},
            {61,new EntityInfo("King",new Rectangle(224,32,16,16))},
            {62,new EntityInfo("Kazuma, facing away",new Rectangle(272,192,16,24))},
            {63,new EntityInfo("Toroko, panicking",new Rectangle(64,64,16,16))},
            {64,new EntityInfo("Critter, Hopping Blue (enemy)",new Rectangle(0,0,16,16))},
            {65,new EntityInfo("Bat, Blue Vertical (enemy)",new Rectangle(32,32,16,16))},
            {66,new EntityInfo("Misery bubble (projectile)",new Rectangle(32,216,23,24))},
            {67,new EntityInfo("Misery, Flying",new Rectangle(80,0,16,16))},
            {68,new EntityInfo("Balrog, Running (boss)",new Rectangle(0,0,40,24))},
            {69,new EntityInfo("Pignon (enemy)",new Rectangle(48,0,16,16))},
            {70,new EntityInfo("Sparkling Item",new Rectangle(96,48,16,16))},
            {71,new EntityInfo("Chinfish (enemy)",new Rectangle(64,32,16,16))},
            {72,new EntityInfo("Sprinkler",new Rectangle(224,48,16,16))},
            {73,new EntityInfo("Water Drop, 1",new Rectangle(0,0,0,0))},
            {74,new EntityInfo("Jack",new Rectangle(64,0,16,16))},
            {75,new EntityInfo("Kanpachi",new Rectangle(277,32,24,24))},
            {76,new EntityInfo("Flowers",new Rectangle(32,0,16,16))},
            {77,new EntityInfo("Sandaime's Pavillion",new Rectangle(0,16,48,32))},
            {78,new EntityInfo("Pot",new Rectangle(160,48,16,16))},
            {79,new EntityInfo("Mahin",new Rectangle(0,0,16,16))},
            {80,new EntityInfo("Gravekeeper (enemy)",new Rectangle(0,64,24,24))},
            {81,new EntityInfo("Giant Pignon (enemy)",new Rectangle(144,64,24,24))},
            {82,new EntityInfo("Misery, Standing",new Rectangle(112,0,16,16))},
            {83,new EntityInfo("Igor, standing",new Rectangle(0,0,40,40))},
            {84,new EntityInfo("Basu energy ball, 1 (projectile)",new Rectangle(64,48,16,16))},
            {85,new EntityInfo("Terminal",new Rectangle(272,96,15,24))},
            {86,new EntityInfo("Missile",new Rectangle(0,80,16,16))},
            {87,new EntityInfo("Heart",new Rectangle(32,80,16,16))},
            {88,new EntityInfo("Igor (boss)",new Rectangle(0,0,40,40))},
            {89,new EntityInfo("Igor, dying",new Rectangle(287,88,21,16))},
            {90,new EntityInfo("Background",new Rectangle(0,0,0,0))},
            {91,new EntityInfo("Cage",new Rectangle(96,88,32,24))},
            {92,new EntityInfo("Sue, facing away",new Rectangle(272,216,16,24))},
            {93,new EntityInfo("Chaco",new Rectangle(128,0,16,16))},
            {94,new EntityInfo("Kulala (enemy)",new Rectangle(272,0,48,24))},
            {95,new EntityInfo("Jelly (enemy)",new Rectangle(192,65,16,16))},
            {96,new EntityInfo("Fan, left",new Rectangle(288,120,16,16))},
            {97,new EntityInfo("Fan, up",new Rectangle(288,136,16,16))},
            {98,new EntityInfo("Fan, right",new Rectangle(288,152,16,16))},
            {99,new EntityInfo("Fan, down",new Rectangle(288,168,16,16))},
            {100,new EntityInfo("Grate",new Rectangle(272,48,16,16))},
            {101,new EntityInfo("Power Controls, screen",new Rectangle(240,136,16,16))},
            {102,new EntityInfo("Power Controls, power flow",new Rectangle(208,120,16,16))},
            {103,new EntityInfo("Mannan red blast (projectile)",new Rectangle(193,99,16,16))},
            {104,new EntityInfo("Frog (enemy)",new Rectangle(0,112,32,32))},
            {105,new EntityInfo("Speech balloon 'Hey' low",new Rectangle(128,32,16,16))},
            {106,new EntityInfo("Speech balloon 'Hey' high",new Rectangle(128,32,16,16))},
            {107,new EntityInfo("Malco",new Rectangle(208,0,16,24))},
            {108,new EntityInfo("Balfrog spitball (projectile)",new Rectangle(96,48,14,15))},
            {109,new EntityInfo("Malco, damaged",new Rectangle(240,0,16,24))},
            {110,new EntityInfo("Puchi (enemy)",new Rectangle(96,128,16,16))},
            {111,new EntityInfo("Quote, teleporting out",new Rectangle(0,0,16,16))},
            {112,new EntityInfo("Quote, teleporting in",new Rectangle(0,0,16,16))},
            {113,new EntityInfo("Professor Booster",new Rectangle(224,0,16,16))},
            {114,new EntityInfo("Press (enemy)",new Rectangle(176,112,16,24))},
            {115,new EntityInfo("Ravil (enemy)",new Rectangle(0,120,24,24))},
            {116,new EntityInfo("Red Flowers",new Rectangle(272,184,48,16))},
            {117,new EntityInfo("Curly, standing",new Rectangle(0,96,16,16))},
            {118,new EntityInfo("Curly (boss)",new Rectangle(0,32,32,24))},
            {119,new EntityInfo("Table & Chair",new Rectangle(248,184,24,16))},
            {120,new EntityInfo("Colon, 1",new Rectangle(0,0,16,16))},
            {121,new EntityInfo("Colon, 2",new Rectangle(64,16,16,16))},
            {122,new EntityInfo("Colon (enemy)",new Rectangle(80,0,16,16))},
            {123,new EntityInfo("Curly Machine Gun bullet (projectile)",new Rectangle(192,0,16,16))},
            {124,new EntityInfo("Sunstone",new Rectangle(192,0,31,32))},
            {125,new EntityInfo("Hidden Heart/Missile",new Rectangle(144,200,16,16))},
            {126,new EntityInfo("Puppy, running",new Rectangle(24,72,8,8))},
            {127,new EntityInfo("Machine Gun trail, Lv2 (projectile)",new Rectangle(64,86,48,4))},
            {128,new EntityInfo("Machine Gun trail, Lv3 (projectile)",new Rectangle(0,0,0,0))},
            {129,new EntityInfo("Fireball/Snake trail (projectile)",new Rectangle(0,0,0,0))},
            {130,new EntityInfo("Puppy, wagging tail",new Rectangle(48,144,16,16))},
            {131,new EntityInfo("Puppy, sleeping",new Rectangle(144,144,16,16))},
            {132,new EntityInfo("Puppy, barking",new Rectangle(128,144,16,16))},
            {133,new EntityInfo("Jenka",new Rectangle(176,32,16,16))},
            {134,new EntityInfo("Armadillo (enemy)",new Rectangle(224,0,32,16))},
            {135,new EntityInfo("Skeleton (enemy)",new Rectangle(288,32,32,32))},
            {136,new EntityInfo("Puppy, carried",new Rectangle(48,144,16,16))},
            {137,new EntityInfo("Doorway, open",new Rectangle(96,136,32,32))},
            {138,new EntityInfo("Doorway, closed doors",new Rectangle(96,112,32,24))},
            {139,new EntityInfo("Doctor, with crown",new Rectangle(0,128,24,32))},
            {140,new EntityInfo("Frenzied Toroko (boss)",new Rectangle(0,0,32,32))},
            {141,new EntityInfo("Toroko block (projectile)",new Rectangle(288,32,16,16))},
            {142,new EntityInfo("Flower Cub (enemy)",new Rectangle(0,128,16,16))},
            {143,new EntityInfo("Jenka, collapsed",new Rectangle(208,32,47,16))},
            {144,new EntityInfo("Toroko, teleporting in",new Rectangle(32,64,16,16))},
            {145,new EntityInfo("King, struck by lightning",new Rectangle(0,0,0,0))},
            {146,new EntityInfo("Lightning",new Rectangle(304,208,16,32))},
            {147,new EntityInfo("Critter, Hovering Purple (enemy)",new Rectangle(0,96,16,16))},
            {148,new EntityInfo("Critter purple ball (projectile)",new Rectangle(96,96,8,8))},
            {149,new EntityInfo("Moving Block, Horizontal",new Rectangle(16,0,32,32))},
            {150,new EntityInfo("Quote",new Rectangle(0,0,16,16))},
            {151,new EntityInfo("Blue Robot",new Rectangle(192,0,16,16))},
            {152,new EntityInfo("Shutter, stuck (enemy)",new Rectangle(96,64,16,32))},
            {153,new EntityInfo("Gaudi (enemy)",new Rectangle(0,0,24,24))},
            {154,new EntityInfo("Gaudi, dying",new Rectangle(192,0,24,24))},
            {155,new EntityInfo("Gaudi, flying (enemy)",new Rectangle(0,48,24,24))},
            {156,new EntityInfo("Gaudi spitball (projectile)",new Rectangle(96,112,16,16))},
            {157,new EntityInfo("Moving Block, Vertical",new Rectangle(16,0,32,32))},
            {158,new EntityInfo("Fish Missile, Green (enemy)",new Rectangle(0,224,16,16))},
            {159,new EntityInfo("Falling Cat, Monster X",new Rectangle(144,131,48,68))},
            {160,new EntityInfo("Puu Black (boss)",new Rectangle(0,0,40,24))},
            {161,new EntityInfo("Puu Black bubble (projectile)",new Rectangle(0,48,16,16))},
            {162,new EntityInfo("Puu Black, dying",new Rectangle(40,0,40,24))},
            {163,new EntityInfo("Dr. Gero",new Rectangle(192,0,16,16))},
            {164,new EntityInfo("Nurse Hasumi",new Rectangle(224,0,16,16))},
            {165,new EntityInfo("Curly, sleeping",new Rectangle(224,96,16,16))},
            {166,new EntityInfo("Chaba",new Rectangle(146,106,35,21))},
            {167,new EntityInfo("Professor Booster, falling",new Rectangle(304,0,16,16))},
            {168,new EntityInfo("Boulder",new Rectangle(263,57,57,39))},
            {169,new EntityInfo("Balrog, Missiles (boss)",new Rectangle(0,0,40,24))},
            {170,new EntityInfo("Balrog missile (projectile)",new Rectangle(112,96,16,8))},
            {171,new EntityInfo("Fire Whirrr (enemy)",new Rectangle(122,48,32,32))},
            {172,new EntityInfo("Fire Ring (projectile)",new Rectangle(248,48,16,32))},
            {173,new EntityInfo("Gaudi, armor",new Rectangle(0,128,24,24))},
            {174,new EntityInfo("Gaudi Blade (projectile)",new Rectangle(120,80,16,16))},
            {175,new EntityInfo("Gaudi Egg (enemy)",new Rectangle(168,80,24,24))},
            {176,new EntityInfo("Buyobuyo Base (enemy)",new Rectangle(96,128,32,16))},
            {177,new EntityInfo("Buyobuyo (enemy)",new Rectangle(192,144,16,16))},
            {178,new EntityInfo("Core Blade (projectile)",new Rectangle(0,224,16,16))},
            {179,new EntityInfo("Core Wisp (projectile)",new Rectangle(48,224,24,16))},
            {180,new EntityInfo("Curly, AI",new Rectangle(0,96,16,16))},
            {181,new EntityInfo("CurlyAI Machine Gun",new Rectangle(0,0,0,0))},
            {182,new EntityInfo("CurlyAI Polar Star",new Rectangle(0,0,0,0))},
            {183,new EntityInfo("Curly Air Tank bubble",new Rectangle(32,216,23,24))},
            {184,new EntityInfo("Shutter, large",new Rectangle(0,64,32,32))},
            {185,new EntityInfo("Shutter, small",new Rectangle(96,64,16,32))},
            {186,new EntityInfo("Lift Block",new Rectangle(48,48,16,16))},
            {187,new EntityInfo("Fuzz Core (enemy)",new Rectangle(224,104,32,32))},
            {188,new EntityInfo("Fuzz (enemy)",new Rectangle(288,104,16,16))},
            {189,new EntityInfo("Homing Flame (enemy)",new Rectangle(48,224,16,16))},
            {190,new EntityInfo("Surface Robot",new Rectangle(192,32,16,16))},
            {191,new EntityInfo("Water Level",new Rectangle(0,208,16,16))},
            {192,new EntityInfo("Scooter",new Rectangle(225,64,29,16))},
            {193,new EntityInfo("Scooter, crashed",new Rectangle(255,95,64,17))},
            {194,new EntityInfo("Blue Robot, destroyed",new Rectangle(192,112,33,15))},
            {195,new EntityInfo("Grate Mouth",new Rectangle(112,64,15,16))},
            {196,new EntityInfo("Moving Wall, visual",new Rectangle(128,32,16,16))},
            {197,new EntityInfo("Porcupine Fish (enemy)",new Rectangle(0,0,16,16))},
            {198,new EntityInfo("IronHead red ring (projectile)",new Rectangle(240,46,-35,25))},
            {199,new EntityInfo("Underwater current, visual",new Rectangle(224,50,16,14))},
            {200,new EntityInfo("Dragon Zombie (enemy)",new Rectangle(0,0,40,40))},
            {201,new EntityInfo("Dragon Zombie, dead",new Rectangle(200,20,42,16))},
            {202,new EntityInfo("Dragon Zombie fire (projectile)",new Rectangle(182,216,18,23))},
            {203,new EntityInfo("Critter, Hopping Aqua (enemy)",new Rectangle(0,80,16,16))},
            {204,new EntityInfo("Falling Spike, small",new Rectangle(240,144,16,16))},
            {205,new EntityInfo("Falling Spike, large",new Rectangle(128,80,16,32))},
            {206,new EntityInfo("Counter Bomb (enemy)",new Rectangle(80,80,40,44))},
            {207,new EntityInfo("Countdown Speech Bubble (projectile)",new Rectangle(0,144,16,13))},
            {208,new EntityInfo("Basu, 2 (enemy)",new Rectangle(248,80,24,22))},
            {209,new EntityInfo("Basu energy ball, 2 (projectile)",new Rectangle(200,112,16,16))},
            {210,new EntityInfo("Beetle, Follow Aqua (enemy)",new Rectangle(0,112,16,16))},
            {211,new EntityInfo("Spikes (small)",new Rectangle(256,200,16,16))},
            {212,new EntityInfo("Sky Dragon",new Rectangle(160,160,37,32))},
            {213,new EntityInfo("Night Spirit (enemy)",new Rectangle(0,0,48,48))},
            {214,new EntityInfo("Night Spirit balloon (projectile)",new Rectangle(144,48,32,16))},
            {215,new EntityInfo("Sandcroc, White (enemy)",new Rectangle(0,120,48,8))},
            {216,new EntityInfo("Debug Cat",new Rectangle(256,200,16,16))},
            {217,new EntityInfo("Itoh",new Rectangle(144,64,-64,96))},
            {218,new EntityInfo("Core large energy ball (projectile)",new Rectangle(0,0,0,0))},
            {219,new EntityInfo("Generator - Smoke/Underwater Current",new Rectangle(16,0,16,16))},
            {220,new EntityInfo("Shovel Brigade, standing",new Rectangle(0,64,16,16))},
            {221,new EntityInfo("Shovel Brigade, walking",new Rectangle(0,64,16,16))},
            {222,new EntityInfo("Prison bars",new Rectangle(96,168,16,32))},
            {223,new EntityInfo("Momorin",new Rectangle(80,194,16,22))},
            {224,new EntityInfo("Chie",new Rectangle(112,32,16,16))},
            {225,new EntityInfo("Megane",new Rectangle(64,64,16,16))},
            {226,new EntityInfo("Kanpachi, standing",new Rectangle(272,32,24,24))},
            {227,new EntityInfo("Bucket",new Rectangle(208,32,16,16))},
            {228,new EntityInfo("Droll, guarding",new Rectangle(0,0,32,42))},
            {229,new EntityInfo("Red Flowers, small",new Rectangle(0,100,16,12))},
            {230,new EntityInfo("Red Flowers, large",new Rectangle(100,100,28,28))},
            {231,new EntityInfo("Rocket",new Rectangle(176,32,32,16))},
            {232,new EntityInfo("Orangebell (enemy)",new Rectangle(128,0,32,32))},
            {233,new EntityInfo("Orangebell bat (enemy)",new Rectangle(0,0,0,0))},
            {234,new EntityInfo("Red Flowers, picked",new Rectangle(144,100,28,12))},
            {235,new EntityInfo("Midorin (enemy)",new Rectangle(192,100,16,12))},
            {236,new EntityInfo("Gunfish (enemy)",new Rectangle(128,60,26,28))},
            {237,new EntityInfo("Gunfish bubble (projectile)",new Rectangle(312,32,7,8))},
            {238,new EntityInfo("Killer Press (enemy)",new Rectangle(184,200,24,16))},
            {239,new EntityInfo("Cage bars",new Rectangle(96,111,48,33))},
            {240,new EntityInfo("Mimiga (wandering)",new Rectangle(0,64,16,16))},
            {241,new EntityInfo("Critter, Hopping Red (enemy)",new Rectangle(0,0,16,16))},
            {242,new EntityInfo("Bat, Red Wave (enemy)",new Rectangle(0,32,16,16))},
            {243,new EntityInfo("Generator - Red Bat",new Rectangle(0,32,16,16))},
            {244,new EntityInfo("Lava Drop (projectile)",new Rectangle(96,0,6,16))},
            {245,new EntityInfo("Generator - Lava Drop",new Rectangle(96,0,6,16))},
            {246,new EntityInfo("Press, Proximity (enemy)",new Rectangle(144,112,16,24))},
            {247,new EntityInfo("Misery (boss)",new Rectangle(0,0,16,16))},
            {248,new EntityInfo("Misery black ring (projectile)",new Rectangle(0,0,16,16))},
            {249,new EntityInfo("Misery, teleporting out",new Rectangle(32,32,16,16))},
            {250,new EntityInfo("Misery black lightning ball (projectile)",new Rectangle(0,32,16,16))},
            {251,new EntityInfo("Misery black lightning (projectile)",new Rectangle(80,32,16,32))},
            {252,new EntityInfo("Misery black orbiting rings (projectile)",new Rectangle(0,0,0,0))},
            {253,new EntityInfo("Energy Capsule ",new Rectangle(0,64,16,16))},
            {254,new EntityInfo("Helicopter",new Rectangle(0,0,128,64))},
            {255,new EntityInfo("Helicopter Blades",new Rectangle(0,0,0,0))},
            {256,new EntityInfo("Doctor, facing away",new Rectangle(48,160,24,32))},
            {257,new EntityInfo("Red Crystal",new Rectangle(176,32,7,16))},
            {258,new EntityInfo("Mimiga, sleeping",new Rectangle(48,32,16,16))},
            {259,new EntityInfo("Curly, carried",new Rectangle(160,96,16,16))},
            {260,new EntityInfo("Shovel Brigade, caged",new Rectangle(0,64,16,16))},
            {261,new EntityInfo("Chie, caged",new Rectangle(112,32,16,16))},
            {262,new EntityInfo("Chaco, caged",new Rectangle(128,0,16,16))},
            {263,new EntityInfo("Doctor (boss)",new Rectangle(182,160,26,32))},
            {264,new EntityInfo("Doctor red wave (projectile)",new Rectangle(286,0,16,16))},
            {265,new EntityInfo("Doctor red ball (projectile)",new Rectangle(286,0,16,16))},
            {266,new EntityInfo("Doctor red ball, bouncing (projectile)",new Rectangle(286,0,16,16))},
            {267,new EntityInfo("Muscle Doctor (boss)",new Rectangle(0,64,40,48))},
            {268,new EntityInfo("Igor (enemy)",new Rectangle(0,0,40,40))},
            {269,new EntityInfo("Bat, Red Bouncing (enemy)",new Rectangle(232,0,16,16))},
            {270,new EntityInfo("Red Energy/Blood",new Rectangle(168,32,6,8))},
            {271,new EntityInfo("Underwater Block",new Rectangle(128,16,16,16))},
            {272,new EntityInfo("Generator - Underwater Block",new Rectangle(128,16,16,16))},
            {273,new EntityInfo("Droll blade (projectile)",new Rectangle(248,40,24,24))},
            {274,new EntityInfo("Droll (enemy)",new Rectangle(0,0,32,42))},
            {275,new EntityInfo("Puppy, sitting",new Rectangle(48,144,16,16))},
            {276,new EntityInfo("Red Demon (boss)",new Rectangle(0,64,32,38))},
            {277,new EntityInfo("Red Demon blade (projectile)",new Rectangle(128,0,22,24))},
            {278,new EntityInfo("Little Family",new Rectangle(0,120,6,8))},
            {279,new EntityInfo("Falling Block",new Rectangle(0,16,32,32))},
            {280,new EntityInfo("Sue, teleporting in",new Rectangle(112,32,15,16))},
            {281,new EntityInfo("Doctor, Red Energy Form",new Rectangle(183,160,25,32))},
            {282,new EntityInfo("Mini Undead Core, moving",new Rectangle(256,120,64,40))},
            {283,new EntityInfo("Misery, Puppet (enemy)",new Rectangle(0,64,32,32))},
            {284,new EntityInfo("Frenzied Sue (enemy)",new Rectangle(0,128,32,32))},
            {285,new EntityInfo("Undead Core flame spiral (projectile)",new Rectangle(0,224,16,16))},
            {286,new EntityInfo("Undead Core flame spiral trail (projectile)",new Rectangle(232,105,15,14))},
            {287,new EntityInfo("Orange Smoke",new Rectangle(48,224,16,-5))},
            {288,new EntityInfo("Undead Core exploding rock (projectile)",new Rectangle(232,71,16,17))},
            {289,new EntityInfo("Critter, Hopping Orange, disappears (enemy)",new Rectangle(160,32,16,16))},
            {290,new EntityInfo("Bat, Fast Orange, disappears (enemy)",new Rectangle(112,32,16,16))},
            {291,new EntityInfo("Mini Undead Core, stationary",new Rectangle(256,0,64,40))},
            {292,new EntityInfo("Quake",new Rectangle(0,0,0,0))},
            {293,new EntityInfo("Undead Core large energy ball (projectile)",new Rectangle(280,200,40,40))},
            {294,new EntityInfo("Generator - Falling Block",new Rectangle(0,16,32,32))},
            {295,new EntityInfo("Cloud",new Rectangle(144,112,48,30))},
            {296,new EntityInfo("Generator - Cloud",new Rectangle(0,0,0,0))},
            {297,new EntityInfo("Sue, on sky dragon",new Rectangle(0,0,0,0))},
            {298,new EntityInfo("Doctor, without crown",new Rectangle(100,160,20,32))},
            {299,new EntityInfo("Balrog/Misery in bubble",new Rectangle(0,0,48,48))},
            {300,new EntityInfo("Demon Crown",new Rectangle(192,80,16,16))},
            {301,new EntityInfo("Fish Missile, Orange (enemy)",new Rectangle(143,0,16,16))},
            {302,new EntityInfo("Camera Focus Marker",new Rectangle(0,0,0,0))},
            {303,new EntityInfo("UNUSED/DOES NOTHING (Old Version of Curly's Gun?)",new Rectangle(0,0,0,0))},
            {304,new EntityInfo("Gaudi, sitting",new Rectangle(0,176,24,16))},
            {305,new EntityInfo("Puppy, small",new Rectangle(160,150,16,10))},
            {306,new EntityInfo("Balrog, nurse",new Rectangle(240,100,40,28))},
            {307,new EntityInfo("Santa, caged",new Rectangle(0,32,16,16))},
            {308,new EntityInfo("Stumpy (enemy)",new Rectangle(126,114,16,16))},
            {309,new EntityInfo("Bute, flying, 1 (enemy)",new Rectangle(0,0,16,16))},
            {310,new EntityInfo("Bute, sword (enemy)",new Rectangle(32,0,22,16))},
            {311,new EntityInfo("Bute, archer (enemy)",new Rectangle(0,32,24,24))},
            {312,new EntityInfo("Bute arrow (projectile)",new Rectangle(0,160,16,16))},
            {313,new EntityInfo("Ma Pignon (boss)",new Rectangle(128,0,16,16))},
            {314,new EntityInfo("Ma Pignon rock (projectile)",new Rectangle(0,0,0,0))},
            {315,new EntityInfo("Ma Pignon clone (enemy)",new Rectangle(0,0,0,0))},
            {316,new EntityInfo("Bute, dying",new Rectangle(271,41,22,14))},
            {317,new EntityInfo("Mesa (enemy)",new Rectangle(0,80,32,38))},
            {318,new EntityInfo("Mesa, dying",new Rectangle(224,80,32,40))},
            {319,new EntityInfo("Mesa block (projectile)",new Rectangle(0,0,0,0))},
            {320,new EntityInfo("Curly, carried, shooting",new Rectangle(0,96,16,16))},
            {321,new EntityInfo("Curly Nemesis bullet spawner (projectile)",new Rectangle(0,0,0,0))},
            {322,new EntityInfo("Deleet (enemy)",new Rectangle(160,216,22,23))},
            {323,new EntityInfo("Bute, spawned (enemy)",new Rectangle(216,32,16,22))},
            {324,new EntityInfo("Generator - Bute",new Rectangle(216,32,16,22))},
            {325,new EntityInfo("Heavy Press lightning (projectile)",new Rectangle(238,0,17,96))},
            {326,new EntityInfo("Itoh/Sue turning human",new Rectangle(97,128,63,26))},
            {327,new EntityInfo("Itoh/Sue 'Ah Choo!'",new Rectangle(0,0,0,0))},
            {328,new EntityInfo("Transmogrifier",new Rectangle(96,0,32,48))},
            {329,new EntityInfo("Building fan",new Rectangle(48,0,16,16))},
            {330,new EntityInfo("Rolling (enemy)",new Rectangle(144,136,16,16))},
            {331,new EntityInfo("Ballos bone (projectile)",new Rectangle(288,80,16,16))},
            {332,new EntityInfo("Ballos shockwave (projectile)",new Rectangle(0,0,0,0))},
            {333,new EntityInfo("Ballos lightning target (projectile)",new Rectangle(260,0,11,240))},
            {334,new EntityInfo("Water Drop, 2",new Rectangle(0,0,0,0))},
            {335,new EntityInfo("Ikachan",new Rectangle(0,16,16,16))},
            {336,new EntityInfo("Generator - Ikachan",new Rectangle(0,16,16,16))},
            {337,new EntityInfo("Numahachi",new Rectangle(256,112,32,38))},
            {338,new EntityInfo("Green Devil (enemy)",new Rectangle(283,0,19,16))},
            {339,new EntityInfo("Generator - Green Devil",new Rectangle(0,0,0,0))},
            {340,new EntityInfo("Ballos (boss)",new Rectangle(0,0,44,40))},
            {341,new EntityInfo("Ballos 1 head",new Rectangle(0,0,0,0))},
            {342,new EntityInfo("Ballos 3 eyeball (enemy)",new Rectangle(0,0,0,0))},
            {343,new EntityInfo("Ballos 2 cutscene",new Rectangle(0,0,0,0))},
            {344,new EntityInfo("Ballos 2 eyes",new Rectangle(0,0,0,0))},
            {345,new EntityInfo("Ballos 3 skull (projectile)",new Rectangle(128,176,16,16))},
            {346,new EntityInfo("Ballos 4 orbiting platform",new Rectangle(0,0,0,0))},
            {347,new EntityInfo("Hoppy (enemy)",new Rectangle(256,48,16,16))},
            {348,new EntityInfo("Ballos 4 spikes",new Rectangle(126,151,18,25))},
            {349,new EntityInfo("Statue",new Rectangle(32,100,32,44))},
            {350,new EntityInfo("Bute, flying archer (enemy)",new Rectangle(49,162,21,20))},
            {351,new EntityInfo("Statue (enemy)",new Rectangle(0,0,0,0))},
            {352,new EntityInfo("Credits-Drop Universal NPC",new Rectangle(224,32,14,16))},
            {353,new EntityInfo("Bute, flying, 2 (enemy)",new Rectangle(198,158,18,18))},
            {354,new EntityInfo("Invisible deathtrap wall",new Rectangle(0,0,0,0))},
            {355,new EntityInfo("Balrog, crashing through wall",new Rectangle(0,0,0,0))},
            {356,new EntityInfo("Balrog, rescue",new Rectangle(0,0,40,24))},
            {357,new EntityInfo("Puppy, ghost",new Rectangle(224,137,16,16))},
            {358,new EntityInfo("Misery, tall, wind",new Rectangle(208,10,14,22))},
            {359,new EntityInfo("Generator - Water Drop",new Rectangle(72,16,2,2))},
            {360,new EntityInfo("Text, 'Thank You!'",new Rectangle(0,176,48,7))},
        });
    }
}

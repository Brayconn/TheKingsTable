using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

namespace CaveStoryModdingFramework.Profiles
{
    public class SavedWeapon
    {
        public int Type { get; set; } = 0;
        public int Level { get; set; } = 0;
        public int EXP { get; set; } = 0;
        public int MaxAmmo { get; set; } = 0;
        public int CurrentAmmo { get; set; } = 0;

        public SavedWeapon(Stream stream)
        {
            using(var br = new BinaryReader(stream, Encoding.Default, true))
            {
                Type = br.ReadInt32();
                Level = br.ReadInt32();
                EXP = br.ReadInt32();
                MaxAmmo = br.ReadInt32();
                CurrentAmmo = br.ReadInt32();
            }
        }
    }
    public class SavedTeleporter
    {
        public int Index { get; set; }
        public int Event { get; set; }

        public SavedTeleporter(Stream stream)
        {
            using(var br = new BinaryReader(stream, Encoding.Default, true))
            {
                Index = br.ReadInt32();
                Event = br.ReadInt32();
            }
        }
    }
    public class Profile
    {
        const string DefaultHeader = "Do041220";
        const string DefaultFlagHeader = "FLAG";

        public bool HeaderValid => Header.SequenceEqual(Encoding.ASCII.GetBytes(DefaultHeader));
        public byte[] Header { get; set; }

        public int CurrentMap { get; set; }
        public int CurrentSong { get; set; }

        public int XPosition { get; set; }
        public int YPosition { get; set; }
        public int Direction { get; set; }
        public short MaxHP { get; set; }
        public short WhimsicalStars { get; set; }
        public short CurrentHP { get; set; }
        //2 byte buffer
        public int CurrentWeapon { get; set; }
        public int CurrentInventoryItem { get; set; }
        public int EquippedItems { get; set; }
        public int UNI { get; set; }
        /// <summary>
        /// unused?
        /// </summary>
        public int FrameCounter { get; set; }

        const int DefaultWeaponArrayLength = 8;
        public SavedWeapon[] SavedWeapons { get; } = new SavedWeapon[DefaultWeaponArrayLength];
        
        const int DefaultInventorySize = 32;
        public int[] Inventory { get; } = new int[DefaultInventorySize];

        const int DefaultTeleportLength = 8;
        public SavedTeleporter[] TeleportDestinations { get; } = new SavedTeleporter[DefaultTeleportLength];

        public const int MapFlagLength = 128;
        public byte[] MapFlags { get; } = new byte[MapFlagLength];

        public const int FlagLength = 1000;
        public bool FlagHeaderValid => FlagHeader.SequenceEqual(Encoding.ASCII.GetBytes(DefaultFlagHeader));
        public byte[] FlagHeader;
        public BitArray Flags { get; } = new BitArray(FlagLength);

        public Profile(Stream stream)
        {
            using(var br = new BinaryReader(stream, Encoding.Default, true))
            {
                Header = br.ReadBytes(DefaultHeader.Length);

                CurrentMap = br.ReadInt32();
                CurrentSong = br.ReadInt32();

                XPosition = br.ReadInt32();
                YPosition = br.ReadInt32();
                Direction = br.ReadInt32();
                MaxHP = br.ReadInt16();
                WhimsicalStars = br.ReadInt16();
                CurrentHP = br.ReadInt16();
                stream.Position += 2; //buffer?
                CurrentWeapon = br.ReadInt32();
                CurrentInventoryItem = br.ReadInt32();
                EquippedItems = br.ReadInt32();
                UNI = br.ReadInt32();
                FrameCounter = br.ReadInt32();

                for (int i = 0; i < DefaultWeaponArrayLength; i++)
                    SavedWeapons[i] = new SavedWeapon(stream);

                for (int i = 0; i < DefaultInventorySize; i++)
                    Inventory[i] = br.ReadInt32();

                for (int i = 0; i < DefaultTeleportLength; i++)
                    TeleportDestinations[i] = new SavedTeleporter(stream);

                for (int i = 0; i < MapFlagLength; i++)
                    MapFlags[i] = br.ReadByte();

                FlagHeader = br.ReadBytes(DefaultFlagHeader.Length);

                var b = br.ReadBytes(FlagLength);
                Flags = new BitArray(b);
            }
        }
    }
    public class CSPlusSubProfile : Profile
    {
        //4 byte padding
        public DateTime Timestamp { get; set; }
        public byte Difficulty { get; set; }
        public CSPlusSubProfile(Stream stream) : base(stream)
        {
            //padding
            stream.Position += 4;
            using(var br = new BinaryReader(stream, Encoding.Default, true))
            {
                Timestamp = DateTime.FromFileTimeUtc(br.ReadInt64());
                Difficulty = br.ReadByte();
                stream.Position += 0xF; //padding
            }
        }
    }
    public class CSPlusProfile
    {
        public const int PerModSlots = 3;
        public const int TotalModSlots = 25;
        public CSPlusSubProfile[] MainProfiles { get; } = new CSPlusSubProfile[PerModSlots];
        public CSPlusSubProfile[] CurlyStoryProfiles { get; } = new CSPlusSubProfile[PerModSlots];
        public CSPlusSubProfile[][] ModProfiles { get; } = new CSPlusSubProfile[TotalModSlots][];

        public byte ActiveMainProfile { get; set; } //0x1 = 1, 0x2 = 2, 0x4 = 3
        public byte ActiveCurlyProfile { get; set; } //see above
        public byte[] ActiveModProfile { get; } = new byte[TotalModSlots]; //see above

        public byte MusicVolume { get; set; }
        public byte SoundVolume { get; set; }
        public byte SeasonalArt { get; set; }
        public byte Soundtrack { get; set; }
        public byte Graphics { get; set; }
        public byte Language { get; set; }
        public byte BeatenHell { get; set; }
        //7 byte buffer

        public const int ChallengeTimesCount = 26;
        public int[] ChallengeTimes { get; } = new int[ChallengeTimesCount];

        public CSPlusProfile()
        {
            for (int i = 0; i < TotalModSlots; i++)
                ModProfiles[i] = new CSPlusSubProfile[PerModSlots];
        }

        public CSPlusProfile(Stream stream) : this()
        {
            using(var br = new BinaryReader(stream, Encoding.Default, true))
            {
                for (int i = 0; i < PerModSlots; i++)
                    MainProfiles[i] = new CSPlusSubProfile(stream);

                for (int i = 0; i < PerModSlots; i++)
                    CurlyStoryProfiles[i] = new CSPlusSubProfile(stream);

                for (int i = 0; i < TotalModSlots; i++)
                    for(int j = 0; j < PerModSlots; j++)
                        ModProfiles[i][j] = new CSPlusSubProfile(stream);

                ActiveMainProfile = br.ReadByte();
                ActiveCurlyProfile = br.ReadByte();
                ActiveModProfile = br.ReadBytes(TotalModSlots);

                MusicVolume = br.ReadByte();
                SoundVolume = br.ReadByte();
                SeasonalArt = br.ReadByte();
                Soundtrack = br.ReadByte();
                Graphics = br.ReadByte();
                Language = br.ReadByte();
                BeatenHell = br.ReadByte();
                stream.Position += 7;
            }
        }
    }
    public class CSPlusSwitchProfile
    {

    }
}

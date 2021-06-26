using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaveStoryModdingFramework.Compatability
{
    public sealed class BitStream : Stream
    {
        public Stream BaseStream;
        public BitStream(Stream stream)
        {
            BaseStream = stream;
        }
        public override bool CanRead => BaseStream.CanRead;

        public override bool CanSeek => BaseStream.CanSeek;

        public override bool CanWrite => BaseStream.CanWrite;

        public override long Length => BaseStream.Length;
        public long BitLength => BaseStream.Length * 8;

        public override long Position { get => BaseStream.Position; set => BaseStream.Position = value; }

        public long AbsoluteBitPosition
        {
            get => (Position * 8) + BitPosition;
            set
            {
                this.BaseStream.Position = value / 8;
                BitPosition = (int)(value % 8);
            }
        }
        int bitIndex = 0;
        public int BitPosition
        {
            get => bitIndex;
            set => bitIndex = (8+value) % 8;
        }
        
        public static int ToNearestPowerOfTwo(int v)
        {
            v--;
            for(int i = 1; i != 0; i <<= 1)
                v |= v >> i;
            return ++v;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer">output buffer</param>
        /// <param name="offset">Where to store the data in the provided buffer</param>
        /// <param name="bitLength">Amount of bits to read</param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int bitLength)
        {
            //don't need to read 0 bits...
            if (bitLength == 0)
                return 0;
                        
            var byteLength = bitLength / 8;
            if (BitPosition != 0 || bitLength % 8 != 0)
                byteLength++;
            var buff = new byte[byteLength];
            BaseStream.Read(buff, 0, byteLength);
            if (BitPosition != 0)
                buff.ShiftLeft(BitPosition);

            Array.Copy(buff, 0, buffer, offset, buff.Length);

            //subtract any extra bytes we had to read
            if (BitPosition != 0 || bitLength % 8 != 0)
                Position--;
            //update the bit position
            BitPosition += (bitLength % 8);


            return bitLength;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public byte[] Read(int bitLength)
        {
            var byteLength = bitLength / 8;
            if (BitPosition != 0 || bitLength % 8 != 0)
                byteLength++;
            var buff = new byte[byteLength];
            //Read(buff, 0, bitLength);
            Read2(buff, bitLength);
            return buff;
        }
        public bool ReadBit()
        {
            var b = BaseStream.ReadByte();
            BaseStream.Position--;
            bool output = (b & (1 << BitPosition)) != 0;
            AbsoluteBitPosition++;
            return output;
        }
        public new byte ReadByte()
        {
            return Read(8)[0];
        }

        public ushort ReadUInt16()
        {
            return BitConverter.ToUInt16(Read(16), 0);
        }

        public uint ReadUInt32()
        {
            return BitConverter.ToUInt32(Read(32), 0);
        }

        public ulong ReadUInt64()
        {
            return BitConverter.ToUInt64(Read(64),0);
        }

        public void Read2(byte[] output, int length)
        {
            if(length != 0)
            {
                //var bitnum = AbsoluteBitPosition;
                var oldpos = Position;
                var index = 0;
                var next_shift_amount = BitPosition;
                var bytesLeft = (length + 7) >> 3;
                var this_byte = BaseStream.ReadByte();
                if(bytesLeft != 0)
                {
                    do
                    {
                        var next_byte = BaseStream.ReadByte();
                        output[index++] = (byte)(this_byte >> next_shift_amount | next_byte << ((8 - next_shift_amount) & 0x1f));
                        this_byte = next_byte;
                        bytesLeft--;
                    } while (bytesLeft != 0);
                    Position = oldpos;
                    AbsoluteBitPosition += length;
                }
            }
        }

        public ulong ReadThing(int bitLength)
        {
            var buff = new byte[sizeof(ulong)];
            Read2(buff, bitLength);
            return BitConverter.ToUInt64(buff,0);
        }


        public ulong ReadRangedInt(int min, int max)
        {
            if (min != max)
            {
                var length = CaveStoryMultiplayer.TrailingZeros(ToNearestPowerOfTwo(max - min+1));
                var value = ReadThing(length);
                if (length != 0x20)
                    value &= (ulong)((1 << ((byte)length & 0x1f)) - 1);
                length = min + (int)value;
                if (min <= length)
                {
                    min = length;
                    if(max < length)
                    {
                        min = max;
                    }
                }
                return (ulong)min;
            }
            else
                return 0;            
        }

        public string ReadString(int maxAmount)
        {
            var strLenLen = CaveStoryMultiplayer.TrailingZeros(ToNearestPowerOfTwo(maxAmount+1));
            var strLen = ReadThing(strLenLen);
            //filtering the output to the requested number of bits
            if (strLenLen != 0x20)
                strLen &= (ulong)((1 << ((byte)strLenLen & 0x1f)) - 1);
            //some 0 check goes here
            if ((ulong)maxAmount < strLen)
                strLen = (ulong)maxAmount;

            var str = Read((int)strLen * 8);

            return Encoding.ASCII.GetString(str);
        }

        public override void Flush()
        {
            BaseStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return BaseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            BaseStream.SetLength(value);
        }
    }

    public static class CaveStoryMultiplayer
    {
        public class CSMPStageEntry : Stages.StageEntry
        {
            public int AreaIndex { get; set; }
            public bool FocusCenterX { get; set; }
            public bool FocusCenterY { get; set; }
        }

        public static string AsBinary(this byte[] data)
        {
            string @out = "";
            foreach (var b in data)
                @out += Convert.ToString(b, 2).PadLeft(8, '0')+" ";
            return @out;
        }

        public static void ShiftLeft(this byte[] ba, int amount)
        {
            var bef = ba.AsBinary();
            //shift the first byte over normally
            ba[0] <<= amount;
            for (int i = 0; i < ba.Length - 1; i++)
            {
                //set any bits that are about to be shifted in from the next byte
                ba[i] |= (byte)(ba[i + 1] >> 8-amount);
                //shift the next byte
                ba[i + 1] <<= amount;
            }
            //get rid of any extra bits on the last byte
            ba[ba.Length - 1] &= (byte)~(0xFF >> 8-amount);
            var af = ba.AsBinary();
        }

        enum BuildTypes
        {
            Debug = 0,
            Release,
            MAX
        }

        public static void ReadPXMOD(string path)
        {
            using(var bs = new BitStream(new FileStream(path, FileMode.Open, FileAccess.Read)))
            {
                var header = Encoding.ASCII.GetString(bs.Read(5 * 8));
                if(header == "PXMOD")
                {
                    var fileVersion = bs.ReadByte();

                    int buildType = 0;
                    if(fileVersion >= 2)
                    {
                        buildType = (int)bs.ReadRangedInt(0, 2);
                    }

                    var maybe_header = bs.ReadString(0x100);
                    var author = bs.ReadString(0x100);

                    ulong modVersion = 0;
                    modVersion |= bs.ReadRangedInt(0, 9);
                    modVersion |= bs.ReadRangedInt(0, 9) << 8;
                    modVersion |= bs.ReadRangedInt(0, 9) << 16;
                    modVersion |= bs.ReadRangedInt(0, 9) << 24;

                    var backgroundsCount = bs.ReadUInt32();
                    var tilesetCount = bs.ReadUInt32();
                    var spritesheetCount = bs.ReadUInt32();
                    var stageCount = bs.ReadUInt16();
                    var areasCount = 0;
                    if(fileVersion >= 2)
                    {
                        areasCount = (int)bs.ReadUInt32();
                    }
                    uint musicCount = 0;
                    if(fileVersion >= 3)
                    {
                        musicCount = bs.ReadUInt32();
                    }
                    if(fileVersion >= 4)
                    {
                        var bulletCount = bs.ReadUInt32();
                        var weaponCount = bs.ReadUInt32();
                    }
                    if(fileVersion >= 5)
                    {
                        var npcCount = bs.ReadUInt32();
                    }
                    //it inits a bunch of stuff
                    if (fileVersion >= 0xb)
                    {
                        var coreDrownFlag = bs.ReadRangedInt(0, 9999);
                        var coreDrownEvent = bs.ReadRangedInt(0, 9999);
                        var everyone_died = bs.ReadRangedInt(0, 9999);
                        var drown_event = bs.ReadRangedInt(0, 9999);
                        var death_event = bs.ReadRangedInt(0, 9999);
                    }
                    if(fileVersion >= 0xc)
                    {
                        var titleMusic = bs.ReadRangedInt(0, (int)musicCount);
                        var netMenuMusic = bs.ReadRangedInt(0, (int)musicCount);
                    }

                    if (fileVersion >= 9)
                    {
                        var allow_carrying = bs.ReadBit();
                        var allow_agility = bs.ReadBit();
                        var allow_fishing = bs.ReadBit();
                        var draw_hp_bar = bs.ReadBit();
                        if (draw_hp_bar)
                        {
                            var draw_hp = bs.ReadBit();
                        }
                        var draw_weapons = bs.ReadBit();
                        var draw_ammo = bs.ReadBit();
                        var draw_exp_bar = bs.ReadBit();
                        if (draw_exp_bar)
                        {
                            var draw_level = bs.ReadBit();
                        }
                        //aqua barrier style iirc
                        var use_collectables = bs.ReadBit();
                        if (use_collectables && buildType == (int)BuildTypes.Debug)
                        {
                            var surface_id = bs.ReadUInt16();
                            var sprite_rect_left = bs.ReadUInt32();
                            var sprite_rect_top = bs.ReadUInt32();
                            var sprite_rect_right = bs.ReadUInt32();
                            var sprite_rect_bottom = bs.ReadUInt32();
                        }
                    }
                    var startMap = bs.ReadRangedInt(0, stageCount);
                    var startX = bs.ReadUInt16();
                    var startY = bs.ReadUInt16();
                    var startEvent = bs.ReadRangedInt(0, 9999);
                    var startHP = bs.ReadUInt16();
                    var startMaxHP = bs.ReadUInt16();
                    var startDirection = bs.ReadRangedInt(0, 7);
                    var startFlags = bs.ReadUInt32();

                    var titleMap = bs.ReadRangedInt(0, stageCount);
                    var titleEvent = bs.ReadRangedInt(0, 9999);
                    var titleX = bs.ReadUInt16();
                    var titleY = bs.ReadUInt16();

                    var backgroundNames = new List<string>((int)backgroundsCount);
                    for (int i = 0; i < backgroundsCount; i++)
                        backgroundNames.Add(bs.ReadString(0x100));

                    var tilesetNames = new List<string>((int)tilesetCount);
                    for (int i = 0; i < tilesetCount; i++)
                        tilesetNames.Add(bs.ReadString(0x100));

                    var spritesheetNames = new List<string>((int)spritesheetCount);
                    for (int i = 0; i < spritesheetCount; i++)
                        spritesheetNames.Add(bs.ReadString(0x100));

                    var stages = new List<CSMPStageEntry>((int)stageCount);
                    for(int i = 0; i < stageCount; i++)
                    {
                        var entry = new CSMPStageEntry();
                        entry.Filename = bs.ReadString(0x10);
                        entry.MapName = bs.ReadString(0x22);
                        entry.BackgroundName = backgroundNames[(int)bs.ReadRangedInt(0, backgroundNames.Count)];
                        entry.Spritesheet1 = spritesheetNames[(int)bs.ReadRangedInt(0, spritesheetNames.Count)];
                        entry.Spritesheet2 = spritesheetNames[(int)bs.ReadRangedInt(0, spritesheetNames.Count)];
                        entry.TilesetName = tilesetNames[(int)bs.ReadRangedInt(0, tilesetNames.Count)];
                        if(fileVersion >= 2)
                        {
                            entry.AreaIndex = (int)bs.ReadRangedInt(0, areasCount);
                        }
                        entry.BackgroundType = (long)bs.ReadRangedInt(0, 7);
                        entry.BossNumber = (long)bs.ReadRangedInt(0, 9);
                        if(fileVersion >= 6)
                        {
                            entry.FocusCenterX = bs.ReadBit();
                        }
                        if(fileVersion >= 7)
                        {
                            entry.FocusCenterY = bs.ReadBit();
                        }

                        stages.Add(entry);
                    }

                    //TODO
                    
                    //read all areas

                    //read music

                    //read bullets

                    //read weapons

                    //read npcs
                }
            }
        }

        public static void ReadPXCHAR(string path)
        {
            using (var br = new BitStream(new FileStream(path, FileMode.Open, FileAccess.Read)))
            {
                var header = Encoding.ASCII.GetString(br.Read(6 * 8));
                if (header == "PXCHAR")
                {
                    var version = br.ReadByte();

                    var name = br.ReadString(0x100);
                    var author = br.ReadString(0x100);

                    var frame_count = br.ReadByte();
                    if (version >= 2)
                    {
                        var face_count = br.ReadByte();
                    }
                    if(version >= 3)
                    {
                        var fx = br.ReadUInt32();
                        var fy = br.ReadUInt32();
                        var fx2 = br.ReadUInt32();
                        var fy2 = br.ReadUInt32();
                    }
                    


                }
            }

        }

        public static int TrailingZeros(int x)
        {
            var trailLut = new int[] {
                0, 1, 28, 2, 29, 14, 24, 3, 30, 22, 20, 15, 25, 17, 4, 8,
                31, 27, 13, 23, 21, 19, 16, 7, 26, 12, 18, 6, 11, 5, 10, 9
              };
            uint step = (uint)(x & -x);
            uint step2 = step * 0x77cb531;
            uint step3 = step2 >> 27;
            return trailLut[step3];
        }

        public static int LeadingZeros(int x)
        {
            const int numIntBits = sizeof(int) * 8; //compile time constant
                                                    //do the smearing
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            //count the ones
            x -= x >> 1 & 0x55555555;
            x = (x >> 2 & 0x33333333) + (x & 0x33333333);
            x = (x >> 4) + x & 0x0f0f0f0f;
            x += x >> 8;
            x += x >> 16;
            return numIntBits - (x & 0x0000003f); //subtract # of 1s from 32
        }


        public static uint ReadUInt24(this BinaryReader br)
        {
            var val = (br.ReadUInt32() & 0x00_FF_FF_FF);
            br.BaseStream.Position--;
            return val;
        }
        public static ulong ReadUInt48(this BinaryReader br)
        {
            var val = (br.ReadUInt64() & 0x00_FF_FF_FF_FF_FF_FF_FF);
            br.BaseStream.Position--;
            return val;
        }
        public static ulong ReadShiftedInt(this BinaryReader br)
        {
            return br.ReadShiftedInt(br.ReadByte());
        }
        public static ulong ReadShiftedInt(this BinaryReader br, byte length)
        {
            var shift = 0;
            var shiftZero = shift == 0;
            if(shift == 0)
            {
                switch (length)
                {
                    case 0:
                        return 0;
                    case 8:
                        return br.ReadByte();
                    case 16:
                        return br.ReadUInt16();
                    case 24:
                        return br.ReadUInt24();
                    case 32:
                        return br.ReadUInt32();
                    case 48:
                        return br.ReadUInt48();
                    case 64:
                        return br.ReadUInt64();
                }
            }
            if(1 <= length && length <= 8)
            {
                return (ulong)((br.ReadUInt16() >> shift) & ((1 << length) - 1));
            }
            else if (9 <= length && length <= 16)
            {
                return (ulong)((br.ReadUInt24() >> shift) & ((1 << length) - 1));
            }
            else if (17 <= length && length <= 24)
            {
                return (ulong)((br.ReadUInt32() >> shift) & ((1 << length) - 1));
            }
            else if (25 <= length && length <= 40)
            {
                return (br.ReadUInt48() >> shift) & (ulong)((1 << length) - 1);
            }
            else if (41 <= length && length <= 56)
            {
                return (br.ReadUInt64() >> shift) & (ulong)((1 << length) - 1);
            }
            else
            {
                throw new ArgumentException();
            }
            /*pxchar:
             * header ("PXCHAR")
             * version (byte)
             * character name (string?)
             * description (string)
             * 
             */
        }
    }
}

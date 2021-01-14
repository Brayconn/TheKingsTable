using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CaveStoryModdingFramework
{
    static class Extensions
    {
        public static T Clamp<T>(this T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
                return min;
            if (value.CompareTo(max) > 0)
                return max;

            return value;
        }

        /// <summary>
        /// Reinterprets any string as its ASCII counterpart
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToASCII(this string value)
        {
            return Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(value));
        }

        //TODO this should really go in filephoenix.extensions at some point
        internal static ReadOnlyDictionary<Type, Func<BinaryReader, dynamic>> BinaryReaderDict = new ReadOnlyDictionary<Type, Func<BinaryReader, dynamic>>(new Dictionary<Type, Func<BinaryReader, dynamic>>()
        {
            { typeof(byte), (BinaryReader br) => br.ReadByte() },
            { typeof(sbyte), (BinaryReader br) => br.ReadSByte() },
            { typeof(short), (BinaryReader br) => br.ReadInt16() },
            { typeof(ushort), (BinaryReader br) => br.ReadUInt16() },
            { typeof(int), (BinaryReader br) => br.ReadInt32() },
            { typeof(uint), (BinaryReader br) => br.ReadUInt32() },
            { typeof(long), (BinaryReader br) => br.ReadInt64() },
            { typeof(ulong), (BinaryReader br) => br.ReadUInt64() },
            { typeof(float), (BinaryReader br) => br.ReadSingle() },
            { typeof(double), (BinaryReader br) => br.ReadDouble() },
            { typeof(decimal), (BinaryReader br) => br.ReadDecimal() },            
        });
        public static T Read<T>(this BinaryReader br) where T : struct
        {
            return BinaryReaderDict[typeof(T)](br);
        }
        public static dynamic Read(this BinaryReader br, Type T)
        {
            return BinaryReaderDict[T](br);
        }

        public static string ReadString(this BinaryReader br, int length, Encoding encoding)
        {
            return encoding?.GetString(br.ReadBytes(length).TakeWhile(x => x != 0).ToArray());
        }

        public static void BufferCopy(Array source, Array dest, int destIndex, int maxCopy)
        {
            if(maxCopy > 0)
                Array.Copy(source, 0, dest, destIndex, Math.Min(source.Length, maxCopy));
        }

        public static byte[] ConvertAndGetBytes(object obj, Type T)
        {
            dynamic conv = Convert.ChangeType(obj, T);
            return (conv.GetType() == typeof(sbyte) || conv.GetType() == typeof(byte))
                ? new byte[] { (byte)conv }
                : BitConverter.GetBytes(conv);
        }

        public static T?[] ToNullable<T>(this T[] array) where T : struct
        {
            T?[] output = new T?[array.Length];
            for (int i = 0; i < array.Length; i++)
                output[i] = array[i];
            return output;
        }
        public static T[] ToNonNullable<T>(this T?[] array) where T : struct
        {
            T[] output = new T[array.Length];
            for (int i = 0; i < array.Length; i++)
                output[i] = array[i] ?? default;
            return output;
        }
    }
}

using System;
using System.Linq;
using System.Text;

namespace CaveStoryModdingFramework.TSC
{
    public static class FlagConverter
    {
        /// <summary>
        /// The address of the first flag 
        /// </summary>
        public const int FirstFlagAddress = 0x49DDA0;

        public const int LowestRealFlagNumber = -195536;

        public const int HighestRealFlagNumber = 87769;

        #region samples
        /* Extremely basic sample methods. Useful for understanding the process
        public static int[] FlagToRealValueTEST(char[] flag)
        {
            int[] number = new int[4];

            number[0] = (flag[0] - 0x30) * 1000;
            number[1] = (flag[1] - 0x30) * 100;
            number[2] = (flag[2] - 0x30) * 10;
            number[3] = (flag[3] - 0x30) * 1;

            //The sum of all numbers in "number" is the "real flag value"
            return number;
        }
        //In practice, undoing the sum to arrive at an int[] that would work in this exact method is the hard part
        public static char[] RealValueToFlagTEST(int[] number)
        {
            char[] flag = new char[4];

            flag[0] = (char)(number[0] / 1000 + 0x30);
            flag[1] = (char)(number[1] / 100 + 0x30);
            flag[2] = (char)(number[2] / 10 + 0x30);
            flag[3] = (char)(number[3] / 1 + 0x30);

            return flag;
        }
        //*/
        #endregion

        #region flag

        /// <summary>
        /// Get the address of a given flag
        /// </summary>
        /// <param name="flag">The flag number</param>
        /// <returns>The address of the </returns>
        public static int FlagToAddress(string flag, int firstFlagAddress = FirstFlagAddress)
        {
            return (FlagToRealValue(flag) / 8) + firstFlagAddress;
        }
        /// <summary>
        /// Get the "real" value of the given TSC flag
        /// </summary>
        /// <param name="flag">The TSC flag</param>
        /// <returns>The "real" flag number</returns>
        public static int FlagToRealValue(string flag)
        {
            //Input sanitation
            //sbyte[] input = flag.Select(x => (sbyte)x).ToArray();
            sbyte[] input = Array.ConvertAll(flag.ToCharArray(), x => (sbyte)x);

            int number = 0;
            for (int i = 0; i < 4; i++)
                number += (input[i] - 0x30) * (1000 / (int)Math.Pow(10, i));
            return number;
        }

        #endregion

        #region Real Value

        /// <summary>
        /// Get the TSC flag for the "real" flag number
        /// </summary>
        /// <param name="number">The "real" flag number</param>
        /// <returns>The TSC flag</returns>
        public static string RealValueToFlag(int number)
        {
            string flag = "";
            for (int i = 0; i < 4; i++)
            {
                int decimalPlace = 1000 / (int)Math.Pow(10, i);
                //This value MUST be clamped to the range of numbers that FlagToRealValue would produce
                var thisC = (number / decimalPlace).Clamp(sbyte.MinValue - 0x30, sbyte.MaxValue - 0x30);
                //Then it can be used safely
                number -= decimalPlace * thisC;
                flag += (char)(byte)(thisC + 0x30);
            }
            return flag;
        }
        /// <summary>
        /// Get the address of a "real" flag number
        /// </summary>
        /// <param name="number">The real value of the flag</param>
        /// <returns>Address of the given flag</returns>
        public static int RealValueToAddress(int number, int firstFlagAddress = FirstFlagAddress)
        {
            return (number / 8) + firstFlagAddress;
        }

        #endregion

        #region address
        /// <summary>
        /// Get the first flag corresponding to the given address
        /// </summary>
        /// <param name="address">Address in the exe</param>
        /// <returns>First TSC flag corresponding to this address</returns>
        public static string AddressToFlag(int address, int firstFlagAddress = FirstFlagAddress)
        {
            return RealValueToFlag((address - firstFlagAddress) * 8);
        }
        /// <summary>
        /// Get the real value of a given address
        /// </summary>
        /// <param name="address">Address in the exe</param>
        /// <returns>Real value of this address</returns>
        public static int AddressToRealValue(int address, int firstFlagAddress = FirstFlagAddress)
        {
            return (address - firstFlagAddress) * 8;
        }
        #endregion
    }
}
using System.Collections;

namespace PolimasterIrDADevicesManagerGUI.Utils
{
    public sealed class ByteHelper
    {

        public static IEnumerable<bool> GetBits(byte b)
        {
            for (int i = 0; i < 8; i++)
            {
                yield return (b & 0x80) != 0;
                b *= 2;
            }
        }


        #region GetBit

        public static bool GetBit(ushort b, int bitNumber)
        {
            return GetBit(new byte[] { (byte)b, (byte)(b >> 8) }, bitNumber);
        }

        public static bool GetBit(byte[] b, int bitNumber)
        {
            return GetBit(b[(int)(bitNumber / 8)], (bitNumber % 8));
        }

        public static bool GetBit(byte b, int bitNumber)
        {
            return ((int)b & 1 << bitNumber) > 0;
        }

        #endregion

        #region SetBit

        public static byte SetBit(byte b, int bitNumber, bool newValue)
        {
            byte mask = (byte)(1 << bitNumber);
            return (byte)(newValue ? (b | mask) : (b & ~mask));
        }

        public static byte[] SetBit(byte[] b, int bitNumber, bool newValue)
        {
            b[(int)(bitNumber / 8)] = SetBit(b[(int)(bitNumber / 8)], (bitNumber % 8), newValue);
            return b;
        }

        #endregion

        public static ushort ReadUshort(byte[] array, int startOffset = 0)
        {
            return (ushort)(array[startOffset + 0] | array[startOffset + 1] << 8);
        }

        public static void WriteUshort(byte[] array, ushort value, int startOffset = 0)
        {
            array[startOffset + 0] = (byte)value;
            array[startOffset + 1] = (byte)(value >> 8);
        }
    }
}

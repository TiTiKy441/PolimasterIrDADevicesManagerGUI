using System.Text;

namespace PolimasterIrDADevicesManagerGUI.Utils
{
    static class RawBytesConverter
    {


        public static string GetStringValue(byte[] b)
        {
            return Encoding.ASCII.GetString(b, 0, b.Length);
        }

        public static double ReadDouble(IList<byte> source, int startOffset = 0)
        {
            var value = new[] { source[startOffset + 1], source[startOffset], source[startOffset + 3], source[startOffset + 2] };
            if (value[0] == 0 && value[1] == 0 && value[2] == 0 && value[3] == 0) return 0;
            var mant = ((value[1] & 0x7f) + (double)value[2] / 0x100 + (double)value[3] / 0x10000) / 0x80 + 0x1;
            if (value[0] != 0) mant *= Math.Pow(2, value[0] - 0x80);
            if (value[1] > 0x80) mant = -mant;
            return Math.Round(mant, 14);
        }

        public static byte[] GetDoubleAsBytes(double value)
        {
            byte[] bytes = new byte[4];
            WriteDouble(value, bytes, 0);
            return bytes;
        }

        public static void WriteDouble(double value, IList<byte> destination, int startOffset = 0)
        {
            byte i, zn1, zn2;
            var x = new byte[4];

            var numValue = value;
            if (numValue == 0) return;
            if (numValue < 0)
            {
                numValue *= -1.0;
                zn1 = 0x80;
            }
            else zn1 = 0;
            if (numValue > 1) zn2 = 0;
            else zn2 = 1;
            byte k1 = 0x80;
            double k2 = 1;
            byte step = 0;
            byte pos1 = 2;
            byte posb = 1;
            while (numValue >= 2)
            {
                step++;
                numValue = numValue / 2;
            }
            while (numValue < 1)
            {
                step++;
                numValue = numValue * 2;
            }
            numValue = numValue - 1;
            for (i = 1; i <= 24; i++)
            {
                if (numValue > k2)
                {
                    x[pos1 - 1] = (byte)(x[pos1 - 1] + k1);
                    numValue = numValue - k2;
                }
                if (numValue == k2)
                {
                    x[pos1 - 1] = (byte)(x[pos1 - 1] + k1);
                    goto lab1;
                }
                k1 = (byte)(k1 / 2);
                k2 = k2 / 2;
                posb++;
                if (posb != 9) continue;
                posb = 1;
                pos1++;
                k1 = 0x80;
            }
        lab1:
            if (zn2 == 1)
                x[0] = (byte)(0x80 - step);
            else x[0] = (byte)(0x80 | step);
            x[1] = (byte)(x[1] | zn1);
            destination[startOffset + 0] = x[1];
            destination[startOffset + 1] = x[0];
            destination[startOffset + 2] = x[3];
            destination[startOffset + 3] = x[2];
        }

        public static byte[] TimeTo4Bytes(DateTime dt)
        {
            var date = (long)(dt - new DateTime(1999, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            var x1 = (byte)date;
            var x2 = (byte)(date >> 8);
            var x3 = (byte)(date >> 16);
            var x4 = (byte)(date >> 24);
            return new[] { x1, x2, x3, x4 };
        }

        public static byte[] TimeTo3Bytes(DateTime dt)
        {
            var ticks = (dt.Ticks - 630507456000000000) / 60 / 10000000;
            var x1 = (byte)ticks;
            var x2 = (byte)(ticks >> 8);
            var x3 = (byte)(ticks >> 16);
            return new[] { x1, x2, x3 };
        }

        public static DateTime Bytes3ToTime(byte[] array, int startOffset = 0) => Bytes3ToTime(array[startOffset], array[startOffset + 1], array[startOffset + 2]);

        public static DateTime Bytes3ToTime(byte x1, byte x2, byte x3)
        {
            var int64LiValue = 630507456000000000 + ((long)x1 + 256 * x2 + 256 * 256 * x3) * 10000000 * 60;
            return new DateTime(int64LiValue);
        }

        public static DateTime Bytes4ToTime(byte[] array, int startOffset = 0) => Bytes4ToTime(array[startOffset], array[startOffset + 1], array[startOffset + 2], array[startOffset + 3]);

        public static DateTime Bytes4ToTime(byte x1, byte x2, byte x3, byte x4)
        {
            long date = ((x4 * 256 + x3) * 256 + x2) * 256 + x1;
            return new DateTime(1999, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMinutes(date);
        }

    }
}

using System;

namespace Jamsaz.FingerPrintPortableAPI.Common
{
    public static class TypeConverter
    {
        public static ushort ToUnInt16(byte[] input)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(input);

            return BitConverter.ToUInt16(input, 0);
        }

        public static uint ToUnInt32(byte[] input)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(input);

            return BitConverter.ToUInt32(input, 0);
        }

        public static  byte[] ToByteArray(int intValue)
        {
            var intBytes = BitConverter.GetBytes(intValue);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(intBytes);
            return intBytes;
        }
    }
}

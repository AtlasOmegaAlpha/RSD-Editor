using System;
using System.Drawing;
using System.Text;

namespace Util.Mii
{
    public class MiiData
    {
        public static string GetMiiName(byte[] miiData)
        {
            byte[] nameBytes = new byte[0x14];
            Array.Copy(miiData, 0x2, nameBytes, 0, 0x14);
            return Encoding.BigEndianUnicode.GetString(nameBytes);
        }

        public static ushort CalculateCRC(byte[] data)
        {
            int length = data.Length;
            ushort num = 0;
            for (int i = 0; i < length; i++)
            {
                for (int j = 7; j >= 0; j--)
                {
                    num = (ushort)(((uint)(num << 1) | ((uint)(data[i] >> j) & 1u)) ^ (((num & 0x8000u) != 0) ? 4129u : 0u));
                }
            }
            for (int k = 16; k > 0; k--)
            {
                num = (ushort)((uint)(num << 1) ^ (((num & 0x8000u) != 0) ? 4129u : 0u));
            }
            return (ushort)(num & 0xFFFFu);
        }
    }
}

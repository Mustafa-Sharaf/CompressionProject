using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesCompressionProject.ShannonFano
{
    public static class BitHelper
    {
        public static List<byte> BitsToBytes(string bits)
        {
            var bytes = new List<byte>();
            for (int i = 0; i < bits.Length; i += 8)
            {
                string byteString = bits.Substring(i, Math.Min(8, bits.Length - i));
                while (byteString.Length < 8)
                    byteString += "0";
                bytes.Add(Convert.ToByte(byteString, 2));
            }
            return bytes;
        }

        public static string BytesToBits(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (var b in bytes)
                sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            return sb.ToString();
        }
    }

}

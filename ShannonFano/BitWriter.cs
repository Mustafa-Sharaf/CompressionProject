using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesCompressionProject.ShannonFano
{
    public class BitWriter
    {
        private List<byte> _bytes = new List<byte>();
        private byte _currentByte = 0;
        private int _bitPosition = 0;

        public void WriteBits(string bits)
        {
            foreach (char bit in bits)
            {
                if (bit == '1')
                    _currentByte |= (byte)(1 << (7 - _bitPosition));

                _bitPosition++;
                if (_bitPosition == 8)
                {
                    _bytes.Add(_currentByte);
                    _currentByte = 0;
                    _bitPosition = 0;
                }
            }
        }

        public List<byte> GetBytes()
        {
            if (_bitPosition > 0)
                _bytes.Add(_currentByte);
            return _bytes;
        }
    }

}

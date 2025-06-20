using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FilesCompressionProject
{
    public class HuffmanCompressor
    {
        private readonly Func<bool> isCancelled;

        public HuffmanCompressor(Func<bool> cancelFunc = null)
        {
            isCancelled = cancelFunc ?? (() => false);
        }
        public void CompressFile(string filePath, string outputPath = "compressed.huff")
        {
            var bytes = File.ReadAllBytes(filePath);
            var freq = BuildFrequencyTable(bytes);
            var tree = BuildTree(freq);

            var codes = new Dictionary<byte, string>();
            GenerateCodes(tree, "", codes);

            var encodedBits = string.Concat(bytes.Select(b => codes[b]));

            using (BinaryWriter writer = new BinaryWriter(File.Open(outputPath, FileMode.Create)))
            {

                writer.Write(codes.Count);


                foreach (var kv in codes)
                {
                    writer.Write(kv.Key);
                    writer.Write(kv.Value);
                }


                writer.Write(encodedBits.Length);


                var bytesToWrite = BitsToBytes(encodedBits);
                writer.Write(bytesToWrite);
            }
        }

        public void DecompressFile(string inputPath, string outputPath = "decompressed_output")
        {
            using (BinaryReader reader = new BinaryReader(File.Open(inputPath, FileMode.Open)))
            {

                int codeCount = reader.ReadInt32();
                var codeTable = new Dictionary<string, byte>();
                for (int i = 0; i < codeCount; i++)
                {
                    byte symbol = reader.ReadByte();
                    string code = reader.ReadString();
                    codeTable[code] = symbol;
                }

                int totalBits = reader.ReadInt32();

                var bitBytes = reader.ReadBytes((totalBits + 7) / 8);
                string bitString = BytesToBits(bitBytes).Substring(0, totalBits);

                List<byte> result = new List<byte>();
                string buffer = "";
                foreach (char bit in bitString)
                {
                    buffer += bit;
                    if (codeTable.ContainsKey(buffer))
                    {
                        result.Add(codeTable[buffer]);
                        buffer = "";
                    }
                }

                File.WriteAllBytes(outputPath, result.ToArray());
            }
        }

        private Dictionary<byte, int> BuildFrequencyTable(byte[] data)
        {
            var freq = new Dictionary<byte, int>();
            foreach (byte b in data)
            {
                if (!freq.ContainsKey(b)) freq[b] = 0;
                freq[b]++;
            }
            return freq;
        }

        private HuffmanNode BuildTree(Dictionary<byte, int> freqTable)
        {
            var pq = new SimplePriorityQueue<HuffmanNode>();

            foreach (var kv in freqTable)
            {
                var node = new HuffmanNode
                {
                    Symbol = kv.Key,
                    Frequency = kv.Value
                };
                pq.Enqueue(node, node.Frequency);
            }

            while (pq.Count > 1)
            {
                var left = pq.Dequeue();
                var right = pq.Dequeue();

                var parent = new HuffmanNode
                {
                    Symbol = 0,
                    Frequency = left.Frequency + right.Frequency,
                    Left = left,
                    Right = right
                };

                pq.Enqueue(parent, parent.Frequency);
            }

            return pq.Dequeue();
        }

        private void GenerateCodes(HuffmanNode node, string code, Dictionary<byte, string> codes)
        {
            if (node == null) return;

            if (node.Left == null && node.Right == null)
                codes[(byte)node.Symbol] = code;

            GenerateCodes(node.Left, code + "0", codes);
            GenerateCodes(node.Right, code + "1", codes);
        }

        private byte[] BitsToBytes(string bits)
        {
            int numBytes = (bits.Length + 7) / 8;
            byte[] result = new byte[numBytes];

            for (int i = 0; i < bits.Length; i++)
            {
                if (bits[i] == '1')
                {
                    result[i / 8] |= (byte)(1 << (7 - (i % 8)));
                }
            }

            return result;
        }

        private string BytesToBits(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder(bytes.Length * 8);
            foreach (var b in bytes)
                sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            return sb.ToString();
        }
    }
}
using FilesCompressionProject.ShannonFano;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace FilesCompressionProject
{

    public class ShannonFanoCompressor
    {

        private Func<bool> _isCancelled;

        public ShannonFanoCompressor(Func<bool> isCancelled = null)
        {
            _isCancelled = isCancelled ?? (() => false);
        }

        public string CompressFile(string inputPath)
        {
            byte[] bytes = File.ReadAllBytes(inputPath);

            var frequencies = bytes.GroupBy(b => b)
                                   .Select(g => new Symbol { ByteValue = g.Key, Frequency = g.Count() })
                                   .OrderByDescending(s => s.Frequency)
                                   .ToList();

            if (_isCancelled()) throw new OperationCanceledException();

            var coder = new ShannonFanoCoder();
            coder.BuildCodes(frequencies);

            var encodingMap = frequencies.ToDictionary(s => s.ByteValue, s => s.Code);

            var bitWriter = new BitWriter();

            foreach (var b in bytes)
            {
                if (_isCancelled()) throw new OperationCanceledException();

                bitWriter.WriteBits(encodingMap[b]);
            }

            var compressedBytes = bitWriter.GetBytes();

            string outputPath = Path.Combine(Path.GetDirectoryName(inputPath),
                                             Path.GetFileNameWithoutExtension(inputPath) + "_sf.shf");

            using (var writer = new BinaryWriter(File.Open(outputPath, FileMode.Create)))
            {
                writer.Write(Path.GetFileName(inputPath));
                writer.Write(frequencies.Count);

                foreach (var s in frequencies)
                {
                    if (_isCancelled()) throw new OperationCanceledException();

                    writer.Write(s.ByteValue);
                    writer.Write(s.Code);
                }

                writer.Write(compressedBytes.Count);
                writer.Write(compressedBytes.ToArray());
            }

            return outputPath;
        }


 
        public void DecompressFile(string inputPath, Func<bool> isCancelled = null)
        {
            if (isCancelled == null)
                isCancelled = () => false;


            using (var reader = new BinaryReader(File.Open(inputPath, FileMode.Open)))
            {
                string originalFileName = reader.ReadString();

                int count = reader.ReadInt32();
                var decodingMap = new Dictionary<string, byte>();
                for (int i = 0; i < count; i++)
                {
                    if (isCancelled()) throw new OperationCanceledException();

                    byte b = reader.ReadByte();
                    string code = reader.ReadString();
                    decodingMap[code] = b;
                }

                int compressedByteCount = reader.ReadInt32();
                byte[] compressedBytes = reader.ReadBytes(compressedByteCount);

                string encodedBits = BitHelper.BytesToBits(compressedBytes);

                var outputBytes = new List<byte>();
                string current = "";
                foreach (char bit in encodedBits)
                {
                    if (isCancelled()) throw new OperationCanceledException();

                    current += bit;
                    if (decodingMap.ContainsKey(current))
                    {
                        outputBytes.Add(decodingMap[current]);
                        current = "";
                    }
                }

                string name = Path.GetFileNameWithoutExtension(originalFileName);
                string ext = Path.GetExtension(originalFileName);
                string outputPath = Path.Combine(Path.GetDirectoryName(inputPath),
                                                 name + "_decompressed" + ext);

                File.WriteAllBytes(outputPath, outputBytes.ToArray());
                MessageBox.Show("تم فك ضغط الملف بنجاح في المسار:\n" + outputPath);
            }
        }

    }

}

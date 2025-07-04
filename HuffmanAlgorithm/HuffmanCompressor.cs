using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FilesCompressionProject
{
    public class HuffmanCompressor
    {
        
     private readonly Func<bool> isCancelled;

    public HuffmanCompressor(Func<bool> cancelFunc = null)
    {
        isCancelled = cancelFunc ?? (() => false);
    }


        //public void CompressFile(string filePath, string outputPath = "compressed.huff")
        public void CompressFile(string filePath, string outputPath = "compressed.huff", string password = null)
        {
            var bytes = File.ReadAllBytes(filePath);

            if (bytes.Length == 0)
            {
                Console.WriteLine($"تم تجاهل الملف الفارغ: {filePath}");
                return;
            }

           /* string password = PromptForPassword();
            if (string.IsNullOrWhiteSpace(password))
                 {
                     System.Media.SystemSounds.Beep.Play(); 
                     System.Windows.Forms.MessageBox.Show("لم يتم إدخال كلمة المرور، تم إلغاء عملية الضغط.", "تنبيه", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                     return;
                 }*/
                if (string.IsNullOrWhiteSpace(password))
                {
                    password = PromptForPassword();
                 }
            if (string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("إلغاء الضغط بسبب عدم إدخال كلمة المرور.");
                return;
            }


            var freq = BuildFrequencyTable(bytes);
            var tree = BuildTree(freq);

            var codes = new Dictionary<byte, string>();
            GenerateCodes(tree, "", codes);

            var encodedBits = string.Concat(bytes.Select(b => codes[b]));
           

            using (BinaryWriter writer = new BinaryWriter(File.Open(outputPath, FileMode.Create)))
            {

                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                writer.Write(passwordBytes.Length);
                writer.Write(passwordBytes);
                string originalFileName = Path.GetFileName(filePath);
                writer.Write(originalFileName);
                writer.Write(codes.Count);


                foreach (var kv in codes)
                {if (isCancelled()) return;

                    writer.Write(kv.Key);
                    writer.Write(kv.Value);
                }


                writer.Write(encodedBits.Length);


                var bytesToWrite = BitsToBytes(encodedBits);
                writer.Write(bytesToWrite);
            }
        }

        public bool DecompressFile(string inputPath, string outputPath)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(inputPath, FileMode.Open)))
            {
                int passwordLength = reader.ReadInt32();
                byte[] storedPasswordBytes = reader.ReadBytes(passwordLength);

                string enteredPassword = PromptForPassword();
                if (string.IsNullOrWhiteSpace(enteredPassword))
                {
                    System.Media.SystemSounds.Beep.Play();
                    MessageBox.Show("يجب إدخال كلمة المرور لفك الضغط.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                byte[] enteredPasswordBytes = Encoding.UTF8.GetBytes(enteredPassword);
                if (!storedPasswordBytes.SequenceEqual(enteredPasswordBytes))
                {
                    Console.WriteLine("كلمة المرور غير صحيحة!");
                    return false;
                }

                string originalFileName = reader.ReadString();
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
                    if (isCancelled()) return false;

                    buffer += bit;
                    if (codeTable.ContainsKey(buffer))
                    {
                        result.Add(codeTable[buffer]);
                        buffer = "";
                    }
                }

                File.WriteAllBytes(outputPath, result.ToArray()); // ✅ Write directly to the provided path
            }
            return true;
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
        //public byte[] CompressBytes(byte[] inputBytes)
        public byte[] CompressBytes(byte[] inputBytes, string password = null)
        {
            string inputTemp = Path.GetTempFileName();
            string outputTemp = Path.GetTempFileName();
            File.WriteAllBytes(inputTemp, inputBytes);
            CompressFile(inputTemp, outputTemp,password);

            if (!File.Exists(outputTemp) || new FileInfo(outputTemp).Length == 0)
            {
                File.Delete(inputTemp);
                File.Delete(outputTemp);
                return null; 
            }

            byte[] compressed = File.ReadAllBytes(outputTemp);
            File.Delete(inputTemp);
            File.Delete(outputTemp);
            return compressed;
        }

        public byte[] DecompressBytes(byte[] compressedBytes)
        {
            string inputTemp = Path.GetTempFileName();
            string outputTemp = Path.GetTempFileName();
            File.WriteAllBytes(inputTemp, compressedBytes);
            bool success = DecompressFile(inputTemp, outputTemp);
            if (!success)
            {
                File.Delete(inputTemp);
                File.Delete(outputTemp);
                return null;
            }
            byte[] decompressed = File.ReadAllBytes(outputTemp);
            File.Delete(inputTemp);
            File.Delete(outputTemp);
            return decompressed;
        }

        private string BytesToBits(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder(bytes.Length * 8);
            foreach (var b in bytes)
                sb.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
            return sb.ToString();
        }
        public string CompressFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                throw new DirectoryNotFoundException("المجلد غير موجود");

            var files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);
            if (files.Length == 0)
                throw new InvalidOperationException("المجلد فارغ");

            string archiveName = Path.GetFileName(folderPath.TrimEnd(Path.DirectorySeparatorChar));
            string parentDir = Path.GetDirectoryName(folderPath.TrimEnd(Path.DirectorySeparatorChar));
            string archivePath = Path.Combine(
                parentDir,
                $"{archiveName}_{DateTime.Now:yyyyMMdd_HHmmss}.huffarc"
            );
            string password = PromptForPassword();//4
            if (string.IsNullOrWhiteSpace(password))
            {
                System.Windows.Forms.MessageBox.Show("لم يتم إدخال كلمة المرور. تم إلغاء الضغط.", "تنبيه", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                return null;
            }


            var entries = new List<HuffmanArchiveEntry>();

            using (var fs = new FileStream(archivePath, FileMode.Create))
            {
                fs.Seek(1024, SeekOrigin.Begin);

                foreach (var file in files)
                {
                    if (isCancelled())
                    {
                        fs.Close();
                        if (File.Exists(archivePath))
                            File.Delete(archivePath);
                        return null;
                    }

                    string relativePath = GetRelativePath(folderPath, file).Replace('\\', '/');


                    byte[] inputBytes = File.ReadAllBytes(file);
                    byte[] compressedBytes = CompressBytes(inputBytes,password);
                    if (compressedBytes == null)
                    {
                        Console.WriteLine($"فشل ضغط الملف: {file}"); continue;
                     }

                long offset = fs.Position;
                    fs.Write(compressedBytes, 0, compressedBytes.Length);

                    entries.Add(new HuffmanArchiveEntry
                    {
                        FileName = relativePath,
                        OriginalSize = inputBytes.Length,
                        CompressedSize = compressedBytes.Length,
                        Offset = offset
                    });
                }

                fs.Seek(0, SeekOrigin.Begin);
                using (BinaryWriter writer = new BinaryWriter(fs, Encoding.UTF8, true))
                {
                    writer.Write(entries.Count);
                    foreach (var entry in entries)
                    {
                        writer.Write(entry.FileName);
                        writer.Write(entry.OriginalSize);
                        writer.Write(entry.CompressedSize);
                        writer.Write(entry.Offset);
                    }
                }
            }

            return archivePath;
        }
        public static string GetRelativePath(string basePath, string fullPath)
        {
            Uri baseUri = new Uri(AppendSlash(basePath));
            Uri fullUri = new Uri(fullPath);
            Uri relativeUri = baseUri.MakeRelativeUri(fullUri);
            return Uri.UnescapeDataString(relativeUri.ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        private static string AppendSlash(string path)
        {
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
                return path + Path.DirectorySeparatorChar;
            return path;
        }
        public static string PromptForPassword()
        {
            return Microsoft.VisualBasic.Interaction.InputBox("أدخل كلمة المرور:", "حماية الملف", "");
        }


    }
}
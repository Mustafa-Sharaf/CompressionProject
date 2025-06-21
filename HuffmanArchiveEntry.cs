using FilesCompressionProject;

namespace FilesCompressionProject
{
    public class HuffmanArchiveEntry
    {
        public string FileName { get; set; }
        public long OriginalSize { get; set; }
        public long CompressedSize { get; set; }
        public long Offset { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JFTP.Lib
{
    public class JFileDetail : JFile
    {
        public long Length { get; set; }
        public long ChunkSize { get; set; }
        public IList<JFileChunk> Chunks { get; set; }

        public JFileDetail()
        {
        }

        public JFileDetail(string path)
            : this(path, 10)
        {
        }

        public JFileDetail(string path, int numberOfChunks)
            : this(path, numberOfChunks, 1024.Kilobytes())
        {
        }

        public JFileDetail(string path, int numberOfChunks, long minimumChunkSize)
            : base(path)
        {
            var fileInfo = new FileInfo(path);
            var chunks = new List<JFileChunk>();

            var chunkSize = new[] 
            { 
                minimumChunkSize, 
                (long)Math.Ceiling(fileInfo.Length / Config.ChunkIncrement / (double)numberOfChunks) * Config.ChunkIncrement 
            }.Max();

            Length = fileInfo.Length;

            for (int i = 0; i < numberOfChunks; i++)
            {
                chunks.Add(new JFileChunk()
                {
                    Part = i + 1,
                    Parts = numberOfChunks,
                    Length = Length > chunkSize * (i + 1) ? chunkSize : Length - chunkSize * i,
                    Offset = chunkSize * i,
                });
            }

            ChunkSize = chunkSize;
            Chunks = chunks;
        }
    }

    public class JFileChunk
    {
        public int Part { get; set; }
        public int Parts { get; set; }
        public long Length { get; set; }
        public long Offset { get; set; }
    }
}

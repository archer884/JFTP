namespace JFTP.Lib
{
    public class ChunkRequest
    {
        public string Token { get; set; }
        public int Chunks { get; set; }
        public int ChunkNumber { get; set; }
    }
}
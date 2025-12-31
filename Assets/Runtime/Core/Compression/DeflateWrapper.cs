using System.IO;
using System.IO.Compression;
using System.Text;

namespace Runtime.Core.Compression
{
    public abstract class DeflateWrapper
    {
        public static byte[] DeflateCompress(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var output = new MemoryStream();
            var deflateStream = new DeflateStream(output, CompressionMode.Compress, true);
            deflateStream.Write(bytes);
            deflateStream.Dispose();
            return output.ToArray();
        }

        public static string DeflateDecompress(byte[] bytes)
        {
            var input = new MemoryStream(bytes);
            var output = new MemoryStream();
            var deflateStream = new DeflateStream(input, CompressionMode.Decompress);
            deflateStream.CopyTo(output);
            return Encoding.UTF8.GetString(output.ToArray());
        }
    }
}

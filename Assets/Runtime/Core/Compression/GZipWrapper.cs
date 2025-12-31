using System.IO;
using System.IO.Compression;
using System.Text;

namespace Runtime.Core.Compression
{
    public class GZipWrapper
    {
        public static byte[] Compress(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            var output = new MemoryStream();
            var gZipStream = new GZipStream(output, CompressionMode.Compress, true);
            gZipStream.Write(bytes);
            gZipStream.Dispose();
            return output.ToArray();
        }

        public static string Decompress(byte[] bytes)
        {
            var input = new MemoryStream(bytes);
            var output = new MemoryStream();
            var gZipStream = new GZipStream(input, CompressionMode.Decompress);
            gZipStream.CopyTo(output);
            var array = output.ToArray();
            var text = Encoding.UTF8.GetString(array);
            return text;
        }
    }
}

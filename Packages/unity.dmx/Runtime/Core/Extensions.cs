namespace Unity_DMX.Core
{
    public static class Extensions
    {
        public static byte[] EnsureCapacity(this byte[] buffer, int capacity)
        {
            if (buffer.Length < capacity)
            {
                var temp = new byte[capacity];
                System.Buffer.BlockCopy(buffer, 0, temp, 0, buffer.Length);
                buffer = temp;
            }
            
            return buffer;
        }

        public static byte[] BlockCopy(this byte[] dstBuffer, byte[] srcBuffer, int srcOffset, int dstOffset, int count)
        {
            System.Buffer.BlockCopy(srcBuffer, srcOffset, dstBuffer, dstOffset, count);
            return dstBuffer;
        }
    }
}

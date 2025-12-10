using System.Collections.Generic;

namespace Unity_DMX.Core
{
    public class DmxData
    {
        private readonly List<byte> buffer;
        
        public int Count => buffer.Count;

        public DmxData(int size)
        {
            buffer = new List<byte>(new byte[size]);
        }

        public DmxData(byte[] data)
        {
            buffer = new List<byte>(data);
        }

        public void EnsureCapacity(int capacity)
        {
            if (buffer.Count >= capacity) return;
            
            buffer.AddRange(new byte[capacity - buffer.Count]);
        }
        
        public void Set(int dstOffset, byte value)
        {
            buffer[dstOffset] = value;
        }

        public void SetRange(int dstOffset, DmxData srcBuffer, int srcOffset = 0, int srcCount = -1)
        {
            if (srcCount < 0) srcCount = srcBuffer.Count;
            buffer.BlockCopy(dstOffset, srcBuffer.GetBuffer(), srcOffset, srcCount);
        }

        public void SetRange(int dstOffset, List<byte> srcBuffer, int srcOffset = 0, int srcCount = -1)
        {
            if (srcCount < 0) srcCount = srcBuffer.Count;
            buffer.BlockCopy(dstOffset, srcBuffer, srcOffset, srcCount);
        }

        public void SetRange(int dstOffset, byte[] srcBuffer, int srcOffset = 0, int srcCount = -1)
        {
            if (srcCount < 0) srcCount = srcBuffer.Length;
            buffer.BlockCopy(dstOffset, srcBuffer, srcOffset, srcCount);
        }

        public List<byte> GetBuffer()
        {
            return buffer;
        }

        public byte[] GetBufferArray()
        {
            byte[] buf = new byte[buffer.Count];

            buf.BlockCopy(buffer.ToArray(), 0, 0, buffer.Count);

            return buf;
        }
    }
}

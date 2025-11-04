using System;
using UnityEngine;

namespace Unity_DMX.Core
{
    public class DmxBuffer : MonoBehaviour
    {
        public byte[] buffer; // TODO: hide this variable and use methods to control data
        
        public event Action<byte[]> OnBufferUpdate = delegate { };

        public ulong bufferUpdates = 0;

        private void Awake()
        {
            buffer = new byte[512 * 40];
            
            OnBufferUpdate += BufferUpdate;
        }

        private void BufferUpdate(byte[] buffer)
        {
            bufferUpdates++;
            if (bufferUpdates >= ulong.MaxValue) bufferUpdates = ulong.MinValue;
        }

        private void Update()
        {
            OnBufferUpdate?.Invoke(buffer);
        }
    }
}

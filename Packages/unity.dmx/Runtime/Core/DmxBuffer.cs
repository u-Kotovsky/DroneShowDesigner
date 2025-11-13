using System;
using UnityEngine;

namespace Unity_DMX.Core
{
    public class DmxBuffer : MonoBehaviour
    {
        public byte[] buffer; // TODO: hide this variable and use methods to control data
        
        public event Action<byte[]> OnBufferUpdate = delegate { };

#if UNITY_EDITOR
        public ulong bufferUpdates = 0;
#endif

        private void Awake()
        {
            buffer = new byte[512 * 40];
            
#if UNITY_EDITOR
            OnBufferUpdate += BufferUpdate;
#endif
        }

#if UNITY_EDITOR
        private void BufferUpdate(byte[] data)
        {
            bufferUpdates++;
            if (bufferUpdates >= ulong.MaxValue) bufferUpdates = ulong.MinValue;
        }
#endif

        private void Update()
        {
            OnBufferUpdate?.Invoke(buffer);
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity_DMX.Core
{
    public class DmxBuffer : MonoBehaviour
    {
        public DmxData Buffer;
        
        public event Action<DmxData> OnBufferUpdate = delegate { };

#if UNITY_EDITOR
        public ulong bufferUpdates = 0;
#endif

        private void Awake()
        {
            Buffer = new DmxData(0);
            
#if UNITY_EDITOR
            OnBufferUpdate += BufferUpdate;
#endif
        }

#if UNITY_EDITOR
        private void BufferUpdate(DmxData data)
        {
            bufferUpdates++;
            if (bufferUpdates >= ulong.MaxValue) bufferUpdates = ulong.MinValue;
        }
#endif

        private void Update()
        {
            OnBufferUpdate?.Invoke(Buffer);
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity_DMX.Core
{
    public class DmxBuffer : MonoBehaviour
    {
        public DmxData Buffer;
        
        private event Action<DmxData> OnBufferUpdate = delegate { };

        public void OnBufferUpdateSubscribe(Action<DmxData> callback)
        {
            OnBufferUpdate += callback;
        }
        
        public void OnBufferUpdateUnSubscribe(Action<DmxData> callback)
        {
            OnBufferUpdate -= callback;
        }
        
        private void Awake()
        {
            Buffer = new DmxData(0);
            
#if UNITY_EDITOR && DRONE_BUFFER_DEBUG
            OnBufferUpdate += BufferUpdate;
#endif
        }

#if UNITY_EDITOR && DRONE_BUFFER_DEBUG
        public ulong bufferUpdates = 0;
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
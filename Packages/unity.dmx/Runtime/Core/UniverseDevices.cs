using System;
using Unity_DMX.Device;
using UnityEngine;

namespace Unity_DMX.Core
{
    [Serializable]
    public class UniverseDevices : MonoBehaviour
    {
        public string universeName;
        public int universe;
        public DMXDevice[] devices;

        public void Initialize()
        {
            var startChannel = 0;
            foreach (var d in devices)
            {
                if (d == null) continue;
                d.startChannel = startChannel;
                startChannel += d.NumChannels;
                d.name = $"{d.GetType()}:({universe},{d.startChannel:d3}-{startChannel - 1:d3})";
            }

            if (512 < startChannel)
            {
                Debug.LogErrorFormat("The number({0}) of channels of the universe {1} exceeds the upper limit(512 channels)!", startChannel, universe);
            }
        }
    }
}

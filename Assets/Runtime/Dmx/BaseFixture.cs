using Runtime.Dmx.Fixtures;
using UnityEngine;

namespace Runtime.Dmx
{
    public abstract class BaseFixture : MonoBehaviour
    {
        public FixtureSpawnManager spawnManager;
        public int globalChannelStart;
        protected byte[] Buffer;
        public int fixtureIndex;

        public abstract void WriteDmxData();

        public byte[] GetDmxData()
        {
            WriteDmxData();
            
            return Buffer;
        }
    }
}

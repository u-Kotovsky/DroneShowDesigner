using UnityEngine;

namespace Runtime.Dmx.Fixtures
{
    public abstract class BaseFixture : MonoBehaviour
    {
        public FixtureSpawnManager spawnManager;
        public int globalChannelStart;
        protected byte[] Buffer;
        protected byte[] Length;
        public int fixtureIndex;

        public abstract void WriteDmxData();

        public byte[] GetDmxData()
        {
            WriteDmxData();

            //if (!gameObject.activeInHierarchy) return Array.Empty<byte>(Length);
            
            return Buffer;
        }

        public static void Poke()
        {
            // Initialize static
        }
    }
}

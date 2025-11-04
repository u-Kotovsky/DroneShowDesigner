using Runtime.Dmx.Fixtures.Shared;
using UnityEngine;

namespace Runtime.Dmx.Fixtures.Lights
{
    // TODO: Boundary box visuals/limit for editor usage
    public class MobileLight : BaseMobile
    {
        private void Awake()
        {
            Buffer = new byte[6];

            Vector3 offset = new Vector3(0, 21f, 0);
            MinPosition = new Vector3(-52.5f, -22.5f, -52.5f) + offset;
            MaxPosition = new Vector3(52.5f, 22.5f, 52.5f) + offset;
        }
        
        private void Update()
        {
            WriteDmxPosition(0, transform.position);
        }

        public override void WriteDmxData()
        {
            //WriteDmxPosition(0, transform.position); // requires mainthread
        }

        #region Static
        public static GameObject mobileLightPrefab = Resources.Load<GameObject>("MobileLight");
        private static GameObject internalPool;
        
        public static void Spawn(FixtureSpawnManager spawnManager, ref MobileLight[] pool, ref int count)
        {
            if (internalPool == null) internalPool = new GameObject("MobileLightPool");
            pool = new MobileLight[count];
            MobileLight fixture = null;
            int offset = 1077; // Start for mobile light.

            for (int i = 0; i < pool.Length; i++)
            {
                Spawn(ref pool, ref i, ref offset, ref fixture);
                fixture.spawnManager = spawnManager;
            }
            
            Debug.Log($"{pool.Length} mobile lights are instanced");
        }

        private static void Spawn(ref MobileLight[] pool, ref int index, ref int offset, ref MobileLight fixture)
        {
            var instance = Instantiate(mobileLightPrefab, new Vector3(index, 2, 0), Quaternion.identity);
            instance.transform.SetParent(internalPool.transform);
            fixture = instance.AddComponent<MobileLight>();
            fixture.fixtureIndex = index;
            fixture.globalChannelStart = offset + (index * fixture.GetDmxData().Length);
            fixture.gameObject.name = "MobileLight #" + fixture.fixtureIndex;
            pool[index] = fixture;
        }
        
        public static void WriteDataToGlobalBuffer(ref MobileLight[] pool, ref byte[] globalDmxBuffer)
        {
            foreach (var fixture in pool)
            {
                byte[] data = fixture.GetDmxData();
                
                System.Buffer.BlockCopy(data, 0, 
                    globalDmxBuffer, fixture.globalChannelStart, data.Length);
            }

            WriteSpecialData(globalDmxBuffer);
        }

        private static void WriteSpecialData(byte[] buffer)
        {
            // 1229 206 16 Way Selector. 0 = No Action; 1 = Hide Search Light mesh [lights can still function]; Rest undefined.
            buffer[1229] = 0;
        }
        #endregion
    }
}

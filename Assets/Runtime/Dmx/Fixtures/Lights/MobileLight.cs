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

        public override void WriteDmxData()
        {
            WriteDmxPosition(0, transform.position);
        }

        #region Static
        public static GameObject mobileLightPrefab = Resources.Load<GameObject>("MobileLight");
        private static GameObject internalPool;
        
        public static void Spawn(FixtureSpawnManager spawnManager, ref GameObject[] pool, ref int count)
        {
            if (internalPool == null) internalPool = new GameObject("MobileLightPool");
            pool = new GameObject[count];
            MobileLight fixture = null;
            int offset = 1077; // Start for mobile light.

            for (int i = 0; i < pool.Length; i++)
            {
                Spawn(ref pool, ref i, ref offset, ref fixture);
                fixture.spawnManager = spawnManager;
            }
            
            Debug.Log($"{pool.Length} mobile lights are instanced");
        }

        private static void Spawn(ref GameObject[] pool, ref int index, ref int offset, ref MobileLight fixture)
        {
            pool[index] = Instantiate(mobileLightPrefab, new Vector3(index, 2, 0), Quaternion.identity);
            pool[index].transform.SetParent(internalPool.transform);
            fixture = pool[index].AddComponent<MobileLight>();
            fixture.fixtureIndex = index;
            fixture.globalChannelStart = offset + (index * fixture.GetDmxData().Length);
            fixture.gameObject.name = "MobileLight #" + fixture.fixtureIndex;
        }
        
        public static void WriteDataToGlobalBuffer(ref GameObject[] pool, ref byte[] globalDmxBuffer)
        {
            foreach (var obj in pool)
            {
                var fixture = obj.GetComponent<MobileLight>();
                byte[] data = fixture.GetDmxData();
                
                System.Buffer.BlockCopy(data, 0, 
                    globalDmxBuffer, fixture.globalChannelStart, data.Length);
            }

            WriteSpecialData(globalDmxBuffer);
        }

        private static void WriteSpecialData(byte[] buffer)
        {
            // 1125 1138 1151 1164 1177 1190 1203 1216
            // 206 16 Way Selector. 0 = No Action; 1 = Hide Search Light mesh [lights can still function]; Rest undefined.
            // 6 8 9 10
        
            // This is basically for debugging in World. Will make dim 100, color white.
            buffer[1125 + 6 - 1] = 255;
            buffer[1125 + 8 - 1] = 255;
            buffer[1125 + 9 - 1] = 255;
            buffer[1125 + 10 - 1] = 255;
        
            buffer[1138 + 6 - 1] = 255;
            buffer[1138 + 8 - 1] = 255;
            buffer[1138 + 9 - 1] = 255;
            buffer[1138 + 10 - 1] = 255;
        
            buffer[1151 + 6 - 1] = 255;
            buffer[1151 + 8 - 1] = 255;
            buffer[1151 + 9 - 1] = 255;
            buffer[1151 + 10 - 1] = 255;
        
            buffer[1164 + 6 - 1] = 255;
            buffer[1164 + 8 - 1] = 255;
            buffer[1164 + 9 - 1] = 255;
            buffer[1164 + 10 - 1] = 255;
        
            buffer[1177 + 6 - 1] = 255;
            buffer[1177 + 8 - 1] = 255;
            buffer[1177 + 9 - 1] = 255;
            buffer[1177 + 10 - 1] = 255;
        
            buffer[1190 + 6 - 1] = 255;
            buffer[1190 + 8 - 1] = 255;
            buffer[1190 + 9 - 1] = 255;
            buffer[1190 + 10 - 1] = 255;
        
            buffer[1203 + 6 - 1] = 255;
            buffer[1203 + 8 - 1] = 255;
            buffer[1203 + 9 - 1] = 255;
            buffer[1203 + 10 - 1] = 255;
        
            buffer[1216 + 6 - 1] = 255;
            buffer[1216 + 8 - 1] = 255;
            buffer[1216 + 9 - 1] = 255;
            buffer[1216 + 10 - 1] = 255;
        }
        #endregion
    }
}

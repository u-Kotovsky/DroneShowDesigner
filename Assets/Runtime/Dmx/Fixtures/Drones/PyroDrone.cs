using UnityEngine;

namespace Runtime.Dmx.Fixtures.Drones
{
    // TODO: Boundary box visuals/limit for editor usage
    public class PyroDrone : BaseDrone
    {
        public byte pitch;
        public byte yaw;
        public byte roll;
        
        public byte index;
        
        private void Awake()
        {
            Buffer = new byte[10]; // (0 -> 6) Position Coarse + Fine, (7 -> 9) Pitch + Yaw + Roll, (10) Index
            MinAngle = -180;
            MaxAngle = 180;
            MinPosition = new Vector3(-800, -800, -800);
            MaxPosition = new Vector3(800, 800, 800);
        }

        private void WriteDmxRotation(Vector3 eulerAngles)
        {
            pitch = (byte)Utility.MapRange(eulerAngles.x, MinAngle, MaxAngle, 0, 255);
            yaw = (byte)Utility.MapRange(eulerAngles.y, MinAngle, MaxAngle, 0, 255);
            roll = (byte)Utility.MapRange(eulerAngles.z, MinAngle, MaxAngle, 0, 255);
            
            Buffer[6] = pitch;
            Buffer[7] = yaw;
            Buffer[8] = roll;
        }

        private void WriteDmxIndex(byte value)
        {
            Buffer[9] = value;
        }

        public override void WriteDmxData()
        {
            WriteDmxPosition(0, transform.position, true);
            WriteDmxRotation(transform.rotation.eulerAngles);
            WriteDmxIndex(index);
        }

        #region Static
        public static GameObject pyroDronePrefab = Resources.Load<GameObject>("PyroDrone");
        private static GameObject internalPool;
        
        public static void Spawn(FixtureSpawnManager spawnManager, ref GameObject[] pool, ref int count)
        {
            if (internalPool == null) internalPool = new GameObject("PyroDronePool");
            pool = new GameObject[count];
            PyroDrone fixture = null;
            int offset = (512 * 5) + 41 - 1; // 2600 is start for pyro drone.

            for (int i = 0; i < pool.Length; i++)
            {
                Spawn(ref pool, ref i, ref offset, ref fixture);
                fixture.spawnManager = spawnManager;
            }
            
            Debug.Log($"{pool.Length} pyro drones are instanced");

            SetPyroDronePreset(pool, PyroDronePresetManager.presets[0]);
        }

        private static void Spawn(ref GameObject[] pool, ref int index, ref int offset, ref PyroDrone fixture)
        {
            pool[index] = Instantiate(pyroDronePrefab, new Vector3(index, 2, 0), Quaternion.identity);
            pool[index].transform.SetParent(internalPool.transform);
            fixture = pool[index].AddComponent<PyroDrone>();
            fixture.fixtureIndex = index;
            fixture.globalChannelStart = offset + (index * fixture.GetDmxData().Length);
            fixture.gameObject.name = "PyroDrone #" + fixture.fixtureIndex;
        }

        private static void SetPyroDronePreset(GameObject[] pool, PyroDronePreset[] preset)
        {
            for (var i = 0; i < preset.Length; i++)
            {
                GameObject obj = pool[i];
                obj.transform.localPosition = preset[i].GetPosition();
            }
        }
        
        public static void WriteDataToGlobalBuffer(ref GameObject[] pool, ref byte[] globalDmxBuffer)
        {
            foreach (var drone in pool)
            {
                var pyroDrone = drone.GetComponent<PyroDrone>();
                byte[] droneData = pyroDrone.GetDmxData();
                
                System.Buffer.BlockCopy(droneData, 0, 
                    globalDmxBuffer, pyroDrone.globalChannelStart, droneData.Length);
            }

            WriteSpecialData(globalDmxBuffer);
        }
        
        private static void WriteSpecialData(byte[] buffer)
        {
            int offset = 512 * 5 - 1;
            buffer[offset + 39] = 255;
            buffer[offset + 40] = 0;
            buffer[offset + 243] = 255;
            buffer[offset + 244] = 0;
            buffer[offset + 245] = 255;
            buffer[offset + 246] = 255;
        }
        
        #endregion
    }
}

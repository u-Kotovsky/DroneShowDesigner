using System;
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

        private void Update()
        {
            WriteDmxPosition(0, transform.position, true);
            WriteDmxRotation(transform.rotation.eulerAngles);
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
            //WriteDmxPosition(0, transform.position, true); // requires main thread
            //WriteDmxRotation(transform.rotation.eulerAngles);
            WriteDmxIndex(index);
        }

        #region Static
        public static GameObject pyroDronePrefab = Resources.Load<GameObject>("PyroDrone");
        private static GameObject _internalPool;
        
        public static void Spawn(FixtureSpawnManager spawnManager, ref PyroDrone[] pool, ref int count)
        {
            if (_internalPool == null) _internalPool = new GameObject("PyroDronePool");
            pool = new PyroDrone[count];
            PyroDrone fixture = null;
            int offset = (512 * 5) + 41 - 1; // 2600 is start for pyro drone.

            for (int i = 0; i < pool.Length; i++)
            {
                Spawn(ref pool, ref i, ref offset, ref fixture);
                fixture.spawnManager = spawnManager;
            }
            
            Debug.Log($"{pool.Length} pyro drones are instanced");

            SetPreset(pool, PyroDronePresetManager.presets[0]);
        }

        private static void Spawn(ref PyroDrone[] pool, ref int index, ref int offset, ref PyroDrone fixture)
        {
            var instance = Instantiate(pyroDronePrefab, new Vector3(index, 2, 0), Quaternion.identity);
            instance.transform.SetParent(_internalPool.transform);
            fixture = instance.AddComponent<PyroDrone>();
            fixture.fixtureIndex = index;
            fixture.globalChannelStart = offset + (index * fixture.GetDmxData().Length);
            fixture.gameObject.name = "PyroDrone #" + fixture.fixtureIndex;
            
            pool[index] = fixture;
        }

        private static void SetPreset(PyroDrone[] pool, PyroDronePreset[] preset)
        {
            for (var i = 0; i < preset.Length; i++)
            {
                var fixture = pool[i];
                fixture.transform.localPosition = preset[i].GetPosition();
            }
        }
        
        public static void WriteDataToGlobalBuffer(ref PyroDrone[] pool, ref byte[] globalDmxBuffer)
        {
            foreach (var pyroDrone in pool)
            {
                byte[] dmxData = pyroDrone.GetDmxData();
                
                System.Buffer.BlockCopy(dmxData, 0, 
                    globalDmxBuffer, pyroDrone.globalChannelStart, dmxData.Length);
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

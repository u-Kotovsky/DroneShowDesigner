using System;
using System.Collections.Generic;
using Runtime.Core.Resources;
using Unity_DMX.Core;
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
            Buffer = new byte[10]; // (0 -> 5) Position Coarse + Fine, (6 -> 8) Pitch + Yaw + Roll, (9) Index
            MinAngle = -360;
            MaxAngle = 360;
            minPosition = Vector3.one * -800;
            maxPosition = Vector3.one * 800;
        }

        private void Update()
        {
            try
            {
                WriteDmxPosition(0, transform.position, true);
                WriteDmxRotation(transform.rotation.eulerAngles);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
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
            WriteDmxIndex(index);
        }

        #region Static
        private static GameObject _pyroDronePrefab;
        private static GameObject _internalPool;
        private const int GlobalDmxChannelOffset = (512 * 5) + 321 - 1; // 2880 is start for FX drone // Offset is probably correct (maybe?)
        //(512 * 5) + 41 - 1; // 2600 is start for pyro drone.
        private const string Prefix = "PyroDrone";
        
        public static void InitializePrefab(Action callback = null)
        {
            if (_pyroDronePrefab != null) return;
            AssetManager.Load("PyroDrone", prefab =>
            {
                if (_pyroDronePrefab != null) return;
                _pyroDronePrefab = prefab;
                callback?.Invoke();
            });
        }
            
        public static void Spawn(FixtureSpawnManager spawnManager, ref PyroDrone[] pool, ref int count)
        {
            if (_internalPool == null) _internalPool = new GameObject("PyroDronePool");
            pool = new PyroDrone[count];
            
            for (int i = 0; i < pool.Length; i++)
            {
                Spawn(ref pool, ref i, GlobalDmxChannelOffset, out _);
            }
                
            //Debug.Log($"'{Prefix}' {pool.Length} pyro drones are instanced");

            SetPreset(pool, PyroDronePresetManager.presets[0]);
        }

        private static void Spawn(ref PyroDrone[] pool, ref int index, int offset, out PyroDrone fixture)
        {
            var instance = Instantiate(_pyroDronePrefab, new Vector3(index, 2, 0), Quaternion.identity);
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
    
        public static void WriteDataToGlobalBuffer(ref PyroDrone[] pool, ref DmxData globalDmxBuffer)
        {
            globalDmxBuffer.EnsureCapacity(GlobalDmxChannelOffset + (pool.Length * 10));
            foreach (var fixture in pool)
            {
                globalDmxBuffer.SetRange(fixture.globalChannelStart, fixture.GetDmxData());
            }

            globalDmxBuffer.EnsureCapacity(2599);
            globalDmxBuffer.Set(2598, 255); // 2598 // Enable FX Drone
        }
        #endregion
    }
}

using System;
using Runtime.Core.Resources;
using Unity_DMX.Core;
using UnityEngine;
using UnityEngine.Splines;

namespace Runtime.Dmx.Fixtures.Drones
{
    // TODO: Boundary box visuals/limit for editor usage
    public class LightingDrone : BaseDrone
    {
        #region Color
        private static readonly int ColorId = Shader.PropertyToID("_Color");
        private byte r;
        private byte g;
        private byte b;
        
        private Color color = new(0, 0, 0, 1);
        public Color Color
        {
            get => color;
            set 
            {
                color = value; 
                WriteDmxColor(value);
                UpdateMaterial(); 
            }
        }

        public void WriteDmxColor(Color value)
        {
            r = (byte)Utility.MapRange(value.r, 0, 1, 0, 255);
            g = (byte)Utility.MapRange(value.g, 0, 1, 0, 255);
            b = (byte)Utility.MapRange(value.b, 0, 1, 0, 255);
            
            Buffer[6] = r;
            Buffer[7] = g;
            Buffer[8] = b;
        }

        public void ForceSetColor(Color value)
        {
            Color = value;
            WriteDmxColor(value);
            UpdateMaterial();
        }
        #endregion
        
        private void Awake()
        {
            Buffer = new byte[9]; // (0 -> 6) Position Coarse + Fine, (7 -> 9) Color

            minPosition = Vector3.one * -800;
            maxPosition = Vector3.one * 800;

            Color = Color.black;
        }

        private void Update()
        {
            WriteDmxPosition(0, transform.position, true);
        }

        private void UpdateMaterial()
        {
            if (DroneRenderers == null) return;
            
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetColor(ColorId, Color);
            
            foreach (var droneRenderer in DroneRenderers)
            {
                if (droneRenderer == null || droneRenderer.sharedMaterial == null)
                    continue;
                
                droneRenderer.SetPropertyBlock(propertyBlock);
            }
        }

        public override void WriteDmxData()
        {
            WriteDmxColor(Color);
        }
        
        #region Static
        private static GameObject _lightingDronePrefab;
        private static GameObject _internalPool;
        private const int GlobalDmxChannelOffset = (512 * 5) + 321 - 1; // 2880 is start for FX drone // Offset is probably correct (maybe?)
        private const string Prefix = "LightingDrone";
        
        public static void InitializePrefab(Action callback = default)
        {
            if (_lightingDronePrefab != null) return;
            AssetManager.Load("LightingDrone", prefab =>
            {
                if (_lightingDronePrefab != null) return;
                _lightingDronePrefab = prefab;
                callback?.Invoke();
            });
        }
        
        public static void Spawn(FixtureSpawnManager spawnManager, ref LightingDrone[] pool, ref int count, ref SplineContainer splineContainer)
        {
            if (_internalPool == null) _internalPool = new GameObject("LightingDronePool");
            pool = new LightingDrone[count];
            
            for (int i = 0; i < pool.Length; i++)
            {
                Spawn(ref pool, ref i, GlobalDmxChannelOffset, out _);
            }
            
            //Debug.Log($"'{Prefix}' {pool.Length} lighting drones are instanced");
        }

        private static void Spawn(ref LightingDrone[] pool, ref int index, int offset, out LightingDrone fixture)
        {
            var instance = Instantiate(_lightingDronePrefab, new Vector3(index, 1, 0), Quaternion.identity);
            instance.transform.SetParent(_internalPool.transform);
            fixture = instance.AddComponent<LightingDrone>();
            fixture.fixtureIndex = index;
            fixture.globalChannelStart = offset + (index * fixture.GetDmxData().Length);
            fixture.gameObject.name = "LightingDrone #" + fixture.fixtureIndex;
            pool[index] = fixture;
        }

        public static void WriteDataToGlobalBuffer(ref LightingDrone[] pool, ref DmxData globalDmxBuffer)
        {
            globalDmxBuffer.EnsureCapacity(GlobalDmxChannelOffset + (pool.Length * 9));
            foreach (var fixture in pool)
            {
                globalDmxBuffer.SetRange(fixture.globalChannelStart, fixture.GetDmxData());
            }

            globalDmxBuffer.EnsureCapacity((512 * 5) + 246 + 1);
            
            globalDmxBuffer.Set(2802, 255); // Enable Misc Control
            globalDmxBuffer.Set(2804, 255); // Enable Huge Drone Map
        }
        #endregion
    }
}

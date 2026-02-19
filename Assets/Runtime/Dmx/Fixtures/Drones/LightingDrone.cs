using System;
using System.Collections.Generic;
using Runtime.Core.Resources;
using Runtime.Dmx.Fixtures.Drones.Movers;
using Runtime.Dmx.Fixtures.Drones.Movers.MeshToDrone;
using Unity_DMX.Core;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

namespace Runtime.Dmx.Fixtures.Drones
{
    // TODO: Boundary box visuals/limit for editor usage
    public class LightingDrone : BaseDrone
    {
        //public RendererToDrones meshToDrones;

        private DroneFromMesh droneFromMesh;
        private bool isDroneFromMeshNull = true;
        public DroneFromMesh DroneFromMesh
        {
            get => droneFromMesh;
            set
            {
                isDroneFromMeshNull = value == null;
                droneFromMesh = value;
            }
        }

        #region Color
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
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

            minPosition = new Vector3(-800, -800, -800);
            maxPosition = new Vector3(800, 800, 800);

            Color = Color.black;
        }

        private void Update()
        {
            if (!isDroneFromMeshNull && DroneFromMesh.isActiveAndEnabled)
            {
                DroneFromMesh.SetTransform(this);
            }
            
            WriteDmxPosition(0, transform.position, true);
        }

        private void UpdateMaterial()
        {
            if (DroneRenderers == null) return;
            
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            propertyBlock.SetColor(BaseColor, Color);
            
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
            LightingDrone fixture = null;
            
            // test mesh 
            var droneFromMesh = FindFirstObjectByType<DroneFromMesh>();
            var balloonDrones = FindFirstObjectByType<BalloonDrones>();

            for (int i = 0; i < pool.Length; i++)
            {
                Spawn(ref pool, ref i, GlobalDmxChannelOffset, out fixture);
                fixture.spawnManager = spawnManager;
                //fixture.gameObject.AddComponent<DroneNavigation>();
                //var pathNav = fixture.gameObject.AddComponent<DronePathNavigation>();
                //pathNav.waitBeforeStart = i * 0.25f;
                fixture.DroneFromMesh = droneFromMesh;
                //var cartFollower = fixture.gameObject.AddComponent<DroneSplineCartFollower>();
                //cartFollower.StartWithDelay(i * 0.2f, splineContainer);
            }
            
            Debug.Log($"'{Prefix}' {pool.Length} lighting drones are instanced");

            if (droneFromMesh != null && droneFromMesh.isActiveAndEnabled)
            {
                Debug.Log($"'{Prefix}' Setup DroneFromMesh");
                droneFromMesh.OnAllDronesReady(pool);
            }

            if (balloonDrones != null && balloonDrones.isActiveAndEnabled)
            {
                Debug.Log($"'{Prefix}' Setup Balloon Drones");
                balloonDrones.Setup(pool);
            }
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
            foreach (var fixture in pool)
            {
                var data = new List<byte>(fixture.GetDmxData());

                globalDmxBuffer.EnsureCapacity(fixture.globalChannelStart + data.Count + (512));
                globalDmxBuffer.SetRange(fixture.globalChannelStart, data);
            }

            WriteSpecialData(ref globalDmxBuffer);
        }

        private static void WriteSpecialData(ref DmxData buffer)
        {
            int offset = 512 * 5; // 2560 - 1 = 2559

            buffer.EnsureCapacity(offset + 246 + 1);
            
            buffer.Set(2802, 255); // Enable Misc Control
            buffer.Set(2804, 255); // Enable Huge Drone Map
        }
        #endregion
    }
}

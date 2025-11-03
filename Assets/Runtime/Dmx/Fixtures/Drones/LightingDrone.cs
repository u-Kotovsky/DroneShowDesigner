using System;
using UnityEngine;
using UnityEngine.Splines;

namespace Runtime.Dmx.Fixtures.Drones
{
    // TODO: Boundary box visuals/limit for editor usage
    public class LightingDrone : BaseDrone
    {
        #region Color
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private byte r;
        private byte g;
        private byte b;
        
        public Color color = new Color(0, 0, 0, 1);

        public void WriteDmxColor(Color value)
        {
            r = (byte)Utility.MapRange(value.r, 0, 1, 0, 255);
            g = (byte)Utility.MapRange(value.g, 0, 1, 0, 255);
            b = (byte)Utility.MapRange(value.b, 0, 1, 0, 255);
            
            Buffer[6] = r;
            Buffer[7] = g;
            Buffer[8] = b;
            
            if (DroneRenderers == null) return;
            foreach (var droneRenderer in DroneRenderers)
            {
                if (droneRenderer == null || droneRenderer.sharedMaterial == null)
                    continue;
                
                droneRenderer.sharedMaterial.SetColor(BaseColor, color);
            }
        }
        #endregion
        
        private void Awake()
        {
            Buffer = new byte[9]; // (0 -> 6) Position Coarse + Fine, (7 -> 9) Color

            MinPosition = new Vector3(-800, -800, -800);
            MaxPosition = new Vector3(800, 800, 800);
        }

        private void Update()
        {
            WriteDmxPosition(0, transform.position, true);
        }

        public override void WriteDmxData()
        {
            //WriteDmxPosition(0, transform.position, true); // requires to be run from mainthread
            WriteDmxColor(color);
        }
        
        #region Static
        public static GameObject lightingDronePrefab = Resources.Load<GameObject>("LightingDrone");
        private static GameObject internalPool;
        
        public static void Spawn(FixtureSpawnManager spawnManager, ref LightingDrone[] pool, ref int count, ref SplineContainer splineContainer)
        {
            if (internalPool == null) internalPool = new GameObject("LightingDronePool");
            pool = new LightingDrone[count];
            LightingDrone fixture = null;
            int offset = (512 * 5) + 321 - 1; // 2880 is start for FX drone // Offset is probably correct (maybe?)

            for (int i = 0; i < pool.Length; i++)
            {
                Spawn(ref pool, ref i, ref offset, ref fixture);
                fixture.spawnManager = spawnManager;
                //fixture.gameObject.AddComponent<DroneNavigation>();
                //var pathNav = fixture.gameObject.AddComponent<DronePathNavigation>();
                //pathNav.waitBeforeStart = i * 0.25f;
                var cartFollower = fixture.gameObject.AddComponent<DroneSplineCartFollower>();
                cartFollower.StartWithDelay(i * 0.2f, splineContainer);
            }
            
            Debug.Log($"{pool.Length} lighting drones are instanced");
        }

        private static void Spawn(ref LightingDrone[] pool, ref int index, ref int offset, ref LightingDrone fixture)
        {
            var instance = Instantiate(lightingDronePrefab, new Vector3(index, 1, 0), Quaternion.identity);
            instance.transform.SetParent(internalPool.transform);
            fixture = instance.AddComponent<LightingDrone>();
            fixture.fixtureIndex = index;
            fixture.globalChannelStart = offset + (index * fixture.GetDmxData().Length);
            fixture.gameObject.name = "LightingDrone #" + fixture.fixtureIndex;
            pool[index] = fixture;
        }
        
        public static void WriteDataToGlobalBuffer(ref LightingDrone[] pool, ref byte[] globalDmxBuffer)
        {
            foreach (var lightingDrone in pool)
            {
                byte[] droneData = lightingDrone.GetDmxData();

                System.Buffer.BlockCopy(droneData, 0, 
                    globalDmxBuffer, lightingDrone.globalChannelStart, droneData.Length);
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

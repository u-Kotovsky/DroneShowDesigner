using System;
using Fixtures.Drones;
using Fixtures.Lights;
using Fixtures.Truss;
using Unity_DMX.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Fixtures
{
    public class FixtureSpawnManager : MonoBehaviour
    {
        public DmxController dmxController;
        
        [Header("Fixtures to enable on start")]
        public bool usePyroDrone = false;
        public bool useLightingDrone = false;
        public bool useMobileTruss = true;
        public bool useMobileLight = true;
        
        [Header("Drone Spawn Settings")]
        public GameObject dronePrefab;
        public GameObject[] lightingDronePool;
        public GameObject[] pyroDronePool;

        public int lightingDroneSpawnCount = 1000;
        public int pyroDroneSpawnCount = 16;
        public float droneMargin = 1;
        
        
        [Header("Mobile Truss Spawn Settings")]
        public GameObject mobileTrussPrefab;
        public GameObject[] mobileTrussPool;
        public int mobileTrussSpawnCount = 12;

        [Header("Mobile Light Spawn Settings")]
        public GameObject mobileLightPrefab;
        public GameObject[] mobileLightPool;
        public int mobileLightSpawnCount = 8;
    
        private byte[] globalDmxBuffer;
    
        private void Start()
        {
            globalDmxBuffer = new byte[512 * 40]; // ~9000 channels
            
            if (usePyroDrone)
                SpawnPyroDrones();
            if (useLightingDrone)
                SpawnLightingDrones();
            if (useMobileTruss)
                SpawnMobileTruss();
            if (useMobileLight)
                SpawnMobileLight();

            Application.targetFrameRate = 60;
            Application.runInBackground = true;
        
            lightingDronePool[0].transform.parent.localPosition = new Vector3(-15, 20, 50);
        
            int counter = 0;
            for (var i = 0; i < 500; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    if (lightingDronePool.Length <= counter) return;
                    var drone = lightingDronePool[counter];
                
                    drone.transform.localPosition = new Vector3(j, i, 0);
                    drone.GetComponent<LightingDrone>().color = Color.white;
                
                    counter++;
                }
            }
        }

        private void Update()
        {
            if (usePyroDrone)
            {
                WritePyroDroneDataToGlobalBuffer();
                WritePyroSpecialData(globalDmxBuffer);
            }

            if (useLightingDrone)
            {
                WriteLightingDroneDataToGlobalBuffer();
                WriteLightingSpecialData(globalDmxBuffer);
            }

            if (useMobileTruss)
            {
                WriteMobileTrussDataToGlobalBuffer();
                WriteTrussSpecialData(globalDmxBuffer);
            }

            if (useMobileLight)
            {
                WriteMobileLightDataToGlobalBuffer();
                WriteLightSpecialData(globalDmxBuffer); // doesn't have it?
            }
            
            SendGlobalBuffer();
        }

        private void SendGlobalBuffer()
        {
            byte[] dmxBuffer = new byte[512];
            for (var i = 0; i < globalDmxBuffer.Length / 512; i++)
            {
                Buffer.BlockCopy(globalDmxBuffer, i * 512, 
                    dmxBuffer, 0, 512);
                dmxController.Send((short)i, dmxBuffer);
            }
        }

        private void GenerateNoise(short universe = 0, byte min = 0, byte max = 255)
        {
            byte[] buffer = new byte[511];
            for (var i = 0; i < buffer.Length; i++)
                buffer[i] = (byte)Random.Range(min, max);
            dmxController.Send(universe, buffer);
        }

        #region Mobile Lights
        private void WriteLightSpecialData(byte[] buffer)
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
        
        public void SpawnMobileLight()
        {
            GameObject pool = new GameObject("MobileLightPool");
            mobileLightPool = new GameObject[mobileLightSpawnCount];
            MobileLight fixture;
            int offset = 1077; // Start for mobile light.

            for (int i = 0; i < mobileLightPool.Length; i++)
            {
                mobileLightPool[i] = Instantiate(mobileLightPrefab, new Vector3(i * droneMargin, 2, 0), Quaternion.identity);
                mobileLightPool[i].transform.SetParent(pool.transform);
                fixture = mobileLightPool[i].AddComponent<MobileLight>();
                fixture.fixtureIndex = i;
                fixture.spawnManager = this;
                fixture.globalChannelStart = offset + (i * fixture.GetDmxData().Length);
                fixture.gameObject.name = "MobileLight #" + fixture.fixtureIndex;
            }
            
            Debug.Log($"{mobileLightPool.Length} mobile lights are instanced");
        }
        
        public void WriteMobileLightDataToGlobalBuffer()
        {
            foreach (var obj in mobileLightPool)
            {
                var fixture = obj.GetComponent<MobileLight>();
                byte[] data = fixture.GetDmxData();
                
                Buffer.BlockCopy(data, 0, 
                    globalDmxBuffer, fixture.globalChannelStart, data.Length);
            }
        }
        #endregion
        
        #region Mobile Truss
        private void WriteTrussSpecialData(byte[] buffer)
        {
            buffer[5] = 255;
        }
        
        public void SpawnMobileTruss()
        {
            GameObject pool = new GameObject("MobileTrussPool");
            mobileTrussPool = new GameObject[mobileTrussSpawnCount];
            MobileTruss fixture;
            int offset = 6; // Start for mobile truss.

            for (int i = 0; i < mobileTrussPool.Length; i++)
            {
                mobileTrussPool[i] = Instantiate(mobileTrussPrefab, new Vector3(i * droneMargin * 9, 2, 0), Quaternion.identity);
                mobileTrussPool[i].transform.SetParent(pool.transform);
                fixture = mobileTrussPool[i].AddComponent<MobileTruss>();
                fixture.fixtureIndex = i;
                fixture.spawnManager = this;
                fixture.globalChannelStart = offset + (i * fixture.GetDmxData().Length);
                fixture.gameObject.name = "MobileTruss #" + fixture.fixtureIndex;
                var nav = fixture.gameObject.AddComponent<MobileTrussNavigation>();
                nav.playTrussPresetSwap = true;
            }
            
            Debug.Log($"{mobileTrussPool.Length} mobile trusses are instanced");
        
            SetTrussPreset(MobileTrussPresetManager.trussPresets[0]);
        }

        private void SetTrussPreset(TrussPreset[] preset)
        {
            for (var i = 0; i < preset.Length; i++)
            {
                GameObject truss1 = mobileTrussPool[i];
                truss1.transform.localPosition = preset[i].GetPosition();
                truss1.transform.localRotation = preset[i].GetRotation();
            }
        }
        
        public void WriteMobileTrussDataToGlobalBuffer()
        {
            foreach (var drone in mobileTrussPool)
            {
                var truss = drone.GetComponent<MobileTruss>();
                byte[] trussData = truss.GetDmxData();
                
                Buffer.BlockCopy(trussData, 0, 
                    globalDmxBuffer, truss.globalChannelStart, trussData.Length);
            }
        }
        #endregion
        
        #region Pyro Drones
        private void WritePyroSpecialData(byte[] buffer)
        {
            int offset = 512 * 5 - 1;
            buffer[offset + 39] = 255;
            buffer[offset + 40] = 0;
            buffer[offset + 243] = 255;
            buffer[offset + 244] = 0;
            buffer[offset + 245] = 255;
            buffer[offset + 246] = 255;
        }
        
        public void SpawnPyroDrones()
        {
            GameObject pool = new GameObject("PyroDronePool");
            pyroDronePool = new GameObject[pyroDroneSpawnCount];
            PyroDrone fixture;
            int offset = (512 * 5) + 41 - 1; // 2600 is start for pyro drone.

            for (int i = 0; i < pyroDronePool.Length; i++)
            {
                pyroDronePool[i] = Instantiate(dronePrefab, new Vector3(i * droneMargin, 2, 0), Quaternion.identity);
                pyroDronePool[i].transform.SetParent(pool.transform);
                fixture = pyroDronePool[i].AddComponent<PyroDrone>();
                fixture.fixtureIndex = i;
                fixture.spawnManager = this;
                fixture.globalChannelStart = offset + (i * fixture.GetDmxData().Length);
                fixture.gameObject.name = "PyroDrone #" + fixture.fixtureIndex;
            }
            
            Debug.Log($"{pyroDronePool.Length} pyro drones are instanced");
        }
        
        public void WritePyroDroneDataToGlobalBuffer()
        {
            foreach (var drone in pyroDronePool)
            {
                var pyroDrone = drone.GetComponent<PyroDrone>();
                byte[] droneData = pyroDrone.GetDmxData();
                
                Buffer.BlockCopy(droneData, 0, 
                    globalDmxBuffer, pyroDrone.globalChannelStart, droneData.Length);
            }
        }
        #endregion

        #region Lighting Drones
        private void WriteLightingSpecialData(byte[] buffer)
        {
            int offset = 512 * 5 - 1;
            buffer[offset + 39] = 255;
            buffer[offset + 40] = 0;
            buffer[offset + 243] = 255;
            buffer[offset + 244] = 0;
            buffer[offset + 245] = 255;
            buffer[offset + 246] = 255;
        }
        
        public void SpawnLightingDrones()
        {
            GameObject pool = new GameObject("LightingDronePool");
            lightingDronePool = new GameObject[lightingDroneSpawnCount];
            LightingDrone fixture;
            int offset = (512 * 5) + 321 - 1; // 2880 is start for FX drone // Offset is probably correct (maybe?)

            for (int i = 0; i < lightingDronePool.Length; i++)
            {
                lightingDronePool[i] = Instantiate(dronePrefab, new Vector3(i * droneMargin, 1, 0), Quaternion.identity);
                lightingDronePool[i].transform.SetParent(pool.transform);
                fixture = lightingDronePool[i].AddComponent<LightingDrone>();
                fixture.fixtureIndex = i;
                fixture.spawnManager = this;
                fixture.globalChannelStart = offset + (i * fixture.GetDmxData().Length);
                fixture.gameObject.AddComponent<DroneNavigation>();
                fixture.gameObject.name = "PyroDrone #" + fixture.fixtureIndex;
            }
            
            Debug.Log($"{lightingDronePool.Length} lighting drones are instanced");
        }
        
        public void WriteLightingDroneDataToGlobalBuffer()
        {
            foreach (var drone in lightingDronePool)
            {
                var lightingDrone = drone.GetComponent<LightingDrone>();
                byte[] droneData = lightingDrone.GetDmxData();

                Buffer.BlockCopy(droneData, 0, 
                    globalDmxBuffer, lightingDrone.globalChannelStart, droneData.Length);
            }
        }
        #endregion
    }
}
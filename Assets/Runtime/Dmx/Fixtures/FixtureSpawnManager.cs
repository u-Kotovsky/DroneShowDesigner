using System;
using Runtime.Dmx.Fixtures.Drones;
using Runtime.Dmx.Fixtures.Lights;
using Runtime.Dmx.Fixtures.Truss;
using Unity_DMX.Core;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

namespace Runtime.Dmx.Fixtures
{
    public class FixtureSpawnManager : MonoBehaviour
    {
        public DmxController dmxController;

        [Header("Cinemachine")] 
        public CinemachineSplineCart cinemachineSplineCart;
        public SplineContainer SplineContainer;
        
        [Header("Fixtures to enable on start")]
        public bool usePyroDrone = false;
        public bool useLightingDrone = false;
        public bool useMobileTruss = true;
        public bool useMobileLight = true;
        
        [Header("Drone Spawn Settings")]
        public float droneMargin = 1;
        
        [Header("Pyro Drone Spawn Settings")]
        public GameObject pyroDronePrefab;
        public int pyroDroneSpawnCount = 16;
        public GameObject[] pyroDronePool;
        
        [Header("Lighting Drone Spawn Settings")]
        public GameObject lightingDronePrefab;
        public int lightingDroneSpawnCount = 1000;
        public GameObject[] lightingDronePool;
        
        [Header("Mobile Truss Spawn Settings")]
        public GameObject mobileTrussPrefab;
        public int mobileTrussSpawnCount = 12;
        public GameObject[] mobileTrussPool;

        [Header("Mobile Light Spawn Settings")]
        public GameObject mobileLightPrefab;
        public int mobileLightSpawnCount = 8;
        public GameObject[] mobileLightPool;
    
        private byte[] globalDmxBuffer;
    
        private void Start()
        {
            globalDmxBuffer = new byte[512 * 40]; // ~9000 channels
            
            Application.targetFrameRate = 60;
            Application.runInBackground = true;
            
            if (usePyroDrone)
                PyroDrone.Spawn(this, ref pyroDronePool, ref pyroDroneSpawnCount);
            if (useLightingDrone)
                LightingDrone.Spawn(this, ref lightingDronePool, ref lightingDroneSpawnCount, ref SplineContainer);
            if (useMobileTruss)
                MobileTruss.Spawn(this, ref mobileTrussPool, ref mobileTrussSpawnCount);
            if (useMobileLight)
                MobileLight.Spawn(this, ref mobileLightPool, ref mobileLightSpawnCount);

            if (useLightingDrone)
            {
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
        }

        private void Update()
        {
            if (usePyroDrone)
                PyroDrone.WriteDataToGlobalBuffer(ref pyroDronePool, ref globalDmxBuffer);

            if (useLightingDrone)
            {
                LightingDrone.WriteDataToGlobalBuffer(ref lightingDronePool, ref globalDmxBuffer);
            }

            if (useMobileTruss)
            {
                MobileTruss.WriteDataToGlobalBuffer(ref mobileTrussPool, ref globalDmxBuffer);
            }

            if (useMobileLight)
            {
                MobileLight.WriteDataToGlobalBuffer(ref mobileLightPool, ref globalDmxBuffer);
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
    }
}
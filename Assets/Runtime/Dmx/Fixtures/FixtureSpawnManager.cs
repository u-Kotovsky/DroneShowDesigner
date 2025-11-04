using System;
using Runtime.Dmx.Fixtures.Drones;
using Runtime.Dmx.Fixtures.Lights;
using Runtime.Dmx.Fixtures.Truss;
using Unity_DMX.Core;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

namespace Runtime.Dmx.Fixtures
{
    public class FixtureSpawnManager : MonoBehaviour
    {
        public DmxController dmxController;

        [Header("Cinemachine")] 
        public SplineContainer splineContainer;
        
        [Header("Fixtures to enable on start")]
        public bool usePyroDrone;
        public bool useLightingDrone;
        public bool useMobileTruss = true;
        public bool useMobileLight = true;
        
        [Header("Pyro Drone Spawn Settings")]
        public int pyroDroneSpawnCount = 16;
        public PyroDrone[] pyroDronePool;
        
        [Header("Lighting Drone Spawn Settings")]
        public int lightingDroneSpawnCount = 1000;
        public LightingDrone[] lightingDronePool;
        
        [Header("Mobile Truss Spawn Settings")]
        public int mobileTrussSpawnCount = 12;
        public MobileTruss[] mobileTrussPool;

        [Header("Mobile Light Spawn Settings")]
        public int mobileLightSpawnCount = 8;
        public MobileLight[] mobileLightPool;
    
        private void Awake()
        {
            try
            {
                Application.targetFrameRate = 60;
                Application.runInBackground = true;
                
                dmxController.OnDmxDataChanged += OnDmxDataChanged;
                
                if (usePyroDrone)
                    PyroDrone.Spawn(this, ref pyroDronePool, ref pyroDroneSpawnCount);
                if (useLightingDrone)
                    LightingDrone.Spawn(this, ref lightingDronePool, ref lightingDroneSpawnCount, ref splineContainer);
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
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void OnDmxDataChanged(short universe, byte[] data, byte[] globalDmxBuffer)
        {
            WriteDmxData(ref globalDmxBuffer);
        }
        
        private void WriteDmxData(ref byte[] buffer)
        {
            if (usePyroDrone)
            {
                PyroDrone.WriteDataToGlobalBuffer(ref pyroDronePool, ref buffer);
            }

            if (useLightingDrone)
            {
                LightingDrone.WriteDataToGlobalBuffer(ref lightingDronePool, ref buffer);
            }

            if (useMobileTruss)
            {
                MobileTruss.WriteDataToGlobalBuffer(ref mobileTrussPool, ref buffer);
            }

            if (useMobileLight)
            {
                MobileLight.WriteDataToGlobalBuffer(ref mobileLightPool, ref buffer);
            }
        }
    }
}
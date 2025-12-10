using System;
using System.Collections.Generic;
using Runtime.Dmx.Fixtures.Drones;
using Runtime.Dmx.Fixtures.Drones.Movers;
using Runtime.Dmx.Fixtures.Lights;
using Runtime.Dmx.Fixtures.Truss;
using Unity_DMX.Core;
using UnityEngine;
using UnityEngine.Splines;

namespace Runtime.Dmx.Fixtures
{
    public class FixtureSpawnManager : MonoBehaviour
    {
        public DmxController dmxController;

        [Header("Cinemachine")] 
        public SplineContainer splineContainer;
        
        [Header("Fixtures to enable on start")]
        private bool usePyroDrone;
        public bool UsePyroDrone
        {
            get => usePyroDrone;
            set
            {
                TogglePool(pyroDronePool, value);
                usePyroDrone = value;
            }
        }
        
        private bool useLightingDrone;
        public bool UseLightingDrone
        {
            get => useLightingDrone;
            set
            {
                TogglePool(lightingDronePool, value);
                useLightingDrone = value;
            }
        }
        
        private bool useMobileTruss;
        public bool UseMobileTruss
        {
            get => useMobileTruss;
            set
            {
                TogglePool(mobileTrussPool, value);
                useMobileTruss = value;
            }
        }

        private bool useMobileLight;
        public bool UseMobileLight
        {
            get => useMobileLight;
            set
            {
                TogglePool(mobileLightPool, value);
                useMobileLight = value;
            }
        }

        private void TogglePool(Component[] pool, bool active)
        {
            foreach (var obj in pool)
                if (obj != null) obj.gameObject.SetActive(active);
        }
        
        public bool IsMobileTrussInitialized { get; private set; }
        public bool IsMobileLightInitialized { get; private set; }
        public bool IsPyroDroneInitialized { get; private set; }
        public bool IsLightingDroneInitialized { get; private set; }
        
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

        private void InitializeMobileTruss()
        {
            MobileTruss.InitializePrefab(() =>
            {
                MobileTruss.Spawn(this, ref mobileTrussPool, ref mobileTrussSpawnCount);
                TogglePool(mobileTrussPool, false);
                IsMobileTrussInitialized = true;
            });
        }

        private void InitializeMobileLight()
        {
            MobileLight.InitializePrefab(() =>
            {
                MobileLight.Spawn(this, ref mobileLightPool, ref mobileLightSpawnCount);
                TogglePool(mobileLightPool, false);
                IsMobileLightInitialized = true;
            });
        }

        private void InitializePyroDrones()
        {
            PyroDrone.InitializePrefab(() =>
            {
                PyroDrone.Spawn(this, ref pyroDronePool, ref pyroDroneSpawnCount);
                TogglePool(pyroDronePool, false);
                IsPyroDroneInitialized = true;
            });
        }

        private void InitializeLightingDrones()
        {
            LightingDrone.InitializePrefab(() =>
            {
                LightingDrone.Spawn(this, ref lightingDronePool, ref lightingDroneSpawnCount, ref splineContainer);
                IsLightingDroneInitialized = true;
                
                TogglePool(lightingDronePool, false);
                
                // Post-setup
                //lightingDronePool[0].transform.parent.localPosition = new Vector3(0, 10, 0);
                
                /*for (var i = 0; i < lightingDronePool.Length; i++)
                {
                    var drone = lightingDronePool[i];
                    var component = drone.gameObject.AddComponent<DroneTakeoff>();
                }*/
                
                /*var counter = 0;
                var size = Mathf.Sqrt(lightingDronePool.Length);
                var offset = size / 2;
                
                for (var z = 0; z < size; z++)
                {
                    for (var x = 0; x < size; x++)
                    {
                        if (lightingDronePool.Length <= counter) return;
                        var drone = lightingDronePool[counter];
                        //var component = drone.gameObject.AddComponent<DroneTakeoff>();

                        //component.xIndex = x;
                        //component.zIndex = z;

                        //var c = counter % 5;
                        //transform.localPosition = new Vector3(x - offset, 10.6f, y - offset); // 700 + (c * 5)
                        //component.SetPositionTakeoffStart(lightingDronePool.Length, size, offset, x, 0.6f, z);
                    
                        counter++;
                    }
                }*/
            });
        }

        public void Awake()
        {
            dmxController.OnDmxDataChanged += OnDmxDataChanged;
            
            InitializeMobileTruss();
            InitializeMobileLight();
            InitializePyroDrones();
            InitializeLightingDrones();
        }

        private void Update()
        {
            WriteDmxData(ref dmxController.dmxBuffer.Buffer); // makes everything update each unity frame
            dmxController.ForceBufferUpdate(); // Maybe causing random dmx shuffle
        }

        private void OnDmxDataChanged(short universe, DmxData data, DmxData globalDmxBuffer)
        {
            WriteDmxData(ref globalDmxBuffer);
        }
        
        private void WriteDmxData(ref DmxData buffer)
        {
            // TODO: Once a feature turned off, send data that cancels these features in dmx buffer
            if (UseMobileTruss && IsMobileTrussInitialized)
            {
                MobileTruss.WriteDataToGlobalBuffer(ref mobileTrussPool, ref buffer);
            }

            if (UseMobileLight && IsMobileLightInitialized)
            {
                MobileLight.WriteDataToGlobalBuffer(ref mobileLightPool, ref buffer);
            }
            
            if (UsePyroDrone && IsPyroDroneInitialized)
            {
                PyroDrone.WriteDataToGlobalBuffer(ref pyroDronePool, ref buffer);
            }

            if (UseLightingDrone && IsLightingDroneInitialized)
            {
                LightingDrone.WriteDataToGlobalBuffer(ref lightingDronePool, ref buffer);
            }
        }
    }
}
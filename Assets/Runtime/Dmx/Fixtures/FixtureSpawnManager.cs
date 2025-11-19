using Runtime.Dmx.Fixtures.Drones;
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
                if (usePyroDrone != value)
                {
                    TogglePool(pyroDronePool, value);
                    usePyroDrone = value;
                }
            }
        }
        
        private bool useLightingDrone;
        public bool UseLightingDrone
        {
            get => useLightingDrone;
            set
            {
                if (useLightingDrone != value)
                {
                    TogglePool(lightingDronePool, value);
                    useLightingDrone = value;
                }
            }
        }
        
        private bool useMobileTruss;
        public bool UseMobileTruss
        {
            get => useMobileTruss;
            set
            {
                if (useMobileTruss != value)
                {
                    TogglePool(mobileTrussPool, value);
                    useMobileTruss = value;
                }
            }
        }

        private bool useMobileLight;
        public bool UseMobileLight
        {
            get => useMobileLight;
            set
            {
                if (useMobileLight != value)
                {
                    TogglePool(mobileLightPool, value);
                    useMobileLight = value;
                }
            }
        }

        #region TogglePool
        
        private void TogglePool(PyroDrone[] pool, bool active)
        {
            foreach (var obj in pool)
                if (obj != null) obj.gameObject.SetActive(active);
        }

        private void TogglePool(LightingDrone[] pool, bool active)
        {
            foreach (var obj in pool)
                if (obj != null) obj.gameObject.SetActive(active);
        }

        private void TogglePool(MobileLight[] pool, bool active)
        {
            foreach (var obj in pool)
                if (obj != null) obj.gameObject.SetActive(active);
        }

        private void TogglePool(MobileTruss[] pool, bool active)
        {
            foreach (var obj in pool)
                if (obj != null) obj.gameObject.SetActive(active);
        }

        #endregion
        
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

        public void InitializeMobileTruss()
        {
            MobileTruss.Spawn(this, ref mobileTrussPool, ref mobileTrussSpawnCount);
            IsMobileTrussInitialized = true;
        }

        public void InitializeMobileLight()
        {
            MobileLight.Spawn(this, ref mobileLightPool, ref mobileLightSpawnCount);
            IsMobileLightInitialized = true;
        }

        public void InitializePyroDrones()
        {
            PyroDrone.Spawn(this, ref pyroDronePool, ref pyroDroneSpawnCount);
            IsPyroDroneInitialized = true;
        }

        public void InitializeLightingDrones()
        {
            LightingDrone.Spawn(this, ref lightingDronePool, ref lightingDroneSpawnCount, ref splineContainer);
            IsLightingDroneInitialized = true;
            
            // Post-setup
            lightingDronePool[0].transform.parent.localPosition = new Vector3(0, 10, 0);
        
            int counter = 0;
            //var (rectWidth, rectHeight) = Utility.GetMostRectangular(lightingDronePool.Length);
            var size = Mathf.Sqrt(lightingDronePool.Length);
            var offset = size / 2;
            
            for (var y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (lightingDronePool.Length <= counter) return;
                    var drone = lightingDronePool[counter];

                    var c = counter % 5;
                
                    drone.transform.localPosition = new Vector3(x - offset, c * 5, y - offset);

                    switch (c)
                    {
                        case 0:
                            drone.color = Color.red;
                            break;
                        case 1:
                            drone.color = Color.yellow;
                            break;
                        case 2:
                            drone.color = Color.green;
                            break;
                        case 3:
                            drone.color = Color.cyan;
                            break;
                        case 4:
                            drone.color = Color.blue;
                            break;
                    }
                
                    counter++;
                }
            }
        }

        public void Awake()
        {
            dmxController.OnDmxDataChanged += OnDmxDataChanged;
            
            MobileTruss.Poke();
            MobileLight.Poke();
            PyroDrone.Poke();
            LightingDrone.Poke();
            
            InitializeMobileTruss();
            InitializeMobileLight();
            InitializePyroDrones();
            InitializeLightingDrones();
        }

        private void Update()
        {
            WriteDmxData(ref dmxController.dmxBuffer.buffer); // makes everything update each unity frame
            dmxController.ForceBufferUpdate();
        }

        private void OnDmxDataChanged(short universe, byte[] data, byte[] globalDmxBuffer)
        {
            WriteDmxData(ref globalDmxBuffer);
        }
        
        private void WriteDmxData(ref byte[] buffer)
        {
            if (UsePyroDrone)
            {
                PyroDrone.WriteDataToGlobalBuffer(ref pyroDronePool, ref buffer);
            }

            if (UseLightingDrone)
            {
                LightingDrone.WriteDataToGlobalBuffer(ref lightingDronePool, ref buffer);
            }

            if (UseMobileTruss)
            {
                MobileTruss.WriteDataToGlobalBuffer(ref mobileTrussPool, ref buffer);
            }

            if (UseMobileLight)
            {
                MobileLight.WriteDataToGlobalBuffer(ref mobileLightPool, ref buffer);
            }
        }
    }
}
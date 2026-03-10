using System;
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

        private bool useInternalRender = false;
        public bool UseInternalRender
        {
            get => useInternalRender;
            set
            {
                useInternalRender = value;
                ToggleRenderPool(lightingDronePool, useInternalRender);
                ToggleRenderPool(pyroDronePool, useInternalRender);
                ToggleRenderPool(mobileTrussPool, useInternalRender);
                ToggleRenderPool(mobileLightPool, useInternalRender);
            }
        }
        
        private void ToggleRenderPool(Component[] pool, bool active)
        {
            foreach (var obj in pool)
            {
                if (obj == null)  continue;

                if (obj.TryGetComponent<Renderer>(out var objRenderer))
                {
                    objRenderer.enabled = active;
                }

                if (obj.transform.childCount > 0)
                {
                    foreach (Transform o in obj.transform)
                    {
                        if (o.TryGetComponent<Renderer>(out var oRenderer))
                        {
                            oRenderer.enabled = active;
                        }
                    }
                }
            }
        }
        
        [Header("Pyro Drone Spawn Settings")]
        public int pyroDroneSpawnCount = 16;
        public PyroDrone[] pyroDronePool;
        public bool IsPyroDroneInitialized { get; private set; }
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
        private void InitializePyroDrones()
        {
            PyroDrone.InitializePrefab(() =>
            {
                PyroDrone.Spawn(this, ref pyroDronePool, ref pyroDroneSpawnCount);
                IsPyroDroneInitialized = true;
                CheckForFixturesInitialized();
            });
        }
        
        public event Action<LightingDrone[]> OnLightingDronesEnabled;
        public event Action<LightingDrone[]> OnLightingDronesDisabled;
        
        [Header("Lighting Drone Spawn Settings")]
        public int lightingDroneSpawnCount = 1000;
        public LightingDrone[] lightingDronePool;
        public bool IsLightingDroneInitialized { get; private set; }
        private bool useLightingDrone;
        public bool UseLightingDrone
        {
            get => useLightingDrone;
            set
            {
                TogglePool(lightingDronePool, value);
                useLightingDrone = value;
                if (value)
                {
                    OnLightingDronesEnabled?.Invoke(lightingDronePool);
                }
                else
                {
                    OnLightingDronesDisabled?.Invoke(lightingDronePool);
                }
            }
        }
        private void InitializeLightingDrones()
        {
            LightingDrone.InitializePrefab(() =>
            {
                LightingDrone.Spawn(this, ref lightingDronePool, ref lightingDroneSpawnCount, ref splineContainer);
                IsLightingDroneInitialized = true;
                CheckForFixturesInitialized();
            });
        }
        
        [Header("Mobile Truss Spawn Settings")]
        public int mobileTrussSpawnCount = 12;
        public MobileTruss[] mobileTrussPool;
        public bool IsMobileTrussInitialized { get; private set; }
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
        private void InitializeMobileTruss()
        {
            MobileTruss.InitializePrefab(() =>
            {
                MobileTruss.Spawn(this, ref mobileTrussPool, ref mobileTrussSpawnCount);
                IsMobileTrussInitialized = true;
                CheckForFixturesInitialized();
            });
        }

        [Header("Mobile Light Spawn Settings")]
        public int mobileLightSpawnCount = 8;
        public MobileLight[] mobileLightPool;
        public bool IsMobileLightInitialized { get; private set; }
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
        private void InitializeMobileLight()
        {
            MobileLight.InitializePrefab(() =>
            {
                MobileLight.Spawn(this, ref mobileLightPool, ref mobileLightSpawnCount);
                IsMobileLightInitialized = true;
                CheckForFixturesInitialized();
            });
        }

        private void TogglePool(Component[] pool, bool active)
        {
            foreach (var obj in pool)
            {
                if (obj == null) continue;
                
                obj.gameObject.SetActive(active);
            }
        }

        public event Action OnFixturesInitialized = delegate { };

        private void CheckForFixturesInitialized()
        {
            if (IsMobileTrussInitialized && IsMobileLightInitialized && IsPyroDroneInitialized &&
                IsLightingDroneInitialized)
            {
                OnFixturesInitialized?.Invoke();
            }
        }
        
        private static FixtureSpawnManager _instance;
        public static FixtureSpawnManager Instance => _instance;

        private void Awake()
        {
            if (_instance != null)
            {
                throw new Exception("Another instance present.");
            }
            
            _instance = this;
            
            dmxController.OnDmxDataChanged += OnDmxDataChanged;
        }

        public void Initialize()
        {
            Debug.Log("Initialize fixtures");
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

        private void OnDmxDataChanged(short universe, DmxData globalDmxBuffer) // data unused here
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
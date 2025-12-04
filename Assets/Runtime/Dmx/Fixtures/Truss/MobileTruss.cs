using System;
using Runtime.Core.Resources;
using Runtime.Dmx.Fixtures.Shared;
using UnityEngine;

namespace Runtime.Dmx.Fixtures.Truss
{
    // TODO: Boundary box visuals/limit for editor usage
    public class MobileTruss : BaseMobile
    {
        private void Awake()
        {
            Buffer = new byte[14];

            MinAngle = -270;
            MaxAngle = 270;

            MinPosition = new Vector3(-50, -50, -50);
            MaxPosition = new Vector3(50, 50, 50);
        }

        private void Update()
        {
            WriteDmxPosition(0, transform.position);
            WriteDmxRotation(6, transform.rotation.eulerAngles);
        }

        public override void WriteDmxData() { }

        #region Static
        private static GameObject _trussPrefab;
        private static GameObject _internalPool;
        private const int GlobalDmxChannelOffset = 6; // Start for mobile truss.
        private const string Prefix = "MobileTruss";
        
        public static void InitializePrefab(Action callback = default)
        {
            if (_trussPrefab != null) return;
            AssetManager.Load("MobileTruss", (prefab) =>
            {
                if (_trussPrefab != null) return;
                _trussPrefab = prefab;
                callback?.Invoke();
            });
        }

        public static void Spawn(FixtureSpawnManager spawnManager, ref MobileTruss[] pool, ref int count)
        {
            if (_internalPool == null) _internalPool = new GameObject("MobileTrussPool");
            pool = new MobileTruss[count];
            MobileTruss fixture = null;

            for (int i = 0; i < pool.Length; i++)
            {
                Spawn(ref pool, ref i, GlobalDmxChannelOffset, ref fixture);

                var nav = fixture.gameObject.AddComponent<MobileTrussNavigation>();
                //nav.playTrussPresetSwap = true;
                fixture.spawnManager = spawnManager;
                nav.cyclePresets = false;
                nav.playTrussPresetSwap = false;
                nav.nextTrussPreset = 6;
            }

            Debug.Log($"'{Prefix}' {pool.Length} mobile trusses are instanced");

            SetPreset(pool, MobileTrussPresetManager.trussPresets[6]);
        }

        private static void Spawn(ref MobileTruss[] pool, ref int index, int offset, ref MobileTruss fixture)
        {
            var instance = Instantiate(_trussPrefab, new Vector3(index * 9, 2, 0), Quaternion.identity);
            instance.transform.SetParent(_internalPool.transform);
            fixture = instance.AddComponent<MobileTruss>();
            fixture.fixtureIndex = index;
            fixture.globalChannelStart = offset + (index * fixture.GetDmxData().Length);
            fixture.gameObject.name = "MobileTruss #" + fixture.fixtureIndex;
            pool[index] = fixture;
        }

        private static void SetPreset(MobileTruss[] pool, TrussPreset[] preset)
        {
            for (var i = 0; i < preset.Length; i++)
            {
                var fixture = pool[i];
                fixture.transform.localPosition = preset[i].GetPosition();
                fixture.transform.localRotation = preset[i].GetRotation();
            }
        }
        
        public static void WriteDataToGlobalBuffer(ref MobileTruss[] pool, ref byte[] globalDmxBuffer)
        {
            foreach (var truss in pool)
            {
                byte[] trussData = truss.GetDmxData();
                
                System.Buffer.BlockCopy(trussData, 0, 
                    globalDmxBuffer, truss.globalChannelStart, trussData.Length);
            }
            
            WriteSpecialData(globalDmxBuffer);
        }
        
        private static void WriteSpecialData(byte[] buffer)
        {
            buffer[5] = 255;
        }
        #endregion
    }
}

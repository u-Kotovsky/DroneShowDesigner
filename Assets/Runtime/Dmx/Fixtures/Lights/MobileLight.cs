using System;
using Runtime.Core.Resources;
using Runtime.Dmx.Fixtures.Shared;
using UnityEngine;

namespace Runtime.Dmx.Fixtures.Lights
{
    // TODO: Boundary box visuals/limit for editor usage
    public class MobileLight : BaseMobile
    {
        private void Awake()
        {
            Buffer = new byte[6];

            Vector3 offset = new Vector3(0, 21f, 0);
            MinPosition = new Vector3(-52.5f, -22.5f, -52.5f) + offset;
            MaxPosition = new Vector3(52.5f, 22.5f, 52.5f) + offset;
        }
        
        private void Update()
        {
            WriteDmxPosition(0, transform.position);
        }

        public override void WriteDmxData() { }

        #region Static
        private static GameObject _mobileLightPrefab;
        private static GameObject _internalPool;
        private const int GlobalDmxChannelOffset = 1077; // Start for mobile light.
        private const string Prefix = "MobileLight";
        
        public static void InitializePrefab(Action callback = default)
        {
            if (_mobileLightPrefab != null) return;
            AssetManager.Load("MobileLight", prefab =>
            {
                if (_mobileLightPrefab != null) return;
                _mobileLightPrefab = prefab;
                callback?.Invoke();
            });
        }

        
        public static void Spawn(FixtureSpawnManager spawnManager, ref MobileLight[] pool, ref int count)
        {
            if (_internalPool == null) _internalPool = new GameObject("MobileLightPool");
            pool = new MobileLight[count];
            MobileLight fixture = null;

            for (int i = 0; i < pool.Length; i++)
            {
                Spawn(ref pool, ref i, GlobalDmxChannelOffset, ref fixture);
                fixture.spawnManager = spawnManager;
            }
            
            Debug.Log($"'{Prefix}' {pool.Length} mobile lights are instanced");

            SetPreset(pool, MobileLightPresetManager.presets[0]);
        }

        private static void Spawn(ref MobileLight[] pool, ref int index, int offset, ref MobileLight fixture)
        {
            var instance = Instantiate(_mobileLightPrefab, new Vector3(index, 2, 0), Quaternion.identity);
            instance.transform.SetParent(_internalPool.transform);
            fixture = instance.AddComponent<MobileLight>();
            fixture.fixtureIndex = index;
            fixture.globalChannelStart = offset + (index * fixture.GetDmxData().Length);
            fixture.gameObject.name = "MobileLight #" + fixture.fixtureIndex;
            pool[index] = fixture;
        }
        
        public static void WriteDataToGlobalBuffer(ref MobileLight[] pool, ref byte[] globalDmxBuffer)
        {
            foreach (var fixture in pool)
            {
                byte[] data = fixture.GetDmxData();
                
                System.Buffer.BlockCopy(data, 0, 
                    globalDmxBuffer, fixture.globalChannelStart, data.Length);
            }

            WriteSpecialData(globalDmxBuffer);
        }

        private static void WriteSpecialData(byte[] buffer)
        {
            // 1229 206 16 Way Selector. 0 = No Action; 1 = Hide Search Light mesh [lights can still function]; Rest undefined.
            buffer[1229] = 0;
        }
        
        private static void SetPreset(MobileLight[] pool, MobileLightPreset[] preset)
        {
            for (var i = 0; i < preset.Length; i++)
            {
                var fixture = pool[i];
                fixture.transform.localPosition = preset[i].GetPosition();
            }
        }
        #endregion
    }
}

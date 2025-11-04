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
            WriteDmxPosition(0, transform.localPosition);
            WriteDmxRotation(6, transform.localRotation.eulerAngles);
        }

        public Vector3 GetMaxPosition()
        {
            return MaxPosition;
        }

        public float GetMaxAngle()
        {
            return MaxAngle;
        }

        public override void WriteDmxData()
        {
            //WriteDmxPosition(0, transform.localPosition); // requires mainthread
            //WriteDmxRotation(6, transform.localRotation.eulerAngles);
        }

        #region Static
        public static GameObject trussPrefab = Resources.Load<GameObject>("MobileTruss");
        private static GameObject internalPool;

        public static void Spawn(FixtureSpawnManager spawnManager, ref MobileTruss[] pool, ref int count)
        {
            if (internalPool == null) internalPool = new GameObject("MobileTrussPool");
            pool = new MobileTruss[count];
            MobileTruss fixture = null;
            int offset = 6; // Start for mobile truss.

            for (int i = 0; i < pool.Length; i++)
            {
                Spawn(ref pool, ref i, ref offset, ref fixture);

                var nav = fixture.gameObject.AddComponent<MobileTrussNavigation>();
                nav.playTrussPresetSwap = true;
                fixture.spawnManager = spawnManager;
            }

            Debug.Log($"{pool.Length} mobile trusses are instanced");

            SetTrussPreset(pool, MobileTrussPresetManager.trussPresets[0]);
        }

        private static void Spawn(ref MobileTruss[] pool, ref int index, ref int offset, ref MobileTruss fixture)
        {
            var instance = Instantiate(trussPrefab, new Vector3(index * 9, 2, 0), Quaternion.identity);
            instance.transform.SetParent(internalPool.transform);
            fixture = instance.AddComponent<MobileTruss>();
            fixture.fixtureIndex = index;
            fixture.globalChannelStart = offset + (index * fixture.GetDmxData().Length);
            fixture.gameObject.name = "MobileTruss #" + fixture.fixtureIndex;
            pool[index] = fixture;
        }

        private static void SetTrussPreset(MobileTruss[] pool, TrussPreset[] preset)
        {
            for (var i = 0; i < preset.Length; i++)
            {
                var truss1 = pool[i];
                truss1.transform.localPosition = preset[i].GetPosition();
                truss1.transform.localRotation = preset[i].GetRotation();
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

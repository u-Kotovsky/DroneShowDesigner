using System;
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
    
    public class MobileTrussPresetManager
    {
        public static TrussPreset[][] trussPresets;
        
        static MobileTrussPresetManager()
        {
            // Default Arches [Set Start]
            TrussPreset[] preset1 = new TrussPreset[12];
            preset1[0] = new TrussPreset(79, 216, 142, 185, 141, 129, 5, 63, 151, 129, 21, 238);
            preset1[1] = new TrussPreset(94, 190, 157, 68, 152, 222, 1, 251, 159, 141, 13, 213);
            preset1[2] = new TrussPreset(116, 40, 165, 81, 159, 40, 165, 145, 166, 0, 4, 211);
            preset1[3] = new TrussPreset(139, 214, 165, 81, 159, 40, 165, 145, 4, 169, 165, 214);
            preset1[4] = new TrussPreset(161, 64, 157, 69, 152, 222, 1, 251, 11, 28, 156, 212);
            preset1[5] = new TrussPreset(176, 38, 142, 184, 141, 129, 5, 63, 19, 40, 148, 187);
            preset1[6] = new TrussPreset(79, 216, 142, 185, 114, 125, 165, 93, 19, 23, 21, 244);
            preset1[7] = new TrussPreset(94, 190, 157, 68, 103, 32, 168, 192, 11, 38, 13, 212);
            preset1[8] = new TrussPreset(116, 40, 165, 81, 96, 214, 5, 32, 4, 173, 4, 211);
            preset1[9] = new TrussPreset(139, 214, 165, 81, 96, 214, 5, 11, 166, 2, 165, 213);
            preset1[10] = new TrussPreset(161, 64, 157, 69, 103, 32, 168, 174, 159, 142, 156, 211);
            preset1[11] = new TrussPreset(176, 38, 142, 184, 114, 125, 165, 97, 151, 140, 148, 184);
            
            // Circling
            TrussPreset[] preset2 = new TrussPreset[12];
            preset2[0] = new TrussPreset(66, 20, 143, 91, 144, 114, 0, 0, 92, 113, 127, 255);
            preset2[1] = new TrussPreset(82, 164, 143, 91, 173, 35, 0, 0, 106, 170, 127, 255);
            preset2[2] = new TrussPreset(111, 85, 143, 91, 189, 179, 0, 0, 120, 226, 127, 255);
            preset2[3] = new TrussPreset(144, 118, 143, 91, 189, 180, 0, 0, 135, 27, 127, 255);
            preset2[4] = new TrussPreset(173, 38, 143, 91, 173, 35, 0, 0, 149, 84, 127, 255);
            preset2[5] = new TrussPreset(189, 183, 143, 91, 144, 115, 0, 0, 163, 141, 127, 255);
            preset2[6] = new TrussPreset(66, 20, 143, 91, 111, 81, 0, 0, 78, 56, 127, 255);
            preset2[7] = new TrussPreset(82, 165, 143, 91, 82, 161, 0, 0, 63, 255, 127, 255);
            preset2[8] = new TrussPreset(111, 85, 143, 91, 66, 16, 0, 0, 49, 198, 127, 255);
            preset2[9] = new TrussPreset(144, 118, 143, 91, 66, 17, 0, 0, 35, 141, 127, 255);
            preset2[10] = new TrussPreset(173, 39, 143, 91, 82, 161, 0, 0, 21, 85, 127, 255);
            preset2[11] = new TrussPreset(189, 183, 143, 91, 111, 82, 0, 0, 7, 28, 127, 255);
            
            // Grid 1
            TrussPreset[] preset3 = new TrussPreset[12];
            preset3[0] = new TrussPreset(102, 102, 158, 183, 163, 214, 0, 0, 0, 0, 0, 0);
            preset3[1] = new TrussPreset(127, 255, 158, 183, 163, 214, 0, 0, 0, 0, 0, 0);
            preset3[2] = new TrussPreset(153, 153, 158, 183, 163, 214, 0, 0, 0, 0, 0, 0);
            preset3[3] = new TrussPreset(102, 102, 158, 183, 140, 204, 0, 0, 0, 0, 0, 0);
            preset3[4] = new TrussPreset(127, 255, 158, 183, 140, 204, 0, 0, 0, 0, 0, 0);
            preset3[5] = new TrussPreset(153, 153, 158, 183, 140, 204, 0, 0, 0, 0, 0, 0);
            preset3[6] = new TrussPreset(102, 102, 158, 183, 115, 50, 0, 0, 0, 0, 0, 0);
            preset3[7] = new TrussPreset(127, 255, 158, 183, 115, 50, 0, 0, 0, 0, 0, 0);
            preset3[8] = new TrussPreset(153, 153, 158, 183, 115, 50, 0, 0, 0, 0, 0, 0);
            preset3[9] = new TrussPreset(102, 102, 158, 183, 92, 40, 0, 0, 0, 0, 0, 0);
            preset3[10] = new TrussPreset(127, 255, 158, 183, 92, 40, 0, 0, 0, 0, 0, 0);
            preset3[11] = new TrussPreset(153, 153, 158, 183, 92, 40, 0, 0, 0, 0, 0, 0);
            
            // Grid 2
            TrussPreset[] preset4 = new TrussPreset[12];
            preset4[0] = new TrussPreset(102, 102, 158, 183, 163, 214, 0, 0, 42, 170, 0, 0);
            preset4[1] = new TrussPreset(127, 255, 158, 183, 163, 214, 0, 0, 42, 170, 0, 0);
            preset4[2] = new TrussPreset(153, 153, 158, 183, 163, 214, 0, 0, 42, 170, 0, 0);
            preset4[3] = new TrussPreset(102, 102, 158, 183, 139, 230, 0, 0, 42, 170, 0, 0);
            preset4[4] = new TrussPreset(127, 255, 158, 183, 139, 230, 0, 0, 42, 170, 0, 0);
            preset4[5] = new TrussPreset(153, 153, 158, 183, 139, 230, 0, 0, 42, 170, 0, 0);
            preset4[6] = new TrussPreset(102, 102, 158, 183, 116, 24, 0, 0, 42, 170, 0, 0);
            preset4[7] = new TrussPreset(127, 255, 158, 183, 116, 24, 0, 0, 42, 170, 0, 0);
            preset4[8] = new TrussPreset(153, 153, 158, 183, 116, 24, 0, 0, 42, 170, 0, 0);
            preset4[9] = new TrussPreset(102, 102, 158, 183, 92, 40, 0, 0, 42, 170, 0, 0);
            preset4[10] = new TrussPreset(127, 255, 158, 183, 92, 40, 0, 0, 42, 170, 0, 0);
            preset4[11] = new TrussPreset(153, 153, 158, 183, 92, 40, 0, 0, 42, 170, 0, 0);
            
            // Arches Half Mirrored
            TrussPreset[] preset5 = new TrussPreset[12];
            preset5[0] = new TrussPreset(79, 216, 142, 185, 141, 129, 5, 63, 151, 129, 21, 238);
            preset5[1] = new TrussPreset(94, 190, 157, 68, 152, 222, 1, 251, 159, 141, 13, 213);
            preset5[2] = new TrussPreset(116, 40, 165, 81, 159, 40, 165, 145, 166, 0, 4, 211);
            preset5[3] = new TrussPreset(139, 214, 165, 81, 159, 40, 5, 24, 89, 254, 4, 211);
            preset5[4] = new TrussPreset(161, 64, 157, 69, 152, 222, 168, 174, 96, 113, 13, 213);
            preset5[5] = new TrussPreset(176, 38, 142, 184, 141, 129, 165, 106, 104, 125, 21, 238);
            preset5[6] = new TrussPreset(79, 216, 142, 132, 114, 62, 164, 211, 19, 67, 21, 106);
            preset5[7] = new TrussPreset(94, 190, 156, 187, 102, 121, 1, 121, 13, 101, 13, 121);
            preset5[8] = new TrussPreset(116, 40, 164, 153, 95, 245, 0, 8, 3, 228, 4, 162);
            preset5[9] = new TrussPreset(139, 214, 164, 153, 95, 245, 170, 161, 81, 112, 4, 162);
            preset5[10] = new TrussPreset(161, 64, 156, 187, 102, 120, 169, 48, 71, 239, 13, 121);
            preset5[11] = new TrussPreset(176, 38, 142, 131, 114, 62, 5, 214, 66, 17, 21, 106);
            
            TrussPreset[] preset6 = new TrussPreset[12];
            preset6[0] = new TrussPreset(66, 19, 159, 255, 144, 113, 42, 170, 135, 28, 0, 0);
            preset6[1] = new TrussPreset(82, 163, 159, 255, 173, 34, 42, 170, 149, 84, 0, 0);
            preset6[2] = new TrussPreset(111, 85, 159, 255, 189, 179, 42, 170, 163, 140, 0, 0);
            preset6[3] = new TrussPreset(144, 117, 159, 255, 189, 180, 42, 170, 7, 27, 0, 0);
            preset6[4] = new TrussPreset(173, 37, 159, 255, 173, 34, 42, 170, 21, 85, 0, 0);
            preset6[5] = new TrussPreset(189, 183, 159, 255, 144, 114, 42, 170, 35, 141, 0, 0);
            preset6[6] = new TrussPreset(66, 19, 159, 255, 111, 81, 42, 170, 120, 226, 0, 0);
            preset6[7] = new TrussPreset(82, 164, 159, 255, 82, 160, 42, 170, 106, 170, 0, 0);
            preset6[8] = new TrussPreset(111, 85, 159, 255, 66, 15, 42, 170, 92, 112, 0, 0);
            preset6[9] = new TrussPreset(144, 117, 159, 255, 66, 16, 42, 170, 78, 55, 0, 0);
            preset6[10] = new TrussPreset(173, 38, 159, 255, 82, 160, 42, 170, 63, 255, 0, 0);
            preset6[11] = new TrussPreset(189, 183, 159, 255, 111, 82, 42, 170, 49, 199, 0, 0);
            
            trussPresets = new TrussPreset[6][];
            trussPresets[0] = preset1;
            trussPresets[1] = preset2;
            trussPresets[2] = preset3;
            trussPresets[3] = preset4;
            trussPresets[4] = preset5;
            trussPresets[5] = preset6;
        }
    }
}

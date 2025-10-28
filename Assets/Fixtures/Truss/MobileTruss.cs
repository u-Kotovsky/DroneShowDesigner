using UnityEditor;
using UnityEngine;

namespace Fixtures.Truss
{
    // TODO: Boundary box visuals/limit for editor usage
    public class MobileTruss : BaseMobile
    {
        public int index;

        private void Awake()
        {
            Buffer = new byte[14];

            MinPosition = -50;
            MaxPosition = 50;
            MinAngle = -270;
            MaxAngle = 270;
        }

        public float GetMaxPosition()
        {
            return MaxPosition;
        }

        public float GetMaxAngle()
        {
            return MaxAngle;
        }

        public new byte[] GetDmxData()
        {
            WriteDmxPosition(0, transform.localPosition);
            WriteDmxRotation(6, transform.localRotation.eulerAngles);

            return Buffer;
        }
        
        
        [CustomEditor(typeof(MobileTruss))]
        public class MobileTruss_Editor : Editor
        {
            private BaseMobile baseMobile;
            
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                
                if (baseMobile == null) baseMobile = target as BaseMobile;
                
                GUILayout.Space(10);

                #region Collect data as [0, 0, 0 ... 0]
                EditorGUILayout.LabelField($"Copy data as raw DMX Array");
                if (GUILayout.Button("Copy All"))
                    Utility.CopyDmxValuesAsArray(baseMobile.GetDmxData());

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Position"))
                    Utility.CopyDmxValuesAsArray(baseMobile.GetDmxData(), 0, 6);
                
                if (GUILayout.Button("Copy Rotation"))
                    Utility.CopyDmxValuesAsArray(baseMobile.GetDmxData(), 7, 6);
                
                EditorGUILayout.EndHorizontal();
                #endregion
                
                GUILayout.Space(10);
                
                #region Collect data as UNIVERSE.CHANNEL_VALUE array
                EditorGUILayout.LabelField($"Copy data as raw DMX Array in format UNIVERSE.CHANNEL_VALUE");
                if (GUILayout.Button("Copy All1"))
                    Utility.CopyAllDmxValuesAsMa3Representation(baseMobile.GetDmxData(), baseMobile.globalChannelStart);

                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Copy Position1"))
                    Utility.CopyDmxValuesWithOffsetAsMa3Representation(baseMobile.GetDmxData(), baseMobile.globalChannelStart,
                        0, 6);
                
                if (GUILayout.Button("Copy Rotation1"))
                    Utility.CopyDmxValuesWithOffsetAsMa3Representation(baseMobile.GetDmxData(), baseMobile.globalChannelStart,
                        6, 6);
                
                EditorGUILayout.EndHorizontal();
                #endregion
            }
        }
    }


    public class MobileTrussPresetManager
    {
        public static TrussPreset[][] trussPresets;
        
        static MobileTrussPresetManager()
        {
            // Default Arches [Set Start[
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
            
            
            
            trussPresets = new TrussPreset[5][];
            trussPresets[0] = preset1;
            trussPresets[1] = preset2;
            trussPresets[2] = preset3;
            trussPresets[3] = preset4;
            trussPresets[4] = preset5;
        }
    }
}

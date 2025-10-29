using UnityEditor;
using UnityEngine;

namespace Fixtures.Drones
{
    // TODO: Boundary box visuals/limit for editor usage
    public class LightingDrone : BaseDrone
    {
        #region Color
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private byte r;
        private byte g;
        private byte b;
        
        public Color color = new Color(0, 0, 0, 1);

        public void WriteDmxColor(Color value)
        {
            r = (byte)Utility.MapRange(value.r, 0, 1, 0, 255);
            g = (byte)Utility.MapRange(value.g, 0, 1, 0, 255);
            b = (byte)Utility.MapRange(value.b, 0, 1, 0, 255);
            
            Buffer[6] = r;
            Buffer[7] = g;
            Buffer[8] = b;
            
            if (DroneRenderers == null) return;
            foreach (var droneRenderer in DroneRenderers)
            {
                if (droneRenderer == null || droneRenderer.sharedMaterial == null)
                    continue;
                
                droneRenderer.sharedMaterial.SetColor(BaseColor, color);
            }
        }
        #endregion
        
        private void Awake()
        {
            Buffer = new byte[9]; // (0 -> 6) Position Coarse + Fine, (7 -> 9) Color

            MinPosition = -800;
            MaxPosition = 800;
        }
        
        public new byte[] GetDmxData()
        {
            WriteDmxPosition(0, transform.position, true);
            WriteDmxColor(color);
            return Buffer;
        }
        
        [CanEditMultipleObjects]
        [CustomEditor(typeof(LightingDrone))]
        public class LightingDrone_Editor : Editor
        {
            private LightingDrone fixture;
            
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                
                if (fixture == null) fixture = (LightingDrone)target;
                
                GUILayout.Space(10);
                GUILayout.Label("Editor-only preview features");
                
                #region Collect data in format [VALUE, ...]
                EditorGUILayout.LabelField($"Copy DMX data in format [VALUE, ...]");
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Copy All"))
                    Utility.CopyDmxValuesAsArray(fixture.GetDmxData());
                
                if (GUILayout.Button("Copy Position"))
                    Utility.CopyDmxValuesAsArray(fixture.GetDmxData(), 0, 6);
                
                if (GUILayout.Button("Copy Color"))
                    Utility.CopyDmxValuesAsArray(fixture.GetDmxData(), 6, 3);
                
                EditorGUILayout.EndHorizontal();
                #endregion
                
                GUILayout.Space(10);
                
                #region Collect data as UNIVERSE.CHANNEL_VALUE array
                EditorGUILayout.LabelField($"Copy DMX data in format UNIVERSE.CHANNEL_VALUE");
                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Copy All"))
                    Utility.CopyAllDmxValuesAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart);
                
                if (GUILayout.Button("Copy Position"))
                    Utility.CopyDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, 0, 6);
                
                if (GUILayout.Button("Copy Color"))
                    Utility.CopyDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, 6, 3);
                
                EditorGUILayout.EndHorizontal();
                #endregion
            }
        }
    }
}

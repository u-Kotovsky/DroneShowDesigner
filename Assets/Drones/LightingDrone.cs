using UnityEditor;
using UnityEngine;

namespace Drones
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
            Buffer = new byte[9];
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
            private bool updateOnValidate = true;
            private Color color = Color.black;
            private LightingDrone drone;
            
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                
                if (drone == null) drone = (LightingDrone)target;
                
                GUILayout.Label("Editor-only preview features");
                
                updateOnValidate = EditorGUILayout.Toggle("Update on Validate", updateOnValidate);
                
                color = EditorGUILayout.ColorField("Color", color);
                
                if (updateOnValidate)
                    WriteDataToDrone();

                if (GUILayout.Button("SetPyroEffect"))
                {
                    WriteDataToDrone();
                }
            }

            private void WriteDataToDrone()
            {
                drone.color = color;
            }
        }
    }
}

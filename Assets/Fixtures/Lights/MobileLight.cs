using UnityEditor;
using UnityEngine;

namespace Fixtures.Lights
{
    // TODO: Boundary box visuals/limit for editor usage
    public class MobileLight : BaseMobile
    {
        public int index;

        private void Awake()
        {
            Buffer = new byte[6];
            
            // TODO: MinXPosition, MinYPosition, MinZPosition as well Max values
            MinPosition = -50;
            MaxPosition = 50;
        }
        
        public new byte[] GetDmxData()
        {
            WriteDmxPosition(0, transform.localPosition);
            
            return Buffer;
        }
        
        [CustomEditor(typeof(MobileLight))]
        public class MobileLight_Editor : Editor
        {
            private BaseMobile baseMobile;
            
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                
                if (baseMobile == null) baseMobile = target as BaseMobile;
                
                GUILayout.Space(10);

                if (GUILayout.Button("Copy Raw DMX Data as Array"))
                {
                    byte[] dmxData = baseMobile.GetDmxData();
                    GUIUtility.systemCopyBuffer = "[" + string.Join(", ", dmxData) + "]";
                }

                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Copy Raw DMX Position as Array"))
                {
                    byte[] dmxData = baseMobile.GetDmxData();
                    byte[] bytes = new byte[6];
                    
                    System.Buffer.BlockCopy(dmxData, 0, bytes, 0, 6);
                    
                    GUIUtility.systemCopyBuffer = "[" + string.Join(", ", bytes) + "]";
                }
                
                if (GUILayout.Button("Copy Raw DMX Rotation as Array"))
                {
                    byte[] dmxData = baseMobile.GetDmxData();
                    byte[] bytes = new byte[6];
                    
                    System.Buffer.BlockCopy(dmxData, 7, bytes, 0, 6);
                    
                    GUIUtility.systemCopyBuffer = "[" + string.Join(", ", bytes) + "]";
                }
                
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}

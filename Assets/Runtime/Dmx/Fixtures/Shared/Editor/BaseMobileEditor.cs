using UnityEditor;
using UnityEngine;

namespace Runtime.Dmx.Fixtures.Shared.Editor
{
    [CustomEditor(typeof(BaseMobile))]
    public class BaseMobileEditor : UnityEditor.Editor
    {
        private BaseMobile baseMobile;
            
        // TODO: Refactor this to be more generic, improve code reusability.
            
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
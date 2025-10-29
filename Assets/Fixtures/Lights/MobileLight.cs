using UnityEditor;
using UnityEngine;

namespace Fixtures.Lights
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
        
        public new byte[] GetDmxData()
        {
            WriteDmxPosition(0, transform.position);
            
            return Buffer;
        }
        
        [CustomEditor(typeof(MobileLight))]
        public class MobileLight_Editor : Editor
        {
            private BaseMobile fixture;
            
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                
                if (fixture == null) fixture = (BaseMobile)target;
                
                GUILayout.Space(10);

                #region Collect data in format [VALUE, ...]
                EditorGUILayout.LabelField($"Copy DMX data in format [VALUE, ...]");
                
                if (GUILayout.Button("Copy All"))
                    Utility.CopyDmxValuesAsArray(fixture.GetDmxData());

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Position"))
                    Utility.CopyDmxValuesAsArray(fixture.GetDmxData(), 0, 6);
                
                if (GUILayout.Button("Copy Rotation"))
                    Utility.CopyDmxValuesAsArray(fixture.GetDmxData(), 7, 6);
                
                EditorGUILayout.EndHorizontal();
                #endregion
                
                GUILayout.Space(10);
                
                #region Collect data as UNIVERSE.CHANNEL_VALUE array
                EditorGUILayout.LabelField($"Copy DMX data in format UNIVERSE.CHANNEL_VALUE");
                
                if (GUILayout.Button("Copy All"))
                    Utility.CopyAllDmxValuesAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart);

                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Copy Position"))
                    Utility.CopyDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, 0, 6);
                
                if (GUILayout.Button("Copy Rotation"))
                    Utility.CopyDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, 6, 6);
                
                EditorGUILayout.EndHorizontal();
                #endregion
            }
        }
    }
}

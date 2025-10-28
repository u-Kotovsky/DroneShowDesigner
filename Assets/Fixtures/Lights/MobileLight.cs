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
}

using Runtime.Dmx.Fixtures.Shared;
using UnityEditor;
using UnityEngine;

namespace Runtime.Dmx.Fixtures.Truss.Editor
{
    [CustomEditor(typeof(MobileTruss))]
    public class MobileTrussEditor : UnityEditor.Editor
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
                
            if (GUILayout.Button("Copy All1"))
                Utility.CopyAllDmxValuesAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart);

            EditorGUILayout.BeginHorizontal();
                
            if (GUILayout.Button("Copy Position"))
                Utility.CopyDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart,
                    0, 6);
                
            if (GUILayout.Button("Copy Rotation"))
                Utility.CopyDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart,
                    6, 6);
                
            EditorGUILayout.EndHorizontal();
            #endregion
        }
    }
}
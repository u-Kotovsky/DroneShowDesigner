using UnityEditor;
using UnityEngine;

namespace Runtime.Dmx.Fixtures.Drones.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(LightingDrone))]
    public class LightingDroneEditor : UnityEditor.Editor
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
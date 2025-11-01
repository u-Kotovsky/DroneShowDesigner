using System;
using UnityEditor;
using UnityEngine;

namespace Runtime.Dmx.Fixtures.Drones.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(PyroDrone))]
    public class PyroDroneEditor : UnityEditor.Editor
    {
        private bool updateOnValidate = true;
        private int pitch = 0;
        private int yaw = 0;
        private int roll = 0;
        private int pyroIndex = 0;

        private PyroDrone fixture;

        private static string[] _elements = null;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (fixture == null) fixture = (PyroDrone)target;
            if (_elements == null) _elements = Enum.GetNames(typeof(PyroFxType));

            GUILayout.Space(10);
            GUILayout.Label("Editor-only preview features");

            updateOnValidate = EditorGUILayout.Toggle("Update on Validate", updateOnValidate);

            pyroIndex = EditorGUILayout.Popup(pyroIndex, _elements);
            pitch = EditorGUILayout.IntField("Pitch", pitch);
            yaw = EditorGUILayout.IntField("Yaw", yaw);
            roll = EditorGUILayout.IntField("Roll", roll);

            if (updateOnValidate)
                WriteDataToDrone();

            if (GUILayout.Button("SetPyroEffect"))
            {
                WriteDataToDrone();
            }
        
            GUILayout.Space(10);
            GUILayout.Label("Editor-only preview features");
        
            #region Collect data in format [VALUE, ...]
            EditorGUILayout.LabelField($"Copy DMX data in format [VALUE, ...]");
            EditorGUILayout.BeginHorizontal();
        
            if (GUILayout.Button("Copy All"))
                Utility.CopyDmxValuesAsArray(fixture.GetDmxData());
        
            if (GUILayout.Button("Copy Position"))
                Utility.CopyDmxValuesAsArray(fixture.GetDmxData(), 0, 6);
        
            if (GUILayout.Button("Copy Pitch Yaw Roll"))
                Utility.CopyDmxValuesAsArray(fixture.GetDmxData(), 6, 3);
        
            EditorGUILayout.EndHorizontal();
        
            if (GUILayout.Button("Copy Index"))
                Utility.CopyDmxValuesAsArray(fixture.GetDmxData(), 9, 1);
            #endregion
        
            GUILayout.Space(10);
        
            #region Collect data as UNIVERSE.CHANNEL_VALUE array
            EditorGUILayout.LabelField($"Copy DMX data in format UNIVERSE.CHANNEL_VALUE");
            EditorGUILayout.BeginHorizontal();
        
            if (GUILayout.Button("Copy All"))
                Utility.CopyAllDmxValuesAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart);
        
            if (GUILayout.Button("Copy Position"))
                Utility.CopyDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, 0, 6);
        
            if (GUILayout.Button("Copy Pitch Yaw Roll"))
                Utility.CopyDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, 6, 3);
        
            EditorGUILayout.EndHorizontal();
        
            if (GUILayout.Button("Copy Index"))
                Utility.CopyDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, 9, 1);
            #endregion
        }

        private void WriteDataToDrone()
        {
            fixture.index = (byte)pyroIndex;
            fixture.pitch = (byte)pitch;
            fixture.yaw = (byte)yaw;
            fixture.roll = (byte)roll;
        }
    }
}
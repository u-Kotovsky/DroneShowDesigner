#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Runtime.Core.Formations
{
    [CustomEditor(typeof(Formation))]
    public class FormationEditor : Editor
    {
        private Formation component;
        
        private Transform pointsRoot;
        private Transform dronesRoot;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            component ??= target as Formation;
            if (component is null)
            {
                EditorGUILayout.HelpBox("Target object is null.", MessageType.Error);
                return;
            }
        }
    }
}
#endif
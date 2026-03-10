#if UNITY_EDITOR && !COMPILER_UDONSHARP
using Runtime.Core.Resources;
using UnityEditor;
using UnityEngine;

namespace Runtime.Core.Formations
{
    public class FormationsTimelineHierarchy : EditorWindow
    {
        private static FormationsTimelineHierarchy _instance;
        public static FormationsTimelineHierarchy Instance => _instance;

        [MenuItem("Window/Formations Timeline/Hierarchy")]
        public static void OpenWindow()
        {
            if (_instance != null)
            {
                Debug.LogError("Instance already present.");
                return;
            }
            
            _instance = GetWindow<FormationsTimelineHierarchy>();
            _instance.Show();
        }

        private TextAsset currentTimeline;
        
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Hierarchy");
            currentTimeline = EditorGUILayout.ObjectField(currentTimeline, typeof(TextAsset), false) as TextAsset;

            if (GUILayout.Button("Create new"))
            {
                // todo: prompt
            }

            if (currentTimeline == null)
            {
                EditorGUILayout.LabelField("Timeline is not selected.", StyleUtility.LabelError);
                return;
            }
        }
    }
}
#endif
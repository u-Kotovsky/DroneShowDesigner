#if UNITY_EDITOR && !COMPILER_UDONSHARP
using Runtime.Core.Resources;
using UnityEditor;

namespace Runtime.Core.Formations
{
    public class FormationsTimelineInspector : EditorWindow
    {
        [MenuItem("Window/Formations Timeline/Inspector")]
        public static void OpenWindow()
        {
            var window = GetWindow<FormationsTimelineInspector>();
            window.Show();
        }
        
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Inspector");

            if (FormationsTimelineHierarchy.Instance == null)
            {
                EditorGUILayout.LabelField("No timeline hierarchy loaded.", StyleUtility.LabelError);
                return;
            }
        }
    }
}
#endif
#if UNITY_EDITOR
using UnityEditor;

namespace Runtime.Core.Formations
{
    [CustomEditor(typeof(FormationManager))]
    public class FormationManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var component = (FormationManager)target;

            int points = 0;
            
            for (var i = 0; i < component.formationInstances.Count; i++)
            {
                if (component.formationInstances[i] == null) continue;
                if (component.formationInstances[i].targetFormation == null) continue;
                points += component.formationInstances[i].targetFormation.points.Length;
            }
            
            EditorGUILayout.LabelField($"Used points: {points}");
        }
    }
}
#endif
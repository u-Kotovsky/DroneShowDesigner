#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Runtime.Core.Formations
{
    public class FormationInstance : MonoBehaviour
    {
        public Formation targetFormation;

        [Range(0, 1)]
        public float weight;

        public bool preview = true;
        public float previewSize = .2f;
        
        // TODO: Object exclusion by colliders or something.

        private void OnDrawGizmosSelected()
        {
            if (!preview) return;
            
            if (targetFormation == null) return;

            for (var i = 0; i < targetFormation.points.Length; i++)
            {
                var point = targetFormation.points[i];
                var position = transform.TransformPoint(point.localPosition);
                Gizmos.DrawWireSphere(position, previewSize);
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(FormationInstance))]
    public class FormationInstanceEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var component = (FormationInstance)target;
            
            EditorGUILayout.Space();
            EditorGUI.indentLevel++;
            if (component == null)
            {
                EditorGUILayout.LabelField($"{component.GetType()} target is null");
                return;
            }

            if (component.targetFormation == null)
            {
                EditorGUILayout.LabelField("Target Formation is null");
                return;
            }
            
            EditorGUILayout.LabelField("Debug info");
            EditorGUILayout.LabelField("Point count: " + component.targetFormation.points.Length);
            EditorGUI.indentLevel--;
        }
    }
#endif
}

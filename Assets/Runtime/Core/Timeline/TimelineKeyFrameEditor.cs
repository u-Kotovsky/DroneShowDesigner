#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Runtime.Core.Timeline
{
    /**
     * TODO: editor tools to easily adjust drone/point indexes
     */
    [CustomEditor(typeof(TimelineKeyFrame))]
    public class TimelineKeyFrameEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var component = (TimelineKeyFrame)target;

            if (GUILayout.Button("Regenerate Index Data"))
            {
                component.indexData.Clear();
                component.RefreshFormationInstances();
            }
        }
    }
}
#endif
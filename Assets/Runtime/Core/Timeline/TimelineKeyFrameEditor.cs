#if UNITY_EDITOR
using UnityEditor;

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
            
            
        }
    }
}
#endif
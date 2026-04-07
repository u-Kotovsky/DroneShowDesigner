#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Core.Timeline
{
    [CustomEditor(typeof(Timeline))]
    public class TimelineEditor : Editor
    {
        private Timeline component;
        private List<string> activeMessages = new List<string>();

        private GUIStyle labelWrap;
        public GUIStyle LabelWrap
        {
            get
            {
                if (labelWrap == null)
                {
                    labelWrap = new GUIStyle("label")
                    {
                        wordWrap = true
                    };
                }
                return labelWrap;
            }
        }
        
        private GUIStyle warningLabel;
        public GUIStyle WarningLabel
        {
            get
            {
                if (warningLabel == null)
                {
                    warningLabel = new GUIStyle("label")
                    {
                        wordWrap = true,
                        normal = new GUIStyleState { textColor = Color.yellow }
                    };
                }
                return warningLabel;
            }
        }

        private void EnsureKeyframesAreCorrect()
        {
            TimelineKeyFrame lastKeyFrame = null;
            
            var messages = new List<string>();

            for (var i = 0; i < component.keyframes.Count; i++)
            {
                var keyframe = component.keyframes[i];

                if (lastKeyFrame == null)
                {
                    lastKeyFrame = keyframe;
                    continue;
                }

                var t = lastKeyFrame.time + lastKeyFrame.delay;

                if (t > keyframe.time)
                {
                    messages.Add($"WARNPrevious keyframe time + delay is bigger than next keyframe time: {t} > {keyframe.time}; '{lastKeyFrame.name}', '{keyframe.name}'");
                }
                
                lastKeyFrame = keyframe;
            }
            
            activeMessages.Clear();
            activeMessages.AddRange(messages);
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            component ??= (Timeline)target;

            EnsureKeyframesAreCorrect();

            if (activeMessages.Count > 0)
            {
                EditorGUILayout.LabelField("There are " + activeMessages.Count + " messages");
            }
            
            for (var i = 0; i < activeMessages.Count; i++)
            {
                var message = activeMessages[i];
                if (message.StartsWith("WARN"))
                {
                    EditorGUILayout.LabelField(message.Remove(0, 4), WarningLabel);
                }
                else
                {
                    EditorGUILayout.LabelField(message, LabelWrap);
                }
            }
        }
    }
}

#endif
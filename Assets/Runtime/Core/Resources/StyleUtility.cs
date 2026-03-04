using UnityEngine;

namespace Runtime.Core.Resources
{
    public abstract class StyleUtility
    {
        public static readonly GUIStyle LabelError = GetLabelWithColor(Color.red);
        public static readonly GUIStyle LabelSuccess = GetLabelWithColor(Color.green);
        
        public static GUIStyle GetLabelWithColor(Color color)
        {
            var style = new GUIStyle("label")
            {
                normal = new GUIStyleState
                {
                    textColor = color
                },
            };
            return style;
        }
    }
}

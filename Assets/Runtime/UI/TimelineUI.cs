using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public abstract class TimelineUI
    {
        public static void BuildUI(Transform parent)
        {
            var timelineRect = UIUtility.AddRect(parent, "Timeline");
            var timelineLayout = timelineRect.gameObject.AddComponent<HorizontalLayoutGroup>();
            timelineLayout.childForceExpandWidth = true; // Forces children to expand horizontally
            timelineLayout.childForceExpandHeight = true;
            
            UIUtility.SetAllStretch(timelineRect, Vector4.zero);
                    
            // List of groups
            var listRect = UIUtility.CreateList(timelineRect, "TimelineList");
                    
            UIUtility.AddItemToList(listRect, 0, 20, "Index", "Name", "Object Count");
            UIUtility.AddItemToList(listRect, 1, 20, "0", "Test", "0");
            UIUtility.AddItemToList(listRect, 2, 20, "1", "Test2", "321");
            UIUtility.AddItemToList(listRect, 3, 20, "3", "Test2", "321");
        }
    }
}

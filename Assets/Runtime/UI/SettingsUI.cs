using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public abstract class SettingsUI
    {
        public static void BuildUI(RectTransform parent)
        {
            var timelineRect = UIUtility.AddRect(parent, "Timeline");
            var timelineLayout = timelineRect.gameObject.AddComponent<HorizontalLayoutGroup>();
            timelineLayout.childForceExpandWidth = true; // Forces children to expand horizontally
            timelineLayout.childForceExpandHeight = true;
            
            UIUtility.SetAllStretch(timelineRect, Vector4.zero);
                    
            // List of groups
            var listRect = UIUtility.CreateList(timelineRect, "SettingsList");

            for (int i = 0; i < 64; i++)
            {
                var element = UIUtility.AddItemToList(listRect, 0, 20, "Setting" + i, "value");
                var button = UIUtility.AddButton(MainUIController.DefaultUISprite, element, "Delete", Color.red * .7f, Color.white);
                button.onClick.AddListener(() =>
                {
                    Object.Destroy(element.gameObject);
                });
            }
        }
    }
}

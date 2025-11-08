using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public abstract class SettingsUI
    {
        public static void BuildUI(RectTransform parent)
        {
            // Root
            var settingRootRect = UIUtility.AddRect(parent, "Settings Root");
            var settingRootLayout = settingRootRect.gameObject.AddComponent<HorizontalLayoutGroup>();
            settingRootLayout.childForceExpandWidth = true; // Forces children to expand horizontally
            settingRootLayout.childForceExpandHeight = true;
            UIUtility.SetAllStretch(settingRootRect, Vector4.zero);
            
            // Left
            var settingGroupRect = UIUtility.AddRect(settingRootRect, "Settings Groups Root");
            var settingGroupLayout = settingGroupRect.gameObject.AddComponent<VerticalLayoutGroup>();
            settingGroupLayout.childForceExpandWidth = true; // Forces children to expand horizontally
            settingGroupLayout.childForceExpandHeight = true;
            UIUtility.SetAllStretch(settingGroupRect, Vector4.zero);
            
            // List of groups
            var groupListRect = UIUtility.CreateList(settingGroupRect, "Settings Group List");

            for (int i = 0; i < 64; i++)
            {
                var element = UIUtility.AddItemToList(groupListRect, 0, 20);
                var button = UIUtility.AddButton(MainUIController.DefaultUISprite, element, "Group" + i, 
                    Color.white * .4f, Color.white);
                button.onClick.AddListener(() =>
                {
                    // TODO: On setting group click, load options for this specific group.
                });
            }
            
            // Right
            var settingsRect = UIUtility.AddRect(settingRootRect, "Settings Parameters List");
            var settingsLayout = settingsRect.gameObject.AddComponent<VerticalLayoutGroup>();
            settingsLayout.childForceExpandWidth = true; // Forces children to expand horizontally
            settingsLayout.childForceExpandHeight = true;
            UIUtility.SetAllStretch(settingsRect, Vector4.zero);
                    
            // List of groups
            var listRect = UIUtility.CreateList(settingsRect, "SettingsList");

            for (int i = 0; i < 64; i++)
            {
                var element = UIUtility.AddItemToList(listRect, 0, 20, "Setting" + i, "value");
                var button = UIUtility.AddButton(MainUIController.DefaultUISprite, element, "value" + i + " (Click to edit)", 
                    Color.white * .4f, Color.white);
                button.onClick.AddListener(() =>
                {
                    // TODO: edit this specific setting
                });
                
                // TODO: this is a list, calculate how much elements fits on the screen, -> load only those first + offset
            }
        }
    }
}

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public abstract class UIUtility
    {
        public static void AddButton(Sprite defaultUISprite, RectTransform rect, string title, Color buttonColor, Color textColor,
            Action<Button> callback)
        {
            var button = AddButton(defaultUISprite, rect, title, buttonColor, textColor);
            callback?.Invoke(button);
        }

        public static Button AddButton(Sprite defaultUISprite, RectTransform rect, string title, Color buttonColor, Color textColor)
        {
            var obj = AddRect(rect, $"Button ({title})");
            
            var button = obj.gameObject.AddComponent<Button>();
            obj.transform.SetParent(rect);

            var image = obj.gameObject.AddComponent<Image>();
            image.sprite = defaultUISprite;
            button.targetGraphic = image;
            button.image.type = Image.Type.Sliced;
            button.image.pixelsPerUnitMultiplier = 1.5f;
            
            var colors = button.colors;
            colors.normalColor = buttonColor;
            colors.highlightedColor = buttonColor * 1.1f;
            colors.selectedColor = buttonColor * .9f;
            colors.pressedColor = buttonColor * .8f;
            colors.disabledColor = buttonColor * 0.1f;
            button.colors = colors;
            
            var text = AddText(obj.GetComponent<RectTransform>(), title, textColor);
            text.alignment = TextAlignmentOptions.Center;
            text.horizontalAlignment = HorizontalAlignmentOptions.Center;
            text.verticalAlignment = VerticalAlignmentOptions.Middle;

            SetAllStretch(text.rectTransform, 3, 3, -3, -3);
            
            return button;
        }

        public static TextMeshProUGUI AddText(RectTransform rect, string text, Color textColor)
        {
            var obj = AddRect(rect, "Text");
            
            var component = obj.gameObject.AddComponent<TextMeshProUGUI>();
            component.text = text;
            component.color = textColor;
            component.enableAutoSizing = true;
            component.fontSizeMax = 36;
            component.fontSizeMin = 7;
            
            return component;
        }

        public static RectTransform AddRect(RectTransform parent, string name = "GameObject", bool worldPositionStays = false)
            => AddRect(parent.transform, name, worldPositionStays);

        public static RectTransform AddRect(GameObject parent, string name = "GameObject", bool worldPositionStays = false)
            => AddRect(parent.transform, name, worldPositionStays);

        public static RectTransform AddRect(Transform parent, string name = "GameObject", bool worldPositionStays = false)
        {
            var rectObj = new GameObject(name);
            rectObj.transform.SetParent(parent.transform, worldPositionStays);
            var rect = rectObj.AddComponent<RectTransform>();
            return rect;
        }

        
        public static RectTransform SetAllStretch(RectTransform rect, Vector4 offset = new())
        {
            // Fill the space
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f); // Center pivot for consistent behavior
            rect.offsetMin = new Vector2(offset.x, offset.y); // Left, bottom
            rect.offsetMax = new Vector2(offset.z, offset.w); // Right, top

            return rect;
        }
        
        public static RectTransform SetAllStretch(RectTransform rect, float x = 0, float y = 0, float z = 0, float w = 0)
            => SetAllStretch(rect, new Vector4(x, y, z, w));
        
        public static RectTransform CreateList(Transform parent, string name)
        {
            var listRect = AddRect(parent, name);
            var listLayout = listRect.gameObject.AddComponent<VerticalLayoutGroup>();
            listLayout.childForceExpandWidth = true;
            listLayout.childForceExpandHeight = false;
            listLayout.childControlHeight = false;
            
            return listRect;
        }
        
        // List is vertical
        public static RectTransform AddItemToList(Transform parent, int internalIndex, float height, params string[] values)
        {
            var elementRect = AddRect(parent, $"Element {internalIndex}");
            elementRect.gameObject.AddComponent<HorizontalLayoutGroup>();
            elementRect.sizeDelta = new Vector2(0, height);
            
            var image = elementRect.gameObject.AddComponent<Image>();
            image.color = internalIndex % 2 == 0 ? Color.white * 0.35f : Color.white * 0.05f;
        
            foreach (var value in values)
            {
                var subElementRect = AddRect(elementRect, value);
                var subElementText = AddText(subElementRect, value, Color.white);
                SetAllStretch(subElementText.rectTransform, Vector4.zero);
            }
                
            return elementRect;
        }
    }
}

using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public static class UIUtility
    {
        public static void StretchToParent(RectTransform child, Vector2 padding = default)
        {
            // Set anchors to stretch all sides
            child.anchorMin = Vector2.zero;
            child.anchorMax = Vector2.one;
    
            // Apply padding
            child.offsetMin = new Vector2(padding.x, padding.y);
            child.offsetMax = new Vector2(-padding.x, -padding.y);
        }

        #region UI Elements (Button, InputField, Toggle etc.)
        public static Button AddButton(RectTransform rect, string title, Color buttonColor, Color textColor,
            Action<Button> callback)
        {
            var button = AddButton(rect, title, buttonColor, textColor);
            callback?.Invoke(button);
            return button;
        }

        public static Button AddButton(RectTransform rect, string title, Color buttonColor, Color textColor)
        {
            var obj = AddRect(rect, $"Button ({title})");
            
            var button = obj.gameObject.AddComponent<Button>();
            obj.transform.SetParent(rect);

            var image = obj.gameObject.AddComponent<Image>();
            image.sprite = MainUIController.DefaultUISprite;
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
        public static void AddToggle(RectTransform rect, Color backgroundColor, Color checkmarkColor, Action<Toggle> callback)
        {
            var toggle = AddToggle(rect, backgroundColor, checkmarkColor);
            callback?.Invoke(toggle);
        }
        
        public static Toggle AddToggle(RectTransform rect, Color backgroundColor, Color checkmarkColor)
        {
            var obj = AddRect(rect, $"Toggle");
            
            var toggle = obj.gameObject.AddComponent<Toggle>();
            obj.transform.SetParent(rect);
            
            var colors = toggle.colors;
            colors.normalColor = new Color(1, 1, 1, 1);
            colors.highlightedColor = new Color(.96f, .96f,.96f, 1);
            colors.pressedColor = new Color(.78f, .78f, .78f, 1);
            colors.selectedColor = new Color(.96f, .96f,.96f, 1);
            colors.disabledColor = new Color(.78f, .78f, .78f, .5f);
            toggle.colors = colors;
            
            var background = AddRect(obj, "Background")
                .StretchToParent(0, 0);
            background.sizeDelta = new Vector2(background.sizeDelta.y, background.sizeDelta.y);
            
            var backgroundImage = background.gameObject.AddComponent<Image>();
            backgroundImage.sprite = MainUIController.DefaultUISprite;
            backgroundImage.type = Image.Type.Sliced;
            backgroundImage.pixelsPerUnitMultiplier = 1.5f;
            backgroundImage.color = backgroundColor;

            var checkmark = AddRect(background, "Checkmark")
                .SetAllStretch(3, 3, -3, -3);
            var checkmarkImage = checkmark.gameObject.AddComponent<Image>();
            checkmarkImage.sprite = MainUIController.DefaultUISprite;
            checkmarkImage.type = Image.Type.Sliced;
            checkmarkImage.pixelsPerUnitMultiplier = 1.5f;
            checkmarkImage.color = checkmarkColor;

            toggle.graphic = checkmarkImage;
            
            //SetAllStretch(checkmark, 3, 3, -3, -3);
            
            return toggle;
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
        
        public static TMP_InputField AddInputField(RectTransform rect, Color elementColor, Color textColor)
        {
            var obj = AddRect(rect, $"InputField");
            
            var inputField = obj.gameObject.AddComponent<TMP_InputField>();
            obj.transform.SetParent(rect);

            var colors = inputField.colors;
            colors.normalColor = new Color(1, 1, 1, 1);
            colors.highlightedColor = new Color(.96f, .96f,.96f, 1);
            colors.pressedColor = new Color(.78f, .78f, .78f, 1);
            colors.selectedColor = new Color(.96f, .96f,.96f, 1);
            colors.disabledColor = new Color(.78f, .78f, .78f, .5f);
            inputField.colors = colors;
            
            var image = obj.gameObject.AddComponent<Image>();
            image.sprite = MainUIController.DefaultUISprite;// defaultUISprite;
            image.color = elementColor;
            inputField.targetGraphic = image;
            inputField.image.type = Image.Type.Sliced;
            inputField.image.pixelsPerUnitMultiplier = 1.5f;
            
            var textArea = AddRect(obj, "TextArea");
            var rectMask2d = textArea.gameObject.AddComponent<RectMask2D>();
            rectMask2d.padding = new Vector4(-8, -8, -5, -5);
            
            var caret = AddRect(textArea, "Caret");
            var selectionCaret = caret.gameObject.AddComponent<TMP_SelectionCaret>();
            
            inputField.textViewport = textArea;
            SetAllStretch(textArea, 3, 3, -3, -3);
            
            var placeholder = AddText(textArea, "Enter text..", textColor * 0.5f);
            placeholder.textWrappingMode = TextWrappingModes.NoWrap;
            placeholder.text = "Placeholder";
            var text = AddText(textArea, "", textColor);
            text.textWrappingMode = TextWrappingModes.PreserveWhitespaceNoWrap;
            
            SetAllStretch(placeholder.rectTransform, 0);
            SetAllStretch(text.rectTransform, 0);
            
            inputField.placeholder = placeholder;
            inputField.textComponent = text;
            
            return inputField;
        }

        public static Image AddImage(RectTransform parent, Sprite sprite, bool doNotMakeNewRect = false)
        {
            RectTransform target = parent;
            
            if (!doNotMakeNewRect)
            {
                target = AddRect(parent, "Image");
            }
            
            var image = target.gameObject.AddComponent<Image>();
            image.overrideSprite = sprite;
            
            return image;
        }

        public static Scrollbar AddScrollbar(RectTransform parent, string name)
        {
            var scrollbarImage = AddImage(parent, MainUIController.DefaultUISprite, true);
            scrollbarImage.gameObject.name = name;
            
            var scrollbar = scrollbarImage.rectTransform.gameObject.AddComponent<Scrollbar>();
            
            var slidingAreaRect = AddRect(scrollbarImage.rectTransform, "SlidingArea");
            
            var handleImage = AddImage(slidingAreaRect, MainUIController.DefaultUISprite);
            scrollbar.handleRect = handleImage.rectTransform;
            scrollbar.targetGraphic = handleImage;
            
            return scrollbar;
        }
        
        public static TMP_Dropdown AddDropdown(RectTransform rect, string[] options, Color elementColor, Color textColor)
        {
            var obj = AddRect(rect, $"Dropdown");
            
            var image = AddImage(obj, MainUIController.DefaultUISprite);
            image.color = elementColor;
            
            var dropdown = obj.gameObject.AddComponent<TMP_Dropdown>();
            obj.transform.SetParent(rect);
            
            dropdown.targetGraphic = image;
            dropdown.image.type = Image.Type.Sliced;
            dropdown.image.pixelsPerUnitMultiplier = 1.5f;
            
            var colors = dropdown.colors;
            colors.normalColor = new Color(1, 1, 1, 1);
            colors.highlightedColor = new Color(.96f, .96f,.96f, 1);
            colors.pressedColor = new Color(.78f, .78f, .78f, 1);
            colors.selectedColor = new Color(.96f, .96f,.96f, 1);
            colors.disabledColor = new Color(.78f, .78f, .78f, .5f);
            dropdown.colors = colors;
            
            var label = AddText(obj, "Text", elementColor);
            label.GetRect().SetAllStretch(2, 2, 2, 2);

            var arrow = AddRect(obj, "Arrow")
                .WithImage(MainUIController.DefaultUISprite);
            
            var template = AddRect(obj, "Template");
            var scrollRect = template.gameObject.AddComponent<ScrollRect>();

            var viewport = AddRect(template, "Viewport")
                .WithImage(MainUIController.DefaultUISprite);
            // content, item, item bg; item checkmark; item label

            var content = AddRect(viewport, "Content");

            var item = AddRect(content, "Item");
            var itemToggle = AddToggle(item, elementColor, textColor);
            var background = AddRect(item, "Item Background");
            var checkmark = AddRect(item, "Item Checkmark");
            var itemLabel = AddText(item, "Text", textColor);
            
            var scrollbar = AddScrollbar(template, "Scrollbar");
            // sliding area, handle

            dropdown.template = template;
            
            // Add options
            
            dropdown.AddOptions(options.ToList());
            
            return dropdown;
        }
        #endregion
        
        #region Rect
        public static RectTransform AddRect(RectTransform parent, string name = "GameObject", bool worldPositionStays = false)
            => AddRect(parent.transform, name, worldPositionStays);

        public static RectTransform AddRect(GameObject parent, string name = "GameObject", bool worldPositionStays = false)
            => AddRect(parent.transform, name, worldPositionStays);

        public static RectTransform AddRect(Transform parent, string name = "GameObject", bool worldPositionStays = false)
        {
            var rectObj = new GameObject(name, typeof(RectTransform));
            rectObj.transform.SetParent(parent.transform, worldPositionStays);
            var rect = rectObj.GetComponent<RectTransform>();
            return rect;
        }
        #endregion
        
        public static RectTransform SetAllStretch(RectTransform rect, float x = 0, float y = 0, float z = 0, float w = 0)
            => SetAllStretch(rect, new Vector4(x, y, z, w));
        
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
        
        public static RectTransform CreateVerticalList(Transform parent, string name)
        {
            var listRect = AddRect(parent, name);
            var listLayout = listRect
                .WithVerticalLayout()
                .ForceExpand(true, false);
            listLayout.childControlHeight = false;
            
            return listRect;
        }
        
        // List is vertical
        public static RectTransform AddItemToList(Transform parent, int internalIndex, float height, params string[] values)
        {
            var elementRect = AddRect(parent, $"Element {internalIndex}")
                .WithHorizontalLayout()
                .GetRect();
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

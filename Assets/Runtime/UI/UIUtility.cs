using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class UIUtility
    {
        public static void AddButton(Sprite defaultUISprite, RectTransform rect, string title, Color buttonColor, Color textColor,
            Action<Button> callback)
        {
            var button = AddButton(defaultUISprite, rect, title, buttonColor, textColor);
            callback?.Invoke(button);
        }

        public static Button AddButton(Sprite defaultUISprite, RectTransform rect, string title, Color buttonColor, Color textColor)
        {
            GameObject obj = new GameObject($"Button ({title})");
            
            var button = obj.AddComponent<Button>();
            obj.transform.SetParent(rect);

            var image = obj.AddComponent<Image>();
            image.sprite = defaultUISprite;
            
            var colors = button.colors;
            colors.normalColor = buttonColor;
            button.colors = colors;
            button.targetGraphic = image;
            button.image.type = Image.Type.Sliced;
            button.image.pixelsPerUnitMultiplier = 1.5f;
            
            var text = AddText(obj.GetComponent<RectTransform>(), title, textColor);
            text.alignment = TextAlignmentOptions.Center;
            text.horizontalAlignment = HorizontalAlignmentOptions.Center;
            text.verticalAlignment = VerticalAlignmentOptions.Middle;
            text.enableAutoSizing = true;
            text.fontSizeMax = 24;
            text.fontSizeMin = 7;

            // I don't know why other stuff doesn't set rect to stretch, so I use this for now
            var layout = button.gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            
            //text.rectTransform.rect.Set();
            //var textRect = text.gameObject.GetComponent<RectTransform>();
            //textRect.anchorMax.Set(1f, 1f);
            //textRect.anchorMin.Set(0f, 0f);
            //textRect.sizeDelta.Set(0f, 0f);
            //textRect.offsetMax.Set(0f, 0f);
            //textRect.offsetMin.Set(0f, 0f);
            //textRect.pivot = new Vector2(0.5f, 0.5f);
            //textRect.anchoredPosition = new Vector2(1f, 1f);
            //textRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, 0);
            //textRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 0);
            
            //text.rectTransform.anchorMin = Vector2.zero;
            //text.rectTransform.anchorMax = Vector2.one;
            //text.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0f, 0f);
            //text.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0f, 0f);
            //text.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, 0f);
            //text.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0f, 0f);
            
            return button;
        }

        public static TextMeshProUGUI AddText(RectTransform rect, string text, Color textColor)
        {
            var obj = new GameObject("Text");
            obj.transform.SetParent(rect, false);
            
            var component = obj.AddComponent<TextMeshProUGUI>();
            component.text = text;
            component.color = textColor;
            
            return component;
        }
    }
}

using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Runtime.UI
{
    public static class UIExtensions
    {
        #region WithLayoutGroup
        public static HorizontalLayoutGroup WithHorizontalLayout(this RectTransform target)
        {
            var layout = target.AddComponent<HorizontalLayoutGroup>();
            return layout;
        }
        public static VerticalLayoutGroup WithVerticalLayout(this RectTransform target)
        {
            var layout = target.AddComponent<VerticalLayoutGroup>();
            return layout;
        }
        #endregion
        
        #region ForceExpand
        public static HorizontalLayoutGroup ForceExpand(this HorizontalLayoutGroup target, bool width, bool height)
        {
            target.childForceExpandWidth = width;
            target.childForceExpandHeight = height;
            return target;
        }

        public static VerticalLayoutGroup ForceExpand(this VerticalLayoutGroup target, bool width, bool height)
        {
            target.childForceExpandWidth = width;
            target.childForceExpandHeight = height;
            return target;
        }
        #endregion

        public static Button OnClick(this Button button, UnityAction callback)
        {
            button.onClick.AddListener(callback);
            return button;
        }

        public static Toggle OnValueChanged(this Toggle toggle, UnityAction<bool> callback)
        {
            toggle.onValueChanged.AddListener(callback);
            return toggle;
        }

        public static TMP_InputField OnValueChanged(this TMP_InputField inputField, UnityAction<string> callback)
        {
            inputField.onValueChanged.AddListener(callback);
            return inputField;
        }

        public static RectTransform WithImage(this RectTransform target, Color color)
        {
            var image = target.AddComponent<Image>();
            image.color = color;
            return target;
        }

        public static RectTransform WithImage(this RectTransform target, Sprite sprite = null)
        {
            var image = target.AddComponent<Image>();
            image.overrideSprite = sprite;
            return target;
        }
    }
}

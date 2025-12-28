using TMPro;
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
            var layout = target.gameObject.AddComponent<HorizontalLayoutGroup>();
            return layout;
        }
        public static VerticalLayoutGroup WithVerticalLayout(this RectTransform target)
        {
            var layout = target.gameObject.AddComponent<VerticalLayoutGroup>();
            return layout;
        }
        #endregion
        
        #region ForceExpand
        public static HorizontalLayoutGroup ForceExpand(this HorizontalLayoutGroup target, bool width = true, bool height = true)
        {
            target.childForceExpandWidth = width;
            target.childForceExpandHeight = height;
            return target;
        }

        public static VerticalLayoutGroup ForceExpand(this VerticalLayoutGroup target, bool width = true, bool height = true)
        {
            target.childForceExpandWidth = width;
            target.childForceExpandHeight = height;
            return target;
        }
        #endregion

        #region ControlChildSize
        public static HorizontalLayoutGroup ControlChildSize(this HorizontalLayoutGroup target, bool width = true, bool height = true)
        {
            target.childControlWidth = width;
            target.childControlHeight = height;
            return target;
        }
        public static VerticalLayoutGroup ControlChildSize(this VerticalLayoutGroup target, bool width = true, bool height = true)
        {
            target.childControlWidth = width;
            target.childControlHeight = height;
            return target;
        }
        #endregion
        
        #region Events
        public static Button OnClick(this Button button, UnityAction callback)
        {
            button.onClick.AddListener(callback);
            return button;
        }
        
        public static TMP_Dropdown OnValueCHanged(this TMP_Dropdown component, UnityAction<int> callback)
        {
            component.onValueChanged.AddListener(callback);
            return component;
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

        public static TMP_InputField WithText(this TMP_InputField inputField, string text)
        {
            inputField.text = text;
            return inputField;
        }

        public static TMP_InputField WithPlaceholder(this TMP_InputField inputField, string text)
        {
            var placeholder = (TextMeshProUGUI)(inputField.placeholder);
            placeholder.text = text;
            return inputField;
        }
        #endregion

        #region With@Component
        public static RectTransform WithImage(this RectTransform target, Color color)
        {
            var image = target.gameObject.AddComponent<Image>();
            image.color = color;
            return target;
        }

        public static RectTransform WithImage(this RectTransform target, Sprite sprite = null)
        {
            var image = target.gameObject.AddComponent<Image>();
            image.overrideSprite = sprite;
            return target;
        }
        #endregion
        
        #region Stretch
        public static RectTransform SetAllStretch(this RectTransform rect, float x = 0, float y = 0, float z = 0, float w = 0)
            => SetAllStretch(rect, new Vector4(x, y, z, w));
        
        public static RectTransform SetAllStretch(this RectTransform rect, Vector4 offset = new())
        {
            UIUtility.SetAllStretch(rect, offset);
            return rect;
        }

        public static LayoutGroup SetAllStretch(this LayoutGroup layout, float x = 0, float y = 0, float z = 0, float w = 0)
            => SetAllStretch(layout, new Vector4(x, y, z, w));
        
        public static LayoutGroup SetAllStretch(this LayoutGroup layout, Vector4 offset = new())
        {
            UIUtility.SetAllStretch(layout.GetRect(), offset);
            return layout;
        }

        public static RectTransform StretchToParent(this RectTransform child, float x = 0, float y = 0)
        {
            UIUtility.StretchToParent(child, new Vector2(x, y));
            return child;
        }

        public static RectTransform StretchToParent(this RectTransform child, Vector2 padding = default)
        {
            UIUtility.StretchToParent(child, padding);
            return child;
        }

        #endregion
        
        public static RectTransform GetRect(this LayoutGroup component)
        {
            return component.GetComponent<RectTransform>();
        }
        
        public static RectTransform GetRect(this Image component)
        {
            return component.rectTransform;
        }
        
        public static RectTransform GetRect(this TextMeshProUGUI component)
        {
            return component.rectTransform;
        }
        
        public static RectTransform GetRect(this Button component)
        {
            return component.GetComponent<RectTransform>();
        }
        
        public static RectTransform GetRect(this TMP_Dropdown component)
        {
            return component.GetComponent<RectTransform>();
        }
        
        public static RectTransform WithSizeDelta(this RectTransform rect, Vector2 sizeDelta = new())
        {
            rect.sizeDelta = sizeDelta;
            
            return rect;
        }
        
        public static RectTransform SetHeight(this RectTransform rect, float height)
        {
            rect.sizeDelta.Set(rect.sizeDelta.x, height);
            return rect;
        }
    }
}

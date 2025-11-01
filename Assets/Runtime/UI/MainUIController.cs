using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class MainUIController : MonoBehaviour
    {
        public RectTransform hotbar;
        public RectTransform pages;

        public RectTransform hierarchy;
        public RectTransform inspector;

        [Header("Unity Assets")]
        [Tooltip("Since Unity doesn't have a proper way of givin' us developers default assets, I steal it through this reference. Please keep it as UISprite.")]
        public Sprite defaultUISprite;

        private void Awake()
        {
            var buttonColor = Color.gray3;
            var textColor = Color.white;
            
            AddButton(hotbar, "Settings", buttonColor, textColor);
            AddButton(hotbar, "Console", buttonColor, textColor);
            AddButton(hotbar, "Editor", buttonColor, textColor);
        }

        public Button AddButton(RectTransform rect, string title, Color buttonColor, Color textColor)
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

        public TextMeshProUGUI AddText(RectTransform rect, string text, Color textColor)
        {
            var obj = new GameObject("Text");
            obj.transform.SetParent(rect, false);
            
            var component = obj.AddComponent<TextMeshProUGUI>();
            component.text = text;
            component.color = textColor;
            
            return component;
        }

        public void LoadHierarchy()
        {
            
        }

        public void InspectElement(GameObject element)
        {
            // TODO: load element components, parameters etc.
        }
    }
}
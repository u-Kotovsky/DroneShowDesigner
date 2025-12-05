using UnityEngine;

namespace Runtime.UI
{
    public static class EffectUI
    {
        private static RectTransform _boxSelection;

        public static void AddBoxSelection(RectTransform root)
        {
            _boxSelection = UIUtility.AddRect(root, "BoxSelectionHelper")
                .WithImage(new Color(.5f, 1f, 1, 0.1f));
        }

        public static void SetBoxSelectionPosition(Vector2 start, Vector2 end)
        {
            // p.s. my math is weird, please help me lol

            if (_boxSelection == null) return;
            
            float width = end.x - start.x;
            float height = end.y - start.y;

            bool widthIsLessZero = width < 0;
            bool heightIsLessZero = height < 0;
            
            float normWidth = widthIsLessZero ? -width : width;
            float normHeight = heightIsLessZero ? -height : height;

            _boxSelection.anchorMin = new Vector2(0, 1f);
            _boxSelection.anchorMax = new Vector2(0, 1f);
            
            _boxSelection.anchoredPosition = new Vector2(start.x, -start.y);
            _boxSelection.sizeDelta = new Vector2(normWidth, normHeight);
            
            _boxSelection.pivot = new Vector2(widthIsLessZero ? 1 : 0, heightIsLessZero ? 1 : 0);
        }

        public static void SetBoxSelectionActive(bool active)
        {
            if (_boxSelection == null) return;
            
            _boxSelection.gameObject.SetActive(active);
        }
    }
}

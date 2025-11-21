using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public abstract class AboutUI
    {
        private static RectTransform _rootRect;
        private static TextMeshProUGUI _infoText;
    
        public static void BuildUI(RectTransform parent)
        {
            // Root
            _rootRect = UIUtility.AddRect(parent, "Settings Root");

            _rootRect
                .WithVerticalLayout()
                .ForceExpand(true, true);
            //rootLayout.childControlHeight = false;
            UIUtility.SetAllStretch(_rootRect, Vector4.zero);
            
            // Current File Info
            var element0 = UIUtility.AddRect(_rootRect, "About");
            var image = element0.gameObject.AddComponent<Image>();
            image.color = new Color(.2f, .2f, .2f, 1);
            
            _infoText = UIUtility.AddText(element0, @"Project is open source and it's source code available at <nobr><link ID='https://github.com/u-Kotovsky/DroneShowDesigner'>https://github.com/u-Kotovsky/DroneShowDesigner</link></nobr> although the name may change so look under user repositories.", Color.white * .7f);
            
            _infoText.alignment = TextAlignmentOptions.Center;
            _infoText.horizontalAlignment = HorizontalAlignmentOptions.Center;
            _infoText.verticalAlignment = VerticalAlignmentOptions.Middle;
            
            UIUtility.SetAllStretch(element0, Vector4.zero);
            UIUtility.SetAllStretch(_infoText.rectTransform, 10, 10, -10, -10);
            // TODO: add "Do not show at startup next time" checkbox so it won't annoy user
        }
    }
}

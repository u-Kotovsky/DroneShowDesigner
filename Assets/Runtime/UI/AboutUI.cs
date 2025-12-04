using TMPro;
using UnityEngine;

namespace Runtime.UI
{
    public abstract class AboutUI
    {
        private static RectTransform _rootRect;
        private static TextMeshProUGUI _infoText;

        private const string Prefix = "AboutUI";
    
        public static void BuildUI(RectTransform parent)
        {
            // Root
            _rootRect = UIUtility.AddRect(parent, "Settings Root");
            _rootRect
                .WithVerticalLayout()
                .ForceExpand()
                .SetAllStretch(Vector4.zero);
            
            // Current File Info
            var element0 = UIUtility.AddRect(_rootRect, "About")
                .WithImage(new Color(.2f, .2f, .2f, 1))
                .SetAllStretch(Vector4.zero);
            
            _infoText = UIUtility.AddText(element0, @"Project is open source and it's source code available at <nobr><link ID='https://github.com/u-Kotovsky/DroneShowDesigner'>https://github.com/u-Kotovsky/DroneShowDesigner</link></nobr> although the name may change so look under user repositories.", Color.white * .7f);
            
            _infoText.alignment = TextAlignmentOptions.Center;
            _infoText.horizontalAlignment = HorizontalAlignmentOptions.Center;
            _infoText.verticalAlignment = VerticalAlignmentOptions.Middle;
            _infoText.rectTransform.SetAllStretch(10, 10, -10, -10);

            var element1 = UIUtility.AddRect(_rootRect, "About")
                .SetAllStretch(3, 3, -3, -3);
            
            UIUtility.AddButton(element1, "Do not show this page on start", Color.white * 0.5f, Color.white)
                .OnClick(DoNotShowAgain);
        }

        private static void DoNotShowAgain()
        {
            Debug.Log($"'{Prefix}' Do not show page about again");
            PlayerPrefs.SetInt("DoNotShowPageAbout", 1);
            MainUIController.Open(1);
        }
    }
}

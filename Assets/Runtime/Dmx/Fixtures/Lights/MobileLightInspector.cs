using Runtime.UI;
using UnityEngine;

namespace Runtime.Dmx.Fixtures.Lights
{
    public static class MobileLightInspector
    {
        public static void OnInspector(RectTransform parent, MobileLight fixture)
        {
            UIUtility.AddText(parent, "MobileLight", Color.white)
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
        }
    }
}

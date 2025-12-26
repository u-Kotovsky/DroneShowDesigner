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
            
            UIUtility.AddButton(parent, "Copy Position", Color.white, Color.black)
                .OnClick(() =>
                {
                    Utility.CopyDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, 0, 6);
                })
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
        }
    }
}

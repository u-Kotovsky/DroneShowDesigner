using Runtime.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Dmx.Fixtures.Truss
{
    public static class MobileTrussInspector
    {
        public static void OnInspector(RectTransform parent, MobileTruss fixture)
        {
            UIUtility.AddText(parent, "MobileTruss", Color.white)
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            
            UIUtility.AddButton(parent, "Copy All", Color.white, Color.black)
                .OnClick(() =>
                {
                    Utility.CopyAllDmxValuesAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart);
                })
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            UIUtility.AddButton(parent, "Copy Position", Color.white, Color.black)
                .OnClick(() =>
                {
                    Utility.CopyDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, 0, 6);
                })
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            UIUtility.AddButton(parent, "Copy Rotation", Color.white, Color.black)
                .OnClick(() =>
                { 
                    Utility.CopyDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, 6, 6);
                })
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
        }
    }
}

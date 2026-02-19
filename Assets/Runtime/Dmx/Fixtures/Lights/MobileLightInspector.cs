using Runtime.Dmx.Fixtures.Shared;
using Runtime.UI;
using UnityEngine;

namespace Runtime.Dmx.Fixtures.Lights
{
    public abstract class MobileLightInspector : BaseFixtureInspector
    {
        public static void OnInspector(RectTransform parent, MobileLight fixture)
        {
            AddTitle(parent, nameof(MobileLight));
            
            var info = UIUtility.AddItemToList(parent, 0, 15, "DMX Copy");
            UIUtility.AddButton(parent, "Position", buttonColor, textColor)
                .OnClick(() => Utility.CopyDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, 0, 6))
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            
            AddMultiPositionCopyPaste(parent, new [] { (BaseMobile)fixture });
        }
        
        public static void OnInspector(RectTransform parent, MobileLight[] fixtures)
        {
            AddTitle(parent, nameof(MobileLight));
            
            var info = UIUtility.AddItemToList(parent, 0, 15, "DMX Copy");
            UIUtility.AddButton(parent, "Position", buttonColor, textColor)
                .OnClick(() => CopyData(fixtures, 0, 6))
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            
            var data = new BaseMobile[fixtures.Length];
            for (var i = 0; i < data.Length; i++)
            {
                data[i] = fixtures[i];
            }
            
            AddMultiPositionCopyPaste(parent, data);
        }
    }
}

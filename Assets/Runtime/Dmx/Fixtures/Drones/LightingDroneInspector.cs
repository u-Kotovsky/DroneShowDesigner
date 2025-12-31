using Runtime.Dmx.Fixtures.Shared;
using Runtime.UI;
using UnityEngine;

namespace Runtime.Dmx.Fixtures.Drones
{
    public abstract class LightingDroneInspector : BaseFixtureInspector
    {
        private const string Name = "LightingDrone";
        
        private static void AddTitle(RectTransform parent)
        {
            UIUtility.AddText(parent, Name, Color.white)
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
        }
        
        public static void OnInspector(RectTransform parent, LightingDrone fixture)
        {
            AddTitle(parent);

            var info = UIUtility.AddItemToList(parent, 0, 15, "DMX Copy");
            UIUtility.AddButton(parent, "Copy All", buttonColor, textColor)
                .OnClick(() =>
                {
                    Utility.CopyAllDmxValuesAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart);
                })
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            UIUtility.AddButton(parent, "Copy Position", buttonColor, textColor)
                .OnClick(() =>
                {
                    Utility.CopyDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, 0, 6);
                })
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            UIUtility.AddButton(parent, "Copy Color", buttonColor, textColor)
                .OnClick(() =>
                {
                    Utility.CopyDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, 6, 3);
                })
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            
            AddMultiPositionCopyPaste(parent, new [] { (BaseMobile)fixture });
        }
        
        public static void OnInspector(RectTransform parent, LightingDrone[] fixtures)
        {
            AddTitle(parent);
            
            var info = UIUtility.AddItemToList(parent, 0, 15, "DMX Copy");
            UIUtility.AddButton(parent, $"Copy All ({fixtures.Length})", buttonColor, textColor)
                .OnClick(() =>
                {
                    CopyData(fixtures);
                })
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            UIUtility.AddButton(parent, $"Copy Position ({fixtures.Length})", buttonColor, textColor)
                .OnClick(() =>
                {
                    CopyData(fixtures, 0, 6);
                })
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            UIUtility.AddButton(parent, $"Copy Color ({fixtures.Length})", buttonColor, textColor)
                .OnClick(() =>
                {
                    CopyData(fixtures, 6, 3);
                })
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

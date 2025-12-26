using Runtime.UI;
using UnityEngine;

namespace Runtime.Dmx.Fixtures.Drones
{
    public static class LightingDroneInspector
    {
        public static void OnInspector(RectTransform parent, LightingDrone fixture)
        {
            UIUtility.AddText(parent, "LightingDrone", Color.white)
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));

            UIUtility.AddButton(parent, "Copy All", Color.white, Color.black, button =>
            {
                Utility.CopyAllDmxValuesAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart);
            });
            UIUtility.AddButton(parent, "Copy Position", Color.white, Color.black, button =>
            {
                Utility.CopyDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, 0, 6);
            });
            UIUtility.AddButton(parent, "Copy Color", Color.white, Color.black, button =>
            { 
                Utility.CopyDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, 6, 3);
            });
        }
    }
}

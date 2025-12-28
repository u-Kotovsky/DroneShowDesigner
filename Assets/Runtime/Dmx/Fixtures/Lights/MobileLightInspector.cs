using System.Text;
using Runtime.UI;
using UnityEngine;

namespace Runtime.Dmx.Fixtures.Lights
{
    public static class MobileLightInspector
    {
        private const string Name = "MobileLight";
        
        public static void OnInspector(RectTransform parent, MobileLight fixture)
        {
            UIUtility.AddText(parent, "MobileLight", Color.white)
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            
            var info = UIUtility.AddItemToList(parent, 0, 15, "DMX Copy");
            UIUtility.AddButton(parent, "Copy Position", Color.white, Color.black)
                .OnClick(() =>
                {
                    Utility.CopyDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, 0, 6);
                })
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
        }
        
        public static void OnInspector(RectTransform parent, MobileLight[] fixtures)
        {
            UIUtility.AddText(parent, Name, Color.white)
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            
            var info = UIUtility.AddItemToList(parent, 0, 15, "DMX Copy");
            UIUtility.AddButton(parent, $"Copy Position ({fixtures.Length})", Color.white, Color.black)
                .OnClick(() =>
                {
                    var builder = new StringBuilder();
                    
                    foreach (var fixture in fixtures)
                    {
                        var data = Utility.GetDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, 0, 6);
                        builder.AppendLine(data);
                    }
                    
                    Utility.CopyValue(builder.ToString());
                })
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
        }
    }
}

using System.Text;
using Runtime.Dmx.Fixtures.Truss;
using Runtime.UI;
using UnityEngine;

namespace Runtime.Dmx.Fixtures.Drones
{
    public static class PyroDroneInspector
    {
        private const string Name = "PyroDrone";
        
        public static void OnInspector(RectTransform parent, PyroDrone fixture)
        {
            UIUtility.AddText(parent, Name, Color.white)
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
            UIUtility.AddButton(parent, "Copy Pitch Yaw Roll", Color.white, Color.black)
                .OnClick(() =>
                { 
                    Utility.CopyDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, 6, 3);
                })
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
        }

        public static void OnInspector(RectTransform parent, PyroDrone[] fixtures)
        {
            UIUtility.AddText(parent, Name, Color.white)
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            
            UIUtility.AddButton(parent, $"Copy All ({fixtures.Length})", Color.white, Color.black)
                .OnClick(() =>
                {
                    var builder = new StringBuilder();
                    
                    foreach (var fixture in fixtures)
                    {
                        var data = Utility.GetAllDmxValuesAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart);
                        builder.AppendLine(data);
                    }
                    
                    Utility.CopyValue(builder.ToString());
                })
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
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
            UIUtility.AddButton(parent, $"Copy Pitch Yaw Roll ({fixtures.Length})", Color.white, Color.black)
                .OnClick(() =>
                { 
                    var builder = new StringBuilder();
                    
                    foreach (var fixture in fixtures)
                    {
                        var data = Utility.GetDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, 6, 3);
                        builder.AppendLine(data);
                    }
                    
                    Utility.CopyValue(builder.ToString());
                })
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
        }
    }
}

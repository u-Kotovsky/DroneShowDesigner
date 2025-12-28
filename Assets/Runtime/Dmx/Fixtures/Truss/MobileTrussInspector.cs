using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Runtime.UI;
using UnityEngine;

namespace Runtime.Dmx.Fixtures.Truss
{
    public static class MobileTrussInspector
    {
        private const string Name = "MobileTruss";
        
        public static void OnInspector(RectTransform parent, MobileTruss fixture)
        {
            UIUtility.AddText(parent, Name, Color.white)
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            
            var info = UIUtility.AddItemToList(parent, 0, 15, "DMX Copy");
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

            // TODO: finish dropdown UI element
            /*if (fixture.gameObject.TryGetComponent<MobileTrussNavigation>(out var navigation))
            {
                var values = Enum.GetNames(typeof(MobileTrussNavigation.TrussState)); // NotValid
                var dropdown = UIUtility.AddDropdown(parent, values, Color.white, Color.black)
                    .OnValueCHanged((value) =>
                    {
                        // GetComponent for navigation if we have one
                        navigation.trussState = (MobileTrussNavigation.TrussState)value;
                    });

                dropdown.MultiSelect = false;
                dropdown.value = (int)navigation.trussState;
                
                dropdown
                    .GetRect()
                    .WithSizeDelta(new Vector2(0, 20));
            }*/
        }
        
        public static void OnInspector(RectTransform parent, MobileTruss[] fixtures)
        {
            UIUtility.AddText(parent, Name, Color.white)
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            
            var info = UIUtility.AddItemToList(parent, 0, 15, "DMX Copy");
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
            UIUtility.AddButton(parent, $"Copy Rotation ({fixtures.Length})", Color.white, Color.black)
                .OnClick(() =>
                { 
                    var builder = new StringBuilder();
                    
                    foreach (var fixture in fixtures)
                    {
                        var data = Utility.GetDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, 6, 6);
                        builder.AppendLine(data);
                    }
                    
                    Utility.CopyValue(builder.ToString());
                })
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
        }
    }
}

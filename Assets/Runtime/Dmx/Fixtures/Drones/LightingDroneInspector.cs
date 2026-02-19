using System;
using Runtime.Dmx.Fixtures.Shared;
using Runtime.UI;
using UnityEngine;

namespace Runtime.Dmx.Fixtures.Drones
{
    public abstract class LightingDroneInspector : BaseFixtureInspector
    {
        public static void OnInspector(RectTransform parent, LightingDrone fixture)
        {
            AddTitle(parent, nameof(LightingDrone));

            var dmxCopyInfo = UIUtility.AddItemToList(parent, 0, 15, "DMX Copy");
            UIUtility.AddButton(parent, "Copy All", buttonColor, textColor)
                .OnClick(() => Utility.CopyAllDmxValuesAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart))
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            UIUtility.AddButton(parent, "Copy Position", buttonColor, textColor)
                .OnClick(() => Utility.CopyDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, 0, 6))
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            UIUtility.AddButton(parent, "Copy Color", buttonColor, textColor)
                .OnClick(() => Utility.CopyDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, 6, 3))
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            
            AddMultiPositionCopyPaste(parent, new [] { (BaseMobile)fixture });
        }
        
        public static void OnInspector(RectTransform parent, LightingDrone[] fixtures)
        {
            AddTitle(parent, nameof(LightingDrone));
            
            var selectionInfo = UIUtility.AddItemToList(parent, 0, 15, $"Selected {fixtures.Length} fixtures");
            var dmxCopyInfo = UIUtility.AddItemToList(parent, 0, 15, "DMX Copy");
            UIUtility.AddButton(parent, "Copy All", buttonColor, textColor)
                .OnClick(() => CopyData(fixtures))
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            UIUtility.AddButton(parent, "Copy Position", buttonColor, textColor)
                .OnClick(() => CopyData(fixtures, 0, 6))
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            UIUtility.AddButton(parent, "Copy Color", buttonColor, textColor)
                .OnClick(() => CopyData(fixtures, 6, 3))
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            
            var shapeInfo = UIUtility.AddItemToList(parent, 0, 15, "Shape generators");
            // TODO: expose options in UI, maybe a modifier that has collection of objects that it controls when enabled (or lerp-ed)
            UIUtility.AddButton(parent, "Circle", buttonColor, textColor)
                .OnClick(() =>
                {
                    Vector3 center = fixtures[0].transform.localPosition;
                    float radius = 3;
                    for (var i = 0; i < fixtures.Length; i++)
                    {
                        var n = ((float)i / fixtures.Length) * MathF.PI * 2;

                        var sin = MathF.Sin(n) * radius;
                        var cos = MathF.Cos(n) * radius;

                        Vector3 position = new Vector3(sin, 0, cos);
                        
                        fixtures[i].transform.localPosition = position + center;
                    }
                })
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            
            UIUtility.AddButton(parent, "Pole", buttonColor, textColor)
                .OnClick(() =>
                {
                    Vector3 center = fixtures[0].transform.localPosition;
                    float padding = 1;
                    for (var i = 0; i < fixtures.Length; i++)
                    {
                        Vector3 position = new Vector3(0, i * padding, 0);
                        
                        fixtures[i].transform.localPosition = position + center;
                    }
                })
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            
            
            var colorInfo = UIUtility.AddItemToList(parent, 0, 15, "Color generators");
            UIUtility.AddButton(parent, "White", buttonColor, textColor)
                .OnClick(() =>
                {
                    for (var i = 0; i < fixtures.Length; i++)
                    {
                        fixtures[i].Color = Color.white;
                    }
                })
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            UIUtility.AddButton(parent, "Black", buttonColor, textColor)
                .OnClick(() =>
                {
                    for (var i = 0; i < fixtures.Length; i++)
                    {
                        fixtures[i].Color = Color.black;
                    }
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

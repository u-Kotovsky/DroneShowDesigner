using Runtime.Dmx.Fixtures.Shared;
using Runtime.UI;
using UnityEngine;

namespace Runtime.Dmx.Fixtures.Truss
{
    public abstract class MobileTrussInspector : BaseFixtureInspector
    {
        private const string Name = "MobileTruss";
        
        private static void AddTitle(RectTransform parent)
        {
            UIUtility.AddText(parent, Name, Color.white)
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
        }
        
        public static void OnInspector(RectTransform parent, MobileTruss fixture)
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
            UIUtility.AddButton(parent, "Copy Rotation", buttonColor, textColor)
                .OnClick(() =>
                { 
                    Utility.CopyDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, 6, 6);
                })
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            
            AddMultiPositionCopyPaste(parent, new [] { (BaseMobile)fixture });

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
            AddTitle(parent);
            
            var info = UIUtility.AddItemToList(parent, 0, 15, "DMX Copy");
            UIUtility.AddButton(parent, $"Copy All ({fixtures.Length})", buttonColor, textColor)
                .OnClick(() =>
                {
                    CopyData(fixtures); // We only read so that should be alright
                })
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            UIUtility.AddButton(parent, $"Copy Position ({fixtures.Length})", buttonColor, textColor)
                .OnClick(() =>
                {
                    CopyData(fixtures, 0, 6); // We only read so that should be alright
                })
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            UIUtility.AddButton(parent, $"Copy Rotation ({fixtures.Length})", buttonColor, textColor)
                .OnClick(() =>
                { 
                    CopyData(fixtures, 6, 6); // We only read so that should be alright
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

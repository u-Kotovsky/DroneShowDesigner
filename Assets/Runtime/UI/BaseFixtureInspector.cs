using System.Text;
using Runtime.Core.Serialization;
using Runtime.Dmx.Fixtures;
using Runtime.Dmx.Fixtures.Shared;
using UnityEngine;

namespace Runtime.UI
{
    public class BaseFixtureInspector
    {
        protected static Color buttonColor = Color.gray3;
        protected static Color textColor = Color.white;
        
        protected static void AddTitle(RectTransform parent, string name)
        {
            UIUtility.AddText(parent, name, textColor)
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
        }
        
        protected static void CopyData(BaseFixture[] fixtures, int offset = -1, int size = -1)
        {
            var builder = new StringBuilder();
                    
            foreach (var fixture in fixtures)
            {
                string data;

                if (offset != -1 && size != -1)
                {
                    data = Utility.GetDmxValuesWithOffsetAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart, offset, size);
                }
                else
                {
                    data = Utility.GetAllDmxValuesAsMa3Representation(fixture.GetDmxData(), fixture.globalChannelStart);
                }
                
                builder.AppendLine(data);
            }
                    
            Utility.CopyValue(builder.ToString());
        }
        
        protected static void AddMultiPositionCopyPaste(RectTransform parent, BaseMobile[] fixtures)
        {
            var info = UIUtility.AddItemToList(parent, 0, 15, "Position Copy/Paste");
            UIUtility.AddButton(parent, $"Copy Position ({fixtures.Length})", buttonColor, textColor)
                .OnClick(() =>
                {
                    // TODO: add header to json to give context what data it contains.
                    var array = new SerializableVector3Array(fixtures.Length);

                    for (var i = 0; i < fixtures.Length; i++)
                    {
                        var fixture = fixtures[i];
                        // TODO: Should we do local or global?
                        // TODO: use DMX values instead.
                        var vec3 = new SerializableVector3(fixture.transform.localPosition.x,
                            fixture.transform.localPosition.y, fixture.transform.localPosition.z);
                        array.value[i] = vec3;
                    }

                    var json = JsonUtility.ToJson(array);
                    Debug.Log($"Copy: {array.value.Length} elements");
                    
                    Utility.CopyValue(json);
                })
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
            UIUtility.AddButton(parent, $"Paste Position ({fixtures.Length})", buttonColor, textColor)
                .OnClick(() =>
                {
                    // TODO: read system clipboard, see if required values are here (x, y, z) and values are valid => paste em in fixtures
                    // also check for array size of fixtures and clipboard values, if it doesn't match, ignore action
                    // and show popup that clipboard data size does not match selected size and show sizes.
                    // TODO: read header to see what data it should contain
                    var text = Utility.GetSystemCopyBuffer();
                    var data = JsonUtility.FromJson<SerializableVector3Array>(text);
                    if (data.value.Length != fixtures.Length)
                    {
                        Debug.LogError($"data count from clipboard is not equal to selected fixture count. ({data.value.Length}, {fixtures.Length})");
                        return;
                    }
                    
                    for (var i = 0; i < fixtures.Length; i++)
                    {
                        fixtures[i].transform.localPosition = data.value[i].GetVector3();
                    }
                    
                    Debug.Log("Paste position successfully!");
                })
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
        }
    }
}

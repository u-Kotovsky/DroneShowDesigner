using Runtime.UI;
using UnityEngine;

namespace Runtime.Dmx.Fixtures.Truss
{
    public static class MobileTrussInspector
    {
        public static void OnInspector(RectTransform parent, MobileTruss fixture)
        {
            UIUtility.AddText(parent, "MobileTruss", Color.white)
                .GetRect()
                .WithSizeDelta(new Vector2(0, 20));
        }
    }
}

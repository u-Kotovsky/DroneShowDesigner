using UnityEngine;

namespace Runtime.Dmx.Fixtures.Lights
{
    public class MobileLightPreset
    {
        private byte xPositionCoarse;
        private byte xPositionFine;
    
        private byte yPositionCoarse;
        private byte yPositionFine;
    
        private byte zPositionCoarse;
        private byte zPositionFine;

        private static readonly Vector3 Offset = new(0f, 21f, 0f);
        private static readonly Vector3 MinPosition = new(-52.5f, -22.5f, -52.5f);
        private static readonly Vector3 MaxPosition = new(52.5f, 22.5f, 52.5f);

        public MobileLightPreset() { }

        public MobileLightPreset(
            byte xPositionCoarse, byte xPositionFine,
            byte yPositionCoarse, byte yPositionFine,
            byte zPositionCoarse, byte zPositionFine)
        {
            this.xPositionCoarse = xPositionCoarse;
            this.xPositionFine = xPositionFine;
            this.yPositionCoarse = yPositionCoarse;
            this.yPositionFine = yPositionFine;
            this.zPositionCoarse = zPositionCoarse;
            this.zPositionFine = zPositionFine;
        }

        public Vector3 GetPosition()
        {
            return new Vector3(
                Utility.GetValueFromCoarseFine(xPositionCoarse, xPositionFine, MinPosition.x, MaxPosition.x),
                Utility.GetValueFromCoarseFine(yPositionCoarse, yPositionFine, MinPosition.y, MaxPosition.y),
                Utility.GetValueFromCoarseFine(zPositionCoarse, zPositionFine, MinPosition.z, MaxPosition.z)) + Offset;
        }
    }
}

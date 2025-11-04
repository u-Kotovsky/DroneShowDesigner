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

        private static readonly Vector3 Offset = new Vector3(0f, 21f, 0f);
        private static readonly Vector3 MinPosition = new Vector3(-52.5f, -22.5f, -52.5f);
        private static readonly Vector3 MaxPosition = new Vector3(52.5f, 22.5f, 52.5f);

        public MobileLightPreset() { }

        public MobileLightPreset(
            byte xpc, byte xpf,
            byte ypc, byte ypf,
            byte zpc, byte zpf)
        {
            xPositionCoarse = xpc;
            xPositionFine = xpf;
            yPositionCoarse = ypc;
            yPositionFine = ypf;
            zPositionCoarse = zpc;
            zPositionFine = zpf;
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

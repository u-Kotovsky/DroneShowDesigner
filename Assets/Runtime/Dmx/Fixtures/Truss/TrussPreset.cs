using UnityEngine;

namespace Runtime.Dmx.Fixtures.Truss
{
    public class TrussPreset
    {
        private byte xPositionCoarse;
        private byte xPositionFine;
    
        private byte yPositionCoarse;
        private byte yPositionFine;
    
        private byte zPositionCoarse;
        private byte zPositionFine;
    
        private byte xRotationCoarse;
        private byte xRotationFine;
    
        private byte yRotationCoarse;
        private byte yRotationFine;
    
        private byte zRotationCoarse;
        private byte zRotationFine;

        public TrussPreset() { }

        public TrussPreset(
            byte xpc, byte xpf,
            byte ypc, byte ypf,
            byte zpc, byte zpf,
        
            byte xrc, byte xrf,
            byte yrc, byte yrf,
            byte zrc, byte zrf)
        {
            xPositionCoarse = xpc;
            xPositionFine = xpf;
            yPositionCoarse = ypc;
            yPositionFine = ypf;
            zPositionCoarse = zpc;
            zPositionFine = zpf;
            xRotationCoarse = xrc;
            xRotationFine = xrf;
            yRotationCoarse = yrc;
            yRotationFine = yrf;
            zRotationCoarse = zrc;
            zRotationFine = zrf;
        }

        public Vector3 GetPosition()
        {
            return new Vector3(
                Utility.GetValueFromCoarseFine(xPositionCoarse, xPositionFine, -50, 50),
                Utility.GetValueFromCoarseFine(yPositionCoarse, yPositionFine, -50, 50),
                Utility.GetValueFromCoarseFine(zPositionCoarse, zPositionFine, -50, 50));
        }

        public Quaternion GetRotation()
        {
            return Quaternion.Euler(new Vector3(
                Utility.GetValueFromCoarseFine(xRotationCoarse, xRotationFine),
                Utility.GetValueFromCoarseFine(yRotationCoarse, yRotationFine),
                Utility.GetValueFromCoarseFine(zRotationCoarse, zRotationFine)) * 540);
        }
    }
}
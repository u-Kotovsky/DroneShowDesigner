using UnityEngine;

namespace Runtime.Dmx.Fixtures.Drones
{
    public class PyroDronePreset
    {
        private byte xPositionCoarse;
        private byte xPositionFine;
    
        private byte yPositionCoarse;
        private byte yPositionFine;
    
        private byte zPositionCoarse;
        private byte zPositionFine;
    
        private byte pitch;
        private byte yaw;
        private byte roll;
        private byte index;

        public PyroDronePreset() { }

        public PyroDronePreset(
            byte xpc, byte xpf,
            byte ypc, byte ypf,
            byte zpc, byte zpf,
        
            byte pitch = 125,
            byte yaw = 125,
            byte roll = 125,
            byte index = 0)
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
                Utility.GetValueFromCoarseFine(xPositionCoarse, xPositionFine, -800, 800),
                Utility.GetValueFromCoarseFine(yPositionCoarse, yPositionFine, -800, 800),
                Utility.GetValueFromCoarseFine(zPositionCoarse, zPositionFine, -800, 800));
        }

        public Quaternion GetRotation()
        {
            return Quaternion.Euler(new Vector3(
                Utility.MapRange(pitch, 0, 255, -180, 180),
                Utility.MapRange(yaw, 0, 255, -180, 180),
                Utility.MapRange(roll, 0, 255, -180, 180)));
        }
    }

    public class PyroDronePresetManager
    {
        public static PyroDronePreset[][] presets;
        
        static PyroDronePresetManager()
        {
            presets = new[]
            {
                new [] 
                {
                    new PyroDronePreset(131, 183, 128, 28, 127, 67),
                    new PyroDronePreset(131, 38, 128, 28, 125, 228),
                    new PyroDronePreset(130, 26, 128, 28, 124, 218),
                    new PyroDronePreset(128, 189, 128, 28, 124, 73),
                    new PyroDronePreset(127, 65, 128, 28, 124, 73),
                    new PyroDronePreset(125, 228, 128, 28, 124, 218),
                    new PyroDronePreset(124, 216, 128, 28, 125, 228),
                    new PyroDronePreset(124, 71, 128, 28, 127, 67),
                    new PyroDronePreset(124, 71, 128, 28, 128, 187),
                    new PyroDronePreset(124, 216, 128, 28, 130, 26),
                    new PyroDronePreset(125, 228, 128, 28, 131, 36),
                    new PyroDronePreset(127, 65, 128, 28, 131, 181),
                    new PyroDronePreset(128, 189, 128, 28, 131, 181),
                    new PyroDronePreset(130, 26, 128, 28, 131, 36),
                    new PyroDronePreset(131, 38, 128, 28, 130, 26),
                    new PyroDronePreset(131, 183, 128, 28, 128, 187)
                }
            };
        }
    }
}

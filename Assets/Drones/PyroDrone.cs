using UnityEngine;

namespace Drones
{
    public class PyroDrone : BaseDrone
    {
        private byte pitch;
        private byte yaw;
        private byte roll;
        
        private byte index = 0;
        
        private void Awake()
        {
            Buffer = new byte[10];
            MinAngle = -180;
            MaxAngle = 180;
        }

        public void WriteDmxRotation(Vector3 eulerAngles)
        {
            pitch = (byte)Utility.MapRange(eulerAngles.x, 0, 1, MinAngle, MaxAngle);
            yaw = (byte)Utility.MapRange(eulerAngles.y, 0, 1, MinAngle, MaxAngle);
            roll = (byte)Utility.MapRange(eulerAngles.z, 0, 1, MinAngle, MaxAngle);
            
            Buffer[6] = pitch;
            Buffer[7] = yaw;
            Buffer[8] = roll;
        }

        public void WriteDmxIndex(byte value)
        {
            Buffer[9] = value;
        }

        public new byte[] GetDmxData()
        {
            WriteDmxPosition(0, transform.position, true);
            WriteDmxRotation(transform.rotation.eulerAngles);
            WriteDmxIndex(index);
            
            return Buffer;
        }
    }
}

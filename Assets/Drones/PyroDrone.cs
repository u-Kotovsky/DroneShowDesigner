using UnityEngine;

namespace Drones
{
    public class PyroDrone : BaseDrone
    {
        private byte pitch;
        private byte yaw;
        private byte roll;
        private readonly float minAngle = -180;
        private readonly float maxAngle = 180;
        
        private byte index = 0;
        
        private void Awake()
        {
            buffer = new byte[10]; // 10 Channels
        }

        public void WriteDmxRotation(Vector3 eulerAngles)
        {
            pitch = (byte)MapRange(eulerAngles.x, 0, 1, minAngle, maxAngle);
            yaw = (byte)MapRange(eulerAngles.y, 0, 1, minAngle, maxAngle);
            roll = (byte)MapRange(eulerAngles.z, 0, 1, minAngle, maxAngle);
            
            buffer[6] = pitch;
            buffer[7] = yaw;
            buffer[8] = roll;
        }

        public void WriteDmxIndex(byte value)
        {
            buffer[9] = value;
        }

        public byte[] GetDmxData()
        {
            WriteDmxPosition(transform.position);
            WriteDmxRotation(transform.rotation.eulerAngles);
            WriteDmxIndex(index);
            return buffer;
        }
    }
}

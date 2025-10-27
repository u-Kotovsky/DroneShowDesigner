using UnityEngine;

namespace Truss
{
    public class MobileTruss : BaseMobile
    {
        private void Awake()
        {
            Buffer = new byte[14];

            MinPosition = -50;
            MaxPosition = 50;
            MinAngle = -270;
            MaxAngle = 270;
        }
        
        // I am going crazy and it does not work as intended
        public void WriteDmxRotation(int offset, Vector3 rotation)
        {
            xRotationCoarse = Utility.GetCoarse(rotation.x, MinAngle, MaxAngle);
            xRotationFine = Utility.GetFine(rotation.x, MinAngle, MaxAngle);
        
            yRotationCoarse = Utility.GetCoarse(rotation.y, MinAngle, MaxAngle);
            yRotationFine = Utility.GetFine(rotation.y, MinAngle, MaxAngle);
        
            zRotationCoarse = Utility.GetCoarse(rotation.z, MinAngle, MaxAngle);
            zRotationFine = Utility.GetFine(rotation.z, MinAngle, MaxAngle);
        
            Buffer[offset + 2] = xRotationCoarse;
            Buffer[offset + 3] = xRotationFine;
    
            Buffer[offset + 0] = yRotationCoarse;
            Buffer[offset + 1] = yRotationFine;
    
            Buffer[offset + 4] = zRotationCoarse;
            Buffer[offset + 5] = zRotationFine;
        }
        
        public new byte[] GetDmxData()
        {
            WriteDmxPosition(0, transform.position);
            WriteDmxRotation(7, transform.rotation.eulerAngles);
            return Buffer;
        }
    }
}

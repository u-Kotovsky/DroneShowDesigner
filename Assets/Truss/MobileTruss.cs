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
        
        public void WriteDmxRotation(int offset, Vector3 rotation, bool flipYtoZ = false)
        {
            var xRotationCoarseFine = CoarseFineChannelSet.GetCoarseFineChannelRepresentation(Mathf.InverseLerp(MinAngle, MaxAngle, rotation.x));
            xRotationCoarse = xRotationCoarseFine.coarse;
            xRotationFine = xRotationCoarseFine.fine;
        
            var yRotationCoarseFine = CoarseFineChannelSet.GetCoarseFineChannelRepresentation(Mathf.InverseLerp(MinAngle, MaxAngle, rotation.y));
            yRotationCoarse = yRotationCoarseFine.coarse;
            yRotationFine = yRotationCoarseFine.fine;
        
            var zRotationCoarseFine = CoarseFineChannelSet.GetCoarseFineChannelRepresentation(Mathf.InverseLerp(MinAngle, MaxAngle, rotation.z));
            zRotationCoarse = zRotationCoarseFine.coarse;
            zRotationFine = zRotationCoarseFine.fine;
        
            Buffer[offset + 0] = xRotationCoarse;
            Buffer[offset + 1] = xRotationFine;
    
            Buffer[offset + 2] = yRotationCoarse;
            Buffer[offset + 3] = yRotationFine;
    
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

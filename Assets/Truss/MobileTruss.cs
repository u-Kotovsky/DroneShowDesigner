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

        public float GetMaxPosition()
        {
            return MaxPosition;
        }

        public float GetMaxAngle()
        {
            return MaxAngle;
        }
        
        public void WriteDmxRotation(int offset, Vector3 rotation)
        {
            // TODO: Look up construct method for coarse/fine in TrussPreset and redo it for this situation.
            //float scaledX = Mathf.InverseLerp(MinAngle, MaxAngle, rotation.x);
            //var xRotationCoarseFine = CoarseFineChannelSet.GetCoarseFineChannelRepresentation(scaledX);
            //xRotationCoarse = xRotationCoarseFine.coarse;
            //xRotationFine = xRotationCoarseFine.fine;
            xRotationCoarse = Utility.GetCoarse(rotation.x, MinAngle, MaxAngle);
            xRotationFine = Utility.GetFine(rotation.x, MinAngle, MaxAngle);
        
            //float scaledY = Mathf.InverseLerp(MinAngle, MaxAngle, rotation.y);
            //var yRotationCoarseFine = CoarseFineChannelSet.GetCoarseFineChannelRepresentation(scaledY);
            //yRotationCoarse = yRotationCoarseFine.coarse;
            //yRotationFine = yRotationCoarseFine.fine;
            yRotationCoarse = Utility.GetCoarse(rotation.y, MinAngle, MaxAngle);
            yRotationFine = Utility.GetFine(rotation.y, MinAngle, MaxAngle);
        
            //float scaledZ = Mathf.InverseLerp(MinAngle, MaxAngle, rotation.z);
            //var zRotationCoarseFine = CoarseFineChannelSet.GetCoarseFineChannelRepresentation(scaledZ);
            //zRotationCoarse = zRotationCoarseFine.coarse;
            //zRotationFine = zRotationCoarseFine.fine;
            zRotationCoarse = Utility.GetCoarse(rotation.z, MinAngle, MaxAngle);
            zRotationFine = Utility.GetFine(rotation.z, MinAngle, MaxAngle);
            
            // XYZ -
            // XZY -
            // YZX +-, bad
            // YXZ +-, bad
            // ZXY -
            // ZYX +-, bad
            // I've tried all combinations and none of them work properly
        
            Buffer[offset + 0] = xRotationCoarse;
            Buffer[offset + 1] = xRotationFine;
    
            Buffer[offset + 2] = yRotationCoarse;
            Buffer[offset + 3] = yRotationFine;
    
            Buffer[offset + 4] = zRotationCoarse;
            Buffer[offset + 5] = zRotationFine;
        }
        
        public new byte[] GetDmxData()
        {
            WriteDmxPosition(0, transform.localPosition);
            WriteDmxRotation(6, transform.localRotation.eulerAngles);
            
            // So buffer would be
            // 0 x pos coarse
            // 1 x pos fine
            // 2 y pos coarse
            // 3 y pos fine
            // 4 z pos coarse
            // 5 z pos fine
            // 6 x rot coarse
            // 7 x rot fine
            // 8 y rot coarse
            // 9 y rot fine
            //10 z rot coarse
            //11 z rot fine
            // total 12 channels used for position/rotation
            
            return Buffer;
        }
    }
}

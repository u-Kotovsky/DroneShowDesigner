using UnityEngine;

namespace Truss
{
    // TODO: Boundary box visuals/limit for editor usage
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
        
        public new byte[] GetDmxData()
        {
            WriteDmxPosition(0, transform.localPosition);
            WriteDmxRotation(6, transform.localRotation.eulerAngles);
            
            return Buffer;
        }
    }
}

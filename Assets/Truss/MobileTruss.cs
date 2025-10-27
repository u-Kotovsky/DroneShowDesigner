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
        
        public new byte[] GetDmxData()
        {
            WriteDmxPosition(0, transform.position);
            WriteDmxRotation(7, transform.rotation.eulerAngles);
            return Buffer;
        }
    }
}

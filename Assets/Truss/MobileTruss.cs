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
            WriteDmxRotation(6, transform.rotation.eulerAngles);
            
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
            // total 12 channels used
            
            return Buffer;
        }
    }
}

using UnityEngine;

namespace Runtime.Dmx.Fixtures.Shared
{
    public class BaseMobile : BaseFixture
    {
        private void Awake()
        {
            Buffer = new byte[12]; // or 6 channels, depends if position are required or rotation
        }

        #region Position
        protected byte XPositionCoarse;
        protected byte XPositionFine;

        protected byte YPositionCoarse;
        protected byte YPositionFine;

        protected byte ZPositionCoarse;
        protected byte ZPositionFine;

        protected Vector3 MinPosition = new(-50, -50, -50);
        protected Vector3 MaxPosition = new(50, 50, 50);
    
        public void WriteDmxPosition(int offset, Vector3 position, bool flipYtoZ = false)
        {
            XPositionCoarse = Utility.GetCoarse(position.x, MinPosition.x, MaxPosition.x);
            XPositionFine = Utility.GetFine(position.x, MinPosition.x, MaxPosition.x);
        
            YPositionCoarse = Utility.GetCoarse(position.y, MinPosition.y, MaxPosition.y);
            YPositionFine = Utility.GetFine(position.y, MinPosition.y, MaxPosition.y);
        
            ZPositionCoarse = Utility.GetCoarse(position.z, MinPosition.z, MaxPosition.z);
            ZPositionFine = Utility.GetFine(position.z, MinPosition.z, MaxPosition.z);
        
            Buffer[offset + 0] = XPositionCoarse;
            Buffer[offset + 1] = XPositionFine;
    
            Buffer[offset + (flipYtoZ ? 4 : 2)] = YPositionCoarse;
            Buffer[offset + (flipYtoZ ? 5 : 3)] = YPositionFine;
    
            Buffer[offset + (flipYtoZ ? 2 : 4)] = ZPositionCoarse;
            Buffer[offset + (flipYtoZ ? 3 : 5)] = ZPositionFine;
        }
        #endregion

        #region Rotation
        protected byte XRotationCoarse;
        protected byte XRotationFine;

        protected byte YRotationCoarse;
        protected byte YRotationFine;

        protected byte ZRotationCoarse;
        protected byte ZRotationFine;

        protected float MinAngle = -180;
        protected float MaxAngle = 180;
    
        public void WriteDmxRotation(int offset, Vector3 rotation)
        {
            float angle = MaxAngle * 2;
            XRotationCoarse = Utility.GetCoarse(rotation.x / angle);
            XRotationFine = Utility.GetFine(rotation.x / angle);
        
            YRotationCoarse = Utility.GetCoarse(rotation.y / angle);
            YRotationFine = Utility.GetFine(rotation.y / angle);
        
            ZRotationCoarse = Utility.GetCoarse(rotation.z / angle);
            ZRotationFine = Utility.GetFine(rotation.z / angle);
        
            Buffer[offset + 0] = XRotationCoarse;
            Buffer[offset + 1] = XRotationFine;
    
            Buffer[offset + 2] = YRotationCoarse;
            Buffer[offset + 3] = YRotationFine;
    
            Buffer[offset + 4] = ZRotationCoarse;
            Buffer[offset + 5] = ZRotationFine;
        }
        #endregion

        public override void WriteDmxData()
        {
            WriteDmxPosition(0, transform.position);
            WriteDmxRotation(6, transform.rotation.eulerAngles);
        }
    }
}

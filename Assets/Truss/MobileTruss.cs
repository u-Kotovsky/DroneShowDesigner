using Drones;
using UnityEngine;

namespace Truss
{
    public class MobileTruss : MonoBehaviour
    {
        public DroneSpawnManager droneSpawnManager;
        protected byte[] buffer;
        public int globalChannelStart;
        
        private void Awake()
        {
            buffer = new byte[14]; // 9 Channels
        }

        #region Position
        private byte xPositionCoarse;
        private byte xPositionFine;
    
        private byte yPositionCoarse;
        private byte yPositionFine;
    
        private byte zPositionCoarse;
        private byte zPositionFine;
        
        public void WriteDmxPosition(Vector3 position)
        {
            xPositionCoarse = BaseDrone.GetCoarse(position.x, -50, 50);
            xPositionFine = BaseDrone.GetFine(position.x, -50, 50);
            
            yPositionCoarse = BaseDrone.GetCoarse(position.y, -50, 50);
            yPositionFine = BaseDrone.GetFine(position.y, -50, 50);
            
            zPositionCoarse = BaseDrone.GetCoarse(position.z, -50, 50);
            zPositionFine = BaseDrone.GetFine(position.z, -50, 50);
            
            buffer[0] = xPositionCoarse;
            buffer[1] = xPositionFine;
        
            buffer[2] = yPositionCoarse;
            buffer[3] = yPositionFine;
        
            buffer[4] = zPositionCoarse;
            buffer[5] = zPositionFine;
        }
        #endregion
    
        #region Rotation
        private byte xRotationCoarse;
        private byte xRotationFine;
    
        private byte yRotationCoarse;
        private byte yRotationFine;
    
        private byte zRotationCoarse;
        private byte zRotationFine;

        public void WriteDmxRotation(Vector3 rotation)
        {
            xRotationCoarse = BaseDrone.GetCoarse(rotation.x, -180, 180);
            xRotationFine = BaseDrone.GetFine(rotation.x, -180, 180);
            
            yRotationCoarse = BaseDrone.GetCoarse(rotation.y, -180, 180);
            yRotationFine = BaseDrone.GetFine(rotation.y, -180, 180);
            
            zRotationCoarse = BaseDrone.GetCoarse(rotation.z, -180, 180);
            zRotationFine = BaseDrone.GetFine(rotation.z, -180, 180);
            
            buffer[7] = xRotationCoarse;
            buffer[8] = xRotationFine;
        
            buffer[9] = yRotationCoarse;
            buffer[10] = yRotationFine;
        
            buffer[11] = zRotationCoarse;
            buffer[12] = zRotationFine;
        }
        #endregion
        
        public byte[] GetDmxData()
        {
            WriteDmxPosition(transform.position);
            WriteDmxRotation(transform.rotation.eulerAngles);
            return buffer;
        }
    }
}

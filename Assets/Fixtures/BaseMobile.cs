using Fixtures.Truss;
using UnityEditor;
using UnityEngine;

namespace Fixtures
{
    public class BaseMobile : MonoBehaviour
    {
        public FixtureSpawnManager spawnManager;
        public int globalChannelStart;
        protected byte[] Buffer;
    
        private void Awake()
        {
            Buffer = new byte[12]; // or 6 channels, depends if position are required or rotation
        }

        #region Position
        private byte xPositionCoarse;
        private byte xPositionFine;

        private byte yPositionCoarse;
        private byte yPositionFine;

        private byte zPositionCoarse;
        private byte zPositionFine;

        protected float MinPosition = -50;
        protected float MaxPosition = 50;
    
        public void WriteDmxPosition(int offset, Vector3 position, bool flipYtoZ = false)
        {
            xPositionCoarse = Utility.GetCoarse(position.x, MinPosition, MaxPosition);
            xPositionFine = Utility.GetFine(position.x, MinPosition, MaxPosition);
        
            yPositionCoarse = Utility.GetCoarse(position.y, MinPosition, MaxPosition);
            yPositionFine = Utility.GetFine(position.y, MinPosition, MaxPosition);
        
            zPositionCoarse = Utility.GetCoarse(position.z, MinPosition, MaxPosition);
            zPositionFine = Utility.GetFine(position.z, MinPosition, MaxPosition);
        
            Buffer[offset + 0] = xPositionCoarse;
            Buffer[offset + 1] = xPositionFine;
    
            Buffer[offset + (flipYtoZ ? 4 : 2)] = yPositionCoarse;
            Buffer[offset + (flipYtoZ ? 5 : 3)] = yPositionFine;
    
            Buffer[offset + (flipYtoZ ? 2 : 4)] = zPositionCoarse;
            Buffer[offset + (flipYtoZ ? 3 : 5)] = zPositionFine;
        }
        #endregion

        #region Rotation
        protected byte xRotationCoarse;
        protected byte xRotationFine;

        protected byte yRotationCoarse;
        protected byte yRotationFine;

        protected byte zRotationCoarse;
        protected byte zRotationFine;

        protected float MinAngle = -180;
        protected float MaxAngle = 180;
    
        public void WriteDmxRotation(int offset, Vector3 rotation)
        {
            float angle = MaxAngle * 2;
            xRotationCoarse = Utility.GetCoarse(rotation.x / angle);
            xRotationFine = Utility.GetFine(rotation.x / angle);
        
            yRotationCoarse = Utility.GetCoarse(rotation.y / angle);
            yRotationFine = Utility.GetFine(rotation.y / angle);
        
            zRotationCoarse = Utility.GetCoarse(rotation.z / angle);
            zRotationFine = Utility.GetFine(rotation.z / angle);
        
            Buffer[offset + 0] = xRotationCoarse;
            Buffer[offset + 1] = xRotationFine;
    
            Buffer[offset + 2] = yRotationCoarse;
            Buffer[offset + 3] = yRotationFine;
    
            Buffer[offset + 4] = zRotationCoarse;
            Buffer[offset + 5] = zRotationFine;
        }
        #endregion
    
        public byte[] GetDmxData()
        {
            WriteDmxPosition(0, transform.position);
            WriteDmxRotation(6, transform.rotation.eulerAngles);
            return Buffer;
        }
        
        [CustomEditor(typeof(BaseMobile))]
        public class BaseMobile_Editor : Editor
        {
            private BaseMobile baseMobile;
            
            // TODO: Refactor this to be more generic, improve code reusability.
            
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                
                if (baseMobile == null) baseMobile = target as BaseMobile;
                
                GUILayout.Space(10);

                if (GUILayout.Button("Copy Raw DMX Data as Array"))
                {
                    byte[] dmxData = baseMobile.GetDmxData();
                    GUIUtility.systemCopyBuffer = "[" + string.Join(", ", dmxData) + "]";
                }

                EditorGUILayout.BeginHorizontal();
                
                if (GUILayout.Button("Copy Raw DMX Position as Array"))
                {
                    byte[] dmxData = baseMobile.GetDmxData();
                    byte[] bytes = new byte[6];
                    
                    System.Buffer.BlockCopy(dmxData, 0, bytes, 0, 6);
                    
                    GUIUtility.systemCopyBuffer = "[" + string.Join(", ", bytes) + "]";
                }
                
                if (GUILayout.Button("Copy Raw DMX Rotation as Array"))
                {
                    byte[] dmxData = baseMobile.GetDmxData();
                    byte[] bytes = new byte[6];
                    
                    System.Buffer.BlockCopy(dmxData, 7, bytes, 0, 6);
                    
                    GUIUtility.systemCopyBuffer = "[" + string.Join(", ", bytes) + "]";
                }
                
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}

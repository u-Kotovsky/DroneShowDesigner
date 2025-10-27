using UnityEditor;
using UnityEngine;

namespace Drones
{
    
    public class PyroDrone : BaseDrone
    {
        protected byte pitch;
        protected byte yaw;
        protected byte roll;
        
        protected byte index = 0;
        
        private void Awake()
        {
            Buffer = new byte[10];
            MinAngle = -180;
            MaxAngle = 180;
        }

        public void WriteDmxRotation(Vector3 eulerAngles)
        {
            pitch = (byte)Utility.MapRange(eulerAngles.x, MinAngle, MaxAngle, 0, 255);
            yaw = (byte)Utility.MapRange(eulerAngles.y, MinAngle, MaxAngle, 0, 255);
            roll = (byte)Utility.MapRange(eulerAngles.z, MinAngle, MaxAngle, 0, 255);
            
            Buffer[6] = pitch;
            Buffer[7] = yaw;
            Buffer[8] = roll;
        }

        public void WriteDmxIndex(byte value)
        {
            Buffer[9] = value;
        }

        public new byte[] GetDmxData()
        {
            WriteDmxPosition(0, transform.position, true);
            WriteDmxRotation(transform.rotation.eulerAngles);
            WriteDmxIndex(index);
            
            return Buffer;
        }
        
        [CanEditMultipleObjects]
        [CustomEditor(typeof(PyroDrone))]
        public class PyroDrone_Editor : Editor
        {
            private bool updateOnValidate = true;
            private int pitch = 0;
            private int yaw = 0;
            private int roll = 0;
            
            private int index = 0;
            
            private PyroDrone drone;
            
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                
                if (drone == null) drone = (PyroDrone)target;
                
                GUILayout.Label("Editor-only preview features");
                
                updateOnValidate = EditorGUILayout.Toggle("Update on Validate", updateOnValidate);
                
                index = EditorGUILayout.IntField("Index", index);
                
                pitch = EditorGUILayout.IntField("Pitch", pitch);
                yaw = EditorGUILayout.IntField("Yaw", yaw);
                roll = EditorGUILayout.IntField("Roll", roll);
                
                if (updateOnValidate)
                    WriteDataToDrone();

                if (GUILayout.Button("SetPyroEffect"))
                {
                    WriteDataToDrone();
                }
            }

            private void WriteDataToDrone()
            {
                drone.index = (byte)index;
                drone.pitch = (byte)pitch;
                drone.yaw = (byte)yaw;
                drone.roll = (byte)roll;
            }
        }
    }

}

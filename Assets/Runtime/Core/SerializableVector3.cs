using System;
using UnityEngine;

namespace Runtime.Core
{
    [Serializable]
    public class SerializableVector3
    {
        [SerializeField] public float x;
        [SerializeField] public float y;
        [SerializeField] public float z;

        public SerializableVector3(Vector3 vector3)
        {
            x = vector3.x;
            y = vector3.y;
            z = vector3.z;
        }

        public SerializableVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    
        public Vector3 GetVector3()
        {
            return new Vector3(x, y, z);
        }
    }
}

using System;
using UnityEngine;

namespace Runtime.Core
{
    [Serializable]
    public class SerializableVector3Array
    {
        // This is stupid
        [SerializeField] public SerializableVector3[] value;

        public SerializableVector3Array(int length)
        {
            value = new SerializableVector3[length];
        }
    }
}
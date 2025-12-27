using System;
using UnityEngine;

namespace Runtime.Core.Selection
{
    public class SelectionEntry
    {
        public GameObject GameObject;
        public Type Type;

        public SelectionEntry(GameObject gameObject, Type type)
        {
            GameObject = gameObject;
            Type = type;
        }
    }
}

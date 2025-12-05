using UnityEngine;

namespace Runtime.Core.Selection
{
    public class SelectionEntry
    {
        public GameObject GameObject;

        public SelectionEntry(GameObject gameObject)
        {
            GameObject = gameObject;
        }
    }
}

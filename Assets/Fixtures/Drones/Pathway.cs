using UnityEngine;

namespace Fixtures.Drones
{
    public class Pathway : MonoBehaviour
    {
        public Transform target;

        private void OnDrawGizmos()
        {
            if (target == null) target = transform;

            Transform lastChild = null;
            
            for (int i = 0; i < target.childCount; i++)
            {
                var child = target.GetChild(i);
                
                if (lastChild != null) Debug.DrawLine(lastChild.position, child.position, Color.yellow);
                
                lastChild = child;
            }
        }
    }
}

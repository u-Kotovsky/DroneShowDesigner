using UnityEngine;

namespace Runtime.Test
{
    public class SimpleRotate : MonoBehaviour
    {
        public Vector3 rotateSpeed;
    
        private void Update()
        {
            transform.Rotate(rotateSpeed * Time.deltaTime);
        }
    }
}

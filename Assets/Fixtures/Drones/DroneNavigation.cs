using UnityEngine;
using Random = UnityEngine.Random;

namespace Fixtures.Drones
{
    public enum DroneNavigationState
    {
        None,
        WaitingForTimer,
        UpdatingPosition,
    }
    public class DroneNavigation : MonoBehaviour
    {
        private BaseDrone drone;
        public DroneNavigationState state = DroneNavigationState.None;
        private float timer;
        
        private Vector3 lastPosition;
        private Vector3 nextPosition;

        public Color waitColor = Color.red;
        public Color moveColor = Color.green;
    
        private void Start()
        {
            drone = GetComponent<BaseDrone>();
            
            lastPosition = transform.localPosition;
        }

        private void Update()
        {
            switch (state)
            {
                case DroneNavigationState.None:
                    break;
                case DroneNavigationState.WaitingForTimer:
                    timer += Time.deltaTime;
                    if (timer >= 5)
                    {
                        timer = 0;
                        state = DroneNavigationState.UpdatingPosition;
                        nextPosition = NextRandomPosition();
                        ((LightingDrone)drone).color = waitColor;
                    }
                    break;
                case DroneNavigationState.UpdatingPosition:
                    timer += Time.deltaTime;
                    transform.localPosition = Vector3.Lerp(lastPosition, nextPosition, 
                        Utility.MapRange(timer, 0, 5, 0, 1));
                    
                    if (timer >= 5)
                    {
                        timer = 0;
                        state = DroneNavigationState.WaitingForTimer;
                        transform.localPosition = nextPosition;
                        lastPosition = nextPosition;
                        ((LightingDrone)drone).color = moveColor;
                    }
                    break;
                
            }
        }

        private Vector3 NextRandomPosition()
        {
            return new Vector3(Random.Range(-22, 22), Random.Range(1, 4), Random.Range(-22, 22));
        }
    }
}

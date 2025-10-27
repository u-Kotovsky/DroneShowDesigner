using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Drones
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
        private DroneNavigationState state = DroneNavigationState.WaitingForTimer;
        private float timer;
        
        private Vector3 lastPosition;
        private Vector3 nextPosition;
    
        private void Start()
        {
            drone = GetComponent<BaseDrone>();
            
            lastPosition = transform.position;
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
                        try
                        {
                            ((LightingDrone)drone).color = Color.red;
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                    break;
                case DroneNavigationState.UpdatingPosition:
                    timer += Time.deltaTime;
                    transform.position = Vector3.Lerp(lastPosition, nextPosition, 
                        Utility.MapRange(timer, 0, 5, 0, 1));
                    
                    if (timer >= 5)
                    {
                        timer = 0;
                        state = DroneNavigationState.WaitingForTimer;
                        transform.position = nextPosition;
                        lastPosition = nextPosition;
                        try
                        {
                            ((LightingDrone)drone).color = Color.green;
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
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

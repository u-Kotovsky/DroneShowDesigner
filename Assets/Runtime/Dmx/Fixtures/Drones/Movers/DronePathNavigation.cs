using UnityEngine;

namespace Runtime.Dmx.Fixtures.Drones
{
    public enum DronePathState
    {
        None,
        WaitingBeforeStart,
        Moving
    }
    
    public class DronePathNavigation : MonoBehaviour
    {
        private BaseDrone drone;
        public Transform targetPathway;
        
        public int currentPathwayIndex = 0;
        public int nextPathwayIndex = 1;

        public float speed = 5;

        public float time;

        public Vector3 position;
        
        public DronePathState state = DronePathState.WaitingBeforeStart;
        public float waitBeforeStart = 0;

        private void Awake()
        {
            drone = GetComponent<BaseDrone>();
            targetPathway = GameObject.Find("LightingDronePathwayTest").transform;
        }

        private void Update()
        {
            if (targetPathway == null) return;

            switch (state)
            {
                case DronePathState.WaitingBeforeStart:
                    WaitBeforeStart();
                    break;
                case DronePathState.Moving:
                    CalculateMovement();
                    break;
            }
        }

        private void WaitBeforeStart()
        {
            time += Time.deltaTime;

            if (time >= waitBeforeStart)
            {
                time = 0;
                state = DronePathState.Moving;
            }
        }

        private void CalculateMovement()
        {
            time += Time.deltaTime;
            
            if (time >= speed)
            {
                time = 0;
                currentPathwayIndex = nextPathwayIndex;
                
                if (nextPathwayIndex + 1 >= targetPathway.childCount) nextPathwayIndex = 0;
                
                nextPathwayIndex++;
            }
            
            position = Vector3.Slerp(targetPathway.GetChild(currentPathwayIndex).position,
                targetPathway.GetChild(nextPathwayIndex).position, Utility.MapRange(time, 0, speed, 0, 1));
            
            transform.position = position;
        }
    }
}

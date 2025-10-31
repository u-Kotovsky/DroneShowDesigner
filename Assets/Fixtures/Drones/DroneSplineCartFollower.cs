using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;

namespace Fixtures.Drones
{
    public enum CinemachineDroneState
    {
        None,
        WaitBeforeStart,
        Active
    }
    
    public class DroneSplineCartFollower : MonoBehaviour
    {
        public SplineContainer splineContainer;
        public CinemachineSplineCart cart;
        
        public CinemachineDroneState state;

        public float time = 0;
        public float waitBeforeStart = 0;
        public float speed = 0.01f;

        private void Update()
        {
            time += Time.deltaTime;
            
            if (state == CinemachineDroneState.WaitBeforeStart)
            {
                if (time >= waitBeforeStart)
                {
                    time = 0;
                    state = CinemachineDroneState.Active;
                    cart.AutomaticDolly.Enabled = true;
                }
            }
        }

        private void InitializeSplineCartOnDrone(SplineContainer container)
        {
            splineContainer = container;
            
            cart = gameObject.AddComponent<CinemachineSplineCart>();
            cart.Spline = container;
            cart.AutomaticDolly = new SplineAutoDolly
            {
                Method = new SplineAutoDolly.FixedSpeed
                {
                    Speed = speed
                },
                Enabled = false
            };
        }

        public void StartWithDelay(float delay, SplineContainer container)
        {
            time = 0;
            waitBeforeStart = delay;
            state = CinemachineDroneState.WaitBeforeStart;
            InitializeSplineCartOnDrone(container);
        }
    }
}

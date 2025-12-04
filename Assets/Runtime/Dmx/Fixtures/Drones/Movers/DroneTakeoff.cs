using UnityEngine;

namespace Runtime.Dmx.Fixtures.Drones.Movers
{
    [RequireComponent(typeof(BaseDrone))]
    public class DroneTakeoff : MonoBehaviour
    {
        public BaseDrone component;

        private int state = 0;

        private int layer = 0;
        private const int TotalCountOfLayers = 8;

        private float timer = 0;
        
        private void Start()
        {
            EnsureComponentAssigned();
        }

        private void EnsureComponentAssigned()
        {
            component ??= GetComponent<BaseDrone>();

            layer = component.fixtureIndex % TotalCountOfLayers;
        }

        private void OnValidate()
        {
            EnsureComponentAssigned();
        }
        
        private void Update()
        {
            switch (state)
            {
                case 0:
                    timer += Time.deltaTime;
                    if (timer >= 0)
                    {
                        timer = 0;
                        state = 1;
                    }
                    break;
                case 1:

                    break;
            }
        }
    }
}

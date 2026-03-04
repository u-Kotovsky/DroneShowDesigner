using System.Collections.Generic;
using Runtime.Dmx.Fixtures.Drones;
using UnityEngine;

namespace Runtime.Core.Formations
{
    public class Formation : MonoBehaviour
    {
        public bool overrideColors = false;
        public bool localPosition = false;

        [Tooltip("List of roots where each root contains a point that references drone target position.")]
        public List<Transform> rootsPoints = new List<Transform>(); // a list of point's roots
        // so
        // list
        // - root1
        //  - point1
        //  - point2
        // - root2
        // etc.
        public Transform[] points;
        public Color[] colors;
        public List<Transform> drones;
        public List<LightingDrone> lightingDrones;

        private void Update() // TODO: optimize this
        {
            // todo sort by closest etc.
            for (var i = 0; i < drones.Count && i < points.Length; i++)
            {
                var drone = drones[i];
                var ld = lightingDrones[i];

                if (i < points.Length)
                {
                    var point = points[i];

                    if (point is null)
                    {
                        drone.localPosition = Vector3.zero;
                        if (ld is not null) ld.Color = Color.black;
                        continue;
                    }

                    if (localPosition)
                    {
                        drone.localPosition = point.localPosition;
                    }
                    else
                    {
                        drone.position = point.position;
                    }

                    if (ld is not null) ld.Color = Color.red;
                }
                else
                {
                    if (ld is not null) ld.Color = Color.black;
                }
            }
        }
    }
}

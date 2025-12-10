using System.Collections.Generic;
using Runtime.Dmx.Fixtures.Shared;
using UnityEngine;

namespace Runtime.Dmx.Fixtures.Drones
{
    public class BaseDrone : BaseMobile, IDrone
    {
        private byte xCoarse;
        private byte xFine;
    
        private byte yCoarse;
        private byte yFine;
    
        private byte zCoarse;
        private byte zFine;
        
        protected Renderer[] DroneRenderers;
        
        private void Start()
        {
            var renderers = new List<Renderer>();
            
            foreach (Transform childs in transform)
                if (childs.gameObject.TryGetComponent(out Renderer droneRenderer))
                    renderers.Add(droneRenderer);
            
            DroneRenderers = renderers.ToArray();
        }
    }
}

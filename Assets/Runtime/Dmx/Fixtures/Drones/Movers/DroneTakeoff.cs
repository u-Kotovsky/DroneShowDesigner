using System;
using UnityEngine;

namespace Runtime.Dmx.Fixtures.Drones.Movers
{
    [RequireComponent(typeof(BaseDrone))]
    public class DroneTakeoff : MonoBehaviour
    {
        public BaseDrone baseDrone;
        public LightingDrone lightingDrone;
        public PyroDrone pyroDrone;
        
        public int state = 0;

        public int layer = 0;
        public int xIndex = 0;
        public int zIndex = 0;
        
        public int layerTakingoff = 0;
        public float timeToTakeOff = 2;
        
        private Vector3 takeoffStartPosition;
        private Vector3 takeoffEndPosition;
        private Vector3 restPosition;
        
        private const int TotalCountOfLayers = 5;

        public float timer = 0;
        
        private void Awake()
        {
            EnsureComponentAssigned();
        }

        private void EnsureComponentAssigned()
        {
            if (TryGetComponent<LightingDrone>(out var lightingDrone))
            {
                this.lightingDrone ??= lightingDrone;
                SetLayer(this.lightingDrone.fixtureIndex);
            }
            else if (TryGetComponent<PyroDrone>(out var pyroDrone))
            {
                this.pyroDrone ??= pyroDrone;
                SetLayer(this.pyroDrone.fixtureIndex);
            }
            else if (TryGetComponent<BaseDrone>(out var baseDrone))
            {
                this.baseDrone ??= baseDrone;
                SetLayer(this.baseDrone.fixtureIndex);
            }
            else
            {
                Debug.LogError("Failed to get drone component for Drone Take off.");
            }
        }

        private void SetLayer(int fixtureIndex)
        {
            layer = fixtureIndex % TotalCountOfLayers;
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
                        
                        lightingDrone.color = Color.black;
                    }
                    
                    break;
                case 1:
                    timer += Time.deltaTime;

                    if (layerTakingoff == layer)
                    {
                        transform.localPosition = Vector3.Lerp(takeoffStartPosition, takeoffEndPosition, timer / timeToTakeOff);
                        lightingDrone.color = Color.magenta;
                    }

                    if (timer >= timeToTakeOff)
                    {
                        if (layerTakingoff == layer)
                        {
                            SetColorFromLayer();
                        }
                        if (layerTakingoff < TotalCountOfLayers - 1)
                        {
                            layerTakingoff++;
                        }
                        else
                        {
                            state = 3; // 2
                            layerTakingoff = 0;
                        }
                        timer = 0;
                    }
                    break;
                
                case 2:
                    // Assuming all drones are in air in their places (almost)
                    // We need to move each drone based on their layer on Z axis by layer (layercount - currentlayer)
                    
                    //var invertLayer = TotalCountOfLayers - layer;
                    
                    timer += Time.deltaTime;
                    
                    if (layerTakingoff == layer) 
                        transform.localPosition = Vector3.Lerp(takeoffEndPosition, restPosition, timer / timeToTakeOff);

                    if (timer >= timeToTakeOff)
                    {
                        if (layerTakingoff < TotalCountOfLayers - 1)
                        {
                            layerTakingoff++;
                        }
                        else
                        {
                            state = 3;
                            layerTakingoff = 0;
                        }
                        
                        timer = 0;
                    }
                    
                    break;
                
                case 3:
                    timer += Time.deltaTime;
                    lightingDrone.color = Color.white;

                    if (timer >= 5)
                    {
                        timer = 0;
                        state = 4;
                        layerTakingoff = TotalCountOfLayers - 1;
                        SetColorFromLayer();
                    }
                    break;
                
                case 4:
                    timer += Time.deltaTime;
                    
                    if (layerTakingoff == layer)
                    {
                        transform.localPosition =
                            Vector3.Lerp(takeoffEndPosition, takeoffStartPosition, timer / timeToTakeOff);
                        lightingDrone.color = Color.magenta;
                    }

                    if (timer >= timeToTakeOff)
                    {
                        if (layerTakingoff == layer)
                        {
                            lightingDrone.color = Color.black;
                        }
                        if (layerTakingoff >= 0)
                        {
                            layerTakingoff--;
                        }
                        else
                        {
                            state = 0; // 2
                            layerTakingoff = 0;
                        }
                        timer = 0;
                    }
                    break;
                
            }
        }

        public void SetColorFromLayer()
        {
            if (lightingDrone == null) return;
            
            switch (layer)
            {
                case 0:
                    lightingDrone.color = Color.red;
                    break;
                case 1:
                    lightingDrone.color = Color.yellow;
                    break;
                case 2:
                    lightingDrone.color = Color.green;
                    break;
                case 3:
                    lightingDrone.color = Color.cyan;
                    break;
                case 4:
                    lightingDrone.color = Color.blue;
                    break;
            }
        }

        public void SetPositionTakeoffStart(int totalCount, float size, float offset, float x, float y, float z)
        {
            //var offset = size / 2;
            
            //transform.localPosition = new Vector3(x - offset, 0/*700 + (c * 5)*/, y - offset);
            //takeoffPosition = new Vector3(x, y, z);
            takeoffStartPosition = new Vector3(x - offset, y, z - offset);

            var layerHeightOffset = Vector3.up * ((5 * layer) + 10);
            takeoffEndPosition = takeoffStartPosition + layerHeightOffset;
            
            // TODO: by their index in X line AND layer index.
            //var invertedLayerIndex = TotalCountOfLayers - layer;
            
            //var invertedXIndex = size - x;
            //var index = invertedXIndex % size;
            
            // x is waht I need to use
            
            //Debug.Log($"{index} {xIndex} {size}, {invertedXIndex} {x} {z}");
            
            restPosition = takeoffEndPosition + (Vector3.right * (size - x));; // TODO: also Vector3.right
            
            transform.localPosition = takeoffStartPosition;

        }

        private void OnDrawGizmos()
        {
            Debug.DrawLine(takeoffStartPosition, takeoffEndPosition, Color.red);
            Debug.DrawLine(takeoffEndPosition, restPosition, Color.green);
        }

        private void Test()
        {
            var counter = 0;
            var size = 1;// Mathf.Sqrt(lightingDronePool.Length);
            var offset = size / 2;
                
            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    //if (lightingDronePool.Length <= counter) return;
                    //var drone = lightingDronePool[counter];

                    var c = counter % 5;
                    
                    transform.localPosition = new Vector3(x - offset, 700 + (c * 5), y - offset);
                    
                    counter++;
                }
            }
        }
    }
}

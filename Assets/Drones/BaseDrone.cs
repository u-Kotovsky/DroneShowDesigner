using System;
using System.Collections.Generic;
using UnityEngine;

namespace Drones
{
    public class BaseDrone : MonoBehaviour, IDrone
    {
        public DroneSpawnManager droneSpawnManager;
    
        protected byte[] buffer;
        
        // btw drone space is -800, 800 in global space
    
        private byte xCoarse;
        private byte xFine;
    
        private byte yCoarse;
        private byte yFine;
    
        private byte zCoarse;
        private byte zFine;
        
        protected Renderer[] droneRenderers; // visuals
        
        public int globalChannelStart;

        private void Start()
        {
            List<Renderer> renderers = new List<Renderer>();
            
            foreach (Transform childs in transform)
            {
                if (childs.gameObject.TryGetComponent<Renderer>(out Renderer droneRenderer))
                {
                    renderers.Add(droneRenderer);
                }
            }
            
            droneRenderers = renderers.ToArray();
        }

        public void WriteDmxPosition(Vector3 position)
        {
            xCoarse = GetCoarse(position.x);
            xFine = GetFine(position.x);
            
            yCoarse = GetCoarse(position.y);
            yFine = GetFine(position.y);
            
            zCoarse = GetCoarse(position.z);
            zFine = GetFine(position.z);
            
            buffer[0] = xCoarse;
            buffer[1] = xFine;
        
            buffer[4] = yCoarse;
            buffer[5] = yFine;
        
            buffer[2] = zCoarse;
            buffer[3] = zFine;
        }
        
        public static float MapRange(float value, float fromMin, float fromMax, float toMin, float toMax) {
            // Clamp input to source range
            //if (value < fromMin) value = fromMin;
            //if (value > fromMax) value = fromMax;
            value = Mathf.Clamp(value, fromMin, fromMax);
    
            // Map from source range to 0-1
            float normalized = (value - fromMin) / (fromMax - fromMin);
    
            // Map from 0-1 to target range
            return toMin + (normalized * (toMax - toMin));
        }
        
        //int coarse = scaledValue >> 8;  // Divide by 256 using bit shift
        //int fine = scaledValue & 0xFF;  // Get remainder using bit mask

        public static byte GetCoarse(float value, float minValue = -800, float maxValue = 800)
        {
            value = Mathf.Clamp(value, minValue, maxValue);
            
            float normalized = Mathf.InverseLerp(minValue, maxValue, value);
            int scaledValue = (int)(normalized * 65535.0f); // Scale to 16-bit range (0-65535)
            byte coarse = (byte)(scaledValue / 256);
            //int coarse = scaledValue >> 8;  // Divide by 256 using bit shift

            return (byte)coarse;
        }

        public static byte GetFine(float value, float minValue = -800, float maxValue = 800)
        {
            value = Mathf.Clamp(value, minValue, maxValue);
            
            float normalized = Mathf.InverseLerp(minValue, maxValue, value);
            int scaledValue = (int)(normalized * 65535.0f); // Scale to 16-bit range (0-65535)
            byte fine = (byte)(scaledValue % 256);
            //int fine = scaledValue & 0xFF;  // Get remainder using bit mask

            return (byte)fine;
        }
    }
}

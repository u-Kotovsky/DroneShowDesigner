using UnityEngine;

namespace Drones
{
    public class LightingDrone : BaseDrone
    {
        #region Color
        private byte r;
        private byte g;
        private byte b;
        
        public Color color = new Color(0, 0, 0, 1);

        public void WriteDmxColor(Color value)
        {
            r = (byte)MapRange(value.r, 0, 1, 0, 255);
            g = (byte)MapRange(value.g, 0, 1, 0, 255);
            b = (byte)MapRange(value.b, 0, 1, 0, 255);
            
            buffer[6] = r;
            buffer[7] = g;
            buffer[8] = b;
            
            if (droneRenderers == null) return;
            foreach (var droneRenderer in droneRenderers)
            {
                if (droneRenderer == null || droneRenderer.sharedMaterial == null)
                    continue;
                
                droneRenderer.sharedMaterial.SetColor("_BaseColor", color);
            }
        }
        #endregion
        
        private void Awake()
        {
            buffer = new byte[9]; // 9 Channels
        }
        
        public byte[] GetDmxData()
        {
            WriteDmxPosition(transform.position);
            WriteDmxColor(color);
            return buffer;
        }
    }
}

using UnityEngine;

namespace Drones
{
    public class LightingDrone : BaseDrone
    {
        #region Color
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private byte r;
        private byte g;
        private byte b;
        
        public Color color = new Color(0, 0, 0, 1);

        public void WriteDmxColor(Color value)
        {
            r = (byte)Utility.MapRange(value.r, 0, 1, 0, 255);
            g = (byte)Utility.MapRange(value.g, 0, 1, 0, 255);
            b = (byte)Utility.MapRange(value.b, 0, 1, 0, 255);
            
            Buffer[6] = r;
            Buffer[7] = g;
            Buffer[8] = b;
            
            if (DroneRenderers == null) return;
            foreach (var droneRenderer in DroneRenderers)
            {
                if (droneRenderer == null || droneRenderer.sharedMaterial == null)
                    continue;
                
                droneRenderer.sharedMaterial.SetColor(BaseColor, color);
            }
        }
        #endregion
        
        private void Awake()
        {
            Buffer = new byte[9];
        }
        
        public new byte[] GetDmxData()
        {
            WriteDmxPosition(0, transform.position, true);
            WriteDmxColor(color);
            return Buffer;
        }
    }
}

using UnityEngine;

namespace Runtime.Dmx.Fixtures.Drones
{
    public class LightingDronePreset
    {
        public byte XCoarse;
        public byte XFine;
    
        public byte YCoarse;
        public byte YFine;
    
        public byte ZCoarse;
        public byte ZFine;
    
        public byte R;
        public byte G;
        public byte B;

        public LightingDronePreset(byte xCoarse, byte xFine, byte yCoarse, byte yFine, byte zCoarse, byte zFine, byte r, byte g, byte b)
        {
            XCoarse = xCoarse;
            XFine = xFine;
            YCoarse = yCoarse;
            YFine = yFine;
            ZCoarse = zCoarse;
            ZFine = zFine;
            
            R = r;
            G = g;
            B = b;
        }

        public void WritePositionToDrone(BaseDrone drone)
        {
            drone.transform.position = GetPosition(drone.minPosition, drone.maxPosition);
        }

        public void WriteColorToDrone(LightingDrone drone)
        {
            drone.color = GetColor();
        }

        public void ReadPositionFromDrone(BaseDrone drone)
        {
            drone.transform.position = GetPosition(drone.minPosition, drone.maxPosition);
        }

        public void ReadColorFromDrone(LightingDrone drone)
        {
            drone.color = GetColor();
        }
        
        public Vector3 GetPosition(Vector3 minPosition, Vector3 maxPosition)
        {
            return new Vector3(
                Utility.GetValueFromCoarseFine(XCoarse, XFine, minPosition.x, maxPosition.x),
                Utility.GetValueFromCoarseFine(YCoarse, YFine, minPosition.y, maxPosition.y),
                Utility.GetValueFromCoarseFine(ZCoarse, ZFine, minPosition.z, maxPosition.z));
        }
        
        public void SetPosition(Vector3 position, Vector3 minPosition, Vector3 maxPosition)
        {
            XCoarse = Utility.GetCoarse(position.x, minPosition.x, maxPosition.x);
            XFine = Utility.GetFine(position.x, minPosition.x, maxPosition.x);
        
            YCoarse = Utility.GetCoarse(position.y, minPosition.y, maxPosition.y);
            YFine = Utility.GetFine(position.y, minPosition.y, maxPosition.y);
        
            ZCoarse = Utility.GetCoarse(position.z, minPosition.z, maxPosition.z);
            ZFine = Utility.GetFine(position.z, minPosition.z, maxPosition.z);
        }

        public Color GetColor()
        {
            return new Color(
                Utility.MapRange(R, 0, 255, 0, 1),
                Utility.MapRange(G, 0, 255, 0, 1),
                Utility.MapRange(B, 0, 255, 0, 1));
        }

        public void SetColor(Color color)
        {
            R = (byte)Utility.MapRange(color.r, 0, 1, 0, 255);
            G = (byte)Utility.MapRange(color.g, 0, 1, 0, 255);
            B = (byte)Utility.MapRange(color.b, 0, 1, 0, 255);
        }
    }
}

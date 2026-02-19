namespace Runtime.Core.Serialization.Skybrush
{
    public class LightingDroneKeyframe
    {
        public int Time; // msec
        
        public float X;
        public float Y;
        public float Z;
        
        public byte R;
        public byte G;
        public byte B;

        public static LightingDroneKeyframe Parse(string line)
        {
            var data = new LightingDroneKeyframe();

            var parts = line.Split(',');
            
            data.Time = int.Parse(parts[0]);
            
            data.X = float.Parse(parts[1]);
            data.Y = float.Parse(parts[2]);
            data.Z = float.Parse(parts[3]);
            
            data.R = byte.Parse(parts[4]);
            data.G = byte.Parse(parts[5]);
            data.B = byte.Parse(parts[6]);

            return data;
        }

        public string Serialize()
        {
            return $"{Time},{X},{Y},{Z},{R},{G},{B}";
        }
    }
}

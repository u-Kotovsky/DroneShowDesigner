using UnityEngine;

namespace Fixtures
{
    /**
     * Thanks a lot to Micca, happyrobot33, Talos for explaining coarse & fine workflow! <3
     */

    public class Utility
    {
        public static float MapRange(float value, float fromMin, float fromMax, float toMin, float toMax) {
            // Clamp input to source range
            value = Mathf.Clamp(value, fromMin, fromMax);
    
            // Map from source range to 0-1
            float normalized = (value - fromMin) / (fromMax - fromMin);
    
            // Map from 0-1 to target range
            return toMin + (normalized * (toMax - toMin));
        }
    
        public static byte GetCoarse(float input, float minValue, float maxValue)
        {
            // 1) normalize
            double normalized = Mathf.InverseLerp(minValue, maxValue, input);
            // 2) scale
            uint value = (uint)(normalized * ushort.MaxValue);
            // 3) get upper byte
            double coarse = value >> 8;
            // 4) return byte value
            return (byte)coarse;
        }

        public static byte GetFine(float input, float minValue, float maxValue)
        {
            // 1) normalize
            double normalized = Mathf.InverseLerp(minValue, maxValue, input);
            // 2) scale
            uint value = (uint)(normalized * ushort.MaxValue);
            // 3) get upper byte
            double fine = value & 0xFF;
            // 4) return byte value
            return (byte)fine;
        }
    
        public static byte GetCoarse(float input) // input = 0 .. 1
        {
            // 1) scale
            uint value = (uint)(input * ushort.MaxValue);
            // 2) get upper byte
            double coarse = value >> 8;
            // 3) return byte value
            return (byte)coarse;
        }

        public static byte GetFine(float input) // input = 0 .. 1
        {
            // 1) scale
            uint value = (uint)(input * ushort.MaxValue);
            // 2) get upper byte
            double fine = value & 0xFF;
            // 3) return byte value
            return (byte)fine;
        }
    
        public static float ReverseCoarseFine(byte coarse, byte fine, float minValue = -800, float maxValue = 800)
        {
            ushort fullValue = (ushort)((fine << 8) | coarse);
            float value = fullValue / (float)ushort.MaxValue;
            return Mathf.Lerp(minValue, maxValue, value);
        }
    
        public static float GetValueFromCoarseFine(byte coarse, byte fine, float minValue, float maxValue)
        {
            uint combinedValue = ((uint)coarse << 8) | fine;
            float normalized = combinedValue / (float)ushort.MaxValue;
            return Mathf.Lerp(minValue, maxValue, normalized);
        }
    
        public static float GetValueFromCoarseFine(byte coarse, byte fine)
        {
            uint combinedValue = ((uint)coarse << 8) | fine;
            float normalized = combinedValue / (float)ushort.MaxValue;
            return normalized;
        }
        
        public static void CopyDmxValuesAsArray(byte[] buffer)
        {
            GUIUtility.systemCopyBuffer = "[" + string.Join(", ", buffer) + "]";
        }
        
        public static void CopyDmxValuesAsArray(byte[] dmxData, int offset, int size)
        {
            byte[] bytes = new byte[size];
                    
            System.Buffer.BlockCopy(dmxData, offset, bytes, 0, size);

            CopyDmxValuesAsArray(bytes);
        }
        
        public static void CopyValuesAsArray(string[] buffer)
        {
            GUIUtility.systemCopyBuffer = string.Join("\n", buffer); 
        }

        public static void CopyAllDmxValuesAsMa3Representation(byte[] dmxData, int globalChannelStart)
        {
            int universe = (int)Mathf.Floor(globalChannelStart / 512) + 1;
            int addressStart = (globalChannelStart % 512) + 1;
            string[] values = new string[dmxData.Length];
                    
            for (var i = 0; i < values.Length; i++)
                values[i] = $"{universe}.{addressStart + i} {dmxData[i]}";

            CopyValuesAsArray(values);
        }

        public static void CopyDmxValuesWithOffsetAsMa3Representation(byte[] dmxData, int globalChannelStart, int offset, int size)
        {
            byte[] bytes = new byte[size];
                    
            System.Buffer.BlockCopy(dmxData, offset, bytes, 0, size);
            
            int universe = (int)Mathf.Floor(globalChannelStart / 512) + 1;
            int addressStart = (globalChannelStart % 512) + 1;
            string[] values = new string[bytes.Length];
                    
            for (var i = offset; i < values.Length; i++)
                values[i] = $"{universe}.{addressStart + i} {bytes[i]}";

            CopyValuesAsArray(values);
            
        }
    }
}
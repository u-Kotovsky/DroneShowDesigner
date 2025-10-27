using UnityEngine;

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
    
    // I don't know what am I doing okay
    public static byte GetCoarse(float input, float minValue = -800, float maxValue = 800)
    {
        // 1) clamp
        input = Mathf.Clamp(input, minValue, maxValue);
        // 2) normalize
        float normalized = Mathf.InverseLerp(minValue, maxValue, input);
        // 3) scale
        uint value = (uint)(normalized * ushort.MaxValue);
        // 4) get upper byte
        float coarse = value >> 8;
        // 5) return byte value
        return (byte)coarse;
    }

    public static byte GetFine(float input, float minValue = -800, float maxValue = 800)
    {
        // 1) clamp
        input = Mathf.Clamp(input, minValue, maxValue);
        // 2) normalize
        float normalized = Mathf.InverseLerp(minValue, maxValue, input);
        // 3) scale
        uint value = (uint)(normalized * ushort.MaxValue);
        // 4) get upper byte
        float fine = value & 0xFF;
        // 5) return byte value
        return (byte)fine;
    }
}

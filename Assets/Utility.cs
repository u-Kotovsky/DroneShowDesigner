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

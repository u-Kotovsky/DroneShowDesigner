using System.Collections.Generic;
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
    public static byte GetCoarse(float input, float minValue, float maxValue)
    {
        // 1) clamp
        //input = Mathf.Clamp(input, minValue, maxValue);
        // 2) normalize
        double normalized = Mathf.InverseLerp(minValue, maxValue, input);
        // 3) scale
        uint value = (uint)(normalized * ushort.MaxValue);
        // 4) get upper byte
        double coarse = value >> 8;
        //Debug.Log($"input {input} {normalized} {value} [{minValue} {maxValue}] fine {coarse}");
        // 5) return byte value
        return (byte)coarse;
    }

    public static byte GetFine(float input, float minValue, float maxValue)
    {
        // 1) clamp
        //input = Mathf.Clamp(input, minValue, maxValue);
        // 2) normalize
        double normalized = Mathf.InverseLerp(minValue, maxValue, input);
        // 3) scale
        uint value = (uint)(normalized * ushort.MaxValue);
        // 4) get upper byte
        double fine = value & 0xFF;
        //Debug.Log($"input {input} {normalized} {value} [{minValue} {maxValue}] fine {fine}");
        // 5) return byte value
        return (byte)fine;
    }
    
    public static byte GetCoarse(float input) // input = 0 .. 1
    {
        // 1) clamp
        //input = Mathf.Clamp(input, minValue, maxValue);
        // 2) normalize
        //double normalized = Mathf.InverseLerp(minValue, maxValue, input);
        // 3) scale
        uint value = (uint)(input * ushort.MaxValue);
        // 4) get upper byte
        double coarse = value >> 8;
        //Debug.Log($"input {input} {input} {value} fine {coarse}");
        // 5) return byte value
        return (byte)coarse;
    }

    public static byte GetFine(float input) // input = 0 .. 1
    {
        // 1) clamp
        //input = Mathf.Clamp(input, minValue, maxValue);
        // 2) normalize
        //double normalized = Mathf.InverseLerp(minValue, maxValue, input);
        // 3) scale
        uint value = (uint)(input * ushort.MaxValue);
        // 4) get upper byte
        double fine = value & 0xFF;
        //Debug.Log($"input {input} {input} {value} [{minValue} {maxValue}] fine {fine}");
        // 5) return byte value
        return (byte)fine;
    }
    
    public static float ReverseCoarseFine(byte coarse, byte fine, float minValue = -800, float maxValue = 800)
    {
        ushort fullValue = (ushort)((fine << 8) | coarse);
        float value = fullValue / (float)ushort.MaxValue;
        return Mathf.Lerp(minValue, maxValue, value); //MapRange(value, 0, 1, minValue, maxValue);
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
}

// Thanks to happyrobot33
// https://github.com/Happyrobot33/HNode/blob/a8d12403b56d42f0db62f41996344e0f3f7ddd91/Assets/Util.cs#L43
public class CoarseFineChannelSet
{
    public byte coarse;
    public byte fine;

    public CoarseFineChannelSet(byte coarse, byte fine)
    {
        this.coarse = coarse;
        this.fine = fine;
    }

    public CoarseFineChannelSet(float value)
    {
        var channelSet = GetCoarseFineChannelRepresentation(value);
        this.coarse = channelSet.coarse;
        this.fine = channelSet.fine;
    }

    /// <summary>
    /// Converts a float value between 0 and 1 to a coarse/fine byte array representation.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static CoarseFineChannelSet GetCoarseFineChannelRepresentation(float value)
    {
        ushort fullValue = (ushort)(Mathf.Clamp01(value) * ushort.MaxValue);
        var bytes = System.BitConverter.GetBytes(fullValue);
        return new CoarseFineChannelSet(bytes[1], bytes[0]);
    }

    public static float GetValueRepresentation(byte coarse, byte fine)
    {
        ushort fullValue = System.BitConverter.ToUInt16(new[] { fine, coarse });
        float value = fullValue / (float) ushort.MaxValue;
        return value;
    }

    public List<byte> ToList()
    {
        return new List<byte> { coarse, fine };
    }
}

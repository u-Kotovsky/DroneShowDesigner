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
    public static byte GetCoarse(float input, float minValue = -800, float maxValue = 800)
    {
        // 1) clamp
        input = Mathf.Clamp(input, minValue, maxValue);
        input = MapRange(input, minValue, maxValue, 0, 1);
        var coarseFine = CoarseFineChannelSet.GetCoarseFineChannelRepresentation(input);
        // 2) normalize
        //float normalized = Mathf.InverseLerp(minValue, maxValue, input);
        // 3) scale
        //uint value = (uint)(normalized * ushort.MaxValue);
        // 4) get upper byte
        //float coarse = value >> 8;
        // 5) return byte value
        //return (byte)coarse;
        return coarseFine.coarse;
    }

    public static byte GetFine(float input, float minValue = -800, float maxValue = 800)
    {
        // 1) clamp
        input = Mathf.Clamp(input, minValue, maxValue);
        input = MapRange(input, minValue, maxValue, 0, 1);
        var coarseFine = CoarseFineChannelSet.GetCoarseFineChannelRepresentation(input);
        // 2) normalize
        //float normalized = Mathf.InverseLerp(minValue, maxValue, input);
        // 3) scale
        //uint value = (uint)(normalized * ushort.MaxValue);
        // 4) get upper byte
        //float fine = value & 0xFF;
        // 5) return byte value
        return coarseFine.fine;
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
        //convert to 16 bit value
        ushort fullValue = (ushort)(Mathf.Clamp01(value) * ushort.MaxValue);
        var bytes = System.BitConverter.GetBytes(fullValue);
        return new CoarseFineChannelSet(bytes[1], bytes[0]);
        /* byte coarse = (byte)Mathf.Floor(value * byte.MaxValue);

        //get a remainding ammount
        double usedbycoarse = Mathf.Floor(value * byte.MaxValue) / byte.MaxValue;
        double newvalue = (value - usedbycoarse);

        byte fine = (byte)Mathf.Round((float)(newvalue * 256 * 255));

        return new CoarseFineChannelSet(coarse, fine); */
    }

    public List<byte> ToList()
    {
        return new List<byte> { coarse, fine };
    }
}

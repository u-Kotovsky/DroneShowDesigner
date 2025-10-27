using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DMXDevice : MonoBehaviour
{
    public byte[] dmxData;
    public int startChannel;
    public abstract int NumChannels { get; }

    public virtual void SetData(byte[] dmxData)
    {
        this.dmxData = dmxData;
    }
    
    public enum ChannelFunction
    {
        Unknown = -1,

        Color_R = 0,
        Color_RFine = 1,
        Color_G = 2,
        Color_GFine = 3,
        Color_B = 4,
        Color_BFine = 5,
        Color_W = 6,
        Color_WFine = 7,

        Pan = 8,
        PanFine = 9,
        Tilt = 10,
        TiltFine = 11,
        RotSpeed = 12,
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class SimpleDMXLight : DMXDevice
{
    new Light light;

    public override int NumChannels{ get { return 4; } }

    public override void SetData(byte[] dmxData)
    {
        base.SetData(dmxData);

        var color = light.color;

        color.r = dmxData[0] / 256f;
        color.g = dmxData[1] / 256f;
        color.b = dmxData[2] / 256f;
        color += Color.white * 0.5f * dmxData[3] / 256f;

        light.color = color;
    }

    private void Start()
    {
        light = GetComponent<Light>();
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class DmxSendGUI : MonoBehaviour
{
    public Slider channelSliderOrigin;
    public short universe;
    public int fps;


    [SerializeField] DmxController controller;
    [SerializeField] Slider[] channelSliders;
    [SerializeField] byte[] dmxData;
    Thread dmxSender;

    [ContextMenu("build GUI")]
    void BuildGUI()
    {
        for (var i = 0; i < 512; i++)
        {
            var slider = channelSliders[i] = Instantiate(channelSliderOrigin, channelSliderOrigin.transform.parent);
            slider.name = string.Format("Slider{0:d3}", i);
            var rTrs = slider.GetComponent<RectTransform>();
            rTrs.localPosition = new Vector3(rTrs.rect.width * (i % 128 - 64), (rTrs.rect.height + 32) * (i / 128 - 2), 0);
        }
    }

    public void SetSendingDMX(bool b)
    {
        if (dmxSender != null)
            dmxSender.Abort();
        if (b)
        {
            dmxSender = new Thread(SendDmx);
            dmxSender.Start();
        }
        else
            dmxSender = null;
    }
    public void SetUniverse(string str)
    {
        universe = short.Parse(str);
    }
    public void SetFps(string str)
    {
        var fps = int.Parse(str);
        this.fps = Mathf.Max(1, fps);
    }
    void SetDmxValue(int channel, float val)
    {
        dmxData[channel] = (byte)Mathf.FloorToInt(Mathf.Lerp(0, 255, val));
    }

    // Use this for initialization
    void Start()
    {
        dmxData = new byte[512];
        universe = 0;
        fps = 30;
        for (var i = 0; i < channelSliders.Length; i++)
        {
            var channel = i;
            channelSliders[channel].onValueChanged.AddListener((f) => SetDmxValue(channel, f));
        }
    }

    private void OnDestroy()
    {
        if (dmxSender != null)
            dmxSender.Abort();
    }

    void SendDmx()
    {
        while (true)
        {
            controller.Send(universe, dmxData);
            Thread.Sleep(System.Math.Max(1, 1000 / fps));
        }
    }
}

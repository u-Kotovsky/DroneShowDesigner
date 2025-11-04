namespace Runtime.Dmx.Fixtures.Lights
{
    public abstract class MobileLightPresetManager
    {
        public static MobileLightPreset[][] presets;
        
        static MobileLightPresetManager()
        {
            // Example
            MobileLightPreset[] preset1 = new MobileLightPreset[8];
            preset1[0] = new MobileLightPreset(127, 255, 122, 79, 157, 65);
            preset1[1] = new MobileLightPreset(149, 240, 122, 79, 149, 240);
            preset1[2] = new MobileLightPreset(157, 65, 122, 79, 127, 255);
            preset1[3] = new MobileLightPreset(149, 240, 122, 79, 106, 14);
            preset1[4] = new MobileLightPreset(127, 255, 122, 79, 98, 189);
            preset1[5] = new MobileLightPreset(106, 14, 122, 79, 106, 14);
            preset1[6] = new MobileLightPreset(98, 189, 122, 79, 127, 255);
            preset1[7] = new MobileLightPreset(106, 14, 122, 79, 149, 240);

            presets = new MobileLightPreset[1][];
            presets[0] = preset1;
        }
    }
}

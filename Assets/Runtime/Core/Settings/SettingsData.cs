using System;

namespace Runtime.Core.Settings
{
    [Serializable]
    public class SettingsData
    {
        public ArtNetData artNetConfig = new();

        public int targetFrameRate = 60;

        public bool enableMobileTruss = false;
        public bool enableMobileLight = false;
        public bool enableLightingDrones = false;
        public bool enablePyroDrones = false;
        
        public bool compressText = false;
    }
}

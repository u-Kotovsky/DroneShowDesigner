using System;

namespace Runtime.Core.Settings
{
    [Serializable]
    public class SettingsData
    {
        public ArtNetData artNetConfig = new();
    }
}

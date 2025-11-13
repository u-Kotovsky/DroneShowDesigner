using System;

namespace Runtime.Core.Timeline
{
    [Serializable]
    public class TimelineData
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }

        public CueData[] Cues { get; set; } = Array.Empty<CueData>();
    }
}
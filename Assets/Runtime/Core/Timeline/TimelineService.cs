using System.IO;
using UnityEngine;

namespace Runtime.Core.Timeline
{
    public class TimelineService
    {
        public static TimelineService instance = new();
        
        public static TimelineData currentTimelineData = new();

        public static TimelineData GetTimelineData()
        {
            return currentTimelineData ?? new TimelineData();
        }

        private static ulong _currentTime = 0;

        public static ulong GetCurrentTime()
        {
            return _currentTime;
        }

        public static void Load(string pathToFile)
        {
            if (!File.Exists(pathToFile))
            {
                throw new FileNotFoundException();
            }
            
            var data = File.ReadAllText(pathToFile);
            currentTimelineData = JsonUtility.FromJson<TimelineData>(data);
        }

        public static void Save(string pathToFile)
        {
            string json = JsonUtility.ToJson(currentTimelineData);

            if (File.Exists(pathToFile))
            {
                // TODO: rename existing file to a .bak
            }
            
            // TODO: compression?
            
            File.WriteAllText(pathToFile, json);
        }
    }
}

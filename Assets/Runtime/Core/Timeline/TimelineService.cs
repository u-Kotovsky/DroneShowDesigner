using System;
using System.IO;
using Melanchall.DryWetMidi.Multimedia;
using Runtime.Midi;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Runtime.Core.Timeline
{
    public class TimelineService
    {
        private static TimelineService _instance;
        public static TimelineService Instance => _instance ??= new();

        public static TimelineData currentTimelineData;
        public static TimelineData GetTimelineData()
        {
            return currentTimelineData ??= new TimelineData();
        }

        private static long _currentTime = 0;
        public static long GetCurrentTime()
        {
            return _currentTime;
        }

        public static event Action<long> OnCurrentTimeChanged = (time) => { };
        
        private static MidiTimeCodeReceiver _midiTimeCodeReceiver;
        public static void Initialize()
        {
            Debug.Log("Initializing TimelineService");
            try
            {
                // hook OnMidiTimeCode event
                _midiTimeCodeReceiver = Object.FindFirstObjectByType<MidiTimeCodeReceiver>();
                Debug.Log(_midiTimeCodeReceiver == null);
                
                _midiTimeCodeReceiver.OnTimeCodeReceived += MidiTimeCodeReceiverOnOnTimeCodeReceived;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private static void MidiTimeCodeReceiverOnOnTimeCodeReceived(MidiTimeCodeReceivedEventArgs e)
        {
            try
            {
                var hoursToFrames = e.Hours * 3600 * 30;
                var minutesToFrames = e.Minutes * 60 * 30;
                var secondsToFrames = e.Seconds * 30;
                
                long totalTime = hoursToFrames + secondsToFrames + minutesToFrames + e.Frames;
                Debug.Log($"MidiTimeCode totalTime: {totalTime}");
                
                OnCurrentTimeChanged?.Invoke(totalTime);
                
                _currentTime = totalTime;
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        public static void Load(string pathToFile)
        {
            if (!File.Exists(pathToFile)) throw new FileNotFoundException();
            
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

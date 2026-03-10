using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Runtime.Core.Settings
{
    public class SettingsService
    {
        public static SettingsService Instance = new();
        public static SettingsData Data = new();

        private const string Prefix = "SettingsService";

        public static event Action<SettingsData> OnSettingsChanged = delegate { };

        public static void Load(string pathToFile)
        {
            Debug.Log($"'{Prefix}' Loading settings from '{pathToFile}'");
            if (!File.Exists(pathToFile))
                throw new FileNotFoundException();
            
            var text = File.ReadAllText(pathToFile, Encoding.UTF8);
            Data = JsonUtility.FromJson<SettingsData>(text);
            OnSettingsChanged?.Invoke(Data);
        }

        public static void Save(string pathToFile)
        {
            Debug.Log($"'{Prefix}' Saving settings to '{pathToFile}'");
            string json = JsonUtility.ToJson(Data);
            
            // TODO: compress settings data
            
            File.WriteAllText(pathToFile, json, Encoding.UTF8);
            OnSettingsChanged?.Invoke(Data);
        }
    }
}
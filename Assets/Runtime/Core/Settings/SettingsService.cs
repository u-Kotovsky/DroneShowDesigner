using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Runtime.Core.Settings
{
    public class SettingsService
    {
        public static SettingsService instance = new();
        public static SettingsData data = new();

        private const string Prefix = "SettingsService";

        public static event Action<SettingsData> OnSettingsChanged = delegate { };

        public static void Load(string pathToFile)
        {
            Debug.Log($"'{Prefix}' Loading settings from '{pathToFile}'");
            if (!File.Exists(pathToFile))
                throw new FileNotFoundException();
            
            var text = File.ReadAllText(pathToFile, Encoding.UTF8);
            data = JsonUtility.FromJson<SettingsData>(text);
            OnSettingsChanged?.Invoke(data);
        }

        public static void Save(string pathToFile)
        {
            Debug.Log($"'{Prefix}' Saving settings to '{pathToFile}'");
            string json = JsonUtility.ToJson(data);
            
            // TODO: compress settings data
            
            File.WriteAllText(pathToFile, json, Encoding.UTF8);
            OnSettingsChanged?.Invoke(data);
        }
    }
}
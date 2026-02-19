using System;
using System.Collections.Generic;
using System.IO;
using Runtime.Core.Serialization.Skybrush;
using Runtime.Core.Skybrush;
using Runtime.Core.Timeline;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace Runtime.UI
{
    public static class SkybrushUI
    {
        private static RectTransform _rootTransform;
        private static RectTransform _listRect;
        private const string Prefix = "SkybrushUI";
        
        static SkybrushUI()
        {
            try
            {
                //var pathToData = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DroneShowDesigner");

                //if (!Directory.Exists(pathToData)) Directory.CreateDirectory(pathToData);
                //_pathToTimelineFile = Path.Join(pathToData, "Timeline.json");
                
                //if (!File.Exists(_pathToTimelineFile)) SaveCurrentFile();
                //LoadFromFile();
                
                TimelineService.Initialize();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public static void Poke()
        {
            // Initialize static part
        }
        
        public static void BuildUI(Transform parent)
        {
            _rootTransform = UIUtility.AddRect(parent, "Timeline");
            _rootTransform.WithVerticalLayout();
            
            UIUtility.SetAllStretch(_rootTransform, Vector4.zero);
            
            // List of groups
            _listRect = UIUtility.CreateVerticalList(_rootTransform, "DronesList");
            
            // Current File Info
            var element0 = UIUtility.AddItemToList(_listRect, -4, 20);
            //_pathToTimelineText = UIUtility.AddText(element0, "path/to/current/timeline", Color.white * .7f);
            
            // Manage Current File
            //var element1 = UIUtility.AddItemToList(_listRect, -3, 20);
            //_saveCurrentFileButton = UIUtility.AddButton(element1, "Save", Color.grey, Color.white)
            //    .OnClick(SaveCurrentFile);
            //_saveAsNewFileButton = UIUtility.AddButton(element1, "Save as File", Color.grey, Color.white)
            //    .OnClick(SaveAsFile);
            //_loadFromFileButton = UIUtility.AddButton(element1, "Load from File", Color.grey, Color.white)
            //    .OnClick(LoadFromFile);
            
            //TimelineService.OnCurrentTimeChanged += OnCurrentTimeChanged;
            //MainUIController.Instance.OnUpdate += Update;
            
            Load();
            RefreshUI();
        }

        private static byte state = 2;
        // 0 = list all drones
        // 1 = list specific drone keyframes
        private static int selectedIndex = -1;

        public static void RefreshUI()
        {
            foreach (Transform o in _listRect.transform)
            {
                UnityEngine.Object.Destroy(o.gameObject);
            }

            switch (state)
            {
                case 0:
                    DrawAllDrones();
                    break;
                case 1:
                    DrawSelectedDrone();
                    break;
                case 2:
                    DrawSelectCsvFile();
                    break;
            }
        }
        
        private static void DrawSelectCsvFile()
        {
            UIUtility.AddItemToList(_listRect, -1, 20, "Select zip file that contains *.csv for each drone"); // Header
            
            // TODO: Select a file popup
            var element = UIUtility.AddItemToList(_listRect, 0, 20);
            UIUtility.AddButton(element, "Select file cue", Color.grey, Color.white)
                .OnClick(() => {  });
            UIUtility.AddButton(element, "Load", Color.grey, Color.white)
                .OnClick(() => {  });
        }

        private static void DrawAllDrones()
        {
            var element = UIUtility.AddItemToList(_listRect, -2, 20);
            
            UIUtility.AddItemToList(_listRect, -1, 20, "Index", "Keyframe count"); // Header
            
            for (var i = 0; i < _lightingDroneKeyFrames.Count; i++)
            {
                var drone = _lightingDroneKeyFrames[i];

                /*for (var j = 0; j < drone.Count; j++)
                {
                    var keyframe = drone[j];
                }*/
                
                var rect = UIUtility.AddItemToList(_listRect, i, 20, $"{i}", $"{drone.Count}"); // Header
                var button = rect.gameObject.AddComponent<Button>();
                var droneIndex = i;
                button.OnClick(() =>
                {
                    selectedIndex = droneIndex;
                    OpenDroneKeyframes(droneIndex); 
                });
            }
        }

        private static void DrawSelectedDrone()
        {
            var element = UIUtility.AddItemToList(_listRect, -2, 20);
            UIUtility.AddButton(element, "Back", Color.grey, Color.white)
                .OnClick(() => { CloseDroneKeyframes(); });
            
            UIUtility.AddItemToList(_listRect, -1, 20, "Index", "Time (msec)", "x", "y", "z", "r", "g", "b"); // Header
            var drone = _lightingDroneKeyFrames[selectedIndex];

            for (var i = 0; i < drone.Count; i++)
            {
                var keyframe = drone[i];
                var rect = UIUtility.AddItemToList(_listRect, i, 20, $"{i}", $"{keyframe.Time}", $"{keyframe.X}", $"{keyframe.Y}", $"{keyframe.Z}", $"{keyframe.R}", $"{keyframe.G}", $"{keyframe.B}"); // Header
                var button = rect.gameObject.AddComponent<Button>();
                var keyframeIndex = i;
                button.OnClick(() =>
                {
                    //selectedIndex = droneIndex;
                    //CloseDroneKeyframes(droneIndex, drone); 
                });
            }
        }

        private static void OpenDroneKeyframes(int index)
        {
            selectedIndex = index;
            state = 1;
            RefreshUI();
        }

        private static void CloseDroneKeyframes()
        {
            selectedIndex = -1;
            state = 0;
            RefreshUI();
        }

        public static void DeconstructUI()
        {
            MainUIController.Instance.OnDeconstructUI -= DeconstructUI;
            //TimelineService.OnCurrentTimeChanged -= OnCurrentTimeChanged;
            //MainUIController.Instance.OnUpdate -= Update;
        }
        
        private static void Update()
        {
            
        }

        private static List<List<LightingDroneKeyframe>> _lightingDroneKeyFrames;

        private static void Load()
        {
            _lightingDroneKeyFrames = SkybrushCSVLoader.Load(@"C:\\Users\Kotovsky\Desktop\skybrush-files\FromationsTestSkybrush.zip");
            SkybrushSKYCLoader.Load(@"C:\\Users\Kotovsky\Desktop\skybrush-files\FromationsTestSkybrush.skyc");
            
            Debug.Log($"Loaded {_lightingDroneKeyFrames.Count} drones");
        }
    }
}

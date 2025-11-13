using System;
using System.Collections.Generic;
using System.IO;
using Runtime.Core.Timeline;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Runtime.UI
{
    public abstract class TimelineUI
    {
        private static GameObject[] _currentPageObjects;
        private static RectTransform[] _currentTimelineObjects;

        private static RectTransform _timelineRootRect, _timelineListRect;
        private static TextMeshProUGUI _pathToTimelineText, _timelineTitleText, _timeCodeText;
        private static Button _saveCurrentFileButton, _saveAsNewFileButton, _loadFromFileButton;
        private static Button _addCue, _editCue, _deleteCue;
        
        static TimelineUI()
        {
            var pathToData =Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DroneShowDesigner");

            if (!Directory.Exists(pathToData))
                Directory.CreateDirectory(pathToData);
            
            _pathToTimelineFile = Path.Join(pathToData, "Timeline.json");
            
            if (!File.Exists(_pathToTimelineFile)) SaveCurrentFile();
            LoadFromFile();
        }
        
        public static void BuildUI(Transform parent)
        {
            _timelineRootRect = UIUtility.AddRect(parent, "Timeline");
            _timelineRootRect.gameObject.AddComponent<VerticalLayoutGroup>();
            
            UIUtility.SetAllStretch(_timelineRootRect, Vector4.zero);
            
            // List of groups
            _timelineListRect = UIUtility.CreateVerticalList(_timelineRootRect, "TimelineList");
            
            // Current File Info
            var element0 = UIUtility.AddItemToList(_timelineListRect, -4, 20);
            _pathToTimelineText = UIUtility.AddText(element0, "path/to/current/timeline", Color.white * .7f);
            
            // Manage Current File
            var element1 = UIUtility.AddItemToList(_timelineListRect, -3, 20);
            _saveCurrentFileButton = UIUtility.AddButton(element1, "Save", Color.grey, Color.white);
            _saveAsNewFileButton = UIUtility.AddButton(element1, "Save as File", Color.grey, Color.white);
            _loadFromFileButton = UIUtility.AddButton(element1, "Load from File", Color.grey, Color.white);
            
            _saveCurrentFileButton.onClick.AddListener(SaveCurrentFile);
            _saveAsNewFileButton.onClick.AddListener(SaveAsFile);
            _loadFromFileButton.onClick.AddListener(LoadFromFile);
            
            // Space
            var element2 = UIUtility.AddItemToList(_timelineListRect, -2, 20);
            UIUtility.AddRect(element2, "Space");
            _timelineTitleText = UIUtility.AddText(element2, "Timeline: CurrentTimelineTitle", Color.white);
            _timeCodeText = UIUtility.AddText(element2, "TimeCode: 00:00:00:00", Color.white);
            
            // Manage cues
            var element3 = UIUtility.AddItemToList(_timelineListRect, -1, 20);
            _addCue = UIUtility.AddButton(element3, "Add cue", Color.grey, Color.white);
            _editCue = UIUtility.AddButton(element3, "Edit cue", Color.grey, Color.white);
            _deleteCue = UIUtility.AddButton(element3, "Delete cue", Color.grey, Color.white);

            _addCue.onClick.AddListener(AddCue);
            _editCue.onClick.AddListener(EditCue);
            _deleteCue.onClick.AddListener(DeleteCue);
            
            UIUtility.AddItemToList(_timelineListRect, 0, 20, "Index", "Name", "Object Count"); // Header

            RefreshUI();
        }

        #region RefreshUI
        public static void RefreshUI()
        {
            RefreshTimelinePathUI();
            RefreshTimelineTitleUI();
            RefreshTimelineListUI();
        }

        private static void RefreshTimelineTitleUI()
        {
            _timelineTitleText.text = $"Timeline: {TimelineService.currentTimelineData.Name}";
        }

        private static void RefreshTimelinePathUI()
        {
            _pathToTimelineText.text = $"{_pathToTimelineFile}";
        }

        private static void RefreshTimelineListUI()
        {
            if (_currentTimelineObjects != null && _currentTimelineObjects.Length > 0)
                foreach (var currentTimelineObject in _currentTimelineObjects)
                    if (currentTimelineObject != null)
                        Object.Destroy(currentTimelineObject.gameObject);

            _currentTimelineObjects = Array.Empty<RectTransform>();
            
            PopulateTimelineListUI();
        }
        
        private static void PopulateTimelineListUI()
        {
            var header = UIUtility.AddItemToList(_timelineListRect, -1, 20, "Index", "Name", "Object Count"); // Header
            List<RectTransform> elements = new() { header };

            for (var i = 0; i < TimelineService.currentTimelineData.Cues.Length; i++)
            {
                var cue = TimelineService.currentTimelineData.Cues[i];
                var element = UIUtility.AddItemToList(_timelineListRect, cue.Index, 20, cue.Name, "0");
                elements.Add(element); 
            }

            _currentTimelineObjects = elements.ToArray();
        }
        #endregion

        #region Save/Load
        private static string _pathToTimelineFile;
        private static void SaveCurrentFile()
        {
            Debug.Log($"Saving current file as '{_pathToTimelineFile}'");
            TimelineService.Save(_pathToTimelineFile);
        }

        private static void SaveAsFile()
        {
            Debug.Log($"Saving new file as '{_pathToTimelineFile}'");
            // TODO: Select where to save popup
            TimelineService.Save(_pathToTimelineFile);
        }

        private static void LoadFromFile()
        {
            try
            {
                Debug.Log($"Loading from file '{_pathToTimelineFile}'");
                // TODO: Select what to load popup
                TimelineService.Load(_pathToTimelineFile);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError($"Failed to load timeline from file '{_pathToTimelineFile}'. Reloading with default settings...");
                // Will not load, but instead save new config file so program won't throw errors anymore, unless there's a bug in it lol
                SaveCurrentFile();
            }
        }

        #endregion
        
        private static void AddCue()
        {
            Debug.Log("Add cue");
        }

        private static void EditCue()
        {
            Debug.Log("Edit cue");
        }

        private static void DeleteCue()
        {
            Debug.Log("Delete cue");
        }
    }
}

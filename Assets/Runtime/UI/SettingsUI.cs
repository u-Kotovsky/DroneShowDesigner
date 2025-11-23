using System;
using System.IO;
using Runtime.Core.Settings;
using Runtime.Dmx.Fixtures;
using TMPro;
using Unity_DMX.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Runtime.UI
{
    public abstract class SettingsUI
    {
        private static RectTransform _rootRect, _groupRect, _containerRect;
        private static TextMeshProUGUI _pathToFileText;
        private static TMP_InputField _targetFrameRate, _artNetInIp, _artNetInPort, _artNetOutIp, _artNetOutPort;
        private static Toggle _enableMobileTruss, _enableMobileLight, _enableLightingDrones, _enablePyroDrones;
        private static Toggle _artNetInToggle, _artNetOutToggle;

        private static FixtureSpawnManager _fixtureSpawner;
        private static DmxController _dmxController;

        static SettingsUI()
        {
            var pathToData = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DroneShowDesigner");

            if (!Directory.Exists(pathToData)) 
                Directory.CreateDirectory(pathToData);
            
            _pathToSettings = Path.Join(pathToData, "Settings.json");

            try
            {
                _fixtureSpawner = UnityEngine.Object.FindFirstObjectByType<FixtureSpawnManager>();
                _dmxController = _fixtureSpawner.dmxController;

                SettingsService.OnSettingsChanged += OnSettingsChanged;

                Load();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError("Failed to hook up settings updates");
            }
        }

        private static void OnSettingsChanged(SettingsData data)
        {
            Debug.Log("Settings Changed");

            try
            {
                // targetFramerate
                Application.targetFrameRate = data.targetFrameRate;
                
                // ArtNet Input
                _dmxController.SetRemote(data.artNetConfig.endPoint.address, data.artNetConfig.endPoint.port);
                _dmxController.enabled = data.artNetConfig.enableInput;
                        
                if (_dmxController.IsArtNetOn) _dmxController.StopArtNet();
                if (data.artNetConfig.enableInput) _dmxController.StartArtNet();
                        
                // ArtNet Output
                _dmxController.redirectTo.SetRemote(data.artNetConfig.redirectTo.address, data.artNetConfig.redirectTo.port);
                _dmxController.redirectTo.enabled = data.artNetConfig.enableOutput;
                _dmxController.redirectPackets = data.artNetConfig.enableOutput;
                        
                if (_dmxController.redirectTo.IsArtNetOn) _dmxController.redirectTo.StopArtNet();
                if (data.artNetConfig.enableInput) _dmxController.redirectTo.StartArtNet();
                        
                // Custom Fixtures
                _fixtureSpawner.UseMobileTruss = data.enableMobileTruss;
                _fixtureSpawner.UseMobileLight = data.enableMobileLight;
                _fixtureSpawner.UsePyroDrone = data.enablePyroDrones;
                _fixtureSpawner.UseLightingDrone = data.enableLightingDrones;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError("Failed to process new settings");
            }
        }

        public static void Poke()
        {
            // Initialize static part lol
        }
        
        public static void BuildUI(RectTransform parent)
        {
            // Root
            _rootRect = UIUtility.AddRect(parent, "Settings Root");
            var rootLayout = _rootRect.gameObject.AddComponent<VerticalLayoutGroup>()
                .ForceExpand(true, false);
            rootLayout.childControlHeight = false;
            UIUtility.SetAllStretch(_rootRect, Vector4.zero);
            
            // Current File Info
            var element0 = UIUtility.AddItemToList(_rootRect, -4, 20);
            _pathToFileText = UIUtility.AddText(element0, "path/to/current/settings", Color.white * .7f);
            
            // Container
            _containerRect = UIUtility.AddRect(_rootRect, "Settings Container");
            var containerLayout = _containerRect.gameObject.AddComponent<HorizontalLayoutGroup>();
            containerLayout.ForceExpand(true, true);
            UIUtility.SetAllStretch(_containerRect, Vector4.zero);
                    
            // List of groups
            var listRect = UIUtility.CreateVerticalList(_containerRect, "SettingsList");
            
            // Save/Cancel
            var actionsRect = UIUtility.AddItemToList(listRect, 0, 20);
            UIUtility.AddButton(actionsRect, "Save", Color.white * .5f, Color.white)
                .OnClick(Save);
            UIUtility.AddButton(actionsRect, "Cancel", Color.white * .5f, Color.white)
                .OnClick(Load);
            
            // Unity frames
            AddInput(listRect, ref _targetFrameRate, 1, "Target Framerate", TMP_InputField.ContentType.IntegerNumber,
                value => { SettingsService.data.targetFrameRate = ushort.Parse(value); });
            
            // ArtNet (Input)
            AddToggle(listRect, ref _artNetInToggle, 2, "Art Net (In) Toggle")
                .OnValueChanged((value) => { SettingsService.data.artNetConfig.enableInput = value; });
            AddInput(listRect, ref _artNetInIp, 3, "Art Net (In) Ip", TMP_InputField.ContentType.Standard)
                .OnValueChanged(value => { SettingsService.data.artNetConfig.endPoint.address = value; });
            AddInput(listRect, ref _artNetInPort, 4, "Art Net (In) Port", TMP_InputField.ContentType.IntegerNumber)
                .OnValueChanged(value => { SettingsService.data.artNetConfig.endPoint.port = ushort.Parse(value); });
            
            // ArtNet (Redirect)
            AddToggle(listRect, ref _artNetOutToggle, 5, "Art Net (Out) Toggle")
                .OnValueChanged(value => { SettingsService.data.artNetConfig.enableOutput = value; });
            AddInput(listRect, ref _artNetOutIp, 6, "Art Net (Out) Ip", TMP_InputField.ContentType.Standard)
                .OnValueChanged(value => { SettingsService.data.artNetConfig.redirectTo.address = value; });
            AddInput(listRect, ref _artNetOutPort, 7, "Art Net (Out) Port", TMP_InputField.ContentType.IntegerNumber)
                .OnValueChanged(value => { SettingsService.data.artNetConfig.redirectTo.port = ushort.Parse(value); });
            
            // Custom Fixtures
            AddToggle(listRect, ref _enableMobileTruss, 8, "Enable Mobile Truss")
                .OnValueChanged(value => { SettingsService.data.enableMobileTruss = value; });
            AddToggle(listRect, ref _enableMobileLight, 9, "Enable Mobile Light")
                .OnValueChanged(value => { SettingsService.data.enableMobileLight = value; });
            AddToggle(listRect, ref _enablePyroDrones, 10, "Enable Pyro Drones")
                .OnValueChanged(value => { SettingsService.data.enablePyroDrones = value; });
            AddToggle(listRect, ref _enableLightingDrones, 11, "Enable Lighting Drones")
                .OnValueChanged(value => { SettingsService.data.enableLightingDrones = value; });
            
            RefreshUI();
        }

        #region Shortenes
        public static Toggle AddToggle(RectTransform listRect, ref Toggle toggle, int index, string text, UnityAction<bool> onValueChanged)
        {
            AddToggle(listRect, ref toggle, index, text);
            toggle.OnValueChanged(onValueChanged);
            return toggle;
        }
        
        public static Toggle AddToggle(RectTransform listRect, ref Toggle toggle, int index, string text)
        {
            var rect = UIUtility.AddItemToList(listRect, index, 20, text);
            toggle = UIUtility.AddToggle(rect, Color.white * .5f, Color.white);
            return toggle;
        }

        public static TMP_InputField AddInput(RectTransform listRect, ref TMP_InputField inputField, int index, string text, 
            TMP_InputField.ContentType contentType, Action<string> onValueChanged)
        {
            AddInput(listRect, ref inputField, index, text, contentType);
            inputField.OnValueChanged(value => { onValueChanged(value); });
            return inputField;
        }

        public static TMP_InputField AddInput(RectTransform listRect, ref TMP_InputField inputField, int index, string text, 
            TMP_InputField.ContentType contentType)
        {
            var rect = UIUtility.AddItemToList(listRect, index, 20, text);
            inputField = UIUtility.AddInputField(rect, Color.white * .5f, Color.white);
            inputField.contentType = contentType;
            return inputField;
        }

        #endregion

        #region RefreshUI
        public static void RefreshUI()
        {
            Debug.Log("Refreshing Settings UI");
            RefreshFixturesUI();
            RefreshFramesPerSecondUI();
            RefreshPathToFileUI();
            RefreshArtNetInputUI();
            RefreshArtNetOutputUI();
        }

        private static void RefreshFixturesUI()
        {
            if (_enableMobileTruss != null) _enableMobileTruss.isOn = SettingsService.data.enableMobileTruss;
            if (_enableMobileLight != null) _enableMobileLight.isOn = SettingsService.data.enableMobileLight;
            if (_enablePyroDrones != null) _enablePyroDrones.isOn = SettingsService.data.enablePyroDrones;
            if (_enableLightingDrones != null) _enableLightingDrones.isOn = SettingsService.data.enableLightingDrones;
        }

        private static void RefreshFramesPerSecondUI()
        {
            if (_targetFrameRate != null) _targetFrameRate.text = "" + SettingsService.data.targetFrameRate;
        }
        
        private static void RefreshPathToFileUI()
        {
            if (_pathToFileText != null) _pathToFileText.text = _pathToSettings;
        }

        private static void RefreshArtNetInputUI()
        {
            if (SettingsService.data == null)
            {
                Debug.LogError("Settings data is null");
                return;
            }
            
            if (_artNetInToggle != null) _artNetInToggle.isOn = SettingsService.data.artNetConfig.enableInput;
            if (_artNetInIp != null) _artNetInIp.text = SettingsService.data.artNetConfig.endPoint.address;
            if (_artNetInPort != null) _artNetInPort.text = "" + SettingsService.data.artNetConfig.endPoint.port;
        }

        private static void RefreshArtNetOutputUI()
        {
            if (SettingsService.data == null)
            {
                Debug.LogError("Settings data is null");
                return;
            }
            
            if (_artNetOutToggle != null) _artNetOutToggle.isOn = SettingsService.data.artNetConfig.enableOutput;
            if (_artNetOutIp != null) _artNetOutIp.text = SettingsService.data.artNetConfig.redirectTo.address;
            if (_artNetOutPort != null) _artNetOutPort.text = "" + SettingsService.data.artNetConfig.redirectTo.port;
        }
        #endregion

        #region Save/Load
        private static string _pathToSettings;
        
        private static void Save()
        {
            SettingsService.Save(_pathToSettings);
        }

        private static void Load()
        {
            if (!File.Exists(_pathToSettings))
            {
                Debug.LogError($"Settings file '{_pathToSettings}' does not exist, loading defaults.");
                Save();
            }
            
            try
            {
                SettingsService.Load(_pathToSettings);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError($"Failed to load settings from file '{_pathToSettings}'. Reloading with default settings...");
                // Will not load, but instead save new config file so program won't throw errors anymore, unless there's a bug in it lol
                Save();
            }

            RefreshUI();
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using Runtime.Core.Movement;
using Runtime.UI.Setup.Patch;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class MainUIController : MonoBehaviour
    {
        public RectTransform hotbar;
        public RectTransform page;

        public RectTransform hierarchy;
        public RectTransform inspector;

        [Header("Unity Assets")]
        [Tooltip("Since Unity doesn't have a proper way of givin' us developers default assets, I steal it through this reference. Please keep it as UISprite.")]
        public Sprite defaultUISprite;
        public static Sprite DefaultUISprite;
        
        private static List<Button> hotbarButtons;

        private Camera targetCamera;
        private SpectatorCameraController cameraController;

        private const string Prefix = "MainUIController";

        private void Awake()
        {
            try
            {
                RefreshCameraReferences();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError($"'{Prefix}' Failed to refresh camera references");
            }
            
            DefaultUISprite ??= defaultUISprite;
            hotbarButtons = new List<Button>();
            var buttonColor = Color.gray3;
            var textColor = Color.white;
            
            var doNotShowPageAbout = PlayerPrefs.GetInt("DoNotShowPageAbout") == 1;
            
            UIUtility.AddButton(hotbar, "About", buttonColor, textColor, button =>
            {
                button.OnClick(() =>
                {
                    CleanScreen();
                    SetHotBarButtons(true);
                    button.interactable = false;
                    Debug.Log($"'{Prefix}' Open About");
                    AboutUI.BuildUI(page);
                    cameraController?.DisableMovement();
                });
                
                hotbarButtons.Add(button);
                if (!doNotShowPageAbout) button.onClick.Invoke();
            });
            
            UIUtility.AddButton(hotbar, "Setup", buttonColor, textColor, button =>
            {
                button.OnClick(() =>
                {
                    CleanScreen();
                    SetHotBarButtons(true);
                    button.interactable = false;
                    Debug.Log($"'{Prefix}' Open Setup");
                    SetupPatchUI.BuildUI(page);
                    cameraController?.DisableMovement();
                });
                
                hotbarButtons.Add(button);
            });
            
            UIUtility.AddButton(hotbar, "Console", buttonColor, textColor, button =>
            {
                button.OnClick(() =>
                {
                    CleanScreen();
                    SetHotBarButtons(true);
                    button.interactable = false;
                    Debug.Log($"'{Prefix}' Open Console");
                    cameraController?.DisableMovement();
                });
                
                hotbarButtons.Add(button);
            });
            
            UIUtility.AddButton(hotbar, "Settings", buttonColor, textColor, button =>
            {
                button.OnClick(() =>
                {
                    CleanScreen();
                    SetHotBarButtons(true);
                    button.interactable = false;
                    Debug.Log($"'{Prefix}' Open Settings");
                    SettingsUI.BuildUI(page);
                    cameraController?.DisableMovement();
                });
                
                hotbarButtons.Add(button);
            });
            
            UIUtility.AddButton(hotbar, "Editor", buttonColor, textColor, button =>
            {
                button.OnClick(() =>
                {
                    CleanScreen();
                    SetHotBarButtons(true);
                    button.interactable = false;
                    Debug.Log($"'{Prefix}' Open Editor");
                    cameraController?.EnableMovement();
                });
                
                hotbarButtons.Add(button);
                if (doNotShowPageAbout) button.onClick.Invoke(); 
            });
            
            UIUtility.AddButton(hotbar, "Timeline", buttonColor, textColor, button =>
            {
                button.OnClick(() =>
                {
                    CleanScreen();
                    SetHotBarButtons(true);
                    button.interactable = false;
                    Debug.Log($"'{Prefix}' Open Timeline");
                    TimelineUI.BuildUI(page);
                    cameraController?.DisableMovement();
                });
                
                hotbarButtons.Add(button);
            });
        }

        private void Start()
        {
            SettingsUI.Poke();
            SettingsUI.Load();
        }

        public static void Open(int index)
        {
            if (index < 0) index = 0;
            if (index > hotbarButtons.Count - 1) index = hotbarButtons.Count - 1;
            
            hotbarButtons[index].onClick.Invoke();
        }

        private void RefreshCameraReferences()
        {
            targetCamera = Camera.main;
            if (targetCamera != null) cameraController = targetCamera?.GetComponent<SpectatorCameraController>();
        }

        private void CleanScreen()
        {
            foreach (Transform child in page.transform)
                Destroy(child.gameObject);
        }

        private void SetHotBarButtons(bool active)
        {
            foreach (var button in hotbarButtons) // null
                button.interactable = active;
        }
    }
}
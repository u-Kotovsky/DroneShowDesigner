using System;
using System.Collections.Generic;
using Runtime.Core.Movement;
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
        
        private List<Button> hotbarButtons;

        private Camera targetCamera;
        private SpectatorCameraController cameraController;

        private void Awake()
        {
            try
            {
                RefreshCameraReferences();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError($"Failed to refresh camera references");
            }
            
            DefaultUISprite ??= defaultUISprite;
            hotbarButtons = new List<Button>();
            var buttonColor = Color.gray3;
            var textColor = Color.white;
            
            UIUtility.AddButton(hotbar, "About", buttonColor, textColor, button =>
            {
                button.onClick.AddListener(() =>
                {
                    CleanScreen();
                    SetHotBarButtons(true);
                    button.interactable = false;
                    Debug.Log("Open About");
                    AboutUI.BuildUI(page);
                    cameraController?.DisableMovement();
                });
                
                hotbarButtons.Add(button);
                button.onClick.Invoke();
            });
            
            UIUtility.AddButton(hotbar, "Console", buttonColor, textColor, button =>
            {
                button.onClick.AddListener(() =>
                {
                    CleanScreen();
                    SetHotBarButtons(true);
                    button.interactable = false;
                    Debug.Log("Open Console");
                    cameraController?.DisableMovement();
                });
                
                hotbarButtons.Add(button);
            });
            
            UIUtility.AddButton(hotbar, "Settings", buttonColor, textColor, button =>
            {
                button.onClick.AddListener(() =>
                {
                    CleanScreen();
                    SetHotBarButtons(true);
                    button.interactable = false;
                    Debug.Log("Open Settings");
                    SettingsUI.BuildUI(page);
                    cameraController?.DisableMovement();
                });
                
                hotbarButtons.Add(button);
            });
            
            UIUtility.AddButton(hotbar, "Editor", buttonColor, textColor, button =>
            {
                button.onClick.AddListener(() =>
                {
                    CleanScreen();
                    SetHotBarButtons(true);
                    button.interactable = false;
                    Debug.Log("Open Editor");
                    cameraController?.EnableMovement();
                });
                
                hotbarButtons.Add(button);
            });
            
            UIUtility.AddButton(hotbar, "Timeline", buttonColor, textColor, button =>
            {
                button.onClick.AddListener(() =>
                {
                    CleanScreen();
                    SetHotBarButtons(true);
                    button.interactable = false;
                    Debug.Log("Open Timeline");
                    TimelineUI.BuildUI(page);
                    cameraController?.DisableMovement();
                });
                
                hotbarButtons.Add(button);
            });
            
            SettingsUI.Poke();
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
            foreach (var hotbarButton in hotbarButtons) // null
                hotbarButton.interactable = active;
        }
        
        public void LoadHierarchy()
        {
            
        }

        public void InspectElement(GameObject element)
        {
            // TODO: load element components, parameters etc.
        }
    }
}
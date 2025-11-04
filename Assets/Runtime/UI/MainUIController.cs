using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.UI
{
    public class MainUIController : MonoBehaviour
    {
        public RectTransform hotbar;
        public RectTransform pages;
        public RectTransform page;

        public RectTransform hierarchy;
        public RectTransform inspector;

        [Header("Unity Assets")]
        [Tooltip("Since Unity doesn't have a proper way of givin' us developers default assets, I steal it through this reference. Please keep it as UISprite.")]
        public Sprite defaultUISprite;
        private static Sprite _defaultUISprite;
        
        private List<Button> hotbarButtons;

        private void Awake()
        {
            _defaultUISprite ??= defaultUISprite;
            hotbarButtons = new List<Button>();
            var buttonColor = Color.gray3;
            var textColor = Color.white;
            
            UIUtility.AddButton(defaultUISprite, hotbar, "Settings", buttonColor, textColor, button =>
            {
                button.onClick.AddListener(() =>
                {
                    SetHotBarButtons(true);
                    button.interactable = false;
                    Debug.Log("Open Settings");
                });
                
                hotbarButtons.Add(button);
                
                button.onClick.Invoke();
            });
            
            UIUtility.AddButton(defaultUISprite, hotbar, "Console", buttonColor, textColor, button =>
            {
                button.onClick.AddListener(() =>
                {
                    SetHotBarButtons(true);
                    button.interactable = false;
                    Debug.Log("Open Console");
                });
                
                hotbarButtons.Add(button);
            });
            
            UIUtility.AddButton(defaultUISprite, hotbar, "Editor", buttonColor, textColor, button =>
            {
                button.onClick.AddListener(() =>
                {
                    SetHotBarButtons(true);
                    button.interactable = false;
                    Debug.Log("Open Editor");
                });
                
                hotbarButtons.Add(button);
            });
            
            
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
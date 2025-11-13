using System.Collections.Generic;
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

        private void Awake()
        {
            DefaultUISprite ??= defaultUISprite;
            hotbarButtons = new List<Button>();
            var buttonColor = Color.gray3;
            var textColor = Color.white;
            
            UIUtility.AddButton(hotbar, "Console", buttonColor, textColor, button =>
            {
                button.onClick.AddListener(() =>
                {
                    CleanScreen();
                    SetHotBarButtons(true);
                    button.interactable = false;
                    Debug.Log("Open Console");
                });
                
                hotbarButtons.Add(button);
                button.onClick.Invoke();
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
                });
                
                hotbarButtons.Add(button);
            });
            
            SettingsUI.Poke();
        }

        private void CleanScreen()
        {
            foreach (Transform child in page.transform)
            {
                Destroy(child.gameObject);
            }
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
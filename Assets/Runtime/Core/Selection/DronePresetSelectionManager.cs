using UnityEngine;

namespace Runtime.Core.Selection
{
    public class DronePresetSelectionManager : MonoBehaviour
    {
        private void Awake()
        {
            // TODO:
            // Listen for keybind to save selection into preset
            // Listen for keybind to write preset into selection
            // A place to have a list of presets?
            // 
        }

        private void Update()
        {
            bool controlKey = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            
            if (controlKey && Input.GetKeyDown(KeyCode.C))
            {
                // TODO: put in clipboard some json with data
                Debug.Log("Copy current selection");
            }

            if (controlKey && Input.GetKeyDown(KeyCode.V))
            {
                // TODO: load preset from clipboard as json text
                Debug.Log("Paste current selection");
            }
            
            if (controlKey && Input.GetKeyDown(KeyCode.S))
            {
                // TODO: Save to file
                Debug.Log("Save current selection");
            }
        }
    }
}

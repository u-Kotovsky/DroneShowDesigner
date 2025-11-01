using UnityEngine;

namespace Runtime.UI
{
    public class MainUIController : MonoBehaviour
    {
        public RectTransform hotbar;
        public RectTransform pages;

        public RectTransform hierarchy;
        public RectTransform inspector;

        private void Awake()
        {
            
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
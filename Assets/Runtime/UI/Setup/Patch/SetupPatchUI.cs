using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Runtime.UI.Setup.Patch
{
    public class SetupPatchUI
    {
        // TODO: view current patch, add/edit/remove layers, fixtures.
        public static bool UIExist { get; private set; }
        public static RectTransform RectTransform { get; private set; }

        private static Button _addLayer;
        private static Button _deleteLayer;
        private static Button _editLayer;

        private static void Button_AddLayer_OnClick()
        {
            Debug.Log("Button_AddLayer_OnClick");
        }
        
        private static void Button_EditLayer_OnClick()
        {
            Debug.Log("Button_EditLayer_OnClick");
        }
        
        private static void Button_DeleteLayer_OnClick()
        {
            Debug.Log("Button_DeleteLayer_OnClick");
        }
        
        public static void BuildUI(RectTransform parent)
        {
            if (UIExist)
            {
                throw new Exception(nameof(SetupPatchUI) + " has already been built.");
            }

            RectTransform = UIUtility.AddRect(parent, nameof(SetupPatchUI));

            var image = RectTransform.gameObject.AddComponent<Image>();
            image.color = new Color(.2f, .2f, .2f, 1);
            
            UIUtility.AddText(RectTransform, "WOOO IM PATCH", Color.white);

            _addLayer = UIUtility.AddButton(RectTransform, "Add Layer", Color.white * .5f, Color.white);
            _editLayer = UIUtility.AddButton(RectTransform, "Edit Layer", Color.white * .5f, Color.white);
            _deleteLayer = UIUtility.AddButton(RectTransform, "Delete Layer", Color.white * .5f, Color.white);

            _addLayer.OnClick(Button_AddLayer_OnClick);
            _editLayer.OnClick(Button_EditLayer_OnClick);
            _deleteLayer.OnClick(Button_DeleteLayer_OnClick);
            
            UIExist = true;
        }

        public static void DeleteUI()
        {
            if (!UIExist)
            {
                throw new Exception(nameof(SetupPatchUI) + " has not been built yet.");
            }

            if (RectTransform == null)
            {
                throw new Exception(nameof(SetupPatchUI) + " RectTransform is null.");
            }
            
            Object.Destroy(RectTransform.gameObject);

            UIExist = false;
        }
    }
}

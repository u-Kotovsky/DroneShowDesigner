using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Runtime.UI.Setup.Patch
{
    public static class SetupPatchUI
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

        private static void Button_AddFixture_OnClick()
        {
            Debug.Log("Button_AddFixture_OnClick");
        }
        
        private static void Button_EditFixture_OnClick()
        {
            Debug.Log("Button_EditLayer_OnClick");
        }
        
        private static void Button_DeleteFixture_OnClick()
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
            RectTransform
                .WithImage(new Color(.2f, .2f, .2f, 1))
                .WithVerticalLayout()
                .ForceExpand(false, false)
                .ControlChildSize(true, false)
                .SetAllStretch(0, 0, 0, 0);
            
            UIUtility.AddText(RectTransform, "Used parameters: -/Infinity", Color.white);

            var listGroup = UIUtility.AddRect(RectTransform, "ListGroup")
                .SetHeight(20)
                .WithHorizontalLayout()
                .ForceExpand(true, false)
                .ControlChildSize(true, false)
                .GetRect();
            
            var layerList = UIUtility.CreateVerticalList(listGroup, "Layers");
            UIUtility.AddItemToList(layerList, 0, 20, "Header");
            UIUtility.AddItemToList(layerList, 1, 20, "Layer 1");
            UIUtility.AddItemToList(layerList, 2, 20, "Layer 2");
            UIUtility.AddItemToList(layerList, 3, 20, "Layer 3");
            UIUtility.AddItemToList(layerList, 4, 20, "Layer 4");
            
            var fixtureList = UIUtility.CreateVerticalList(listGroup, "Fixtures");
            UIUtility.AddItemToList(fixtureList, 0, 20, "Header");
            UIUtility.AddItemToList(fixtureList, 1, 20, "Fixture 1");
            UIUtility.AddItemToList(fixtureList, 2, 20, "Fixture 2");
            UIUtility.AddItemToList(fixtureList, 3, 20, "Fixture 3");
            UIUtility.AddItemToList(fixtureList, 4, 20, "Fixture 4");
            
            var buttonGroup = UIUtility.AddRect(RectTransform, "ButtonGroup")
                .SetHeight(20)
                .WithHorizontalLayout()
                .ForceExpand(true, false)
                .GetRect();

            var buttonGroup1 = UIUtility.AddRect(buttonGroup, "ButtonGroup1")
                .WithHorizontalLayout()
                .ForceExpand()
                .GetRect();
            
            _addLayer = UIUtility.AddButton(buttonGroup1, "Add Layer", Color.white * .5f, Color.white)
                .OnClick(Button_AddLayer_OnClick);
            _editLayer = UIUtility.AddButton(buttonGroup1, "Edit Layer", Color.white * .5f, Color.white)
                .OnClick(Button_EditLayer_OnClick);
            _deleteLayer = UIUtility.AddButton(buttonGroup1, "Delete Layer", Color.white * .5f, Color.white)
                .OnClick(Button_DeleteLayer_OnClick);

            var buttonGroup2 = UIUtility.AddRect(buttonGroup, "ButtonGroup2")
                .WithHorizontalLayout()
                .ForceExpand()
                .GetRect();
            
            UIUtility.AddButton(buttonGroup2, "Add Fixture", Color.white * .5f, Color.white);
            UIUtility.AddButton(buttonGroup2, "Edit Fixture", Color.white * .5f, Color.white);
            UIUtility.AddButton(buttonGroup2, "Delete Fixture", Color.white * .5f, Color.white);
            
            UIExist = true;
        }
        
        public static void DeconstructUI()
        {
            MainUIController.Instance.OnDeconstructUI -= DeconstructUI;
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

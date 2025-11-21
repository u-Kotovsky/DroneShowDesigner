using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Runtime.UI.Setup.Fixture
{
    public class SetupFixtureTypesUI
    {
        // TODO: view existing FixtureTypes in list.
        
        public static bool UIExist { get; private set; }
        public static RectTransform RectTransform { get; private set; }
        
        
        public static void BuildUI(RectTransform parent)
        {
            if (UIExist)
            {
                throw new Exception(nameof(SetupFixtureTypesUI) + " has already been built.");
            }

            RectTransform = UIUtility.AddRect(parent, nameof(SetupFixtureTypesUI));

            UIExist = true;
        }

        public static void DeleteUI()
        {
            if (!UIExist)
            {
                throw new Exception(nameof(SetupFixtureTypesUI) + " has not been built yet.");
            }

            if (RectTransform == null)
            {
                throw new Exception(nameof(SetupFixtureTypesUI) + " RectTransform is null.");
            }
            
            Object.Destroy(RectTransform.gameObject);

            UIExist = false;
        }
    }
}

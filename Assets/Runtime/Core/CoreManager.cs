using System;
using Runtime.Core.Selection;
using Runtime.Dmx.Fixtures;
using Runtime.UI;
using UnityEngine;

namespace Runtime.Core
{
    public class CoreManager : MonoBehaviour
    {
        private void Awake()
        {
            var fixtureSpawnManager = FindFirstObjectByType<FixtureSpawnManager>();
            fixtureSpawnManager.OnFixturesInitialized += PostInit;
            fixtureSpawnManager.Initialize();
            
            var mainUIController = FindFirstObjectByType<MainUIController>();
            mainUIController.Initialize();
            ;
            var objectManipulateManager = FindFirstObjectByType<ObjectManipulateManager>();
            objectManipulateManager.Initialize();
        }

        private void PostInit()
        {
            Debug.Log("Post-initialization");
            SettingsUI.Poke();
            SettingsUI.Load();
            TimelineUI.Poke();
        }
    }
}

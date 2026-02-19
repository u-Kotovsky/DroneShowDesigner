using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Core
{
    public class KeybindManager
    {
        private static KeybindManager instance;

        public static KeybindManager GetInstance()
        {
            return instance ??= new KeybindManager();
        }
        
        public Dictionary<string, List<KeyCode>> bindings = new ()
        {
            { "Select.Single", new List<KeyCode> { KeyCode.Mouse0 } },
        };
        
        // TODO: a keybinds manager from which every script will take keycodes and check for their state.
        // TODO: an UI panel that lets you change keybinds.
    }
}

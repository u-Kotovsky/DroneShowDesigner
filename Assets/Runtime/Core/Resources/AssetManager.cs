using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.Core.Resources
{
    public class AssetManager : MonoBehaviour
    {
        // TODO: Use unity bundles instead of internal resources.
        public static Queue<AssetRequest<GameObject>> gameObjectQueue { get; private set; } = new();
        private const string Prefix = "AssetManager";
        
        public static void Load(string path, Action<GameObject> callback)
        {
            var request = new AssetRequest<GameObject>(path, callback);
            Debug.Log($"'{Prefix}' Added resource request '{request.Path}' to gameObject queue.");
            gameObjectQueue.Enqueue(request);
        }

        private void Update()
        {
            while (gameObjectQueue.Count > 0)
            {
                try
                {
                    AssetRequest<GameObject> request = gameObjectQueue.Dequeue();
                    Debug.Log($"'{Prefix}' load '{request.Path}' prefab");
                    var asset = UnityEngine.Resources.Load<GameObject>(request.Path);
                    if (asset == null) throw new Exception($"'{Prefix}' Failed to load resource request '{request.Path}'.");
                    request.Callback(asset);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}

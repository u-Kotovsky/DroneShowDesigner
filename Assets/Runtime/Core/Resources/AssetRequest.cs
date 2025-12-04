using System;

namespace Runtime.Core.Resources
{
    public class AssetRequest<T>
    {
        public string Path { get; set; }
        public Action<T> Callback { get; set; }

        public AssetRequest(string path, Action<T> callback)
        {
            Path = path;
            Callback = callback;
        }
    }
}

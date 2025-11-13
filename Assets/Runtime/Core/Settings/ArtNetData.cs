using System;

namespace Runtime.Core.Settings
{
    [Serializable]
    public class ArtNetData
    {
        public bool isServer = false;
        public bool isBroadcast = false;
        
        public bool enableInput = false;
        public SimpleEndPointData endPoint = new();

        public bool enableOutput = false;
        public SimpleEndPointData redirectTo = new("127.0.0.1", 6455);
    }
}
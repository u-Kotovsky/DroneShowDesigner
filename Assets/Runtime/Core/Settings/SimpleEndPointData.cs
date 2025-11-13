using System;

namespace Runtime.Core.Settings
{
    [Serializable]
    public class SimpleEndPointData
    {
        public string address = "127.0.0.1";
        public ushort port = 6454;

        public SimpleEndPointData() {}

        public SimpleEndPointData(string address, ushort port = 6454)
        {
            this.address = address;
            this.port = port;
        }
    }
}

using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace Unity_DMX
{
    public abstract class NetworkUtility
    {
        public static IPAddress FindFromHostName(string hostname)
        {
            var address = IPAddress.None;
            try
            {
                if (IPAddress.TryParse(hostname, out address)) return address;

                var addresses = Dns.GetHostAddresses(hostname);
                foreach (var t in addresses)
                {
                    if (t.AddressFamily == AddressFamily.InterNetwork)
                    {
                        address = t;
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Failed to find IP for :\n host name = {0}\n exception={1}", hostname, e);
            }
            return address;
        }
    }
}

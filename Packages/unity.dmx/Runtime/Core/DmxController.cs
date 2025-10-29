using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using ArtNet.Packets;
using ArtNet.Sockets;
using ArtNet.Enums;
using Unity_DMX.Device;
using UnityEngine;

namespace Unity_DMX.Core
{
    public class DmxController : MonoBehaviour
    {
        [Header("Send dmx")]
        public bool useBroadcast;
        public string remoteIP = "localhost";
        private IPEndPoint remote;
        public int remotePort = 6454;

        [Header("DMX devices")]
        public UniverseDevices[] universes;
        public bool isServer;

        private ArtNetSocket artnet;
        [Header("Send/Recieve DMX data for debug")]
        //[SerializeField] 
        //private ArtNetDmxPacket latestReceivedDmx;
        //[SerializeField] 
        private ArtNetDmxPacket dmxToSend;
        private byte[] _dmxData;
        private Dictionary<int, byte[]> dmxDataMap;
    
        public void Send(short universe, byte[] dmxData)
        {
            dmxToSend.Universe = universe;
        
            Buffer.BlockCopy(dmxData, 0, dmxToSend.DmxData, 0, dmxData.Length);

            if (useBroadcast && isServer)
                artnet.Send(dmxToSend);
            else
                artnet.Send(dmxToSend, remote);
        }

        private void OnValidate()
        {
            foreach (var u in universes)
                u.Initialize();
        }
        
        private void OnDestroy()
        {
            artnet.Close();
        }
        
        private void Awake()
        {
            artnet = new ArtNetSocket();
            if (isServer) artnet.Open(FindFromHostName(remoteIP), remotePort, null);
            //サブネットマスクを設定すると、自分に送らないアドレスを設定してくれる（便利！）
            //なのだが、デバッグがめんどくさくなる
            dmxToSend ??= new ArtNetDmxPacket();
            dmxToSend.DmxData = new byte[512];
            dmxDataMap = new Dictionary<int, byte[]>();
            
            artnet.NewPacket += (object sender, NewPacketEventArgs<ArtNetPacket> e) =>
            {
                if (e.Packet.OpCode != ArtNetOpCodes.Dmx) return;
                
                var packet = e.Packet as ArtNetDmxPacket;
                
                if (packet == null) throw new NullReferenceException();
                
                if (packet.DmxData != _dmxData) _dmxData = packet.DmxData;

                var universe = packet.Universe;
                
                if (dmxDataMap.ContainsKey(universe))
                {
                    dmxDataMap[universe] = packet.DmxData;
                    return;
                }
                
                dmxDataMap.Add(universe, packet.DmxData);
            };

            if (!useBroadcast || !isServer) remote = new IPEndPoint(FindFromHostName(remoteIP), remotePort);
        }

        private void Update()
        {
            if (dmxToSend is null) return;
            var keys = dmxDataMap.Keys.ToArray();

            for (var i = 0; i < keys.Length; i++)
            {
                var universe = keys[i];
                var dmxData = dmxDataMap[universe];
                if (dmxData == null) continue;

                var universeDevices = universes.FirstOrDefault(u => u.universe == universe);
                if (universeDevices != null)
                    foreach (var d in universeDevices.devices)
                        d.SetData(dmxData.Skip(d.startChannel).Take(d.NumChannels).ToArray());

                dmxDataMap[universe] = null;
            }
        }

        private static IPAddress FindFromHostName(string hostname)
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

        [Serializable]
        public class UniverseDevices
        {
            public string universeName;
            public int universe;
            public DMXDevice[] devices;

            public void Initialize()
            {
                var startChannel = 0;
                foreach (var d in devices)
                {
                    if (d == null) continue;
                    d.startChannel = startChannel;
                    startChannel += d.NumChannels;
                    d.name = $"{d.GetType()}:({universe},{d.startChannel:d3}-{startChannel - 1:d3})";
                }

                if (512 < startChannel)
                {
                    Debug.LogErrorFormat("The number({0}) of channels of the universe {1} exceeds the upper limit(512 channels)!", startChannel, universe);
                }
            }
        }
    }
}

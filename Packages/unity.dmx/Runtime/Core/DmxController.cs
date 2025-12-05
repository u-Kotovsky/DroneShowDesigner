using System;
using System.Collections.Generic;
using System.Net;
using ArtNet.Packets;
using ArtNet.Sockets;
using ArtNet.Enums;
using UnityEngine;

namespace Unity_DMX.Core
{
    public class DmxController : MonoBehaviour
    {
        [Header("Configuration")]
        public DmxBuffer dmxBuffer;
        public bool useBroadcast;
        public string remoteIp = "127.0.0.1";
        private IPEndPoint remote; // For sending packets?
        public int remotePort = 6454;
        public bool isServer;

        [Header("Redirect")]
        public bool redirectPackets;
        public DmxController redirectTo;

        private ArtNetSocket socket;
        private ArtNetDmxPacket dmxToSend;
        
        private const string Prefix = "DmxControlller";
        private string ServerOrClient => isServer ? "server" : "client";
        
        public void SetRemote(string ip, int port)
        {
            remoteIp = ip;
            remotePort = port;
        }
        
        private void Awake()
        {
            StartArtNet();
            
            dmxToSend ??= new ArtNetDmxPacket();
            dmxToSend.DmxData ??= new byte[512];
            dmxDataMap = new Dictionary<int, byte[]>();
            
            dmxBuffer.OnBufferUpdate += OnBufferUpdate;
        }

        public void InitializeSocket()
        {
            Debug.Log($"'{Prefix}' '{remotePort}' is " + ServerOrClient);
            socket = new ArtNetSocket(remotePort);
            socket.NewPacket += OnNewPacketReceived;
            
            if (isServer) // !useBroadcast || !isServer
            {
                // When you set the subnet mask, it will set the address you do not send to yourself (Convenient!）
                // However, debugging becomes troublesome
                socket.Open(NetworkUtility.FindFromHostName(remoteIp), remotePort, NetworkUtility.FindFromHostName(remoteIp));
            }
            else
            {
                remote = new IPEndPoint(NetworkUtility.FindFromHostName(remoteIp), remotePort);
            }
            
            dmxBuffer.OnBufferUpdate += OnBufferUpdate;
        }

        #region ArtNet Toggle
        public bool IsArtNetOn { get; private set; }
        public void StartArtNet()
        {
            Debug.Log($"'{Prefix}' '{remotePort}' as " + ServerOrClient + " is now requested to be started.");
            if (IsArtNetOn) return;
            InitializeSocket();
            IsArtNetOn = true;
        }

        public void StopArtNet()
        {
            Debug.Log($"'{Prefix}' '{remotePort}' as " + ServerOrClient + " is now requested to be stopped.");
            if (!IsArtNetOn) return;
            socket.Close();
            socket.NewPacket -= OnNewPacketReceived;
            socket = null;
            remote = null;
            IsArtNetOn = false;
            dmxBuffer.OnBufferUpdate -= OnBufferUpdate;
        }
        #endregion

        #region Buffer or DMX512 data
        private Dictionary<int, byte[]> dmxDataMap;
        public event Action<short, byte[], byte[]> OnDmxDataChanged = delegate { };
        public void ForceBufferUpdate()
        {
            OnBufferUpdate(dmxBuffer.buffer);
        }

        private void OnBufferUpdate(byte[] buffer)
        {
            var universeCount = (short)(dmxBuffer.buffer.Length / 512);

            for (short i = 0; i < universeCount; i++)
            {
                var universeBuffer = BufferUtility.TakeUniverseFromGlobalBuffer(i, dmxBuffer.buffer);
                OnDmxDataChanged?.Invoke(i, universeBuffer, dmxBuffer.buffer);
                SendDmxData(i);
            }
        }
        #endregion
        
        private void OnDestroy()
        {
            socket.Close();
        }
        
        private void OnNewPacketReceived(object sender, NewPacketEventArgs<ArtNetPacket> e)
        {
            try
            {
                if (e.Packet.OpCode != ArtNetOpCodes.Dmx) return;
                
                var packet = e.Packet as ArtNetDmxPacket;
                if (packet == null) throw new NullReferenceException();
                
                if (dmxDataMap.ContainsKey(packet.Universe) && dmxDataMap[packet.Universe] == packet.DmxData) 
                    return;
                if (dmxDataMap.Count < packet.Universe)
                {
                    for (int i = dmxDataMap.Count; i <= packet.Universe; i++)
                        dmxDataMap.Add(i, i == packet.Universe ? packet.DmxData : new byte[512]);
                }
                else
                {
                    dmxDataMap[packet.Universe] = packet.DmxData;
                }

                dmxDataMap[packet.Universe] = packet.DmxData;
            
                // Server only?
                BufferUtility.WriteDmxToGlobalBuffer(ref dmxBuffer.buffer, ref packet, (universe, data) =>
                {
                    OnDmxDataChanged?.Invoke(universe, data, dmxBuffer.buffer);
                    SendDmxData(universe);
                });
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        /// <summary>
        /// Send universe buffer if we are redirecting packets.
        /// </summary>
        /// <param name="universe"></param>
        private void SendDmxData(short universe)
        {
            if (!redirectPackets) return;
            // TODO: look into comparing redirectTo component in a different way (optimization)
            if (redirectTo == null) return;
            BufferUtility.SendUniverseFromGlobalBuffer(redirectTo, universe, dmxBuffer.buffer);
        }
        
        public void Send(short universe, byte[] dmxData)
        {
            if (dmxData == null) throw new Exception("Dmx data is null");
            if (dmxData.Length != 512) throw new Exception("Dmx data length is not 512");
            if (dmxToSend == null) throw new Exception("Dmx to send is null");
            if (dmxToSend.DmxData == null) throw new Exception("Dmx data to send is null");
            if (socket == null) return; // ignore that
            
            dmxToSend.Universe = universe;
            
            Buffer.BlockCopy(dmxData, 0, dmxToSend.DmxData, 0, dmxData.Length);

            if (useBroadcast && isServer)
            {
                socket.Send(dmxToSend);
            }
            else if (remote != null)
            {
                socket.Send(dmxToSend, remote);
            }
            else
            {
                // Fail?
            }
        } 
    }
}

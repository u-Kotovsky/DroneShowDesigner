using ArtNet.IO;
using ArtNet.Packets;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace ArtNet.Sockets
{
    public class ArtNetSocket : Socket
    {
        public ArtNetSocket(int port = 6454) : base(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
        {
            Port = port;
        }
        
        public event UnhandledExceptionEventHandler UnhandledException;
        public event EventHandler<NewPacketEventArgs<ArtNetPacket>> NewPacket = delegate { };
        
        public int Port;
        
        #region Information
        public IPAddress LocalIP { get; protected set; }

        public IPAddress LocalSubnetMask { get; protected set; }
        private bool portOpen = false;

        public bool PortOpen
        {
            get => portOpen;
            set { portOpen = value; }
        }
        
        private static IPAddress GetBroadcastAddress(IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAddressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAddressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAddressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
                broadcastAddress[i] = (byte)(ipAddressBytes[i] | (subnetMaskBytes[i] ^ 255));
            
            return new IPAddress(broadcastAddress);
        }

        public IPAddress BroadcastAddress
        {
            get
            {
                if (LocalSubnetMask == null || LocalIP == null)
                    return IPAddress.Broadcast;
                return GetBroadcastAddress(LocalIP, LocalSubnetMask);
            }
        }

        #region Last Packet
        private DateTime? lastPacket = null;

        public DateTime? LastPacket
        {
            get { return lastPacket; }
            protected set { lastPacket = value; }
        }
        #endregion
        #endregion
        
        public void Open(IPAddress localIp, int remotePort, IPAddress localSubnetMask)
        {
            LocalIP = localIp;
            LocalSubnetMask = localSubnetMask;
            Port = remotePort;

            SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            Bind(new IPEndPoint(LocalIP, Port));
            SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            PortOpen = true;

            StartReceive();
        }

        public void StartReceive() // This could have an issue with something?
        {
            try
            {
                EndPoint localPort = new IPEndPoint(IPAddress.Any, Port);
                var data = new ArtNetRecieveData();
                
                BeginReceiveFrom(data.buffer, 0, data.bufferSize, SocketFlags.None, ref localPort, OnReceive, data); // new AsyncCallback
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                OnUnhandledException(new ApplicationException("An error occurred while trying to start receiving ArtNet.", e));
            }
        }

        private void OnReceive(IAsyncResult state)
        {
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            if (!PortOpen) return;
            
            try
            {
                var data = (ArtNetRecieveData)state.AsyncState;

                if (data != null)
                {
                    data.DataLength = EndReceiveFrom(state, ref remoteEndPoint);

                    // Protect against UDP loopback where we receive our own packets.
                    if (LocalEndPoint != remoteEndPoint && data.Valid)
                    {
                        LastPacket = DateTime.Now;

                        ProcessPacket((IPEndPoint)remoteEndPoint, ArtNetPacket.Create(data, (short)Port));
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                OnUnhandledException(e);
            }
            finally
            {
                // Attempt to receive another packet.
                StartReceive();
            }
        }

        private void ProcessPacket(IPEndPoint source, ArtNetPacket packet)
        {
            if (packet == null)
            {
                Debug.LogError("Packet is null");
                return;
            }
            
            if (NewPacket == null)
            {
                Debug.LogError("NewPacket event is null");
                return;
            }
            
            NewPacket(this, new NewPacketEventArgs<ArtNetPacket>(source, packet));
        }

        protected void OnUnhandledException(Exception ex)
        {
            if (UnhandledException != null) UnhandledException(this, new UnhandledExceptionEventArgs((object)ex, false));
        }

        #region Sending
        public void Send(ArtNetPacket packet)
        {
            Send(packet, new IPEndPoint(BroadcastAddress, Port));
        }

        public void Send(ArtNetPacket packet, IPEndPoint remote)
        {
            SendTo(packet.ToArray(), remote);
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            PortOpen = false;

            base.Dispose(disposing);
        }
    }
}

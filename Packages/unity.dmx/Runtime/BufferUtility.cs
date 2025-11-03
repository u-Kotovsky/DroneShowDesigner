using System;
using System.Linq;
using ArtNet.Packets;
using Unity_DMX.Core;

namespace Unity_DMX
{
    public abstract class BufferUtility
    {
        /// <summary>
        /// This will send whole global buffer to the target.
        /// </summary>
        /// <param name="dmxController"></param>
        /// <param name="buffer"></param>
        public static void SendGlobalBuffer(DmxController target, byte[] buffer)
        {
            byte[] dmxBuffer = new byte[512];
            
            for (var i = 0; i < buffer.Length / 512; i++)
            {
                Buffer.BlockCopy(buffer, i * 512, dmxBuffer, 0, 512);
                target.Send((short)i, dmxBuffer);
            }
        }
        
        /// <summary>
        /// This will send a whole universe to a target from global buffer.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="universe"></param>
        /// <param name="buffer"></param>
        public static void SendUniverseFromGlobalBuffer(DmxController target, short universe, byte[] buffer)
        {
            byte[] dmxData = TakeUniverseFromGlobalBuffer(universe, buffer);
            
            target.Send(universe, dmxData);
        }

        /// <summary>
        /// This will ensure buffer has this length initialized.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        public static void EnsureCapacity(ref byte[] buffer, int length) // could cause an issue with all dmx channels = 0
        {
            if (buffer.Length >= length) return;
            
            byte[] temp = new byte[length];
            
            Buffer.BlockCopy(buffer, 0, temp, 0, buffer.Length);

            buffer = temp;
        }

        /// <summary>
        /// Is all channels are zero in buffer?
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static bool IsAllZero(byte[] buffer) => buffer.All(channel => channel == 0);
        
        /// <summary>
        /// This will write a ArtNetDmxPacket into global buffer and then invokes a callback with changed universe.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="packet"></param>
        /// <param name="callback"></param>
        public static void WriteDmxToGlobalBuffer(ref byte[] buffer, ref ArtNetDmxPacket packet, Action<short, byte[]> callback)
        {
            //EnsureCapacity(ref buffer, (packet.Universe + 1) * 512);
            
            int offset = packet.Universe * 512;

            Buffer.BlockCopy(packet.DmxData, 0, buffer, offset, packet.DmxData.Length);
            
            callback?.Invoke(packet.Universe, TakeUniverseFromGlobalBuffer(packet.Universe, buffer));
        }

        /// <summary>
        /// This will take a whole universe from global buffer.
        /// </summary>
        /// <param name="universe"></param>
        /// <param name="globalDmxBuffer"></param>
        /// <returns></returns>
        public static byte[] TakeUniverseFromGlobalBuffer(short universe, byte[] globalDmxBuffer)
        {
            int offset = universe * 512;
            byte[] dmxBuffer = new byte[512];
            
            Buffer.BlockCopy(globalDmxBuffer, offset, dmxBuffer, 0, dmxBuffer.Length);
            
            return dmxBuffer;
        }
    }
}

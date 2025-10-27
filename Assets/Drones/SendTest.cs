using UnityEngine;

namespace Drones
{
    public class SendTest : MonoBehaviour
    {
        public DmxController dmxController;
        public short universe;
        public byte[] dmxData;
    
        void Start()
        {
            dmxData = new byte[511];

            for (var i = 0; i < dmxData.Length; i++)
            {
                dmxData[i] = 0;//(byte)Random.Range(0, 255);
            }
        
            dmxController.Send(universe, dmxData);
        }
    }
}

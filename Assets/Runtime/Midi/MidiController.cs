using Melanchall.DryWetMidi.Multimedia;
using UnityEngine;

namespace Runtime.Midi
{
    public class MidiController : MonoBehaviour
    {
        private MidiSender midiSender;
        private MidiReceiver midiReceiver;
        
        private void Awake()
        {
            string inputDeviceName = "";
            foreach (var inputDevice in InputDevice.GetAll())
            {
                Debug.Log("MIDI Input Device: " + inputDevice.Name);
                inputDeviceName = inputDevice.Name;
            }
            
            string outputDeviceName = "";
            foreach (var outputDevice in OutputDevice.GetAll())
            {
                Debug.Log("MIDI Output Device: " + outputDevice.Name);
                outputDeviceName = outputDevice.Name;
            }
            
            //midiSender = gameObject.AddComponent<MidiSender>();
            midiReceiver = gameObject.AddComponent<MidiReceiver>();
            midiReceiver.Create(inputDeviceName);
        }
    }
}
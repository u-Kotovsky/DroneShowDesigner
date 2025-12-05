using System;
using Melanchall.DryWetMidi.Multimedia;
using UnityEngine;

namespace Runtime.Midi
{
    public class MidiTimeCodeReceiver : MonoBehaviour
    {
        public MidiReceiver midiReceiver;

        public event Action<MidiTimeCodeReceivedEventArgs> OnTimeCodeReceived = (e) => { };

        public static MidiTimeCodeReceiver Instance { get; private set; }

        private void Awake()
        {
            string inputDeviceName = "";
            foreach (var inputDevice in InputDevice.GetAll())
            {
                Debug.Log("MIDI Input Device: " + inputDevice.Name);
                inputDeviceName = inputDevice.Name;
            }
            
            Create(inputDeviceName);
        }

        public void Create(string deviceName)
        {
            if (Instance != null)
            {
                Debug.LogError($"{nameof(MidiTimeCodeReceiver)} already exists.");
                enabled = false;
                return;
            }
            
            Instance = this;

            try
            {
                midiReceiver.Create(deviceName);
                midiReceiver.OnMidiTimeCodeReceived += MidiReceiverOnOnMidiEventReceived;
            }
            catch (Exception)
            {
                enabled = false;
                throw;
            }
            
        }

        private void MidiReceiverOnOnMidiEventReceived(MidiDevice arg1, MidiTimeCodeReceivedEventArgs arg2)
        {
            try
            {
                OnTimeCodeReceived?.Invoke(arg2);
            }
            catch (Exception)
            {
                enabled = false;
                throw;
            }
        }
    }
}

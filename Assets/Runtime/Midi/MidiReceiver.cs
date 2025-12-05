using System;
using Melanchall.DryWetMidi.Multimedia;
using UnityEngine;

namespace Runtime.Midi
{
    public class MidiReceiver : MonoBehaviour
    {
        private const string Prefix = "MidiReceiver";
        
        private InputDevice midiInputDevice;

        public event Action<MidiDevice, MidiEventReceivedEventArgs> OnMidiEventReceived = (device, e) => { };
        public event Action<MidiDevice, MidiTimeCodeReceivedEventArgs> OnMidiTimeCodeReceived = (device, e) => { };

        public void Create(string deviceName)
        {
            try
            {
                Debug.Log($"'{Prefix}' Create receiver for MIDI device '{deviceName}'");
                midiInputDevice = InputDevice.GetByName(deviceName);
                midiInputDevice.EventReceived += OnEventReceived;
                midiInputDevice.MidiTimeCodeReceived += MidiInputDeviceOnMidiTimeCodeReceived;
                midiInputDevice.StartEventsListening();
            }
            catch (Exception)
            {
                enabled = false;
                throw;
            }
        }

        private void OnDestroy()
        {
            if (midiInputDevice == null) return;
            
            midiInputDevice.EventReceived -= OnEventReceived;
            midiInputDevice.StopEventsListening();
            midiInputDevice.Dispose();
        }

        private void MidiInputDeviceOnMidiTimeCodeReceived(object sender, MidiTimeCodeReceivedEventArgs e)
        {
            try
            {
                var midiDevice = (MidiDevice)sender;
                
                OnMidiTimeCodeReceived?.Invoke(midiDevice, e);
            }
            catch (Exception)
            {
                enabled = false;
                throw;
            }
        }

        private void OnEventReceived(object sender, MidiEventReceivedEventArgs ev)
        {
            try
            {
                var midiDevice = (MidiDevice)sender;
                //Debug.Log($"'{Prefix}' Event received from '{midiDevice.Name}' at {DateTime.Now}: {ev.Event}");
                
                OnMidiEventReceived?.Invoke(midiDevice, ev);
            }
            catch (Exception)
            {
                enabled = false;
                throw;
            }
        }
    }
}

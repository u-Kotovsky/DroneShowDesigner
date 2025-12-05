using System;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using UnityEngine;

namespace Runtime.Midi
{
    public class MidiSender : MonoBehaviour
    {
        private const string Prefix = "MidiSender";
        
        private OutputDevice midiOutputDevice;
    
        public void Create(string deviceName)
        {
            try
            {
                Debug.Log($"'{Prefix}' Create sender for MIDI device '{deviceName}'");
                midiOutputDevice = OutputDevice.GetByName(deviceName);
                midiOutputDevice.EventSent += OnEventSent;

                midiOutputDevice.SendEvent(new NoteOnEvent());
                midiOutputDevice.SendEvent(new NoteOffEvent());
            }
            catch (Exception)
            {
                enabled = false;
                throw;
            }
        }

        private void OnEventSent(object sender, MidiEventSentEventArgs e)
        {
            try
            {
                var midiDevice = (MidiDevice)sender;
                Debug.Log($"'{Prefix}' Event sent to '{midiDevice.Name}' at {DateTime.Now}: {e.Event}");
            }
            catch (Exception)
            {
                enabled = false;
                throw;
            }
        }
    }
}

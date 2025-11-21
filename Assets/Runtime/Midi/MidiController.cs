using System;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using UnityEngine;

namespace Runtime.Midi
{
    public class MidiController : MonoBehaviour
    {
        
        /*private InputDevice midiInputDevice;
        private OutputDevice midiOutputDevice;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            // Input
            string lastMidiInputDevice = "";
            foreach (var inputDevice in InputDevice.GetAll())
            {
                Debug.Log("MIDI InputDevice: " + inputDevice.Name);
                lastMidiInputDevice = inputDevice.Name;
            }
            
            midiInputDevice = InputDevice.GetByName(lastMidiInputDevice);
            midiInputDevice.EventReceived += OnEventReceived;
            midiInputDevice.StartEventsListening();
            
            // Output
            string lastMidiOutputDevice = "";
            foreach (var outputDevice in OutputDevice.GetAll())
            {
                Debug.Log("MIDI OutputDevice: " + outputDevice.Name);
                lastMidiOutputDevice = outputDevice.Name;
            }
              
            midiOutputDevice = OutputDevice.GetByName(lastMidiOutputDevice);
            midiOutputDevice.EventSent += OnEventSent;

            midiOutputDevice.SendEvent(new NoteOnEvent());
            midiOutputDevice.SendEvent(new NoteOffEvent());
        }

        private void OnEventSent(object sender, MidiEventSentEventArgs e)
        {
            var midiDevice = (MidiDevice)sender;
            Console.WriteLine($"Event sent to '{midiDevice.Name}' at {DateTime.Now}: {e.Event}");
        }
            
        private static void OnEventReceived(object sender, MidiEventReceivedEventArgs e)
        {
            var midiDevice = (MidiDevice)sender;
            //Debug.Log($"Event received from '{midiDevice.Name}' at {DateTime.Now}: {e.Event}");

            if (e.Event.EventType == MidiEventType.MidiTimeCode)
            {
                //Debug.Log("MIDI TimeCode: " + e.Event.DeltaTime + " from device " + midiDevice.Name);
            }
        }*/
    }
}

using ComposerTools.Classes.MIDI;
using Melanchall.DryWetMidi.Smf;
using NHotkey;
using NHotkey.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ComposerTools.Classes.SystemInteraction
{
    class HotKeyMapper
    {
        public void initializeHotkeys()
        {
            HotkeyManager.Current.AddOrReplace("AltP", Key.P, ModifierKeys.Alt, OnAltP);
        }

        private void OnAltP(object sender, HotkeyEventArgs e)
        {
            MidiFile midiRaw = Midi_Communicator.GetMidiFromFL();
            MidiFile processedMidi = new Midi_Processor(midiRaw).removeNote();
            Thread.Sleep(1000); //Delay because otherwise context menu may still be open
            Midi_Communicator.SendMidiToFL(processedMidi);
        }
    }
}

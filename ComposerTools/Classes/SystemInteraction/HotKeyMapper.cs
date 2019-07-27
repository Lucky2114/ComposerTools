using ComposerTools.Classes.MIDI;
using Melanchall.DryWetMidi.Smf;
using NHotkey;
using NHotkey.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
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

            //FileStream fs = new FileStream(@"C:\Users\Kevin\Desktop\project.flp", FileMode.Open);

            //Project project = Project.Load(fs, false);

            //foreach (var item in project.Patterns)
            //{
            //    foreach (var note in item.Notes.Values)
            //    {
            //        foreach (var item2 in note)
            //        {
            //            item2.Key = 100;
            //        }
            //    }
            //}

            //fs.Dispose();


            MidiFile midiRaw = Midi_Communicator.GetMidiFromFL();
            MidiFile processedMidi = new Midi_Processor(midiRaw).removeNote();
            Thread.Sleep(1500); //Delay because otherwise context menu may still be open
            Midi_Communicator.SendMidiToFL(processedMidi);
        }
    }
}

using ComposerTools.Classes.FLStudio;
using Melanchall.DryWetMidi.Smf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ComposerTools.Classes.MIDI
{
    class Midi_Communicator
    {
        public static MidiFile GetMidiFromFL()
        {
            System.Windows.Clipboard.Clear();
            System.Windows.Forms.Clipboard.Clear();
            FLStudio_Communicator.getInstance().getClipboardFromFL();
            return Midi_Exchanger.GetMidiFromClipboard();
        }

        public static void SendMidiToFL(MidiFile midi)
        {
            Midi_Exchanger.SetMidiToClipboard(midi);
            FLStudio_Communicator.getInstance().SendClipboardToFLStudio();
        }
    }
}

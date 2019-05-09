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
        public static MidiFile getMidiFromFL()
        {
            FLStudio_Communicator.getInstance().getClipboardFromFL();
            return Midi_Exchanger.getMidiFromClipboard();
        }

        public static void setMidiToFL(MidiFile midi)
        {
            Midi_Exchanger.setMidiToClipboard(midi);
            FLStudio_Communicator.getInstance().sendClipboardToFLStudio();
        }
    }
}

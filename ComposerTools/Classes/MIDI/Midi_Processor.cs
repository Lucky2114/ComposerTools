using Melanchall.DryWetMidi.MusicTheory;
using Melanchall.DryWetMidi.Smf;
using Melanchall.DryWetMidi.Smf.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComposerTools.Classes.MIDI
{
    class Midi_Processor
    {
        MidiFile midiToProcess = null;
        public Midi_Processor(MidiFile midi)
        {
            midiToProcess = midi;
        }

        public MidiFile removeNote()
        {
            midiToProcess.RemoveNotes(n => n.NoteName == NoteName.FSharp);
            return midiToProcess;
        }
    }
}

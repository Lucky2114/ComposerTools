using Melanchall.DryWetMidi.Smf;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace ComposerTools.Classes.MIDI
{
    class Midi_Exchanger
    {
        private static readonly string midiFormat = "Standard MIDI File";
        public static MidiFile GetMidiFromClipboard()
        {  
            if (midiFormat.Equals(Clipboard.GetDataObject().GetFormats().FirstOrDefault()))
            {
                MemoryStream ms = (MemoryStream)Clipboard.GetDataObject().GetData(midiFormat);
                return MidiFile.Read(ms); 
            }
            else
            {
                return null;
            }
        }

        public static void SetMidiToClipboard(MidiFile midi)
        {
            byte[] result;
            using (MemoryStream ms2 = new MemoryStream())
            {
                midi.Write(ms2);
                result = ms2.ToArray();
            } //Stream gets disposed after write by drywetmidi -> user toArray();

            MemoryStream ms3 = new MemoryStream(result);
            Clipboard.Clear();
            Clipboard.SetData(midiFormat, ms3);
        }
    }
}

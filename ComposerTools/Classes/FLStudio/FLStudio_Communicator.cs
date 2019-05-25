using System;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using TestStack.White.Factory;
using TestStack.White.UIItems;

namespace ComposerTools.Classes.FLStudio
{
    class FLStudio_Communicator
    {


        private static FLStudio_Communicator instance;

        TestStack.White.UIItems.WindowItems.Window window = null;
        IntPtr windowHandle;
        TestStack.White.UIItems.Panel itemToClick = null;

        public static FLStudio_Communicator getInstance()
        {
            if (instance == null)
            {
                instance = new FLStudio_Communicator();
                return instance;
            }
            else
            {
                return instance;
            }
        }

        //These Methods are void because the midi gets copied to the clipboard
        public void getClipboardFromFL()
        {
            FL_Interaction.openPianoRollContextMenu(windowHandle, window);
            FL_Interaction.openPianoRollContextMenuFile(windowHandle, window);
            SendKeys.SendWait("c"); //->Copy Midi To Clipboard
            Thread.Sleep(100); //TODO: This delay?? (It's needed because otherwise the midi is not copied to the clipboard)
            SendKeys.Flush();
        }

        public void sendClipboardToFLStudio()
        {
            TestStack.White.UIItemEvents.UIItemClickEvent t = new TestStack.White.UIItemEvents.UIItemClickEvent(itemToClick);
            FL_Interaction.openPianoRollContextMenu(windowHandle, window);
            SendKeys.SendWait("f"); //->File
            SendKeys.SendWait("p"); //->Paste Midi From Clipboard
            SendKeys.Flush();

        }

        public void openFlStudio()
        {
            TestStack.White.Application application = TestStack.White.Application.Launch(@"C:\Program Files (x86)\Image-Line\FL Studio 20\FL.exe");
            this.window = application.GetWindow("FL Studio 20", InitializeOption.NoCache);
            this.windowHandle = application.Process.Handle;
            Logger.Log($"Recived window Handle: {windowHandle}");

            UIItemCollection coll = window.Items;
        }


    }
}

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

        private const string flStudioPath = @"C:\Program Files (x86)\Image-Line\FL Studio 20\FL64.exe";

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
            Thread.Sleep(400); //This delay is needed because of the animation (fade in) of the context menu
            SendKeys.SendWait("c"); //->Copy Midi To Clipboard
            
            SendKeys.Flush();
        }

        public void SendClipboardToFLStudio()
        {
            //TestStack.White.UIItemEvents.UIItemClickEvent t = new TestStack.White.UIItemEvents.UIItemClickEvent(itemToClick);
            //FL_Interaction.openPianoRollContextMenu(windowHandle, window);
            //SendKeys.SendWait("f"); //->File
            //SendKeys.SendWait("p"); //->Paste Midi From Clipboard
            //SendKeys.Flush();

            //TODO: not really safe: fl studio may not be focused
            SendKeys.SendWait("^+{v}");

        }

        public void openFlStudio()
        {
            TestStack.White.Application application = TestStack.White.Application.Launch(flStudioPath);
            this.window = application.GetWindow("FL Studio 20", InitializeOption.NoCache);
            this.windowHandle = application.Process.Handle;
            Logger.Log($"Recived window Handle: {windowHandle}");

            UIItemCollection coll = window.Items;
        }


    }
}

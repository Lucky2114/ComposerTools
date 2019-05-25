using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using TestStack.White.Factory;
using TestStack.White.UIItems;

namespace ComposerTools.Classes.FLStudio
{
    class FLStudio_Communicator
    {
        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        private const string flstudioExePath = @"D:\Programs\FL Studio 20\FL64.exe";

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
            System.Windows.Clipboard.Clear();
            System.Windows.Forms.Clipboard.Clear();

            UIItemCollection coll = window.Items;
            foreach (UIItem item in coll)
            {
                if (item.Name.Contains("Piano roll"))
                {
                    if (((TestStack.White.UIItems.Panel)item).Items.Count == 0)
                    {
                        itemToClick = (TestStack.White.UIItems.Panel)item;
                        break;
                    }
                }
            }

            SetForegroundWindow(windowHandle);
            window.Focus();
            var mouse = window.Mouse;
            Point p = itemToClick.Location;
            p.Offset(11, 12);

            mouse.Click(p);
            SendKeys.SendWait("f");
            SendKeys.SendWait("c");
            Thread.Sleep(100); //TODO: This delay?? (It's needed because otherwise the midi is not copied to the clipboard)
            SendKeys.Flush();
        }

        public void sendClipboardToFLStudio()
        {
            bool focus = SetForegroundWindow(windowHandle);
            if (focus)
            {
                var mouse = window.Mouse;
                Point p = itemToClick.Location;
                p.Offset(11, 12);
                mouse.Click(p);
                Thread.Sleep(500);
                SendKeys.SendWait("f");
                SendKeys.SendWait("p");
                SendKeys.Flush();
            } else
            {
                Logger.Log("Unable to get Focus when sending clipboard back to fl");
            }
        }

        public void openFlStudio()
        {
            TestStack.White.Application application = TestStack.White.Application.Launch(flstudioExePath);
            this.window = application.GetWindow("FL Studio 20");
            this.windowHandle = application.Process.Handle;
            Logger.Log($"Recived window Handle: {windowHandle}");

            UIItemCollection coll = window.Items;
        }


    }
}

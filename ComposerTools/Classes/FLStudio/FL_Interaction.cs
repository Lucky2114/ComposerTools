using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Forms;
using TestStack.White.UIItems;

namespace ComposerTools.Classes.FLStudio
{
    class FL_Interaction
    {
        //TODO: Find faster way to iterate through window controls

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        //If it's allready open -> returns true. If it's closed it tries opens it.
        public static void openPianoRollContextMenu(IntPtr windowHandle, TestStack.White.UIItems.WindowItems.Window window)
        {
            TestStack.White.UIItems.Panel itemToClick = null;

            //TODO: use isPianoRollContextMenuOpen?
            //if (!isPianoRollContextMenuOpen(windowHandle, window))
            //{
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
            //TODO: Focus alway returns false
            bool focus = SetForegroundWindow(windowHandle);
            if (focus)
            {
                var mouse = window.Mouse;
                Point p = itemToClick.Location;
                p.Offset(11, 12);
                mouse.Click(p);
            }
            else
            {
                //Do it as well, but log it
                Debug.Print("Failed to focus window");
                var mouse = window.Mouse;
                Point p = itemToClick.Location;
                p.Offset(11, 12);
                mouse.Click(p);
            }
            //}
        }

        public static bool openPianoRollContextMenuFile(IntPtr windowHandle, TestStack.White.UIItems.WindowItems.Window window)
        {
            bool res = false;
            //TODO: FIX THIS
            //  if (isPianoRollContextSubMenuOpen(windowHandle, window))
            // {
            SendKeys.SendWait("f"); //->File
            res = true;
            // }
            return res;
        }

        public static void pianoRollCopyMidiToClipBoard()
        {

        }

        public static bool isPianoRollContextMenuOpen(IntPtr windowHandle, TestStack.White.UIItems.WindowItems.Window window)
        {
            bool focus = SetForegroundWindow(windowHandle);
            foreach (var item in window.Items)
            {
                try
                {
                    AutomationElement automationElement = item.AutomationElement;
                    if (automationElement.Current.ClassName.Equals("TQuickPopupMenuWindow"))
                    {
                        return true;
                    }
                }
                catch
                {
                    return true;
                }
            }
            return false;
        }

        public static bool isPianoRollContextSubMenuOpen(IntPtr windowHandle, TestStack.White.UIItems.WindowItems.Window window)
        {
            bool focus = SetForegroundWindow(windowHandle);
            int counter = 0;
            foreach (var item in window.Items)
            {
                AutomationElement automationElement = item.AutomationElement;
                if (automationElement.Current.ClassName.Equals("TQuickPopupMenuWindow"))
                {
                    counter++;
                    if (counter == 2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

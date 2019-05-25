using ComposerTools.Classes.FLStudio;
using ComposerTools.Classes.SystemInteraction;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using TestStack.White.Factory;
using TestStack.White.UIItems;

namespace ComposerTools.View
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        


        
        public MainWindow()
        {
            InitializeComponent();

            HotKeyMapper hotkeymapper = new HotKeyMapper();
            hotkeymapper.initializeHotkeys();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FLStudio_Communicator.getInstance().openFlStudio();
        }

    }
}


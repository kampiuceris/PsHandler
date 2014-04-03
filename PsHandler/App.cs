using System;
using System.Threading;
using System.Windows;

namespace PsHandler
{
    public class App : Application
    {
        private static WindowMain Gui;

        public App()
        {
            Gui = new WindowMain();
            Gui.Show();
            Handler.Start();
        }

        public static void Quit()
        {
            Handler.Stop();
            Gui.IsClosing = true;
            new Thread(() => Gui.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => Gui.Close()))).Start();
            //Current.Shutdown();
        }
    }
}

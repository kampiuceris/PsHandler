using System;
using System.Threading;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using PsHandler.Hud;
using PsHandler.UI;

namespace PsHandler
{
    public class App : Application
    {
        public static WindowMain WindowMain;
        public static TaskbarIcon TaskbarIcon { get { return WindowMain.TaskbarIcon_NotifyIcon; } }
        public static KeyboardHook KeyboardHook;
        public static LobbyTime LobbyTime;

        public App()
        {
            Config.Load();
            LobbyTime = new LobbyTime();
            //RegisterKeyboardHook();
            WindowMain = new WindowMain();
            WindowMain.Show();
            Handler.Start();
            ReleaseOnly();
        }

        public static void RegisterKeyboardHook()
        {
            KeyboardHook = new KeyboardHook();
            KeyboardHook.KeyCombinationDownMethods.Add(kc =>
            {
                if (kc.Equals(Config.HotkeyHandReplay))
                {
                    Handler.ClickReplayHandButton();
                }
                if (kc.Equals(Config.HotkeyExit))
                {
                    Quit();
                }
            });
        }

        public static void Quit()
        {
            Autoupdate.Quit();
            HudManager.Stop();
            //KeyboardHook.Dispose();
            LobbyTime.StopSync();
            Handler.Stop();
            Config.Save();

            //close gui
            WindowMain.IsClosing = true;
            new Thread(() => Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => WindowMain.Close()))).Start();
        }

        private static void ReleaseOnly()
        {
#if DEBUG
#else
            Autoupdate.CheckForUpdates(Config.UPDATE_HREF + "?v=" + Config.VERSION + "&id=" + (string.IsNullOrEmpty(Config.MACHINE_GUID) ? "" : Config.MACHINE_GUID), Config.UPDATE_HREF, "PsHandler", "PsHandler.exe", AppDomain.CurrentDomain.BaseDirectory, WindowMain, Quit);
#endif
        }
    }
}

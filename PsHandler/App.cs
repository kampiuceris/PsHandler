using System;
using System.Threading;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using PsHandler.Hud;
using PsHandler.TableTiler;
using PsHandler.UI;
using PsHandler.Hud.Import;

namespace PsHandler
{
    public class App : Application
    {
        public static WindowMain WindowMain;
        public static TaskbarIcon TaskbarIcon { get { return WindowMain.TaskbarIcon_NotifyIcon; } }
        public static KeyboardHook KeyboardHook;
        public static Import Import;

        public App()
        {
            Config.LoadXml();
            RegisterKeyboardHook();
            WindowMain = new WindowMain();
            WindowMain.Show();
            Import = new Import();
            Handler.Start();
            TableTileManager.Start();
            ReleaseOnly();
        }

        public static void Quit()
        {
            Autoupdate.Quit();
            TableTileManager.Stop();
            HudManager.Stop();
            Handler.Stop();
            Import.Stop();
            KeyboardHook.Dispose();
            Config.SaveXml();

            //close gui
            WindowMain.IsClosing = true;
            new Thread(() => Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => WindowMain.Close()))).Start();
            //Current.Shutdown();
        }

        private static void RegisterKeyboardHook()
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
                TableTileManager.SetKeyCombination(kc);
            });
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

using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using Hardcodet.Wpf.TaskbarNotification;
using PsHandler.Custom;
using PsHandler.Hook;
using PsHandler.Hook.WinApi;
using PsHandler.Hud;
using PsHandler.Import;
using PsHandler.PokerTypes;
using PsHandler.Randomizer;
using PsHandler.TableTiler;
using PsHandler.UI;
using Application = System.Windows.Application;

namespace PsHandler
{
    public class App : Application
    {
        public static WindowMain WindowMain;
        public static TaskbarIcon TaskbarIcon { get { return WindowMain.TaskbarIcon_NotifyIcon; } }
        public static KeyboardHookListener KeyboardHook;
        public static MouseHookListener MouseHook;
        public static HandHistoryManager HandHistoryManager;
        public static TableManager TableManager;

        public App()
        {
            RegisterHook();
            Config.LoadXml();
            HandHistoryManager = new HandHistoryManager();
            TableManager = new TableManager();
            TableTileManager.Start();
            Handler.Start();

            WindowMain = new WindowMain();
            WindowMain.Show();

            TableManager.Start();
            HandHistoryManager.Observer = WindowMain.UcStatusBar;
            TableManager.ObserverTableManagerTableList = WindowMain.UCTables;
            TableManager.ObserverTableManagerTableCount = WindowMain.UcStatusBar;

            ReleaseOnly();
        }

        public static void Quit()
        {
            Autoupdate.Quit();

            Handler.Stop();
            TableTileManager.Stop();
            HandHistoryManager.Stop();

            Methods.UiInvoke(() =>
            {
                Config.GuiLocationX = (int)WindowMain.Left;
                Config.GuiLocationY = (int)WindowMain.Top;
                Config.GuiWidth = (int)WindowMain.Width;
                Config.GuiHeight = (int)WindowMain.Height;
            });

            Config.SaveXml();
            if (KeyboardHook != null) KeyboardHook.Enabled = false;
            if (KeyboardHook != null) MouseHook.Enabled = false;

            Config.EnableCustomTablesWindowStyle = false;
            TableManager.EnsureTablesStyle();
            TableManager.Stop();

            //close gui
            WindowMain.IsClosing = true;
            new Thread(() => Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => WindowMain.Close()))).Start();
        }

        private static void RegisterHook()
        {
            KeyboardHook = new KeyboardHookListener(new GlobalHooker()) { Enabled = true };
            MouseHook = new MouseHookListener(new GlobalHooker()) { Enabled = true };

            KeyboardHook.KeyDownMethods.Add(kc =>
            {
                if (kc.Equals(Config.HotkeyHandReplay))
                {
                    Handler.ClickReplayHandButton();
                }
                if (kc.Equals(Config.HotkeyQuickPreview))
                {
                    Handler.QuickPreviewStart();
                }
                if (kc.Equals(Config.HotkeyExit))
                {
                    Quit();
                }
                TableTileManager.SetKeyCombination(kc);
                RandomizerManager.CheckKeyCombination(kc);
            });

            KeyboardHook.KeyUpMethods.Add(kc =>
            {
                if (kc.Equals(Config.HotkeyQuickPreview))
                {
                    Handler.QuickPreviewStop();
                }
            });
        }

        private static void ReleaseOnly()
        {
#if DEBUG
            return;
            Autoupdate.CheckForUpdates(Config.UPDATE_HREF + "?v=" + Config.VERSION + "&id=" + (string.IsNullOrEmpty(Config.MACHINE_GUID) ? "" : Config.MACHINE_GUID),
                Config.UPDATE_HREF, "PsHandler", "PsHandler.exe", AppDomain.CurrentDomain.BaseDirectory, WindowMain, Quit,
                Methods.GetEmbeddedResourceBitmap("PsHandler.Images.EmbeddedResources.Size16x16.update.png").ToBitmapSource(),
                Methods.GetEmbeddedResourceBitmap("PsHandler.Images.EmbeddedResources.Size16x16.cancel.png").ToBitmapSource(),
                Methods.GetEmbeddedResourceBitmap("PsHandler.Images.EmbeddedResources.Size16x16.update.png").ToBitmapSource());
#else
            Autoupdate.CheckForUpdates(Config.UPDATE_HREF + "?v=" + Config.VERSION + "&id=" + (string.IsNullOrEmpty(Config.MACHINE_GUID) ? "" : Config.MACHINE_GUID),
                Config.UPDATE_HREF, "PsHandler", "PsHandler.exe", AppDomain.CurrentDomain.BaseDirectory, WindowMain, Quit,
                Methods.GetEmbeddedResourceBitmap("PsHandler.Images.EmbeddedResources.Size16x16.update.png").ToBitmapSource(),
                Methods.GetEmbeddedResourceBitmap("PsHandler.Images.EmbeddedResources.Size16x16.cancel.png").ToBitmapSource(),
                Methods.GetEmbeddedResourceBitmap("PsHandler.Images.EmbeddedResources.Size16x16.update.png").ToBitmapSource());
#endif
        }
    }
}

using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using Hardcodet.Wpf.TaskbarNotification;
using PsHandler.Custom;
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
        public static KeyboardHook KeyboardHook;
        public static HandHistoryManager HandHistoryManager;
        public static PokerTypeManager PokerTypeManager;
        public static TableTileManager TableTileManager;
        public static Handler Handler;
        public static RandomizerManager RandomizerManager;
        public static TableManager TableManager;

        public App()
        {
            RegisterHook();
            TableTileManager = new TableTileManager();
            PokerTypeManager = new PokerTypeManager();
            RandomizerManager = new RandomizerManager();
            HandHistoryManager = new HandHistoryManager();
            Handler = new Handler();
            TableManager = new TableManager();

            Config.LoadXml();

            WindowMain = new WindowMain();
            WindowMain.Show();


            HandHistoryManager.Observer = WindowMain.UcStatusBar;
            TableManager.ObserverTableManagerTableList = WindowMain.UCTables;
            TableManager.ObserverTableManagerTableCount = WindowMain.UcStatusBar;

            ReleaseOnly();
        }

        public static void Quit()
        {
            Autoupdate.Quit();

            Methods.UiInvoke(() =>
            {
                Config.GuiLocationX = (int)WindowMain.Left;
                Config.GuiLocationY = (int)WindowMain.Top;
                Config.GuiWidth = (int)WindowMain.Width;
                Config.GuiHeight = (int)WindowMain.Height;
            });
            Config.SaveXml();

            Handler.Stop();
            TableTileManager.Stop();
            HandHistoryManager.Stop();
            if (KeyboardHook != null) KeyboardHook.Dispose();

            Config.EnableCustomTablesWindowStyle = false;
            TableManager.EnsureTablesStyle();
            TableManager.Stop();

            //close gui
            WindowMain.IsClosing = true;
            new Thread(() => Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => WindowMain.Close()))).Start();
        }

        private static void RegisterHook()
        {
            KeyboardHook = new KeyboardHook();

            KeyboardHook.KeyCombinationDownMethods.Add(kc =>
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

            KeyboardHook.KeyCombinationUpMethods.Add(kc =>
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
            //Autoupdate.CheckForUpdates(Config.UPDATE_HREF + "?v=" + Config.VERSION + "&id=" + (string.IsNullOrEmpty(Config.MACHINE_GUID) ? "" : Config.MACHINE_GUID),
            //    Config.UPDATE_HREF, "PsHandler", "PsHandler.exe", AppDomain.CurrentDomain.BaseDirectory, WindowMain, Quit,
            //    Methods.GetEmbeddedResourceBitmap("PsHandler.Images.EmbeddedResources.Size16x16.update.png").ToBitmapSource(),
            //    Methods.GetEmbeddedResourceBitmap("PsHandler.Images.EmbeddedResources.Size16x16.cancel.png").ToBitmapSource(),
            //    Methods.GetEmbeddedResourceBitmap("PsHandler.Images.EmbeddedResources.Size16x16.update.png").ToBitmapSource());
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

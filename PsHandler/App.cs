// PsHandler - poker software helping tool.
// Copyright (C) 2014  kampiuceris

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using PsHandler.Custom;
using PsHandler.Import;
using PsHandler.PokerMath;
using PsHandler.PokerTypes;
using PsHandler.Randomizer;
using PsHandler.Replayer.UI;
using PsHandler.TableTiler;
using PsHandler.UI;
using System.Windows;

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
        public static WindowReplayer WindowReplayer;

        public App()
        {
            WindowReplayer = new WindowReplayer();
            WindowReplayer.Show();
            WindowReplayer.UcReplayerTable_Main.ReplayHand(PokerData.FromText(File.ReadAllText(@"C:\Users\WinWork\Desktop\test.txt")).PokerHands[0]);

            return;

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

            if (!CheckGnuGplV3Agreement()) return;

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

        private static bool CheckGnuGplV3Agreement()
        {
            if (!Config.GnuGplV3Agreement)
            {
                var dialog = new WindowGNUGeneralPublicLicense(WindowMain);
                dialog.ShowDialog();
                if (dialog.Agrees)
                {
                    Config.GnuGplV3Agreement = true;
                    return true;
                }
                else
                {
                    Quit();
                    return false;
                }
            }
            return true;
        }
    }
}

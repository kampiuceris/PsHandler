using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using PsHandler.Hud;
using Image = System.Drawing.Image;

namespace PsHandler
{
    public class App : Application
    {
        public const string NAME = "PsHandler";
        public const int VERSION = 6;
        public static string MACHINE_GUID = ConfigManager.GetMachineGuid();
        public const string UPDATE_HREF = "http://chainer.projektas.in/PsHandler/update.php";
        public static WindowMain Gui;
        public static KeyboardHook KeyboardHook;
        public static LobbyTime LobbyTime;

        public static PokerStarsThemeLobby PokerStarsThemeLobby
        {
            get
            {
                PokerStarsThemeLobby value = null;
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                {
                    value = Gui.ComboBox_PokerStarsThemeLobby.SelectedItem as PokerStarsThemeLobby ?? new PokerStarsThemesLobby.Unknown();
                }));
                return value;
            }
        }

        public static PokerStarsThemeTable PokerStarsThemeTable
        {
            get
            {
                PokerStarsThemeTable value = null;
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                {
                    value = Gui.ComboBox_PokerStarsThemeTable.SelectedItem as PokerStarsThemeTable ?? new PokerStarsThemesTable.Unknown();
                }));
                return value;
            }
        }

        public static KeyCombination HotkeyHandReplay
        {
            get
            {
                KeyCombination keyCombination = new KeyCombination(Key.None, false, false, false);
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                {
                    keyCombination = Gui.TextBoxHotkey_HandReplay.KeyCombination;
                }));
                return keyCombination;
            }
        }

        public static KeyCombination HotkeyExit
        {
            get
            {
                KeyCombination keyCombination = new KeyCombination(Key.None, false, false, false);
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                {
                    keyCombination = Gui.TextBoxHotkey_Exit.KeyCombination;
                }));
                return keyCombination;
            }
        }

        public static bool AutoclickImBack
        {
            get
            {
                bool value = true;
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                {
                    value = Gui.CheckBox_AutoclickImBack.IsChecked == true;
                }));
                return value;
            }
        }

        public static bool AutoclickTimebank
        {
            get
            {
                bool value = true;
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                {
                    value = Gui.CheckBox_AutoclickTimebank.IsChecked == true;
                }));
                return value;
            }
        }

        public static bool AutocloseTournamentRegistrationPopups
        {
            get
            {
                bool value = true;
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                {
                    value = Gui.CheckBox_AutocloseTournamentRegistrationPopups.IsChecked == true;
                }));
                return value;
            }
        }

        public static bool AutocloseHM2ApplyToSimilarTablesPopups
        {
            get
            {
                bool value = true;
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                {
                    value = Gui.CheckBox_AutocloseHM2ApplyToSimilarTablesPopups.IsChecked == true;
                }));
                return value;
            }
        }

        public static bool MinimizeToSystemTray
        {
            get
            {
                bool value = false;
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                {
                    value = Gui.CheckBox_MinimizeToSystemTray.IsChecked == true;
                }));
                return value;
            }
        }

        public static bool StartMinimized
        {
            get
            {
                bool value = false;
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                {
                    value = Gui.CheckBox_StartMinimized.IsChecked == true;
                }));
                return value;
            }
        }

        public static string AppDataPath
        {
            get
            {
                string value = "";
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                {
                    value = Gui.TextBox_AppDataPath.Text;
                }));
                return value;
            }
        }

        public static int TimeDiff
        {
            get
            {
                int value = 0;
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                {
                    int.TryParse(Gui.TextBox_TimeDiff.Text, out value);
                }));
                return value;
            }
        }

        public static bool TimerHud
        {
            get
            {
                bool value = true;
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                {
                    value = Gui.CheckBox_TimerHud.IsChecked == true;
                }));
                return value;
            }
        }

        // --------------------------------------

        public App()
        {
            Gui = new WindowMain();
            Gui.Show();
            RegisterKeyboardHook();
            ConfigManager.LoadRegistry();
            if (StartMinimized) Gui.WindowState = WindowState.Minimized;
            LobbyTime = new LobbyTime();
            Handler.Start();

            //Autoupdate.CheckForUpdates(UPDATE_HREF + "?v=" + VERSION + "&id=" + (string.IsNullOrEmpty(MACHINE_GUID) ? "" : MACHINE_GUID), UPDATE_HREF, "PsHandler", "PsHandler.exe", Gui, Quit);
        }

        public static void RegisterKeyboardHook()
        {
            KeyboardHook = new KeyboardHook();
            KeyboardHook.KeyCombinationDownMethods.Add(kc =>
            {
                if (kc.Equals(App.HotkeyHandReplay))
                {
                    Handler.ClickReplayHandButton();
                }
                if (kc.Equals(App.HotkeyExit))
                {
                    App.Quit();
                }
            });
        }

        public static void Quit()
        {
            Autoupdate.Quit();
            HudManager.Stop();
            KeyboardHook.Dispose();
            Handler.Stop();
            ConfigManager.SaveRegistry();
            Gui.IsClosing = true;
            new Thread(() => Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => Gui.Close()))).Start();
        }
    }
}

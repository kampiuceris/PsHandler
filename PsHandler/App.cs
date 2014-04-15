using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace PsHandler
{
    public class App : Application
    {
        public const string NAME = "PsHandler";
        public const string VERSION = "1.5";
        public const string UPDATE_PATH = "http://chainer.puslapiai.lt/PsHandler/update.xml";
        public static WindowMain Gui;
        public static KeyboardHook KeyboardHook;
        public static Thread ThreadUpdate;

        public static PokerStarsTheme PokerStarsTheme
        {
            get
            {
                PokerStarsTheme value = null;
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                {
                    value = Gui.ComboBox_PokerStarsTheme.SelectedItem as PokerStarsTheme ?? new PokerStarsThemes.Unknown();
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

        public App()
        {
            Gui = new WindowMain();
            Gui.Show();
            RegisterKeyboardHook();
            LoadRegistry();

            Handler.Start();

            Autoupdate.CheckForUpdates(out ThreadUpdate, UPDATE_PATH, "PsHandler", "PsHandler.exe", Gui, Quit);
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
            KeyboardHook.Dispose();
            Handler.Stop();
            SaveRegistry();
            Gui.IsClosing = true;
            new Thread(() => Gui.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => Gui.Close()))).Start();
        }

        public static void LoadRegistry()
        {
            CheckRegistry();

            try
            {
                RegistryKey keyPsHandler = Registry.CurrentUser.OpenSubKey(@"Software\PSHandler");

                Gui.CheckBox_AutoclickImBack.IsChecked = (int)keyPsHandler.GetValue("AutoclickImBack") != 0;
                Gui.CheckBox_AutoclickTimebank.IsChecked = (int)keyPsHandler.GetValue("AutoclickTimebank") != 0;
                Gui.CheckBox_AutocloseTournamentRegistrationPopups.IsChecked = (int)keyPsHandler.GetValue("AutocloseTournamentRegistrationPopups") != 0;
                Gui.CheckBox_AutocloseHM2ApplyToSimilarTablesPopups.IsChecked = (int)keyPsHandler.GetValue("AutocloseHM2ApplyToSimilarTablesPopups") != 0;
                Gui.CheckBox_MinimizeToSystemTray.IsChecked = (int)keyPsHandler.GetValue("MinimizeToSystemTray") != 0;

                string pokerStarsTheme = (string)keyPsHandler.GetValue("PokerStarsTheme");
                foreach (var item in Gui.ComboBox_PokerStarsTheme.Items)
                {
                    if (item.ToString().Equals(pokerStarsTheme))
                    {
                        Gui.ComboBox_PokerStarsTheme.SelectedItem = item;
                        break;
                    }
                }

                string handReplayHotkey = (string)keyPsHandler.GetValue("HandReplayHotkey");
                if (handReplayHotkey != null)
                {
                    Gui.TextBoxHotkey_HandReplay.KeyCombination = KeyCombination.Parse("False False False " + handReplayHotkey);
                    using (RegistryKey keyPsHandlerDelete = Registry.CurrentUser.OpenSubKey(@"Software\PSHandler", true)) keyPsHandlerDelete.DeleteValue("HandReplayHotkey"); //delete v1.4 version hotkey
                }
                else
                {
                    Gui.TextBoxHotkey_HandReplay.KeyCombination = KeyCombination.Parse((string)keyPsHandler.GetValue("HotkeyHandReplay"));
                }

                Gui.TextBoxHotkey_Exit.KeyCombination = KeyCombination.Parse((string)keyPsHandler.GetValue("HotkeyExit"));

                keyPsHandler.Dispose();
            }
            catch (Exception)
            {
            }
        }

        public static void SaveRegistry()
        {
            CheckRegistry();

            try
            {
                // check if registry is okay
                RegistryKey keyPsHandler = Registry.CurrentUser.OpenSubKey(@"Software\PSHandler", true);

                keyPsHandler.SetValue("AutoclickImBack", AutoclickImBack ? 1 : 0);
                keyPsHandler.SetValue("AutoclickTimebank", AutoclickTimebank ? 1 : 0);
                keyPsHandler.SetValue("AutocloseTournamentRegistrationPopups", AutocloseTournamentRegistrationPopups ? 1 : 0);
                keyPsHandler.SetValue("AutocloseHM2ApplyToSimilarTablesPopups", AutocloseHM2ApplyToSimilarTablesPopups ? 1 : 0);
                keyPsHandler.SetValue("MinimizeToSystemTray", MinimizeToSystemTray ? 1 : 0);
                keyPsHandler.SetValue("PokerStarsTheme", PokerStarsTheme.ToString());
                keyPsHandler.SetValue("HotkeyHandReplay", HotkeyHandReplay.ToString());
                keyPsHandler.SetValue("HotkeyExit", HotkeyExit.ToString());

                keyPsHandler.Dispose();
            }
            catch (Exception)
            {
            }
        }

        public static void CheckRegistry()
        {
            try
            {
                // check if registry is okay
                RegistryKey keyPsHandler = Registry.CurrentUser.OpenSubKey(@"Software\PSHandler", true);
                if (keyPsHandler == null)
                {
                    using (RegistryKey keySoftware = Registry.CurrentUser.OpenSubKey(@"Software", true))
                    {
                        keyPsHandler = keySoftware.CreateSubKey("PsHandler");
                    }
                }

                if (keyPsHandler.GetValue("Version") == null)
                {
                    keyPsHandler.SetValue("Version", VERSION);
                }

                if (keyPsHandler.GetValue("AutoclickImBack") == null)
                {
                    keyPsHandler.SetValue("AutoclickImBack", 0);
                }

                if (keyPsHandler.GetValue("AutoclickTimebank") == null)
                {
                    keyPsHandler.SetValue("AutoclickTimebank", 0);
                }

                if (keyPsHandler.GetValue("AutocloseTournamentRegistrationPopups") == null)
                {
                    keyPsHandler.SetValue("AutocloseTournamentRegistrationPopups", 0);
                }

                if (keyPsHandler.GetValue("AutocloseHM2ApplyToSimilarTablesPopups") == null)
                {
                    keyPsHandler.SetValue("AutocloseHM2ApplyToSimilarTablesPopups", 0);
                }

                if (keyPsHandler.GetValue("MinimizeToSystemTray") == null)
                {
                    keyPsHandler.SetValue("MinimizeToSystemTray", 0);
                }

                if (keyPsHandler.GetValue("PokerStarsTheme") == null)
                {
                    keyPsHandler.SetValue("PokerStarsTheme", "Unknown");
                }

                if (keyPsHandler.GetValue("HotkeyHandReplay") == null)
                {
                    keyPsHandler.SetValue("HotkeyHandReplay", new KeyCombination(Key.None, false, false, false).ToString());
                }

                if (keyPsHandler.GetValue("HotkeyExit") == null)
                {
                    keyPsHandler.SetValue("HotkeyExit", new KeyCombination(Key.None, false, false, false).ToString());
                }

                keyPsHandler.Close();
                keyPsHandler.Dispose();
            }
            catch (Exception)
            {
            }
        }
    }
}

using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using PsHandler.Hooks;

namespace PsHandler
{
    public class App : Application
    {
        private static WindowMain Gui;
        private static KeyboardHook KeyboardHook;

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

        public static Key HandReplayHotkey
        {
            get
            {
                Key? value = Key.None;
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                {
                    value = Gui.ComboBox_HandReplayHotkey.SelectedItem as Key?;
                }));
                return value == null ? Key.None : (Key)value;
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
        }

        public static void RegisterKeyboardHook()
        {
            KeyboardHook = new KeyboardHook();
            KeyboardHook.CallbacksKeyDown.Add(keyDown =>
            {
                if (keyDown == App.HandReplayHotkey)
                {
                    Handler.ClickReplayHandButton();
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
            //Current.Shutdown();
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
                foreach (var item in Gui.ComboBox_HandReplayHotkey.Items)
                {
                    if (item.ToString().Equals(handReplayHotkey))
                    {
                        Gui.ComboBox_HandReplayHotkey.SelectedItem = item;
                        break;
                    }
                }

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
                keyPsHandler.SetValue("MinimizeToSystemTray", MinimizeToSystemTray ? 1 : 0);
                keyPsHandler.SetValue("PokerStarsTheme", PokerStarsTheme.ToString());
                keyPsHandler.SetValue("HandReplayHotkey", HandReplayHotkey.ToString());

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

                if (keyPsHandler.GetValue("MinimizeToSystemTray") == null)
                {
                    keyPsHandler.SetValue("MinimizeToSystemTray", 0);
                }

                if (keyPsHandler.GetValue("PokerStarsTheme") == null)
                {
                    keyPsHandler.SetValue("PokerStarsTheme", "Unknown");
                }

                if (keyPsHandler.GetValue("HandReplayHotkey") == null)
                {
                    keyPsHandler.SetValue("HandReplayHotkey", "");
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

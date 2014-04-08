using System;
using System.Threading;
using System.Windows;
using Microsoft.Win32;


namespace PsHandler
{
    public class App : Application
    {
        private static WindowMain Gui;

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
            LoadRegistry();
            Handler.Start();
        }

        public static void Quit()
        {
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

                int autoclickImBack = (int)keyPsHandler.GetValue("AutoclickImBack");
                int autocloseTournamentRegistrationPopups = (int)keyPsHandler.GetValue("AutocloseTournamentRegistrationPopups");
                int minimizeToSystemTray = (int)keyPsHandler.GetValue("MinimizeToSystemTray");
                string pokerStarsTheme = (string)keyPsHandler.GetValue("PokerStarsTheme");

                Gui.CheckBox_AutoclickImBack.IsChecked = autoclickImBack != 0;
                Gui.CheckBox_AutocloseTournamentRegistrationPopups.IsChecked = autocloseTournamentRegistrationPopups != 0;
                Gui.CheckBox_MinimizeToSystemTray.IsChecked = minimizeToSystemTray != 0;
                foreach (var item in Gui.ComboBox_PokerStarsTheme.Items)
                {
                    if (item.ToString().Equals(pokerStarsTheme))
                    {
                        Gui.ComboBox_PokerStarsTheme.SelectedItem = item;
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
                keyPsHandler.SetValue("AutocloseTournamentRegistrationPopups", AutocloseTournamentRegistrationPopups ? 1 : 0);
                keyPsHandler.SetValue("MinimizeToSystemTray", MinimizeToSystemTray ? 1 : 0);
                keyPsHandler.SetValue("PokerStarsTheme", PokerStarsTheme.ToString());

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

                keyPsHandler.Close();
                keyPsHandler.Dispose();
            }
            catch (Exception)
            {
            }
        }
    }
}

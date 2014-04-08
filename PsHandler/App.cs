using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Shapes;

namespace PsHandler
{
    public class App : Application
    {
        private static WindowMain Gui;

        public static string PokerStarsAppDataPath
        {
            get
            {
                string value = "";
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                {
                    value = Gui.TextBox_PokerStarsAppDataPath.Text;
                }));
                return value;
            }
        }

        public static PokerStarsTheme GetPokerStarsTheme
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

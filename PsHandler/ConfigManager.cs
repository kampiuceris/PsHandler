using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Win32;

namespace PsHandler
{
    public class ConfigManager
    {
        public static void LoadRegistry()
        {
            CheckRegistry();

            try
            {
                RegistryKey keyPsHandler = Registry.CurrentUser.OpenSubKey(@"Software\PSHandler");

                App.Gui.CheckBox_AutoclickImBack.IsChecked = (int)keyPsHandler.GetValue("AutoclickImBack") != 0;
                App.Gui.CheckBox_AutoclickTimebank.IsChecked = (int)keyPsHandler.GetValue("AutoclickTimebank") != 0;
                App.Gui.CheckBox_AutocloseTournamentRegistrationPopups.IsChecked = (int)keyPsHandler.GetValue("AutocloseTournamentRegistrationPopups") != 0;
                App.Gui.CheckBox_AutocloseHM2ApplyToSimilarTablesPopups.IsChecked = (int)keyPsHandler.GetValue("AutocloseHM2ApplyToSimilarTablesPopups") != 0;
                App.Gui.CheckBox_MinimizeToSystemTray.IsChecked = (int)keyPsHandler.GetValue("MinimizeToSystemTray") != 0;
                App.Gui.CheckBox_StartMinimized.IsChecked = (int)keyPsHandler.GetValue("StartMinimized") != 0;

                string pokerStarsThemeLobby = (string)keyPsHandler.GetValue("PokerStarsThemeLobby");
                foreach (var item in App.Gui.ComboBox_PokerStarsThemeLobby.Items)
                {
                    if (item.ToString().Equals(pokerStarsThemeLobby))
                    {
                        App.Gui.ComboBox_PokerStarsThemeLobby.SelectedItem = item;
                        break;
                    }
                }

                string pokerStarsThemeTable = (string)keyPsHandler.GetValue("PokerStarsTheme");
                if (pokerStarsThemeTable == null)
                    pokerStarsThemeTable = (string)keyPsHandler.GetValue("PokerStarsThemeTable");
                else
                    using (RegistryKey keyPsHandlerDelete = Registry.CurrentUser.OpenSubKey(@"Software\PSHandler", true)) keyPsHandlerDelete.DeleteValue("PokerStarsTheme"); //delete v1.4 tabletheme
                foreach (var item in App.Gui.ComboBox_PokerStarsThemeTable.Items)
                {
                    if (item.ToString().Equals(pokerStarsThemeTable))
                    {
                        App.Gui.ComboBox_PokerStarsThemeTable.SelectedItem = item;
                        break;
                    }
                }

                string handReplayHotkey = (string)keyPsHandler.GetValue("HandReplayHotkey");
                if (handReplayHotkey != null)
                {
                    App.Gui.TextBoxHotkey_HandReplay.KeyCombination = KeyCombination.Parse("False False False " + handReplayHotkey);
                    using (RegistryKey keyPsHandlerDelete = Registry.CurrentUser.OpenSubKey(@"Software\PSHandler", true)) keyPsHandlerDelete.DeleteValue("HandReplayHotkey"); //delete v1.4 version hotkey
                }
                else
                {
                    App.Gui.TextBoxHotkey_HandReplay.KeyCombination = KeyCombination.Parse((string)keyPsHandler.GetValue("HotkeyHandReplay"));
                }

                App.Gui.TextBoxHotkey_Exit.KeyCombination = KeyCombination.Parse((string)keyPsHandler.GetValue("HotkeyExit"));
                App.Gui.TextBox_TimeDiff.Text = keyPsHandler.GetValue("TimeDiff").ToString();
                App.Gui.CheckBox_TimerHud.IsChecked = (int)keyPsHandler.GetValue("TimerHud") != 0;
                App.Gui.TextBox_AppDataPath.Text = (string)keyPsHandler.GetValue("AppDataPath");

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

                keyPsHandler.SetValue("Version", App.VERSION);
                keyPsHandler.SetValue("AutoclickImBack", App.AutoclickImBack ? 1 : 0);
                keyPsHandler.SetValue("AutoclickTimebank", App.AutoclickTimebank ? 1 : 0);
                keyPsHandler.SetValue("AutocloseTournamentRegistrationPopups", App.AutocloseTournamentRegistrationPopups ? 1 : 0);
                keyPsHandler.SetValue("AutocloseHM2ApplyToSimilarTablesPopups", App.AutocloseHM2ApplyToSimilarTablesPopups ? 1 : 0);
                keyPsHandler.SetValue("MinimizeToSystemTray", App.MinimizeToSystemTray ? 1 : 0);
                keyPsHandler.SetValue("StartMinimized", App.StartMinimized ? 1 : 0);
                keyPsHandler.SetValue("PokerStarsThemeLobby", App.PokerStarsThemeLobby.ToString());
                keyPsHandler.SetValue("PokerStarsThemeTable", App.PokerStarsThemeTable.ToString());
                keyPsHandler.SetValue("HotkeyHandReplay", App.HotkeyHandReplay.ToString());
                keyPsHandler.SetValue("HotkeyExit", App.HotkeyExit.ToString());
                keyPsHandler.SetValue("TimeDiff", App.TimeDiff);
                keyPsHandler.SetValue("TimerHud", App.TimerHud ? 1 : 0);
                keyPsHandler.SetValue("AppDataPath", App.AppDataPath);

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
                    keyPsHandler.SetValue("Version", App.VERSION);
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

                if (keyPsHandler.GetValue("StartMinimized") == null)
                {
                    keyPsHandler.SetValue("StartMinimized", 0);
                }

                if (keyPsHandler.GetValue("PokerStarsThemeLobby") == null)
                {
                    keyPsHandler.SetValue("PokerStarsThemeLobby", "Unknown");
                }

                if (keyPsHandler.GetValue("PokerStarsThemeTable") == null)
                {
                    keyPsHandler.SetValue("PokerStarsThemeTable", "Unknown");
                }

                if (keyPsHandler.GetValue("HotkeyHandReplay") == null)
                {
                    keyPsHandler.SetValue("HotkeyHandReplay", new KeyCombination(Key.None, false, false, false).ToString());
                }

                if (keyPsHandler.GetValue("HotkeyExit") == null)
                {
                    keyPsHandler.SetValue("HotkeyExit", new KeyCombination(Key.None, false, false, false).ToString());
                }

                if (keyPsHandler.GetValue("TimeDiff") == null)
                {
                    keyPsHandler.SetValue("TimeDiff", 0);
                }

                if (keyPsHandler.GetValue("TimerHud") == null)
                {
                    keyPsHandler.SetValue("TimerHud", 0);
                }

                if (keyPsHandler.GetValue("AppDataPath") == null)
                {
                    keyPsHandler.SetValue("AppDataPath", "");
                }

                keyPsHandler.Close();
                keyPsHandler.Dispose();
            }
            catch (Exception)
            {
            }
        }

        public static string GetMachineGuid()
        {
            try
            {
                using (RegistryKey keyCryptography = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography"))
                {
                    return keyCryptography.GetValue("MachineGuid") as string;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

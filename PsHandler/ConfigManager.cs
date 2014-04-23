using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Win32;
using PsHandler.Types;

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

                keyPsHandler.SetValue("Version", App.VERSION);

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

                if (keyPsHandler.GetValue("PokerStarsThemeTable") == null)
                {
                    keyPsHandler.SetValue("PokerStarsThemeTable", "Unknown");
                }

                if (keyPsHandler.GetValue("PokerStarsThemeLobby") != null)
                {
                    keyPsHandler.DeleteValue("PokerStarsThemeLobby"); //delete from 1.5v
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

                using (RegistryKey keyPokerTypes = keyPsHandler.OpenSubKey("PokerTypes", true))
                {
                    if (keyPokerTypes == null)
                    {
                        RegistryKey keyPokerTypesNew = keyPsHandler.CreateSubKey("PokerTypes");

                        #region Seed Poker Types

                        PokerType pt;

                        // 10-max Fifty50

                        pt = new PokerType
                        {
                            Name = "10-max Fifty50 Regular",
                            LevelLengthInSeconds = 360,
                            IncludeAnd = new[] { "Logged In as", "Tournament", "Fifty50" },
                            IncludeOr = new string[0],
                            ExcludeAnd = new string[0],
                            ExcludeOr = new[] { "Turbo", "Hyper", "6-Max" },
                            BuyInAndRake = new[] { "$1.35 + $0.15", "$3.26 + $0.24", "$6.60 + $0.40", "$14.15 + $0.85", "$28.30 + $1.70", "$56.60 + $3.40", "$94.90 + $5.10", "$191.35 + $8.65" }
                        };
                        keyPokerTypesNew.SetValue(pt.Name, pt.ToXml());

                        pt = new PokerType
                        {
                            Name = "10-max Fifty50 Turbo",
                            LevelLengthInSeconds = 180,
                            IncludeAnd = new[] { "Logged In as", "Tournament", "Fifty50", "Turbo" },
                            IncludeOr = new string[0],
                            ExcludeAnd = new string[0],
                            ExcludeOr = new[] { "Hyper", "6-Max" },
                            BuyInAndRake = new[] { "$1.39 + $0.11", "$3.30 + $0.20", "$6.68 + $0.32", "$14.31 + $0.69", "$28.63 + $1.37", "$57.25 + $2.75", "$95.86 + $4.14", "$193.05 + $6.95", "$291.60 + $8.40", "$487.20 + $12.80" }
                        };
                        keyPokerTypesNew.SetValue(pt.Name, pt.ToXml());

                        // 2-max

                        pt = new PokerType
                        {
                            Name = "2-max 2 Players Regular",
                            LevelLengthInSeconds = 360,
                            IncludeAnd = new[] { "Logged In as", "Tournament", "HU", "2 Players" },
                            IncludeOr = new string[0],
                            ExcludeAnd = new string[0],
                            ExcludeOr = new[] { "Turbo", "Hyper", "6-Max" },
                            BuyInAndRake = new[] { "$1.38 + $0.12", "$3.29 + $0.21", "$6.67 + $0.33", "$14.29 + $0.71", "$28.57 + $1.43", "$57.28 + $2.72", "$95.69 + $4.31", "$192.75 + $7.25", "$289.85 + $10.15", "$485.40 + $14.60", "$975.60 + $24.40", "$1956.00 + $44.00", "$4926.00 + $74.00" }
                        };
                        keyPokerTypesNew.SetValue(pt.Name, pt.ToXml());

                        pt = new PokerType
                        {
                            Name = "2-max 2 Players Turbo",
                            LevelLengthInSeconds = 180,
                            IncludeAnd = new[] { "Logged In as", "Tournament", "HU", "2 Players", "Turbo" },
                            IncludeOr = new string[0],
                            ExcludeAnd = new string[0],
                            ExcludeOr = new[] { "Hyper", "6-Max" },
                            BuyInAndRake = new[] { "$1.40 + $0.10", "$3.32 + $0.18", "$6.71 + $0.29", "$14.39 + $0.61", "$28.78 + $1.22", "$57.67 + $2.33", "$96.32 + $3.68", "$193.85 + $6.15", "$291.25 + $8.75", "$487.60 + $12.40", "$979.20 + $20.80", "$1962.50 + $37.50", "$4937.00 + $63.00" }
                        };
                        keyPokerTypesNew.SetValue(pt.Name, pt.ToXml());

                        pt = new PokerType
                        {
                            Name = "2-max 2 Players Hyper-Turbo",
                            LevelLengthInSeconds = 120,
                            IncludeAnd = new[] { "Logged In as", "Tournament", "HU", "2 Players", "Hyper-Turbo" },
                            IncludeOr = new string[0],
                            ExcludeAnd = new string[0],
                            ExcludeOr = new[] { "6-Max" },
                            BuyInAndRake = new[] { "$1.44 + $0.06", "$3.40 + $0.10", "$6.85 + $0.15", "$14.69 + $0.31", "$29.37 + $0.63", "$58.74 + $1.26", "$98.12 + $1.88", "$196.66 + $3.34", "$295.51 + $4.49", "$493.35 + $6.65", "$988.80 + $11.20" }
                        };
                        keyPokerTypesNew.SetValue(pt.Name, pt.ToXml());

                        // 9-max

                        pt = new PokerType
                        {
                            Name = "9-max Regular",
                            LevelLengthInSeconds = 600,
                            IncludeAnd = new[] { "Logged In as", "Tournament" },
                            IncludeOr = new string[0],
                            ExcludeAnd = new string[0],
                            ExcludeOr = new[] { "Turbo", "Hyper", "6-Max" },
                            BuyInAndRake = new[] { "$1.29 + $0.21", "$3.11 + $0.39", "$6.37 + $0.63", "$13.70 + $1.30", "$27.40 + $2.60", "$54.80 + $5.20", "$92.15 + $7.85", "$186.50 + $13.50", "$281.00 + $19.00", "$471.75 + $28.25" }
                        };
                        keyPokerTypesNew.SetValue(pt.Name, pt.ToXml());

                        pt = new PokerType
                        {
                            Name = "9-max Turbo",
                            LevelLengthInSeconds = 300,
                            IncludeAnd = new[] { "Logged In as", "Tournament", "Turbo" },
                            IncludeOr = new string[0],
                            ExcludeAnd = new string[0],
                            ExcludeOr = new[] { "Hyper", "6-Max" },
                            BuyInAndRake = new[] { "$1.32 + $0.18", "$3.16 + $0.34", "$6.45 + $0.55", "$13.89 + $1.11", "$27.78 + $2.22", "$55.56 + $4.44", "$92.80 + $7.20", "$187.80 + $12.20", "$283.00 + $17.00", "$474.00 + $26.00", "$957.00 + $43.00", "$1923.00 + $77.00" }
                        };
                        keyPokerTypesNew.SetValue(pt.Name, pt.ToXml());

                        pt = new PokerType
                        {
                            Name = "9-max Hyper-Turbo",
                            LevelLengthInSeconds = 120,
                            IncludeAnd = new[] { "Logged In as", "Tournament", "Hyper-Turbo" },
                            IncludeOr = new string[0],
                            ExcludeAnd = new string[0],
                            ExcludeOr = new[] { "6-Max" },
                            BuyInAndRake = new[] { "$1.39 + $0.11", "$3.31 + $0.19", "$6.70 + $0.30", "$14.39 + $0.61", "$28.77 + $1.23", "$57.54 + $2.46", "$96.32 + $3.68", "$193.18 + $6.82" }
                        };
                        keyPokerTypesNew.SetValue(pt.Name, pt.ToXml());

                        // 6-max

                        pt = new PokerType
                        {
                            Name = "6-max Regular",
                            LevelLengthInSeconds = 600,
                            IncludeAnd = new[] { "Logged In as", "Tournament", "6-Max" },
                            IncludeOr = new string[0],
                            ExcludeAnd = new string[0],
                            ExcludeOr = new[] { "Turbo", "Hyper" },
                            BuyInAndRake = new[] { "$1.29 + $0.21", "$3.13 + $0.37", "$6.39 + $0.61", "$13.79 + $1.21", "$27.58 + $2.42", "$55.13 + $4.84", "$92.60 + $7.40", "$186.90 + $13.10", "$281.70 + $18.30", "$472.75 + $27.25" }
                        };
                        keyPokerTypesNew.SetValue(pt.Name, pt.ToXml());

                        pt = new PokerType
                        {
                            Name = "6-max Turbo",
                            LevelLengthInSeconds = 300,
                            IncludeAnd = new[] { "Logged In as", "Tournament", "6-Max", "Turbo" },
                            IncludeOr = new string[0],
                            ExcludeAnd = new string[0],
                            ExcludeOr = new[] { "Hyper" },
                            BuyInAndRake = new[] { "$1.32 + $0.18", "$3.19 + $0.31", "$6.48 + $0.52", "$13.92 + $1.08", "$27.84 + $2.16", "$55.68 + $4.32", "$93.25 + $6.75", "$188.20 + $11.80", "$283.70 + $16.30", "$475.00 + $25.00", "$959.25 + $40.75", "$1928.00 + $72.00", "$4878.00 + $122.00" }
                        };
                        keyPokerTypesNew.SetValue(pt.Name, pt.ToXml());

                        pt = new PokerType
                        {
                            Name = "6-max Hyper-Turbo",
                            LevelLengthInSeconds = 120,
                            IncludeAnd = new[] { "Logged In as", "Tournament", "6-Max", "Hyper-Turbo" },
                            IncludeOr = new string[0],
                            ExcludeAnd = new string[0],
                            ExcludeOr = new string[0],
                            BuyInAndRake = new[] { "$1.40 + $0.10", "$3.32 + $0.18", "$6.71 + $0.29", "$14.41 + $0.59", "$28.83 + $1.17", "$57.66 + $2.34", "$96.49 + $3.51", "$193.52 + $6.48", "$291.40 + $8.60", "$487.52 + $12.48" }
                        };
                        keyPokerTypesNew.SetValue(pt.Name, pt.ToXml());

                        //  9-max knockout

                        pt = new PokerType
                        {
                            Name = "9-max Knockout Regular",
                            LevelLengthInSeconds = 600,
                            IncludeAnd = new[] { "Logged In as", "Tournament", "Knockout" },
                            IncludeOr = new string[0],
                            ExcludeAnd = new string[0],
                            ExcludeOr = new[] { "Turbo", "Hyper", "6-Max" },
                            BuyInAndRake = new[] { "$1.35 + $0.15", "$3.20 + $0.30", "$6.50 + $0.50", "$13.90 + $1.10", "$27.80 + $2.20", "$55.75 + $4.25", "$93.60 + $6.40", "$189.05 + $10.95" }
                        };
                        keyPokerTypesNew.SetValue(pt.Name, pt.ToXml());

                        pt = new PokerType
                        {
                            Name = "9-max Knockout Turbo",
                            LevelLengthInSeconds = 300,
                            IncludeAnd = new[] { "Logged In as", "Tournament", "Knockout", "Turbo" },
                            IncludeOr = new string[0],
                            ExcludeAnd = new string[0],
                            ExcludeOr = new[] { "Hyper", "6-Max" },
                            BuyInAndRake = new[] { "$1.35 + $0.15", "$3.20 + $0.30", "$6.55 + $0.45", "$14.10 + $0.90", "$28.15 + $1.85", "$56.40 + $3.60", "$94.15 + $5.85" }
                        };
                        keyPokerTypesNew.SetValue(pt.Name, pt.ToXml());

                        #endregion
                    }
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

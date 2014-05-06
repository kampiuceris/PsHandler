using System.Globalization;
using System.Windows.Input;
using Microsoft.Win32;
using PsHandler.Hud;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using PsHandler.PokerTypes;
using PsHandler.TableTiler;

namespace PsHandler
{
    public class Config
    {
        // Constants

        public const string NAME = "PsHandler";
        public const int VERSION = 8;
        public const string UPDATE_HREF = "http://chainer.projektas.in/PsHandler/update.php";
        public static string MACHINE_GUID = GetMachineGuid();

        // Settings

        public static string AppDataPath;
        public static PokerStarsThemeTable PokerStarsThemeTable;
        public static bool MinimizeToSystemTray;
        public static bool StartMinimized;
        public static KeyCombination HotkeyExit;

        // Controller

        public static bool AutoclickImBack;
        public static bool AutoclickTimebank;
        public static bool AutoclickYesSeatAvailable;
        public static bool AutocloseTournamentRegistrationPopups;
        public static bool AutocloseHM2ApplyToSimilarTablesPopups;
        public static KeyCombination HotkeyHandReplay;

        // HUD

        public static bool TimerHud;
        public static int TimeDiff;

        // Registry

        private static string GetMachineGuid()
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

        private static bool SetValue(string relativePath, object value)
        {
            try
            {
                string[] paths = relativePath.Split(new[] { @"\", @"/" }, StringSplitOptions.RemoveEmptyEntries);

                RegistryKey keyPsHandler = Registry.CurrentUser.OpenSubKey(@"Software\PSHandler", true);
                if (keyPsHandler == null)
                {
                    using (RegistryKey keySoftware = Registry.CurrentUser.OpenSubKey(@"Software", true))
                    {
                        if (keySoftware == null) throw new NotSupportedException("Cannot load 'HKEY_CURRENTY_USER/Software'");
                        keyPsHandler = keySoftware.CreateSubKey("PsHandler");
                        if (keyPsHandler == null) throw new NotSupportedException("Cannot create 'HKEY_CURRENTY_USER/Software/PsHandler'");
                    }
                }

                List<RegistryKey> keys = new List<RegistryKey> { keyPsHandler };
                for (int i = 0; i < paths.Length - 1; i++)
                {
                    RegistryKey subKey = keys[keys.Count - 1].OpenSubKey(paths[i], true) ?? keys[keys.Count - 1].CreateSubKey(paths[i]);
                    if (subKey == null) throw new NotSupportedException("Cannot create ('" + relativePath + "') '" + paths[i] + "'");
                    keys.Add(subKey);
                }
                keys[keys.Count - 1].SetValue(paths[paths.Length - 1], value);

                foreach (var key in keys)
                {
                    key.Dispose();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static object GetValue(string relativePath, object defaultValue)
        {
            try
            {
                string[] paths = relativePath.Split(new[] { @"\", @"/" }, StringSplitOptions.RemoveEmptyEntries);

                RegistryKey keyPsHandler = Registry.CurrentUser.OpenSubKey(@"Software\PSHandler", true);
                if (keyPsHandler == null)
                {
                    using (RegistryKey keySoftware = Registry.CurrentUser.OpenSubKey(@"Software", true))
                    {
                        if (keySoftware == null) throw new NotSupportedException("Cannot load 'HKEY_CURRENTY_USER/Software'");
                        keyPsHandler = keySoftware.CreateSubKey("PsHandler");
                        if (keyPsHandler == null) throw new NotSupportedException("Cannot create 'HKEY_CURRENTY_USER/Software/PsHandler'");
                    }
                }

                List<RegistryKey> keys = new List<RegistryKey> { keyPsHandler };
                for (int i = 0; i < paths.Length - 1; i++)
                {
                    RegistryKey subKey = keys[keys.Count - 1].OpenSubKey(paths[i], true) ?? keys[keys.Count - 1].CreateSubKey(paths[i]);
                    if (subKey == null) throw new NotSupportedException("Cannot create ('" + relativePath + "') '" + paths[i] + "'");
                    keys.Add(subKey);
                }

                object value = keys[keys.Count - 1].GetValue(paths[paths.Length - 1]);
                if (value == null)
                {
                    keys[keys.Count - 1].SetValue(paths[paths.Length - 1], defaultValue);
                    value = defaultValue;
                }

                foreach (var key in keys)
                {
                    key.Dispose();
                }

                return value;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool GetBool(string relativePath, object defaultValue)
        {
            return (int)GetValue(relativePath, defaultValue) != 0;
        }

        private static int GetInt(string relativePath, object defaultValue)
        {
            return (int)GetValue(relativePath, defaultValue);
        }

        private static string GetString(string relativePath, object defaultValue)
        {
            return (string)GetValue(relativePath, defaultValue);
        }

        private static float GetFloat(string relativePath, object defaultValue)
        {
            return float.Parse((string)GetValue(relativePath, defaultValue));
        }

        //

        public static void Load()
        {
            // settings

            AppDataPath = GetString("AppDataPath", "");
            PokerStarsThemeTable = PokerStarsThemeTable.Parse(GetString("PokerStarsThemeTable", "Unknown"));
            MinimizeToSystemTray = GetBool("MinimizeToSystemTray", 0);
            StartMinimized = GetBool("StartMinimized", 0);
            HotkeyExit = KeyCombination.Parse(GetString("HotkeyExit", new KeyCombination(Key.None, false, false, false).ToString()));

            // controller

            AutoclickImBack = GetBool("AutoclickImBack", 0);
            AutoclickTimebank = GetBool("AutoclickTimebank", 0);
            AutoclickYesSeatAvailable = GetBool("AutoclickYesSeatAvailable", 0);
            AutocloseTournamentRegistrationPopups = GetBool("AutocloseTournamentRegistrationPopups", 0);
            AutocloseHM2ApplyToSimilarTablesPopups = GetBool("AutocloseHM2ApplyToSimilarTablesPopups", 0);
            HotkeyHandReplay = KeyCombination.Parse(GetString("HotkeyHandReplay", new KeyCombination(Key.None, false, false, false).ToString()));

            // hud

            TimerHud = GetBool("TimerHud", 0);
            TimeDiff = GetInt("TimeDiff", 0);

            HudManager.TimerHudLocationLocked = GetBool("TimerHudLocationLocked", 0);
            HudManager.TimerHudLocationX = GetFloat("TimerHudLocationX", HudManager.TimerHudLocationX.ToString(CultureInfo.InvariantCulture));
            HudManager.TimerHudLocationY = GetFloat("TimerHudLocationY", HudManager.TimerHudLocationY.ToString(CultureInfo.InvariantCulture));
            HudManager.TimerHudBackground = (Color)ColorConverter.ConvertFromString(GetString("TimerHudBackground", HudManager.TimerHudBackground.ToString(CultureInfo.InvariantCulture)));
            HudManager.TimerHudForeground = (Color)ColorConverter.ConvertFromString(GetString("TimerHudForeground", HudManager.TimerHudForeground.ToString(CultureInfo.InvariantCulture)));
            HudManager.TimerHudFontFamily = new FontFamily(GetString("TimerHudFontFamily", HudManager.TimerHudFontFamily.ToString()));
            HudManager.TimerHudFontWeight = (FontWeight)new FontWeightConverter().ConvertFrom(GetString("TimerHudFontWeight", HudManager.TimerHudFontWeight.ToString()));
            HudManager.TimerHudFontStyle = (FontStyle)new FontStyleConverter().ConvertFrom(GetString("TimerHudFontStyle", HudManager.TimerHudFontStyle.ToString()));
            HudManager.TimerHudFontSize = GetFloat("TimerHudFontSize", HudManager.TimerHudFontSize.ToString(CultureInfo.InvariantCulture));

            // check invalid values

            if (HudManager.TimerHudLocationX < -10) HudManager.TimerHudLocationX = -10;
            if (HudManager.TimerHudLocationX > 10) HudManager.TimerHudLocationX = 10;
            if (HudManager.TimerHudLocationY < -10) HudManager.TimerHudLocationY = -10;
            if (HudManager.TimerHudLocationY > 10) HudManager.TimerHudLocationY = 10;
            if (HudManager.TimerHudFontSize < 1) HudManager.TimerHudFontSize = 1;
            if (HudManager.TimerHudFontSize > 72) HudManager.TimerHudFontSize = 72;

            // seed poker types if needed

            #region poker types

            using (RegistryKey keyPokerTypes = Registry.CurrentUser.OpenSubKey(@"Software\PSHandler\PokerTypes"))
            {
                if (keyPokerTypes == null)
                {
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
                    SetValue(@"PokerTypes\" + pt.Name, pt.ToXml());

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
                    SetValue(@"PokerTypes\" + pt.Name, pt.ToXml());

                    // 2-max

                    pt = new PokerType
                    {
                        Name = "2-max 2 _players Regular",
                        LevelLengthInSeconds = 360,
                        IncludeAnd = new[] { "Logged In as", "Tournament", "HU", "2 _players" },
                        IncludeOr = new string[0],
                        ExcludeAnd = new string[0],
                        ExcludeOr = new[] { "Turbo", "Hyper", "6-Max" },
                        BuyInAndRake = new[] { "$1.38 + $0.12", "$3.29 + $0.21", "$6.67 + $0.33", "$14.29 + $0.71", "$28.57 + $1.43", "$57.28 + $2.72", "$95.69 + $4.31", "$192.75 + $7.25", "$289.85 + $10.15", "$485.40 + $14.60", "$975.60 + $24.40", "$1956.00 + $44.00", "$4926.00 + $74.00" }
                    };
                    SetValue(@"PokerTypes\" + pt.Name, pt.ToXml());

                    pt = new PokerType
                    {
                        Name = "2-max 2 _players Turbo",
                        LevelLengthInSeconds = 180,
                        IncludeAnd = new[] { "Logged In as", "Tournament", "HU", "2 _players", "Turbo" },
                        IncludeOr = new string[0],
                        ExcludeAnd = new string[0],
                        ExcludeOr = new[] { "Hyper", "6-Max" },
                        BuyInAndRake = new[] { "$1.40 + $0.10", "$3.32 + $0.18", "$6.71 + $0.29", "$14.39 + $0.61", "$28.78 + $1.22", "$57.67 + $2.33", "$96.32 + $3.68", "$193.85 + $6.15", "$291.25 + $8.75", "$487.60 + $12.40", "$979.20 + $20.80", "$1962.50 + $37.50", "$4937.00 + $63.00" }
                    };
                    SetValue(@"PokerTypes\" + pt.Name, pt.ToXml());

                    pt = new PokerType
                    {
                        Name = "2-max 2 _players Hyper-Turbo",
                        LevelLengthInSeconds = 120,
                        IncludeAnd = new[] { "Logged In as", "Tournament", "HU", "2 _players", "Hyper-Turbo" },
                        IncludeOr = new string[0],
                        ExcludeAnd = new string[0],
                        ExcludeOr = new[] { "6-Max" },
                        BuyInAndRake = new[] { "$1.44 + $0.06", "$3.40 + $0.10", "$6.85 + $0.15", "$14.69 + $0.31", "$29.37 + $0.63", "$58.74 + $1.26", "$98.12 + $1.88", "$196.66 + $3.34", "$295.51 + $4.49", "$493.35 + $6.65", "$988.80 + $11.20" }
                    };
                    SetValue(@"PokerTypes\" + pt.Name, pt.ToXml());

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
                    SetValue(@"PokerTypes\" + pt.Name, pt.ToXml());

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
                    SetValue(@"PokerTypes\" + pt.Name, pt.ToXml());

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
                    SetValue(@"PokerTypes\" + pt.Name, pt.ToXml());

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
                    SetValue(@"PokerTypes\" + pt.Name, pt.ToXml());

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
                    SetValue(@"PokerTypes\" + pt.Name, pt.ToXml());

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
                    SetValue(@"PokerTypes\" + pt.Name, pt.ToXml());

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
                    SetValue(@"PokerTypes\" + pt.Name, pt.ToXml());

                    pt = new PokerType
                    {
                        Name = "9-max Knockout Turbo",
                        LevelLengthInSeconds = 300,
                        IncludeAnd = new[] { "Logged In as", "Tournament", " Knockout", "Turbo" },
                        IncludeOr = new string[0],
                        ExcludeAnd = new string[0],
                        ExcludeOr = new[] { "Hyper", "6-Max" },
                        BuyInAndRake = new[] { "$1.35 + $0.15", "$3.20 + $0.30", "$6.55 + $0.45", "$14.10 + $0.90", "$28.15 + $1.85", "$56.40 + $3.60", "$94.15 + $5.85" }
                    };
                    SetValue(@"PokerTypes\" + pt.Name, pt.ToXml());
                }
            }

            #endregion

            // TableTiler

            TableTileManager.Load();
        }

        public static void Save()
        {
            SetValue("Version", VERSION);

            // settings

            SetValue("AppDataPath", AppDataPath);
            SetValue("PokerStarsThemeTable", PokerStarsThemeTable.ToString());
            SetValue("MinimizeToSystemTray", MinimizeToSystemTray.ToInt());
            SetValue("StartMinimized", StartMinimized.ToInt());
            SetValue("HotkeyExit", HotkeyExit.ToString());

            // controller

            SetValue("AutoclickImBack", AutoclickImBack.ToInt());
            SetValue("AutoclickTimebank", AutoclickTimebank.ToInt());
            SetValue("AutoclickYesSeatAvailable", AutoclickYesSeatAvailable.ToInt());
            SetValue("AutocloseTournamentRegistrationPopups", AutocloseTournamentRegistrationPopups.ToInt());
            SetValue("AutocloseHM2ApplyToSimilarTablesPopups", AutocloseHM2ApplyToSimilarTablesPopups.ToInt());
            SetValue("HotkeyHandReplay", HotkeyHandReplay.ToString());

            // hud

            SetValue("TimerHud", TimerHud.ToInt());
            SetValue("TimeDiff", TimeDiff);

            SetValue("TimerHudLocationLocked", HudManager.TimerHudLocationLocked.ToInt());
            SetValue("TimerHudLocationX", HudManager.TimerHudLocationX.ToString(CultureInfo.InvariantCulture));
            SetValue("TimerHudLocationY", HudManager.TimerHudLocationY.ToString(CultureInfo.InvariantCulture));
            SetValue("TimerHudBackground", HudManager.TimerHudBackground.ToString(CultureInfo.InvariantCulture));
            SetValue("TimerHudForeground", HudManager.TimerHudForeground.ToString(CultureInfo.InvariantCulture));
            SetValue("TimerHudFontFamily", HudManager.TimerHudFontFamily.ToString());
            SetValue("TimerHudFontWeight", HudManager.TimerHudFontWeight.ToString());
            SetValue("TimerHudFontStyle", HudManager.TimerHudFontStyle.ToString());
            SetValue("TimerHudFontSize", HudManager.TimerHudFontSize.ToString(CultureInfo.InvariantCulture));

            // TableTiler

            TableTileManager.Save();
        }
    }
}

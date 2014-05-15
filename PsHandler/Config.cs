using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Win32;
using PsHandler.Hud;
using System;
using System.Collections.Generic;
using System.Windows;
using PsHandler.PokerTypes;
using PsHandler.TableTiler;
using System.Threading;
using System.IO;

namespace PsHandler
{
    public class Config
    {
        // Constants

        public const string NAME = "PsHandler";
        public const int VERSION = 10;
        public const string UPDATE_HREF = "http://chainer.projektas.in/PsHandler/update.php";
        public static string MACHINE_GUID = GetMachineGuid();

        // Settings

        public static string AppDataPath = "";
        public static PokerStarsThemeTable PokerStarsThemeTable = new PokerStarsThemesTable.Unknown();
        public static bool MinimizeToSystemTray = false;
        public static bool StartMinimized = false;
        public static KeyCombination HotkeyExit = new KeyCombination(Key.None, false, false, false);
        public static bool SaveGuiLocation = false;
        public static int GuiLocationX = 0;
        public static int GuiLocationY = 0;
        public static bool SaveGuiSize = false;
        public static int GuiWidth = 600;
        public static int GuiHeight = 400;

        // Controller

        public static bool AutoclickImBack = false;
        public static bool AutoclickTimebank = false;
        public static bool AutoclickYesSeatAvailable = false;
        public static bool AutocloseTournamentRegistrationPopups = false;
        public static bool AutocloseHM2ApplyToSimilarTablesPopups = false;
        public static KeyCombination HotkeyHandReplay = new KeyCombination(Key.None, false, false, false);

        // HUD

        public static bool EnableHud = false;
        public static int TimerDiff = 0;
        public static string TimerHHNotFound = "HH not found";
        public static string TimerPokerTypeNotFound = "Poker Type not found";
        public static string TimerMultiplePokerTypes = "Multiple Poker Types";
        public static int BigBlindDecimals = 0;

        // Table Tiler
        public static bool EnableTableTiler = false;

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

        private static bool DeleteValue(string relativePath)
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

                keys[keys.Count - 1].DeleteValue(paths[paths.Length - 1]);

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

        //

        public static void Load()
        {
            using (RegistryKey keyPsHandler = Registry.CurrentUser.OpenSubKey(@"Software\PSHandler"))
            {
                if (keyPsHandler == null)
                {
                    return; //TODO obsolete code
                }
            }

            // settings

            AppDataPath = GetString("AppDataPath", "");
            PokerStarsThemeTable = PokerStarsThemeTable.Parse(GetString("PokerStarsThemeTable", "Unknown"));
            MinimizeToSystemTray = GetBool("MinimizeToSystemTray", 0);
            StartMinimized = GetBool("StartMinimized", 0);
            HotkeyExit = KeyCombination.Parse(GetString("HotkeyExit", new KeyCombination(Key.None, false, false, false).ToString()));
            SaveGuiLocation = GetBool("SaveGuiLocation", 0);
            GuiLocationX = GetInt("GuiLocationX", 0);
            GuiLocationY = GetInt("GuiLocationY", 0);
            SaveGuiSize = GetBool("SaveGuiSize", 0);
            GuiWidth = GetInt("GuiWidth", 0);
            GuiHeight = GetInt("GuiHeight", 0);

            // controller

            AutoclickImBack = GetBool("AutoclickImBack", 0);
            AutoclickTimebank = GetBool("AutoclickTimebank", 0);
            AutoclickYesSeatAvailable = GetBool("AutoclickYesSeatAvailable", 0);
            AutocloseTournamentRegistrationPopups = GetBool("AutocloseTournamentRegistrationPopups", 0);
            AutocloseHM2ApplyToSimilarTablesPopups = GetBool("AutocloseHM2ApplyToSimilarTablesPopups", 0);
            HotkeyHandReplay = KeyCombination.Parse(GetString("HotkeyHandReplay", new KeyCombination(Key.None, false, false, false).ToString()));

            // hud

            EnableHud = GetBool("EnableHud", 0);
            HudManager.EnableHudTimer = GetBool("EnableHudTimer", 0);
            HudManager.EnableHudBigBlind = GetBool("EnableHudBigBlind", 0);
            TimerDiff = GetInt("TimerDiff", 0);
            TimerHHNotFound = GetString("TimerHHNotFound", "HH not found");
            TimerPokerTypeNotFound = GetString("TimerPokerTypeNotFound", "Poker Type not found");
            TimerMultiplePokerTypes = GetString("TimerMultiplePokerTypes", "Multiple Poker Types");
            BigBlindDecimals = GetInt("BigBlindDecimals", 0);

            HudManager.TimerHudLocationLocked = GetBool("TimerHudLocationLocked", 0);
            HudManager.SetTimerHudLocationX(GetFloat("TimerHudLocationX", HudManager.GetTimerHudLocationX(null).ToString(CultureInfo.InvariantCulture)), null);
            HudManager.SetTimerHudLocationY(GetFloat("TimerHudLocationY", HudManager.GetTimerHudLocationY(null).ToString(CultureInfo.InvariantCulture)), null);
            HudManager.TimerHudBackground = (Color)ColorConverter.ConvertFromString(GetString("TimerHudBackground", HudManager.TimerHudBackground.ToString(CultureInfo.InvariantCulture)));
            HudManager.TimerHudForeground = (Color)ColorConverter.ConvertFromString(GetString("TimerHudForeground", HudManager.TimerHudForeground.ToString(CultureInfo.InvariantCulture)));
            HudManager.TimerHudFontFamily = new FontFamily(GetString("TimerHudFontFamily", HudManager.TimerHudFontFamily.ToString()));
            HudManager.TimerHudFontWeight = (FontWeight)new FontWeightConverter().ConvertFrom(GetString("TimerHudFontWeight", HudManager.TimerHudFontWeight.ToString()));
            HudManager.TimerHudFontStyle = (FontStyle)new FontStyleConverter().ConvertFrom(GetString("TimerHudFontStyle", HudManager.TimerHudFontStyle.ToString()));
            HudManager.TimerHudFontSize = GetFloat("TimerHudFontSize", HudManager.TimerHudFontSize.ToString(CultureInfo.InvariantCulture));
            HudManager.BigBlindHudLocationLocked = GetBool("BigBlindHudLocationLocked", 0);
            HudManager.SetBigBlindHudLocationX(GetFloat("BigBlindHudLocationX", HudManager.GetBigBlindHudLocationX(null).ToString(CultureInfo.InvariantCulture)), null);
            HudManager.SetBigBlindHudLocationY(GetFloat("BigBlindHudLocationY", HudManager.GetBigBlindHudLocationY(null).ToString(CultureInfo.InvariantCulture)), null);
            HudManager.BigBlindHudBackground = (Color)ColorConverter.ConvertFromString(GetString("BigBlindHudBackground", HudManager.BigBlindHudBackground.ToString(CultureInfo.InvariantCulture)));
            HudManager.BigBlindHudForeground = (Color)ColorConverter.ConvertFromString(GetString("BigBlindHudForeground", HudManager.BigBlindHudForeground.ToString(CultureInfo.InvariantCulture)));
            HudManager.BigBlindHudFontFamily = new FontFamily(GetString("BigBlindHudFontFamily", HudManager.BigBlindHudFontFamily.ToString()));
            HudManager.BigBlindHudFontWeight = (FontWeight)new FontWeightConverter().ConvertFrom(GetString("BigBlindHudFontWeight", HudManager.BigBlindHudFontWeight.ToString()));
            HudManager.BigBlindHudFontStyle = (FontStyle)new FontStyleConverter().ConvertFrom(GetString("BigBlindHudFontStyle", HudManager.BigBlindHudFontStyle.ToString()));
            HudManager.BigBlindHudFontSize = GetFloat("BigBlindHudFontSize", HudManager.BigBlindHudFontSize.ToString(CultureInfo.InvariantCulture));

            // check invalid values
            if (HudManager.TimerHudFontSize < 1) HudManager.TimerHudFontSize = 1;
            if (HudManager.TimerHudFontSize > 72) HudManager.TimerHudFontSize = 72;

            // Poker Types

            PokerTypeManager.Load();
            PokerTypeManager.SeedDefaultValues();

            // TableTiler

            EnableTableTiler = GetBool("EnableTableTiler", 0);
            TableTileManager.LoadConfig();
            TableTileManager.SeedDefaultValues();
        }

        public static void Save()
        {
            try
            {
                bool needToBeDeleted = false;
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\PSHandler"))
                {
                    if (key != null)
                    {
                        needToBeDeleted = true;
                    }
                }
                if (needToBeDeleted)
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software", true))
                    {
                        if (key != null)
                        {
                            key.DeleteSubKeyTree("PsHandler");
                        }
                    }
                }
            }
            catch
            {
            }

            return;

            SetValue("Version", VERSION);

            // settings

            SetValue("AppDataPath", AppDataPath);
            SetValue("PokerStarsThemeTable", PokerStarsThemeTable.ToString());
            SetValue("MinimizeToSystemTray", MinimizeToSystemTray.ToInt());
            SetValue("StartMinimized", StartMinimized.ToInt());
            SetValue("HotkeyExit", HotkeyExit.ToString());
            SetValue("SaveGuiLocation", SaveGuiLocation.ToInt());
            SetValue("GuiLocationX", GuiLocationX);
            SetValue("GuiLocationY", GuiLocationY);
            SetValue("SaveGuiSize", SaveGuiSize.ToInt());
            SetValue("GuiWidth", GuiWidth);
            SetValue("GuiHeight", GuiHeight);

            // controller

            SetValue("AutoclickImBack", AutoclickImBack.ToInt());
            SetValue("AutoclickTimebank", AutoclickTimebank.ToInt());
            SetValue("AutoclickYesSeatAvailable", AutoclickYesSeatAvailable.ToInt());
            SetValue("AutocloseTournamentRegistrationPopups", AutocloseTournamentRegistrationPopups.ToInt());
            SetValue("AutocloseHM2ApplyToSimilarTablesPopups", AutocloseHM2ApplyToSimilarTablesPopups.ToInt());
            SetValue("HotkeyHandReplay", HotkeyHandReplay.ToString());

            // hud

            SetValue("EnableHud", EnableHud.ToInt());
            SetValue("EnableHudTimer", HudManager.EnableHudTimer.ToInt());
            SetValue("EnableHudBigBlind", HudManager.EnableHudBigBlind.ToInt());
            SetValue("TimerDiff", TimerDiff);
            SetValue("TimerHHNotFound", TimerHHNotFound);
            SetValue("TimerPokerTypeNotFound", TimerPokerTypeNotFound);
            SetValue("TimerMultiplePokerTypes", TimerMultiplePokerTypes);
            SetValue("BigBlindDecimals", BigBlindDecimals);

            SetValue("TimerHudLocationLocked", HudManager.TimerHudLocationLocked.ToInt());
            SetValue("TimerHudLocationX", HudManager.GetTimerHudLocationX(null).ToString(CultureInfo.InvariantCulture));
            SetValue("TimerHudLocationY", HudManager.GetTimerHudLocationY(null).ToString(CultureInfo.InvariantCulture));
            SetValue("TimerHudBackground", HudManager.TimerHudBackground.ToString(CultureInfo.InvariantCulture));
            SetValue("TimerHudForeground", HudManager.TimerHudForeground.ToString(CultureInfo.InvariantCulture));
            SetValue("TimerHudFontFamily", HudManager.TimerHudFontFamily.ToString());
            SetValue("TimerHudFontWeight", HudManager.TimerHudFontWeight.ToString());
            SetValue("TimerHudFontStyle", HudManager.TimerHudFontStyle.ToString());
            SetValue("TimerHudFontSize", HudManager.TimerHudFontSize.ToString(CultureInfo.InvariantCulture));
            SetValue("BigBlindHudLocationLocked", HudManager.BigBlindHudLocationLocked.ToInt());
            SetValue("BigBlindHudLocationX", HudManager.GetBigBlindHudLocationX(null).ToString(CultureInfo.InvariantCulture));
            SetValue("BigBlindHudLocationY", HudManager.GetBigBlindHudLocationY(null).ToString(CultureInfo.InvariantCulture));
            SetValue("BigBlindHudBackground", HudManager.BigBlindHudBackground.ToString(CultureInfo.InvariantCulture));
            SetValue("BigBlindHudForeground", HudManager.BigBlindHudForeground.ToString(CultureInfo.InvariantCulture));
            SetValue("BigBlindHudFontFamily", HudManager.BigBlindHudFontFamily.ToString());
            SetValue("BigBlindHudFontWeight", HudManager.BigBlindHudFontWeight.ToString());
            SetValue("BigBlindHudFontStyle", HudManager.BigBlindHudFontStyle.ToString());
            SetValue("BigBlindHudFontSize", HudManager.BigBlindHudFontSize.ToString(CultureInfo.InvariantCulture));

            // Poker Types

            PokerTypeManager.Save();

            // TableTiler

            SetValue("EnableTableTiler", EnableTableTiler.ToInt());
            TableTileManager.SaveConfig();
        }

        //

        private static void Set(XElement xElement, string name, object o, ref int errorCode, object defaultValue = null)
        {
            try
            {
                xElement.Add(new XElement(name, o != null ? o.ToString() : defaultValue.ToString()));
            }
            catch (Exception)
            {
                errorCode++;
            }
        }

        //

        private static bool GetBool(XElement xElement, string name, ref int errorCode, bool defaultValue = default(bool))
        {
            try
            {
                return bool.Parse(xElement.Element(name).Value);
            }
            catch (Exception)
            {
                errorCode += 1;
                return defaultValue;
            }
        }

        private static string GetString(XElement xElement, string name, ref int errorCode, string defaultValue = default(string))
        {
            try
            {
                return xElement.Element(name).Value;
            }
            catch (Exception)
            {
                errorCode += 1;
                return defaultValue;
            }
        }

        private static int GetInt(XElement xElement, string name, ref int errorCode, int defaultValue = default(int))
        {
            try
            {
                return int.Parse(xElement.Element(name).Value);
            }
            catch (Exception)
            {
                errorCode += 1;
                return defaultValue;
            }
        }

        private static float GetFloat(XElement xElement, string name, ref int errorCode, float defaultValue = default(float))
        {
            try
            {
                return float.Parse(xElement.Element(name).Value);
            }
            catch (Exception)
            {
                errorCode += 1;
                return defaultValue;
            }
        }

        private static Color GetColor(XElement xElement, string name, ref int errorCode, Color defaultValue = default(Color))
        {
            try
            {
                return (Color)ColorConverter.ConvertFromString(xElement.Element(name).Value);
            }
            catch (Exception)
            {
                errorCode += 1;
                return defaultValue;
            }
        }

        private static FontFamily GetFontFamily(XElement xElement, string name, ref int errorCode, FontFamily defaultValue = default(FontFamily))
        {
            try
            {
                return new FontFamily(xElement.Element(name).Value);
            }
            catch (Exception)
            {
                errorCode += 1;
                return defaultValue;
            }
        }

        private static FontWeight GetFontWeight(XElement xElement, string name, ref int errorCode, FontWeight defaultValue = default(FontWeight))
        {
            try
            {
                return (FontWeight)new FontWeightConverter().ConvertFrom(xElement.Element(name).Value);
            }
            catch (Exception)
            {
                errorCode += 1;
                return defaultValue;
            }
        }

        private static FontStyle GetFontStyle(XElement xElement, string name, ref int errorCode, FontStyle defaultValue = default(FontStyle))
        {
            try
            {
                return (FontStyle)new FontStyleConverter().ConvertFrom(xElement.Element(name).Value);
            }
            catch (Exception)
            {
                errorCode += 1;
                return defaultValue;
            }
        }

        //

        public static int SaveXml()
        {
            int errors = 0;
            try
            {
                XDocument xDoc = new XDocument();
                XElement root = new XElement("Config");
                xDoc.Add(root);

                //Set(root, "", );

                Set(root, "Version", VERSION, ref errors);

                //settings

                XElement xAppDataPaths = new XElement("AppDataPaths");
                root.Add(xAppDataPaths);
                Set(xAppDataPaths, "AppDataPath", AppDataPath, ref errors, "");

                Set(root, "PokerStarsThemeTable", PokerStarsThemeTable, ref errors, new PokerStarsThemesTable.Unknown());
                Set(root, "MinimizeToSystemTray", MinimizeToSystemTray, ref errors);
                Set(root, "StartMinimized", StartMinimized, ref errors);
                Set(root, "HotkeyExit", HotkeyExit, ref errors, KeyCombination.Parse(null));
                Set(root, "SaveGuiLocation", SaveGuiLocation, ref errors);
                Set(root, "GuiLocationX", GuiLocationX, ref errors);
                Set(root, "GuiLocationY", GuiLocationY, ref errors);
                Set(root, "SaveGuiSize", SaveGuiSize, ref errors);
                Set(root, "GuiWidth", GuiWidth, ref errors);
                Set(root, "GuiHeight", GuiHeight, ref errors);

                // controller

                Set(root, "AutoclickImBack", AutoclickImBack, ref errors);
                Set(root, "AutoclickTimebank", AutoclickTimebank, ref errors);
                Set(root, "AutoclickYesSeatAvailable", AutoclickYesSeatAvailable, ref errors);
                Set(root, "AutocloseTournamentRegistrationPopups", AutocloseTournamentRegistrationPopups, ref errors);
                Set(root, "AutocloseHM2ApplyToSimilarTablesPopups", AutocloseHM2ApplyToSimilarTablesPopups, ref errors);
                Set(root, "HotkeyHandReplay", HotkeyHandReplay, ref errors, KeyCombination.Parse(null));

                // hud

                Set(root, "EnableHud", EnableHud, ref errors);
                Set(root, "EnableHudTimer", HudManager.EnableHudTimer, ref errors);
                Set(root, "EnableHudBigBlind", HudManager.EnableHudBigBlind, ref errors);
                Set(root, "TimerDiff", TimerDiff, ref errors);
                Set(root, "TimerHHNotFound", TimerHHNotFound, ref errors, "");
                Set(root, "TimerPokerTypeNotFound", TimerPokerTypeNotFound, ref errors, "");
                Set(root, "TimerMultiplePokerTypes", TimerMultiplePokerTypes, ref errors, "");
                Set(root, "BigBlindDecimals", BigBlindDecimals, ref errors);

                Set(root, "TimerHudLocationLocked", HudManager.TimerHudLocationLocked, ref errors);
                Set(root, "TimerHudLocationX", HudManager.GetTimerHudLocationX(null), ref errors);
                Set(root, "TimerHudLocationY", HudManager.GetTimerHudLocationY(null), ref errors);
                Set(root, "TimerHudBackground", HudManager.TimerHudBackground, ref errors);
                Set(root, "TimerHudForeground", HudManager.TimerHudForeground, ref errors);
                Set(root, "TimerHudFontFamily", HudManager.TimerHudFontFamily, ref errors);
                Set(root, "TimerHudFontWeight", HudManager.TimerHudFontWeight, ref errors);
                Set(root, "TimerHudFontStyle", HudManager.TimerHudFontStyle, ref errors);
                Set(root, "TimerHudFontSize", HudManager.TimerHudFontSize, ref errors);
                Set(root, "BigBlindHudLocationLocked", HudManager.BigBlindHudLocationLocked, ref errors);
                Set(root, "BigBlindHudLocationX", HudManager.GetBigBlindHudLocationX(null), ref errors);
                Set(root, "BigBlindHudLocationY", HudManager.GetBigBlindHudLocationY(null), ref errors);
                Set(root, "BigBlindHudBackground", HudManager.BigBlindHudBackground, ref errors);
                Set(root, "BigBlindHudForeground", HudManager.BigBlindHudForeground, ref errors);
                Set(root, "BigBlindHudFontFamily", HudManager.BigBlindHudFontFamily, ref errors);
                Set(root, "BigBlindHudFontWeight", HudManager.BigBlindHudFontWeight, ref errors);
                Set(root, "BigBlindHudFontStyle", HudManager.BigBlindHudFontStyle, ref errors);
                Set(root, "BigBlindHudFontSize", HudManager.BigBlindHudFontSize, ref errors);

                // Poker Types

                PokerTypeManager.Save();

                XElement xPokerTypes = new XElement("PokerTypes");
                root.Add(xPokerTypes);
                foreach (PokerType pokerType in PokerTypeManager.PokerTypes)
                {
                    xPokerTypes.Add(pokerType.ToXElement());
                }

                // TableTiler

                Set(root, "EnableTableTiler", EnableTableTiler, ref errors);

                XElement xTableTiles = new XElement("TableTiles");
                root.Add(xTableTiles);
                foreach (var tableTile in TableTileManager.GetTableTilesCopy())
                {
                    xTableTiles.Add(tableTile.ToXElement());
                }

                //

                xDoc.Save("pshandler.xml");
            }
            catch (Exception)
            {
                errors++;
            }

            if (errors != 0)
            {
                App.TaskbarIcon.ShowBalloonTip("Error Saving Config XML", "Some configurations weren't saved. Contact support." + Environment.NewLine + "(Program will continue after 10 seconds)", BalloonIcon.Error);
                Thread.Sleep(10000);
            }

            return errors;
        }

        public static int LoadXml()
        {
            int errors = 0;
            try
            {
                XDocument xDoc = XDocument.Load("pshandler.xml");
                XElement root = xDoc.Element("Config");

                foreach (XElement xAppDataPath in root.Elements("AppDataPaths").SelectMany(o => o.Elements("AppDataPath")))
                {
                    AppDataPath = xAppDataPath.Value;
                }

                PokerStarsThemeTable = PokerStarsThemeTable.Parse(GetString(root, "PokerStarsThemeTable", ref errors));
                MinimizeToSystemTray = GetBool(root, "MinimizeToSystemTray", ref errors);
                StartMinimized = GetBool(root, "StartMinimized", ref errors);
                HotkeyExit = KeyCombination.Parse(GetString(root, "HotkeyExit", ref errors));
                SaveGuiLocation = GetBool(root, "SaveGuiLocation", ref errors);
                GuiLocationX = GetInt(root, "GuiLocationX", ref errors);
                GuiLocationY = GetInt(root, "GuiLocationY", ref errors);
                SaveGuiSize = GetBool(root, "SaveGuiSize", ref errors);
                GuiWidth = GetInt(root, "GuiWidth", ref errors);
                GuiHeight = GetInt(root, "GuiHeight", ref errors);

                // controller

                AutoclickImBack = GetBool(root, "AutoclickImBack", ref errors);
                AutoclickTimebank = GetBool(root, "AutoclickTimebank", ref errors);
                AutoclickYesSeatAvailable = GetBool(root, "AutoclickYesSeatAvailable", ref errors);
                AutocloseTournamentRegistrationPopups = GetBool(root, "AutocloseTournamentRegistrationPopups", ref errors);
                AutocloseHM2ApplyToSimilarTablesPopups = GetBool(root, "AutocloseHM2ApplyToSimilarTablesPopups", ref errors);
                HotkeyHandReplay = KeyCombination.Parse(GetString(root, "HotkeyHandReplay", ref errors));

                // hud

                EnableHud = GetBool(root, "EnableHud", ref errors);
                HudManager.EnableHudTimer = GetBool(root, "EnableHudTimer", ref errors);
                HudManager.EnableHudBigBlind = GetBool(root, "EnableHudBigBlind", ref errors);
                TimerDiff = GetInt(root, "TimerDiff", ref errors);
                TimerHHNotFound = GetString(root, "TimerHHNotFound", ref errors, "HH not found");
                TimerPokerTypeNotFound = GetString(root, "TimerPokerTypeNotFound", ref errors, "Poker Type not found");
                TimerMultiplePokerTypes = GetString(root, "TimerMultiplePokerTypes", ref errors, "Multiple Poker Types");
                BigBlindDecimals = GetInt(root, "BigBlindDecimals", ref errors);


                HudManager.TimerHudLocationLocked = GetBool(root, "TimerHudLocationLocked", ref errors);
                HudManager.SetTimerHudLocationX(GetFloat(root, "TimerHudLocationX", ref errors), null);
                HudManager.SetTimerHudLocationY(GetFloat(root, "TimerHudLocationY", ref errors), null);
                HudManager.TimerHudBackground = GetColor(root, "TimerHudBackground", ref errors, Colors.Black);
                HudManager.TimerHudForeground = GetColor(root, "TimerHudForeground", ref errors, Colors.White);
                HudManager.TimerHudFontFamily = GetFontFamily(root, "TimerHudFontFamily", ref errors, new FontFamily("Consolas"));
                HudManager.TimerHudFontWeight = GetFontWeight(root, "TimerHudFontWeight", ref errors);
                HudManager.TimerHudFontStyle = GetFontStyle(root, "TimerHudFontStyle", ref errors);
                HudManager.TimerHudFontSize = GetFloat(root, "TimerHudFontSize", ref errors, 10);
                if (HudManager.TimerHudFontSize < 1) HudManager.TimerHudFontSize = 1;
                HudManager.BigBlindHudLocationLocked = GetBool(root, "BigBlindHudLocationLocked", ref errors);
                HudManager.SetBigBlindHudLocationX(GetFloat(root, "BigBlindHudLocationX", ref errors), null);
                HudManager.SetBigBlindHudLocationY(GetFloat(root, "BigBlindHudLocationY", ref errors), null);
                HudManager.BigBlindHudBackground = GetColor(root, "BigBlindHudBackground", ref errors, Colors.Black);
                HudManager.BigBlindHudForeground = GetColor(root, "BigBlindHudForeground", ref errors, Colors.White);
                HudManager.BigBlindHudFontFamily = GetFontFamily(root, "BigBlindHudFontFamily", ref errors, new FontFamily("Consolas"));
                HudManager.BigBlindHudFontWeight = GetFontWeight(root, "BigBlindHudFontWeight", ref errors);
                HudManager.BigBlindHudFontStyle = GetFontStyle(root, "BigBlindHudFontStyle", ref errors);
                HudManager.BigBlindHudFontSize = GetFloat(root, "BigBlindHudFontSize", ref errors, 10);
                if (HudManager.TimerHudFontSize > 72) HudManager.TimerHudFontSize = 72;

                // Poker Types

                foreach (XElement xElement in root.Elements("PokerTypes"))
                {
                    PokerTypeManager.Add(xElement.Elements("PokerType").Select(PokerType.FromXElement).Where(o => o != null));
                }

                // TableTiler

                EnableTableTiler = GetBool(root, "EnableTableTiler", ref errors);

                foreach (XElement xElement in root.Elements("TableTiles"))
                {
                    TableTileManager.Add(xElement.Elements("TableTile").Select(TableTile.FromXElement).Where(o => o != null));
                }

            }
            catch (Exception)
            {
                errors++;
            }

            PokerTypeManager.SeedDefaultValues();
            TableTileManager.SeedDefaultValues();

            return errors;
        }
    }
}

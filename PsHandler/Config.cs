using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Threading;
using PsHandler.Hook;
using PsHandler.Hud;
using PsHandler.PokerTypes;
using PsHandler.Randomizer;
using PsHandler.TableTiler;

namespace PsHandler
{
    public class Config
    {
        // Constants

        public const string NAME = "PsHandler";
        public const int VERSION = 17;
        public const string UPDATE_HREF = "http://chainer.projektas.in/PsHandler/update.php";
        public static string MACHINE_GUID = GetMachineGuid();
        public static string CONFIG_FILENAME = "pshandler.xml";
        public static int WINDOWS_BORDER_THICKNESS = WinApi.GetSystemMetrics(WinApi.SystemMetric.SM_CXSIZEFRAME) * 2;
        public static int WINDOWS_TITLE_BORDER_THICKNESS = WinApi.GetSystemMetrics(WinApi.SystemMetric.SM_CYCAPTION);
        public static System.Drawing.Size POKERSTARS_TABLE_CLIENT_SIZE_MIN = new System.Drawing.Size(475, 327);
        public static System.Drawing.Size POKERSTARS_TABLE_CLIENT_SIZE_DEFAULT = new System.Drawing.Size(792, 546);

        // Settings

        public static List<string> ImportFolders = new List<string>();
        public static PokerStarsThemeTable PokerStarsThemeTable = new PokerStarsThemesTable.Unknown();
        public static bool MinimizeToSystemTray = false;
        public static bool StartMinimized = false;
        public static KeyCombination HotkeyExit = new KeyCombination(Key.None, false, false, false);
        public static KeyCombination HotkeyQuickPreview = new KeyCombination(Key.None, false, false, false);
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
        //
        public static bool EnableCustomTablesWindowStyle = false;
        public enum TableWindowStyle { NoCaption, Borderless };
        public static TableWindowStyle CustomTablesWindowStyle = TableWindowStyle.Borderless;

        // HUD

        public static bool EnableHud = false;
        public static int TimerDiff = 0;
        public static string TimerHHNotFound = "HH not found";
        public static string TimerPokerTypeNotFound = "Poker Type not found";
        public static string TimerMultiplePokerTypes = "Multiple Poker Types";
        public static int BigBlindDecimals = 0;
        public static string BigBlindHHNotFound = "X";
        public static string BigBlindPrefix = "";
        public static string BigBlindPostfix = "";
        public static bool BigBlindShowTournamentM = false;
        public static bool BigBlindMByPlayerCount = true;
        public static bool BigBlindMByTableSize = false;

        // HUD design

        public static Color HudTimerBackground = Colors.Black;
        public static Color HudTimerForeground = Colors.White;
        public static FontFamily HudTimerFontFamily = new FontFamily("Consolas");
        public static FontWeight HudTimerFontWeight = FontWeights.Bold;
        public static FontStyle HudTimerFontStyle = FontStyles.Normal;
        public static double HudTimerFontSize = 15;
        public static Thickness HudTimerMargin = new Thickness(2, 2, 2, 2);

        public static Color HudBigBlindBackground = Colors.Transparent;
        public static Color HudBigBlindForeground = Colors.RoyalBlue;
        public static FontFamily HudBigBlindFontFamily = new FontFamily("Consolas");
        public static FontWeight HudBigBlindFontWeight = FontWeights.Bold;
        public static FontStyle HudBigBlindFontStyle = FontStyles.Normal;
        public static double HudBigBlindFontSize = 25;
        public static Thickness HudBigBlindMargin = new Thickness(2, 2, 2, 2);
        public static List<ColorByValue> HudBigBlindColorsByValue = new List<ColorByValue>();

        // Table Tiler

        public static bool EnableTableTiler = false;

        // Randomizer

        public static bool EnableRandomizer;
        public static KeyCombination HotkeyRandomizerChance10 = new KeyCombination(Key.NumPad1, true, false, false);
        public static KeyCombination HotkeyRandomizerChance20 = new KeyCombination(Key.NumPad2, true, false, false);
        public static KeyCombination HotkeyRandomizerChance30 = new KeyCombination(Key.NumPad3, true, false, false);
        public static KeyCombination HotkeyRandomizerChance40 = new KeyCombination(Key.NumPad4, true, false, false);
        public static KeyCombination HotkeyRandomizerChance50 = new KeyCombination(Key.NumPad5, true, false, false);
        public static KeyCombination HotkeyRandomizerChance60 = new KeyCombination(Key.NumPad6, true, false, false);
        public static KeyCombination HotkeyRandomizerChance70 = new KeyCombination(Key.NumPad7, true, false, false);
        public static KeyCombination HotkeyRandomizerChance80 = new KeyCombination(Key.NumPad8, true, false, false);
        public static KeyCombination HotkeyRandomizerChance90 = new KeyCombination(Key.NumPad9, true, false, false);
        public static int RandomizerChance10 = 10;
        public static int RandomizerChance20 = 20;
        public static int RandomizerChance30 = 30;
        public static int RandomizerChance40 = 40;
        public static int RandomizerChance50 = 50;
        public static int RandomizerChance60 = 60;
        public static int RandomizerChance70 = 70;
        public static int RandomizerChance80 = 80;
        public static int RandomizerChance90 = 90;

        //

        #region Get/Set xml

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

        private static Thickness GetThickness(XElement xElement, string name, ref int errorCode, Thickness defaultValue = default(Thickness))
        {
            try
            {
                var split = xElement.Element(name).Value.Split(new[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                return new Thickness(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]), int.Parse(split[3]));
            }
            catch (Exception)
            {
                errorCode += 1;
                return defaultValue;
            }
        }

        #endregion

        //

        public static int LoadXml()
        {
            int errors = 0;
            try
            {
                XDocument xDoc = XDocument.Load(CONFIG_FILENAME);
                XElement root = xDoc.Element("Config");

                int version = int.Parse(root.Element("Version").Value);
                if (version < 17)
                {
                    System.IO.File.Move("pshandler.xml", "pshandler" + version + ".xml");
                    throw new NotSupportedException("Old pshandler.xml config.");
                }

                // Version

                VersionControl(root);

                #region Settings

                MinimizeToSystemTray = GetBool(root, "MinimizeToSystemTray", ref errors, false);
                StartMinimized = GetBool(root, "StartMinimized", ref errors, false);
                SaveGuiLocation = GetBool(root, "SaveGuiLocation", ref errors, false);
                GuiLocationX = GetInt(root, "GuiLocationX", ref errors);
                GuiLocationY = GetInt(root, "GuiLocationY", ref errors);
                SaveGuiSize = GetBool(root, "SaveGuiSize", ref errors, false);
                GuiWidth = GetInt(root, "GuiWidth", ref errors);
                GuiHeight = GetInt(root, "GuiHeight", ref errors);
                HotkeyExit = KeyCombination.Parse(GetString(root, "HotkeyExit", ref errors));
                PokerStarsThemeTable = PokerStarsThemeTable.Parse(GetString(root, "PokerStarsThemeTable", ref errors, new PokerStarsThemesTable.Unknown().ToString()));
                foreach (XElement xImportFolderPath in root.Elements("ImportFolderPaths").SelectMany(o => o.Elements("ImportFolderPath"))) if (!String.IsNullOrEmpty(xImportFolderPath.Value)) ImportFolders.Add(xImportFolderPath.Value);

                #endregion

                #region Controller

                AutoclickImBack = GetBool(root, "AutoclickImBack", ref errors, false);
                AutoclickTimebank = GetBool(root, "AutoclickTimebank", ref errors, false);
                AutoclickYesSeatAvailable = GetBool(root, "AutoclickYesSeatAvailable", ref errors, false);
                AutocloseTournamentRegistrationPopups = GetBool(root, "AutocloseTournamentRegistrationPopups", ref errors, false);
                AutocloseHM2ApplyToSimilarTablesPopups = GetBool(root, "AutocloseHM2ApplyToSimilarTablesPopups", ref errors, false);
                HotkeyHandReplay = KeyCombination.Parse(GetString(root, "HotkeyHandReplay", ref errors));
                HotkeyQuickPreview = KeyCombination.Parse(GetString(root, "HotkeyQuickPreview", ref errors));
                EnableCustomTablesWindowStyle = GetBool(root, "EnableCustomTablesWindowStyle", ref errors, false);
                string tableWindowStyleStr = GetString(root, "CustomTablesWindowStyle", ref errors, TableWindowStyle.Borderless.ToString());
                foreach (TableWindowStyle tableWindowStyle in Enum.GetValues(typeof(TableWindowStyle)).Cast<TableWindowStyle>().Where(tableWindowStyle => tableWindowStyle.ToString().Equals(tableWindowStyleStr)))
                {
                    CustomTablesWindowStyle = tableWindowStyle;
                    break;
                }

                #endregion

                #region Randomizer

                EnableRandomizer = GetBool(root, "EnableRandomizer", ref errors, false);
                RandomizerChance10 = GetInt(root, "RandomizerChance10", ref errors, 10);
                RandomizerChance20 = GetInt(root, "RandomizerChance20", ref errors, 20);
                RandomizerChance30 = GetInt(root, "RandomizerChance30", ref errors, 30);
                RandomizerChance40 = GetInt(root, "RandomizerChance40", ref errors, 40);
                RandomizerChance50 = GetInt(root, "RandomizerChance50", ref errors, 50);
                RandomizerChance60 = GetInt(root, "RandomizerChance60", ref errors, 60);
                RandomizerChance70 = GetInt(root, "RandomizerChance70", ref errors, 70);
                RandomizerChance80 = GetInt(root, "RandomizerChance80", ref errors, 80);
                RandomizerChance90 = GetInt(root, "RandomizerChance90", ref errors, 90);
                HotkeyRandomizerChance10 = KeyCombination.Parse(GetString(root, "HotkeyRandomizerChance10", ref errors, new KeyCombination(Key.NumPad1, true, false, false).ToString()));
                HotkeyRandomizerChance20 = KeyCombination.Parse(GetString(root, "HotkeyRandomizerChance20", ref errors, new KeyCombination(Key.NumPad2, true, false, false).ToString()));
                HotkeyRandomizerChance30 = KeyCombination.Parse(GetString(root, "HotkeyRandomizerChance30", ref errors, new KeyCombination(Key.NumPad3, true, false, false).ToString()));
                HotkeyRandomizerChance40 = KeyCombination.Parse(GetString(root, "HotkeyRandomizerChance40", ref errors, new KeyCombination(Key.NumPad4, true, false, false).ToString()));
                HotkeyRandomizerChance50 = KeyCombination.Parse(GetString(root, "HotkeyRandomizerChance50", ref errors, new KeyCombination(Key.NumPad5, true, false, false).ToString()));
                HotkeyRandomizerChance60 = KeyCombination.Parse(GetString(root, "HotkeyRandomizerChance60", ref errors, new KeyCombination(Key.NumPad6, true, false, false).ToString()));
                HotkeyRandomizerChance70 = KeyCombination.Parse(GetString(root, "HotkeyRandomizerChance70", ref errors, new KeyCombination(Key.NumPad7, true, false, false).ToString()));
                HotkeyRandomizerChance80 = KeyCombination.Parse(GetString(root, "HotkeyRandomizerChance80", ref errors, new KeyCombination(Key.NumPad8, true, false, false).ToString()));
                HotkeyRandomizerChance90 = KeyCombination.Parse(GetString(root, "HotkeyRandomizerChance90", ref errors, new KeyCombination(Key.NumPad9, true, false, false).ToString()));

                #endregion

                #region Hud

                EnableHud = GetBool(root, "EnableHud", ref errors);

                TableManager.EnableHudTimer = GetBool(root, "EnableHudTimer", ref errors, false);
                TimerDiff = GetInt(root, "TimerDiff", ref errors, 0);
                TimerHHNotFound = GetString(root, "TimerHHNotFound", ref errors, "HH not found");
                TimerPokerTypeNotFound = GetString(root, "TimerPokerTypeNotFound", ref errors, "Poker Type not found");
                TimerMultiplePokerTypes = GetString(root, "TimerMultiplePokerTypes", ref errors, "Multiple Poker Types");

                TableManager.EnableHudBigBlind = GetBool(root, "EnableHudBigBlind", ref errors, false);
                BigBlindShowTournamentM = GetBool(root, "BigBlindShowTournamentM", ref errors, false);
                BigBlindMByPlayerCount = GetBool(root, "BigBlindMByPlayerCount", ref errors, true);
                BigBlindMByTableSize = GetBool(root, "BigBlindMByTableSize", ref errors, false);
                BigBlindDecimals = GetInt(root, "BigBlindDecimals", ref errors, 0);
                BigBlindHHNotFound = GetString(root, "BigBlindHHNotFound", ref errors, "X");
                BigBlindPrefix = GetString(root, "BigBlindPrefix", ref errors, "");
                BigBlindPostfix = GetString(root, "BigBlindPostfix", ref errors, "");

                #endregion

                #region Hud Design

                HudTimerBackground = GetColor(root, "HudTimerBackground", ref errors, Colors.Black);
                HudTimerForeground = GetColor(root, "HudTimerForeground", ref errors, Colors.White);
                HudTimerFontFamily = GetFontFamily(root, "HudTimerFontFamily", ref errors, new FontFamily("Consolas"));
                HudTimerFontWeight = GetFontWeight(root, "HudTimerFontWeight", ref errors, FontWeights.Normal);
                HudTimerFontStyle = GetFontStyle(root, "HudTimerFontStyle", ref errors, FontStyles.Normal);
                HudTimerFontSize = GetFloat(root, "HudTimerFontSize", ref errors, 15);
                HudTimerMargin = GetThickness(root, "HudTimerMargin", ref errors, new Thickness(2, 2, 2, 2));

                HudBigBlindBackground = GetColor(root, "HudBigBlindBackground", ref errors, Colors.Black);
                HudBigBlindForeground = GetColor(root, "HudBigBlindForeground", ref errors, Colors.White);
                HudBigBlindFontFamily = GetFontFamily(root, "HudBigBlindFontFamily", ref errors, new FontFamily("Consolas"));
                HudBigBlindFontWeight = GetFontWeight(root, "HudBigBlindFontWeight", ref errors, FontWeights.Normal);
                HudBigBlindFontStyle = GetFontStyle(root, "HudBigBlindFontStyle", ref errors, FontStyles.Normal);
                HudBigBlindFontSize = GetFloat(root, "HudBigBlindFontSize", ref errors, 25);
                HudBigBlindMargin = GetThickness(root, "HudBigBlindMargin", ref errors, new Thickness(2, 2, 2, 2));
                foreach (XElement xElement in root.Elements("HudBigBlindColorsByValue")) HudBigBlindColorsByValue.AddRange(xElement.Elements("ColorByValue").Select(ColorByValue.FromXElement).Where(o => o != null));

                #endregion

                TableManager.HudTimerLocationLocked = GetBool(root, "HudTimerLocationLocked", ref errors, false);
                TableManager.FromXElementHudTimerLocations(root.Element("HudTimerLocations"), ref errors);
                TableManager.HudBigBlindLocationLocked = GetBool(root, "HudBigBlindLocationLocked", ref errors, false);
                TableManager.FromXElementHudBigBlindLocations(root.Element("HudBigBlindLocations"), ref errors);
                EnableTableTiler = GetBool(root, "EnableTableTiler", ref errors, false);
                TableTileManager.FromXElement(root.Element("TableTiles"));
                PokerTypeManager.FromXElement(root.Element("PokerTypes"));
            }
            catch (Exception)
            {
                errors++;
            }

            TableTileManager.SeedDefaultValues();
            PokerTypeManager.SeedDefaultValues();
            RandomizerManager.SeedDefaultValues();

            return errors;
        }

        public static int SaveXml()
        {
            int errors = 0;
            try
            {
                XDocument xDoc = new XDocument();
                XElement root = new XElement("Config");
                xDoc.Add(root);

                // Version

                Set(root, "Version", VERSION, ref errors);

                #region Settings

                Set(root, "MinimizeToSystemTray", MinimizeToSystemTray, ref errors);
                Set(root, "StartMinimized", StartMinimized, ref errors);
                Set(root, "SaveGuiLocation", SaveGuiLocation, ref errors);
                Set(root, "GuiLocationX", GuiLocationX, ref errors);
                Set(root, "GuiLocationY", GuiLocationY, ref errors);
                Set(root, "SaveGuiSize", SaveGuiSize, ref errors);
                Set(root, "GuiWidth", GuiWidth, ref errors);
                Set(root, "GuiHeight", GuiHeight, ref errors);
                Set(root, "HotkeyExit", HotkeyExit, ref errors, KeyCombination.Parse(null));
                Set(root, "PokerStarsThemeTable", PokerStarsThemeTable, ref errors, new PokerStarsThemesTable.Unknown());
                XElement xImportFolderPaths = new XElement("ImportFolderPaths"); root.Add(xImportFolderPaths); foreach (var path in ImportFolders.ToArray()) Set(xImportFolderPaths, "ImportFolderPath", path, ref errors, "");

                #endregion

                #region Controller

                Set(root, "AutoclickImBack", AutoclickImBack, ref errors);
                Set(root, "AutoclickTimebank", AutoclickTimebank, ref errors);
                Set(root, "AutoclickYesSeatAvailable", AutoclickYesSeatAvailable, ref errors);
                Set(root, "AutocloseTournamentRegistrationPopups", AutocloseTournamentRegistrationPopups, ref errors);
                Set(root, "AutocloseHM2ApplyToSimilarTablesPopups", AutocloseHM2ApplyToSimilarTablesPopups, ref errors);
                Set(root, "HotkeyHandReplay", HotkeyHandReplay, ref errors, KeyCombination.Parse(null));
                Set(root, "HotkeyQuickPreview", HotkeyQuickPreview, ref errors, KeyCombination.Parse(null));
                Set(root, "EnableCustomTablesWindowStyle", EnableCustomTablesWindowStyle, ref errors);
                Set(root, "CustomTablesWindowStyle", CustomTablesWindowStyle, ref errors);

                #endregion

                #region Randomizer

                Set(root, "EnableRandomizer", EnableRandomizer, ref errors);
                Set(root, "RandomizerChance10", RandomizerChance10, ref errors);
                Set(root, "RandomizerChance20", RandomizerChance20, ref errors);
                Set(root, "RandomizerChance30", RandomizerChance30, ref errors);
                Set(root, "RandomizerChance40", RandomizerChance40, ref errors);
                Set(root, "RandomizerChance50", RandomizerChance50, ref errors);
                Set(root, "RandomizerChance60", RandomizerChance60, ref errors);
                Set(root, "RandomizerChance70", RandomizerChance70, ref errors);
                Set(root, "RandomizerChance80", RandomizerChance80, ref errors);
                Set(root, "RandomizerChance90", RandomizerChance90, ref errors);
                Set(root, "HotkeyRandomizerChance10", HotkeyRandomizerChance10, ref errors, KeyCombination.Parse(null));
                Set(root, "HotkeyRandomizerChance20", HotkeyRandomizerChance20, ref errors, KeyCombination.Parse(null));
                Set(root, "HotkeyRandomizerChance30", HotkeyRandomizerChance30, ref errors, KeyCombination.Parse(null));
                Set(root, "HotkeyRandomizerChance40", HotkeyRandomizerChance40, ref errors, KeyCombination.Parse(null));
                Set(root, "HotkeyRandomizerChance50", HotkeyRandomizerChance50, ref errors, KeyCombination.Parse(null));
                Set(root, "HotkeyRandomizerChance60", HotkeyRandomizerChance60, ref errors, KeyCombination.Parse(null));
                Set(root, "HotkeyRandomizerChance70", HotkeyRandomizerChance70, ref errors, KeyCombination.Parse(null));
                Set(root, "HotkeyRandomizerChance80", HotkeyRandomizerChance80, ref errors, KeyCombination.Parse(null));
                Set(root, "HotkeyRandomizerChance90", HotkeyRandomizerChance90, ref errors, KeyCombination.Parse(null));

                #endregion

                #region Hud

                Set(root, "EnableHud", EnableHud, ref errors);

                Set(root, "EnableHudTimer", TableManager.EnableHudTimer, ref errors);
                Set(root, "TimerDiff", TimerDiff, ref errors);
                Set(root, "TimerHHNotFound", TimerHHNotFound, ref errors, "");
                Set(root, "TimerPokerTypeNotFound", TimerPokerTypeNotFound, ref errors, "");
                Set(root, "TimerMultiplePokerTypes", TimerMultiplePokerTypes, ref errors, "");

                Set(root, "EnableHudBigBlind", TableManager.EnableHudBigBlind, ref errors);
                Set(root, "BigBlindShowTournamentM", BigBlindShowTournamentM, ref errors);
                Set(root, "BigBlindMByPlayerCount", BigBlindMByPlayerCount, ref errors);
                Set(root, "BigBlindMByTableSize", BigBlindMByTableSize, ref errors);
                Set(root, "BigBlindDecimals", BigBlindDecimals, ref errors);
                Set(root, "BigBlindHHNotFound", BigBlindHHNotFound, ref errors);
                Set(root, "BigBlindPrefix", BigBlindPrefix, ref errors);
                Set(root, "BigBlindPostfix", BigBlindPostfix, ref errors);

                #endregion

                #region Hud Design

                Set(root, "HudTimerBackground", HudTimerBackground, ref errors);
                Set(root, "HudTimerForeground", HudTimerForeground, ref errors);
                Set(root, "HudTimerFontFamily", HudTimerFontFamily, ref errors);
                Set(root, "HudTimerFontWeight", HudTimerFontWeight, ref errors);
                Set(root, "HudTimerFontStyle", HudTimerFontStyle, ref errors);
                Set(root, "HudTimerFontSize", HudTimerFontSize, ref errors);
                Set(root, "HudTimerMargin", HudTimerMargin, ref errors);

                Set(root, "HudBigBlindBackground", HudBigBlindBackground, ref errors);
                Set(root, "HudBigBlindForeground", HudBigBlindForeground, ref errors);
                Set(root, "HudBigBlindFontFamily", HudBigBlindFontFamily, ref errors);
                Set(root, "HudBigBlindFontWeight", HudBigBlindFontWeight, ref errors);
                Set(root, "HudBigBlindFontStyle", HudBigBlindFontStyle, ref errors);
                Set(root, "HudBigBlindFontSize", HudBigBlindFontSize, ref errors);
                Set(root, "HudBigBlindMargin", HudBigBlindMargin, ref errors);
                XElement xBigBlindColorsByValue = new XElement("HudBigBlindColorsByValue"); root.Add(xBigBlindColorsByValue); foreach (var item in HudBigBlindColorsByValue) xBigBlindColorsByValue.Add(item.ToXElement());

                #endregion

                Set(root, "HudTimerLocationLocked", TableManager.HudTimerLocationLocked, ref errors);
                root.Add(TableManager.ToXElementHudTimerLocations());
                Set(root, "HudBigBlindLocationLocked", TableManager.HudBigBlindLocationLocked, ref errors);
                root.Add(TableManager.ToXElementHudBigBlindLocations());
                Set(root, "EnableTableTiler", EnableTableTiler, ref errors);
                root.Add(TableTileManager.ToXElement());
                root.Add(PokerTypeManager.ToXElement());

                //

                #region Handle hidden files

                FileInfo fi = new FileInfo(CONFIG_FILENAME);
                bool isHidden = false;
                if (fi.Exists)
                {
                    isHidden = ((File.GetAttributes(CONFIG_FILENAME) & FileAttributes.Hidden) == FileAttributes.Hidden);
                    if (isHidden)
                    {
                        fi.Attributes &= ~FileAttributes.Hidden; // remove the hidden attribute from the file
                    }
                }
                xDoc.Save(CONFIG_FILENAME);
                if (isHidden)
                {
                    fi.Attributes |= FileAttributes.Hidden; // set the file as hidden
                }

                #endregion
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                errors++;
            }

            if (errors != 0)
            {
                App.TaskbarIcon.ShowBalloonTip("Error Saving Config XML", "Some configurations weren't saved. Contact support." + Environment.NewLine + "(Program will continue after 5 seconds)", BalloonIcon.Error);
                Thread.Sleep(5000);
            }

            return errors;
        }

        public static void VersionControl(XElement root)
        {
            // AppDataPaths -> ImportFolderPaths (v1.15 -> v1.16)

            foreach (XElement xAppDataPath in root.Elements("AppDataPaths").SelectMany(o => o.Elements("AppDataPath")))
            {
                if (!String.IsNullOrEmpty(xAppDataPath.Value))
                {
                    DirectoryInfo dirPokerStarsAppData = new DirectoryInfo(xAppDataPath.Value);
                    if (dirPokerStarsAppData.Exists)
                    {
                        DirectoryInfo[] dirsHandHistories = dirPokerStarsAppData.GetDirectories("HandHistory");
                        foreach (DirectoryInfo dirHandHistory in dirsHandHistories)
                        {
                            if (dirHandHistory.Exists)
                            {
                                DirectoryInfo[] dirPlayers = dirHandHistory.GetDirectories();
                                foreach (DirectoryInfo dirPlayer in dirPlayers)
                                {
                                    if (dirPlayer.Exists)
                                    {
                                        ImportFolders.Add(dirPlayer.FullName);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //
        }
    }
}

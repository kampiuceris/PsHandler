using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using Hardcodet.Wpf.TaskbarNotification;
using Hardcodet.Wpf.TaskbarNotification.Interop;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Threading;
using PsHandler.Custom;
using PsHandler.Hud;
using PsHandler.PokerTypes;
using PsHandler.Randomizer;
using PsHandler.TableTiler;
using PsHandler.UI;

namespace PsHandler
{
    public class Config
    {
        // Constants

        public const string NAME = "PsHandler";
        public const int VERSION = 23;
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
        public static int AutoTileCheckingTimeMs = 3000;

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

        private static void Set(XElement xElement, string name, object o, ref List<ExceptionPsHandler> exceptions, string exceptionHeader)
        {
            try
            {
                xElement.Add(new XElement(name, o.ToString()));
            }
            catch (Exception e)
            {
                exceptions.Add(new ExceptionPsHandler(e, exceptionHeader));
            }
        }

        private static XElement GetXElement(XElement xElement, string name, ref List<ExceptionPsHandler> exceptions, string exceptionHeader, XElement defaultValue = default(XElement))
        {
            try
            {
                return xElement.Element(name);
            }
            catch (Exception e)
            {
                exceptions.Add(new ExceptionPsHandler(e, exceptionHeader));
                return defaultValue;
            }
        }

        private static bool GetBool(XElement xElement, string name, ref List<ExceptionPsHandler> exceptions, string exceptionHeader, bool defaultValue = default(bool))
        {
            try
            {
                return bool.Parse(xElement.Element(name).Value);
            }
            catch (Exception e)
            {
                exceptions.Add(new ExceptionPsHandler(e, exceptionHeader));
                return defaultValue;
            }
        }

        private static string GetString(XElement xElement, string name, ref List<ExceptionPsHandler> exceptions, string exceptionHeader, string defaultValue = default(string))
        {
            try
            {
                return xElement.Element(name).Value;
            }
            catch (Exception e)
            {
                exceptions.Add(new ExceptionPsHandler(e, exceptionHeader));
                return defaultValue;
            }
        }

        private static int GetInt(XElement xElement, string name, ref List<ExceptionPsHandler> exceptions, string exceptionHeader, int defaultValue = default(int))
        {
            try
            {
                return int.Parse(xElement.Element(name).Value);
            }
            catch (Exception e)
            {
                exceptions.Add(new ExceptionPsHandler(e, exceptionHeader));
                return defaultValue;
            }
        }

        private static float GetFloat(XElement xElement, string name, ref List<ExceptionPsHandler> exceptions, string exceptionHeader, float defaultValue = default(float))
        {
            try
            {
                return float.Parse(xElement.Element(name).Value);
            }
            catch (Exception e)
            {
                exceptions.Add(new ExceptionPsHandler(e, exceptionHeader));
                return defaultValue;
            }
        }

        private static Color GetColor(XElement xElement, string name, ref List<ExceptionPsHandler> exceptions, string exceptionHeader, Color defaultValue = default(Color))
        {
            try
            {
                return (Color)ColorConverter.ConvertFromString(xElement.Element(name).Value);
            }
            catch (Exception e)
            {
                exceptions.Add(new ExceptionPsHandler(e, exceptionHeader));
                return defaultValue;
            }
        }

        private static FontFamily GetFontFamily(XElement xElement, string name, ref List<ExceptionPsHandler> exceptions, string exceptionHeader, FontFamily defaultValue = default(FontFamily))
        {
            try
            {
                return new FontFamily(xElement.Element(name).Value);
            }
            catch (Exception e)
            {
                exceptions.Add(new ExceptionPsHandler(e, exceptionHeader));
                return defaultValue;
            }
        }

        private static FontWeight GetFontWeight(XElement xElement, string name, ref List<ExceptionPsHandler> exceptions, string exceptionHeader, FontWeight defaultValue = default(FontWeight))
        {
            try
            {
                return (FontWeight)new FontWeightConverter().ConvertFrom(xElement.Element(name).Value);
            }
            catch (Exception e)
            {
                exceptions.Add(new ExceptionPsHandler(e, exceptionHeader));
                return defaultValue;
            }
        }

        private static FontStyle GetFontStyle(XElement xElement, string name, ref List<ExceptionPsHandler> exceptions, string exceptionHeader, FontStyle defaultValue = default(FontStyle))
        {
            try
            {
                return (FontStyle)new FontStyleConverter().ConvertFrom(xElement.Element(name).Value);
            }
            catch (Exception e)
            {
                exceptions.Add(new ExceptionPsHandler(e, exceptionHeader));
                return defaultValue;
            }
        }

        private static Thickness GetThickness(XElement xElement, string name, ref List<ExceptionPsHandler> exceptions, string exceptionHeader, Thickness defaultValue = default(Thickness))
        {
            try
            {
                var split = xElement.Element(name).Value.Split(new[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                return new Thickness(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]), int.Parse(split[3]));
            }
            catch (Exception e)
            {
                exceptions.Add(new ExceptionPsHandler(e, exceptionHeader));
                return defaultValue;
            }
        }

        #endregion

        //

        public static IEnumerable<ExceptionPsHandler> LoadXml()
        {
            List<ExceptionPsHandler> exceptions = new List<ExceptionPsHandler>();
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

                VersionControl(root, version);

                #region Settings

                MinimizeToSystemTray = GetBool(root, "MinimizeToSystemTray", ref exceptions, "LoadXml() MinimizeToSystemTray", false);
                StartMinimized = GetBool(root, "StartMinimized", ref exceptions, "LoadXml() StartMinimized", false);
                SaveGuiLocation = GetBool(root, "SaveGuiLocation", ref exceptions, "LoadXml() SaveGuiLocation", false);
                GuiLocationX = GetInt(root, "GuiLocationX", ref exceptions, "LoadXml() GuiLocationX");
                GuiLocationY = GetInt(root, "GuiLocationY", ref exceptions, "LoadXml() GuiLocationY");
                SaveGuiSize = GetBool(root, "SaveGuiSize", ref exceptions, "LoadXml() SaveGuiSize", false);
                GuiWidth = GetInt(root, "GuiWidth", ref exceptions, "LoadXml() GuiWidth", 600);
                GuiHeight = GetInt(root, "GuiHeight", ref exceptions, "LoadXml() GuiHeight", 400);
                HotkeyExit = KeyCombination.Parse(GetString(root, "HotkeyExit", ref exceptions, "LoadXml() HotkeyExit"));
                PokerStarsThemeTable = PokerStarsThemeTable.Parse(GetString(root, "PokerStarsThemeTable", ref exceptions, "LoadXml() PokerStarsThemeTable", new PokerStarsThemesTable.Unknown().ToString()));
                foreach (XElement xImportFolderPath in root.Elements("ImportFolderPaths").SelectMany(o => o.Elements("ImportFolderPath")))
                    if (!String.IsNullOrEmpty(xImportFolderPath.Value))
                        ImportFolders.Add(xImportFolderPath.Value);

                #endregion

                #region Controller

                AutoclickImBack = GetBool(root, "AutoclickImBack", ref exceptions, "LoadXml() AutoclickImBack", false);
                AutoclickTimebank = GetBool(root, "AutoclickTimebank", ref exceptions, "LoadXml() AutoclickTimebank", false);
                AutoclickYesSeatAvailable = GetBool(root, "AutoclickYesSeatAvailable", ref exceptions, "LoadXml() AutoclickYesSeatAvailable", false);
                AutocloseTournamentRegistrationPopups = GetBool(root, "AutocloseTournamentRegistrationPopups", ref exceptions, "LoadXml() AutocloseTournamentRegistrationPopups", false);
                AutocloseHM2ApplyToSimilarTablesPopups = GetBool(root, "AutocloseHM2ApplyToSimilarTablesPopups", ref exceptions, "LoadXml() AutocloseHM2ApplyToSimilarTablesPopups", false);
                HotkeyHandReplay = KeyCombination.Parse(GetString(root, "HotkeyHandReplay", ref exceptions, "LoadXml() HotkeyHandReplay"));
                HotkeyQuickPreview = KeyCombination.Parse(GetString(root, "HotkeyQuickPreview", ref exceptions, "LoadXml() HotkeyQuickPreview"));
                EnableCustomTablesWindowStyle = GetBool(root, "EnableCustomTablesWindowStyle", ref exceptions, "LoadXml() EnableCustomTablesWindowStyle", false);
                string tableWindowStyleStr = GetString(root, "CustomTablesWindowStyle", ref exceptions, "LoadXml() CustomTablesWindowStyle", TableWindowStyle.Borderless.ToString());
                foreach (TableWindowStyle tableWindowStyle in Enum.GetValues(typeof(TableWindowStyle)).Cast<TableWindowStyle>().Where(tableWindowStyle => tableWindowStyle.ToString().Equals(tableWindowStyleStr)))
                {
                    CustomTablesWindowStyle = tableWindowStyle;
                    break;
                }

                #endregion

                #region Randomizer

                EnableRandomizer = GetBool(root, "EnableRandomizer", ref exceptions, "LoadXml() EnableRandomizer", false);
                RandomizerChance10 = GetInt(root, "RandomizerChance10", ref exceptions, "LoadXml() RandomizerChance10", 10);
                RandomizerChance20 = GetInt(root, "RandomizerChance20", ref exceptions, "LoadXml() RandomizerChance20", 20);
                RandomizerChance30 = GetInt(root, "RandomizerChance30", ref exceptions, "LoadXml() RandomizerChance30", 30);
                RandomizerChance40 = GetInt(root, "RandomizerChance40", ref exceptions, "LoadXml() RandomizerChance40", 40);
                RandomizerChance50 = GetInt(root, "RandomizerChance50", ref exceptions, "LoadXml() RandomizerChance50", 50);
                RandomizerChance60 = GetInt(root, "RandomizerChance60", ref exceptions, "LoadXml() RandomizerChance60", 60);
                RandomizerChance70 = GetInt(root, "RandomizerChance70", ref exceptions, "LoadXml() RandomizerChance70", 70);
                RandomizerChance80 = GetInt(root, "RandomizerChance80", ref exceptions, "LoadXml() RandomizerChance80", 80);
                RandomizerChance90 = GetInt(root, "RandomizerChance90", ref exceptions, "LoadXml() RandomizerChance90", 90);
                HotkeyRandomizerChance10 = KeyCombination.Parse(GetString(root, "HotkeyRandomizerChance10", ref exceptions, "LoadXml() HotkeyRandomizerChance10", new KeyCombination(Key.NumPad1, true, false, false).ToString()));
                HotkeyRandomizerChance20 = KeyCombination.Parse(GetString(root, "HotkeyRandomizerChance20", ref exceptions, "LoadXml() HotkeyRandomizerChance20", new KeyCombination(Key.NumPad2, true, false, false).ToString()));
                HotkeyRandomizerChance30 = KeyCombination.Parse(GetString(root, "HotkeyRandomizerChance30", ref exceptions, "LoadXml() HotkeyRandomizerChance30", new KeyCombination(Key.NumPad3, true, false, false).ToString()));
                HotkeyRandomizerChance40 = KeyCombination.Parse(GetString(root, "HotkeyRandomizerChance40", ref exceptions, "LoadXml() HotkeyRandomizerChance40", new KeyCombination(Key.NumPad4, true, false, false).ToString()));
                HotkeyRandomizerChance50 = KeyCombination.Parse(GetString(root, "HotkeyRandomizerChance50", ref exceptions, "LoadXml() HotkeyRandomizerChance50", new KeyCombination(Key.NumPad5, true, false, false).ToString()));
                HotkeyRandomizerChance60 = KeyCombination.Parse(GetString(root, "HotkeyRandomizerChance60", ref exceptions, "LoadXml() HotkeyRandomizerChance60", new KeyCombination(Key.NumPad6, true, false, false).ToString()));
                HotkeyRandomizerChance70 = KeyCombination.Parse(GetString(root, "HotkeyRandomizerChance70", ref exceptions, "LoadXml() HotkeyRandomizerChance70", new KeyCombination(Key.NumPad7, true, false, false).ToString()));
                HotkeyRandomizerChance80 = KeyCombination.Parse(GetString(root, "HotkeyRandomizerChance80", ref exceptions, "LoadXml() HotkeyRandomizerChance80", new KeyCombination(Key.NumPad8, true, false, false).ToString()));
                HotkeyRandomizerChance90 = KeyCombination.Parse(GetString(root, "HotkeyRandomizerChance90", ref exceptions, "LoadXml() HotkeyRandomizerChance90", new KeyCombination(Key.NumPad9, true, false, false).ToString()));

                #endregion

                #region Hud

                EnableHud = GetBool(root, "EnableHud", ref exceptions, "LoadXml() EnableHud", false);

                TableManager.EnableHudTimer = GetBool(root, "EnableHudTimer", ref exceptions, "LoadXml() EnableHudTimer", false);
                TimerDiff = GetInt(root, "TimerDiff", ref exceptions, "LoadXml() TimerDiff", 0);
                TimerHHNotFound = GetString(root, "TimerHHNotFound", ref exceptions, "LoadXml() TimerHHNotFound", "HH not found");
                TimerPokerTypeNotFound = GetString(root, "TimerPokerTypeNotFound", ref exceptions, "LoadXml() TimerPokerTypeNotFound", "Poker Type not found");
                TimerMultiplePokerTypes = GetString(root, "TimerMultiplePokerTypes", ref exceptions, "LoadXml() TimerMultiplePokerTypes", "Multiple Poker Types");

                TableManager.EnableHudBigBlind = GetBool(root, "EnableHudBigBlind", ref exceptions, "LoadXml() EnableHudBigBlind", false);
                BigBlindShowTournamentM = GetBool(root, "BigBlindShowTournamentM", ref exceptions, "LoadXml() BigBlindShowTournamentM", false);
                BigBlindMByPlayerCount = GetBool(root, "BigBlindMByPlayerCount", ref exceptions, "LoadXml() BigBlindMByPlayerCount", true);
                BigBlindMByTableSize = GetBool(root, "BigBlindMByTableSize", ref exceptions, "LoadXml() BigBlindMByTableSize", false);
                BigBlindDecimals = GetInt(root, "BigBlindDecimals", ref exceptions, "LoadXml() BigBlindDecimals", 0);
                BigBlindHHNotFound = GetString(root, "BigBlindHHNotFound", ref exceptions, "LoadXml() BigBlindHHNotFound", "X");
                BigBlindPrefix = GetString(root, "BigBlindPrefix", ref exceptions, "LoadXml() BigBlindPrefix", "");
                BigBlindPostfix = GetString(root, "BigBlindPostfix", ref exceptions, "LoadXml() BigBlindPostfix", "");

                #endregion

                #region Hud Design

                HudTimerBackground = GetColor(root, "HudTimerBackground", ref exceptions, "LoadXml() HudTimerBackground", Colors.Black);
                HudTimerForeground = GetColor(root, "HudTimerForeground", ref exceptions, "LoadXml() HudTimerForeground", Colors.White);
                HudTimerFontFamily = GetFontFamily(root, "HudTimerFontFamily", ref exceptions, "LoadXml() HudTimerFontFamily", new FontFamily("Consolas"));
                HudTimerFontWeight = GetFontWeight(root, "HudTimerFontWeight", ref exceptions, "LoadXml() HudTimerFontWeight", FontWeights.Normal);
                HudTimerFontStyle = GetFontStyle(root, "HudTimerFontStyle", ref exceptions, "LoadXml() HudTimerFontStyle", FontStyles.Normal);
                HudTimerFontSize = GetFloat(root, "HudTimerFontSize", ref exceptions, "LoadXml() HudTimerFontSize", 15);
                HudTimerMargin = GetThickness(root, "HudTimerMargin", ref exceptions, "LoadXml() HudTimerMargin", new Thickness(2, 2, 2, 2));

                HudBigBlindBackground = GetColor(root, "HudBigBlindBackground", ref exceptions, "LoadXml() HudBigBlindBackground", Colors.Black);
                HudBigBlindForeground = GetColor(root, "HudBigBlindForeground", ref exceptions, "LoadXml() HudBigBlindForeground", Colors.White);
                HudBigBlindFontFamily = GetFontFamily(root, "HudBigBlindFontFamily", ref exceptions, "LoadXml() HudBigBlindFontFamily", new FontFamily("Consolas"));
                HudBigBlindFontWeight = GetFontWeight(root, "HudBigBlindFontWeight", ref exceptions, "LoadXml() HudBigBlindFontWeight", FontWeights.Normal);
                HudBigBlindFontStyle = GetFontStyle(root, "HudBigBlindFontStyle", ref exceptions, "LoadXml() HudBigBlindFontStyle", FontStyles.Normal);
                HudBigBlindFontSize = GetFloat(root, "HudBigBlindFontSize", ref exceptions, "LoadXml() HudBigBlindFontSize", 25);
                HudBigBlindMargin = GetThickness(root, "HudBigBlindMargin", ref exceptions, "LoadXml() HudBigBlindMargin", new Thickness(2, 2, 2, 2));
                foreach (XElement element in GetXElement(root, "HudBigBlindColorsByValue", ref exceptions, "LoadXml() HudBigBlindMargin", new XElement("HudBigBlindColorsByValue")).Elements("ColorByValue"))
                {
                    ColorByValue colorByValue = ColorByValue.FromXElement(element, ref exceptions, "LoadXml() ColorByValue");
                    if (colorByValue != null)
                    {
                        HudBigBlindColorsByValue.Add(colorByValue);
                    }
                }


                #endregion

                TableManager.HudTimerLocationLocked = GetBool(root, "HudTimerLocationLocked", ref exceptions, "LoadXml() HudTimerLocationLocked", false);
                TableManager.FromXElementHudTimerLocations(root.Element("HudTimerLocations"), ref exceptions, "LoadXml()");
                TableManager.HudBigBlindLocationLocked = GetBool(root, "HudBigBlindLocationLocked", ref exceptions, "LoadXml() HudBigBlindLocationLocked", false);
                TableManager.FromXElementHudBigBlindLocations(root.Element("HudBigBlindLocations"), ref exceptions, "LoadXml()");
                EnableTableTiler = GetBool(root, "EnableTableTiler", ref exceptions, "LoadXml() EnableTableTiler", false);
                AutoTileCheckingTimeMs = GetInt(root, "AutoTileCheckingTimeMs", ref exceptions, "LoadXml() AutoTileCheckingTimeMs", 3000);
                App.TableTileManager.FromXElement(root.Element("TableTiles"), ref exceptions, "LoadXml()");
                App.PokerTypeManager.FromXElement(root.Element("PokerTypes"), ref exceptions, "LoadXml()");
            }
            catch (Exception e)
            {
                exceptions.Add(new ExceptionPsHandler(e, "LoadXml() Main Exception"));
            }

            App.PokerTypeManager.SeedDefaultValues();
            App.TableTileManager.SeedDefaultValues();

            //if (exceptions.Any())
            //{
            //    StringBuilder sb = new StringBuilder();
            //    foreach (ExceptionPsHandler e in exceptions)
            //    {
            //        sb.AppendLine("Header:");
            //        sb.AppendLine(e.Header);
            //        sb.AppendLine("Exception:");
            //        sb.AppendLine(e.Exception.ToString());
            //        sb.AppendLine();
            //        sb.AppendLine();
            //    }
            //    string fileName = "pshandler_error_" + DateTime.Now.Ticks + ".log";
            //    File.WriteAllText(fileName, sb.ToString());
            //    WindowMessage.ShowDialog("Some configurations weren't loaded." + Environment.NewLine + Environment.NewLine + "Log file: " + fileName, "Error Loading Config XML", WindowMessageButtons.OK, WindowMessageImage.Error, App.WindowMain, WindowStartupLocation.CenterScreen);
            //}

            return exceptions;
        }

        public static IEnumerable<ExceptionPsHandler> SaveXml()
        {
            List<ExceptionPsHandler> exceptions = new List<ExceptionPsHandler>();
            try
            {
                XDocument xDoc = new XDocument();
                XElement root = new XElement("Config");
                xDoc.Add(root);

                // Version

                Set(root, "Version", VERSION, ref exceptions, "SaveXml() Version");

                #region Settings

                Set(root, "MinimizeToSystemTray", MinimizeToSystemTray, ref exceptions, "SaveXml() MinimizeToSystemTray");
                Set(root, "StartMinimized", StartMinimized, ref exceptions, "SaveXml() StartMinimized");
                Set(root, "SaveGuiLocation", SaveGuiLocation, ref exceptions, "SaveXml() SaveGuiLocation");
                Set(root, "GuiLocationX", GuiLocationX, ref exceptions, "SaveXml() GuiLocationX");
                Set(root, "GuiLocationY", GuiLocationY, ref exceptions, "SaveXml() GuiLocationY");
                Set(root, "SaveGuiSize", SaveGuiSize, ref exceptions, "SaveXml() SaveGuiSize");
                Set(root, "GuiWidth", GuiWidth, ref exceptions, "SaveXml() GuiWidth");
                Set(root, "GuiHeight", GuiHeight, ref exceptions, "SaveXml() GuiHeight");
                Set(root, "HotkeyExit", HotkeyExit, ref exceptions, "SaveXml() HotkeyExit");
                Set(root, "PokerStarsThemeTable", PokerStarsThemeTable, ref exceptions, "SaveXml() PokerStarsThemeTable");
                XElement xImportFolderPaths = new XElement("ImportFolderPaths");
                root.Add(xImportFolderPaths);
                foreach (var path in ImportFolders.ToArray())
                    Set(xImportFolderPaths, "ImportFolderPath", path, ref exceptions, "SaveXml() ImportFolderPath");

                #endregion

                #region Controller

                Set(root, "AutoclickImBack", AutoclickImBack, ref exceptions, "SaveXml() AutoclickImBack");
                Set(root, "AutoclickTimebank", AutoclickTimebank, ref exceptions, "SaveXml() AutoclickTimebank");
                Set(root, "AutoclickYesSeatAvailable", AutoclickYesSeatAvailable, ref exceptions, "SaveXml() AutoclickYesSeatAvailable");
                Set(root, "AutocloseTournamentRegistrationPopups", AutocloseTournamentRegistrationPopups, ref exceptions, "SaveXml() AutocloseTournamentRegistrationPopups");
                Set(root, "AutocloseHM2ApplyToSimilarTablesPopups", AutocloseHM2ApplyToSimilarTablesPopups, ref exceptions, "SaveXml() AutocloseHM2ApplyToSimilarTablesPopups");
                Set(root, "HotkeyHandReplay", HotkeyHandReplay, ref exceptions, "SaveXml() HotkeyHandReplay");
                Set(root, "HotkeyQuickPreview", HotkeyQuickPreview, ref exceptions, "SaveXml() HotkeyQuickPreview");
                Set(root, "EnableCustomTablesWindowStyle", EnableCustomTablesWindowStyle, ref exceptions, "SaveXml() EnableCustomTablesWindowStyle");
                Set(root, "CustomTablesWindowStyle", CustomTablesWindowStyle, ref exceptions, "SaveXml() CustomTablesWindowStyle");

                #endregion

                #region Randomizer

                Set(root, "EnableRandomizer", EnableRandomizer, ref exceptions, "SaveXml() EnableRandomizer");
                Set(root, "RandomizerChance10", RandomizerChance10, ref exceptions, "SaveXml() RandomizerChance10");
                Set(root, "RandomizerChance20", RandomizerChance20, ref exceptions, "SaveXml() RandomizerChance20");
                Set(root, "RandomizerChance30", RandomizerChance30, ref exceptions, "SaveXml() RandomizerChance30");
                Set(root, "RandomizerChance40", RandomizerChance40, ref exceptions, "SaveXml() RandomizerChance40");
                Set(root, "RandomizerChance50", RandomizerChance50, ref exceptions, "SaveXml() RandomizerChance50");
                Set(root, "RandomizerChance60", RandomizerChance60, ref exceptions, "SaveXml() RandomizerChance60");
                Set(root, "RandomizerChance70", RandomizerChance70, ref exceptions, "SaveXml() RandomizerChance70");
                Set(root, "RandomizerChance80", RandomizerChance80, ref exceptions, "SaveXml() RandomizerChance80");
                Set(root, "RandomizerChance90", RandomizerChance90, ref exceptions, "SaveXml() RandomizerChance90");
                Set(root, "HotkeyRandomizerChance10", HotkeyRandomizerChance10, ref exceptions, "SaveXml() HotkeyRandomizerChance10");
                Set(root, "HotkeyRandomizerChance20", HotkeyRandomizerChance20, ref exceptions, "SaveXml() HotkeyRandomizerChance20");
                Set(root, "HotkeyRandomizerChance30", HotkeyRandomizerChance30, ref exceptions, "SaveXml() HotkeyRandomizerChance30");
                Set(root, "HotkeyRandomizerChance40", HotkeyRandomizerChance40, ref exceptions, "SaveXml() HotkeyRandomizerChance40");
                Set(root, "HotkeyRandomizerChance50", HotkeyRandomizerChance50, ref exceptions, "SaveXml() HotkeyRandomizerChance50");
                Set(root, "HotkeyRandomizerChance60", HotkeyRandomizerChance60, ref exceptions, "SaveXml() HotkeyRandomizerChance60");
                Set(root, "HotkeyRandomizerChance70", HotkeyRandomizerChance70, ref exceptions, "SaveXml() HotkeyRandomizerChance70");
                Set(root, "HotkeyRandomizerChance80", HotkeyRandomizerChance80, ref exceptions, "SaveXml() HotkeyRandomizerChance80");
                Set(root, "HotkeyRandomizerChance90", HotkeyRandomizerChance90, ref exceptions, "SaveXml() HotkeyRandomizerChance90");

                #endregion

                #region Hud

                Set(root, "EnableHud", EnableHud, ref exceptions, "SaveXml() EnableHud");

                Set(root, "EnableHudTimer", TableManager.EnableHudTimer, ref exceptions, "SaveXml() EnableHudTimer");
                Set(root, "TimerDiff", TimerDiff, ref exceptions, "SaveXml() TimerDiff");
                Set(root, "TimerHHNotFound", TimerHHNotFound, ref exceptions, "SaveXml() TimerHHNotFound");
                Set(root, "TimerPokerTypeNotFound", TimerPokerTypeNotFound, ref exceptions, "SaveXml() TimerPokerTypeNotFound");
                Set(root, "TimerMultiplePokerTypes", TimerMultiplePokerTypes, ref exceptions, "SaveXml() TimerMultiplePokerTypes");

                Set(root, "EnableHudBigBlind", TableManager.EnableHudBigBlind, ref exceptions, "SaveXml() EnableHudBigBlind");
                Set(root, "BigBlindShowTournamentM", BigBlindShowTournamentM, ref exceptions, "SaveXml() BigBlindShowTournamentM");
                Set(root, "BigBlindMByPlayerCount", BigBlindMByPlayerCount, ref exceptions, "SaveXml() BigBlindMByPlayerCount");
                Set(root, "BigBlindMByTableSize", BigBlindMByTableSize, ref exceptions, "SaveXml() BigBlindMByTableSize");
                Set(root, "BigBlindDecimals", BigBlindDecimals, ref exceptions, "SaveXml() BigBlindDecimals");
                Set(root, "BigBlindHHNotFound", BigBlindHHNotFound, ref exceptions, "SaveXml() BigBlindHHNotFound");
                Set(root, "BigBlindPrefix", BigBlindPrefix, ref exceptions, "SaveXml() BigBlindPrefix");
                Set(root, "BigBlindPostfix", BigBlindPostfix, ref exceptions, "SaveXml() BigBlindPostfix");

                #endregion

                #region Hud Design

                Set(root, "HudTimerBackground", HudTimerBackground, ref exceptions, "SaveXml() HudTimerBackground");
                Set(root, "HudTimerForeground", HudTimerForeground, ref exceptions, "SaveXml() HudTimerForeground");
                Set(root, "HudTimerFontFamily", HudTimerFontFamily, ref exceptions, "SaveXml() HudTimerFontFamily");
                Set(root, "HudTimerFontWeight", HudTimerFontWeight, ref exceptions, "SaveXml() HudTimerFontWeight");
                Set(root, "HudTimerFontStyle", HudTimerFontStyle, ref exceptions, "SaveXml() HudTimerFontStyle");
                Set(root, "HudTimerFontSize", HudTimerFontSize, ref exceptions, "SaveXml() HudTimerFontSize");
                Set(root, "HudTimerMargin", HudTimerMargin, ref exceptions, "SaveXml() HudTimerMargin");

                Set(root, "HudBigBlindBackground", HudBigBlindBackground, ref exceptions, "SaveXml() HudBigBlindBackground");
                Set(root, "HudBigBlindForeground", HudBigBlindForeground, ref exceptions, "SaveXml() HudBigBlindForeground");
                Set(root, "HudBigBlindFontFamily", HudBigBlindFontFamily, ref exceptions, "SaveXml() HudBigBlindFontFamily");
                Set(root, "HudBigBlindFontWeight", HudBigBlindFontWeight, ref exceptions, "SaveXml() HudBigBlindFontWeight");
                Set(root, "HudBigBlindFontStyle", HudBigBlindFontStyle, ref exceptions, "SaveXml() HudBigBlindFontStyle");
                Set(root, "HudBigBlindFontSize", HudBigBlindFontSize, ref exceptions, "SaveXml() HudBigBlindFontSize");
                Set(root, "HudBigBlindMargin", HudBigBlindMargin, ref exceptions, "SaveXml() HudBigBlindMargin");
                XElement xBigBlindColorsByValue = new XElement("HudBigBlindColorsByValue");
                root.Add(xBigBlindColorsByValue);
                foreach (var item in HudBigBlindColorsByValue)
                {
                    xBigBlindColorsByValue.Add(item.ToXElement());
                }

                #endregion

                Set(root, "HudTimerLocationLocked", TableManager.HudTimerLocationLocked, ref exceptions, "SaveXml() HudTimerLocationLocked");
                root.Add(TableManager.ToXElementHudTimerLocations());
                Set(root, "HudBigBlindLocationLocked", TableManager.HudBigBlindLocationLocked, ref exceptions, "SaveXml() HudBigBlindLocationLocked");
                root.Add(TableManager.ToXElementHudBigBlindLocations());
                Set(root, "EnableTableTiler", EnableTableTiler, ref exceptions, "SaveXml() EnableTableTiler");
                Set(root, "AutoTileCheckingTimeMs", AutoTileCheckingTimeMs, ref exceptions, "SaveXml() AutoTileCheckingTimeMs");
                root.Add(App.TableTileManager.ToXElement());
                root.Add(App.PokerTypeManager.ToXElement());

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
                exceptions.Add(new ExceptionPsHandler(e, "SaveXml() Main Exception"));
            }

            if (exceptions.Any())
            {
                StringBuilder sb = new StringBuilder();
                foreach (ExceptionPsHandler e in exceptions)
                {
                    sb.AppendLine("Header:");
                    sb.AppendLine(e.Header);
                    sb.AppendLine("Exception:");
                    sb.AppendLine(e.Exception.ToString());
                    sb.AppendLine();
                    sb.AppendLine();
                }
                string fileName = "pshandler_error_" + DateTime.Now.Ticks + ".log";
                File.WriteAllText(fileName, sb.ToString());
                WindowMessage.ShowDialog("Some configurations weren't saved." + Environment.NewLine + Environment.NewLine + "Log file: " + fileName, "Error Saving Config XML", WindowMessageButtons.OK, WindowMessageImage.Error, App.WindowMain, WindowStartupLocation.CenterScreen);
            }

            return exceptions;
        }

        public static void VersionControl(XElement root, int version)
        {
            try
            {
                if (version < 20)
                {
                    // poker types delaut override:
                    List<PokerType> defaultPokerTypes = PokerType.GetDefaultValues().ToList();
                    foreach (PokerType pokerType in App.PokerTypeManager.GetPokerTypesCopy())
                    {
                        if (!defaultPokerTypes.Any(a => a.Name.Equals(pokerType.Name)))
                        {
                            defaultPokerTypes.Add(pokerType);
                        }
                    }
                    App.PokerTypeManager.RemoveAll();
                    App.PokerTypeManager.Add(defaultPokerTypes);
                }
            }
            catch
            {
            }
        }
    }
}

using System.Globalization;
using System.IO;
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
using PsHandler.Hud.Import;

namespace PsHandler
{
    public class Config
    {
        // Constants

        public const string NAME = "PsHandler";
        public const int VERSION = 12;
        public const string UPDATE_HREF = "http://chainer.projektas.in/PsHandler/update.php";
        public static string MACHINE_GUID = GetMachineGuid();
        public static string CONFIG_FILENAME = "pshandler.xml";

        // Settings

        public static List<string> AppDataPaths = new List<string>();
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

        //

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
                foreach (var path in AppDataPaths.ToArray())
                {
                    Set(xAppDataPaths, "AppDataPath", path, ref errors, "");
                }

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

                Set(root, "BigBlindHudBackground", HudManager.BigBlindHudBackground, ref errors);
                Set(root, "BigBlindHudForeground", HudManager.BigBlindHudForeground, ref errors);
                Set(root, "BigBlindHudFontFamily", HudManager.BigBlindHudFontFamily, ref errors);
                Set(root, "BigBlindHudFontWeight", HudManager.BigBlindHudFontWeight, ref errors);
                Set(root, "BigBlindHudFontStyle", HudManager.BigBlindHudFontStyle, ref errors);
                Set(root, "BigBlindHudFontSize", HudManager.BigBlindHudFontSize, ref errors);
                XElement xBigBlindColorsByValue = new XElement("BigBlindColorsByValue");
                root.Add(xBigBlindColorsByValue);
                foreach (var item in HudManager.BigBlindColorsByValue)
                {
                    xBigBlindColorsByValue.Add(item.ToXElement());
                }
                XElement xBigBlindLocationsX = new XElement("BigBlindHudLocations");
                root.Add(xBigBlindLocationsX);
                foreach (TableSize tableSize in Enum.GetValues(typeof(TableSize)))
                {
                    xBigBlindLocationsX.Add(
                        new XElement("LocationByTableSize",
                             new XElement("TableSize", (int)tableSize),
                             new XElement("LocationX", HudManager.GetBigBlindHudLocationX(tableSize)),
                             new XElement("LocationY", HudManager.GetBigBlindHudLocationY(tableSize))
                    ));
                }

                XElement xBigBlindLocationsY = new XElement("BigBlindHudLocationsY");
                root.Add(xBigBlindLocationsY);
                foreach (TableSize tableSize in Enum.GetValues(typeof(TableSize)))
                {
                    xBigBlindLocationsY.Add(new XElement(tableSize.ToString(), HudManager.GetBigBlindHudLocationX(tableSize)));
                }

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

                // handle hidden files
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
                XDocument xDoc = XDocument.Load(CONFIG_FILENAME);
                XElement root = xDoc.Element("Config");

                foreach (XElement xAppDataPath in root.Elements("AppDataPaths").SelectMany(o => o.Elements("AppDataPath")))
                {
                    if (!String.IsNullOrEmpty(xAppDataPath.Value))
                    {
                        AppDataPaths.Add(xAppDataPath.Value);
                    }
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
                HudManager.BigBlindHudBackground = GetColor(root, "BigBlindHudBackground", ref errors, Colors.Black);
                HudManager.BigBlindHudForeground = GetColor(root, "BigBlindHudForeground", ref errors, Colors.White);
                HudManager.BigBlindHudFontFamily = GetFontFamily(root, "BigBlindHudFontFamily", ref errors, new FontFamily("Consolas"));
                HudManager.BigBlindHudFontWeight = GetFontWeight(root, "BigBlindHudFontWeight", ref errors);
                HudManager.BigBlindHudFontStyle = GetFontStyle(root, "BigBlindHudFontStyle", ref errors);
                HudManager.BigBlindHudFontSize = GetFloat(root, "BigBlindHudFontSize", ref errors, 10);
                if (HudManager.TimerHudFontSize > 72) HudManager.TimerHudFontSize = 72;
                foreach (XElement xElement in root.Elements("BigBlindColorsByValue"))
                {
                    HudManager.BigBlindColorsByValue.AddRange(xElement.Elements("ColorByValue").Select(ColorByValue.FromXElement).Where(o => o != null));
                }

                foreach (XElement xElementBigBlindLocations in root.Elements("BigBlindHudLocations"))
                {
                    foreach (XElement xElementLocationByTableSize in xElementBigBlindLocations.Elements("LocationByTableSize"))
                    {
                        int tableSize; float locationX, locationY;
                        if (int.TryParse(xElementLocationByTableSize.Element("TableSize").Value, out tableSize)
                            && float.TryParse(xElementLocationByTableSize.Element("LocationX").Value, out locationX)
                            && float.TryParse(xElementLocationByTableSize.Element("LocationY").Value, out locationY))
                        {
                            HudManager.SetBigBlindHudLocationX((TableSize)tableSize, locationX, null);
                            HudManager.SetBigBlindHudLocationY((TableSize)tableSize, locationY, null);
                        }
                    }
                }
                //HudManager.SetBigBlindHudLocationX(GetFloat(root, "BigBlindHudLocationX", ref errors), null);
                //HudManager.SetBigBlindHudLocationY(GetFloat(root, "BigBlindHudLocationY", ref errors), null);


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

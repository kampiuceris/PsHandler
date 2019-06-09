// PsHandler - poker software helping tool.
// Copyright (C) 2014-2015  kampiuceris

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

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
using PsHandler.ColorPicker;
using PsHandler.Custom;
using PsHandler.Hud;
using PsHandler.PokerMath;
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
        public const int VERSION = 35;
        public const string ROOT_HREF = "http://pshandler.azurewebsites.net";
        public const string UPDATE_HREF = ROOT_HREF + "/update.php";
        public const string DOWNLOAD_HREF = ROOT_HREF + "/PsHandler.exe";
        public static string MACHINE_GUID = GetMachineGuid();
        public static string CONFIG_FILENAME = "pshandler.xml";
        public static int WINDOWS_BORDER_THICKNESS = WinApi.GetSystemMetrics(WinApi.SystemMetric.SM_CXSIZEFRAME) * 2;
        public static int WINDOWS_TITLE_BORDER_THICKNESS = WinApi.GetSystemMetrics(WinApi.SystemMetric.SM_CYCAPTION);
        public static System.Drawing.Size POKERSTARS_TABLE_CLIENT_SIZE_MIN = new System.Drawing.Size(475, 327);
        public static System.Drawing.Size POKERSTARS_TABLE_CLIENT_SIZE_DEFAULT = new System.Drawing.Size(792, 546);

        // GNU

        public static bool GnuGplV3Agreement = false;

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

        // HUD

        public static int[] PreferredSeat = new int[11]
        {
            0, // Default
            0, // 1-max
            0, // 2-max
            1, // 3-max
            1, // 4-max
            2, // 5-max
            2, // 6-max
            3, // 7-max
            3, // 8-max
            4, // 9-max
            4, // 10-max
        };

        public static bool HudEnable = false;

        public static bool HudTimerEnable = true;
        public static bool HudTimerLocationLocked = false;
        public static bool HudTimerShowTimer = true;
        public static bool HudTimerShowHandCount = false;
        public static int HudTimerDiff = 0;
        public static string HudTimerHHNotFound = "HH not found";
        public static string HudTimerPokerTypeNotFound = "Poker Type not found";
        public static string HudTimerMultiplePokerTypes = "Multiple Poker Types";

        public static bool HudBigBlindEnable = true;
        public static bool HudBigBlindLocationLocked = false;
        public static bool HudBigBlindShowBB = true;
        public static bool HudBigBlindShowAdjustedBB = false;
        public static bool HudBigBlindShowTournamentM = false;
        public static bool HudBigBlindMByPlayerCount = true;
        public static bool HudBigBlindMByTableSize = false;
        public static bool HudBigBlindShowForOpponents = true;
        public static bool HudBigBlindShowForHero = true;
        public static int HudBigBlindDecimals = 0;
        public static string HudBigBlindHHNotFound = "X";
        public static string HudBigBlindPrefix = "";
        public static string HudBigBlindPostfix = "";

        // HUD Locations

        public static double[] DefaultHudTimerLocationsX = new double[11] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, };
        public static double[] DefaultHudTimerLocationsY = new double[11] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, };
        public static double[] HudTimerLocationsX = new double[11] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, };
        public static double[] HudTimerLocationsY = new double[11] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, };
        #region DEFAULT HudBigBlindLocations X,Y Default = HudBigBlindLocationsX/Y[TableSize][IndexOfSeat]

        #region DEFAULT HudBigBlindLocationsX

        public static double[][] DefaultHudBigBlindLocationsX = new double[11][]
        {
            // Default
            new double[10]
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
            },

            // 1-max
            new double[10]
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
            },

            // 2-max
            new double[10]
            {
                640 / 792.0,
                11 / 792.0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
            },
 
            // 3-max
            new double[10]
            {
                648 / 792.0,
                326 / 792.0,
                4 / 792.0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
            },

            // 4-max
            new double[10]
            {
                495 / 792.0,
                495 / 792.0,
                157 / 792.0,
                157 / 792.0,
                0,
                0,
                0,
                0,
                0,
                0,
            },

            // 5-max
            new double[10]
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
            },

            // 6-max
            new double[10]
            {
                495 / 792.0,
                640 / 792.0,
                495 / 792.0,
                157 / 792.0,
                10 / 792.0,
                157 / 792.0,
                0,
                0,
                0,
                0,
            },

            // 7-max
            new double[10]
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
            },
            
            // 8-max
            new double[10]
            {
                484 / 792.0, 
                642 / 792.0, 
                642 / 792.0, 
                424 / 792.0, 
                226 / 792.0, 
                9 / 792.0, 
                9 / 792.0, 
                166 / 792.0, 
                0,
                0,
            },

            // 9-max
            new double[10]
            {
                497 / 792.0, 
                648 / 792.0, 
                649/ 792.0,
                522 / 792.0,
                326 / 792.0,
                131 / 792.0,
                4 / 792.0,
                4 / 792.0,
                156 / 792.0,
                0,
            },

            // 10-max
            new double[10]
            {
                485 / 792.0, 
                634 / 792.0, 
                644 / 792.0,
                634 / 792.0,
                434 / 792.0,
                217 / 792.0,
                16 / 792.0,
                6 / 792.0,
                16 / 792.0,
                166 / 792.0,
            }, 
        };

        #endregion

        #region DEFAULT HudBigBlindLocationsY

        public static double[][] DefaultHudBigBlindLocationsY = new double[11][]
        {
            // Default
            new double[10]
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
            },

            // 1-max
            new double[10]
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
            },

            // 2-max
            new double[10]
            {
                194 / 546.0,
                194 / 546.0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
            },

            // 3-max
            new double[10]
            {
                129 / 546.0,
                361 / 546.0,
                129 / 546.0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
            },

            // 4-max
            new double[10]
            {
                33 / 546.0,
                355 / 546.0,
                355 / 546.0,
                33 / 546.0,
                0,
                0,
                0,
                0,
                0,
                0,
            },

            // 5-max
            new double[10]
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
            },

            // 6-max
            new double[10]
            {
                33 / 546.0, 
                194 / 546.0, 
                355 / 546.0, 
                355 / 546.0, 
                194 / 546.0, 
                33 / 546.0, 
                0,
                0,
                0,
                0,
            },

            // 7-max
            new double[10]
            {
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
            },

            // 8-max
            new double[10]
            {
                35 / 546.0,
                150 / 546.0,
                265 / 546.0,
                368 / 546.0,
                368 / 546.0,
                265 / 546.0,
                150 / 546.0,
                35 / 546.0,
                0,
                0,
            }, 

            // 9-max
            new double[10]
            {
                35 / 546.0, 
                128 / 546.0, 
                243 / 546.0,
                350 / 546.0,
                361 / 546.0,
                350 / 546.0,
                243 / 546.0,
                129 / 546.0,
                35 / 546.0,
                0,
            },

            // 10-max
            new double[10]
            {
                36 / 546.0,
                117 / 546.0,
                211 / 546.0,
                299 / 546.0,
                360 / 546.0,
                360 / 546.0,
                299 / 546.0,
                211 / 546.0,
                117 / 546.0,
                36 / 546.0,
            },
        };

        #endregion

        #endregion
        #region HudBigBlindLocations X,Y Default = HudBigBlindLocationsX/Y[TableSize][IndexOfSeat]

        public static double[][] HudBigBlindLocationsX = new double[11][]
        {
            new double[10], // Default
            new double[10], // 1-max
            new double[10], // 2-max
            new double[10], // 3-max
            new double[10], // 4-max
            new double[10], // 5-max
            new double[10], // 6-max
            new double[10], // 7-max
            new double[10], // 8-max
            new double[10], // 9-max    
            new double[10], // 10-max
        };

        public static double[][] HudBigBlindLocationsY = new double[11][]
        {
            new double[10], // Default
            new double[10], // 1-max
            new double[10], // 2-max
            new double[10], // 3-max
            new double[10], // 4-max
            new double[10], // 5-max
            new double[10], // 6-max
            new double[10], // 7-max
            new double[10], // 8-max
            new double[10], // 9-max    
            new double[10], // 10-max
        };

        #endregion

        // HUD Design

        public static Color HudTimerBackground = Colors.Black;
        public static Color HudTimerForeground = Colors.White;
        public static FontFamily HudTimerFontFamily = new FontFamily("Consolas");
        public static FontWeight HudTimerFontWeight = FontWeights.Bold;
        public static FontStyle HudTimerFontStyle = FontStyles.Normal;
        public static double HudTimerFontSize = 15;
        public static Thickness HudTimerMargin = new Thickness(2, 2, 2, 2);
        public static Color HudTimerBorderBrush = Colors.Transparent;
        public static Thickness HudTimerBorderThickness = new Thickness(0, 0, 0, 0);
        public static CornerRadius HudTimerCornerRadius = new CornerRadius(0, 0, 0, 0);

        public static Color HudBigBlindOpponentsBackground = Colors.Transparent;
        public static Color HudBigBlindOpponentsForeground = Colors.RoyalBlue;
        public static FontFamily HudBigBlindOpponentsFontFamily = new FontFamily("Consolas");
        public static FontWeight HudBigBlindOpponentsFontWeight = FontWeights.Bold;
        public static FontStyle HudBigBlindOpponentsFontStyle = FontStyles.Normal;
        public static double HudBigBlindOpponentsFontSize = 25;
        public static Thickness HudBigBlindOpponentsMargin = new Thickness(2, 2, 2, 2);
        public static Color HudBigBlindOpponentsBorderBrush = Colors.Transparent;
        public static Thickness HudBigBlindOpponentsBorderThickness = new Thickness(0, 0, 0, 0);
        public static CornerRadius HudBigBlindOpponentsCornerRadius = new CornerRadius(0, 0, 0, 0);

        public static Color HudBigBlindHeroBackground = Colors.Transparent;
        public static Color HudBigBlindHeroForeground = Colors.RoyalBlue;
        public static FontFamily HudBigBlindHeroFontFamily = new FontFamily("Consolas");
        public static FontWeight HudBigBlindHeroFontWeight = FontWeights.Bold;
        public static FontStyle HudBigBlindHeroFontStyle = FontStyles.Normal;
        public static double HudBigBlindHeroFontSize = 25;
        public static Thickness HudBigBlindHeroMargin = new Thickness(2, 2, 2, 2);
        public static Color HudBigBlindHeroBorderBrush = Colors.Transparent;
        public static Thickness HudBigBlindHeroBorderThickness = new Thickness(0, 0, 0, 0);
        public static CornerRadius HudBigBlindHeroCornerRadius = new CornerRadius(0, 0, 0, 0);

        public static List<ColorByValue> HudBigBlindOpponentsColorsByValue = new List<ColorByValue>();
        public static List<ColorByValue> HudBigBlindHeroColorsByValue = new List<ColorByValue>();

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

        private static float ___GetFloat(XElement xElement, string name, ref List<ExceptionPsHandler> exceptions, string exceptionHeader, float defaultValue = default(float))
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

        private static double GetDouble(XElement xElement, string name, ref List<ExceptionPsHandler> exceptions, string exceptionHeader, double defaultValue = default(double))
        {
            try
            {
                return double.Parse(xElement.Element(name).Value);
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

        private static CornerRadius GetCornerRadius(XElement xElement, string name, ref List<ExceptionPsHandler> exceptions, string exceptionHeader, CornerRadius defaultValue = default(CornerRadius))
        {
            try
            {
                var split = xElement.Element(name).Value.Split(new[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                return new CornerRadius(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]), int.Parse(split[3]));
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

                GnuGplV3Agreement = GetBool(root, "GnuGplV3Agreement", ref exceptions, "LoadXml() GnuGplV3Agreement", GnuGplV3Agreement);

                #region Settings

                MinimizeToSystemTray = GetBool(root, "MinimizeToSystemTray", ref exceptions, "LoadXml() MinimizeToSystemTray", MinimizeToSystemTray);
                StartMinimized = GetBool(root, "StartMinimized", ref exceptions, "LoadXml() StartMinimized", StartMinimized);
                SaveGuiLocation = GetBool(root, "SaveGuiLocation", ref exceptions, "LoadXml() SaveGuiLocation", SaveGuiLocation);
                GuiLocationX = GetInt(root, "GuiLocationX", ref exceptions, "LoadXml() GuiLocationX", GuiLocationX);
                GuiLocationY = GetInt(root, "GuiLocationY", ref exceptions, "LoadXml() GuiLocationY", GuiLocationY);
                SaveGuiSize = GetBool(root, "SaveGuiSize", ref exceptions, "LoadXml() SaveGuiSize", SaveGuiSize);
                GuiWidth = GetInt(root, "GuiWidth", ref exceptions, "LoadXml() GuiWidth", GuiWidth);
                GuiHeight = GetInt(root, "GuiHeight", ref exceptions, "LoadXml() GuiHeight", GuiHeight);
                HotkeyExit = KeyCombination.Parse(GetString(root, "HotkeyExit", ref exceptions, "LoadXml() HotkeyExit", HotkeyExit.ToString()));
                PokerStarsThemeTable = PokerStarsThemeTable.Parse(GetString(root, "PokerStarsThemeTable", ref exceptions, "LoadXml() PokerStarsThemeTable", new PokerStarsThemesTable.Unknown().ToString()));
                foreach (XElement xImportFolderPath in root.Elements("ImportFolderPaths").SelectMany(o => o.Elements("ImportFolderPath")))
                    if (!String.IsNullOrEmpty(xImportFolderPath.Value))
                        ImportFolders.Add(xImportFolderPath.Value);

                #endregion

                #region Controller

                AutoclickImBack = GetBool(root, "AutoclickImBack", ref exceptions, "LoadXml() AutoclickImBack", AutoclickImBack);
                AutoclickTimebank = GetBool(root, "AutoclickTimebank", ref exceptions, "LoadXml() AutoclickTimebank", AutoclickTimebank);
                AutoclickYesSeatAvailable = GetBool(root, "AutoclickYesSeatAvailable", ref exceptions, "LoadXml() AutoclickYesSeatAvailable", AutoclickYesSeatAvailable);
                AutocloseTournamentRegistrationPopups = GetBool(root, "AutocloseTournamentRegistrationPopups", ref exceptions, "LoadXml() AutocloseTournamentRegistrationPopups", AutocloseTournamentRegistrationPopups);
                AutocloseHM2ApplyToSimilarTablesPopups = GetBool(root, "AutocloseHM2ApplyToSimilarTablesPopups", ref exceptions, "LoadXml() AutocloseHM2ApplyToSimilarTablesPopups", AutocloseHM2ApplyToSimilarTablesPopups);
                HotkeyHandReplay = KeyCombination.Parse(GetString(root, "HotkeyHandReplay", ref exceptions, "LoadXml() HotkeyHandReplay", HotkeyHandReplay.ToString()));
                HotkeyQuickPreview = KeyCombination.Parse(GetString(root, "HotkeyQuickPreview", ref exceptions, "LoadXml() HotkeyQuickPreview", HotkeyQuickPreview.ToString()));

                #endregion

                #region Randomizer

                EnableRandomizer = GetBool(root, "EnableRandomizer", ref exceptions, "LoadXml() EnableRandomizer", EnableRandomizer);
                RandomizerChance10 = GetInt(root, "RandomizerChance10", ref exceptions, "LoadXml() RandomizerChance10", RandomizerChance10);
                RandomizerChance20 = GetInt(root, "RandomizerChance20", ref exceptions, "LoadXml() RandomizerChance20", RandomizerChance20);
                RandomizerChance30 = GetInt(root, "RandomizerChance30", ref exceptions, "LoadXml() RandomizerChance30", RandomizerChance30);
                RandomizerChance40 = GetInt(root, "RandomizerChance40", ref exceptions, "LoadXml() RandomizerChance40", RandomizerChance40);
                RandomizerChance50 = GetInt(root, "RandomizerChance50", ref exceptions, "LoadXml() RandomizerChance50", RandomizerChance50);
                RandomizerChance60 = GetInt(root, "RandomizerChance60", ref exceptions, "LoadXml() RandomizerChance60", RandomizerChance60);
                RandomizerChance70 = GetInt(root, "RandomizerChance70", ref exceptions, "LoadXml() RandomizerChance70", RandomizerChance70);
                RandomizerChance80 = GetInt(root, "RandomizerChance80", ref exceptions, "LoadXml() RandomizerChance80", RandomizerChance80);
                RandomizerChance90 = GetInt(root, "RandomizerChance90", ref exceptions, "LoadXml() RandomizerChance90", RandomizerChance90);
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

                HudEnable = GetBool(root, "HudEnable", ref exceptions, "LoadXml() HudEnable", HudEnable);

                HudTimerEnable = GetBool(root, "HudTimerEnable", ref exceptions, "LoadXml() HudTimerEnable", HudTimerEnable);

                HudTimerShowTimer = GetBool(root, "HudTimerShowTimer", ref exceptions, "LoadXml() HudTimerShowTimer", HudTimerShowTimer);
                HudTimerLocationLocked = GetBool(root, "HudTimerLocationLocked", ref exceptions, "LoadXml() HudTimerLocationLocked", HudTimerLocationLocked);
                HudTimerShowHandCount = GetBool(root, "HudTimerShowHandCount", ref exceptions, "LoadXml() HudTimerShowHandCount", HudTimerShowHandCount);
                HudTimerDiff = GetInt(root, "HudTimerDiff", ref exceptions, "LoadXml() HudTimerDiff", HudTimerDiff);
                HudTimerHHNotFound = GetString(root, "HudTimerHHNotFound", ref exceptions, "LoadXml() HudTimerHHNotFound", HudTimerHHNotFound);
                HudTimerPokerTypeNotFound = GetString(root, "HudTimerPokerTypeNotFound", ref exceptions, "LoadXml() HudTimerPokerTypeNotFound", HudTimerPokerTypeNotFound);
                HudTimerMultiplePokerTypes = GetString(root, "HudTimerMultiplePokerTypes", ref exceptions, "LoadXml() HudTimerMultiplePokerTypes", HudTimerMultiplePokerTypes);

                HudBigBlindEnable = GetBool(root, "HudBigBlindEnable", ref exceptions, "LoadXml() HudBigBlindEnable", HudBigBlindEnable);
                HudBigBlindLocationLocked = GetBool(root, "HudBigBlindLocationLocked", ref exceptions, "LoadXml() HudBigBlindLocationLocked", HudBigBlindLocationLocked);
                HudBigBlindShowBB = GetBool(root, "HudBigBlindShowBB", ref exceptions, "LoadXml() HudBigBlindShowBB", HudBigBlindShowBB);
                HudBigBlindShowAdjustedBB = GetBool(root, "HudBigBlindShowAdjustedBB", ref exceptions, "LoadXml() HudBigBlindShowAdjustedBB", HudBigBlindShowAdjustedBB);
                HudBigBlindShowTournamentM = GetBool(root, "HudBigBlindShowTournamentM", ref exceptions, "LoadXml() HudBigBlindShowTournamentM", HudBigBlindShowTournamentM);
                HudBigBlindMByPlayerCount = GetBool(root, "HudBigBlindMByPlayerCount", ref exceptions, "LoadXml() HudBigBlindMByPlayerCount", HudBigBlindMByPlayerCount);
                HudBigBlindMByTableSize = GetBool(root, "HudBigBlindMByTableSize", ref exceptions, "LoadXml() HudBigBlindMByTableSize", HudBigBlindMByTableSize);
                HudBigBlindShowForOpponents = GetBool(root, "HudBigBlindShowForOpponents", ref exceptions, "LoadXml() HudBigBlindShowForOpponents", HudBigBlindShowForOpponents);
                HudBigBlindShowForHero = GetBool(root, "HudBigBlindShowForHero", ref exceptions, "LoadXml() HudBigBlindShowForHero", HudBigBlindShowForHero);
                HudBigBlindDecimals = GetInt(root, "HudBigBlindDecimals", ref exceptions, "LoadXml() HudBigBlindDecimals", HudBigBlindDecimals);
                HudBigBlindHHNotFound = GetString(root, "HudBigBlindHHNotFound", ref exceptions, "LoadXml() HudBigBlindHHNotFound", HudBigBlindHHNotFound);
                HudBigBlindPrefix = GetString(root, "HudBigBlindPrefix", ref exceptions, "LoadXml() HudBigBlindPrefix", HudBigBlindPrefix);
                HudBigBlindPostfix = GetString(root, "HudBigBlindPostfix", ref exceptions, "LoadXml() HudBigBlindPostfix", HudBigBlindPostfix);

                #endregion

                #region Hud Design

                HudTimerBackground = GetColor(root, "HudTimerBackground", ref exceptions, "LoadXml() HudTimerBackground", HudTimerBackground);
                HudTimerForeground = GetColor(root, "HudTimerForeground", ref exceptions, "LoadXml() HudTimerForeground", HudTimerForeground);
                HudTimerFontFamily = GetFontFamily(root, "HudTimerFontFamily", ref exceptions, "LoadXml() HudTimerFontFamily", HudTimerFontFamily);
                HudTimerFontWeight = GetFontWeight(root, "HudTimerFontWeight", ref exceptions, "LoadXml() HudTimerFontWeight", HudTimerFontWeight);
                HudTimerFontStyle = GetFontStyle(root, "HudTimerFontStyle", ref exceptions, "LoadXml() HudTimerFontStyle", HudTimerFontStyle);
                HudTimerFontSize = GetDouble(root, "HudTimerFontSize", ref exceptions, "LoadXml() HudTimerFontSize", HudTimerFontSize);
                HudTimerMargin = GetThickness(root, "HudTimerMargin", ref exceptions, "LoadXml() HudTimerMargin", HudTimerMargin);
                HudTimerBorderBrush = GetColor(root, "HudTimerBorderBrush", ref exceptions, "LoadXml() HudTimerBorderBrush", HudTimerBorderBrush);
                HudTimerBorderThickness = GetThickness(root, "HudTimerBorderThickness", ref exceptions, "LoadXml() HudTimerBorderThickness", HudTimerBorderThickness);
                HudTimerCornerRadius = GetCornerRadius(root, "HudTimerCornerRadius", ref exceptions, "LoadXml() HudTimerCornerRadius", HudTimerCornerRadius);

                HudBigBlindOpponentsBackground = GetColor(root, "HudBigBlindOpponentsBackground", ref exceptions, "LoadXml() HudBigBlindOpponentsBackground", HudBigBlindOpponentsBackground);
                HudBigBlindOpponentsForeground = GetColor(root, "HudBigBlindOpponentsForeground", ref exceptions, "LoadXml() HudBigBlindOpponentsForeground", HudBigBlindOpponentsForeground);
                HudBigBlindOpponentsFontFamily = GetFontFamily(root, "HudBigBlindOpponentsFontFamily", ref exceptions, "LoadXml() HudBigBlindOpponentsFontFamily", HudBigBlindOpponentsFontFamily);
                HudBigBlindOpponentsFontWeight = GetFontWeight(root, "HudBigBlindOpponentsFontWeight", ref exceptions, "LoadXml() HudBigBlindOpponentsFontWeight", HudBigBlindOpponentsFontWeight);
                HudBigBlindOpponentsFontStyle = GetFontStyle(root, "HudBigBlindOpponentsFontStyle", ref exceptions, "LoadXml() HudBigBlindOpponentsFontStyle", HudBigBlindOpponentsFontStyle);
                HudBigBlindOpponentsFontSize = GetDouble(root, "HudBigBlindOpponentsFontSize", ref exceptions, "LoadXml() HudBigBlindOpponentsFontSize", HudBigBlindOpponentsFontSize);
                HudBigBlindOpponentsMargin = GetThickness(root, "HudBigBlindOpponentsMargin", ref exceptions, "LoadXml() HudBigBlindOpponentsMargin", HudBigBlindOpponentsMargin);
                HudBigBlindOpponentsBorderBrush = GetColor(root, "HudBigBlindOpponentsBorderBrush", ref exceptions, "LoadXml() HudBigBlindOpponentsBorderBrush", HudBigBlindOpponentsBorderBrush);
                HudBigBlindOpponentsBorderThickness = GetThickness(root, "HudBigBlindOpponentsBorderThickness", ref exceptions, "LoadXml() HudBigBlindOpponentsBorderThickness", HudBigBlindOpponentsBorderThickness);
                HudBigBlindOpponentsCornerRadius = GetCornerRadius(root, "HudBigBlindOpponentsCornerRadius", ref exceptions, "LoadXml() HudBigBlindOpponentsCornerRadius", HudBigBlindOpponentsCornerRadius);

                HudBigBlindHeroBackground = GetColor(root, "HudBigBlindHeroBackground", ref exceptions, "LoadXml() HudBigBlindHeroBackground", HudBigBlindHeroBackground);
                HudBigBlindHeroForeground = GetColor(root, "HudBigBlindHeroForeground", ref exceptions, "LoadXml() HudBigBlindHeroForeground", HudBigBlindHeroForeground);
                HudBigBlindHeroFontFamily = GetFontFamily(root, "HudBigBlindHeroFontFamily", ref exceptions, "LoadXml() HudBigBlindHeroFontFamily", HudBigBlindHeroFontFamily);
                HudBigBlindHeroFontWeight = GetFontWeight(root, "HudBigBlindHeroFontWeight", ref exceptions, "LoadXml() HudBigBlindHeroFontWeight", HudBigBlindHeroFontWeight);
                HudBigBlindHeroFontStyle = GetFontStyle(root, "HudBigBlindHeroFontStyle", ref exceptions, "LoadXml() HudBigBlindHeroFontStyle", HudBigBlindHeroFontStyle);
                HudBigBlindHeroFontSize = GetDouble(root, "HudBigBlindHeroFontSize", ref exceptions, "LoadXml() HudBigBlindHeroFontSize", HudBigBlindHeroFontSize);
                HudBigBlindHeroMargin = GetThickness(root, "HudBigBlindHeroMargin", ref exceptions, "LoadXml() HudBigBlindHeroMargin", HudBigBlindHeroMargin);
                HudBigBlindHeroBorderBrush = GetColor(root, "HudBigBlindHeroBorderBrush", ref exceptions, "LoadXml() HudBigBlindHeroBorderBrush", HudBigBlindHeroBorderBrush);
                HudBigBlindHeroBorderThickness = GetThickness(root, "HudBigBlindHeroBorderThickness", ref exceptions, "LoadXml() HudBigBlindHeroBorderThickness", HudBigBlindHeroBorderThickness);
                HudBigBlindHeroCornerRadius = GetCornerRadius(root, "HudBigBlindHeroCornerRadius", ref exceptions, "LoadXml() HudBigBlindHeroCornerRadius", HudBigBlindHeroCornerRadius);

                try
                {
                    foreach (XElement element in GetXElement(root, "HudBigBlindOpponentsColorsByValue", ref exceptions, "LoadXml() HudBigBlindOpponentsColorsByValue", new XElement("HudBigBlindOpponentsColorsByValue")).Elements("ColorByValue"))
                    {
                        ColorByValue colorByValue = ColorByValue.FromXElement(element, ref exceptions, "LoadXml() ColorByValue");
                        if (colorByValue != null && HudBigBlindOpponentsColorsByValue.Count < 3)
                        {
                            HudBigBlindOpponentsColorsByValue.Add(colorByValue);
                        }
                    }
                }
                catch
                {
                }
                try
                {
                    foreach (XElement element in GetXElement(root, "HudBigBlindHeroColorsByValue", ref exceptions, "LoadXml() HudBigBlindHeroColorsByValue", new XElement("HudBigBlindHeroColorsByValue")).Elements("ColorByValue"))
                    {
                        ColorByValue colorByValue = ColorByValue.FromXElement(element, ref exceptions, "LoadXml() ColorByValue");
                        if (colorByValue != null && HudBigBlindHeroColorsByValue.Count < 3)
                        {
                            HudBigBlindHeroColorsByValue.Add(colorByValue);
                        }
                    }
                }
                catch (Exception)
                {
                }

                #endregion

                EnableTableTiler = GetBool(root, "EnableTableTiler", ref exceptions, "LoadXml() EnableTableTiler", EnableTableTiler);
                AutoTileCheckingTimeMs = GetInt(root, "AutoTileCheckingTimeMs", ref exceptions, "LoadXml() AutoTileCheckingTimeMs", 3000);

                #region Preferred Seat / Hud Timer+BigBlind LocationsXY

                for (int tableSize = 0; tableSize < 11; tableSize++)
                {
                    PreferredSeat[tableSize] = GetInt(root, string.Format("PreferredSeat_{0}", tableSize), ref exceptions, string.Format("LoadXml() PreferredSeat_{0}", tableSize), PreferredSeat[tableSize]);
                }

                for (int tableSize = 0; tableSize < 11; tableSize++)
                {
                    try
                    {
                        var xy = GetString(root, string.Format("HudTimerLocationsXY_{0}", tableSize), ref exceptions, string.Format("LoadXml() HudTimerLocationsXY_{0}", tableSize), string.Format("{0} {1}", DefaultHudTimerLocationsX[tableSize], DefaultHudTimerLocationsY[tableSize])).Split(' ');
                        HudTimerLocationsX[tableSize] = double.Parse(xy[0]);
                        HudTimerLocationsY[tableSize] = double.Parse(xy[1]);
                    }
                    catch
                    {
                    }
                }

                for (int tableSize = 0; tableSize < 11; tableSize++)
                {
                    for (int position = 0; position < 10; position++)
                    {
                        try
                        {
                            var xy = GetString(root, string.Format("HudBigBlindLocationsXY_{0}_{1}", tableSize, position), ref exceptions, string.Format("LoadXml() HudBigBlindLocationsXY_{0}_{1}", tableSize, position),
                                string.Format("{0} {1}", DefaultHudBigBlindLocationsX[tableSize][position], DefaultHudBigBlindLocationsX[tableSize][position])).Split(' ');
                            HudBigBlindLocationsX[tableSize][position] = double.Parse(xy[0]);
                            HudBigBlindLocationsY[tableSize][position] = double.Parse(xy[1]);
                        }
                        catch
                        {
                        }
                    }
                }

                #endregion

                App.TableTileManager.FromXElement(root.Element("TableTiles"), ref exceptions, "LoadXml()");
                App.PokerTypeManager.FromXElement(root.Element("PokerTypes"), ref exceptions, "LoadXml()");
            }
            catch (Exception e)
            {
                exceptions.Add(new ExceptionPsHandler(e, "LoadXml() Main Exception"));
            }

            App.PokerTypeManager.SeedDefaultValues();
            App.TableTileManager.SeedDefaultValues();
            CollectRecentColors();

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
                Set(root, "GnuGplV3Agreement", GnuGplV3Agreement, ref exceptions, "SaveXml() GnuGplV3Agreement");

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

                Set(root, "HudEnable", HudEnable, ref exceptions, "SaveXml() HudEnable");

                Set(root, "HudTimerEnable", HudTimerEnable, ref exceptions, "SaveXml() HudTimerEnable");
                Set(root, "HudTimerLocationLocked", HudTimerLocationLocked, ref exceptions, "SaveXml() HudTimerLocationLocked");
                Set(root, "HudTimerShowTimer", HudTimerShowTimer, ref exceptions, "SaveXml() HudTimerShowTimer");
                Set(root, "HudTimerShowHandCount", HudTimerShowHandCount, ref exceptions, "SaveXml() HudTimerShowHandCount");
                Set(root, "HudTimerDiff", HudTimerDiff, ref exceptions, "SaveXml() HudTimerDiff");
                Set(root, "HudTimerHHNotFound", HudTimerHHNotFound, ref exceptions, "SaveXml() HudTimerHHNotFound");
                Set(root, "HudTimerPokerTypeNotFound", HudTimerPokerTypeNotFound, ref exceptions, "SaveXml() HudTimerPokerTypeNotFound");
                Set(root, "HudTimerMultiplePokerTypes", HudTimerMultiplePokerTypes, ref exceptions, "SaveXml() HudTimerMultiplePokerTypes");

                Set(root, "HudBigBlindEnable", HudBigBlindEnable, ref exceptions, "SaveXml() HudBigBlindEnable");
                Set(root, "HudBigBlindLocationLocked", HudBigBlindLocationLocked, ref exceptions, "SaveXml() HudBigBlindLocationLocked");
                Set(root, "HudBigBlindShowBB", HudBigBlindShowBB, ref exceptions, "SaveXml() HudBigBlindShowBB");
                Set(root, "HudBigBlindShowAdjustedBB", HudBigBlindShowAdjustedBB, ref exceptions, "SaveXml() HudBigBlindShowAdjustedBB");
                Set(root, "HudBigBlindShowTournamentM", HudBigBlindShowTournamentM, ref exceptions, "SaveXml() HudBigBlindShowTournamentM");
                Set(root, "HudBigBlindMByPlayerCount", HudBigBlindMByPlayerCount, ref exceptions, "SaveXml() HudBigBlindMByPlayerCount");
                Set(root, "HudBigBlindMByTableSize", HudBigBlindMByTableSize, ref exceptions, "SaveXml() HudBigBlindMByTableSize");
                Set(root, "HudBigBlindShowForOpponents", HudBigBlindShowForOpponents, ref exceptions, "SaveXml() HudBigBlindShowForOpponents");
                Set(root, "HudBigBlindShowForHero", HudBigBlindShowForHero, ref exceptions, "SaveXml() HudBigBlindShowForHero");
                Set(root, "HudBigBlindDecimals", HudBigBlindDecimals, ref exceptions, "SaveXml() HudBigBlindDecimals");
                Set(root, "HudBigBlindHHNotFound", HudBigBlindHHNotFound, ref exceptions, "SaveXml() HudBigBlindHHNotFound");
                Set(root, "HudBigBlindPrefix", HudBigBlindPrefix, ref exceptions, "SaveXml() HudBigBlindPrefix");
                Set(root, "HudBigBlindPostfix", HudBigBlindPostfix, ref exceptions, "SaveXml() HudBigBlindPostfix");

                #endregion

                #region Hud Design

                Set(root, "HudTimerBackground", HudTimerBackground, ref exceptions, "SaveXml() HudTimerBackground");
                Set(root, "HudTimerForeground", HudTimerForeground, ref exceptions, "SaveXml() HudTimerForeground");
                Set(root, "HudTimerFontFamily", HudTimerFontFamily, ref exceptions, "SaveXml() HudTimerFontFamily");
                Set(root, "HudTimerFontWeight", HudTimerFontWeight, ref exceptions, "SaveXml() HudTimerFontWeight");
                Set(root, "HudTimerFontStyle", HudTimerFontStyle, ref exceptions, "SaveXml() HudTimerFontStyle");
                Set(root, "HudTimerFontSize", HudTimerFontSize, ref exceptions, "SaveXml() HudTimerFontSize");
                Set(root, "HudTimerMargin", HudTimerMargin, ref exceptions, "SaveXml() HudTimerMargin");
                Set(root, "HudTimerBorderBrush", HudTimerBorderBrush, ref exceptions, "SaveXml() HudTimerBorderBrush");
                Set(root, "HudTimerBorderThickness", HudTimerBorderThickness, ref exceptions, "SaveXml() HudTimerBorderThickness");
                Set(root, "HudTimerCornerRadius", HudTimerCornerRadius, ref exceptions, "SaveXml() HudTimerCornerRadius");

                Set(root, "HudBigBlindOpponentsBackground", HudBigBlindOpponentsBackground, ref exceptions, "SaveXml() HudBigBlindOpponentsBackground");
                Set(root, "HudBigBlindOpponentsForeground", HudBigBlindOpponentsForeground, ref exceptions, "SaveXml() HudBigBlindOpponentsForeground");
                Set(root, "HudBigBlindOpponentsFontFamily", HudBigBlindOpponentsFontFamily, ref exceptions, "SaveXml() HudBigBlindOpponentsFontFamily");
                Set(root, "HudBigBlindOpponentsFontWeight", HudBigBlindOpponentsFontWeight, ref exceptions, "SaveXml() HudBigBlindOpponentsFontWeight");
                Set(root, "HudBigBlindOpponentsFontStyle", HudBigBlindOpponentsFontStyle, ref exceptions, "SaveXml() HudBigBlindOpponentsFontStyle");
                Set(root, "HudBigBlindOpponentsFontSize", HudBigBlindOpponentsFontSize, ref exceptions, "SaveXml() HudBigBlindOpponentsFontSize");
                Set(root, "HudBigBlindOpponentsMargin", HudBigBlindOpponentsMargin, ref exceptions, "SaveXml() HudBigBlindOpponentsMargin");
                Set(root, "HudBigBlindOpponentsBorderBrush", HudBigBlindOpponentsBorderBrush, ref exceptions, "SaveXml() HudBigBlindOpponentsBorderBrush");
                Set(root, "HudBigBlindOpponentsBorderThickness", HudBigBlindOpponentsBorderThickness, ref exceptions, "SaveXml() HudBigBlindOpponentsBorderThickness");
                Set(root, "HudBigBlindOpponentsCornerRadius", HudBigBlindOpponentsCornerRadius, ref exceptions, "SaveXml() HudBigBlindOpponentsCornerRadius");

                Set(root, "HudBigBlindHeroBackground", HudBigBlindHeroBackground, ref exceptions, "SaveXml() HudBigBlindHeroBackground");
                Set(root, "HudBigBlindHeroForeground", HudBigBlindHeroForeground, ref exceptions, "SaveXml() HudBigBlindHeroForeground");
                Set(root, "HudBigBlindHeroFontFamily", HudBigBlindHeroFontFamily, ref exceptions, "SaveXml() HudBigBlindHeroFontFamily");
                Set(root, "HudBigBlindHeroFontWeight", HudBigBlindHeroFontWeight, ref exceptions, "SaveXml() HudBigBlindHeroFontWeight");
                Set(root, "HudBigBlindHeroFontStyle", HudBigBlindHeroFontStyle, ref exceptions, "SaveXml() HudBigBlindHeroFontStyle");
                Set(root, "HudBigBlindHeroFontSize", HudBigBlindHeroFontSize, ref exceptions, "SaveXml() HudBigBlindHeroFontSize");
                Set(root, "HudBigBlindHeroMargin", HudBigBlindHeroMargin, ref exceptions, "SaveXml() HudBigBlindHeroMargin");
                Set(root, "HudBigBlindHeroBorderBrush", HudBigBlindHeroBorderBrush, ref exceptions, "SaveXml() HudBigBlindHeroBorderBrush");
                Set(root, "HudBigBlindHeroBorderThickness", HudBigBlindHeroBorderThickness, ref exceptions, "SaveXml() HudBigBlindHeroBorderThickness");
                Set(root, "HudBigBlindHeroCornerRadius", HudBigBlindHeroCornerRadius, ref exceptions, "SaveXml() HudBigBlindHeroCornerRadius");

                XElement xBigBlindOpponentsColorsByValue = new XElement("HudBigBlindOpponentsColorsByValue");
                root.Add(xBigBlindOpponentsColorsByValue);
                foreach (var item in HudBigBlindOpponentsColorsByValue)
                {
                    xBigBlindOpponentsColorsByValue.Add(item.ToXElement());
                }

                XElement xBigBlindHeroColorsByValue = new XElement("HudBigBlindHeroColorsByValue");
                root.Add(xBigBlindHeroColorsByValue);
                foreach (var item in HudBigBlindHeroColorsByValue)
                {
                    xBigBlindHeroColorsByValue.Add(item.ToXElement());
                }

                #endregion

                Set(root, "EnableTableTiler", EnableTableTiler, ref exceptions, "SaveXml() EnableTableTiler");
                Set(root, "AutoTileCheckingTimeMs", AutoTileCheckingTimeMs, ref exceptions, "SaveXml() AutoTileCheckingTimeMs");

                #region Preferred Seat / Hud Timer+BigBlind LocationsXY

                for (int tableSize = 0; tableSize < 11; tableSize++)
                {
                    Set(root, string.Format("PreferredSeat_{0}", tableSize), string.Format("{0}", PreferredSeat[tableSize]), ref exceptions, string.Format("SaveXml() PreferredSeat_{0}", tableSize));
                }

                for (int tableSize = 0; tableSize < 11; tableSize++)
                {
                    Set(root, string.Format("HudTimerLocationsXY_{0}", tableSize), string.Format("{0} {1}", HudTimerLocationsX[tableSize], HudTimerLocationsY[tableSize]), ref exceptions, string.Format("SaveXml() HudTimerLocationsXY_{0}", tableSize));
                }

                for (int tableSize = 0; tableSize < 11; tableSize++)
                {
                    for (int position = 0; position < 10; position++)
                    {
                        Set(root, string.Format("HudBigBlindLocationsXY_{0}_{1}", tableSize, position), string.Format("{0} {1}", HudBigBlindLocationsX[tableSize][position], HudBigBlindLocationsY[tableSize][position]), ref exceptions, string.Format("SaveXml() HudBigBlindLocationsXY_{0}_{1}", tableSize, position));
                    }
                }

                #endregion

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

        //

        private static void CollectRecentColors()
        {
            UcColorPicker.AddRecentColor(HudTimerBackground);
            UcColorPicker.AddRecentColor(HudTimerForeground);
            UcColorPicker.AddRecentColor(HudBigBlindOpponentsBackground);
            UcColorPicker.AddRecentColor(HudBigBlindOpponentsForeground);
            UcColorPicker.AddRecentColor(HudBigBlindHeroBackground);
            UcColorPicker.AddRecentColor(HudBigBlindHeroForeground);
            foreach (var colorByValue in HudBigBlindOpponentsColorsByValue)
            {
                UcColorPicker.AddRecentColor(colorByValue.Color);
            }
            foreach (var colorByValue in HudBigBlindHeroColorsByValue)
            {
                UcColorPicker.AddRecentColor(colorByValue.Color);
            }
        }

        public static void VersionControl(XElement root, int version)
        {
            try
            {
                if (version < 27)
                {
                    // poker types delaut override:
                    var defaultPokerTypes = PokerType.GetDefaultValues().ToList();
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

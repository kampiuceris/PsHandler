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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Xml.Linq;

namespace PsHandler.TableTiler
{
    public enum AutoTileMethod
    {
        ToTheTopSlot,
        ToTheClosestSlot,
    }

    public class TableTile
    {
        public bool IsEnabled;

        public string Name = "";
        public KeyCombination KeyCombination = new KeyCombination(Key.None, false, false, false);
        public bool SortByStartingHand = false;
        public bool BringToFront = false;
        public bool AutoTile = false;
        public AutoTileMethod AutoTileMethod = AutoTileMethod.ToTheTopSlot;
        public Regex RegexWindowTitle = new Regex("");
        public Regex RegexWindowClass = new Regex("");
        public Rectangle[] XYWHs = new Rectangle[0];
        public int TableCountEqualOrGreaterThan = 1;
        public int TableCountEqualOrLessThan = 100;

        public static IEnumerable<TableTile> GetDefaultValues()
        {
            List<TableTile> tableTilesDefault = new List<TableTile>();

            List<Rectangle> rects = new List<Rectangle>();
            int offsetX = 0, offsetY = 0;

            //

            rects.Clear();
            offsetX = 0;
            offsetY = 0;
            for (int i = 0; i < 8; i++)
            {
                rects.Add(new Rectangle(
                    0 + offsetX,
                    0 + offsetY,
                    Config.POKERSTARS_TABLE_CLIENT_SIZE_DEFAULT.Width + Config.WINDOWS_BORDER_THICKNESS + Config.WINDOWS_BORDER_THICKNESS,
                    Config.POKERSTARS_TABLE_CLIENT_SIZE_DEFAULT.Height + Config.WINDOWS_BORDER_THICKNESS + Config.WINDOWS_BORDER_THICKNESS + Config.WINDOWS_TITLE_BORDER_THICKNESS
                    ));
                offsetX += Config.WINDOWS_TITLE_BORDER_THICKNESS + Config.WINDOWS_BORDER_THICKNESS;
                offsetY += Config.WINDOWS_TITLE_BORDER_THICKNESS + Config.WINDOWS_BORDER_THICKNESS;
            }
            tableTilesDefault.Add(new TableTile
            {
                IsEnabled = false,
                KeyCombination = new KeyCombination(Key.None, false, false, false),
                SortByStartingHand = true,
                Name = "Sample: Cascade All PokerStars Tables",
                RegexWindowTitle = new Regex(""),
                RegexWindowClass = new Regex(@"\APokerStarsTableFrameClass\z"),
                XYWHs = rects.ToArray()
            });

            //

            rects.Clear();
            offsetX = 0;
            offsetY = 0;
            for (int i = 0; i < 6; i++)
            {
                rects.Add(new Rectangle(
                    (i % 3) * (Config.WINDOWS_BORDER_THICKNESS + Config.POKERSTARS_TABLE_CLIENT_SIZE_MIN.Width + Config.WINDOWS_BORDER_THICKNESS),
                    (i / 3) * (Config.WINDOWS_BORDER_THICKNESS + Config.WINDOWS_TITLE_BORDER_THICKNESS + Config.POKERSTARS_TABLE_CLIENT_SIZE_MIN.Height + Config.WINDOWS_BORDER_THICKNESS),
                    Config.WINDOWS_BORDER_THICKNESS + Config.POKERSTARS_TABLE_CLIENT_SIZE_MIN.Width + Config.WINDOWS_BORDER_THICKNESS,
                    Config.WINDOWS_BORDER_THICKNESS + Config.WINDOWS_TITLE_BORDER_THICKNESS + Config.POKERSTARS_TABLE_CLIENT_SIZE_MIN.Height + Config.WINDOWS_BORDER_THICKNESS
                    ));
            }
            tableTilesDefault.Add(new TableTile
            {
                IsEnabled = false,
                KeyCombination = new KeyCombination(Key.None, false, false, false),
                SortByStartingHand = true,
                Name = "Sample: Tile All PokerStars Tables",
                RegexWindowTitle = new Regex(""),
                RegexWindowClass = new Regex(@"\APokerStarsTableFrameClass\z"),
                XYWHs = rects.ToArray()
            });

            return tableTilesDefault;
        }

        public static AutoTileMethod ParseAutoTileMethod(string text)
        {
            return Enum.GetValues(typeof(AutoTileMethod)).Cast<AutoTileMethod>().FirstOrDefault(item => item.ToString().Equals(text));
        }

        public XElement ToXElement()
        {
            return new XElement("TableTile",
                                    new XElement("Name", Name),
                                    new XElement("IsEnabled", IsEnabled.ToString()),
                                    new XElement("Hotkey", KeyCombination.ToString()),
                                    new XElement("SortByStartingHand", SortByStartingHand.ToString()),
                                    new XElement("BringToFront", BringToFront),
                                    new XElement("AutoTile", AutoTile.ToString()),
                                    new XElement("AutoTileMethod", AutoTileMethod.ToString()),
                                    new XElement("RegexWindowTitle", RegexWindowTitle.ToString()),
                                    new XElement("RegexWindowClass", RegexWindowClass.ToString()),
                                    new XElement("XYWHs", XYWHs.Select(o => new XElement("XYWH", string.Format("{0} {1} {2} {3}", o.X, o.Y, o.Width, o.Height)))),
                                    new XElement("TableCountEqualOrGreaterThan", TableCountEqualOrGreaterThan),
                                    new XElement("TableCountEqualOrLessThan", TableCountEqualOrLessThan)
                                );
        }

        public static TableTile FromXElement(XElement xElement)
        {
            List<ExceptionPsHandler> exceptions = new List<ExceptionPsHandler>();
            return FromXElement(xElement, ref exceptions, "");
        }

        public static TableTile FromXElement(XElement xElement, ref List<ExceptionPsHandler> exceptions, string exceptionHeader)
        {
            var tableTile = new TableTile();

            try
            {
                tableTile.Name = xElement.Element("Name").Value;
            }
            catch (Exception e)
            {
                exceptions.Add(new ExceptionPsHandler(e, exceptionHeader + " TableTile.FromXElement() xElement:" + Environment.NewLine + xElement));
                return null;
            }
            try
            {
                tableTile.IsEnabled = bool.Parse(xElement.Element("IsEnabled").Value);
            }
            catch
            {
            }
            try
            {
                tableTile.KeyCombination = KeyCombination.Parse(xElement.Element("Hotkey").Value);
            }
            catch
            {
            }
            try
            {
                tableTile.SortByStartingHand = bool.Parse(xElement.Element("SortByStartingHand").Value);
            }
            catch
            {
            }
            try
            {
                tableTile.BringToFront = bool.Parse(xElement.Element("BringToFront").Value);
            }
            catch
            {
            }
            try
            {
                tableTile.AutoTile = bool.Parse(xElement.Element("AutoTile").Value);
            }
            catch
            {
            }
            try
            {
                tableTile.AutoTileMethod = ParseAutoTileMethod(xElement.Element("AutoTileMethod").Value);
            }
            catch
            {
            }
            try
            {
                tableTile.RegexWindowTitle = new Regex(xElement.Element("RegexWindowTitle").Value);
            }
            catch
            {
            }
            try
            {
                tableTile.RegexWindowClass = new Regex(xElement.Element("RegexWindowClass").Value);
            }
            catch
            {
            }
            try
            {
                tableTile.XYWHs = xElement.Element("XYWHs") == null ? new Rectangle[0] : xElement.Element("XYWHs").Elements().Select(o =>
                {
                    string[] s = o.Value.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    return new Rectangle(int.Parse(s[0]), int.Parse(s[1]), int.Parse(s[2]), int.Parse(s[3]));
                }).ToArray();
            }
            catch
            {
            }
            try
            {
                tableTile.TableCountEqualOrGreaterThan = int.Parse(xElement.Element("TableCountEqualOrGreaterThan").Value);
            }
            catch
            {
            }
            try
            {
                tableTile.TableCountEqualOrLessThan = int.Parse(xElement.Element("TableCountEqualOrLessThan").Value);
            }
            catch
            {
            }

            return tableTile;
        }
    }
}

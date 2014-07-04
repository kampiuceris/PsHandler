using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Xml.Linq;

namespace PsHandler.TableTiler
{
    public class TableTile
    {
        public string Name = "";
        public KeyCombination KeyCombination;
        public bool SortByStartingHand;
        public string[] IncludeAnd = new string[0];
        public string[] IncludeOr = new string[0];
        public string[] ExcludeAnd = new string[0];
        public string[] ExcludeOr = new string[0];
        public string WindowClass = "";
        public Rectangle[] XYWHs = new Rectangle[0];
        public bool IsEnabled;

        public override string ToString()
        {
            return Name;
        }

        public string ToXml()
        {
            return new XDocument(ToXElement()).ToString();
        }

        public XElement ToXElement()
        {
            return new XElement("TableTile",
                                     new XElement("Name", Name),
                                     new XElement("Hotkey", KeyCombination.ToString()),
                                     new XElement("SortByStartingHand", SortByStartingHand),
                                     new XElement("IncludeAnd", IncludeAnd.Select(s => new XElement("Text", s))),
                                     new XElement("IncludeOr", IncludeOr.Select(s => new XElement("Text", s))),
                                     new XElement("ExcludeAnd", ExcludeAnd.Select(s => new XElement("Text", s))),
                                     new XElement("ExcludeOr", ExcludeOr.Select(s => new XElement("Text", s))),
                                     new XElement("WindowClass", WindowClass),
                                     new XElement("XYWHs", XYWHs.Select(s => new XElement("XYWH", string.Format("{0} {1} {2} {3}", s.X, s.Y, s.Width, s.Height)))),
                                     new XElement("IsEnabled", IsEnabled)
                                );
        }

        public static TableTile FromXml(string xml)
        {
            try
            {
                return FromXElement(XDocument.Parse(xml).Root);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static TableTile FromXElement(XElement xElement)
        {
            try
            {
                return new TableTile
                {
                    Name = xElement.Element("Name") == null ? "" : xElement.Element("Name").Value,
                    KeyCombination = xElement.Element("Hotkey") == null ? new KeyCombination(Key.None, false, false, false) : KeyCombination.Parse(xElement.Element("Hotkey").Value),
                    SortByStartingHand = xElement.Element("SortByStartingHand") == null ? false : bool.Parse(xElement.Element("SortByStartingHand").Value),
                    IncludeAnd = xElement.Element("IncludeAnd") == null ? new string[0] : xElement.Element("IncludeAnd").Elements().Select(o => o.Value).ToArray(),
                    IncludeOr = xElement.Element("IncludeOr") == null ? new string[0] : xElement.Element("IncludeOr").Elements().Select(o => o.Value).ToArray(),
                    ExcludeAnd = xElement.Element("ExcludeAnd") == null ? new string[0] : xElement.Element("ExcludeAnd").Elements().Select(o => o.Value).ToArray(),
                    ExcludeOr = xElement.Element("ExcludeOr") == null ? new string[0] : xElement.Element("ExcludeOr").Elements().Select(o => o.Value).ToArray(),
                    WindowClass = xElement.Element("WindowClass") == null ? "" : xElement.Element("WindowClass").Value,
                    XYWHs = xElement.Element("XYWHs") == null ? new Rectangle[0] : xElement.Element("XYWHs").Elements().Select(o =>
                    {
                        string[] s = o.Value.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        return new Rectangle(int.Parse(s[0]), int.Parse(s[1]), int.Parse(s[2]), int.Parse(s[3]));
                    }).ToArray(),
                    IsEnabled = xElement.Element("IsEnabled") == null ? false : bool.Parse(xElement.Element("IsEnabled").Value),
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IEnumerable<TableTile> GetDefaultValues()
        {
            List<TableTile> collection = new List<TableTile>();

            TableTile tt;

            // Sample: Cascade Cash

            tt = new TableTile
            {
                IsEnabled = false,
                KeyCombination = new KeyCombination(Key.None, false, false, false),
                SortByStartingHand = false,
                Name = "Sample: Cascade Cash (No Limit)",
                IncludeAnd = new[] { "Logged In as" },
                IncludeOr = new[] { "$", "€", "£" },
                ExcludeAnd = new string[0],
                ExcludeOr = new[] { "Tournament" },
                WindowClass = "",
                XYWHs = new Rectangle[]
			    {
				    new Rectangle(0, 0, 808, 580),
				    new Rectangle(34, 34, 808, 580),
				    new Rectangle(68, 68, 808, 580),
				    new Rectangle(102, 102, 808, 580),
				    new Rectangle(136, 136, 808, 580),
				    new Rectangle(170, 170, 808, 580),
				    new Rectangle(204, 204, 808, 580),
				    new Rectangle(238, 238, 808, 580),
			    }
            };
            collection.Add(tt);
            // Sample: Tile Tournament Sort

            tt = new TableTile
            {
                IsEnabled = false,
                KeyCombination = new KeyCombination(Key.None, false, false, false),
                SortByStartingHand = true,
                Name = "Sample: Tile Tournament Sort",
                IncludeAnd = new[] { "Logged In as", "Tournament" },
                IncludeOr = new[] { "$", "€", "£" },
                ExcludeAnd = new string[0],
                ExcludeOr = new string[0],
                WindowClass = "",
                XYWHs = new Rectangle[]
			    {
				    new Rectangle(0, 0, 534, 391),
				    new Rectangle(534, 0, 534, 391),
				    new Rectangle(1068, 0, 534, 391),
				    new Rectangle(0, 390, 534, 391),
				    new Rectangle(534, 390, 534, 391),
				    new Rectangle(1068, 390, 534, 391),
			    }
            };
            collection.Add(tt);

            return collection;
        }
    }
}

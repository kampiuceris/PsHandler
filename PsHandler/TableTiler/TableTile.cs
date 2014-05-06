using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
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
        public Rectangle[] XYWHs = new Rectangle[0];
        public bool IsEnabled;

        public override string ToString()
        {
            return Name;
        }

        public string ToXml()
        {
            return new XDocument(
                new XElement("PokerType",
                             new XElement("Name", Name),
                             new XElement("Hotkey", KeyCombination.ToString()),
                             new XElement("SortByStartingHand", SortByStartingHand),
                             new XElement("IncludeAnd", IncludeAnd.Select(s => new XElement("Text", s))),
                             new XElement("IncludeOr", IncludeOr.Select(s => new XElement("Text", s))),
                             new XElement("ExcludeAnd", ExcludeAnd.Select(s => new XElement("Text", s))),
                             new XElement("ExcludeOr", ExcludeOr.Select(s => new XElement("Text", s))),
                             new XElement("XYWHs", XYWHs.Select(s => new XElement("XYWH", string.Format("{0} {1} {2} {3}", s.X, s.Y, s.Width, s.Height)))),
                             new XElement("IsEnabled", IsEnabled)
                    )).ToString();
        }

        public static TableTile FromXml(string xml)
        {
            try
            {
                XDocument xdoc = XDocument.Parse(xml);
                XElement root = xdoc.Root;

                return new TableTile
                {
                    Name = root.Element("Name").Value,
                    KeyCombination = KeyCombination.Parse(root.Element("Hotkey").Value),
                    SortByStartingHand = bool.Parse(root.Element("SortByStartingHand").Value),
                    IncludeAnd = root.Element("IncludeAnd").Elements().Select(xElement => xElement.Value).ToArray(),
                    IncludeOr = root.Element("IncludeOr").Elements().Select(xElement => xElement.Value).ToArray(),
                    ExcludeAnd = root.Element("ExcludeAnd").Elements().Select(xElement => xElement.Value).ToArray(),
                    ExcludeOr = root.Element("ExcludeOr").Elements().Select(xElement => xElement.Value).ToArray(),
                    XYWHs = root.Element("XYWHs").Elements().Select(xElement =>
                    {
                        string[] s = xElement.Value.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        return new Rectangle(int.Parse(s[0]), int.Parse(s[1]), int.Parse(s[2]), int.Parse(s[3]));
                    }).ToArray(),
                    IsEnabled = bool.Parse(root.Element("IsEnabled").Value),
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

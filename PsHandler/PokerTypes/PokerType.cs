using System;
using System.Linq;
using System.Xml.Linq;

namespace PsHandler.PokerTypes
{
    public class PokerType
    {
        public string Name = "";
        public int LevelLengthInSeconds = 0;
        public string[] IncludeAnd = new string[0];
        public string[] IncludeOr = new string[0];
        public string[] ExcludeAnd = new string[0];
        public string[] ExcludeOr = new string[0];
        public string[] BuyInAndRake = new string[0];

        public override string ToString()
        {
            return Name;
        }

        public string ToXml()
        {
            return new XDocument(
                new XElement("PokerType",
                             new XElement("Name", Name),
                             new XElement("LevelLengthInSeconds", LevelLengthInSeconds),
                             new XElement("IncludeAnd", IncludeAnd.Select(s => new XElement("Text", s))),
                             new XElement("IncludeOr", IncludeOr.Select(s => new XElement("Text", s))),
                             new XElement("ExcludeAnd", ExcludeAnd.Select(s => new XElement("Text", s))),
                             new XElement("ExcludeOr", ExcludeOr.Select(s => new XElement("Text", s))),
                             new XElement("BuyInAndRake", BuyInAndRake.Select(s => new XElement("Text", s)))
                    )).ToString();
        }

        public static PokerType FromXml(string xml)
        {
            try
            {
                XDocument xdoc = XDocument.Parse(xml);
                XElement root = xdoc.Root;

                return new PokerType
                {
                    Name = root.Element("Name").Value,
                    LevelLengthInSeconds = int.Parse(root.Element("LevelLengthInSeconds").Value),
                    IncludeAnd = root.Element("IncludeAnd").Elements().Select(xElement => xElement.Value).ToArray(),
                    IncludeOr = root.Element("IncludeOr").Elements().Select(xElement => xElement.Value).ToArray(),
                    ExcludeAnd = root.Element("ExcludeAnd").Elements().Select(xElement => xElement.Value).ToArray(),
                    ExcludeOr = root.Element("ExcludeOr").Elements().Select(xElement => xElement.Value).ToArray(),
                    BuyInAndRake = root.Element("BuyInAndRake").Elements().Select(xElement => xElement.Value).ToArray()
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

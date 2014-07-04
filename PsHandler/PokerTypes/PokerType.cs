using System;
using System.Collections.Generic;
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
        public string WindowClass = "";

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
            return new XElement("PokerType",
                                    new XElement("Name", Name),
                                    new XElement("LevelLengthInSeconds", LevelLengthInSeconds),
                                    new XElement("IncludeAnd", IncludeAnd.Select(s => new XElement("Text", s))),
                                    new XElement("IncludeOr", IncludeOr.Select(s => new XElement("Text", s))),
                                    new XElement("ExcludeAnd", ExcludeAnd.Select(s => new XElement("Text", s))),
                                    new XElement("ExcludeOr", ExcludeOr.Select(s => new XElement("Text", s))),
                                    new XElement("WindowClass", WindowClass),
                                    new XElement("BuyInAndRake", BuyInAndRake.Select(s => new XElement("Text", s)))
                                );
        }

        public static PokerType FromXml(string xml)
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

        public static PokerType FromXElement(XElement xElement)
        {
            try
            {
                return new PokerType
                {
                    Name = xElement.Element("Name") == null ? "" : xElement.Element("Name").Value,
                    LevelLengthInSeconds = xElement.Element("LevelLengthInSeconds") == null ? 0 : int.Parse(xElement.Element("LevelLengthInSeconds").Value),
                    IncludeAnd = xElement.Element("IncludeAnd") == null ? new string[0] : xElement.Element("IncludeAnd").Elements().Select(o => o.Value).ToArray(),
                    IncludeOr = xElement.Element("IncludeOr") == null ? new string[0] : xElement.Element("IncludeOr").Elements().Select(o => o.Value).ToArray(),
                    ExcludeAnd = xElement.Element("ExcludeAnd") == null ? new string[0] : xElement.Element("ExcludeAnd").Elements().Select(o => o.Value).ToArray(),
                    ExcludeOr = xElement.Element("ExcludeOr") == null ? new string[0] : xElement.Element("ExcludeOr").Elements().Select(o => o.Value).ToArray(),
                    WindowClass = xElement.Element("WindowClass") == null ? "" : xElement.Element("WindowClass").Value,
                    BuyInAndRake = xElement.Element("BuyInAndRake") == null ? new string[0] : xElement.Element("BuyInAndRake").Elements().Select(o => o.Value).ToArray()
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IEnumerable<PokerType> GetDefaultValues()
        {
            List<PokerType> collection = new List<PokerType>();

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
                WindowClass = "",
                BuyInAndRake = new[] { "$1.35 + $0.15", "$3.26 + $0.24", "$6.60 + $0.40", "$14.15 + $0.85", "$28.30 + $1.70", "$56.60 + $3.40", "$94.90 + $5.10", "$191.35 + $8.65" }
            };
            collection.Add(pt);

            pt = new PokerType
            {
                Name = "10-max Fifty50 Turbo",
                LevelLengthInSeconds = 180,
                IncludeAnd = new[] { "Logged In as", "Tournament", "Fifty50", "Turbo" },
                IncludeOr = new string[0],
                ExcludeAnd = new string[0],
                ExcludeOr = new[] { "Hyper", "6-Max" },
                WindowClass = "",
                BuyInAndRake = new[] { "$1.39 + $0.11", "$3.30 + $0.20", "$6.68 + $0.32", "$14.31 + $0.69", "$28.63 + $1.37", "$57.25 + $2.75", "$95.86 + $4.14", "$193.05 + $6.95", "$291.60 + $8.40", "$487.20 + $12.80" }
            };
            collection.Add(pt);

            // 2-max

            pt = new PokerType
            {
                Name = "2-max 2 Players Regular",
                LevelLengthInSeconds = 360,
                IncludeAnd = new[] { "Logged In as", "Tournament", "HU", "2 Players" },
                IncludeOr = new string[0],
                ExcludeAnd = new string[0],
                ExcludeOr = new[] { "Turbo", "Hyper", "6-Max" },
                WindowClass = "",
                BuyInAndRake = new[] { "$1.38 + $0.12", "$3.29 + $0.21", "$6.67 + $0.33", "$14.29 + $0.71", "$28.57 + $1.43", "$57.28 + $2.72", "$95.69 + $4.31", "$192.75 + $7.25", "$289.85 + $10.15", "$485.40 + $14.60", "$975.60 + $24.40", "$1956.00 + $44.00", "$4926.00 + $74.00" }
            };
            collection.Add(pt);

            pt = new PokerType
            {
                Name = "2-max 2 Players Turbo",
                LevelLengthInSeconds = 180,
                IncludeAnd = new[] { "Logged In as", "Tournament", "HU", "2 Players", "Turbo" },
                IncludeOr = new string[0],
                ExcludeAnd = new string[0],
                ExcludeOr = new[] { "Hyper", "6-Max" },
                WindowClass = "",
                BuyInAndRake = new[] { "$1.40 + $0.10", "$3.32 + $0.18", "$6.71 + $0.29", "$14.39 + $0.61", "$28.78 + $1.22", "$57.67 + $2.33", "$96.32 + $3.68", "$193.85 + $6.15", "$291.25 + $8.75", "$487.60 + $12.40", "$979.20 + $20.80", "$1962.50 + $37.50", "$4937.00 + $63.00" }
            };
            collection.Add(pt);

            pt = new PokerType
            {
                Name = "2-max 2 Players Hyper-Turbo",
                LevelLengthInSeconds = 120,
                IncludeAnd = new[] { "Logged In as", "Tournament", "HU", "2 Players", "Hyper-Turbo" },
                IncludeOr = new string[0],
                ExcludeAnd = new string[0],
                ExcludeOr = new[] { "6-Max" },
                WindowClass = "",
                BuyInAndRake = new[] { "$1.44 + $0.06", "$3.40 + $0.10", "$6.85 + $0.15", "$14.69 + $0.31", "$29.37 + $0.63", "$58.74 + $1.26", "$98.12 + $1.88", "$196.66 + $3.34", "$295.51 + $4.49", "$493.35 + $6.65", "$988.80 + $11.20" }
            };
            collection.Add(pt);

            // 9-max

            pt = new PokerType
            {
                Name = "9-max Regular",
                LevelLengthInSeconds = 600,
                IncludeAnd = new[] { "Logged In as", "Tournament" },
                IncludeOr = new string[0],
                ExcludeAnd = new string[0],
                ExcludeOr = new[] { "Turbo", "Hyper", "6-Max" },
                WindowClass = "",
                BuyInAndRake = new[] { "$1.29 + $0.21", "$3.11 + $0.39", "$6.37 + $0.63", "$13.70 + $1.30", "$27.40 + $2.60", "$54.80 + $5.20", "$92.15 + $7.85", "$186.50 + $13.50", "$281.00 + $19.00", "$471.75 + $28.25" }
            };
            collection.Add(pt);

            pt = new PokerType
            {
                Name = "9-max Turbo",
                LevelLengthInSeconds = 300,
                IncludeAnd = new[] { "Logged In as", "Tournament", "Turbo" },
                IncludeOr = new string[0],
                ExcludeAnd = new string[0],
                ExcludeOr = new[] { "Hyper", "6-Max" },
                WindowClass = "",
                BuyInAndRake = new[] { "$1.32 + $0.18", "$3.16 + $0.34", "$6.45 + $0.55", "$13.89 + $1.11", "$27.78 + $2.22", "$55.56 + $4.44", "$92.80 + $7.20", "$187.80 + $12.20", "$283.00 + $17.00", "$474.00 + $26.00", "$957.00 + $43.00", "$1923.00 + $77.00" }
            };
            collection.Add(pt);

            pt = new PokerType
            {
                Name = "9-max Hyper-Turbo",
                LevelLengthInSeconds = 120,
                IncludeAnd = new[] { "Logged In as", "Tournament", "Hyper-Turbo" },
                IncludeOr = new string[0],
                ExcludeAnd = new string[0],
                ExcludeOr = new[] { "6-Max" },
                WindowClass = "",
                BuyInAndRake = new[] { "$1.39 + $0.11", "$3.31 + $0.19", "$6.70 + $0.30", "$14.39 + $0.61", "$28.77 + $1.23", "$57.54 + $2.46", "$96.32 + $3.68", "$193.18 + $6.82" }
            };
            collection.Add(pt);

            // 6-max

            pt = new PokerType
            {
                Name = "6-max Regular",
                LevelLengthInSeconds = 600,
                IncludeAnd = new[] { "Logged In as", "Tournament", "6-Max" },
                IncludeOr = new string[0],
                ExcludeAnd = new string[0],
                ExcludeOr = new[] { "Turbo", "Hyper" },
                WindowClass = "",
                BuyInAndRake = new[] { "$1.29 + $0.21", "$3.13 + $0.37", "$6.39 + $0.61", "$13.79 + $1.21", "$27.58 + $2.42", "$55.13 + $4.84", "$92.60 + $7.40", "$186.90 + $13.10", "$281.70 + $18.30", "$472.75 + $27.25" }
            };
            collection.Add(pt);

            pt = new PokerType
            {
                Name = "6-max Turbo",
                LevelLengthInSeconds = 300,
                IncludeAnd = new[] { "Logged In as", "Tournament", "6-Max", "Turbo" },
                IncludeOr = new string[0],
                ExcludeAnd = new string[0],
                ExcludeOr = new[] { "Hyper" },
                WindowClass = "",
                BuyInAndRake = new[] { "$1.32 + $0.18", "$3.19 + $0.31", "$6.48 + $0.52", "$13.92 + $1.08", "$27.84 + $2.16", "$55.68 + $4.32", "$93.25 + $6.75", "$188.20 + $11.80", "$283.70 + $16.30", "$475.00 + $25.00", "$959.25 + $40.75", "$1928.00 + $72.00", "$4878.00 + $122.00" }
            };
            collection.Add(pt);

            pt = new PokerType
            {
                Name = "6-max Hyper-Turbo",
                LevelLengthInSeconds = 120,
                IncludeAnd = new[] { "Logged In as", "Tournament", "6-Max", "Hyper-Turbo" },
                IncludeOr = new string[0],
                ExcludeAnd = new string[0],
                ExcludeOr = new string[0],
                WindowClass = "",
                BuyInAndRake = new[] { "$1.40 + $0.10", "$3.32 + $0.18", "$6.71 + $0.29", "$14.41 + $0.59", "$28.83 + $1.17", "$57.66 + $2.34", "$96.49 + $3.51", "$193.52 + $6.48", "$291.40 + $8.60", "$487.52 + $12.48" }
            };
            collection.Add(pt);

            //  9-max knockout

            pt = new PokerType
            {
                Name = "9-max Knockout Regular",
                LevelLengthInSeconds = 600,
                IncludeAnd = new[] { "Logged In as", "Tournament", "Knockout" },
                IncludeOr = new string[0],
                ExcludeAnd = new string[0],
                ExcludeOr = new[] { "Turbo", "Hyper", "6-Max" },
                WindowClass = "",
                BuyInAndRake = new[] { "$1.35 + $0.15", "$3.20 + $0.30", "$6.50 + $0.50", "$13.90 + $1.10", "$27.80 + $2.20", "$55.75 + $4.25", "$93.60 + $6.40", "$189.05 + $10.95" }
            };
            collection.Add(pt);

            pt = new PokerType
            {
                Name = "9-max Knockout Turbo",
                LevelLengthInSeconds = 300,
                IncludeAnd = new[] { "Logged In as", "Tournament", "Knockout", "Turbo" },
                IncludeOr = new string[0],
                ExcludeAnd = new string[0],
                ExcludeOr = new[] { "Hyper", "6-Max" },
                WindowClass = "",
                BuyInAndRake = new[] { "$1.35 + $0.15", "$3.20 + $0.30", "$6.55 + $0.45", "$14.10 + $0.90", "$28.15 + $1.85", "$56.40 + $3.60", "$94.15 + $5.85" }
            };
            collection.Add(pt);

            return collection;
        }
    }
}

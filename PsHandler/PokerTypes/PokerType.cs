using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PsHandler.PokerTypes
{
    public class PokerType
    {
        public string Name = "";
        public TimeSpan LevelLength = new TimeSpan(0);
        public Regex RegexWindowTitle = new Regex("");
        public Regex RegexWindowClass = new Regex("");

        public static IEnumerable<PokerType> GetDefaultValues()
        {
            List<PokerType> pokerTypesDefault = new List<PokerType>();

            pokerTypesDefault.Add(new PokerType
            {
                Name = "NLHE 10-max Fifty50 Regular",
                LevelLength = new TimeSpan(0, 6, 0),
                RegexWindowTitle = new Regex(@"\A(?<currency>\$|€|£)(?<buyin>.+) NL Hold'em \[Fifty50\][ ]+- Blinds (?<blinds>.+) - Tournament (?<tournament>\d+) Table (?<table>\d)"),
                RegexWindowClass = new Regex(@"\APokerStarsTableFrameClass\z"),
            });
            pokerTypesDefault.Add(new PokerType
            {
                Name = "NLHE 10-max Fifty50 Turbo",
                LevelLength = new TimeSpan(0, 3, 0),
                RegexWindowTitle = new Regex(@"\A(?<currency>\$|€|£)(?<buyin>.+) NL Hold'em \[Turbo, Fifty50\][ ]+- Blinds (?<blinds>.+) - Tournament (?<tournament>\d+) Table (?<table>\d)"),
                RegexWindowClass = new Regex(@"\APokerStarsTableFrameClass\z"),
            });
            pokerTypesDefault.Add(new PokerType
            {
                Name = "NLHE 2-max 2 Players Regular",
                LevelLength = new TimeSpan(0, 6, 0),
                RegexWindowTitle = new Regex(@"\A(?<currency>\$|€|£)(?<buyin>.+) NL Hold'em \[HU, 2 Players\][ ]+- Blinds (?<blinds>.+) - Tournament (?<tournament>\d+) Table (?<table>\d)"),
                RegexWindowClass = new Regex(@"\APokerStarsTableFrameClass\z"),
            });
            pokerTypesDefault.Add(new PokerType
            {
                Name = "NLHE 2-max 2 Players Turbo",
                LevelLength = new TimeSpan(0, 3, 0),
                RegexWindowTitle = new Regex(@"\A(?<currency>\$|€|£)(?<buyin>.+) NL Hold'em \[HU, Turbo, 2 Players\][ ]+- Blinds (?<blinds>.+) - Tournament (?<tournament>\d+) Table (?<table>\d)"),
                RegexWindowClass = new Regex(@"\APokerStarsTableFrameClass\z"),
            });
            pokerTypesDefault.Add(new PokerType
            {
                Name = "NLHE 2-max 2 Players Hyper-Turbo",
                LevelLength = new TimeSpan(0, 2, 0),
                RegexWindowTitle = new Regex(@"\A(?<currency>\$|€|£)(?<buyin>.+) NL Hold'em \[HU, Hyper-Turbo, 2 Players\][ ]+- Blinds (?<blinds>.+) - Tournament (?<tournament>\d+) Table (?<table>\d)"),
                RegexWindowClass = new Regex(@"\APokerStarsTableFrameClass\z"),
            });


            pokerTypesDefault.Add(new PokerType
            {
                Name = "NLHE 3-max Spin & Go Hyper-Turbo",
                LevelLength = new TimeSpan(0, 3, 0),
                RegexWindowTitle = new Regex(@"\A(?<currency>\$|€|£)(?<buyin>.+) Spin & Go[ ]+- Blinds (?<blinds>.+) - Tournament (?<tournament>\d+) Table (?<table>\d)"),
                RegexWindowClass = new Regex(@"\APokerStarsTableFrameClass\z"),
            });


            pokerTypesDefault.Add(new PokerType
            {
                Name = "NLHE 6-max",
                LevelLength = new TimeSpan(0, 10, 0),
                RegexWindowTitle = new Regex(@"\A(?<currency>\$|€|£)(?<buyin>.+) NL Hold'em \[6-Max\][ ]+- Blinds (?<blinds>.+) - Tournament (?<tournament>\d+) Table (?<table>\d)"),
                RegexWindowClass = new Regex(@"\APokerStarsTableFrameClass\z"),
            });
            pokerTypesDefault.Add(new PokerType
            {
                Name = "NLHE 6-max Turbo",
                LevelLength = new TimeSpan(0, 5, 0),
                RegexWindowTitle = new Regex(@"\A(?<currency>\$|€|£)(?<buyin>.+) NL Hold'em \[6-Max, Turbo\][ ]+- Blinds (?<blinds>.+) - Tournament (?<tournament>\d+) Table (?<table>\d)"),
                RegexWindowClass = new Regex(@"\APokerStarsTableFrameClass\z"),
            });
            pokerTypesDefault.Add(new PokerType
            {
                Name = "NLHE 6-max Hyper-Turbo",
                LevelLength = new TimeSpan(0, 2, 0),
                RegexWindowTitle = new Regex(@"\A(?<currency>\$|€|£)(?<buyin>.+) NL Hold'em \[6-Max, Hyper-Turbo\][ ]+- Blinds (?<blinds>.+) - Tournament (?<tournament>\d+) Table (?<table>\d)"),
                RegexWindowClass = new Regex(@"\APokerStarsTableFrameClass\z"),
            });
            pokerTypesDefault.Add(new PokerType
            {
                Name = "NLHE 9-max Regular",
                LevelLength = new TimeSpan(0, 10, 0),
                RegexWindowTitle = new Regex(@"\A(?<currency>\$|€|£)(?<buyin>.+) NL Hold'em[ ]+- Blinds (?<blinds>.+) - Tournament (?<tournament>\d+) Table (?<table>\d)"),
                RegexWindowClass = new Regex(@"\APokerStarsTableFrameClass\z"),
            }); pokerTypesDefault.Add(new PokerType
            {
                Name = "NLHE 9-max Turbo",
                LevelLength = new TimeSpan(0, 5, 0),
                RegexWindowTitle = new Regex(@"\A(?<currency>\$|€|£)(?<buyin>.+) NL Hold'em \[Turbo\][ ]+- Blinds (?<blinds>.+) - Tournament (?<tournament>\d+) Table (?<table>\d)"),
                RegexWindowClass = new Regex(@"\APokerStarsTableFrameClass\z"),
            });
            pokerTypesDefault.Add(new PokerType
            {
                Name = "NLHE 9-max Hyper-Turbo",
                LevelLength = new TimeSpan(0, 2, 0),
                RegexWindowTitle = new Regex(@"\A(?<currency>\$|€|£)(?<buyin>.+) NL Hold'em \[Hyper-Turbo\][ ]+- Blinds (?<blinds>.+) - Tournament (?<tournament>\d+) Table (?<table>\d)"),
                RegexWindowClass = new Regex(@"\APokerStarsTableFrameClass\z"),
            });
            pokerTypesDefault.Add(new PokerType
            {
                Name = "NLHE 9-max Knockout Regular",
                LevelLength = new TimeSpan(0, 10, 0),
                RegexWindowTitle = new Regex(@"\A(?<currency>\$|€|£)(?<buyin>.+) NL Hold'em \[Knockout\][ ]+- Blinds (?<blinds>.+) - Tournament (?<tournament>\d+) Table (?<table>\d)"),
                RegexWindowClass = new Regex(@"\APokerStarsTableFrameClass\z"),
            });
            pokerTypesDefault.Add(new PokerType
            {
                Name = "NLHE 9-max Knockout Turbo",
                LevelLength = new TimeSpan(0, 5, 0),
                RegexWindowTitle = new Regex(@"\A(?<currency>\$|€|£)(?<buyin>.+) NL Hold'em \[Turbo, Knockout\][ ]+- Blinds (?<blinds>.+) - Tournament (?<tournament>\d+) Table (?<table>\d)"),
                RegexWindowClass = new Regex(@"\APokerStarsTableFrameClass\z"),
            });
            pokerTypesDefault.Add(new PokerType
            {
                Name = "Sat: 6-max Hyper-Turbo",
                LevelLength = new TimeSpan(0, 3, 0),
                RegexWindowTitle = new Regex(@"\A(?<target>.+) Sat: (?<currency>\$|€|£)(?<buyin>.+) (\[Hyper-Turbo, 2 Seats\]|Hyper-Turbo \[2 Seats\])[ ]+- Blinds (?<blinds>.+) - Tournament (?<tournament>\d+) Table (?<table>\d)"),
                RegexWindowClass = new Regex(@"\APokerStarsTableFrameClass\z"),
            });

            return pokerTypesDefault;
        }

        public XElement ToXElement()
        {
            return new XElement("PokerType",
                                    new XElement("Name", Name),
                                    new XElement("LevelLength", LevelLength.Ticks),
                                    new XElement("RegexWindowTitle", RegexWindowTitle),
                                    new XElement("RegexWindowClass", RegexWindowClass)
                                    );
        }

        public static PokerType FromXElement(XElement xElement, ref List<ExceptionPsHandler> exceptions, string exceptionHeader)
        {
            try
            {
                return new PokerType
                {
                    Name = xElement.Element("Name") == null ? "" : xElement.Element("Name").Value,
                    LevelLength = new TimeSpan(long.Parse(xElement.Element("LevelLength").Value)),
                    RegexWindowTitle = new Regex(xElement.Element("RegexWindowTitle").Value),
                    RegexWindowClass = new Regex(xElement.Element("RegexWindowClass").Value),
                };
            }
            catch (Exception e)
            {
                exceptions.Add(new ExceptionPsHandler(e, exceptionHeader + " PokerType.FromXElement() xElement:" + Environment.NewLine + xElement));
                return null;
            }
        }
    }
}

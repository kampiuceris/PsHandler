using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;

namespace PsHandler.Hud
{
    public class ColorByValue
    {
        public decimal GreaterOrEqual;
        public decimal Less;
        public Color Color;

        public XElement ToXElement()
        {
            return new XElement("ColorByValue",
                             new XElement("Color", Color.ToString()),
                             new XElement("GreaterOrEqual", GreaterOrEqual),
                             new XElement("Less", Less)
                    );
        }

        public static ColorByValue FromXElement(XElement xElement)
        {
            try
            {
                return new ColorByValue
                {
                    GreaterOrEqual = decimal.Parse(xElement.Element("GreaterOrEqual").Value),
                    Less = decimal.Parse(xElement.Element("Less").Value),
                    Color = (Color)ColorConverter.ConvertFromString(xElement.Element("Color").Value)
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public override string ToString()
        {
            string greaterOrEqual = GreaterOrEqual.ToString();
            if (GreaterOrEqual == decimal.MinValue) greaterOrEqual = "-inf";
            if (GreaterOrEqual == decimal.MaxValue) greaterOrEqual = "+inf";

            string less = Less.ToString();
            if (Less == decimal.MinValue) less = "-inf";
            if (Less == decimal.MaxValue) less = "+inf";

            string color = Color.ToString();
            List<string> colorNames = typeof(System.Drawing.Color).GetProperties(BindingFlags.Public | BindingFlags.Static).Select(item => item.Name).ToList();
            foreach (string colorName in colorNames)
            {
                System.Drawing.Color colorDrawing = System.Drawing.Color.FromName(colorName);
                System.Windows.Media.Color colorMedia = Color.FromArgb(colorDrawing.A, colorDrawing.R, colorDrawing.G, colorDrawing.B);
                if (Color.Equals(colorMedia))
                {
                    color = colorName;
                    break;
                }
            }

            return string.Format("{0} <= {1} < {2}", greaterOrEqual, color, less);
        }

        public static Color GetColorByValue(Color defaultColor, decimal value, IEnumerable<ColorByValue> colorsByValue)
        {
            Color color = defaultColor;
            foreach (ColorByValue colorByValue in colorsByValue)
            {
                if (colorByValue.GreaterOrEqual <= value && value < colorByValue.Less)
                {
                    color = colorByValue.Color;
                }
            }
            return color;
        }

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

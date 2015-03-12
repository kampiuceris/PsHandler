// PsHandler - poker software helping tool.
// Copyright (C) 2014  kampiuceris

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

        public ColorByValue Clone()
        {
            return new ColorByValue
            {
                GreaterOrEqual = GreaterOrEqual,
                Less = Less,
                Color = Color,
            };
        }

        public XElement ToXElement()
        {
            return new XElement("ColorByValue",
                             new XElement("Color", Color.ToString()),
                             new XElement("GreaterOrEqual", GreaterOrEqual),
                             new XElement("Less", Less)
                    );
        }

        public static ColorByValue FromXElement(XElement xElement, ref List<ExceptionPsHandler> exceptions, string exceptionHeader)
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
            catch (Exception e)
            {
                exceptions.Add(new ExceptionPsHandler(e, exceptionHeader));
                return null;
            }
        }
    }
}

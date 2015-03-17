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
using System.Globalization;
using System.Text.RegularExpressions;

namespace PsHandler.ColorPicker
{
    public class ColorARGBHSV
    {
        //   A,R,G,B=[0..255]   H,S,V=[0..1]   A,R,G,B=0..255   H=0..360   S,V=0..100

        public static readonly Regex RegexColorHex = new Regex(@"\A(?<alpha>[0123456789ABCDEFabcdef]{2})(?<red>[0123456789ABCDEFabcdef]{2})(?<green>[0123456789ABCDEFabcdef]{2})(?<blue>[0123456789ABCDEFabcdef]{2})\z");

        private byte _a = 255;
        private byte _r = 255;
        private byte _g = 255;
        private byte _b = 255;
        private double _h = 0;
        private double _s = 0;
        private double _v = 1;
        private System.Drawing.Color _colorDrawing = System.Drawing.Color.White;
        private System.Windows.Media.Color _colorMedia = System.Windows.Media.Colors.White;
        private string _stringHex = "FFFFFFFF";

        public byte A
        {
            get { return _a; }
            set
            {
                _a = value;
                _colorDrawing = System.Drawing.Color.FromArgb(_a, _r, _g, _b);
                _colorMedia = System.Windows.Media.Color.FromArgb(_a, _r, _g, _b);
                _stringHex = ToStringHex(_a, _r, _g, _b);
            }
        }
        public byte R
        {
            get { return _r; }
            set
            {
                _r = value;
                RGBtoHSV(_r, _g, _b, out _h, out _s, out _v);
                _colorDrawing = System.Drawing.Color.FromArgb(_a, _r, _g, _b);
                _colorMedia = System.Windows.Media.Color.FromArgb(_a, _r, _g, _b);
                _stringHex = ToStringHex(_a, _r, _g, _b);
            }
        }
        public byte G
        {
            get { return _g; }
            set
            {
                _g = value;
                RGBtoHSV(_r, _g, _b, out _h, out _s, out _v);
                _colorDrawing = System.Drawing.Color.FromArgb(_a, _r, _g, _b);
                _colorMedia = System.Windows.Media.Color.FromArgb(_a, _r, _g, _b);
                _stringHex = ToStringHex(_a, _r, _g, _b);
            }
        }
        public byte B
        {
            get { return _b; }
            set
            {
                _b = value;
                RGBtoHSV(_r, _g, _b, out _h, out _s, out _v);
                _colorDrawing = System.Drawing.Color.FromArgb(_a, _r, _g, _b);
                _colorMedia = System.Windows.Media.Color.FromArgb(_a, _r, _g, _b);
                _stringHex = ToStringHex(_a, _r, _g, _b);
            }
        }
        public double H
        {
            get { return _h; }
            set
            {
                _h = value;
                HSVtoRGB(_h, _s, _v, out _r, out _g, out _b);
                _colorDrawing = System.Drawing.Color.FromArgb(_a, _r, _g, _b);
                _colorMedia = System.Windows.Media.Color.FromArgb(_a, _r, _g, _b);
                _stringHex = ToStringHex(_a, _r, _g, _b);
            }
        }
        public double S
        {
            get { return _s; }
            set
            {
                _s = value;
                HSVtoRGB(_h, _s, _v, out _r, out _g, out _b);
                _colorDrawing = System.Drawing.Color.FromArgb(_a, _r, _g, _b);
                _colorMedia = System.Windows.Media.Color.FromArgb(_a, _r, _g, _b);
                _stringHex = ToStringHex(_a, _r, _g, _b);
            }
        }
        public double V
        {
            get { return _v; }
            set
            {
                _v = value;
                HSVtoRGB(_h, _s, _v, out _r, out _g, out _b);
                _colorDrawing = System.Drawing.Color.FromArgb(_a, _r, _g, _b);
                _colorMedia = System.Windows.Media.Color.FromArgb(_a, _r, _g, _b);
                _stringHex = ToStringHex(_a, _r, _g, _b);
            }
        }
        public System.Drawing.Color ColorDrawing
        {
            get { return _colorDrawing; }
            set
            {
                _colorDrawing = value;
                _a = _colorDrawing.A;
                _r = _colorDrawing.R;
                _g = _colorDrawing.G;
                _b = _colorDrawing.B;
                RGBtoHSV(_r, _g, _b, out _h, out _s, out _v);
                _colorMedia = System.Windows.Media.Color.FromArgb(_a, _r, _g, _b);
                _stringHex = ToStringHex(_a, _r, _g, _b);
            }
        }
        public System.Windows.Media.Color ColorMedia
        {
            get { return _colorMedia; }
            set
            {
                _colorMedia = value;
                _a = _colorMedia.A;
                _r = _colorMedia.R;
                _g = _colorMedia.G;
                _b = _colorMedia.B;
                RGBtoHSV(_r, _g, _b, out _h, out _s, out _v);
                _colorDrawing = System.Drawing.Color.FromArgb(_a, _r, _g, _b);
                _stringHex = ToStringHex(_a, _r, _g, _b);
            }
        }
        public string StringHex
        {
            get { return _stringHex; }
            set
            {
                var fromStringHex = FromStringHex(value);
                _a = fromStringHex.A;
                _r = fromStringHex.R;
                _g = fromStringHex.G;
                _b = fromStringHex.B;
                RGBtoHSV(_r, _g, _b, out _h, out _s, out _v);
                _colorDrawing = System.Drawing.Color.FromArgb(_a, _r, _g, _b);
                _colorMedia = System.Windows.Media.Color.FromArgb(_a, _r, _g, _b);
            }
        }

        public ColorARGBHSV(byte r, byte g, byte b, byte a = 255)
        {
            _a = a;
            _r = r;
            _g = g;
            _b = b;
            RGBtoHSV(_r, _g, _b, out _h, out _s, out _v);
            _colorDrawing = System.Drawing.Color.FromArgb(_a, _r, _g, _b);
            _colorMedia = System.Windows.Media.Color.FromArgb(_a, _r, _g, _b);
            _stringHex = ToStringHex(_a, _r, _g, _b);
        }

        public ColorARGBHSV(double h, double s, double v, byte a = 255)
        {
            _a = a;
            _h = h;
            _s = s;
            _v = v;
            HSVtoRGB(_h, _s, _v, out _r, out _g, out _b);
            _colorDrawing = System.Drawing.Color.FromArgb(_a, _r, _g, _b);
            _colorMedia = System.Windows.Media.Color.FromArgb(_a, _r, _g, _b);
            _stringHex = ToStringHex(_a, _r, _g, _b);
        }

        public void SetRGB(byte r, byte g, byte b)
        {
            _r = r;
            _g = g;
            _b = b;
            RGBtoHSV(_r, _g, _b, out _h, out _s, out _v);
            _colorDrawing = System.Drawing.Color.FromArgb(_a, _r, _g, _b);
            _colorMedia = System.Windows.Media.Color.FromArgb(_a, _r, _g, _b);
        }

        public void SetARGB(byte a, byte r, byte g, byte b)
        {
            _a = a;
            _r = r;
            _g = g;
            _b = b;
            RGBtoHSV(_r, _g, _b, out _h, out _s, out _v);
            _colorDrawing = System.Drawing.Color.FromArgb(_a, _r, _g, _b);
            _colorMedia = System.Windows.Media.Color.FromArgb(_a, _r, _g, _b);
        }

        public void SetHSV(double h, double s, double v)
        {
            _h = h;
            _s = s;
            _v = v;
            HSVtoRGB(_h, _s, _v, out _r, out _g, out _b);
            _colorDrawing = System.Drawing.Color.FromArgb(_a, _r, _g, _b);
            _colorMedia = System.Windows.Media.Color.FromArgb(_a, _r, _g, _b);
            _stringHex = ToStringHex(_a, _r, _g, _b);
        }

        //

        public static void RGBtoHSV(byte r, byte g, byte b, out double h, out double s, out double v)
        {
            var max = Math.Max(r, Math.Max(g, b));
            var min = Math.Min(r, Math.Min(g, b));

            h = GetHue(r, g, b);
            s = (max == 0) ? 0 : 1d - (1d * min / max);
            v = max / 255d;
        }

        public static void HSVtoRGB(double h, double s, double v, out byte r, out byte g, out byte b)
        {
            h *= 360d;

            if (h < 0) h = 0;
            if (s < 0) s = 0;
            if (v < 0) v = 0;
            if (h >= 360) h = 359;
            if (s > 1) s = 1;
            if (v > 1) v = 1;

            var C = v * s;
            var hh = h / 60;
            var X = C * (1 - Math.Abs(hh % 2 - 1));
            double rf = 0, gf = 0, bf = 0;

            if (hh >= 0 && hh < 1)
            {
                rf = C;
                gf = X;
            }
            else if (hh >= 1 && hh < 2)
            {
                rf = X;
                gf = C;
            }
            else if (hh >= 2 && hh < 3)
            {
                gf = C;
                bf = X;
            }
            else if (hh >= 3 && hh < 4)
            {
                gf = X;
                bf = C;
            }
            else if (hh >= 4 && hh < 5)
            {
                rf = X;
                bf = C;
            }
            else
            {
                rf = C;
                bf = X;
            }

            var m = v - C;
            rf += m;
            gf += m;
            bf += m;

            r = Convert.ToByte(Math.Floor(rf * 255));
            g = Convert.ToByte(Math.Floor(gf * 255));
            b = Convert.ToByte(Math.Floor(bf * 255));
        }

        private static double GetHue(byte red, byte green, byte blue)
        {
            if (red == green && green == blue) return 0; // 0 makes as good an UNDEFINED value as any

            double r = red / 255.0;
            double g = green / 255.0;
            double b = blue / 255.0;

            double max, min;
            double delta;
            double hue = 0.0f;

            max = r; min = r;

            if (g > max) max = g;
            if (b > max) max = b;

            if (g < min) min = g;
            if (b < min) min = b;

            delta = max - min;

            if (r == max)
            {
                hue = (g - b) / delta;
            }
            else if (g == max)
            {
                hue = 2 + (b - r) / delta;
            }
            else if (b == max)
            {
                hue = 4 + (r - g) / delta;
            }
            hue *= 60;

            if (hue < 0.0f)
            {
                hue += 360.0f;
            }
            return hue / 360;
        }

        //

        public static string ToStringHex(System.Windows.Media.Color colorMedia)
        {
            return string.Format("{0}{1}{2}{3}", colorMedia.A.ToString("X2"), colorMedia.R.ToString("X2"), colorMedia.G.ToString("X2"), colorMedia.B.ToString("X2"));
        }

        public static string ToStringHex(byte a, byte r, byte g, byte b)
        {
            return string.Format("{0}{1}{2}{3}", a.ToString("X2"), r.ToString("X2"), g.ToString("X2"), b.ToString("X2"));
        }

        public static ColorARGBHSV FromStringHex(string stringHex)
        {
            try
            {
                var match = RegexColorHex.Match(stringHex.ToUpper());
                if (match.Success)
                {
                    return new ColorARGBHSV(
                        byte.Parse(match.Groups["red"].Value, NumberStyles.HexNumber),
                        byte.Parse(match.Groups["green"].Value, NumberStyles.HexNumber),
                        byte.Parse(match.Groups["blue"].Value, NumberStyles.HexNumber),
                        byte.Parse(match.Groups["alpha"].Value, NumberStyles.HexNumber)
                        );
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            catch
            {
                return null;
            }
        }

        public override string ToString()
        {
            return string.Format("[A={0}] [R={1} G={2} B={3}] [H={4:0.00}({7:0.00}°) S={5:0.00} V={6:0.00}]", A, R, G, B, H, S, V, H * 360d);
        }
    }
}

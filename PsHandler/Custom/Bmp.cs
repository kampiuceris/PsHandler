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
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace PsHandler.Custom
{
    public class Bmp
    {
        private byte[] _bgra;
        public int Width { private set; get; }
        public int Height { private set; get; }

        public Bmp(int width, int height)
        {
            Width = width;
            Height = height;
            _bgra = new byte[width * height * 4];
        }

        public Bmp(int width, int height, Color initColor)
            : this(width, height)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    SetPixelA(x, y, initColor.A);
                    SetPixelR(x, y, initColor.R);
                    SetPixelG(x, y, initColor.G);
                    SetPixelB(x, y, initColor.B);
                }
            }
        }

        public Bmp(int width, int height, System.Windows.Media.Color initColor)
            : this(width, height, Color.FromArgb(initColor.A, initColor.R, initColor.G, initColor.B))
        {
        }

        public Bmp(Bitmap bmp)
        {
            using (bmp)
            {
                Width = bmp.Width;
                Height = bmp.Height;
                _bgra = BitmapToBytes_Unsafe(bmp);
            }
        }

        public Bmp(byte[] bgra, int width, int height)
        {
            if (width * height * 4 != bgra.Length)
                throw new ArgumentException("(byte[] bgra) length not equal to (width * length * 4)");
            _bgra = bgra;
            Width = width;
            Height = height;
        }

        public byte[] GetBytesBGRA()
        {
            return _bgra;
        }

        /// <summary>
        /// Slower way to set/get pixels. Faster GetPixels A/R/G/B(int x, int y).
        /// </summary>
        /// <returns>byte[] { A, R, G, B }</returns>
        public byte[] this[int x, int y]
        {
            set
            {
                int i = x * 4 + (Width * 4) * y;
                _bgra[i + 3] = value[0];
                _bgra[i + 2] = value[1];
                _bgra[i + 1] = value[2];
                _bgra[i] = value[3];
            }
            get
            {
                int i = x * 4 + (Width * 4) * y;
                return new[] { _bgra[i + 3], _bgra[i + 2], _bgra[i + 1], _bgra[i] };
            }
        }

        public byte GetPixelA(int x, int y)
        {
            return _bgra[x * 4 + (Width * 4) * y + 3];
        }

        public byte GetPixelR(int x, int y)
        {
            return _bgra[x * 4 + (Width * 4) * y + 2];
        }

        public byte GetPixelG(int x, int y)
        {
            return _bgra[x * 4 + (Width * 4) * y + 1];
        }

        public byte GetPixelB(int x, int y)
        {
            return _bgra[x * 4 + (Width * 4) * y];
        }

        public void SetPixelA(int x, int y, byte value)
        {
            _bgra[x * 4 + (Width * 4) * y + 3] = value;
        }

        public void SetPixelR(int x, int y, byte value)
        {
            _bgra[x * 4 + (Width * 4) * y + 2] = value;
        }

        public void SetPixelG(int x, int y, byte value)
        {
            _bgra[x * 4 + (Width * 4) * y + 1] = value;
        }

        public void SetPixelB(int x, int y, byte value)
        {
            _bgra[x * 4 + (Width * 4) * y] = value;
        }

        // static

        public static Bitmap CutBitmap(Bitmap bitmap, Rectangle rect)
        {
            Bitmap bitmapCut = new Bitmap(rect.Right - rect.Left, rect.Bottom - rect.Top);
            using (Graphics g = Graphics.FromImage(bitmapCut))
            {
                g.DrawImage(bitmap, new Rectangle(0, 0, bitmapCut.Width, bitmapCut.Height), rect, GraphicsUnit.Pixel);
            }
            return bitmapCut;
        }

        public static Bmp ___CutBmp(Bmp bmp, Rectangle rect)
        {
            Bmp bmpCut;
            using (Bitmap bitmap = BmpToBitmap(bmp))
            {
                using (Bitmap bitmapCut = CutBitmap(bitmap, rect))
                {
                    bmpCut = new Bmp(bitmapCut);
                }
            }
            return bmpCut;
        }

        public Bmp ___CutRectangle(Rectangle rect)
        {
            Bmp bmpCut = new Bmp(rect.Width, rect.Height);

            for (int y = rect.Y; y < rect.Height; y++)
            {
                for (int x = rect.X; x < rect.Width; x++)
                {
                    bmpCut.SetPixelA(x - rect.X, y - rect.Y, GetPixelA(x, y));
                }
            }

            return bmpCut;
        }

        public Bmp CutRectangle(Rectangle rect)
        {
            byte[] bgra = new byte[rect.Width * rect.Height * 4];
            for (int yOffset = 0; yOffset < rect.Height; yOffset++)
            {
                Array.Copy(_bgra, rect.X * 4 + (Width * 4) * (rect.Y + yOffset), bgra, (rect.Width * 4) * yOffset, rect.Width * 4);
            }
            return new Bmp(bgra, rect.Width, rect.Height);
        }

        public static BitmapSource BitmapToBitmapSource(Bitmap bitmap)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero,
                System.Windows.Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(bitmap.Width, bitmap.Height));
        }

        public static byte[] BitmapToBytes_Unsafe(Bitmap bitmap)
        {
            BitmapData bData = bitmap.LockBits(new Rectangle(new Point(), bitmap.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int byteCount = bData.Stride * bitmap.Height; // number of bytes in the bitmap
            byte[] bmpBytes = new byte[byteCount];
            Marshal.Copy(bData.Scan0, bmpBytes, 0, byteCount); // Copy the locked bytes from memory
            bitmap.UnlockBits(bData); // don't forget to unlock the bitmap!!
            return bmpBytes;
        }

        public static Bitmap BytesToBitmap_Unsafe(byte[] argb, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
            Marshal.Copy(argb, 0, bmpData.Scan0, argb.Length);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        public static Bitmap BmpToBitmap(Bmp bmp)
        {
            return BytesToBitmap_Unsafe(bmp.GetBytesBGRA(), bmp.Width, bmp.Height);
        }

        public Bitmap ToBitmap()
        {
            return BytesToBitmap_Unsafe(GetBytesBGRA(), Width, Height);
        }

        public static Bmp BitmapToBmp(Bitmap bitmap)
        {
            return new Bmp(bitmap);
        }

        public static Bmp[] BitmapArrayToBmpArray(Bitmap[] bitmaps)
        {
            if (bitmaps == null) return null;
            Bmp[] bmps = new Bmp[bitmaps.Length];
            for (int i = 0; i < bitmaps.Length; i++)
            {
                bmps[i] = new Bmp(bitmaps[i]);
            }
            return bmps;
        }

        public Bmp ToHighContrast()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (GetPixelA(x, y) < byte.MaxValue / 2)
                    {
                        SetPixelA(x, y, 0);
                        SetPixelR(x, y, 0);
                        SetPixelG(x, y, 0);
                        SetPixelB(x, y, 0);
                    }
                    else
                    {
                        double h, s, l;
                        RgbToHsl(GetPixelR(x, y), GetPixelG(x, y), GetPixelB(x, y), out h, out s, out l);
                        if (l < 0.5)
                        {
                            SetPixelA(x, y, 255);
                            SetPixelR(x, y, 0);
                            SetPixelG(x, y, 0);
                            SetPixelB(x, y, 0);
                        }
                        else
                        {
                            SetPixelA(x, y, 255);
                            SetPixelR(x, y, 255);
                            SetPixelG(x, y, 255);
                            SetPixelB(x, y, 255);
                        }
                    }
                }
            }

            return this;
        }

        public Bmp ToHighContrastClone()
        {
            double h, s, l;
            Bmp bmpHighContrast = new Bmp(Width, Height);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (GetPixelA(x, y) < byte.MaxValue / 2)
                    {
                        bmpHighContrast.SetPixelA(x, y, 0);
                        bmpHighContrast.SetPixelR(x, y, 0);
                        bmpHighContrast.SetPixelG(x, y, 0);
                        bmpHighContrast.SetPixelB(x, y, 0);
                    }
                    else
                    {
                        RgbToHsl(GetPixelR(x, y), GetPixelG(x, y), GetPixelB(x, y), out h, out s, out l);
                        if (l < 0.5)
                        {
                            bmpHighContrast.SetPixelA(x, y, 255);
                            bmpHighContrast.SetPixelR(x, y, 0);
                            bmpHighContrast.SetPixelG(x, y, 0);
                            bmpHighContrast.SetPixelB(x, y, 0);
                        }
                        else
                        {
                            bmpHighContrast.SetPixelA(x, y, 255);
                            bmpHighContrast.SetPixelR(x, y, 255);
                            bmpHighContrast.SetPixelG(x, y, 255);
                            bmpHighContrast.SetPixelB(x, y, 255);
                        }
                    }
                }
            }

            return bmpHighContrast;
        }

        public static Color HslToRgb(double h, double s, double l)
        {
            double r, g, b;
            HslToRgb(h, s, l, out r, out g, out b);
            return Color.FromArgb(255, Convert.ToByte(r * 255.0f), Convert.ToByte(g * 255.0f), Convert.ToByte(b * 255.0f));
        }

        public static void RgbToHsl(Color rgb, out double h, out double s, out double l)
        {
            RgbToHsl(rgb.R, rgb.G, rgb.B, out h, out s, out l);
        }

        public static void HslToRgb(double h, double s, double l, out double r, out double g, out double b)
        {
            double v;

            r = l;   // default to gray
            g = l;
            b = l;
            v = (l <= 0.5) ? (l * (1.0 + s)) : (l + s - l * s);
            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0;
                sextant = (int)h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }

            r = Convert.ToByte(r * 255.0f);
            g = Convert.ToByte(g * 255.0f);
            b = Convert.ToByte(b * 255.0f);
        }

        public static void RgbToHsl(double r, double g, double b, out double h, out double s, out double l)
        {
            r /= 255.0;
            g /= 255.0;
            b /= 255.0;
            double v;
            double m;
            double vm;
            double r2, g2, b2;

            h = 0; // default to black
            s = 0;
            l = 0;
            v = Math.Max(r, g);
            v = Math.Max(v, b);
            m = Math.Min(r, g);
            m = Math.Min(m, b);
            l = (m + v) / 2.0;
            if (l <= 0.0)
            {
                return;
            }
            vm = v - m;
            s = vm;
            if (s > 0.0)
            {
                s /= (l <= 0.5) ? (v + m) : (2.0 - v - m);
            }
            else
            {
                return;
            }
            r2 = (v - r) / vm;
            g2 = (v - g) / vm;
            b2 = (v - b) / vm;
            if (r == v)
            {
                h = (g == m ? 5.0 + b2 : 1.0 - g2);
            }
            else if (g == v)
            {
                h = (b == m ? 1.0 + r2 : 3.0 - b2);
            }
            else
            {
                h = (r == m ? 3.0 + g2 : 5.0 - r2);
            }
            h /= 6.0;
        }
    }
}

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace PsHandler.Custom
{
    public class Bmp
    {
        private readonly byte[] _bgra;
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

        public byte[] GetBytesARGB()
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

        public static Bitmap CutBitmap(Bitmap source, Rectangle rect)
        {
            //System.Diagnostics.Debug.WriteLine(rect.LocationX + " " + rect.LocationY + " " + rect.Width + " " + rect.Height);
            Bitmap bitmapCut = new Bitmap(rect.Right - rect.Left, rect.Bottom - rect.Top);
            using (Graphics g = Graphics.FromImage(bitmapCut))
            {
                g.DrawImage(source, new Rectangle(0, 0, bitmapCut.Width, bitmapCut.Height), rect, GraphicsUnit.Pixel);
            }
            return bitmapCut;
        }

        public static Bmp CutBmp(Bmp bmp, Rectangle rect)
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
            return BytesToBitmap_Unsafe(bmp.GetBytesARGB(), bmp.Width, bmp.Height);
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
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media.Imaging;

namespace PsHandler
{
    public class Bmp
    {
        private readonly byte[] _rbga;
        public int Width { private set; get; }
        public int Height { private set; get; }

        public Bmp(int width, int height)
        {
            Width = width;
            Height = height;
            _rbga = new byte[width * height * 4];
        }

        public Bmp(Bitmap bmp)
        {
            using (bmp)
            {
                Width = bmp.Width;
                Height = bmp.Height;
                _rbga = BitmapToBytes_Unsafe(bmp);
            }
        }

        public byte[] GetBytes()
        {
            return _rbga;
        }

        /// <summary>
        /// Slower way to set/get pixels. Faster GetPixels R/G/B/A(int x, int y).
        /// </summary>
        /// <returns>byte[] { R, G, B, A }</returns>
        public byte[] this[int x, int y]
        {
            set
            {
                int i = x * 4 + (Width * 4) * y;
                _rbga[i] = value[0];
                _rbga[i + 1] = value[1];
                _rbga[i + 2] = value[2];
                _rbga[i + 3] = value[3];
            }
            get
            {
                int i = x * 4 + (Width * 4) * y;
                return new byte[] { _rbga[i], _rbga[i + 1], _rbga[i + 2], _rbga[i + 3] };
            }
        }

        public byte GetPixelR(int x, int y)
        {
            return _rbga[x * 4 + (Width * 4) * y];
        }

        public byte GetPixelG(int x, int y)
        {
            return _rbga[x * 4 + (Width * 4) * y + 1];
        }

        public byte GetPixelB(int x, int y)
        {
            return _rbga[x * 4 + (Width * 4) * y + 2];
        }

        public byte GetPixelA(int x, int y)
        {
            return _rbga[x * 4 + (Width * 4) * y + 3];
        }

        public void SetPixelR(int x, int y, byte value)
        {
            _rbga[x * 4 + (Width * 4) * y] = value;
        }

        public void SetPixelG(int x, int y, byte value)
        {
            _rbga[x * 4 + (Width * 4) * y + 1] = value;
        }

        public void SetPixelB(int x, int y, byte value)
        {
            _rbga[x * 4 + (Width * 4) * y + 2] = value;
        }

        public void SetPixelA(int x, int y, byte value)
        {
            _rbga[x * 4 + (Width * 4) * y + 3] = value;
        }

        // static

        public static Bitmap CutBitmap(Bitmap source, Rectangle rect)
        {
            System.Diagnostics.Debug.WriteLine(rect.X + " " + rect.Y + " " + rect.Width + " " + rect.Height);
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

        public static Bitmap BytesToBitmap_Unsafe(byte[] rbga, int width, int height)
        {
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
            Marshal.Copy(rbga, 0, bmpData.Scan0, rbga.Length);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        public static Bitmap BmpToBitmap(Bmp bmp)
        {
            return BytesToBitmap_Unsafe(bmp.GetBytes(), bmp.Width, bmp.Height);
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

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
using System.Drawing;
using System.Linq;
using System.Text;

namespace PsHandler.Custom
{
    public class ScreenCapture
    {
        public static Bitmap[] GetBitmapScreensAll()
        {
            System.Windows.Forms.Screen[] screens = System.Windows.Forms.Screen.AllScreens;
            //System.Diagnostics.Debug.WriteLine(screens.Length);
            Bitmap[] bitmapScreens = new Bitmap[screens.Length];
            for (int i = 0; i < screens.Length; i++)
            {
                //System.Diagnostics.Debug.WriteLine(screens[i].Bounds);
                Bitmap bitmap = new Bitmap(screens[i].Bounds.Width, screens[i].Bounds.Height);
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.CopyFromScreen(new Point(screens[i].Bounds.X, screens[i].Bounds.Y), new Point(0, 0), new Size(screens[i].Bounds.Width, screens[i].Bounds.Height));
                    }
                }
                bitmapScreens[i] = bitmap;
            }
            return bitmapScreens;
        }

        public static Bitmap GetBitmapWindow(IntPtr handle)
        {
            try
            {
                // get te hDC of the target window
                IntPtr hdcSrc = WinApi.GetWindowDC(handle);
                // get the size
                WinApi.RECT windowRect;
                WinApi.GetWindowRect(handle, out windowRect);
                int width = windowRect.Right - windowRect.Left;
                int height = windowRect.Bottom - windowRect.Top;
                // create a device context we can copy to
                IntPtr hdcDest = WinApi.CreateCompatibleDC(hdcSrc);
                // create a bitmap we can copy it to,
                // using GetDeviceCaps to get the width/height
                IntPtr hBitmap = WinApi.CreateCompatibleBitmap(hdcSrc, width, height);
                // select the bitmap object
                IntPtr hOld = WinApi.SelectObject(hdcDest, hBitmap);
                // bitblt over
                WinApi.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, WinApi.SRCCOPY);
                // restore selection
                WinApi.SelectObject(hdcDest, hOld);
                // clean up 
                WinApi.DeleteDC(hdcDest);
                WinApi.ReleaseDC(handle, hdcSrc);

                // get a .NET image object for it
                //Image img = Image.FromHbitmap(hBitmap);
                Bitmap bmp = System.Drawing.Image.FromHbitmap(hBitmap);

                // free up the Bitmap object
                WinApi.DeleteObject(hBitmap);

                return bmp;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Bitmap GetBitmapWindowClient(IntPtr handle)
        {
            try
            {
                // get te hDC of the target window
                IntPtr hdcSrc = WinApi.GetWindowDC(handle);
                // get the size
                //RECT windowRect;
                //WinApi.GetWindowRect(handle, out windowRect);
                //int width = windowRect.Right - windowRect.Left;
                //int height = windowRect.Bottom - windowRect.Top;

                Rectangle rw = WinApi.GetWindowRectangle(handle);
                Rectangle rc = WinApi.GetClientRectangle(handle);
                int width = rc.Right - rc.Left;
                int height = rc.Bottom - rc.Top;
                // create a device context we can copy to
                IntPtr hdcDest = WinApi.CreateCompatibleDC(hdcSrc);
                // create a bitmap we can copy it to,
                // using GetDeviceCaps to get the width/height
                IntPtr hBitmap = WinApi.CreateCompatibleBitmap(hdcSrc, width, height);
                // select the bitmap object
                IntPtr hOld = WinApi.SelectObject(hdcDest, hBitmap);
                // bitblt over
                WinApi.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, rc.X - rw.X, rc.Y - rw.Y, WinApi.SRCCOPY);
                // restore selection
                WinApi.SelectObject(hdcDest, hOld);
                // clean up 
                WinApi.DeleteDC(hdcDest);
                WinApi.ReleaseDC(handle, hdcSrc);
                Bitmap bitmap = System.Drawing.Image.FromHbitmap(hBitmap);
                // free up the Bitmap object
                WinApi.DeleteObject(hBitmap);

                return bitmap;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Bitmap GetBitmapRectangleFromScreen(Rectangle rect)
        {
            Bitmap bitmap = new Bitmap(rect.Width, rect.Height);
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(new Point(rect.X, rect.Y), new Point(0, 0), new Size(rect.Width, rect.Height));
                }
            }
            return bitmap;
        }
    }
}

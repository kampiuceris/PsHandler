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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PsHandler.UI;

namespace PsHandler.Custom
{
    public class Methods
    {
        private static readonly Random _getRandom = new Random();
        private static readonly object _syncLock = new object();
        public static int GetRandomNumber(int min, int max) { lock (_syncLock) { return _getRandom.Next(min, max); } }

        public static void UiInvoke(Action action)
        {
            Application.Current.Dispatcher.Invoke(action.Invoke);
        }

        public static Bitmap GetEmbeddedResourceBitmap(string path)
        {
            return new Bitmap(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream(path));
        }

        public static void CheckButtonAndClick(Bmp bmp, PokerStarsButton button, IntPtr handle)
        {
            Rectangle rect = new Rectangle(
                (int)Math.Round(button.LocationX * bmp.Width),
                (int)Math.Round(button.LocationY * bmp.Height),
                (int)Math.Round(button.Width * bmp.Width),
                (int)Math.Round(button.Height * bmp.Height));
            double r, g, b;
            AverageColor(bmp, rect, out r, out g, out b);
            //Debug.WriteLine(string.Format("{0:0.000} {1:0.000} {2:0.000}", r - button.AvgR, g - button.AvgG, b - button.AvgB)); return;
            if (CompareColors(r, g, b, button.AvgR, button.AvgG, button.AvgB, button.MaxDiffR, button.MaxDiffG, button.MaxDiffB))
            {
                if (button.ButtonSecondaryCheck != null)// secondary check for some buggy themes
                {
                    CheckButtonAndClick(bmp, button.ButtonSecondaryCheck, handle);
                }
                else
                {
                    //Debug.Write("CLICK ");
                    LeftMouseClickRelativeScaled(handle, button.ClickX, button.ClickY, true);
                }
            }
        }

        public static bool IsMinimized(IntPtr handle)
        {
            return (WinApi.GetWindowLong(handle, WinApi.GWL_STYLE) & WinApi.WS_MINIMIZE) != 0;
        }

        public static void LeftMouseClickRelativeScaled(IntPtr handle, double x, double y, bool checkIfMinimized = false)
        {
            if (checkIfMinimized && IsMinimized(handle)) WinApi.ShowWindow(handle, WinApi.SW_RESTORE); // check if window is minimized & restore it
            var rectangle = WinApi.GetClientRectangle(handle);
            LeftMouseClick(handle, (int)Math.Round(rectangle.Width * x, 0), (int)Math.Round(rectangle.Height * y, 0));
        }

        public static void LeftMouseClickMiddle(IntPtr handle)
        {
            Rectangle clientRectangle = WinApi.GetClientRectangle(handle);
            IntPtr lParam = WinApi.GetLParam(clientRectangle.Width / 2, clientRectangle.Height / 2);
            WinApi.PostMessage(handle, WinApi.WM_LBUTTONDOWN, new IntPtr(WinApi.MK_LBUTTON), lParam);
            WinApi.PostMessage(handle, WinApi.WM_LBUTTONUP, IntPtr.Zero, lParam);
        }

        public static void LeftMouseClick(IntPtr handle, int x, int y)
        {
            IntPtr lParam = WinApi.GetLParam(x, y);
            WinApi.PostMessage(handle, WinApi.WM_LBUTTONDOWN, new IntPtr(WinApi.MK_LBUTTON), lParam);
            WinApi.PostMessage(handle, WinApi.WM_LBUTTONUP, IntPtr.Zero, lParam);
        }

        public static void LeftMouseDoubleClick(IntPtr handle, int x, int y)
        {
            IntPtr lParam = WinApi.GetLParam(x, y);
            WinApi.PostMessage(handle, WinApi.WM_LBUTTONDBLCLK, IntPtr.Zero, lParam);
        }

        public static void MouseEnterLeftMouseClickMouseLeave(IntPtr handle, int x, int y)
        {
            IntPtr lParam = WinApi.GetLParam(x, y);
            WinApi.PostMessage(handle, WinApi.WM_MOUSEHOVER, IntPtr.Zero, IntPtr.Zero);
            WinApi.PostMessage(handle, WinApi.WM_LBUTTONDOWN, new IntPtr(WinApi.MK_LBUTTON), lParam);
            WinApi.PostMessage(handle, WinApi.WM_LBUTTONUP, IntPtr.Zero, lParam);
            WinApi.PostMessage(handle, WinApi.WM_MOUSELEAVE, IntPtr.Zero, IntPtr.Zero);
        }

        public static void MouseEnterLeftMouseClickMiddleMouseLeave(IntPtr handle)
        {
            Rectangle clientRectangle = WinApi.GetClientRectangle(handle);
            IntPtr lParam = WinApi.GetLParam(clientRectangle.Width / 2, clientRectangle.Height / 2);
            WinApi.PostMessage(handle, WinApi.WM_MOUSEHOVER, IntPtr.Zero, IntPtr.Zero);
            WinApi.PostMessage(handle, WinApi.WM_LBUTTONDOWN, new IntPtr(WinApi.MK_LBUTTON), lParam);
            WinApi.PostMessage(handle, WinApi.WM_LBUTTONUP, IntPtr.Zero, lParam);
            WinApi.PostMessage(handle, WinApi.WM_MOUSELEAVE, IntPtr.Zero, IntPtr.Zero);
        }

        public static void AverageColor(Bmp bmp, Rectangle r, out double avgRed, out double avgGreen, out double avgBlue)
        {
            long redSum = 0, greenSum = 0, blueSum = 0;
            for (int y = r.Y; y < r.Y + r.Height; y++)
            {
                for (int x = r.X; x < r.X + r.Width; x++)
                {
                    redSum += bmp.GetPixelR(x, y);
                    greenSum += bmp.GetPixelG(x, y);
                    blueSum += bmp.GetPixelB(x, y);
                }
            }
            avgRed = (double)redSum / (r.Width * r.Height);
            avgGreen = (double)greenSum / (r.Width * r.Height);
            avgBlue = (double)blueSum / (r.Width * r.Height);
        }

        public static bool CompareColors(double r0, double g0, double b0, double r1, double g1, double b1, double maxDifferenceR, double maxDifferenceG, double maxDifferenceB)
        {
            return Math.Abs(r0 - r1) < maxDifferenceR && Math.Abs(g0 - g1) < maxDifferenceG && Math.Abs(b0 - b1) < maxDifferenceB;
        }

        public static string ReadSeek(string path, long seek)
        {
            string text = "";
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                fs.Seek(seek, SeekOrigin.Begin);
                byte[] b = new byte[fs.Length - seek];
                fs.Read(b, 0, (int)(fs.Length - seek));
                text = System.Text.Encoding.UTF8.GetString(b);
            }
            return text;
        }

        public static bool CheckIfPointIsInArea(System.Drawing.Point point, Rectangle area)
        {
            return (point.X > area.X && point.X < area.X + area.Width) && (point.Y > area.Y && point.Y < area.Y + area.Height);
        }

        public static int[] GetWindowsBorderThicknessLeftTopRightBottom()
        {
            Rectangle clientRectangle = WinApi.GetClientRectangle(App.WindowMain.GetHandle());
            Rectangle windowRectangle = WinApi.GetWindowRectangle(App.WindowMain.GetHandle());
            return new int[]
            {
                clientRectangle.Left - windowRectangle.Left,
                clientRectangle.Top - windowRectangle.Top,
                windowRectangle.Right - clientRectangle.Right,
                windowRectangle.Bottom - clientRectangle.Bottom
            };
        }

        public static void DisplayException(Exception e, Window owner, WindowStartupLocation windowStartupLocation)
        {
            UiInvoke(() =>
            {
                string fileName = "pshandler_error_" + DateTime.Now.Ticks + ".log";
                File.WriteAllText(fileName, e.Message + Environment.NewLine + Environment.NewLine + e.StackTrace);
                WindowMessage.ShowDialog(e.Message + Environment.NewLine + Environment.NewLine + "Log file: " + fileName, "Error", WindowMessageButtons.OK, WindowMessageImage.Error, owner, windowStartupLocation);
            });
        }

        public static void DisplayBitmap(Bitmap bitmap, bool dialog = false)
        {
            //UiInvoke(() =>
            //{
            System.Windows.Controls.Image image = new System.Windows.Controls.Image
            {
                Source = bitmap.ToBitmapSource(),
                Width = bitmap.Width,
                Height = bitmap.Height
            };
            Window window = new Window
            {
                SizeToContent = SizeToContent.WidthAndHeight,
                UseLayoutRounding = true,
                Content = image,
                ResizeMode = ResizeMode.NoResize,
                Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 255, 0))
            };
            if (dialog)
            {
                window.ShowDialog();
            }
            else
            {
                window.Show();
            }
            //});
        }

        public static string GetClipboardText()
        {
            var text = Clipboard.GetText();
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            return text;
        }

        public static void SetClipboardText(string text)
        {
            Clipboard.SetText(text);
        }
    }

    public static class ExtensionMethods
    {
        public static void SetProperty(this object o, string propertyName, object property, bool setForChildren = false)
        {
            if (o == null) return;

            PropertyInfo pi = o.GetType().GetProperty(propertyName);
            if (pi != null)
            {
                pi.SetValue(o, property, null);

                if (setForChildren)
                {
                    if (o is GroupBox)
                    {
                        (o as GroupBox).Content.SetProperty(propertyName, property, true);
                    }
                    if (o is Grid)
                    {
                        foreach (var child in (o as Grid).Children)
                        {
                            child.SetProperty(propertyName, property, true);
                        }
                    }
                    if (o is StackPanel)
                    {
                        foreach (var child in (o as StackPanel).Children)
                        {
                            child.SetProperty(propertyName, property, true);
                        }
                    }
                    if (o is UserControl)
                    {
                        (o as UserControl).Content.SetProperty(propertyName, property, true);
                    }
                }
            }
        }

        /// <summary>
        /// Converts a <see cref="System.Drawing.Image"/> into a WPF <see cref="BitmapSource"/>.
        /// </summary>
        /// <param name="source">The source image.</param>
        /// <returns>A BitmapSource</returns>
        public static BitmapSource ToBitmapSource(this System.Drawing.Image source)
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(source);

            var bitSrc = bitmap.ToBitmapSource();

            bitmap.Dispose();
            bitmap = null;

            return bitSrc;
        }

        /// <summary>
        /// Converts a <see cref="System.Drawing.Bitmap"/> into a WPF <see cref="BitmapSource"/>.
        /// </summary>
        /// <remarks>Uses GDI to do the conversion. Hence the call to the marshalled DeleteObject.
        /// </remarks>
        /// <param name="source">The source bitmap.</param>
        /// <returns>A BitmapSource</returns>
        public static BitmapSource ToBitmapSource(this System.Drawing.Bitmap source)
        {
            BitmapSource bitSrc = null;

            var hBitmap = source.GetHbitmap();

            try
            {
                bitSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Win32Exception)
            {
                bitSrc = null;
            }
            finally
            {
                WinApi.DeleteObject(hBitmap);
            }

            return bitSrc;
        }

        public static int ToInt(this bool source)
        {
            return source ? 1 : 0;
        }

        public static bool ToBool(this int source)
        {
            return source != 0;
        }

        public static void ResetColumnWidths(this GridView gridView)
        {
            if (gridView != null)
            {
                foreach (var col in gridView.Columns)
                {
                    if (double.IsNaN(col.Width)) col.Width = col.ActualWidth;
                    col.Width = double.NaN;
                }
            }
        }

        public static IntPtr GetHandle(this Window window)
        {
            IntPtr handle = IntPtr.Zero;
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate { handle = new WindowInteropHelper(window).Handle; }));
            return handle;
        }

        public static bool IsValid(this System.Drawing.Point point)
        {
            return point.X != int.MinValue && point.Y != int.MinValue;
        }
    }
}

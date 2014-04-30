using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;

namespace PsHandler
{
    public class Methods
    {
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
            if (CompareColors(r, g, b, button.AvgR, button.AvgG, button.AvgB, button.MaxDiffR, button.MaxDiffG, button.MaxDiffB))
            {
                if (button.ButtonSecondaryCheck != null)// secondary check for some buggy themes
                {
                    CheckButtonAndClick(bmp, button.ButtonSecondaryCheck, handle);
                }
                else
                {
                    //Debug.Write("CLICK ");
                    LeftMouseClickRelative(handle, button.ClickX, button.ClickY, true);
                }
            }
            //Debug.WriteLine(string.Format("{0:0.000} {1:0.000} {2:0.000}", r - button.AvgR, g - button.AvgG, b - button.AvgB));
        }

        private static void ___CheckCheckBoxAndClick(Bmp bmp, ___PokerStarsCheckBox checkBox0, ___PokerStarsCheckBox checkBox1, ___PokerStarsCheckBox checkBox2, bool needToBeChecked, IntPtr handle)
        {
            if (false)
            {
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                {
                    //App.WindowMain.UCHome.StackPanel_Main.Children.Clear();
                    for (int i = 0; i < 3; i++)
                    {
                        Bitmap bitmap = Bmp.CutBitmap(Bmp.BmpToBitmap(bmp), new Rectangle());
                        System.IO.MemoryStream ms = new System.IO.MemoryStream();
                        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        ms.Position = 0;
                        System.Windows.Media.Imaging.BitmapImage bi = new System.Windows.Media.Imaging.BitmapImage();
                        bi.BeginInit();
                        bi.StreamSource = ms;
                        bi.EndInit();

                        System.Windows.Controls.Canvas canvas = new System.Windows.Controls.Canvas();
                        System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                        img.Source = bi;
                        img.Margin = new Thickness(0);
                        canvas.Children.Add(img);
                        canvas.Width = bitmap.Width;
                        canvas.Height = bitmap.Height;
                        canvas.Margin = new Thickness(5);
                        //App.WindowMain.UCHome.StackPanel_Main.Children.Add(canvas);
                    }
                }));
            }
        }

        public static bool IsMinimized(IntPtr handle)
        {
            return (WinApi.GetWindowLong(handle, WinApi.GWL_STYLE) & WinApi.WS_MINIMIZE) != 0;
        }

        public static void LeftMouseClickRelative(IntPtr handle, double x, double y, bool checkIfMinimized)
        {
            if (checkIfMinimized && IsMinimized(handle)) WinApi.ShowWindow(handle, WinApi.SW_RESTORE); // check if window is minimized & restore it
            var rectangle = WinApi.GetClientRectangle(handle);
            LeftMouseClick(handle, (int)Math.Round(rectangle.Width * x, 0), (int)Math.Round(rectangle.Height * y, 0));
        }

        public static void LeftMouseClick(IntPtr handle, int x, int y)
        {
            IntPtr lParam = WinApi.GetLParam(x, y);
            WinApi.PostMessage(handle, WinApi.WM_LBUTTONDOWN, new IntPtr(WinApi.MK_LBUTTON), lParam);
            WinApi.PostMessage(handle, WinApi.WM_LBUTTONUP, IntPtr.Zero, lParam);
        }

        public static void AverageColor(Bmp bmp, Rectangle r, out double redAvg, out double greenAvg, out double blueAvg)
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
            redAvg = (double)redSum / (r.Width * r.Height);
            greenAvg = (double)greenSum / (r.Width * r.Height);
            blueAvg = (double)blueSum / (r.Width * r.Height);
        }

        public static bool CompareColors(double r0, double g0, double b0, double r1, double g1, double b1, double maxDifferenceR, double maxDifferenceG, double maxDifferenceB)
        {
            return Math.Abs(r0 - r1) < maxDifferenceR && Math.Abs(g0 - g1) < maxDifferenceG && Math.Abs(b0 - b1) < maxDifferenceB;
        }

        public static System.Windows.Controls.Canvas GetImage(string pathToImage)
        {
            Bitmap bitmap = GetEmbeddedResourceBitmap(pathToImage);

            System.Windows.Controls.Canvas canvas = new System.Windows.Controls.Canvas();
            System.Windows.Controls.Image img = new System.Windows.Controls.Image { Source = bitmap.ToBitmapSource(), Margin = new Thickness(0) };

            canvas.Children.Add(img);
            canvas.Width = bitmap.Width;
            canvas.Height = bitmap.Height;

            return canvas;
        }
    }
}

using System;
using System.Drawing;

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
                (int)Math.Round(button.X * bmp.Width),
                (int)Math.Round(button.Y * bmp.Height),
                (int)Math.Round(button.Width * bmp.Width),
                (int)Math.Round(button.Height * bmp.Height));
            double r, g, b;
            AverageColor(bmp, rect, out r, out g, out b);
            if (CompareColors(r, g, b, button.AvgRed, button.AvgGreen, button.AvgBlue, button.MaxDiffR, button.MaxDiffG, button.MaxDiffB))
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
            //Debug.WriteLine(string.Format("{0:0.000} {1:0.000} {2:0.000}", r - button.AvgRed, g - button.AvgGreen, b - button.AvgBlue));
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
    }
}

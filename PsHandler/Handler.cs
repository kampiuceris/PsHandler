using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Point = System.Windows.Point;

namespace PsHandler
{
    public class Handler
    {
        private const int DELAY_AUTOCLOSE_TOURNAMENT_REGISTRATION_POPUPS = 250;
        private const int DELAY_TABLE_CONTROL = 2000;
        private static Thread _threadAutocloseTournamentRegistrationPopups;
        private static Thread _threadTableControl;
        private const string LOG_COPY_PATH = "log";

        public static void Start()
        {
            _threadTableControl = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        if (App.AutoclickImBack)
                        {
                            foreach (var process in Process.GetProcessesByName("PokerStars"))
                            {
                                foreach (IntPtr handle in WinApi.EnumerateProcessWindowHandles(process.Id).Where(WinApi.IsWindowVisible))
                                {
                                    string className = WinApi.GetClassName(handle);
                                    if (className.Equals("PokerStarsTableFrameClass"))
                                    {
                                        Bmp bmp = new Bmp(ScreenCapture.GetBitmapWindowClient(handle));
                                        PokerStarsTheme theme = App.PokerStarsTheme;
                                        System.Drawing.Rectangle rect = new System.Drawing.Rectangle((int)Math.Round(theme.ButtonImBackX * bmp.Width),
                                            (int)Math.Round(theme.ButtonImBackY * bmp.Height),
                                            (int)Math.Round(theme.ButtonImBackWidth * bmp.Width),
                                            (int)Math.Round(theme.ButtonImBackHeight * bmp.Height));
                                        double r, g, b;
                                        AverageColor(bmp, rect, out r, out g, out b);
                                        //Debug.WriteLine(string.Format("{0:0.000} {1:0.000} {2:0.000}", r - theme.BmpButtonImBackRed, g - theme.BmpButtonImBackGreen, b - theme.BmpButtonImBackBlue));
                                        if (CompareColors(r, g, b, theme.BmpButtonImBackRed, theme.BmpButtonImBackGreen, theme.BmpButtonImBackBlue, theme.MaxDifferenceR, theme.MaxDifferenceG, theme.MaxDifferenceB))
                                        {
                                            LeftMouseClickRelative(handle, theme.ButtonImBack);
                                        }
                                    }
                                }
                            }
                        }

                        Thread.Sleep(1000);
                    }
                }
                catch (Exception e)
                {
                    if (!(e is ThreadInterruptedException))
                    {
                        System.Windows.MessageBox.Show(e.Message, "Error");
                        System.IO.File.WriteAllText(DateTime.Now.Ticks + ".log", e.Message + Environment.NewLine + Environment.NewLine + e.StackTrace);
                        Stop();
                    }
                }
            });
            _threadTableControl.Start();

            #region AutocloseTournamentRegistrationPopups

            _threadAutocloseTournamentRegistrationPopups = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        if (App.AutocloseTournamentRegistrationPopups)
                        {
                            foreach (var process in Process.GetProcessesByName("PokerStars"))
                            {
                                foreach (IntPtr handle in WinApi.EnumerateProcessWindowHandles(process.Id))
                                {
                                    string className = WinApi.GetClassName(handle);
                                    if (className.Equals("#32770"))
                                    {
                                        string windowTitle = WinApi.GetWindowTitle(handle);
                                        if (windowTitle.Equals("Tournament Registration"))
                                        {
                                            IntPtr buttonOkToClick = WinApi.FindChildWindow(handle, "PokerStarsButtonClass", "");
                                            if (buttonOkToClick.Equals(IntPtr.Zero))
                                            {
                                                buttonOkToClick = WinApi.FindChildWindow(handle, "Button", "OK");
                                            }
                                            if (!buttonOkToClick.Equals(IntPtr.Zero))
                                            {
                                                var rect = WinApi.GetWindowRectangle(buttonOkToClick);
                                                //Debug.WriteLine(string.Format("{0},{1} {2}x{3}", rect.X, rect.Y, rect.Width, rect.Height));

                                                // 85x28 = "OK" button decorated
                                                // 77x24 = "OK" button undecorated
                                                // 133x28 = "Show Lobby" button decorated
                                                // 98x28 = "Close" button decorated

                                                if ((rect.Width == 85 && rect.Height == 28) || (rect.Width == 77 && rect.Height == 24)) // "OK" (decorated) || "OK" (undecorated)
                                                {
                                                    LeftMouseClick(buttonOkToClick, new Point(5, 5));
                                                }
                                                else if (rect.Width == 133 && rect.Height == 28) // "Show Lobby"
                                                {
                                                    IntPtr childAfter = buttonOkToClick;
                                                    buttonOkToClick = WinApi.FindChildWindow(handle, childAfter, "PokerStarsButtonClass", "");
                                                    if (buttonOkToClick != IntPtr.Zero)
                                                    {
                                                        rect = WinApi.GetWindowRectangle(buttonOkToClick);
                                                        if (rect.Width == 98 && rect.Height == 28) // "Close"
                                                        {
                                                            LeftMouseClick(buttonOkToClick, new Point(5, 5));
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        Thread.Sleep(DELAY_AUTOCLOSE_TOURNAMENT_REGISTRATION_POPUPS);
                    }
                }
                catch (Exception e)
                {
                    if (!(e is ThreadInterruptedException))
                    {
                        System.Windows.MessageBox.Show(e.Message, "Error");
                        System.IO.File.WriteAllText(DateTime.Now.Ticks + ".log", e.Message + Environment.NewLine + Environment.NewLine + e.StackTrace);
                        Stop();
                    }
                }
            });
            _threadAutocloseTournamentRegistrationPopups.Start();
            #endregion
        }

        private static string ReadSeek(string path, long seek)
        {
            string text = "";
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                fs.Seek(seek, SeekOrigin.Begin);
                byte[] b = new byte[fs.Length - 100];
                fs.Read(b, 0, (int)(fs.Length - 100));
                text = System.Text.Encoding.UTF8.GetString(b);
            }
            return text;
        }

        private static void LeftMouseClick(IntPtr handle, Point point)
        {
            IntPtr lParam = WinApi.GetLParam((int)point.X, (int)point.Y);
            WinApi.PostMessage(handle, WinApi.WM_LBUTTONDOWN, new IntPtr(WinApi.MK_LBUTTON), lParam);
            WinApi.PostMessage(handle, WinApi.WM_LBUTTONUP, IntPtr.Zero, lParam);
        }

        private static void LeftMouseClickRelative(IntPtr handle, Point relativePoint)
        {
            if ((WinApi.GetWindowLong(handle, WinApi.GWL_STYLE) & WinApi.WS_MINIMIZE) != 0) // check if window is minimized
            {
                WinApi.ShowWindow(handle, WinApi.SW_RESTORE); // restore minimzed window
            }
            var rectangle = WinApi.GetClientRectangle(handle);
            IntPtr lParam = WinApi.GetLParam((int)Math.Round(rectangle.Width * relativePoint.X, 0), (int)Math.Round(rectangle.Height * relativePoint.Y, 0));
            WinApi.PostMessage(handle, WinApi.WM_LBUTTONDOWN, new IntPtr(WinApi.MK_LBUTTON), lParam);
            WinApi.PostMessage(handle, WinApi.WM_LBUTTONUP, IntPtr.Zero, lParam);
        }

        public static void Stop()
        {
            if (_threadAutocloseTournamentRegistrationPopups != null)
            {
                _threadAutocloseTournamentRegistrationPopups.Interrupt();
            }
            if (_threadTableControl != null)
            {
                _threadTableControl.Interrupt();
            }
        }

        public static void AverageColor(Bmp bmp, System.Drawing.Rectangle r, out double redAvg, out double greenAvg, out double blueAvg)
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

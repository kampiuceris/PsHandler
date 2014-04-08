using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;

namespace PsHandler
{
    public class Handler
    {
        private const int DELAY_AUTOCLOSE_TOURNAMENT_REGISTRATION_POPUPS = 250;
        private const int DELAY_TABLE_HANDLE = 1000;
        private static Thread _threadAutocloseTournamentRegistrationPopups;
        private static Thread _threadTableControl;
        private const string LOG_COPY_PATH = "log";

        public static void Start()
        {
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

            #region TableControl

            _threadTableControl = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        string pathPokerStarsLog0 = null;
                        DirectoryInfo di = new DirectoryInfo(App.PokerStarsAppDataPath);
                        if (di.Exists)
                        {
                            pathPokerStarsLog0 = (from fi in di.GetFiles() where fi.Name.Equals("PokerStars.log.0") select fi.FullName).FirstOrDefault();
                        }
                        if (pathPokerStarsLog0 != null)
                        {
                            FileInfo fi = new FileInfo(LOG_COPY_PATH);
                            long seek = 0;
                            if (fi.Exists)
                            {
                                seek = fi.Length;
                            }
                            File.Copy(pathPokerStarsLog0, LOG_COPY_PATH, true);
                            string[] lines = ReadSeek(LOG_COPY_PATH, seek).Split(new string[] { Environment.NewLine, "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (var line in lines)
                            {
                                if (App.AutoclickImBack)
                                {
                                    Match matchSitOut = new Regex(@"<- MSG_0x000F-T\s\w{8}\s(?<handle>\w{8})").Match(line);
                                    Match matchSitOutTimedOut = new Regex(@"-> MSG_0x0036-T\s\w{8}\s(?<handle>\w{8})").Match(line);
                                    if (matchSitOut.Success || matchSitOutTimedOut.Success)
                                    {
                                        IntPtr handle = new IntPtr(int.Parse((matchSitOut.Success ? matchSitOut : matchSitOutTimedOut).Groups["handle"].Value, NumberStyles.HexNumber));
                                        LeftMouseClickRelative(handle, App.GetPokerStarsTheme.ButtonImBack);
                                    }
                                }
                                if (App.AutoclickTimebank)
                                {
                                    Match matchTimeBank = new Regex(@"-> MSG_0x0021-T\s\w{8}\s(?<handle>\w{8})").Match(line);
                                    if (matchTimeBank.Success)
                                    {
                                        IntPtr handle = new IntPtr(int.Parse(matchTimeBank.Groups["handle"].Value, NumberStyles.HexNumber));
                                        LeftMouseClickRelative(handle, App.GetPokerStarsTheme.ButtonTimer);
                                    }
                                }
                            }
                        }
                        Thread.Sleep(DELAY_TABLE_HANDLE);
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
            FileInfo fi = new FileInfo(LOG_COPY_PATH);
            if (fi.Exists)
            {
                fi.Delete();
            }
        }
    }
}

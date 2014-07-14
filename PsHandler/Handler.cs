using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;

namespace PsHandler
{
    public class Handler
    {
        private const int DELAY_AUTOCLOSE_TOURNAMENT_REGISTRATION_POPUPS = 250;
        private const int DELAY_TABLE_CONTROL = 2000;
        private static Thread _threadAutocloseTournamentRegistrationPopups;
        private static Thread _threadTableControl;

        public static void Start()
        {
            #region TableControl

            _threadTableControl = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        if (Config.AutoclickImBack || Config.AutoclickTimebank)
                        {
                            foreach (var process in Process.GetProcessesByName("PokerStars"))
                            {
                                foreach (IntPtr handle in WinApi.EnumerateProcessWindowHandles(process.Id).Where(o => !Methods.IsMinimized(o)))
                                {
                                    string className = WinApi.GetClassName(handle);
                                    if (className.Equals("PokerStarsTableFrameClass"))
                                    {
                                        Bmp bmp = new Bmp(ScreenCapture.GetBitmapWindowClient(handle));
                                        if (Config.AutoclickImBack) Methods.CheckButtonAndClick(bmp, Config.PokerStarsThemeTable.ButtonImBack, handle);
                                        if (Config.AutoclickTimebank) Methods.CheckButtonAndClick(bmp, Config.PokerStarsThemeTable.ButtonTimebank, handle);
                                    }
                                }
                            }
                        }

                        Thread.Sleep(DELAY_TABLE_CONTROL);
                        //Thread.Sleep(500);
                    }
                }
                catch (Exception e)
                {
                    if (!(e is ThreadInterruptedException))
                    {
                        MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        System.IO.File.WriteAllText(DateTime.Now.Ticks + ".log", e.Message + Environment.NewLine + Environment.NewLine + e.StackTrace);
                        Stop();
                    }
                }
            });
            _threadTableControl.Start();

            #endregion

            #region AutoclosePopups

            _threadAutocloseTournamentRegistrationPopups = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        if (Config.AutocloseTournamentRegistrationPopups || Config.AutoclickYesSeatAvailable)
                        {
                            foreach (var process in Process.GetProcessesByName("PokerStars"))
                            {
                                foreach (IntPtr handle in WinApi.EnumerateProcessWindowHandles(process.Id))
                                {
                                    string className = WinApi.GetClassName(handle);
                                    if (className.Equals("#32770"))
                                    {
                                        string windowTitle = WinApi.GetWindowTitle(handle);
                                        if (Config.AutocloseTournamentRegistrationPopups)
                                        {
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
                                                    //Debug.WriteLine(string.Format("{0},{1} {2}x{3}", rect.LocationX, rect.LocationY, rect.Width, rect.Height));

                                                    // 85x28 = "OK" button decorated
                                                    // 77x24 = "OK" button undecorated
                                                    // 133x28 = "Show Lobby" button decorated
                                                    // 98x28 = "Close" button decorated

                                                    if ((rect.Width == 85 && rect.Height == 28) || (rect.Width == 77 && rect.Height == 28))
                                                    // Registration "OK" (decorated) || Unregister "OK" (decorated) (2014-06-25 update)
                                                    {
                                                        Methods.LeftMouseClick(buttonOkToClick, 5, 5);
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
                                                                Methods.LeftMouseClick(buttonOkToClick, 5, 5);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (Config.AutoclickYesSeatAvailable)
                                        {
                                            if (windowTitle.Equals("Seat Available"))
                                            {
                                                IntPtr buttonYes = WinApi.FindChildWindow(handle, "Button", "Yes");
                                                if (!buttonYes.Equals(IntPtr.Zero))
                                                {
                                                    Methods.LeftMouseClick(buttonYes, 5, 5);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (Config.AutocloseHM2ApplyToSimilarTablesPopups)
                        {
                            foreach (var process in Process.GetProcessesByName("HoldemManager"))
                            {
                                foreach (IntPtr handle in WinApi.EnumerateProcessWindowHandles(process.Id))
                                {
                                    string windowTitle = WinApi.GetWindowTitle(handle);
                                    if (windowTitle.Equals("Apply to similar tables?"))
                                    {
                                        WinApi.SendMessage(handle, WinApi.WM_SYSCOMMAND, new IntPtr(WinApi.SC_CLOSE), IntPtr.Zero);
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
                        MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        System.IO.File.WriteAllText(DateTime.Now.Ticks + ".log", e.Message + Environment.NewLine + Environment.NewLine + e.StackTrace);
                        Stop();
                    }
                }
            });
            _threadAutocloseTournamentRegistrationPopups.Start();
            #endregion
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

        public static void ClickReplayHandButton()
        {
            IntPtr handle = WinApi.GetForegroundWindow();
            string className = WinApi.GetClassName(handle);
            if (className.Equals("PokerStarsTableFrameClass"))
            {
                Methods.LeftMouseClickRelative(handle, Config.PokerStarsThemeTable.ButtonHandReplayX, Config.PokerStarsThemeTable.ButtonHandReplayY, false);
            }
        }
    }
}

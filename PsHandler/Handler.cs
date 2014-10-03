using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using PsHandler.Custom;
using PsHandler.UI;
using System.Windows.Controls;

namespace PsHandler
{
    public class Handler
    {
        private const int DELAY_MS = 250;

        private static Thread _threadHandler;

        public static void Start()
        {
            _threadHandler = new Thread(() =>
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

                        Thread.Sleep(DELAY_MS);
                    }
                }
#if (DEBUG)
                catch (ThreadInterruptedException)
                {
                }
#else
                catch (Exception e)
                {
                    if (!(e is ThreadInterruptedException))
                    {
                        Methods.DisplayException(e);
                    }
                }
#endif
            });
            _threadHandler.Start();
        }

        public static void Stop()
        {
            if (_threadHandler != null)
            {
                _threadHandler.Interrupt();
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

        // Quick Preview

        private static Thread _threadQuickPreview;
        public static bool _quickPreviewIsRunning;
        public static Window _windowQuickPreview;
        public static Image _imageQuickPreview;

        public static void QuickPreviewStart()
        {
            if (_threadQuickPreview == null || !_quickPreviewIsRunning)
            {
                QuickPreviewStop();
                _quickPreviewIsRunning = true;

                _threadQuickPreview = new Thread(() =>
                {
                    try
                    {
                        IntPtr handle = WinApi.GetForegroundWindow();
                        string windowTitle = WinApi.GetWindowTitle(handle);
                        string windowClass = WinApi.GetClassName(handle);

                        if (!(windowClass.Equals("PokerStarsTableFrameClass") && WinApi.IsWindowVisible(handle) &&
                            !Methods.IsMinimized(handle) && !string.IsNullOrEmpty(windowTitle)))
                        {
                            handle = IntPtr.Zero;
                        }

                        Methods.UiInvoke(() =>
                        {
                            _windowQuickPreview = new Window
                            {
                                Title = "Quick Preview",
                                SizeToContent = SizeToContent.WidthAndHeight,
                                Topmost = true,
                                WindowStyle = WindowStyle.None,
                                UseLayoutRounding = true,
                                ResizeMode = ResizeMode.NoResize,
                                Background = Brushes.Black,
                                Width = 5,
                                Height = 5,
                                ShowInTaskbar = false,
                                Focusable = false,
                                Visibility = Visibility.Visible
                            };
                            _imageQuickPreview = new Image();
                            _windowQuickPreview.Content = _imageQuickPreview;
                            _windowQuickPreview.SourceInitialized += (sender, args) =>
                            {
                                var interopHelper = new WindowInteropHelper(_windowQuickPreview);
                                int exStyle = (int)WinApi.GetWindowLong(interopHelper.Handle, WinApi.GWL_EXSTYLE);
                                WinApi.SetWindowLong(interopHelper.Handle, WinApi.GWL_EXSTYLE, exStyle | WinApi.WS_EX_NOACTIVATE);
                            };
                            _windowQuickPreview.Show();
                        });

                        while (true)
                        {
                            if (handle != IntPtr.Zero)
                            {
                                System.Drawing.Bitmap bitmapWindowClient = ScreenCapture.GetBitmapWindowClient(handle);
                                if (bitmapWindowClient != null)
                                {
                                    Methods.UiInvoke(() =>
                                    {
                                        _imageQuickPreview.Width = bitmapWindowClient.Width;
                                        _imageQuickPreview.Height = bitmapWindowClient.Height;
                                        _imageQuickPreview.Source = bitmapWindowClient.ToBitmapSource();
                                        System.Drawing.Rectangle clientRectangle = WinApi.GetClientRectangle(handle);
                                        _windowQuickPreview.Left = clientRectangle.Left;
                                        _windowQuickPreview.Top = clientRectangle.Top;
                                    });
                                }
                            }

                            Thread.Sleep(25);
                        }
                    }
#if (DEBUG)
                    catch (ThreadInterruptedException)
                    {
                    }
#else
                    catch (Exception e)
                    {
                        if (!(e is ThreadInterruptedException))
                        {
                            Methods.DisplayException(e);
                        }
                    }
#endif
                    finally
                    {
                        Methods.UiInvoke(() => { _windowQuickPreview.Close(); });
                        _quickPreviewIsRunning = false;
                    }
                });
                _threadQuickPreview.Start();
            }
        }

        public static void QuickPreviewStop()
        {
            if (_threadQuickPreview != null)
            {
                _threadQuickPreview.Interrupt();
            }
        }
    }
}

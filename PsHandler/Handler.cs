// PsHandler - poker software helping tool.
// Copyright (C) 2014-2015  kampiuceris

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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using PsHandler.Custom;
using PsHandler.UI;
using Brushes = System.Windows.Media.Brushes;
using Image = System.Windows.Controls.Image;

namespace PsHandler
{
    public class Handler
    {
        private const int DELAY_MS = 250;

        private Thread _threadHandler;

        public Handler()
        {
            Start();
        }

        public void Start()
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
                                                // button "OK"
                                                IntPtr handleButton = WinApi.FindChildWindow(handle, "PokerStarsButtonClass", "OK");
                                                // button "Confirm"
                                                if (handleButton.Equals(IntPtr.Zero))
                                                {
                                                    handleButton = WinApi.FindChildWindow(handle, "PokerStarsButtonClass", "Confirm");
                                                }
                                                // Ctrl + S -> button "Close"
                                                if (handleButton.Equals(IntPtr.Zero))
                                                {
                                                    IntPtr handleButtonShowLobby = WinApi.FindChildWindow(handle, "PokerStarsButtonClass", "Show Lobby");
                                                    if (!handleButtonShowLobby.Equals(IntPtr.Zero))
                                                    {
                                                        handleButton = WinApi.FindChildWindow(handle, "PokerStarsButtonClass", "Close");
                                                    }
                                                }
                                                // click if possible
                                                if (!handleButton.Equals(IntPtr.Zero))
                                                {
                                                    Rectangle r = WinApi.GetClientRectangle(handleButton);
                                                    Methods.LeftMouseClick(handleButton, r.Width / 2, r.Height / 2);
                                                }
                                            }
                                        }
                                        if (Config.AutoclickYesSeatAvailable)
                                        {
                                            if (windowTitle.Equals("Seat Available"))
                                            {
                                                IntPtr handleButton = WinApi.FindChildWindow(handle, "Button", "Yes");
                                                // click if possible
                                                if (!handleButton.Equals(IntPtr.Zero))
                                                {
                                                    Rectangle r = WinApi.GetClientRectangle(handleButton);
                                                    Methods.LeftMouseClick(handleButton, r.Width / 2, r.Height / 2);
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
                        Methods.DisplayException(e, App.WindowMain, WindowStartupLocation.CenterOwner);
                    }
                }
#endif
            });
            _threadHandler.Start();
        }

        public void Stop()
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
                Methods.LeftMouseClickRelativeScaled(handle, Config.PokerStarsThemeTable.ButtonHandReplayX, Config.PokerStarsThemeTable.ButtonHandReplayY, false);
            }
        }

        // Quick Preview

        private Thread _threadQuickPreview;
        public bool _quickPreviewIsRunning;
        public Window _windowQuickPreview;
        public Image _imageQuickPreview;

        public void QuickPreviewStart()
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
                            Methods.DisplayException(e, App.WindowMain, WindowStartupLocation.CenterOwner);
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

        public void QuickPreviewStop()
        {
            if (_threadQuickPreview != null)
            {
                _threadQuickPreview.Interrupt();
            }
        }
    }
}

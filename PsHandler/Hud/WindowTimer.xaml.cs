using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PsHandler.Types;
using Rectangle = System.Drawing.Rectangle;

namespace PsHandler.Hud
{
    /// <summary>
    /// Interaction logic for WindowTimer.xaml
    /// </summary>
    public partial class WindowTimer : Window
    {
        public IntPtr HandleOwner;
        private Thread _thread;
        private PokerType _pokerType;

        public IntPtr Handle
        {
            get
            {
                IntPtr handle = IntPtr.Zero;
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate { handle = new WindowInteropHelper(this).Handle; }));
                return handle;
            }
        }

        public WindowTimer()
        {
            InitializeComponent();
            Timer_Main.SetBackground(HudManager.TimerHudBackground);
            Timer_Main.SetForeground(HudManager.TimerHudForeground);
            Timer_Main.SetFontFamily(HudManager.TimerHudFontFamily);
            Timer_Main.SetFontWeight(HudManager.TimerHudFontWeight);
            Timer_Main.SetFontStyle(HudManager.TimerHudFontStyle);
            Timer_Main.SetFontSize(HudManager.TimerHudFontSize);
        }

        public void SetOwner(IntPtr handleOwner)
        {
            if (HandleOwner.ToInt32() != 0) throw new NotSupportedException("Cannot set owner more than once.");

            HandleOwner = handleOwner;
            WinApi.SetWindowLong(Handle, -8, HandleOwner.ToInt32()); // set owner

            _thread = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        if (WinApi.IsWindow(HandleOwner))
                        {
                            Rectangle rect = WinApi.GetClientRectangle(HandleOwner);
                            string title = WinApi.GetWindowTitle(handleOwner);
                            Regex regex = new Regex(@".+- Tournament (?<tn>\d+).+Logged In as.+");
                            Match match = regex.Match(title);

                            string textboxContent = "";
                            if (match.Success)
                            {
                                long toutnamentNumber = long.Parse(match.Groups["tn"].Value);
                                TournamentInfo tournamentInfo = HudManager.GetTournamentInfo(toutnamentNumber);
                                if (tournamentInfo != null)
                                {
                                    if (_pokerType == null)
                                    {
                                        _pokerType = HudManager.FindPokerType(title, tournamentInfo.FileInfo.Name);
                                    }
                                    if (_pokerType != null)
                                    {
                                        DateTime dateTimeNow = DateTime.Now.AddSeconds(-App.TimeDiff);
                                        DateTime dateTimeNextLevel = tournamentInfo.TimestampStarted;
                                        while (dateTimeNextLevel < dateTimeNow) dateTimeNextLevel = dateTimeNextLevel.AddSeconds(_pokerType.LevelLengthInSeconds);
                                        TimeSpan timeSpan = dateTimeNextLevel - dateTimeNow;
                                        textboxContent = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
                                    }
                                    else
                                    {
                                        textboxContent = string.Format("Unknown PokerType");
                                    }
                                }
                                else
                                {
                                    textboxContent = string.Format("HH not found");
                                }
                            }

                            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                            {
                                if (!_mouseDown)
                                {
                                    Left = rect.X + rect.Width * HudManager.TimerHudLocationX;
                                    Top = rect.Y + rect.Height * HudManager.TimerHudLocationY;
                                }
                            }));
                            Timer_Main.SetText(textboxContent);
                        }
                        else
                        {
                            break;
                        }
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception e)
                {
                }
                finally
                {
                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(Close));
                }
            });
            _thread.Start();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (_thread != null) _thread.Interrupt();
            base.OnClosing(e);
        }

        // move

        private bool _mouseDown;
        private Point _startPosition;

        private void WindowMouseDown(object sender, MouseButtonEventArgs e)
        {
            _mouseDown = true;
        }

        private void WindowMouseUp(object sender, MouseButtonEventArgs e)
        {
            _mouseDown = false;
        }

        private void WindowLocationChanged(object sender, EventArgs e)
        {
            if (_mouseDown)
            {
                Rectangle r = WinApi.GetClientRectangle(HandleOwner);
                HudManager.TimerHudLocationX = (float)((Left - r.Left) / r.Width);
                HudManager.TimerHudLocationY = (float)((Top - r.Top) / r.Height);
            }
        }

        private void Window_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPosition = e.GetPosition(this);
        }

        private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!HudManager.TimerHudLocationLocked)
            {
                if (e.RightButton == MouseButtonState.Pressed)
                {
                    Point endPosition = e.GetPosition(this);
                    Vector vector = endPosition - _startPosition;
                    Left = (int)(Left + vector.X);
                    Top = (int)(Top + vector.Y);
                }
            }
        }
    }
}

using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using PsHandler.Hud.Import;
using PsHandler.PokerTypes;
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
        private static readonly Regex _regexTournament = new Regex(@".+- Tournament (?<tn>\d+).+");
        private static readonly Regex _regexTournamentLoggedIn = new Regex(@".+- Tournament (?<tn>\d+).+Logged In as (?<hero>.+)");

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
                            string title = WinApi.GetWindowTitle(handleOwner);
                            string textboxContent = "";
                            Match match = _regexTournament.Match(title);
                            if (match.Success)
                            {
                                long toutnamentNumber = long.Parse(match.Groups["tn"].Value);

                                string hero = null;
                                match = _regexTournamentLoggedIn.Match(title);
                                if (match.Success) hero = match.Groups["hero"].Value;

                                Tournament tournamentInfo = App.Import.GetTournament(toutnamentNumber);
                                if (tournamentInfo != null)
                                {
                                    int pokerTypeErrors = -1;
                                    if (_pokerType == null)
                                    {
                                        _pokerType = HudManager.FindPokerType(title, tournamentInfo.FileInfo.Name, out pokerTypeErrors);
                                        if (pokerTypeErrors != 0)
                                        {
                                            _pokerType = null;
                                        }
                                    }
                                    if (_pokerType != null)
                                    {
                                        DateTime dateTimeNow = DateTime.Now.AddSeconds(-Config.TimeDiff);

                                        DateTime dateTimeNextLevel = tournamentInfo.GetFirstHandTimestamp();
                                        while (dateTimeNextLevel < dateTimeNow) dateTimeNextLevel = dateTimeNextLevel.AddSeconds(_pokerType.LevelLengthInSeconds);
                                        TimeSpan timeSpan = dateTimeNextLevel - dateTimeNow;
                                        textboxContent = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);

                                        decimal latestStack = tournamentInfo.GetLatestStack(hero);
                                        if (latestStack != decimal.MinValue)
                                        {
                                            textboxContent += " " + latestStack;
                                        }
                                    }
                                    else
                                    {
                                        if (pokerTypeErrors == 1)
                                        {
                                            textboxContent = string.Format("PokerType not found");
                                        }
                                        else if (pokerTypeErrors == 2)
                                        {
                                            textboxContent = string.Format("Multiple PokerTypes");
                                        }
                                        else
                                        {
                                            textboxContent = string.Format("Unknown Error");
                                        }
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
                                    Rectangle rect = WinApi.GetClientRectangle(HandleOwner);
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
                catch (Exception)
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

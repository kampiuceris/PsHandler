using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
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

        public WindowTimer(IntPtr handleOwner)
        {
            InitializeComponent();
            UCLabel_Main.SetBackground(HudManager.TimerHudBackground);
            UCLabel_Main.SetForeground(HudManager.TimerHudForeground);
            UCLabel_Main.SetFontFamily(HudManager.TimerHudFontFamily);
            UCLabel_Main.SetFontWeight(HudManager.TimerHudFontWeight);
            UCLabel_Main.SetFontStyle(HudManager.TimerHudFontStyle);
            UCLabel_Main.SetFontSize(HudManager.TimerHudFontSize);
            Opacity = 0;

            Loaded += (sender, args) => SetOwner(handleOwner);
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
                            bool visible = false;
                            Match match = _regexTournament.Match(title);
                            if (match.Success)
                            {
                                visible = true;
                                long toutnamentNumber = long.Parse(match.Groups["tn"].Value);
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
                                        DateTime dateTimeNow = DateTime.Now.AddSeconds(-Config.TimerDiff);
                                        DateTime dateTimeNextLevel = tournamentInfo.GetFirstHandTimestamp();
                                        while (dateTimeNextLevel < dateTimeNow) dateTimeNextLevel = dateTimeNextLevel.AddSeconds(_pokerType.LevelLengthInSeconds);
                                        TimeSpan timeSpan = dateTimeNextLevel - dateTimeNow;
                                        textboxContent = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
                                    }
                                    else
                                    {
                                        if (pokerTypeErrors == 1)
                                        {
                                            //textboxContent = string.Format("PokerType not found");
                                            textboxContent = Config.TimerPokerTypeNotFound;
                                        }
                                        else if (pokerTypeErrors == 2)
                                        {
                                            //textboxContent = string.Format("Multiple PokerTypes");
                                            textboxContent = Config.TimerMultiplePokerTypes;
                                        }
                                        else
                                        {
                                            textboxContent = string.Format("Unknown Error");
                                        }
                                    }
                                }
                                else
                                {
                                    //textboxContent = string.Format("HH not found");
                                    textboxContent = Config.TimerHHNotFound;
                                }
                            }
                            else
                            {
                                visible = false;
                            }

                            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                            {
                                if (!_mouseDown)
                                {
                                    System.Drawing.Rectangle rect = WinApi.GetClientRectangle(HandleOwner);
                                    Left = rect.X + rect.Width * HudManager.GetTimerHudLocationX(this);
                                    Top = rect.Y + rect.Height * HudManager.GetTimerHudLocationY(this);
                                    if (visible)
                                    {
                                        Opacity = 1;
                                        UCLabel_Main.SetBackground(HudManager.TimerHudBackground);
                                        UCLabel_Main.SetForeground(HudManager.TimerHudForeground);
                                        UCLabel_Main.SetFontFamily(HudManager.TimerHudFontFamily);
                                        UCLabel_Main.SetFontWeight(HudManager.TimerHudFontWeight);
                                        UCLabel_Main.SetFontStyle(HudManager.TimerHudFontStyle);
                                        UCLabel_Main.SetFontSize(HudManager.TimerHudFontSize);
                                    }
                                    else
                                    {
                                        Opacity = 0;
                                    }
                                }
                            }));
                            UCLabel_Main.SetText(textboxContent);
                        }
                        else
                        {
                            break;
                            //throw new Exception("Parent is not a window.");
                        }
                        Thread.Sleep(1000);
                    }
                }
                catch (Exception)
                {
                }
                finally
                {
                    Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(Close));
                }
            });
            _thread.Start();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (_thread != null) _thread.Interrupt();
            base.OnClosing(e);
        }

        //

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
                System.Drawing.Rectangle r = WinApi.GetClientRectangle(HandleOwner);
                HudManager.SetTimerHudLocationX((float)((Left - r.Left) / r.Width), this);
                HudManager.SetTimerHudLocationY((float)((Top - r.Top) / r.Height), this);
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

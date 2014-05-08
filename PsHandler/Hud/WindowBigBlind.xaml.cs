using PsHandler.Hud.Import;
using System;
using System.Collections.Generic;
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

namespace PsHandler.Hud
{
    /// <summary>
    /// Interaction logic for WindowBigBlind.xaml
    /// </summary>
    public partial class WindowBigBlind : Window
    {
        public IntPtr HandleOwner;
        private Thread _thread;
        private static readonly Regex _regexSbBb = new Regex(@".+Blinds .{0,1}(?<sb>[\d\.]+)\/.{0,1}(?<bb>[\d\.]+) - Tournament (?<tournament_number>\d+).+Logged In as (?<hero>.+)");
        private static readonly Regex _regexSbBbAnte = new Regex(@".+Blinds .{0,1}(?<sb>[\d\.]+)\/.{0,1}(?<bb>[\d\.]+) Ante .{0,1}(?<ante>[\d\.]+) - Tournament (?<tournament_number>\d+).+Logged In as (?<hero>.+)");

        public WindowBigBlind(IntPtr handleOwner)
        {
            InitializeComponent();
            UCLabel_Main.SetBackground(HudManager.BigBlindHudBackground);
            UCLabel_Main.SetForeground(HudManager.BigBlindHudForeground);
            UCLabel_Main.SetFontFamily(HudManager.BigBlindHudFontFamily);
            UCLabel_Main.SetFontWeight(HudManager.BigBlindHudFontWeight);
            UCLabel_Main.SetFontStyle(HudManager.BigBlindHudFontStyle);
            UCLabel_Main.SetFontSize(HudManager.BigBlindHudFontSize);
            Opacity = 0;

            Loaded += (sender, args) => SetOwner(handleOwner);
        }

        public IntPtr Handle
        {
            get
            {
                IntPtr handle = IntPtr.Zero;
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate { handle = new WindowInteropHelper(this).Handle; }));
                return handle;
            }
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
                            string textboxContent = "X";
                            bool visible = false;

                            decimal sb = decimal.MinValue, bb = decimal.MinValue, ante = decimal.MinValue;
                            string hero = "";
                            Match match = _regexSbBb.Match(title);
                            if (!match.Success)
                            {
                                match = _regexSbBbAnte.Match(title);
                                if (match.Success)
                                {
                                    ante = decimal.Parse(match.Groups["ante"].Value);
                                }
                            }
                            if (match.Success)
                            {
                                visible = true;
                                sb = decimal.Parse(match.Groups["sb"].Value);
                                bb = decimal.Parse(match.Groups["bb"].Value);
                                long toutnamentNumber = long.Parse(match.Groups["tournament_number"].Value);
                                hero = match.Groups["hero"].Value;

                                Tournament tournamentInfo = App.Import.GetTournament(toutnamentNumber);
                                if (tournamentInfo != null)
                                {
                                    decimal latestStack = tournamentInfo.GetLatestStack(hero);
                                    if (latestStack != decimal.MinValue)
                                    {
                                        //textboxContent = sb + "/" + bb + (ante == decimal.MinValue ? "" : (" " + ante)) + " " + latestStack;
                                        decimal bigBlinds = Math.Round(latestStack / bb, Config.BigBlindDecimals);
                                        string decimalFormat = "0:0.";
                                        for (int i = 0; i < Config.BigBlindDecimals; i++) decimalFormat += "0";
                                        textboxContent = string.Format("{" + decimalFormat + "}", bigBlinds);
                                    }
                                }
                            }

                            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                            {
                                if (!_mouseDown)
                                {
                                    System.Drawing.Rectangle rect = WinApi.GetClientRectangle(HandleOwner);
                                    Left = rect.X + rect.Width * HudManager.GetBigBlindHudLocationX(this);
                                    Top = rect.Y + rect.Height * HudManager.GetBigBlindHudLocationY(this);
                                    if (visible)
                                    {
                                        Opacity = 1;
                                        UCLabel_Main.SetBackground(HudManager.BigBlindHudBackground);
                                        UCLabel_Main.SetForeground(HudManager.BigBlindHudForeground);
                                        UCLabel_Main.SetFontFamily(HudManager.BigBlindHudFontFamily);
                                        UCLabel_Main.SetFontWeight(HudManager.BigBlindHudFontWeight);
                                        UCLabel_Main.SetFontStyle(HudManager.BigBlindHudFontStyle);
                                        // size relative to default PS table size
                                        double tableSizeRatio = rect.Height / PokerStarsThemeTable.HEIGHT;
                                        UCLabel_Main.SetFontSize(HudManager.BigBlindHudFontSize * tableSizeRatio);
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
                        Thread.Sleep(2000);
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
                HudManager.SetBigBlindHudLocationX((float)((Left - r.Left) / r.Width), this);
                HudManager.SetBigBlindHudLocationY((float)((Top - r.Top) / r.Height), this);
            }
        }

        private void Window_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPosition = e.GetPosition(this);
        }

        private void Window_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!HudManager.BigBlindHudLocationLocked)
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

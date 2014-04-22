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
                            Regex regex = new Regex(@".+- Tournament (?<tn>\d+)");
                            Match match = regex.Match(title);

                            string textboxContent = "--:--";
                            if (match.Success)
                            {
                                long toutnamentNumber = long.Parse(match.Groups["tn"].Value);
                                TournamentInfo tournamentInfo = HudManager.GetTournamentInfo(toutnamentNumber);
                                if (tournamentInfo != null)
                                {
                                    TimeSpan timeSpan = DateTime.Now.AddSeconds(-App.TimeDiff) - tournamentInfo.TimestampStarted;
                                    int levelMinutes = 3;

                                    double min = (levelMinutes - 1) - (int)timeSpan.TotalMinutes % levelMinutes;
                                    double s = 59 - (int)timeSpan.TotalSeconds % 60;
                                    textboxContent = string.Format("{0:00}:{1:00}", min, s);

                                    //Debug.WriteLine(timeSpan.TotalMinutes + " " + timeSpan.TotalSeconds + " " + textboxContent);
                                }
                            }

                            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                            {
                                Left = rect.X;
                                Top = rect.Top;
                                LabelTimer.Content = textboxContent;
                            }));
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
    }
}

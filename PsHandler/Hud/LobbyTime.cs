using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;

namespace PsHandler
{
    class Symbol
    {
        public string SymbolText;
        public Bmp Bmp;
    }

    public class LobbyTime
    {
        private readonly List<Symbol> _symbolsBlack;
        private readonly List<Symbol> _symbolsClassic;
        private Thread _threadSyncTime;
        public bool SyncInWork;

        public LobbyTime()
        {
            _symbolsBlack = new List<Symbol>();
            _symbolsClassic = new List<Symbol>();

            List<string> list = new List<string>();
            for (int i = 0; i < 10; i++)
                list.Add(i.ToString(CultureInfo.InvariantCulture));
            //list.Add("separator");

            foreach (string s in list)
            {
                Bmp bmp = new Bmp(Methods.GetEmbeddedResourceBitmap(string.Format("PsHandler.Images.Lobby.Black.{0}.png", s)));
                MakeBlackWhite(ref bmp, 255, 255, 255, 50, 50, 50);
                _symbolsBlack.Add(new Symbol { SymbolText = s.Equals("separator") ? ":" : s, Bmp = bmp });

                bmp = new Bmp(Methods.GetEmbeddedResourceBitmap(string.Format("PsHandler.Images.Lobby.Classic.{0}.png", s)));
                MakeBlackWhite(ref bmp, 231, 201, 106, 100, 100, 100);
                _symbolsClassic.Add(new Symbol { SymbolText = s.Equals("separator") ? ":" : s, Bmp = bmp });
            }
        }

        public static void MakeBlackWhite(ref Bmp bmp, int r, int g, int b, int diffR, int diffG, int diffB)
        {
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    bmp[x, y] = (Math.Abs(bmp.GetPixelR(x, y) - r) < diffR && Math.Abs(bmp.GetPixelG(x, y) - g) < diffG && Math.Abs(bmp.GetPixelB(x, y) - b) < diffB) ? new byte[] { 255, 255, 255, 255 } : new byte[] { 255, 0, 0, 0 };
                }
            }

        }

        private string GetLetter(Bmp bmp, int x, int y, bool themeClassic)
        {
            List<Symbol> _symbols = themeClassic ? _symbolsClassic : _symbolsBlack;

            Symbol bestSymbol = null;
            double compatibility = 0;

            foreach (Symbol s in _symbols)
            {
                bool enoughWidthAndHeight = (x + s.Bmp.Width < bmp.Width) && (y + s.Bmp.Height < bmp.Height);
                if (enoughWidthAndHeight)
                {
                    int matchSum = 0;
                    for (int sy = 0; sy < s.Bmp.Height; sy++)
                    {
                        for (int sx = 0; sx < s.Bmp.Width; sx++)
                        {
                            if (bmp.GetPixelR(x + sx, y + sy) == s.Bmp.GetPixelR(sx, sy)
                                && bmp.GetPixelG(x + sx, y + sy) == s.Bmp.GetPixelG(sx, sy)
                                && bmp.GetPixelB(x + sx, y + sy) == s.Bmp.GetPixelB(sx, sy))
                            {
                                matchSum++;
                            }
                        }
                    }
                    double avg = (double)matchSum / (s.Bmp.Width * s.Bmp.Height);
                    if (avg > compatibility)
                    {
                        bestSymbol = s;
                        compatibility = avg;
                    }
                }
            }

            double compMax = themeClassic ? 0.9 : 0.95;
            if (compatibility < compMax) bestSymbol = null;
            //if (bestSymbol != null) Debug.WriteLine(bestSymbol.SymbolText + " " + compatibility);

            return bestSymbol == null ? "" : bestSymbol.SymbolText;
        }

        public string GetText(Bmp bmp, bool themeClassic)
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    sb.Append(GetLetter(bmp, x, y, themeClassic));
                }
            }
            return sb.ToString();
        }

        public void StartSync()
        {
            SyncInWork = true;

            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
            {
                App.Gui.Label_TimeDiff.Visibility = Visibility.Hidden;
                App.Gui.TextBox_TimeDiff.Visibility = Visibility.Hidden;
                App.Gui.ProgressBar_TimeSync.Visibility = Visibility.Visible;
                App.Gui.Button_SyncTime.Content = "Cancel Sync";
            }));

            _threadSyncTime = new Thread(() =>
            {
                try
                {
                    DateTime dateTimeStarted = DateTime.Now;
                    Regex regex = new Regex(@"\d\d\d\d");
                    int hours = -1;
                    int minutes = -1;

                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate { App.Gui.ProgressBar_TimeSync.Maximum = 70; App.Gui.ProgressBar_TimeSync.Value = 0; }));

                    while (true)
                    {
                        PokerStarsThemeLobby selectedThemeLobby = App.PokerStarsThemeLobby;
                        if (selectedThemeLobby is PokerStarsThemesLobby.Unknown) break;
                        bool isClassicTheme = selectedThemeLobby is PokerStarsThemesLobby.Classic;

                        Process[] processesByName = Process.GetProcessesByName("PokerStars");
                        if (processesByName.Length == 0) throw new Exception("No 'Pokerstars' process.");

                        foreach (var process in processesByName)
                        {
                            foreach (IntPtr handle in WinApi.EnumerateProcessWindowHandles(process.Id))
                            {
                                string className = WinApi.GetClassName(handle);
                                if (className.Equals("#32770"))
                                {
                                    string windowTitle = WinApi.GetWindowTitle(handle);
                                    if (windowTitle.StartsWith("PokerStars Lobby"))
                                    {
                                        if (Methods.IsMinimized(handle)) throw new Exception("PokerStars Lobby minimized");

                                        Bmp bmp = new Bmp(ScreenCapture.GetBitmapWindowClient(handle));
                                        DateTime dateTimeNow = DateTime.Now;

                                        if (!isClassicTheme)
                                        {
                                            bmp = Bmp.CutBmp(bmp, new Rectangle((int)(bmp.Width * 0.70328), bmp.Height - 56, (int)(bmp.Width * 0.82071 - bmp.Width * 0.70328), 25));
                                            MakeBlackWhite(ref bmp, 255, 255, 255, 50, 50, 50);
                                        }
                                        else
                                        {
                                            bmp = Bmp.CutBmp(bmp, new Rectangle(0, bmp.Height - 34, 80, 20));
                                            MakeBlackWhite(ref bmp, 231, 201, 106, 100, 100, 100);
                                        }

                                        //Debug.WriteLine(GetText(bmp, isClassicTheme) + " " + DateTime.Now);
                                        string text = GetText(bmp, isClassicTheme);
                                        if (text.Length == 4 && regex.IsMatch(text))
                                        {
                                            int h = int.Parse(text.Substring(0, 2));
                                            int m = int.Parse(text.Substring(2, 2));

                                            if (hours == -1 && minutes == -1)
                                            {
                                                // set base
                                                hours = h;
                                                minutes = m;
                                            }
                                            else
                                            {
                                                // check for changes
                                                if (hours != h || minutes != m)
                                                {
                                                    // first change, actual PS server time is h:m:00
                                                    DateTime dt = new DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day, h, m, 00);
                                                    Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate { App.Gui.TextBox_TimeDiff.Text = Math.Round((dateTimeNow - dt).TotalSeconds).ToString(); }));
                                                    throw new Exception("Done!");
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        Thread.Sleep(500);

                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate { App.Gui.ProgressBar_TimeSync.Value = (DateTime.Now - dateTimeStarted).TotalSeconds; }));
                        if ((DateTime.Now - dateTimeStarted).TotalSeconds > 70)
                        {
                            MessageBox.Show("Cannot scan time PokerStars Lobby window. Check if your lobby theme is set correctly.", "Sync Time Out", MessageBoxButton.OK, MessageBoxImage.Warning);
                            throw new Exception("Time out.");
                        }
                    }
                }
                catch
                {
                }
                finally
                {
                    new Thread(StopSync).Start();
                }
            });
            _threadSyncTime.Start();
        }

        public void StopSync()
        {
            if (_threadSyncTime != null) _threadSyncTime.Abort();

            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
            {
                App.Gui.Button_SyncTime.Content = "Synchronize (beta)";
                App.Gui.ProgressBar_TimeSync.Visibility = Visibility.Hidden;
                App.Gui.Label_TimeDiff.Visibility = Visibility.Visible;
                App.Gui.TextBox_TimeDiff.Visibility = Visibility.Visible;
            }));

            SyncInWork = false;
        }
    }
}

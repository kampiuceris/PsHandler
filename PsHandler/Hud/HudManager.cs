using Microsoft.Win32;
using PsHandler.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace PsHandler.Hud
{
    public class HudManager
    {
        private static readonly List<WindowTimer> _timerWindows = new List<WindowTimer>();
        private static readonly object _lock = new object();
        private static readonly List<TournamentInfo> _tournamentInfos = new List<TournamentInfo>();
        private static Thread _thread;

        private static readonly Regex _RegexFileName = new Regex(@"HH\d{8} T(?<tn>\d{9,10})");
        private static readonly Regex _RegexTimestamp = new Regex(@".+ - (?<timestamp>\d{4}.\d{2}.\d{2} \d{1,2}:\d{2}:\d{2})");
        public static readonly List<PokerType> PokerTypes = new List<PokerType>();
        //
        public static bool TimerHudLocationLocked = false;
        public static float TimerHudLocationX = 0;
        public static float TimerHudLocationY = 0;
        public static Color TimerHudBackground = Colors.Black;
        public static Color TimerHudForeground = Colors.White;
        public static FontFamily TimerHudFontFamily = new FontFamily("Consolas");
        public static FontWeight TimerHudFontWeight = FontWeights.Bold;
        public static FontStyle TimerHudFontStyle = FontStyles.Normal;
        public static double TimerHudFontSize = 10;

        public static void Start()
        {
            Stop();

            _thread = new Thread(() =>
            {
                try
                {
                    // load poker types
                    LoadPokerTypesFromRegistry();

                    // search for new pokerstars table windows
                    while (true)
                    {
                        // update tournament info list
                        UpdateTournamentInfos();

                        foreach (var process in Process.GetProcessesByName("PokerStars"))
                        {
                            foreach (IntPtr handle in WinApi.EnumerateProcessWindowHandles(process.Id))
                            {
                                string className = WinApi.GetClassName(handle);
                                if (className.Equals("PokerStarsTableFrameClass"))
                                {
                                    WindowTimer find = _timerWindows.FirstOrDefault(tw => tw.HandleOwner.Equals(handle));

                                    if (find == null)
                                    {
                                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                                        {
                                            // new window
                                            WindowTimer wt = new WindowTimer();
                                            wt.Show();
                                            wt.SetOwner(handle);
                                            _timerWindows.Add(wt);
                                        }));
                                    }
                                }
                            }
                        }

                        // clean closed windows
                        foreach (WindowTimer tw in _timerWindows.Where(tw => !WinApi.IsWindow(tw.Handle)).ToList()) _timerWindows.Remove(tw);

                        Thread.Sleep(2000);
                    }
                }
                catch (Exception e)
                {
                    if (e is ThreadInterruptedException)
                    {
                        foreach (WindowTimer tw in _timerWindows) Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => tw.Close()));
                        _timerWindows.Clear();
                    }
                }
                finally
                {
                    Stop();
                }
            });
            _thread.Start();
        }

        public static void Stop()
        {
            if (_thread != null)
            {
                _thread.Interrupt();
            }
        }

        private static void LoadPokerTypesFromRegistry()
        {
            PokerTypes.Clear();

            // load Poker Types from registry
            using (RegistryKey keyPokerTypes = Registry.CurrentUser.OpenSubKey(@"Software\PSHandler\PokerTypes", true))
            {
                foreach (string valueName in keyPokerTypes.GetValueNames())
                {
                    PokerType pokerType = PokerType.FromXml(keyPokerTypes.GetValue(valueName) as string);
                    if (pokerType != null)
                    {
                        PokerTypes.Add(pokerType);
                    }
                }
            }
        }

        private static void UpdateTournamentInfos()
        {
            DirectoryInfo dirPs = new DirectoryInfo(Config.AppDataPath);
            if (!dirPs.Exists) return;
            DirectoryInfo dirHH = dirPs.GetDirectories().FirstOrDefault(di => di.Name.Equals("HandHistory"));
            if (dirHH == null || !dirHH.Exists) return;

            foreach (var dirPlayers in dirHH.GetDirectories())
            {
                FileInfo[] fis = dirPlayers.GetFiles();
                foreach (FileInfo fi in fis)
                {
                    Match matchFileName = _RegexFileName.Match(fi.Name);
                    if (matchFileName.Success)
                    {
                        long tournamentNumber = long.Parse(matchFileName.Groups["tn"].Value);

                        TournamentInfo ti;
                        lock (_lock)
                        {
                            ti = _tournamentInfos.FirstOrDefault(o => o.TournamentNumber.Equals(tournamentNumber));
                        }
                        if (ti == null)
                        {
                            lock (_lock)
                            {
                                _tournamentInfos.Add(new TournamentInfo { TournamentNumber = tournamentNumber, FileInfo = fi, Hands = Hand.Parse(File.ReadAllText(fi.FullName)), LastLength = fi.Length });
                            }
                        }
                        else
                        {
                            ti.UpdateHands();
                        }
                    }
                }
            }
        }

        public static TournamentInfo GetTournamentInfo(long tournamentNumber)
        {
            lock (_lock)
            {
                return _tournamentInfos.FirstOrDefault(ti => ti.TournamentNumber == tournamentNumber);
            }
        }

        public static PokerType FindPokerType(string title, string fileName, out int errorFlags)
        {
            List<PokerType> possiblePokerTypes = new List<PokerType>();
            foreach (PokerType pokerType in PokerTypes)
            {
                var IncludeAnd = pokerType.IncludeAnd.Length == 0 || pokerType.IncludeAnd.All(title.Contains);
                var IncludeOr = pokerType.IncludeOr.Length == 0 || pokerType.IncludeOr.Any(title.Contains);
                var ExcludeAnd = pokerType.ExcludeAnd.Length == 0 || !pokerType.ExcludeAnd.All(title.Contains);
                var ExcludeOr = pokerType.ExcludeOr.Length == 0 || !pokerType.ExcludeOr.Any(title.Contains);
                var BuyInAndRake = pokerType.BuyInAndRake.Length == 0 || pokerType.BuyInAndRake.Any(fileName.Contains);
                if (IncludeAnd && IncludeOr && ExcludeAnd && ExcludeOr && BuyInAndRake)
                {
                    possiblePokerTypes.Add(pokerType);
                }
            }
            if (possiblePokerTypes.Count == 1)
            {
                errorFlags = 0; // OK
                return possiblePokerTypes[0];
            }
            if (possiblePokerTypes.Count == 0)
            {
                errorFlags = 1; // Not found
                return null;
            }
            if (possiblePokerTypes.Count > 1)
            {
                errorFlags = 2; // More than one PokerType found
                return null;
            }
            errorFlags = 3; // Unknown error
            return null;
        }
    }
}

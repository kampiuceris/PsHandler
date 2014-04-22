using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;

namespace PsHandler.Hud
{
    public class HudManager
    {
        private static readonly List<WindowTimer> TimerWindows = new List<WindowTimer>();
        private static readonly object _lock = new object();
        private static readonly List<TournamentInfo> TournamentInfos = new List<TournamentInfo>();
        private static Thread _thread;
        private static Regex RegexFileName = new Regex(@"HH\d{8} T(?<tn>\d{9,10})");
        private static Regex RegexTimestamp = new Regex(@".+ - (?<timestamp>\d{4}.\d{2}.\d{2} \d{1,2}:\d{2}:\d{2})");

        public static void Start()
        {
            Stop();
            _thread = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        foreach (var process in Process.GetProcessesByName("PokerStars"))
                        {
                            foreach (IntPtr handle in WinApi.EnumerateProcessWindowHandles(process.Id))
                            {
                                string className = WinApi.GetClassName(handle);
                                if (className.Equals("PokerStarsTableFrameClass"))
                                {
                                    WindowTimer find = TimerWindows.FirstOrDefault(tw => tw.HandleOwner.Equals(handle));

                                    if (find == null)
                                    {
                                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                                        {
                                            // new window
                                            WindowTimer wt = new WindowTimer();
                                            wt.Show();
                                            wt.SetOwner(handle);
                                            TimerWindows.Add(wt);
                                        }));
                                    }
                                }
                            }
                        }

                        // clean closed windows
                        foreach (WindowTimer tw in TimerWindows.Where(tw => !WinApi.IsWindow(tw.Handle)).ToList()) TimerWindows.Remove(tw);

                        // update tournament infos list
                        UpdateTournamentInfos();

                        //Debug.WriteLine(TimerWindows.Count + " " + TournamentInfos.Count);
                        Thread.Sleep(2000);
                    }
                }
                catch (Exception e)
                {
                    if (e is ThreadInterruptedException)
                    {
                        foreach (WindowTimer tw in TimerWindows) Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => tw.Close()));
                        TimerWindows.Clear();
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
            if (_thread != null) _thread.Interrupt();
        }

        private static void UpdateTournamentInfos()
        {
            //DirectoryInfo di = new DirectoryInfo(@"C:\Users\Martin\AppData\Local\PokerStars.EU\HandHistory\kampiuceris");
            DirectoryInfo dirPs = new DirectoryInfo(App.AppDataPath);
            if (!dirPs.Exists) return;
            DirectoryInfo dirHH = dirPs.GetDirectories().FirstOrDefault(di => di.Name.Equals("HandHistory"));
            if (dirHH == null || !dirHH.Exists) return;

            foreach (var dirPlayers in dirHH.GetDirectories())
            {
                FileInfo[] fis = dirPlayers.GetFiles();
                foreach (FileInfo fi in fis)
                {
                    Match matchFileName = RegexFileName.Match(fi.Name);
                    if (matchFileName.Success)
                    {
                        long tournamentNumber = long.Parse(matchFileName.Groups["tn"].Value);

                        bool any;
                        lock (_lock) any = TournamentInfos.Any(ti => ti.TournamentNumber.Equals(tournamentNumber));
                        if (!any)
                        {
                            // get date
                            string line;
                            using (StreamReader reader = new StreamReader(fi.FullName)) line = reader.ReadLine(); // read first line
                            Match matchTimestamp = RegexTimestamp.Match(line);
                            if (matchTimestamp.Success)
                            {
                                DateTime timestamp = DateTime.Parse(matchTimestamp.Groups["timestamp"].Value);
                                lock (_lock) TournamentInfos.Add(new TournamentInfo { TournamentNumber = tournamentNumber, TimestampStarted = timestamp });
                            }
                        }
                    }
                }
            }
        }

        public static TournamentInfo GetTournamentInfo(long tournamentNumber)
        {
            lock (_lock) return TournamentInfos.FirstOrDefault(ti => ti.TournamentNumber == tournamentNumber);
        }
    }

    public class TournamentInfo
    {
        public long TournamentNumber;
        public DateTime TimestampStarted;
    }
}

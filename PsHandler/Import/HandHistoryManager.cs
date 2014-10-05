using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using PsHandler.Custom;

namespace PsHandler.Import
{
    public interface IObserverHandHistoryManager
    {
        void SetImportedTournaments(int value);
        void SetImportedHands(int value);
        void SetImportedErrors(int value);
    }

    public class HandHistoryManager
    {
        private readonly Regex _RegexFileName = new Regex(@"HH\d{8} T(?<tn>\d{9,10})");
        private readonly object _lock = new object();
        private readonly List<Tournament> _tournaments = new List<Tournament>();
        private int _importErrors;
        private Thread _thread;
        public IObserverHandHistoryManager Observer;

        public HandHistoryManager()
        {
            Start();
        }

        public void Start()
        {
            Stop();

            _thread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        Thread.Sleep(3000);
                        UpdateTournaments();
                    }
                    catch (Exception e)
                    {
                        if (e is ThreadInterruptedException)
                        {
                            break;
                        }
                    }
                }
            });
            _thread.Start();
        }

        public void Stop()
        {
            if (_thread != null)
            {
                _thread.Interrupt();
                _tournaments.Clear();
            }
        }

        private void UpdateTournaments()
        {
            string[] importFolderPaths = Config.ImportFolders.ToArray();

            foreach (string importFolderPath in importFolderPaths)
            {
                if (String.IsNullOrEmpty(importFolderPath)) continue;

                DirectoryInfo dir = new DirectoryInfo(importFolderPath);

                if (!dir.Exists) continue;

                foreach (FileInfo fi in dir.GetFiles().Where(o => o.Name.EndsWith(".txt")))
                {
                    Match matchFileName = _RegexFileName.Match(fi.Name);
                    if (matchFileName.Success)
                    {
                        long tournamentNumber = long.Parse(matchFileName.Groups["tn"].Value);

                        Tournament tournament;
                        lock (_lock)
                        {
                            tournament = _tournaments.FirstOrDefault(o => o.TournamentNumber.Equals(tournamentNumber));
                        }
                        if (tournament == null)
                        {
                            lock (_lock)
                            {
                                int importErrors;
                                List<Hand> hands = Hand.Parse(File.ReadAllText(fi.FullName), out importErrors); // import time 
                                _importErrors += importErrors;
                                _tournaments.Add(new Tournament { TournamentNumber = tournamentNumber, FileInfo = fi, Hands = hands, LastLength = fi.Length });
                            }
                        }
                        else
                        {
                            UpdateHands(tournament);
                        }
                    }
                    if (Observer != null)
                    {
                        Observer.SetImportedTournaments(_tournaments.Count);
                        Observer.SetImportedHands(_tournaments.Sum(o => o.Hands.Count));
                        Observer.SetImportedErrors(_importErrors);
                    }
                }
            }
        }

        private void UpdateHands(Tournament tournament)
        {
            tournament.FileInfo = new FileInfo(tournament.FileInfo.FullName);
            if (tournament.FileInfo.Length > tournament.LastLength)
            {
                int importErrors;
                List<Hand> hands = Hand.Parse(Methods.ReadSeek(tournament.FileInfo.FullName, tournament.LastLength), out importErrors);
                _importErrors += importErrors;
                tournament.LastLength = tournament.FileInfo.Length;
                tournament.AddHands(hands);
            }
        }

        public Tournament GetTournament(long tournamentNumber)
        {
            lock (_lock)
            {
                return _tournaments.FirstOrDefault(ti => ti.TournamentNumber == tournamentNumber);
            }
        }
    }
}

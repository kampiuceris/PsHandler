using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace PsHandler.Hud.Import
{
    public class Import
    {
        private static readonly Regex _RegexFileName = new Regex(@"HH\d{8} T(?<tn>\d{9,10})");
        //private static readonly Regex _RegexTimestamp = new Regex(@".+ - (?<timestamp>\d{4}.\d{2}.\d{2} \d{1,2}:\d{2}:\d{2})");
        private static readonly object _lock = new object();
        private static readonly List<Tournament> _tournaments = new List<Tournament>();
        private static int _importErrors;
        private static Thread _thread;

        public Import()
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
                        UpdateTournaments();
                    }
                    catch
                    {
                    }
                    Thread.Sleep(3000);
                }
            });
            _thread.Start();
        }

        public void Stop()
        {
            if (_thread != null)
            {
                _thread.Abort();
                _tournaments.Clear();
            }
        }

        public Tournament GetTournament(long tournamentNumber)
        {
            lock (_lock)
            {
                return _tournaments.FirstOrDefault(ti => ti.TournamentNumber == tournamentNumber);
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

                foreach (FileInfo fi in dir.GetFiles())
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
                                App.WindowMain.Importing = true;
                                int importErrors;
                                List<Hand> hands = Hand.Parse(File.ReadAllText(fi.FullName), out importErrors); // import time 
                                _importErrors += importErrors;
                                App.WindowMain.Errors = _importErrors;
                                _tournaments.Add(new Tournament { TournamentNumber = tournamentNumber, FileInfo = fi, Hands = hands, LastLength = fi.Length });
                                App.WindowMain.Importing = false;
                            }
                        }
                        else
                        {
                            UpdateHands(tournament);
                        }
                        App.WindowMain.Tournaments = _tournaments.Count;
                        App.WindowMain.Hands = _tournaments.Sum(o => o.Hands.Count);
                    }
                }
            }
        }

        public void UpdateHands(Tournament tournament)
        {
            tournament.FileInfo = new FileInfo(tournament.FileInfo.FullName);
            if (tournament.FileInfo.Length > tournament.LastLength)
            {
                App.WindowMain.Importing = true;
                int importErrors;
                List<Hand> hands = Hand.Parse(Methods.ReadSeek(tournament.FileInfo.FullName, tournament.LastLength), out importErrors);
                _importErrors += importErrors;
                App.WindowMain.Errors = _importErrors;
                tournament.LastLength = tournament.FileInfo.Length;
                tournament.AddHands(hands);
                App.WindowMain.Importing = false;
            }
        }
    }
}

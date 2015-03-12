// PsHandler - poker software helping tool.
// Copyright (C) 2014-2015  kampiuceris

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using PsHandler.Custom;
using PsHandler.PokerMath;
using PsHandler.UI;

namespace PsHandler.Import
{
    public interface IObserverHandHistoryManager
    {
        void SetImportedTournaments(int value);
        void SetImportedHands(int value);
        void SetImportedErrors(int value);
    }

    public interface IObservableCollectionHand
    {
        void AddHand(Hand hand);
    }

    public class HandHistoryManager
    {
        private readonly Regex _RegexFileName = new Regex(@"\AHH\d{8}.*No Limit Hold'em.*\.txt\z");
        private readonly object _lock = new object();
        private readonly Dictionary<string, long> _fileNameBytesRead = new Dictionary<string, long>();
        private readonly Dictionary<long, Hand> _dictionaryCashHands = new Dictionary<long, Hand>();
        private readonly Dictionary<long, Tournament> _dictionaryTournaments = new Dictionary<long, Tournament>();
        private int _importErrors;
        private Thread _thread;
        public IObserverHandHistoryManager ObserverStatus;
        public IObservableCollectionHand ObserverHands;

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
                        Import();
                        Thread.Sleep(3000);
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
                _dictionaryCashHands.Clear();
                _dictionaryTournaments.Clear();
            }
        }

        public Tournament GetTournament(long tournamentNumber)
        {
            lock (_lock)
            {
                if (_dictionaryTournaments.ContainsKey(tournamentNumber))
                {
                    return _dictionaryTournaments[tournamentNumber];
                }
                else
                {
                    return null;
                }
            }
        }

        private void Import()
        {
            UpdateFileList();
            ReadFiles();
        }

        private void UpdateFileList()
        {
            foreach (string importFolderPath in Config.ImportFolders.ToArray().Where(a => !String.IsNullOrEmpty(a)))
            {
                DirectoryInfo di = new DirectoryInfo(importFolderPath);
                if (!di.Exists) continue;

                foreach (FileInfo fi in di.GetFiles())
                {
                    Match matchFileName = _RegexFileName.Match(fi.Name);
                    if (matchFileName.Success)
                    {
                        if (!_fileNameBytesRead.ContainsKey(fi.FullName))
                        {
                            _fileNameBytesRead.Add(fi.FullName, 0);
                        }
                    }
                }
            }
        }

        private void ReadFiles()
        {
            var toRemove = new List<string>();
            var toUpdate = new Dictionary<string, long>();

            foreach (var fileNameBytesRead in _fileNameBytesRead)
            {
                var fi = new FileInfo(fileNameBytesRead.Key);
                if (!fi.Exists)
                {
                    // add ro remove list
                    toRemove.Add(fileNameBytesRead.Key);
                }
                else if (fi.Length > fileNameBytesRead.Value)
                {
                    // needs to read
                    var pokerData = PokerData.FromText(Methods.ReadSeek(fi.FullName, fileNameBytesRead.Value));
                    var hands = pokerData.PokerHands.Select(pokerHand => Hand.Parse(pokerHand.HandHistory)).Where(hand => hand != null).ToList();
                    AddHands(hands);

                    toUpdate.Add(fi.FullName, fi.Length);
                    _importErrors += pokerData.ErrorHandHistories.Count;
                }

                UpdateUI();
            }

            // remove non-existing files
            foreach (var fileName in toRemove)
            {
                _fileNameBytesRead.Remove(fileName);
            }

            // update existing
            foreach (var pair in toUpdate)
            {
                _fileNameBytesRead[pair.Key] = pair.Value;
            }
        }

        private void AddHands(IEnumerable<Hand> hands)
        {
            foreach (var hand in hands)
            {
                lock (_lock)
                {
                    if (!hand.IsTournament)
                    {
                        // cash hand
                        if (!_dictionaryCashHands.ContainsKey(hand.HandNumber))
                        {
                            _dictionaryCashHands.Add(hand.HandNumber, hand);
                        }
                    }
                    else
                    {
                        // tournament hand
                        if (!_dictionaryTournaments.ContainsKey(hand.TournamentNumber))
                        {
                            _dictionaryTournaments.Add(hand.TournamentNumber, new Tournament { TournamentNumber = hand.TournamentNumber });
                        }
                        _dictionaryTournaments[hand.TournamentNumber].AddHand(hand);
                    }
                }

                if (ObserverHands != null)
                {
                    ObserverHands.AddHand(hand);
                }
            }
        }

        private void UpdateUI()
        {
            if (ObserverStatus != null)
            {
                ObserverStatus.SetImportedTournaments(_dictionaryTournaments.Count);
                ObserverStatus.SetImportedHands(_dictionaryTournaments.Sum(a => a.Value.Hands.Count) + _dictionaryCashHands.Count);
                ObserverStatus.SetImportedErrors(_importErrors);
            }
        }
    }
}

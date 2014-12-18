// PsHandler - poker software helping tool.
// Copyright (C) 2014  kampiuceris

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
using System.Text;
using System.Text.RegularExpressions;

namespace PsHandler.Import
{
    public class Tournament
    {
        public long TournamentNumber;
        public FileInfo FileInfo;
        public long LastLength;
        public List<Hand> Hands = new List<Hand>();
        private readonly object _lock = new object();

        public DateTime GetFirstHandTimestampUtc()
        {
            lock (_lock)
            {
                return Hands[0].TimestampUtc;
            }
        }

        public DateTime GetFirstHandTimestamp()
        {
            lock (_lock)
            {
                return Hands[0].Timestamp;
            }
        }

        public TimeZone GetFirstHandTimeZone()
        {
            lock (_lock)
            {
                return Hands[0].TimeZone;
            }
        }

        public DateTime GetLastHandTimestamp()
        {
            lock (_lock)
            {
                return Hands[Hands.Count - 1].Timestamp;
            }
        }

        public decimal GetLatestStack(string name)
        {
            if (string.IsNullOrEmpty(name)) return decimal.MinValue;

            lock (_lock)
            {
                Player firstOrDefault = Hands[Hands.Count - 1].Players.FirstOrDefault(o => o.Name.Equals(name));
                if (firstOrDefault != null)
                {
                    return firstOrDefault.Stack;
                }
                return decimal.MinValue;
            }
        }

        public TableSize GetLastHandTableSize()
        {
            lock (_lock)
            {
                return !Hands.Any() ? TableSize.Default : Hands.Last().TableSize;
            }
        }

        public int GetLastHandPlayerCount()
        {
            lock (_lock)
            {
                return !Hands.Any() ? 0 : Hands.Last().Players.Length;
            }
        }

        public int GetLastHandPlayerCountAfterHand()
        {
            lock (_lock)
            {
                return !Hands.Any() ? 0 : Hands.Last().PlayersAfterHand.Length;
            }
        }

        //

        public void AddHands(IEnumerable<Hand> hands)
        {
            lock (_lock)
            {
                Hands.AddRange(hands);
                Hands.Sort((o1, o2) => DateTime.Compare(o1.Timestamp, o2.Timestamp));
            }
        }

        public int CountLevelHands(decimal smallBlind, decimal bigBlind)
        {
            lock (_lock)
            {
                return Hands.Count(o => o.Level.SmallBlind == smallBlind && o.Level.BigBlind == bigBlind);
            }
        }

        public override string ToString()
        {
            return string.Format("Tournament: {0}, Hands: {1}", TournamentNumber, Hands.Count);
        }
    }
}

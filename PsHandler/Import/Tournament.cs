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
using System.IO;
using System.Linq;
using PsHandler.PokerMath;

namespace PsHandler.Import
{
    public class Tournament
    {
        public long TournamentNumber;
        public List<Hand> Hands = new List<Hand>();
        private readonly object _lock = new object();

        public DateTime GetFirstHandTimestampET()
        {
            lock (_lock)
            {
                return !Hands.Any() ? DateTime.MinValue : Hands.First().TimeStampET;
            }
        }

        public decimal GetLatestStack(string name)
        {
            if (string.IsNullOrEmpty(name)) return decimal.MinValue;

            lock (_lock)
            {
                if (!Hands.Any()) return decimal.MinValue;
                int index = Array.IndexOf(Hands.Last().PlayerNames, name);
                if (index < 0) return decimal.MinValue;
                return Hands.Last().StacksAfterHand[index];
            }
        }

        public PokerEnums.TableSize GetLastHandTableSize()
        {
            lock (_lock)
            {
                return !Hands.Any() ? PokerEnums.TableSize.Default : Hands.Last().TableSize;
            }
        }

        public int CountLevelHands(decimal smallBlind, decimal bigBlind)
        {
            lock (_lock)
            {
                return Hands.Count(o => o.Level.SmallBlind == smallBlind && o.Level.BigBlind == bigBlind);
            }
        }

        public string[] GetLastHandHudPlayerNames()
        {
            lock (_lock)
            {
                return !Hands.Any() ? null : Hands.Last().HudPlayerNames;
            }
        }

        public decimal[] GetLastHandHudPlayerStacks()
        {
            lock (_lock)
            {
                return !Hands.Any() ? null : Hands.Last().HudPlayerStacks;
            }
        }

        public int GetLastHandHudHeroSeat()
        {
            lock (_lock)
            {
                return !Hands.Any() ? 0 : Hands.Last().HudHeroSeat;
            }
        }

        //

        public void AddHand(Hand hand)
        {
            lock (_lock)
            {
                Hands.Add(hand);
                Hands.Sort((o1, o2) => DateTime.Compare(o1.TimeStampET, o2.TimeStampET));
            }
        }

        public void AddHands(IEnumerable<Hand> hands)
        {
            lock (_lock)
            {
                Hands.AddRange(hands);
                Hands.Sort((o1, o2) => DateTime.Compare(o1.TimeStampET, o2.TimeStampET));
            }
        }

        //

        public override string ToString()
        {
            return string.Format("Tournament: {0}, Hands: {1}", TournamentNumber, Hands.Count);
        }
    }
}

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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Shapes;
using PsHandler.PokerMath;

namespace PsHandler.Import
{
    // Level

    public class Level
    {
        public decimal Ante;
        public decimal SmallBlind;
        public decimal BigBlind;
        public bool IsAnteDefined;
    }

    // Hand

    public class Hand : PokerHand
    {
        public Level Level;
        public string[] PlayerNames;
        public decimal[] StacksAfterHand;

        public Hand(string handHistoryText)
            : base(handHistoryText)
        {
            Level = new Level { SmallBlind = LevelSmallBlind, BigBlind = LevelBigBlind, Ante = LevelAnte, IsAnteDefined = true };
            var table = new PokerMath.Table();
            table.LoadHand(this);
            table.ToDoCommandsAll();
            PlayerNames = new string[table.PlayerCount];
            StacksAfterHand = new decimal[table.PlayerCount];
            for (int i = 0; i < table.PlayerCount; i++)
            {
                PlayerNames[i] = table.Players[i].PlayerName;
                StacksAfterHand[i] = table.Players[i].Stack;
            }
        }

        public new static Hand Parse(string handHistoryText)
        {
            try { return new Hand(handHistoryText); }
            catch { return null; }
        }

        public static List<Hand> ___Parse(string text, out int importErrors)
        {
            importErrors = 0;
            var hands = new List<Hand>();

            string[] handHistories, tournamentSummaries;
            GetHandHistoriesTournamentSummaries(text, out handHistories, out tournamentSummaries);
            foreach (var handHistory in handHistories)
            {
                var hand = Parse(handHistory);
                if (hand != null)
                {
                    hands.Add(hand);
                }
                else
                {
                    importErrors++;
                }
            }
            return hands;
        }

        private static void GetHandHistoriesTournamentSummaries(string text, out string[] handHistories, out string[] tournamentSummaries)
        {
            var lines = text.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            while (lines.Any() && !lines[0].StartsWith("PokerStars Hand #") && !lines[0].StartsWith("PokerStars Zoom Hand #") && !lines[0].StartsWith("PokerStars Tournament #"))
            {
                lines.RemoveAt(0);
            }

            List<string> hhs = new List<string>();
            List<string> tss = new List<string>();
            StringBuilder sb = new StringBuilder();
            bool isPokerHand = false, isTournamentSummary = false;
            foreach (var line in lines)
            {
                if (line.StartsWith("PokerStars Hand #") || line.StartsWith("PokerStars Zoom Hand #"))
                {
                    if (sb.Length > 0)
                    {
                        if (isPokerHand) hhs.Add(sb.ToString());
                        if (isTournamentSummary) tss.Add(sb.ToString());
                    }
                    sb.Clear();

                    isPokerHand = true;
                    isTournamentSummary = false;
                }
                else if (line.StartsWith("PokerStars Tournament #"))
                {
                    if (sb.Length > 0)
                    {
                        if (isPokerHand) hhs.Add(sb.ToString());
                        if (isTournamentSummary) tss.Add(sb.ToString());
                    }
                    sb.Clear();

                    isPokerHand = false;
                    isTournamentSummary = true;
                }

                sb.AppendLine(line);
            }
            if (sb.Length > 0)
            {
                if (isPokerHand) hhs.Add(sb.ToString());
                if (isTournamentSummary) tss.Add(sb.ToString());
            }

            handHistories = hhs.ToArray();
            tournamentSummaries = tss.ToArray();
        }

    }
}

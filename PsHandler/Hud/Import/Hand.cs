﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PsHandler.Hud.Import
{
    // SeatMax

    public enum TableSize
    {
        Default,
        Max10,
        Max9,
        Max8,
        Max7,
        Max6,
        Max4,
        Max2,
    }

    // Player

    public class Player
    {
        public string Name;
        public decimal Stack;
        public decimal Bet;

        public override string ToString()
        {
            return string.Format("{0} {1} ({2})", Name, Stack, Bet);
        }
    }

    // Hand

    public class Hand
    {
        public long HandNumber;
        public long TournamentNumber;
        public DateTime Timestamp;
        public Player[] Players;
        public TableSize TableSize;
        public short ButtonSeat;

        private readonly List<Player> _players = new List<Player>();
        private bool _smallBlindCollectPots;

        public static List<Hand> Parse(string text, out int importErrors)
        {
            List<Hand> hands = new List<Hand>();

            string[] handsText = text.Split(new[] { "PokerStars Ha" }, StringSplitOptions.RemoveEmptyEntries);
            List<string> handHistories = new List<string>();
            for (int i = 0; i < handsText.Length; i++)
            {
                if (handsText[i].StartsWith("nd #"))
                {
                    handHistories.Add("PokerStars Ha" + handsText[i]);
                }
            }

            importErrors = 0;

            foreach (string ht in handHistories)
            {
                try
                {
                    string[] lines = ht.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    Hand hand = new Hand();

                    foreach (string line in lines)
                    {
                        AnalyzeLine(line, ref hand);
                    }

                    // finalize hand
                    hand.Players = hand._players.ToArray();

                    hands.Add(hand);
                }
                catch (Exception)
                {
                    importErrors++;
                }
            }

            return hands;
        }

        public override string ToString()
        {
            return string.Format("Hand: {0}, [{1}], Players: {2}", HandNumber, Timestamp, Players.Length);
        }

        //

        public static Regex RegexHeader = new Regex(@"\APokerStars Hand #(?<hand_id>\d+): Tournament #(?<tournament_id>\d+),.+- (?<year>\d\d\d\d).(?<month>\d\d).(?<day>\d\d) (?<hour>\d{1,2}):(?<minute>\d{1,2}):(?<second>\d{1,2}) (?<timezone>.+) \[.+");
        public static Regex RegexSeatMaxButton = new Regex(@"\ATable '\d+ \d+' (?<table_size>\d+)-max Seat #(?<button_seat>\d+) is the button");
        public static Regex RegexSeat = new Regex(@"Seat \d{1,2}: (?<name>.+) \((?<stack>\d+) in chips\)");
        public static Regex RegexAnte = new Regex(@"(?<name>.+): posts the ante (?<amount>\d+)");
        public static Regex RegexSmallBlind = new Regex(@"(?<name>.+): posts small blind (?<amount>\d+)");
        public static Regex RegexBigBlind = new Regex(@"(?<name>.+): posts big blind (?<amount>\d+)");
        public static Regex RegexFlop = new Regex(@"\*\*\* FLOP \*\*\*");
        public static Regex RegexTurn = new Regex(@"\*\*\* TURN \*\*\*");
        public static Regex RegexRiver = new Regex(@"\*\*\* RIVER \*\*\*");
        public static Regex RegexBets = new Regex(@"(?<name>.+): bets (?<amount>\d+)");
        public static Regex RegexCalls = new Regex(@"(?<name>.+): calls (?<amount>\d+)");
        public static Regex RegexRaises = new Regex(@"(?<name>.+): raises (?<amount>\d+) to (?<amount_total>\d+)");
        public static Regex RegexUncalledAmount = new Regex(@"Uncalled bet \((?<amount>\d+)\) returned to (?<name>.+)");
        public static Regex RegexCollectedFromPot = new Regex(@"(?<name>.+) collected (?<amount>\d+) from.+pot");

        private static void AnalyzeLine(string line, ref Hand hand)
        {
            Match match;

            match = RegexHeader.Match(line);
            if (match.Success)
            {
                hand.HandNumber = long.Parse(match.Groups["hand_id"].Value);
                hand.TournamentNumber = long.Parse(match.Groups["tournament_id"].Value);
                int year = int.Parse(match.Groups["year"].Value);
                int month = int.Parse(match.Groups["month"].Value);
                int day = int.Parse(match.Groups["day"].Value);
                int hour = int.Parse(match.Groups["hour"].Value);
                int minute = int.Parse(match.Groups["minute"].Value);
                int second = int.Parse(match.Groups["second"].Value);
                hand.Timestamp = new DateTime(year, month, day, hour, minute, second);
                return;
            }

            match = RegexSeatMaxButton.Match(line);
            if (match.Success)
            {
                hand.TableSize = SeatToTableSize(int.Parse(match.Groups["table_size"].Value));
                hand.ButtonSeat = short.Parse(match.Groups["button_seat"].Value);
                return;
            }

            match = RegexSeat.Match(line);
            if (match.Success)
            {
                hand._players.Add(new Player { Name = match.Groups["name"].Value, Stack = decimal.Parse(match.Groups["stack"].Value) });
                return;
            }

            match = RegexAnte.Match(line);
            if (match.Success)
            {
                Player player = hand._players.First(p => p.Name.Equals(match.Groups["name"].Value));
                decimal amount = decimal.Parse(match.Groups["amount"].Value);
                player.Stack -= amount;
                player.Bet += amount;
                return;
            }

            match = RegexSmallBlind.Match(line);
            if (match.Success)
            {
                hand.CollectBets();
                hand._smallBlindCollectPots = true;
                Player player = hand._players.First(p => p.Name.Equals(match.Groups["name"].Value));
                decimal amount = decimal.Parse(match.Groups["amount"].Value);
                player.Stack -= amount;
                player.Bet += amount;
                return;
            }

            match = RegexBigBlind.Match(line);
            if (match.Success)
            {
                if (!hand._smallBlindCollectPots) hand.CollectBets();
                Player player = hand._players.First(p => p.Name.Equals(match.Groups["name"].Value));
                decimal amount = decimal.Parse(match.Groups["amount"].Value);
                player.Stack -= amount;
                player.Bet += amount;
                return;
            }

            match = RegexFlop.Match(line);
            if (match.Success)
            {
                hand.CollectBets();
                return;
            }

            match = RegexTurn.Match(line);
            if (match.Success)
            {
                hand.CollectBets();
                return;
            }

            match = RegexRiver.Match(line);
            if (match.Success)
            {
                hand.CollectBets();
                return;
            }

            match = RegexBets.Match(line);
            if (match.Success)
            {
                Player player = hand._players.First(p => p.Name.Equals(match.Groups["name"].Value));
                decimal amount = decimal.Parse(match.Groups["amount"].Value);
                player.Stack -= amount;
                player.Bet += amount;
                return;
            }

            match = RegexCalls.Match(line);
            if (match.Success)
            {
                Player player = hand._players.First(p => p.Name.Equals(match.Groups["name"].Value));
                decimal amount = decimal.Parse(match.Groups["amount"].Value);
                player.Stack -= amount;
                player.Bet += amount;
                return;
            }

            match = RegexRaises.Match(line);
            if (match.Success)
            {
                Player player = hand._players.First(p => p.Name.Equals(match.Groups["name"].Value));
                //decimal amount = decimal.Parse(match.Groups["amount"].Value);
                decimal amountTotal = decimal.Parse(match.Groups["amount_total"].Value);

                decimal amountReal = amountTotal - player.Bet;
                player.Bet += amountReal;
                player.Stack -= amountReal;
                return;
            }


            match = RegexUncalledAmount.Match(line);
            if (match.Success)
            {
                Player player = hand._players.First(p => p.Name.Equals(match.Groups["name"].Value));
                decimal amount = decimal.Parse(match.Groups["amount"].Value);
                player.Stack += amount;
                player.Bet -= amount;
                return;
            }

            match = RegexCollectedFromPot.Match(line);
            if (match.Success)
            {
                Player player = hand._players.First(p => p.Name.Equals(match.Groups["name"].Value));
                decimal amount = decimal.Parse(match.Groups["amount"].Value);
                player.Stack += amount;
                return;
            }
        }

        public void CollectBets()
        {
            foreach (var player in _players)
                player.Bet = 0;
        }

        public static TableSize SeatToTableSize(int tableSize)
        {
            switch (tableSize)
            {
                case 10:
                return TableSize.Max10;
                case 9:
                return TableSize.Max9;
                case 8:
                return TableSize.Max8;
                case 7:
                return TableSize.Max7;
                case 6:
                return TableSize.Max6;
                case 4:
                return TableSize.Max4;
                case 2:
                return TableSize.Max2;
                default:
                return TableSize.Default;
            }
        }
    }
}

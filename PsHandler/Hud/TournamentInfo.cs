using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PsHandler.Hud
{
    public class TournamentInfo
    {
        public long TournamentNumber;
        public FileInfo FileInfo;
        public long LastLength;
        public List<Hand> Hands = new List<Hand>();
        private readonly object _lock = new object();

        public void AddHands(IEnumerable<Hand> hands)
        {
            lock (_lock)
            {
                Hands.AddRange(hands);
            }
        }

        public DateTime FirstHandTimestamp()
        {
            lock (_lock)
            {
                return Hands[0].Timestamp;
            }
        }

        public DateTime LastHandTimestamp()
        {
            lock (_lock)
            {
                return Hands[Hands.Count - 1].Timestamp;
            }
        }

        public decimal LatestStack(string name)
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

        public void UpdateHands()
        {
            FileInfo = new FileInfo(FileInfo.FullName);
            if (FileInfo.Length > LastLength)
            {
                string text = ReadSeek(FileInfo.FullName, LastLength);
                LastLength = FileInfo.Length;
                AddHands(Hand.Parse(text));
            }
        }

        private static string ReadSeek(string path, long seek)
        {
            string text = "";
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                fs.Seek(seek, SeekOrigin.Begin);
                byte[] b = new byte[fs.Length - seek];
                fs.Read(b, 0, (int)(fs.Length - seek));
                text = System.Text.Encoding.UTF8.GetString(b);
            }
            return text;
        }
    }

    public class Hand
    {
        public long HandNumber;
        public long TournamentNumber;
        public DateTime Timestamp;
        public Player[] Players;

        private readonly List<Player> _players = new List<Player>();
        private bool _smallBlindCollectPots;

        public static List<Hand> Parse(string text)
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
                }
            }

            return hands;
        }

        //

        public static Regex RegexHeader = new Regex(@"PokerStars Hand #(?<hand_id>\d+): Tournament #(?<tournament_id>\d+),.+- (?<year>\d\d\d\d).(?<month>\d\d).(?<day>\d\d) (?<hour>\d\d):(?<minute>\d\d):(?<second>\d\d) (?<timezone>.+) \[.+");
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
    }

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
}

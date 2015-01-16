using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PsHandler.Replayer
{
    public class PokerEnums
    {
        public enum PokerSite { Unknown, PokerStars_unknown, PokerStars_com, PokerStars_eu, PokerStars_fr, FullTiltPoker_unknown, FullTiltPoker_com, FullTiltPoker_eu, FullTiltPoker_fr, };
        public enum Currency { Unknown, PlayMoney, Freeroll, FPP, USD, EUR, GBP, };
        public enum Position { EP, MP, CO, BU, SB, BB, };
    }

    public class PokerData
    {
        public List<PokerHand> PokerHands = new List<PokerHand>();
        public List<PokerTournamentSummary> PokerTournamentSummaries = new List<PokerTournamentSummary>();
        public List<string> ErrorHandHistories = new List<string>();
        public List<string> ErrorTournamentSummaries = new List<string>();

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

        public static PokerData FromText(string text)
        {
            var pokerData = new PokerData();

            string[] hhs, tss;
            GetHandHistoriesTournamentSummaries(text, out hhs, out tss);

            foreach (var hh in hhs)
            {
                var pokerHand = PokerHand.FromHandHistory(hh);
                if (pokerHand != null)
                {
                    pokerData.PokerHands.Add(pokerHand);
                }
                else
                {
                    pokerData.ErrorHandHistories.Add(hh);
                }
            }

            foreach (var ts in tss)
            {
                var pokerTournamentSummary = PokerTournamentSummary.FromTournamentSummary(ts);
                if (pokerTournamentSummary != null)
                {
                    pokerData.PokerTournamentSummaries.Add(pokerTournamentSummary);
                }
                else
                {
                    pokerData.ErrorTournamentSummaries.Add(ts);
                }
            }

            return pokerData;
        }
    }

    public class PokerTournamentSummary
    {
        public class Player
        {
            public int Place;
            public string PlayerName;
            public string Country;
            public PokerEnums.Currency CurrencyPrize;
            public decimal Prize;
            public bool QualifiedForTournament;
            public bool StillPlaying;
        }

        public string TournamentSummary;
        public long TournamentNumber;
        public string GameType;
        public bool IsSatellite;
        public PokerEnums.Currency CurrencyTargetBuyIn;
        public decimal TargetTournamentBuyIn;
        public PokerEnums.Currency CurrencyBuyIn = PokerEnums.Currency.Unknown;
        public decimal TotalBuyIn;
        public decimal BuyIn;
        public decimal Rake;
        public int PlayerCount;
        public PokerEnums.Currency CurrencyPrizePool = PokerEnums.Currency.Unknown;
        public decimal PrizePool;
        public string TournamentStartedLocalTimeZone;
        public DateTime TournamentStartedLocal;
        public DateTime TournamentStartedET;
        public string TournamentFinishedLocalTimeZone;
        public DateTime TournamentFinishedLocal;
        public DateTime TournamentFinishedET;
        public List<Player> Players = new List<Player>();
        public int HeroPlace;

        public static PokerTournamentSummary FromTournamentSummary(string tournamentSummary)
        {
            PokerTournamentSummary pts = new PokerTournamentSummary();
            foreach (var line in tournamentSummary.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                AnalyzeLine(line.TrimStart(' ').TrimEnd(' '), pts);
            }
            pts.TournamentSummary = tournamentSummary;
            return pts;
        }

        #region Regex

        public static Regex RegexHeader = new Regex(@"\APokerStars Tournament #(?<tournament_number>\d+), (?<game_type>.+)\z");
        public static Regex RegexSuperSatellite = new Regex(@"\ASuper Satellite\z");
        public static Regex RegexBuyIn = new Regex(@"\ABuy-In: (?<buyin_currency_symbol>\$|€|£)?(?<buyin>(\d|\.)+)\/(?<rake_currency_symbol>\$|€|£)?(?<rake>(\d|\.)+) (?<currency>(USD|EUR|GBP))?\z");
        public static Regex RegexPlayerCount = new Regex(@"\A(?<player_count>\d+) players\z");
        public static Regex RegexPrizePool = new Regex(@"\ATotal Prize Pool: (?<prize_currency_symbol>\$|€|£)?(?<prize>(\d|\.)+) (?<currency>(USD|EUR|GBP))?\z");
        public static Regex RegexTargetTournament = new Regex(@"\ATarget Tournament #(?<tournament_number>\d+) Buy-In: (?<tournament_currency_symbol>\$|€|£)?(?<tournament_buyin>(\d|\.)+) (?<tournament_currency>(USD|EUR|GBP))?\z");
        public static Regex RegexTournamentStarted = new Regex(@"\ATournament started (?<year>\d\d\d\d).(?<month>\d\d).(?<day>\d\d) (?<hour>\d{1,2}):(?<minute>\d{1,2}):(?<second>\d{1,2}) (?<timezone>.+) \[(?<year_et>\d\d\d\d).(?<month_et>\d\d).(?<day_et>\d\d) (?<hour_et>\d{1,2}):(?<minute_et>\d{1,2}):(?<second_et>\d{1,2}) (?<timezone_et>.+)\]\z");
        public static Regex RegexTournamentFinished = new Regex(@"\ATournament finished (?<year>\d\d\d\d).(?<month>\d\d).(?<day>\d\d) (?<hour>\d{1,2}):(?<minute>\d{1,2}):(?<second>\d{1,2}) (?<timezone>.+) \[(?<year_et>\d\d\d\d).(?<month_et>\d\d).(?<day_et>\d\d) (?<hour_et>\d{1,2}):(?<minute_et>\d{1,2}):(?<second_et>\d{1,2}) (?<timezone_et>.+)\]\z");
        public static Regex RegexPlayer = new Regex(@"\A *(?<place>\d+): (?<player_name>.+) \((?<country>.+)?\), *((?<prize_currency_symbol>(\$|€|£))?(?<prize>(\d|\.|,)+))?( *\((?<prize_pool_percents>(\d|\.)+)%\))?(?<still_playing>still playing)?(?<qualified>\(qualified for the target tournament\))?\z");
        public static Regex RegexFinishedPlace = new Regex(@"\AYou finished in (?<place>\d+)(st|nd|rd|th) place( \(eliminated at hand #(?<hand_number>\d+)\))?.\z");

        #endregion

        private static bool AnalyzeLine(string text, PokerTournamentSummary pts)
        {
            if (text.Equals("Freeroll")) { pts.CurrencyBuyIn = PokerEnums.Currency.Freeroll; return true; }
            if (text.Equals("Super Satellite")) { pts.IsSatellite = true; return true; }
            if (text.Equals("You are still playing in this tournament.")) return true;
            if (text.Equals("Statistics for this tournament are not available.")) return true;
            if (text.Equals("We currently do not record hands in freeroll tournaments.")) return true;

            Match match;

            #region Header

            match = RegexHeader.Match(text);
            if (match.Success)
            {
                pts.TournamentNumber = long.Parse(match.Groups["tournament_number"].Value);
                pts.GameType = match.Groups["game_type"].Value;
                return true;
            }

            #endregion

            #region BuyIn

            match = RegexBuyIn.Match(text);
            if (match.Success)
            {
                if (match.Groups["buyin_currency_symbol"].Success)
                {
                    switch (match.Groups["buyin_currency_symbol"].Value)
                    {
                        case "$":
                            pts.CurrencyBuyIn = PokerEnums.Currency.USD;
                            break;
                        case "€":
                            pts.CurrencyBuyIn = PokerEnums.Currency.EUR;
                            break;
                        case "£":
                            pts.CurrencyBuyIn = PokerEnums.Currency.GBP;
                            break;
                        default:
                            pts.CurrencyBuyIn = PokerEnums.Currency.Unknown;
                            break;
                    }
                }
                else
                {
                    pts.CurrencyBuyIn = PokerEnums.Currency.PlayMoney;
                }

                pts.BuyIn = decimal.Parse(match.Groups["buyin"].Value);
                pts.Rake = decimal.Parse(match.Groups["rake"].Value);

                return true;
            }

            #endregion

            #region PlayerCount

            match = RegexPlayerCount.Match(text);
            if (match.Success)
            {
                pts.PlayerCount = int.Parse(match.Groups["player_count"].Value);
                return true;
            }

            #endregion

            #region PrizePool

            match = RegexPrizePool.Match(text);
            if (match.Success)
            {
                if (match.Groups["prize_currency_symbol"].Success)
                {
                    switch (match.Groups["prize_currency_symbol"].Value)
                    {
                        case "$":
                            pts.CurrencyPrizePool = PokerEnums.Currency.USD;
                            break;
                        case "€":
                            pts.CurrencyPrizePool = PokerEnums.Currency.EUR;
                            break;
                        case "£":
                            pts.CurrencyPrizePool = PokerEnums.Currency.GBP;
                            break;
                        default:
                            pts.CurrencyPrizePool = PokerEnums.Currency.Unknown;
                            break;
                    }
                }
                else
                {
                    pts.CurrencyPrizePool = PokerEnums.Currency.PlayMoney;
                }

                pts.PrizePool = decimal.Parse(match.Groups["prize"].Value);
                return true;
            }

            #endregion

            #region TargetTournament

            match = RegexTargetTournament.Match(text);
            if (match.Success)
            {
                if (match.Groups["tournament_currency_symbol"].Success)
                {
                    switch (match.Groups["tournament_currency_symbol"].Value)
                    {
                        case "$":
                            pts.CurrencyTargetBuyIn = PokerEnums.Currency.USD;
                            break;
                        case "€":
                            pts.CurrencyTargetBuyIn = PokerEnums.Currency.EUR;
                            break;
                        case "£":
                            pts.CurrencyTargetBuyIn = PokerEnums.Currency.GBP;
                            break;
                        default:
                            pts.CurrencyTargetBuyIn = PokerEnums.Currency.Unknown;
                            break;
                    }
                }
                else
                {
                    pts.CurrencyTargetBuyIn = PokerEnums.Currency.PlayMoney;
                }

                pts.TargetTournamentBuyIn = decimal.Parse(match.Groups["tournament_buyin"].Value);
                return true;
            }

            #endregion

            #region TournamentStarted

            match = RegexTournamentStarted.Match(text);
            if (match.Success)
            {
                pts.TournamentStartedLocal = new DateTime(int.Parse(match.Groups["year"].Value), int.Parse(match.Groups["month"].Value), int.Parse(match.Groups["day"].Value), int.Parse(match.Groups["hour"].Value), int.Parse(match.Groups["minute"].Value), int.Parse(match.Groups["second"].Value));
                pts.TournamentStartedLocalTimeZone = match.Groups["timezone"].Value;
                pts.TournamentStartedET = new DateTime(int.Parse(match.Groups["year_et"].Value), int.Parse(match.Groups["month_et"].Value), int.Parse(match.Groups["day_et"].Value), int.Parse(match.Groups["hour_et"].Value), int.Parse(match.Groups["minute_et"].Value), int.Parse(match.Groups["second_et"].Value));
                return true;
            }

            #endregion

            #region TournamentFinished

            match = RegexTournamentFinished.Match(text);
            if (match.Success)
            {
                pts.TournamentFinishedLocal = new DateTime(int.Parse(match.Groups["year"].Value), int.Parse(match.Groups["month"].Value), int.Parse(match.Groups["day"].Value), int.Parse(match.Groups["hour"].Value), int.Parse(match.Groups["minute"].Value), int.Parse(match.Groups["second"].Value));
                pts.TournamentFinishedLocalTimeZone = match.Groups["timezone"].Value;
                pts.TournamentFinishedET = new DateTime(int.Parse(match.Groups["year_et"].Value), int.Parse(match.Groups["month_et"].Value), int.Parse(match.Groups["day_et"].Value), int.Parse(match.Groups["hour_et"].Value), int.Parse(match.Groups["minute_et"].Value), int.Parse(match.Groups["second_et"].Value));
                return true;
            }

            #endregion

            #region Player

            match = RegexPlayer.Match(text);
            if (match.Success)
            {
                var player = new Player
                {
                    Place = int.Parse(match.Groups["place"].Value),
                    PlayerName = match.Groups["player_name"].Value,
                };
                if (match.Groups["country"].Success) player.Country = match.Groups["country"].Value;
                if (match.Groups["still_playing"].Success) player.StillPlaying = true;
                if (match.Groups["qualified"].Success) player.QualifiedForTournament = true;
                if (match.Groups["prize"].Success)
                {
                    player.Prize = decimal.Parse(match.Groups["prize"].Value);

                    if (match.Groups["prize_currency_symbol"].Success)
                    {
                        switch (match.Groups["prize_currency_symbol"].Value)
                        {
                            case "$":
                                pts.CurrencyPrizePool = PokerEnums.Currency.USD;
                                break;
                            case "€":
                                pts.CurrencyPrizePool = PokerEnums.Currency.EUR;
                                break;
                            case "£":
                                pts.CurrencyPrizePool = PokerEnums.Currency.GBP;
                                break;
                            default:
                                pts.CurrencyPrizePool = PokerEnums.Currency.Unknown;
                                break;
                        }
                    }
                    else
                    {
                        pts.CurrencyPrizePool = PokerEnums.Currency.PlayMoney;
                    }
                }
                pts.Players.Add(player);

                return true;
            }

            #endregion

            #region FinishedPlace

            match = RegexFinishedPlace.Match(text);
            if (match.Success)
            {
                pts.HeroPlace = int.Parse(match.Groups["place"].Value);
                return true;
            }

            #endregion

            return false;
        }
    }

    public class PokerHand
    {
        public string HandHistory;
        public long HandNumber;
        public bool IsZoom;

        public decimal LevelSmallBlind;
        public decimal LevelBigBlind;
        public decimal LevelAnte;
        public DateTime TimeStampET;
        public DateTime TimeStampLocal;
        public string LocalTimeZone;

        public bool IsTournament;
        public long TournamentNumber;
        public PokerEnums.Currency Currency = PokerEnums.Currency.Unknown;
        public decimal TotalBuyIn;
        public decimal BuyIn;
        public decimal Bounty;
        public decimal Rake;
        public string GameType;
        public string AdditionalInfo;
        public string LevelNumber;

        public string TableName;
        public int TableSize;
        public int ButtonSeat;
        public int ButtonSeatHandHistory;

        public Player[] Seats;
        public List<PokerCommand> PokerCommands = new List<PokerCommand>();

        public Ev Ev;

        public static PokerHand FromHandHistory(string handHistoryText)
        {
            PokerHand pokerHand = new PokerHand();
            foreach (var line in handHistoryText.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                AnalyzeLine(line.TrimEnd(' '), pokerHand);
            }
            pokerHand.HandHistory = handHistoryText;
            var postAntes = pokerHand.PokerCommands.OfType<PokerCommands.PostAnte>().ToArray();
            if (postAntes.Any())
            {
                // set ante
                pokerHand.LevelAnte = postAntes.Max(o => o.Amount);
                // set collect bets after antes are placed
                pokerHand.PokerCommands.Insert(pokerHand.PokerCommands.FindLastIndex(o => o is PokerCommands.PostAnte) + 1, new PokerCommands.CollectPots(Street.Preflop));
            }

            #region Set Position

            var players = pokerHand.Seats.Where(a => a != null).ToList();
            while (players[0].SeatNumberHandHistory != pokerHand.ButtonSeatHandHistory)
            {
                players.Add(players[0]);
                players.RemoveAt(0);
            }

            switch (players.Count)
            {
                case 2:
                    players[0].Position = PokerEnums.Position.BU;
                    players[0 + 1].Position = PokerEnums.Position.SB;
                    break;
                case 3:
                    players[0].Position = PokerEnums.Position.BU;
                    players[0 + 1].Position = PokerEnums.Position.SB;
                    players[0 + 2].Position = PokerEnums.Position.BB;
                    break;
                case 4:
                    players[0].Position = PokerEnums.Position.BU;
                    players[0 + 1].Position = PokerEnums.Position.SB;
                    players[0 + 2].Position = PokerEnums.Position.BB;
                    players[0 + 3].Position = PokerEnums.Position.CO;
                    break;
                case 5:
                    players[0].Position = PokerEnums.Position.BU;
                    players[0 + 1].Position = PokerEnums.Position.SB;
                    players[0 + 2].Position = PokerEnums.Position.BB;
                    players[0 + 3].Position = PokerEnums.Position.EP;
                    players[0 + 4].Position = PokerEnums.Position.CO;
                    break;
                case 6:
                    players[0].Position = PokerEnums.Position.BU;
                    players[0 + 1].Position = PokerEnums.Position.SB;
                    players[0 + 2].Position = PokerEnums.Position.BB;
                    players[0 + 3].Position = PokerEnums.Position.EP;
                    players[0 + 4].Position = PokerEnums.Position.MP;
                    players[0 + 5].Position = PokerEnums.Position.CO;
                    break;
                case 7:
                    players[0].Position = PokerEnums.Position.BU;
                    players[0 + 1].Position = PokerEnums.Position.SB;
                    players[0 + 2].Position = PokerEnums.Position.BB;
                    players[0 + 3].Position = PokerEnums.Position.EP;
                    players[0 + 4].Position = PokerEnums.Position.MP;
                    players[0 + 5].Position = PokerEnums.Position.MP;
                    players[0 + 6].Position = PokerEnums.Position.CO;
                    break;
                case 8:
                    players[0].Position = PokerEnums.Position.BU;
                    players[0 + 1].Position = PokerEnums.Position.SB;
                    players[0 + 2].Position = PokerEnums.Position.BB;
                    players[0 + 3].Position = PokerEnums.Position.EP;
                    players[0 + 4].Position = PokerEnums.Position.EP;
                    players[0 + 5].Position = PokerEnums.Position.MP;
                    players[0 + 6].Position = PokerEnums.Position.MP;
                    players[0 + 7].Position = PokerEnums.Position.CO;
                    break;
                case 9:
                    players[0].Position = PokerEnums.Position.BU;
                    players[0 + 1].Position = PokerEnums.Position.SB;
                    players[0 + 2].Position = PokerEnums.Position.BB;
                    players[0 + 3].Position = PokerEnums.Position.EP;
                    players[0 + 4].Position = PokerEnums.Position.EP;
                    players[0 + 5].Position = PokerEnums.Position.MP;
                    players[0 + 6].Position = PokerEnums.Position.MP;
                    players[0 + 7].Position = PokerEnums.Position.MP;
                    players[0 + 8].Position = PokerEnums.Position.CO;
                    break;
                case 10:
                    players[0].Position = PokerEnums.Position.BU;
                    players[0 + 1].Position = PokerEnums.Position.SB;
                    players[0 + 2].Position = PokerEnums.Position.BB;
                    players[0 + 3].Position = PokerEnums.Position.EP;
                    players[0 + 4].Position = PokerEnums.Position.EP;
                    players[0 + 5].Position = PokerEnums.Position.EP;
                    players[0 + 6].Position = PokerEnums.Position.MP;
                    players[0 + 7].Position = PokerEnums.Position.MP;
                    players[0 + 8].Position = PokerEnums.Position.MP;
                    players[0 + 9].Position = PokerEnums.Position.CO;
                    break;
            }

            if (players.Count < 3)
            {
                players[0].SeatFromUtgToBb = 0;
                players[0].SeatFromBbToUtg = 1;
                players[1].SeatFromUtgToBb = 1;
                players[1].SeatFromBbToUtg = 0;
            }
            else
            {
                players.Add(players[0]);
                players.RemoveAt(0);
                players.Add(players[0]);
                players.RemoveAt(0);
                players.Add(players[0]);
                players.RemoveAt(0);

                for (int i = 0; i < players.Count; i++)
                {
                    players[i].SeatFromUtgToBb = players.IndexOf(players[i]);
                }

                players.Reverse();

                for (int i = 0; i < players.Count; i++)
                {
                    players[i].SeatFromBbToUtg = players.IndexOf(players[i]);
                }
            }

            #endregion

            return pokerHand;
        }

        #region Regex

        // Regex debug info

        // Tournament Hand

        // \APokerStars Hand #(?<hand_id>\d+): +(?<zoom>Zoom |)Tournament #(?<tournament_id>\d+), +((?<buyin_fpp>\d+)FPP|(?<freeroll>Freeroll)|(\$|€|£|)(?<buyin>(\d|\.)+)(\+(\$|€|£|)(?<bounty>(\d|\.)+))?\+(\$|€|£|)(?<rake>(\d|\.)+)) *(?<currency>(USD|EUR|GBP|)) +(?<game_type>.+) +- +(?<additional_info>.*)Level (?<level_number>(I|V|X|L|C|D|M))+ \((?<level_sb>\d+)\/(?<level_bb>\d+)\) +- +(?<year>\d\d\d\d).(?<month>\d\d).(?<day>\d\d) (?<hour>\d{1,2}):(?<minute>\d{1,2}):(?<second>\d{1,2}) (?<timezone>.+) \[(?<year_et>\d\d\d\d).(?<month_et>\d\d).(?<day_et>\d\d) (?<hour_et>\d{1,2}):(?<minute_et>\d{1,2}):(?<second_et>\d{1,2}) (?<timezone_et>.+)\]\z
        // PokerStars Hand #126991433006: Tournament #1080084995, $7.35+$0.15 USD Hold'em No Limit - Level I (25/50) - 2014/12/17 3:42:52 EET [2014/12/16 20:42:52 ET]
        // PokerStars Hand #127729299039: Tournament #1087458547, $10.00+$2.50+$1.25 USD Hold'em No Limit - Level XXXII (8000/16000) - 2014/12/30 6:21:55 EET [2014/12/29 23:21:55 ET]
        // PokerStars Hand #127729394086: Tournament #1057766685, Freeroll  Hold'em No Limit - Level XVI (1000/2000) - 2014/12/30 6:26:15 EET [2014/12/29 23:26:15 ET]
        // PokerStars Hand #127729452661: Tournament #1087297757, 4500+500 Courchevel Pot Limit - Level III (25/50) - 2014/12/30 6:28:58 EET [2014/12/29 23:28:58 ET]
        // PokerStars Hand #127729497630: Tournament #1087266190, 140000+40000+20000 Hold'em No Limit - Level IV (50/100) - 2014/12/30 6:31:04 EET [2014/12/29 23:31:04 ET]
        // PokerStars Hand #127730077127: Tournament #1093318289, €3.29+€0.21 EUR Hold'em No Limit - Match Round I, Level IV (25/50) - 2014/12/30 7:00:15 EET [2014/12/30 0:00:15 ET]
        // PokerStars Hand #127544216893: Zoom Tournament #1082326263, $8.00+$0.80 USD Hold'em No Limit - Level XVIII (500/1000) - 2014/12/27 6:23:19 EET [2014/12/26 23:23:19 ET]

        // hand_id	            126991433006
        // tournament_id	    1080084995
        // zoom                 
        // (?)buyin_fpp	 
        // (?)freeroll	 
        // buyin	            7.35
        // (?)bounty	 
        // rake	                0.15
        // (?)currency	        USD
        // game_type	        Hold'em No Limit
        // (?)additional_info      
        // level_number	        I
        // level_sb	            25
        // level_bb	            50

        // year	                2014
        // month	            12
        // day	                17
        // hour	                3
        // minute	            42
        // second	            52
        // timezone	            EET
        // year_et	            2014
        // month_et	            12
        // day_et	            16
        // hour_et	            20
        // minute_et	        42
        // second_et	        52
        // timezone_et	        ET

        // Cash Hand

        // \APokerStars (?<zoom>Zoom |)Hand #(?<hand_id>\d+): +(?<game_type>.+) \((?<level_sb_currency_symbol>(\$|€|£|))(?<level_sb>(\d|\.)+)\/(?<level_bb_currency_symbol>(\$|€|£|))(?<level_bb>(\d|\.)+) *(?<currency>(USD|EUR|GBP|))\) +- +(?<year>\d\d\d\d).(?<month>\d\d).(?<day>\d\d) (?<hour>\d{1,2}):(?<minute>\d{1,2}):(?<second>\d{1,2}) (?<timezone>.+) \[(?<year_et>\d\d\d\d).(?<month_et>\d\d).(?<day_et>\d\d) (?<hour_et>\d{1,2}):(?<minute_et>\d{1,2}):(?<second_et>\d{1,2}) (?<timezone_et>.+)\]\z
        // PokerStars Hand #127728989406:  Hold'em No Limit ($0.01/$0.02 USD) - 2014/12/30 6:08:13 EET [2014/12/29 23:08:13 ET]
        // PokerStars Hand #127728946819:  Hold'em No Limit (€0.01/€0.02 EUR) - 2014/12/30 6:06:22 EET [2014/12/29 23:06:22 ET]
        // PokerStars Hand #127728928558:  Hold'em No Limit (£0.02/£0.05 GBP) - 2014/12/30 6:05:35 EET [2014/12/29 23:05:35 ET]
        // PokerStars Zoom Hand #127671329615:  Hold'em No Limit ($2.50/$5.00) - 2014/12/29 7:24:59 EET [2014/12/29 0:24:59 ET]
        // PokerStars Hand #127729737119:  Hold'em No Limit (25/50) - 2014/12/30 6:42:32 EET [2014/12/29 23:42:32 ET]

        // (?)zoom	 
        // hand_id	                        127728989406
        // game_type	                    Hold'em No Limit
        // (?)level_sb_currency_symbol	    $
        // level_sb	                        0.01
        // (?)level_bb_currency_symbol	    $
        // level_bb	                        0.02
        // (?)currency	                    USD

        // year	                            2014
        // month	                        12
        // day	                            30
        // hour	                            6
        // minute	                        08
        // second	                        13
        // timezone	                        EET
        // year_et	                        2014
        // month_et	                        12
        // day_et	                        29
        // hour_et	                        23
        // minute_et	                    08
        // second_et	                    13
        // timezone_et	                    ET

        private static readonly Regex RegexHeaderTournament = new Regex(@"\APokerStars Hand #(?<hand_id>\d+): +(?<zoom>Zoom )?Tournament #(?<tournament_id>\d+), +((?<buyin_fpp>\d+)FPP|(?<freeroll>Freeroll)|(\$|€|£)?(?<buyin>(\d|\.)+)(\+(\$|€|£)?(?<bounty>(\d|\.)+))?\+(\$|€|£)?(?<rake>(\d|\.)+)) *(?<currency>(USD|EUR|GBP)?) +(?<game_type>.+) +- +(?<additional_info>.+)?Level (?<level_number>(I|V|X|L|C|D|M))+ \((?<level_sb>\d+)\/(?<level_bb>\d+)\) +- +(?<year>\d\d\d\d).(?<month>\d\d).(?<day>\d\d) (?<hour>\d{1,2}):(?<minute>\d{1,2}):(?<second>\d{1,2}) (?<timezone>.+) \[(?<year_et>\d\d\d\d).(?<month_et>\d\d).(?<day_et>\d\d) (?<hour_et>\d{1,2}):(?<minute_et>\d{1,2}):(?<second_et>\d{1,2}) (?<timezone_et>.+)\]\z");
        private static readonly Regex RegexHeaderCash = new Regex(@"\APokerStars (?<zoom>Zoom )?Hand #(?<hand_id>\d+): +(?<game_type>.+) \((?<level_sb_currency_symbol>(\$|€|£)?)(?<level_sb>(\d|\.)+)\/(?<level_bb_currency_symbol>(\$|€|£)?)(?<level_bb>(\d|\.)+) *(?<currency>(USD|EUR|GBP)?)\) +- +(?<year>\d\d\d\d).(?<month>\d\d).(?<day>\d\d) (?<hour>\d{1,2}):(?<minute>\d{1,2}):(?<second>\d{1,2}) (?<timezone>.+) \[(?<year_et>\d\d\d\d).(?<month_et>\d\d).(?<day_et>\d\d) (?<hour_et>\d{1,2}):(?<minute_et>\d{1,2}):(?<second_et>\d{1,2}) (?<timezone_et>.+)\]\z");
        public static Regex RegexSeatMaxButton = new Regex(@"\ATable '(?<table_name>.+)' (?<table_size>\d+)-max Seat #(?<button_seat>\d+) is the button\z");
        public static Regex RegexPlayer = new Regex(@"\ASeat (?<seat_number>\d{1,2}): (?<player_name>.+) \((\$|€|£)?(?<stack>(\d|\.)+) in chips\)( is sitting out| out of hand \(moved from another table into small blind\))?\z");
        public static Regex RegexPostBlinds = new Regex(@"\A(?<player_name>.+): posts (?<type>small blind|big blind|the ante) (\$|€|£)?(?<amount>(\d|\.)+)(?<all_in> and is all-in|)\z");
        public static Regex RegexDealtTo = new Regex(@"\ADealt to (?<player_name>.+) \[(?<card0>..) (?<card1>..)\]\z");
        public static Regex RegexFold = new Regex(@"\A(?<player_name>.+): folds( \[(?<card0>..)( (?<card1>..))?\])?\z");
        public static Regex RegexCheck = new Regex(@"\A(?<player_name>.+): checks\z");
        public static Regex RegexCall = new Regex(@"\A(?<player_name>.+): calls (\$|€|£)?(?<amount>(\d|\.)+)(?<all_in> and is all-in|)\z");
        public static Regex RegexBet = new Regex(@"\A(?<player_name>.+): bets (\$|€|£)?(?<amount>(\d|\.)+)(?<all_in> and is all-in|)\z");
        public static Regex RegexRaise = new Regex(@"\A(?<player_name>.+): raises (\$|€|£)?(?<amount>(\d|\.)+) to (\$|€|£)?(?<amount_in_total>(\d|\.)+)(?<all_in> and is all-in|)\z");
        public static Regex RegexUncalledBetReturn = new Regex(@"\AUncalled bet \((\$|€|£)?(?<amount>(\d|\.)+)\) returned to (?<player_name>.+)\z");
        public static Regex RegexShows = new Regex(@"\A(?<player_name>.+): shows (\[(?<card0>..) (?<card1>..)\]|\[(?<card0one>..)\])( \((?<showdown_info>.+)\))?\z");
        public static Regex RegexCollectFromPot = new Regex(@"\A(?<player_name>.+) collected (\$|€|£)?(?<amount>(\d|\.)+) from.+pot");
        public static Regex RegexMuck = new Regex(@"\A(?<player_name>.+): mucks hand\z");
        public static Regex RegexTotalPot = new Regex(@"\ATotal pot.+\| Rake (\$|€|£)?(?<rake>(\d|\.)+)\z");
        public static Regex RegexMuckedInfo = new Regex(@"\ASeat \d{1,2}: (?<player_name>.+) mucked \[(?<card0>..) (?<card1>..)\]\z");
        public static Regex RegexFlop = new Regex(@"\A\*\*\* FLOP \*\*\* \[(?<card0>..) (?<card1>..) (?<card2>..)\]\z");
        public static Regex RegexTurn = new Regex(@"\A\*\*\* TURN \*\*\* \[(?<card0>..) (?<card1>..) (?<card2>..)\] \[(?<card3>..)\]\z");
        public static Regex RegexRiver = new Regex(@"\A\*\*\* RIVER \*\*\* \[(?<card0>..) (?<card1>..) (?<card2>..) (?<card3>..)\] \[(?<card4>..)\].?\z"); // .? gale, nes kai kur budavo ne i tema "A" raide paciam gale wtf..
        public static Regex RegexWinTournament = new Regex(@"(?<player_name>.+) wins the tournament and receives (?<winnings_currency_symbol>(\$|€|£)?)(?<winnings>(\d|\.)+) - congratulations!\z");
        public static Regex RegexFinished = new Regex(@"(?<player_name>.+) finished the tournament in (?<place>\d+)(st|nd|rd|th) place( and received (?<winnings_currency_symbol>(\$|€|£)?)(?<winnings>(\d|\.)+).|)\z");
        public static Regex RegexChat = new Regex("(?<player_name>.+) said, \\\"(?<text>.*)\\\"\\z");


        #endregion

        private static bool AnalyzeLine(string text, PokerHand pokerHand)
        {
            if (text.Equals("*** SHOW DOWN ***")) { pokerHand.PokerCommands.Add(new PokerCommands.CollectPots(Street.River)); pokerHand.PokerCommands.Add(new PokerCommands.FinalizePots()); return true; }
            if (text.Equals("*** HOLE CARDS ***")) return true;
            if (text.Equals("*** SUMMARY ***")) return true;
            if (text.StartsWith("Board [")) return true;
            if (text.StartsWith("Seat ") && (text.Contains("folded") || text.Contains("showed") || text.Contains("collected"))) return true;
            if (text.Contains("doesn't show hand")) return true;
            if (text.Contains(" has timed out")) return true;
            if (text.Contains(" has timed out while being disconnected")) return true;
            if (text.Contains(" has timed out while disconnected")) return true;
            if (text.Contains(" is sitting out") && !text.StartsWith("Seat ")) return true;
            if (text.Contains(" has returned")) return true;
            if (text.Contains(" is disconnected")) return true;
            if (text.Contains(" is connected")) return true;
            if (text.Contains(" sits out")) return true;
            if (text.Contains(" joins the table at seat #")) return true;
            if (text.Contains(" will be allowed to play after the button")) return true;
            if (text.Contains(" leaves the table")) return true;
            if (text.Contains(" for eliminating ") && text.Contains(" bounty ")) return true;
            if (text.Contains(" wins an entry to tournament ")) return true;
            if (text.Contains(" wins the tournament - congratulations!")) return true;
            if (text.Contains(" re-buys and receives ")) return true;
            if (text.Contains(" takes the add-on and receives ")) return true;
            if (text.Contains("also received a Golden Sit & Go reward of ")) return true;

            Match match;

            #region 1st Line

            if (text.StartsWith("PokerStars Hand #") || text.StartsWith("PokerStars Zoom Hand #"))
            {
                if (text.Contains("Tournament #"))
                {
                    match = RegexHeaderTournament.Match(text);
                    if (!match.Success) throw new NotSupportedException();

                    pokerHand.IsTournament = true;
                    pokerHand.HandNumber = long.Parse(match.Groups["hand_id"].Value);
                    pokerHand.TournamentNumber = long.Parse(match.Groups["tournament_id"].Value);
                    if (match.Groups["zoom"].Success) pokerHand.IsZoom = true;
                    if (match.Groups["buyin_fpp"].Success) pokerHand.Currency = PokerEnums.Currency.FPP;
                    if (match.Groups["freeroll"].Success) pokerHand.Currency = PokerEnums.Currency.Freeroll;
                    if (match.Groups["currency"].Success)
                    {
                        switch (match.Groups["currency"].Value)
                        {
                            case "USD":
                                pokerHand.Currency = PokerEnums.Currency.USD;
                                break;
                            case "EUR":
                                pokerHand.Currency = PokerEnums.Currency.EUR;
                                break;
                            case "BGP":
                                pokerHand.Currency = PokerEnums.Currency.GBP;
                                break;
                            default:
                                pokerHand.Currency = PokerEnums.Currency.Unknown;
                                break;
                        }
                    }
                    else
                    {
                        if (pokerHand.Currency == PokerEnums.Currency.Unknown)
                        {
                            pokerHand.Currency = PokerEnums.Currency.PlayMoney;
                        }
                    }
                    if (pokerHand.Currency == PokerEnums.Currency.USD || pokerHand.Currency == PokerEnums.Currency.EUR || pokerHand.Currency == PokerEnums.Currency.GBP)
                    {
                        pokerHand.BuyIn = decimal.Parse(match.Groups["buyin"].Value);
                        if (match.Groups["bounty"].Success) pokerHand.Bounty = decimal.Parse(match.Groups["bounty"].Value);
                        pokerHand.Rake = decimal.Parse(match.Groups["rake"].Value);
                    }
                    if (pokerHand.Currency == PokerEnums.Currency.FPP) pokerHand.BuyIn = decimal.Parse(match.Groups["buyin_fpp"].Value);
                    pokerHand.GameType = match.Groups["game_type"].Value;
                    if (match.Groups["additional_info"].Success) pokerHand.AdditionalInfo = match.Groups["additional_info"].Value;
                    pokerHand.LevelNumber = match.Groups["level_number"].Value;

                    pokerHand.LevelSmallBlind = decimal.Parse(match.Groups["level_sb"].Value);
                    pokerHand.LevelBigBlind = decimal.Parse(match.Groups["level_bb"].Value);

                    pokerHand.TimeStampLocal = new DateTime(int.Parse(match.Groups["year"].Value), int.Parse(match.Groups["month"].Value), int.Parse(match.Groups["day"].Value), int.Parse(match.Groups["hour"].Value), int.Parse(match.Groups["minute"].Value), int.Parse(match.Groups["second"].Value));
                    pokerHand.LocalTimeZone = match.Groups["timezone"].Value;
                    pokerHand.TimeStampET = new DateTime(int.Parse(match.Groups["year_et"].Value), int.Parse(match.Groups["month_et"].Value), int.Parse(match.Groups["day_et"].Value), int.Parse(match.Groups["hour_et"].Value), int.Parse(match.Groups["minute_et"].Value), int.Parse(match.Groups["second_et"].Value));


                    pokerHand.TotalBuyIn = pokerHand.BuyIn + pokerHand.Bounty + pokerHand.Rake;
                    return true;
                }
                else
                {
                    match = RegexHeaderCash.Match(text);
                    if (!match.Success) throw new NotSupportedException();

                    if (match.Groups["zoom"].Success) pokerHand.IsZoom = true;
                    pokerHand.HandNumber = long.Parse(match.Groups["hand_id"].Value);
                    pokerHand.GameType = match.Groups["game_type"].Value;
                    pokerHand.LevelSmallBlind = decimal.Parse(match.Groups["level_sb"].Value);
                    pokerHand.LevelBigBlind = decimal.Parse(match.Groups["level_bb"].Value);
                    if (match.Groups["currency"].Success)
                    {
                        switch (match.Groups["currency"].Value)
                        {
                            case "USD":
                                pokerHand.Currency = PokerEnums.Currency.USD;
                                break;
                            case "EUR":
                                pokerHand.Currency = PokerEnums.Currency.EUR;
                                break;
                            case "BGP":
                                pokerHand.Currency = PokerEnums.Currency.GBP;
                                break;
                            default:
                                pokerHand.Currency = PokerEnums.Currency.Unknown;
                                break;
                        }
                    }
                    else
                    {
                        if (match.Groups["level_sb_currency_symbol"].Success)
                        {
                            switch (match.Groups["level_sb_currency_symbol"].Value)
                            {
                                case "$":
                                    pokerHand.Currency = PokerEnums.Currency.USD;
                                    break;
                                case "€":
                                    pokerHand.Currency = PokerEnums.Currency.EUR;
                                    break;
                                case "£":
                                    pokerHand.Currency = PokerEnums.Currency.GBP;
                                    break;
                                default:
                                    pokerHand.Currency = PokerEnums.Currency.Unknown;
                                    break;
                            }
                        }
                        else
                        {
                            pokerHand.Currency = PokerEnums.Currency.PlayMoney;
                        }
                    }

                    pokerHand.TimeStampLocal = new DateTime(int.Parse(match.Groups["year"].Value), int.Parse(match.Groups["month"].Value), int.Parse(match.Groups["day"].Value), int.Parse(match.Groups["hour"].Value), int.Parse(match.Groups["minute"].Value), int.Parse(match.Groups["second"].Value));
                    pokerHand.LocalTimeZone = match.Groups["timezone"].Value;
                    pokerHand.TimeStampET = new DateTime(int.Parse(match.Groups["year_et"].Value), int.Parse(match.Groups["month_et"].Value), int.Parse(match.Groups["day_et"].Value), int.Parse(match.Groups["hour_et"].Value), int.Parse(match.Groups["minute_et"].Value), int.Parse(match.Groups["second_et"].Value));

                    return true;
                }
            }

            #endregion

            #region 2nd Line

            match = RegexSeatMaxButton.Match(text);
            if (match.Success)
            {
                pokerHand.TableName = match.Groups["table_name"].Value;
                pokerHand.TableSize = int.Parse(match.Groups["table_size"].Value);
                pokerHand.ButtonSeatHandHistory = int.Parse(match.Groups["button_seat"].Value);
                pokerHand.ButtonSeat = pokerHand.ButtonSeatHandHistory - 1;

                pokerHand.Seats = new Player[pokerHand.TableSize];

                return true;
            }

            #endregion

            #region Player

            match = RegexPlayer.Match(text);
            if (match.Success)
            {
                Player player = new Player();
                player.SeatNumberHandHistory = int.Parse(match.Groups["seat_number"].Value);
                player.SeatNumber = player.SeatNumberHandHistory - 1;
                player.PlayerName = match.Groups["player_name"].Value;
                player.Stack = decimal.Parse(match.Groups["stack"].Value);
                player.IsInPlay = true;

                pokerHand.Seats[player.SeatNumber] = player;

                return true;
            }

            #endregion

            //

            #region Post sb/bb/ante

            match = RegexPostBlinds.Match(text);
            if (match.Success)
            {
                var player = pokerHand.Seats.First(o => o != null && o.PlayerName.Equals(match.Groups["player_name"].Value));
                decimal amount = decimal.Parse(match.Groups["amount"].Value);

                switch (match.Groups["type"].Value)
                {
                    case "the ante":
                        pokerHand.PokerCommands.Add(new PokerCommands.PostAnte(text, player, amount));
                        break;
                    case "small blind":
                        pokerHand.PokerCommands.Add(new PokerCommands.PostSmallBlind(text, player, amount));
                        break;
                    case "big blind":
                        pokerHand.PokerCommands.Add(new PokerCommands.PostBigBlind(text, player, amount));
                        break;
                }

                return true;
            }

            #endregion

            #region Dealt to

            match = RegexDealtTo.Match(text);
            if (match.Success)
            {
                var player = pokerHand.Seats.First(o => o != null && o.PlayerName.Equals(match.Groups["player_name"].Value));
                player.PocketCards = new[] { match.Groups["card0"].Value, match.Groups["card1"].Value };
                player.IsHero = true;

                pokerHand.PokerCommands.Add(new PokerCommands.DealtTo(text, player));

                return true;
            }

            #endregion

            #region Fold

            match = RegexFold.Match(text);
            if (match.Success)
            {
                var player = pokerHand.Seats.First(o => o != null && o.PlayerName.Equals(match.Groups["player_name"].Value));
                if (match.Groups["card1"].Success)
                {
                    player.PocketCards = new[] { match.Groups["card0"].Value, match.Groups["card1"].Value };
                }
                else if (match.Groups["card0"].Success)
                {
                    player.PocketCards = new[] { match.Groups["card0"].Value };
                }
                pokerHand.PokerCommands.Add(new PokerCommands.Fold(text, player));

                return true;
            }

            #endregion

            #region Check

            match = RegexCheck.Match(text);
            if (match.Success)
            {
                var player = pokerHand.Seats.First(o => o != null && o.PlayerName.Equals(match.Groups["player_name"].Value));
                pokerHand.PokerCommands.Add(new PokerCommands.Check(text, player));

                return true;
            }

            #endregion

            #region Call

            match = RegexCall.Match(text);
            if (match.Success)
            {
                var player = pokerHand.Seats.First(o => o != null && o.PlayerName.Equals(match.Groups["player_name"].Value));
                decimal amount = decimal.Parse(match.Groups["amount"].Value);
                bool allIn = match.Groups["all_in"].Success;

                pokerHand.PokerCommands.Add(new PokerCommands.Call(text, player, amount, allIn));

                return true;
            }

            #endregion

            #region Bet

            match = RegexBet.Match(text);
            if (match.Success)
            {
                var player = pokerHand.Seats.First(o => o != null && o.PlayerName.Equals(match.Groups["player_name"].Value));
                decimal amount = decimal.Parse(match.Groups["amount"].Value);
                bool allIn = match.Groups["all_in"].Success;

                pokerHand.PokerCommands.Add(new PokerCommands.Bet(text, player, amount, allIn));

                return true;
            }

            #endregion

            #region Raise

            match = RegexRaise.Match(text);
            if (match.Success)
            {
                var player = pokerHand.Seats.First(o => o != null && o.PlayerName.Equals(match.Groups["player_name"].Value));
                decimal amountInTotal = decimal.Parse(match.Groups["amount_in_total"].Value);
                bool allIn = match.Groups["all_in"].Success;

                pokerHand.PokerCommands.Add(new PokerCommands.Raise(text, player, amountInTotal, allIn));

                return true;
            }

            #endregion

            #region Uncalled Bet Return

            match = RegexUncalledBetReturn.Match(text);
            if (match.Success)
            {
                var player = pokerHand.Seats.First(o => o != null && o.PlayerName.Equals(match.Groups["player_name"].Value));
                decimal amount = decimal.Parse(match.Groups["amount"].Value);

                pokerHand.PokerCommands.Add(new PokerCommands.UncalledBetReturn(text, player, amount));

                return true;
            }

            #endregion

            #region Shows

            match = RegexShows.Match(text);
            if (match.Success)
            {
                var player = pokerHand.Seats.First(o => o != null && o.PlayerName.Equals(match.Groups["player_name"].Value));
                if (match.Groups["card0"].Success && match.Groups["card1"].Success)
                {
                    player.PocketCards = new[] { match.Groups["card0"].Value, match.Groups["card1"].Value };
                }
                if (match.Groups["card0one"].Success)
                {
                    player.PocketCards = new[] { match.Groups["card0one"].Value };
                }
                string info = match.Groups["showdown_info"].Value;

                pokerHand.PokerCommands.Add(new PokerCommands.Shows(text, player, info));

                return true;
            }

            #endregion

            #region Collect From Pot

            match = RegexCollectFromPot.Match(text);
            if (match.Success)
            {
                var player = pokerHand.Seats.First(o => o != null && o.PlayerName.Equals(match.Groups["player_name"].Value));
                decimal amount = decimal.Parse(match.Groups["amount"].Value);

                pokerHand.PokerCommands.Add(new PokerCommands.CollectFromPot(text, player, amount));

                return true;
            }

            #endregion

            #region Muck

            match = RegexMuck.Match(text);
            if (match.Success)
            {
                var player = pokerHand.Seats.First(o => o != null && o.PlayerName.Equals(match.Groups["player_name"].Value));
                pokerHand.PokerCommands.Add(new PokerCommands.Fold(text, player));

                return true;
            }

            #endregion

            #region Total Pot

            match = RegexTotalPot.Match(text);
            if (match.Success)
            {
                if (!pokerHand.IsTournament)
                {
                    pokerHand.Rake = decimal.Parse(match.Groups["rake"].Value);
                    return true;
                }
                else
                {
                    return true;
                }
            }


            #endregion

            #region MuckedInfo

            if (text.Contains(" mucked "))
            {
                match = RegexMuckedInfo.Match(text.Replace("(button) ", "").Replace("(small blind) ", "").Replace("(big blind) ", ""));
                if (match.Success)
                {
                    var player = pokerHand.Seats.First(o => o != null && o.PlayerName.Equals(match.Groups["player_name"].Value));
                    player.PocketCards = new[] { match.Groups["card0"].Value, match.Groups["card1"].Value };

                    return true;
                }
            }

            #endregion

            #region Flop

            match = RegexFlop.Match(text);
            if (match.Success)
            {
                string[] cards = { match.Groups["card0"].Value, match.Groups["card1"].Value, match.Groups["card2"].Value };

                pokerHand.PokerCommands.Add(new PokerCommands.CollectPots(Street.Preflop));
                pokerHand.PokerCommands.Add(new PokerCommands.Flop(text, cards));

                return true;
            }

            #endregion

            #region Turn

            match = RegexTurn.Match(text);
            if (match.Success)
            {
                string card = match.Groups["card3"].Value;

                pokerHand.PokerCommands.Add(new PokerCommands.CollectPots(Street.Flop));
                pokerHand.PokerCommands.Add(new PokerCommands.Turn(text, card));

                return true;
            }

            #endregion

            #region River

            match = RegexRiver.Match(text);
            if (match.Success)
            {
                string card = match.Groups["card4"].Value;

                pokerHand.PokerCommands.Add(new PokerCommands.CollectPots(Street.Turn));
                pokerHand.PokerCommands.Add(new PokerCommands.River(text, card));

                return true;
            }

            #endregion

            #region Win Tournament

            match = RegexWinTournament.Match(text);
            if (match.Success)
            {
                return true;
            }

            #endregion

            #region Finished

            match = RegexFinished.Match(text);
            if (match.Success)
            {
                return true;
            }

            #endregion

            #region Chat

            match = RegexChat.Match(text);
            if (match.Success)
            {
                return true;
            }

            #endregion

            return false;
        }
    }

    public class Table
    {
        public decimal TotalPot { get { return Pots.Sum(o => o.Amount) + Players.Sum(o => o.Bet); } }
        public decimal TotalPotBeforeCollection;
        public List<Pot> Pots = new List<Pot>();
        public List<Pot> PotsStreetByStreet = new List<Pot>();
        public Player[] Seats = null;
        public int TableSize = -1;
        public Player[] Players = new Player[0];
        public int PlayerCount { get { return Players.Length; } }
        public int ButtonSeatNumber = -1;
        public string[] CommunityCards = new string[5];
        public List<PokerCommand> ToDo = new List<PokerCommand>();
        public List<PokerCommand> UnDo = new List<PokerCommand>();

        public void Log(string text)
        {
        }

        public void LoadHand(PokerHand pokerHand)
        {
            Seats = pokerHand.Seats;
            Players = Seats.Where(o => o != null).ToArray();
            TableSize = pokerHand.TableSize;
            ButtonSeatNumber = pokerHand.ButtonSeat;
            foreach (var player in Players) player.Table = this;
            ToDo.AddRange(pokerHand.PokerCommands);
        }

        public void ToDoCommand()
        {
            if (ToDo.Any())
            {
                var pokerCommand = ToDo[0];
                ToDo.RemoveAt(0);
                pokerCommand.Exec(this);
                UnDo.Insert(0, pokerCommand);
            }
        }

        public void UnDoCommand()
        {
            if (UnDo.Any())
            {
                var pokerCommand = UnDo[0];
                UnDo.RemoveAt(0);
                pokerCommand.Undo(this);
                ToDo.Insert(0, pokerCommand);
            }
        }

        public void ToDoCommandsAll()
        {
            while (ToDo.Any())
            {
                ToDoCommand();
            }
        }

        public void UnDoCommandsAll()
        {
            while (UnDo.Any())
            {
                UnDoCommand();
            }
        }

        public void ToDoCommandsBeginning()
        {
            while (ToDo.Any() && !(ToDo[0] is PokerCommands.Fold || ToDo[0] is PokerCommands.Call || ToDo[0] is PokerCommands.Raise))
            {
                ToDoCommand();
            }
        }

        public string TableStateToString()
        {
            if (Players == null) return "Table empty.";

            StringBuilder sb = new StringBuilder();
            foreach (var player in Players)
            {
                sb.AppendLine(player.ToString());
            }
            return sb.ToString();
        }
    }

    public class Player
    {
        public Table Table;
        public string PlayerName;
        public int SeatNumberHandHistory;
        public int SeatNumber;
        public int SeatFromUtgToBb;
        public int SeatFromBbToUtg;
        public PokerEnums.Position Position;
        public decimal Stack;
        public decimal Bet;
        public bool IsInPlay;
        public string[] PocketCards;
        public bool ArePocketCardsOpenfaced;
        public bool IsHero;

        public void Fold()
        {
            IsInPlay = false;
        }

        public void UnFold()
        {
            IsInPlay = true;
        }

        public void PutChips(decimal amount)
        {
            Stack -= amount;
            Bet += amount;
        }

        public decimal PutChipsInTotal(decimal amountInTotal)
        {
            decimal amount = amountInTotal - Bet;
            Stack -= amount;
            Bet += amount;
            return amount;
        }

        public void CollectChipsFromPot(decimal amount)
        {
            Table.TotalPotBeforeCollection -= amount;
            Stack += amount;
        }

        public override string ToString()
        {
            return string.Format("{0}({1}) [{5}{6}] {2} {3} ({4})", IsInPlay ? "+" : "-", SeatNumber, PlayerName, Stack, Bet, PocketCards != null ? PocketCards[0] : "  ", PocketCards != null ? PocketCards[1] : "  ");
        }
    }

    public class Pot
    {
        public List<Player> Players = new List<Player>();
        public decimal Amount;
        public Street Street;

        public Pot(List<Player> players, decimal amount, Street street)
        {
            Players = players;
            Amount = amount;
            Street = street;
        }

        public static Pot FindPot(List<Pot> pots, List<Player> players)
        {
            return pots.FirstOrDefault(pot => players.All(player => pot.Players.Contains(player)));
        }
    }

    public interface IPokerCommand
    {
        string CommandText { set; get; }
        void Exec(Table table);
        void Undo(Table table);
    }

    public class PokerCommand : IPokerCommand
    {
        public string CommandText { get; set; }

        public PokerCommand(string commandText)
        {
            CommandText = commandText;
        }

        public virtual void Exec(Table table)
        {
            table.Log("[Do] " + CommandText);
        }

        public virtual void Undo(Table table)
        {
            table.Log("[UnDo] " + CommandText);
        }
    }

    public enum Street
    {
        Unknown,
        Preflop,
        Flop, Turn,
        River
    }

    public class PokerCommands
    {
        #region CollectPots

        public class CollectPots : PokerCommand
        {
            public List<Pot> Pots = new List<Pot>();
            public Street Street;

            public CollectPots(Street street)
                : base("Collect Pots")
            {
                Street = street;
            }

            public override void Exec(Table table)
            {
                while (table.Players.Any(o => o.Bet != 0))
                {
                    decimal amount = 0;
                    decimal bet = table.Players.Where(p => p.Bet > 0).Min(p => p.Bet);
                    var players = new List<Player>();
                    foreach (var player in table.Players)
                    {
                        if (player.Bet >= bet)
                        {
                            player.Bet -= bet;
                            amount += bet;
                            players.Add(player);
                        }
                    }
                    var pot = new Pot(players, amount, Street);
                    Pots.Add(pot);
                }

                table.Pots.AddRange(Pots);
            }

            public override void Undo(Table table)
            {
                foreach (var pot in Pots)
                {
                    decimal amountPerPlayer = pot.Amount / pot.Players.Count;
                    foreach (var player in pot.Players)
                    {
                        player.Bet += amountPerPlayer;
                    }
                }

                foreach (var pot in Pots)
                {
                    table.Pots.Remove(pot);
                }
                Pots.Clear();
            }
        }

        #endregion

        #region FinalizePots

        public class FinalizePots : PokerCommand
        {
            public List<Pot> Pots = new List<Pot>();
            public List<Pot> PotsStreetByStreet = new List<Pot>();

            public FinalizePots()
                : base("Finalize Pots")
            {
            }

            public override void Exec(Table table)
            {
                var finalizedPots = new List<Pot>();
                var finalizedPotsSbs = new List<Pot>();
                foreach (var pot in table.Pots)
                {
                    var findPot = finalizedPots.FirstOrDefault(o => pot.Players.Where(player => player.IsInPlay).All(player => o.Players.Contains(player)) && o.Players.Count == pot.Players.Count(player => player.IsInPlay));
                    var findPotSbs = finalizedPotsSbs.FirstOrDefault(o => pot.Players.Where(player => player.IsInPlay).All(player => o.Players.Contains(player)) && o.Players.Count == pot.Players.Count(player => player.IsInPlay) && o.Street == pot.Street);
                    if (findPot == null)
                    {
                        findPot = new Pot(pot.Players.Where(player => player.IsInPlay).ToList(), 0, Street.Unknown);
                        finalizedPots.Add(findPot);
                    }
                    if (findPotSbs == null)
                    {
                        findPotSbs = new Pot(pot.Players.Where(player => player.IsInPlay).ToList(), 0, pot.Street);
                        finalizedPotsSbs.Add(findPotSbs);
                    }
                    findPot.Amount += pot.Amount;
                    findPotSbs.Amount += pot.Amount;
                }

                Pots = table.Pots;
                PotsStreetByStreet = table.PotsStreetByStreet;
                table.Pots = finalizedPots;
                table.PotsStreetByStreet = finalizedPotsSbs;
                table.TotalPotBeforeCollection = Pots.Sum(o => o.Amount);
            }

            public override void Undo(Table table)
            {
                table.Pots = Pots;
                table.PotsStreetByStreet = PotsStreetByStreet;
                table.TotalPotBeforeCollection = 0;
            }
        }

        #endregion

        #region Flop

        public class Flop : PokerCommand
        {
            public readonly string[] FlopCards;

            public Flop(string commandText, string[] flopCards)
                : base(commandText)
            {
                FlopCards = flopCards;
            }

            public override void Exec(Table table)
            {
                base.Exec(table);
                table.CommunityCards[0] = FlopCards[0];
                table.CommunityCards[1] = FlopCards[1];
                table.CommunityCards[2] = FlopCards[2];
            }

            public override void Undo(Table table)
            {
                base.Undo(table);
                table.CommunityCards[0] = null;
                table.CommunityCards[1] = null;
                table.CommunityCards[2] = null;
            }
        }

        #endregion

        #region Turn

        public class Turn : PokerCommand
        {
            public readonly string TurnCard;

            public Turn(string commandText, string turnCard)
                : base(commandText)
            {
                TurnCard = turnCard;
            }

            public override void Exec(Table table)
            {
                base.Exec(table);
                table.CommunityCards[3] = TurnCard;
            }

            public override void Undo(Table table)
            {
                base.Undo(table);
                table.CommunityCards[3] = null;
            }
        }

        #endregion

        #region River

        public class River : PokerCommand
        {
            public readonly string RiverCard;

            public River(string commandText, string riverCard)
                : base(commandText)
            {
                RiverCard = riverCard;
            }

            public override void Exec(Table table)
            {
                base.Exec(table);
                table.CommunityCards[4] = RiverCard;
            }

            public override void Undo(Table table)
            {
                base.Undo(table);
                table.CommunityCards[4] = null;
            }
        }

        #endregion

        #region Post Small Blind

        public class PostSmallBlind : PokerCommand
        {
            public readonly Player Player;
            public readonly decimal Amount;

            public PostSmallBlind(string commandText, Player player, decimal amount)
                : base(commandText)
            {
                Player = player;
                Amount = amount;
            }

            public override void Exec(Table table)
            {
                base.Exec(table);
                Player.PutChips(Amount);
            }

            public override void Undo(Table table)
            {
                base.Undo(table);
                Player.PutChips(-Amount);
            }
        }

        #endregion

        #region Post Big Blind

        public class PostBigBlind : PokerCommand
        {
            public readonly Player Player;
            public readonly decimal Amount;

            public PostBigBlind(string commandText, Player player, decimal amount)
                : base(commandText)
            {
                Player = player;
                Amount = amount;
            }

            public override void Exec(Table table)
            {
                base.Exec(table);
                Player.PutChips(Amount);
            }

            public override void Undo(Table table)
            {
                base.Undo(table);
                Player.PutChips(-Amount);
            }
        }

        #endregion

        #region Post Ante

        public class PostAnte : PokerCommand
        {
            public readonly Player Player;
            public readonly decimal Amount;

            public PostAnte(string commandText, Player player, decimal amount)
                : base(commandText)
            {
                Player = player;
                Amount = amount;
            }

            public override void Exec(Table table)
            {
                base.Exec(table);
                Player.PutChips(Amount);
            }

            public override void Undo(Table table)
            {
                base.Undo(table);
                Player.PutChips(-Amount);
            }
        }

        #endregion

        #region Dealt To

        public class DealtTo : PokerCommand
        {
            public readonly Player Player;

            public DealtTo(string commandText, Player player)
                : base(commandText)
            {
                Player = player;
            }

            public override void Exec(Table table)
            {
                base.Exec(table);
                Player.ArePocketCardsOpenfaced = true;
            }

            public override void Undo(Table table)
            {
                base.Undo(table);
                Player.ArePocketCardsOpenfaced = false;
            }
        }

        #endregion

        #region Fold

        public class Fold : PokerCommand
        {
            public readonly Player Player;

            public Fold(string commandText, Player player)
                : base(commandText)
            {
                Player = player;
            }

            public override void Exec(Table table)
            {
                base.Exec(table);
                Player.Fold();
            }

            public override void Undo(Table table)
            {
                base.Undo(table);
                Player.UnFold();
            }
        }

        #endregion

        #region Check

        public class Check : PokerCommand
        {
            public readonly Player Player;

            public Check(string commandText, Player player)
                : base(commandText)
            {
                Player = player;
            }

            public override void Exec(Table table)
            {
                base.Exec(table);
            }

            public override void Undo(Table table)
            {
                base.Undo(table);
            }
        }

        #endregion

        #region Call

        public class Call : PokerCommand
        {
            public readonly Player Player;
            public readonly decimal Amount;
            public readonly bool AllIn;

            public Call(string commandText, Player player, decimal amount, bool allIn)
                : base(commandText)
            {
                Player = player;
                Amount = amount;
                AllIn = allIn;
            }

            public override void Exec(Table table)
            {
                base.Exec(table);
                Player.PutChips(Amount);
            }

            public override void Undo(Table table)
            {
                base.Undo(table);
                Player.PutChips(-Amount);
            }
        }

        #endregion

        #region Bet

        public class Bet : PokerCommand
        {
            public readonly Player Player;
            public readonly decimal Amount;
            public readonly bool AllIn;

            public Bet(string commandText, Player player, decimal amount, bool allIn)
                : base(commandText)
            {
                Player = player;
                Amount = amount;
                AllIn = allIn;
            }

            public override void Exec(Table table)
            {
                base.Exec(table);
                Player.PutChips(Amount);
            }

            public override void Undo(Table table)
            {
                base.Undo(table);
                Player.PutChips(-Amount);
            }
        }

        #endregion

        #region Raise

        public class Raise : PokerCommand
        {
            public readonly Player Player;
            public readonly decimal AmountInTotal;
            public readonly bool AllIn;
            public decimal Amount;

            public Raise(string commandText, Player player, decimal amountInTotal, bool allIn)
                : base(commandText)
            {
                Player = player;
                AmountInTotal = amountInTotal;
                AllIn = allIn;
            }

            public override void Exec(Table table)
            {
                base.Exec(table);
                Amount = Player.PutChipsInTotal(AmountInTotal);
            }

            public override void Undo(Table table)
            {
                base.Undo(table);
                Player.PutChips(-Amount);
            }
        }

        #endregion

        #region Uncalled Bet Return

        public class UncalledBetReturn : PokerCommand
        {
            public readonly Player Player;
            public readonly decimal Amount;

            public UncalledBetReturn(string commandText, Player player, decimal amount)
                : base(commandText)
            {
                Player = player;
                Amount = amount;
            }

            public override void Exec(Table table)
            {
                base.Exec(table);
                Player.PutChips(-Amount);
            }

            public override void Undo(Table table)
            {
                base.Undo(table);
                Player.PutChips(Amount);
            }
        }

        #endregion

        #region Shows

        public class Shows : PokerCommand
        {
            public readonly Player Player;
            public readonly string Info;

            public Shows(string commandText, Player player, string info)
                : base(commandText)
            {
                Player = player;
                Info = info;
            }

            public override void Exec(Table table)
            {
                base.Exec(table);
                Player.ArePocketCardsOpenfaced = true;
            }

            public override void Undo(Table table)
            {
                base.Undo(table);
                Player.ArePocketCardsOpenfaced = false;
            }
        }

        #endregion

        #region Collect From Pot

        public class CollectFromPot : PokerCommand
        {
            public readonly Player Player;
            public readonly decimal Amount;

            public CollectFromPot(string commandText, Player player, decimal amount)
                : base(commandText)
            {
                Player = player;
                Amount = amount;
            }

            public override void Exec(Table table)
            {
                base.Exec(table);
                Player.CollectChipsFromPot(Amount);
            }

            public override void Undo(Table table)
            {
                base.Undo(table);
                Player.CollectChipsFromPot(-Amount);
            }
        }

        #endregion
    }
}

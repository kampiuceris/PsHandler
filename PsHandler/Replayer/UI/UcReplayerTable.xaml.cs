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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using PsHandler.PokerMath;

namespace PsHandler.Replayer.UI
{
    /// <summary>
    /// Interaction logic for UcReplayerTable.xaml
    /// </summary>
    public partial class UcReplayerTable : UserControl
    {
        private readonly UcReplayerPlayer[] _ucReplayerPlayers = new UcReplayerPlayer[10];
        private readonly IReplayerBet[] _ucReplayerPlayerBets = new IReplayerBet[10];
        private readonly Image[] _imageCommunityCards = new Image[10];
        public PokerHand PokerHand = new PokerHand();
        public PokerMath.Table Table = new PokerMath.Table();
        public int PreferredSeat = 4; // TODO

        public UcReplayerTable()
        {
            InitializeComponent();

            _ucReplayerPlayers[0] = UcPlayer0;
            _ucReplayerPlayers[1] = UcPlayer1;
            _ucReplayerPlayers[2] = UcPlayer2;
            _ucReplayerPlayers[3] = UcPlayer3;
            _ucReplayerPlayers[4] = UcPlayer4;
            _ucReplayerPlayers[5] = UcPlayer5;
            _ucReplayerPlayers[6] = UcPlayer6;
            _ucReplayerPlayers[7] = UcPlayer7;
            _ucReplayerPlayers[8] = UcPlayer8;
            _ucReplayerPlayers[9] = UcPlayer9;

            _ucReplayerPlayerBets[0] = UcPlayerBet0;
            _ucReplayerPlayerBets[1] = UcPlayerBet1;
            _ucReplayerPlayerBets[2] = UcPlayerBet2;
            _ucReplayerPlayerBets[3] = UcPlayerBet3;
            _ucReplayerPlayerBets[4] = UcPlayerBet4;
            _ucReplayerPlayerBets[5] = UcPlayerBet5;
            _ucReplayerPlayerBets[6] = UcPlayerBet6;
            _ucReplayerPlayerBets[7] = UcPlayerBet7;
            _ucReplayerPlayerBets[8] = UcPlayerBet8;
            _ucReplayerPlayerBets[9] = UcPlayerBet9;

            _imageCommunityCards[0] = Image_CommunityCard0;
            _imageCommunityCards[1] = Image_CommunityCard1;
            _imageCommunityCards[2] = Image_CommunityCard2;
            _imageCommunityCards[3] = Image_CommunityCard3;
            _imageCommunityCards[4] = Image_CommunityCard4;

            CleanTable();
        }

        //

        public void CleanTable()
        {
            for (int i = 0; i < 10; i++)
            {
                SetPlayerVisible(i, false);
                SetButton(-1);
                SetPlayerBet(i, 0);
                SetPot(0);
                SetPotTotal(0);
            }
            for (int i = 0; i < 5; i++)
            {
                SetCommunityCard(i, null);
            }
        }

        private int MapToPreferredSeat(int seat)
        {
            var dealtTo = PokerHand.PokerCommands.OfType<PokerCommands.DealtTo>();
            if (!dealtTo.Any())
            {
                return seat;
            }
            var hero = dealtTo.ElementAt(0).Player;
            var neededOffset = PreferredSeat + 10 - hero.SeatNumber;
            return (seat + neededOffset) % 10;
        }

        private void SetButton(int buttonId)
        {
            if (buttonId < 0)
            {
                Image_Button.Visibility = Visibility.Collapsed;
            }
            else
            {
                Point[] DEFAULT_BUTTON_XY =
                {
                    new Point(475, 67),
                    new Point(592, 91),
                    new Point(633, 161), 
                    new Point(628, 250),
                    new Point(528, 295),
                    new Point(310, 297),
                    new Point(204, 267),
                    new Point(150, 220),
                    new Point(153, 145),
                    new Point(227, 74),
                };
                Image_Button.Margin = new Thickness(DEFAULT_BUTTON_XY[buttonId].X, DEFAULT_BUTTON_XY[buttonId].Y, 0, 0);
                Image_Button.Visibility = Visibility.Visible;
            }
        }

        private void SetPot(decimal amount)
        {
            UcPot_Main.SetAmount(amount);
        }

        private void SetPotTotal(decimal amount)
        {
            var amountStr = ReplayerBet.AmountToString(amount);
            TextBlock_PotTotal.Text = "Pot: " + (string.IsNullOrEmpty(amountStr) ? "0" : amountStr);
        }

        private void SetPlayerVisible(int playerId, bool isVisible)
        {
            _ucReplayerPlayers[playerId].Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SetPlayerBet(int playerId, decimal amount)
        {
            _ucReplayerPlayerBets[playerId].SetAmount(amount);
        }

        private void SetPlayerCards(int playerId, string[] cards)
        {
            if (cards == null)
            {
                _ucReplayerPlayers[playerId].Image_Card0.Source = GetCardBitmapImage(null);
                _ucReplayerPlayers[playerId].Image_Card1.Source = GetCardBitmapImage(null);
            }
            else
            {
                _ucReplayerPlayers[playerId].Image_Card0.Source = GetCardBitmapImage(cards[0]);
                if (cards.Length == 2)
                {
                    _ucReplayerPlayers[playerId].Image_Card1.Source = GetCardBitmapImage(cards[1]);
                }
            }
        }

        private void SetCommunityCard(int cardId, string cardStr)
        {
            if (cardStr == null)
            {
                _imageCommunityCards[cardId].Visibility = Visibility.Collapsed;
            }
            else
            {
                _imageCommunityCards[cardId].Source = GetCardBitmapImage(cardStr);
                _imageCommunityCards[cardId].Visibility = Visibility.Visible;
            }
        }

        private BitmapImage GetCardBitmapImage(string cardStr)
        {
            if (cardStr == null)
            {
                return new BitmapImage(new Uri(string.Format(@"/PsHandler;component/Images/Resources/Replayer/Cards/back.png"), UriKind.Relative));
            }
            else
            {
                return new BitmapImage(new Uri(string.Format(@"/PsHandler;component/Images/Resources/Replayer/Cards/{0}.png", cardStr), UriKind.Relative));
            }
        }

        //

        public void ReplayHand(PokerHand pokerHand)
        {
            CleanTable();
            PokerHand = pokerHand;

            if (PokerHand != null)
            {
                Table.LoadHand(PokerHand);
                GoToPreflop();
            }
        }

        private void VisualizeHandState()
        {
            // players
            for (int i = 0; i < PokerHand.Seats.Length; i++)
            {
                var mappedId = MapToPreferredSeat(i);

                var player = PokerHand.Seats[i];
                if (player == null)
                {
                    SetPlayerVisible(mappedId, false);
                }
                else
                {
                    SetPlayerVisible(mappedId, true);
                    _ucReplayerPlayers[mappedId].SetPlayerName(player.PlayerName);
                    _ucReplayerPlayers[mappedId].SetPlayerStack(player.Stack);
                    _ucReplayerPlayerBets[mappedId].SetAmount(player.Bet);

                    _ucReplayerPlayers[mappedId].Image_Card0.Visibility = player.IsInPlay ? Visibility.Visible : Visibility.Collapsed;
                    _ucReplayerPlayers[mappedId].Image_Card1.Visibility = player.IsInPlay ? Visibility.Visible : Visibility.Collapsed;

                    if (player.IsHero)
                    {
                        SetPlayerCards(mappedId, Table.UnDo.Any(a => a is PokerCommands.DealtTo) ? player.PocketCards : null);
                    }
                    else
                    {
                        SetPlayerCards(mappedId, Table.UnDo.OfType<PokerCommands.Shows>().Any(a => a.Player == player) ? player.PocketCards : null);
                    }
                }
            }

            // button
            SetButton(MapToPreferredSeat(PokerHand.ButtonSeat));

            // pot + pot total
            var potsTotal = Table.Pots.Sum(a => a.Amount);
            var potsAlreadyCollected = Table.UnDo.OfType<PokerCommands.CollectFromPot>().Sum(a => a.Amount);
            SetPot(potsTotal - potsAlreadyCollected);
            SetPotTotal(potsTotal - potsAlreadyCollected + _ucReplayerPlayerBets.Sum(a => a.GetAmount()));

            // community cards
            for (int i = 0; i < 5; i++) SetCommunityCard(i, null);
            foreach (var flop in Table.UnDo.OfType<PokerCommands.Flop>()) for (int i = 0; i < flop.FlopCards.Length; i++) SetCommunityCard(i, flop.FlopCards[i]);
            foreach (var turn in Table.UnDo.OfType<PokerCommands.Turn>()) SetCommunityCard(3, turn.TurnCard);
            foreach (var river in Table.UnDo.OfType<PokerCommands.River>()) SetCommunityCard(4, river.RiverCard);

            // next action glow
            foreach (var ucReplayerPlayer in _ucReplayerPlayers) ucReplayerPlayer.SetActionGlow(false);
            if (Table.ToDo.Any())
            {
                var pokerCommand = Table.ToDo.First();
                if (pokerCommand is PokerCommands.Fold) _ucReplayerPlayers[MapToPreferredSeat(((PokerCommands.Fold)pokerCommand).Player.SeatNumber)].SetActionGlow(true);
                if (pokerCommand is PokerCommands.Check) _ucReplayerPlayers[MapToPreferredSeat(((PokerCommands.Check)pokerCommand).Player.SeatNumber)].SetActionGlow(true);
                if (pokerCommand is PokerCommands.Call) _ucReplayerPlayers[MapToPreferredSeat(((PokerCommands.Call)pokerCommand).Player.SeatNumber)].SetActionGlow(true);
                if (pokerCommand is PokerCommands.Raise) _ucReplayerPlayers[MapToPreferredSeat(((PokerCommands.Raise)pokerCommand).Player.SeatNumber)].SetActionGlow(true);
                if (pokerCommand is PokerCommands.Bet) _ucReplayerPlayers[MapToPreferredSeat(((PokerCommands.Bet)pokerCommand).Player.SeatNumber)].SetActionGlow(true);
            }

            // display info
            //StringBuilder sb = new StringBuilder();
            //for (int i = _table.UnDo.Count - 1; i >= 0; i--)
            //{
            //    sb.AppendLine(_table.UnDo[i].CommandText);
            //}
            //if (App.WindowReplayer != null)
            //{
            //    App.WindowReplayer.TextBlock_Chat.Text = sb.ToString();
            //}
        }

        private void PostCommandVisualFixes(PokerCommand pokerCommand)
        {
            if (pokerCommand == null)
            {
                return;
            }

            if (pokerCommand is PokerCommands.Fold)
            {
                var fold = pokerCommand as PokerCommands.Fold;
                if (fold.FoldAndShow)
                {
                    SetPlayerCards(MapToPreferredSeat(fold.Player.SeatNumber), fold.Player.PocketCards);
                    _ucReplayerPlayers[MapToPreferredSeat(fold.Player.SeatNumber)].Image_Card0.Visibility = Visibility.Visible;
                    _ucReplayerPlayers[MapToPreferredSeat(fold.Player.SeatNumber)].Image_Card1.Visibility = Visibility.Visible;
                }
            }
        }

        public void DoCommand()
        {
            while (Table.ToDo.Any() && (Table.ToDo[0] is PokerCommands.CollectPots || Table.ToDo[0] is PokerCommands.FinalizePots))
            {
                Table.ToDoCommand();
            }
            Table.ToDoCommand();
            VisualizeHandState();
            PostCommandVisualFixes(Table.UnDo.FirstOrDefault());

            if (App.WindowReplayer != null)
            {
                var pokerCommand = Table.UnDo.FirstOrDefault(a => !(a is PokerCommands.CollectPots) && !(a is PokerCommands.FinalizePots));
                if (pokerCommand != null)
                {
                    App.WindowReplayer.TextBlock_Chat.Text = string.Format("[Do] {0}", pokerCommand.CommandText);
                }
            }
        }

        public void UndoCommand()
        {
            Table.UnDoCommand();
            while (Table.UnDo.Any() && (Table.UnDo[0] is PokerCommands.CollectPots || Table.UnDo[0] is PokerCommands.FinalizePots))
            {
                Table.UnDoCommand();
            }
            VisualizeHandState();
            PostCommandVisualFixes(Table.UnDo.FirstOrDefault());

            if (App.WindowReplayer != null)
            {
                var pokerCommand = Table.ToDo.FirstOrDefault(a => !(a is PokerCommands.CollectPots) && !(a is PokerCommands.FinalizePots));
                if (pokerCommand != null)
                {
                    App.WindowReplayer.TextBlock_Chat.Text = string.Format("[Undo] {0}", pokerCommand.CommandText);
                }
            }
        }

        public void DoCommandAll()
        {
            while (Table.ToDo.Any())
            {
                DoCommand();
            }
        }

        public void UndoCommandAll()
        {
            while (Table.UnDo.Any())
            {
                UndoCommand();
            }
        }

        public void GoToPreflop()
        {
            UndoCommandAll();
            while (Table.ToDo.Any())
            {
                var pokerCommand = Table.ToDo.First();
                if (pokerCommand is PokerCommands.Fold || pokerCommand is PokerCommands.Call || pokerCommand is PokerCommands.Raise)
                {
                    break;
                }
                DoCommand();
            }
        }

        public void GoToFlop()
        {
            UndoCommandAll();
            while (Table.ToDo.Any())
            {
                DoCommand();
                if (Table.UnDo.Any() && Table.UnDo.First() is PokerCommands.Flop)
                {
                    break;
                }
            }
        }

        public void GoToTurn()
        {
            UndoCommandAll();
            while (Table.ToDo.Any())
            {
                DoCommand();
                if (Table.UnDo.Any() && Table.UnDo.First() is PokerCommands.Turn)
                {
                    break;
                }
            }
        }

        public void GoToRiver()
        {
            UndoCommandAll();
            while (Table.ToDo.Any())
            {
                DoCommand();
                if (Table.UnDo.Any() && Table.UnDo.First() is PokerCommands.River)
                {
                    break;
                }
            }
        }
    }
}

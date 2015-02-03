using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        private PokerHand _pokerHand = new PokerHand();
        public PokerMath.Table _table = new PokerMath.Table();
        public int PreferredSeat = 4;

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

            Loaded += (sender, args) => ReplayHand(PokerData.FromText(File.ReadAllText(@"C:\Users\WinWork\Desktop\test.txt")).PokerHands[0]); //TODO
        }

        //

        private void CleanTable()
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
            var dealtTo = _pokerHand.PokerCommands.OfType<PokerCommands.DealtTo>();
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
            _pokerHand = pokerHand;
            _table.LoadHand(_pokerHand);
            _table.ToDoCommandsBeginning();
            VisualizeHandState();
        }

        private void VisualizeHandState()
        {
            // players
            for (int i = 0; i < _pokerHand.Seats.Length; i++)
            {
                var mappedId = MapToPreferredSeat(i);

                var player = _pokerHand.Seats[i];
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
                        SetPlayerCards(mappedId, _table.UnDo.Any(a => a is PokerCommands.DealtTo) ? player.PocketCards : null);
                    }
                    else
                    {
                        SetPlayerCards(mappedId, _table.UnDo.OfType<PokerCommands.Shows>().Any(a => a.Player == player) ? player.PocketCards : null);
                    }
                }
            }

            // button
            SetButton(MapToPreferredSeat(_pokerHand.ButtonSeat));

            // pot + pot total
            var potsTotal = _table.Pots.Sum(a => a.Amount);
            var potsAlreadyCollected = _table.UnDo.OfType<PokerCommands.CollectFromPot>().Sum(a => a.Amount);
            SetPot(potsTotal - potsAlreadyCollected);
            SetPotTotal(potsTotal - potsAlreadyCollected + _ucReplayerPlayerBets.Sum(a => a.GetAmount()));

            // community cards
            for (int i = 0; i < 5; i++) SetCommunityCard(i, null);
            foreach (var flop in _table.UnDo.OfType<PokerCommands.Flop>()) for (int i = 0; i < flop.FlopCards.Length; i++) SetCommunityCard(i, flop.FlopCards[i]);
            foreach (var turn in _table.UnDo.OfType<PokerCommands.Turn>()) SetCommunityCard(3, turn.TurnCard);
            foreach (var river in _table.UnDo.OfType<PokerCommands.River>()) SetCommunityCard(4, river.RiverCard);

            // next action glow
            foreach (var ucReplayerPlayer in _ucReplayerPlayers) ucReplayerPlayer.SetActionGlow(false);
            if (_table.ToDo.Any())
            {
                var pokerCommand = _table.ToDo.First();
                if (pokerCommand is PokerCommands.Fold) _ucReplayerPlayers[MapToPreferredSeat(((PokerCommands.Fold)pokerCommand).Player.SeatNumber)].SetActionGlow(true);
                if (pokerCommand is PokerCommands.Check) _ucReplayerPlayers[MapToPreferredSeat(((PokerCommands.Check)pokerCommand).Player.SeatNumber)].SetActionGlow(true);
                if (pokerCommand is PokerCommands.Call) _ucReplayerPlayers[MapToPreferredSeat(((PokerCommands.Call)pokerCommand).Player.SeatNumber)].SetActionGlow(true);
                if (pokerCommand is PokerCommands.Raise) _ucReplayerPlayers[MapToPreferredSeat(((PokerCommands.Raise)pokerCommand).Player.SeatNumber)].SetActionGlow(true);
                if (pokerCommand is PokerCommands.Bet) _ucReplayerPlayers[MapToPreferredSeat(((PokerCommands.Bet)pokerCommand).Player.SeatNumber)].SetActionGlow(true);
            }

            // display info
            StringBuilder sb = new StringBuilder();
            for (int i = _table.UnDo.Count - 1; i >= 0; i--)
            {
                sb.AppendLine(_table.UnDo[i].CommandText);
            }
            if (App.WindowReplayer != null)
            {
                App.WindowReplayer.TextBlock_Chat.Text = sb.ToString();
                App.WindowReplayer.ScrollViewer_Chat.ScrollToBottom();
            }
        }

        public void DoCommand()
        {
            while (_table.ToDo.Any() && (_table.ToDo[0] is PokerCommands.CollectPots || _table.ToDo[0] is PokerCommands.FinalizePots))
            {
                _table.ToDoCommand();
            }
            _table.ToDoCommand();
            VisualizeHandState();
            PostCommandVisualFixes(_table.UnDo.FirstOrDefault());
        }

        public void UndoCommand()
        {
            _table.UnDoCommand();
            while (_table.UnDo.Any() && (_table.UnDo[0] is PokerCommands.CollectPots || _table.UnDo[0] is PokerCommands.FinalizePots))
            {
                _table.UnDoCommand();
            }
            VisualizeHandState();
            PostCommandVisualFixes(_table.UnDo.FirstOrDefault());
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
    }
}

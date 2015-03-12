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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
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
using PsHandler.Annotations;
using PsHandler.Custom;
using PsHandler.Import;
using PsHandler.PokerMath;

namespace PsHandler.UI
{
    /// <summary>
    /// Interaction logic for UCHands.xaml
    /// </summary>
    public partial class UCHands : UserControl, IObservableCollectionHand
    {
        private readonly ObservableCollection<HandInfo> _handsInfo = new ObservableCollection<HandInfo>();

        public UCHands()
        {
            InitializeComponent();
            ListView_Hands.ItemsSource = _handsInfo;

            #region Context Menu

            ContextMenu menu = new ContextMenu();
            MenuItem menuItem;

            menuItem = new MenuItem { Header = "Replay Hand" };
            menuItem.Click += (sender, args) =>
            {
                var selectedItem = ListView_Hands.SelectedItem as HandInfo;
                if (selectedItem != null)
                {
                    App.WindowReplayer.ReplayHand(selectedItem.Hand.HandHistory);
                }
            };
            menu.Items.Add(menuItem);

            menuItem = new MenuItem { Header = "Copy HandHistory to Clipboard" };
            menuItem.Click += (sender, args) =>
            {
                var selectedItem = ListView_Hands.SelectedItem as HandInfo;
                if (selectedItem != null)
                {
                    Methods.SetClipboardText(selectedItem.Hand.HandHistory);
                }
            };
            menu.Items.Add(menuItem);

            ListView_Hands.ContextMenu = menu;

            #endregion

            ListView_Hands.MouseDoubleClick += (sender, args) =>
            {
                var selectedItem = ListView_Hands.SelectedItem as HandInfo;
                if (selectedItem != null)
                {
                    App.WindowReplayer.ReplayHand(selectedItem.Hand.HandHistory);
                }
            };
        }

        public void AddHand(Hand hand)
        {
            Methods.UiInvoke(() =>
            {
                if (_handsInfo.All(a => a.Hand.HandNumber != hand.HandNumber))
                {
                    _handsInfo.Add(new HandInfo(hand));
                }
                GridView_TablesInfo.ResetColumnWidths();
            });
        }

        #region Grid View Header Sort

        private GridViewColumnHeader _lastHeaderClicked = null;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;

        private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    string header = headerClicked.Column.Header as string;
                    Sort(header, direction);

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate = Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate = Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    // Remove arrow from previously sorted header 
                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }


                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(ListView_Hands.ItemsSource ?? ListView_Hands.Items);
            dataView.SortDescriptions.Clear();

            switch (sortBy)
            {
                case "Time":
                    sortBy = "Time";
                    break;
                case "Hand":
                    sortBy = "HandRanking";
                    break;
                case "Net Won":
                    sortBy = "NetWon";
                    break;
                case "BB Won":
                    sortBy = "BBWon";
                    break;
                case "Level":
                    sortBy = "Level";
                    break;
                case "BuyIn":
                    sortBy = "BuyIn";
                    break;
                case "Hand Number":
                    sortBy = "HandNumber";
                    break;
                case "Community Cards":
                    sortBy = "HandNumber";
                    break;
                case "Tournament":
                    sortBy = "TournamentNumber";
                    break;
            }

            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }

        #endregion
    }

    public class HandInfo : INotifyPropertyChanged
    {
        public Hand Hand;

        private string _handNumber;
        private string _tournamentNumber;
        private string _buyIn;
        private string _level;
        private DateTime _time;
        private ImageSource _imageSourceCard0;
        private ImageSource _imageSourceCard1;
        private ImageSource _imageSourceCommunityCard0;
        private ImageSource _imageSourceCommunityCard1;
        private ImageSource _imageSourceCommunityCard2;
        private ImageSource _imageSourceCommunityCard3;
        private ImageSource _imageSourceCommunityCard4;
        private int _handRanking;
        private decimal _netWon;
        private decimal _bbWon;

        public string HandNumber
        {
            get
            {
                return _handNumber;
            }
            set
            {
                _handNumber = value;
                OnPropertyChanged("HandNumber");
            }
        }
        public string TournamentNumber
        {
            get
            {
                return _tournamentNumber;
            }
            set
            {
                _tournamentNumber = value;
                OnPropertyChanged("TournamentNumber");
            }
        }
        public string BuyIn
        {
            get
            {
                return _buyIn;
            }
            set
            {
                _buyIn = value;
                OnPropertyChanged("BuyIn");
            }
        }
        public string Level
        {
            get
            {
                return _level;
            }
            set
            {
                _level = value;
                OnPropertyChanged("Level");
            }
        }
        public DateTime Time
        {
            get
            {
                return _time;
            }
            set
            {
                _time = value;
                OnPropertyChanged("Time");
            }
        }
        public ImageSource ImageSourceCard0
        {
            get
            {
                return _imageSourceCard0;
            }
            set
            {
                _imageSourceCard0 = value;
                OnPropertyChanged("ImageSourceCard0");
            }
        }
        public ImageSource ImageSourceCard1
        {
            get
            {
                return _imageSourceCard1;
            }
            set
            {
                _imageSourceCard1 = value;
                OnPropertyChanged("ImageSourceCard1");
            }
        }
        public ImageSource ImageSourceCommunityCard0
        {
            get
            {
                return _imageSourceCommunityCard0;
            }
            set
            {
                _imageSourceCommunityCard0 = value;
                OnPropertyChanged("ImageSourceCommunityCard0");
            }
        }
        public ImageSource ImageSourceCommunityCard1
        {
            get
            {
                return _imageSourceCommunityCard1;
            }
            set
            {
                _imageSourceCommunityCard1 = value;
                OnPropertyChanged("ImageSourceCommunityCard1");
            }
        }
        public ImageSource ImageSourceCommunityCard2
        {
            get
            {
                return _imageSourceCommunityCard2;
            }
            set
            {
                _imageSourceCommunityCard2 = value;
                OnPropertyChanged("ImageSourceCommunityCard2");
            }
        }
        public ImageSource ImageSourceCommunityCard3
        {
            get
            {
                return _imageSourceCommunityCard3;
            }
            set
            {
                _imageSourceCommunityCard3 = value;
                OnPropertyChanged("ImageSourceCommunityCard3");
            }
        }
        public ImageSource ImageSourceCommunityCard4
        {
            get
            {
                return _imageSourceCommunityCard4;
            }
            set
            {
                _imageSourceCommunityCard4 = value;
                OnPropertyChanged("ImageSourceCommunityCard4");
            }
        }
        public int HandRanking
        {
            get
            {
                return _handRanking;
            }
            set
            {
                _handRanking = value;
                OnPropertyChanged("HandRanking");
            }
        }
        public decimal NetWon
        {
            get
            {
                return _netWon;
            }
            set
            {
                _netWon = value;
                OnPropertyChanged("NetWon");
            }
        }
        public decimal BBWon
        {
            get
            {
                return _bbWon;
            }
            set
            {
                _bbWon = value;
                OnPropertyChanged("BBWon");
            }
        }

        public HandInfo(Hand hand)
        {
            Hand = hand;
            HandNumber = string.Format("{0}", Hand.HandNumber);
            TournamentNumber = Hand.TournamentNumber > 0 ? string.Format("{0}", Hand.TournamentNumber) : "";
            Level = string.Format("{0}/{1}{2}", Hand.LevelSmallBlind, Hand.LevelBigBlind, Hand.LevelAnte > 0 ? string.Format(" ({0})", Hand.LevelAnte) : "");
            BuyIn = Hand.IsTournament ? string.Format("{1}{0}", Hand.TotalBuyIn, PokerEnums.CurrencySigns[(int)Hand.Currency]) : "";

            Time = hand.TimeStampLocal;

            ImageSourceCard0 = new BitmapImage(new Uri(string.Format(@"pack://application:,,,/Images/Resources/Replayer/CardsSmall/blank.png"), UriKind.Absolute));
            ImageSourceCard1 = new BitmapImage(new Uri(string.Format(@"pack://application:,,,/Images/Resources/Replayer/CardsSmall/blank.png"), UriKind.Absolute));
            ImageSourceCommunityCard0 = new BitmapImage(new Uri(string.Format(@"pack://application:,,,/Images/Resources/Replayer/CardsSmall/empty.png"), UriKind.Absolute));
            ImageSourceCommunityCard1 = new BitmapImage(new Uri(string.Format(@"pack://application:,,,/Images/Resources/Replayer/CardsSmall/empty.png"), UriKind.Absolute));
            ImageSourceCommunityCard2 = new BitmapImage(new Uri(string.Format(@"pack://application:,,,/Images/Resources/Replayer/CardsSmall/empty.png"), UriKind.Absolute));
            ImageSourceCommunityCard3 = new BitmapImage(new Uri(string.Format(@"pack://application:,,,/Images/Resources/Replayer/CardsSmall/empty.png"), UriKind.Absolute));
            ImageSourceCommunityCard4 = new BitmapImage(new Uri(string.Format(@"pack://application:,,,/Images/Resources/Replayer/CardsSmall/empty.png"), UriKind.Absolute));
            HandRanking = -1;

            var dealtTo = Hand.PokerCommands.OfType<PokerCommands.DealtTo>();
            if (dealtTo.Count() == 1)
            {
                var hero = dealtTo.First().Player;
                var pocketCards = hero.PocketCards;
                if (pocketCards != null && pocketCards.Length == 2 && pocketCards.All(a => !string.IsNullOrEmpty(a)))
                {
                    ImageSourceCard0 = new BitmapImage(new Uri(string.Format(@"pack://application:,,,/Images/Resources/Replayer/CardsSmall/{0}.png", pocketCards[0]), UriKind.Absolute));
                    ImageSourceCard1 = new BitmapImage(new Uri(string.Format(@"pack://application:,,,/Images/Resources/Replayer/CardsSmall/{0}.png", pocketCards[1]), UriKind.Absolute));
                    if (Array.IndexOf(VALUES, pocketCards[0][0]) < Array.IndexOf(VALUES, pocketCards[1][0]))
                    {
                        var temp = ImageSourceCard0;
                        ImageSourceCard0 = ImageSourceCard1;
                        ImageSourceCard1 = temp;
                    }
                    HandRanking = Array.IndexOf(RANKING_STR, PocketFullToShort(pocketCards));
                }

                //

                NetWon = Hand.StacksAfterHand[Hand.HeroId] - Hand.StacksBeforeHand[Hand.HeroId];
                BBWon = NetWon / Hand.LevelBigBlind;
            }

            var flop = Hand.PokerCommands.OfType<PokerCommands.Flop>();
            if (flop.Count() == 1)
            {
                ImageSourceCommunityCard0 = new BitmapImage(new Uri(string.Format(@"pack://application:,,,/Images/Resources/Replayer/CardsSmall/{0}.png", flop.First().FlopCards[0]), UriKind.Absolute));
                ImageSourceCommunityCard1 = new BitmapImage(new Uri(string.Format(@"pack://application:,,,/Images/Resources/Replayer/CardsSmall/{0}.png", flop.First().FlopCards[1]), UriKind.Absolute));
                ImageSourceCommunityCard2 = new BitmapImage(new Uri(string.Format(@"pack://application:,,,/Images/Resources/Replayer/CardsSmall/{0}.png", flop.First().FlopCards[2]), UriKind.Absolute));

                var turn = Hand.PokerCommands.OfType<PokerCommands.Turn>();
                if (turn.Count() == 1)
                {
                    ImageSourceCommunityCard3 = new BitmapImage(new Uri(string.Format(@"pack://application:,,,/Images/Resources/Replayer/CardsSmall/{0}.png", turn.First().TurnCard), UriKind.Absolute));

                    var river = Hand.PokerCommands.OfType<PokerCommands.River>();
                    if (river.Count() == 1)
                    {
                        ImageSourceCommunityCard4 = new BitmapImage(new Uri(string.Format(@"pack://application:,,,/Images/Resources/Replayer/CardsSmall/{0}.png", river.First().RiverCard), UriKind.Absolute));
                    }
                }
            }
        }

        //

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        //

        private readonly static string[] RANKING_STR =
        {
            "AA", "AKs", "AKo", "AQs", "AQo", "AJs", "AJo", "ATs", "ATo", "A9s", "A9o", "A8s", "A8o", "A7s", "A7o", "A6s", "A6o", "A5s", "A5o", "A4s", "A4o", "A3s", "A3o", "A2s", "A2o", //25   0-24
            "KK", "KQs", "KQo", "KJs", "KJo", "KTs", "KTo", "K9s", "K9o", "K8s", "K8o", "K7s", "K7o", "K6s", "K6o", "K5s", "K5o", "K4s", "K4o", "K3s", "K3o", "K2s", "K2o", //23   25-47
            "QQ", "QJs", "QJo", "QTs", "QTo", "Q9s", "Q9o", "Q8s", "Q8o", "Q7s", "Q7o", "Q6s", "Q6o", "Q5s", "Q5o", "Q4s", "Q4o", "Q3s", "Q3o", "Q2s", "Q2o", //21   48-68
            "JJ", "JTs", "JTo", "J9s", "J9o", "J8s", "J8o", "J7s", "J7o", "J6s", "J6o", "J5s", "J5o", "J4s", "J4o", "J3s", "J3o", "J2s", "J2o", //19   69-87
            "TT", "T9s", "T9o", "T8s", "T8o", "T7s", "T7o", "T6s", "T6o", "T5s", "T5o", "T4s", "T4o", "T3s", "T3o", "T2s", "T2o", //17   88-104
            "99", "98s", "98o", "97s", "97o", "96s", "96o", "95s", "95o", "94s", "94o", "93s", "93o", "92s", "92o", //15   105-119
            "88", "87s", "87o", "86s", "86o", "85s", "85o", "84s", "84o", "83s", "83o", "82s", "82o", //13   120-132
            "77", "76s", "76o", "75s", "75o", "74s", "74o", "73s", "73o", "72s", "72o", //11   133-143
            "66", "65s", "65o", "64s", "64o", "63s", "63o", "62s", "62o", //9   144-152
            "55", "54s", "54o", "53s", "53o", "52s", "52o", //7   153-159
            "44", "43s", "43o", "42s", "42o", //5   160-164
            "33", "32s", "32o", //3   165-167
            "22" //1   168
        };

        private readonly static char[] VALUES = { '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A' };

        private string PocketFullToShort(string[] pocketFull)
        {
            var pocketShort = string.Format("{0}{1}", pocketFull[0][0], pocketFull[1][0]);
            if (pocketShort[0] == pocketShort[1])
            {
                return pocketShort;
            }
            if (Array.IndexOf(VALUES, pocketShort[0]) < Array.IndexOf(VALUES, pocketShort[1]))
            {
                pocketShort = string.Format("{1}{0}", pocketShort[0], pocketShort[1]);
            }
            if (pocketFull[0][1] != pocketFull[1][1])
            {
                pocketShort += "o";
            }
            else
            {
                pocketShort += "s";
            }
            return pocketShort;
        }
    }
}

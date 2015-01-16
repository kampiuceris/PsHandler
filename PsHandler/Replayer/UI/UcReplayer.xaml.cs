using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using PsHandler.PokerMath;

namespace PsHandler.Replayer.UI
{
    /// <summary>
    /// Interaction logic for UcReplayer.xaml
    /// </summary>
    public partial class UcReplayer : UserControl
    {
        SolidColorBrush RectanglePlayerStroke = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EE252525"));
        LinearGradientBrush RectanglePlayerFill = new LinearGradientBrush
        {
            StartPoint = new Point(0.5, 0),
            EndPoint = new Point(0.5, 1),
            GradientStops = new GradientStopCollection
            {
                new GradientStop{ Color = (Color)ColorConverter.ConvertFromString("#EE4F4F4F"), Offset = 0 },
                new GradientStop{ Color = (Color)ColorConverter.ConvertFromString("#EE292929"), Offset = 1 },
            }
        };
        UcButton UcButton;
        int ButtonSeat = 0;
        Viewbox ViewboxPot;
        TextBlock TextBlockPot;
        Rectangle[] RectanglesPlayers;
        Viewbox[] ViewboxesPlayerNames;
        TextBlock[] TextBlocksPlayerNames;
        Viewbox[] ViewboxesPlayerStacks;
        TextBlock[] TextBlocksPlayerStacks;
        Viewbox[] ViewboxesPlayerBets;
        TextBlock[] TextBlocksPlayerBets;
        Canvas[] CanvasPlayerBets;
        int CurrentChipsSize;
        //
        PokerMath.Table Table = new PokerMath.Table();
        PokerMath.PokerHand PokerHand = new PokerHand();
        int PreferredSeat = 4;
        //

        public UcReplayer()
        {
            InitializeComponent();
            Init();
            SizeChanged += (sender, args) => Update();
            Loaded += (sender, args) => Update();
            Loaded += (sender, args) => ReplayHand(PokerMath.PokerData.FromText(System.IO.File.ReadAllText(@"C:\Users\WinWork\Desktop\test.txt")).PokerHands[0]);
        }

        private void Init()
        {
            // button
            UcButton = new UcButton();
            CanvasTable.Children.Add(UcButton);

            // player rectangle
            RectanglesPlayers = new Rectangle[10];
            for (int i = 0; i < 10; i++)
            {
                var r = new Rectangle
                {
                    Fill = RectanglePlayerFill,
                    Stroke = RectanglePlayerStroke,
                };
                RectanglesPlayers[i] = r;
                CanvasTable.Children.Add(r);
            }

            // player name + player stack
            ViewboxesPlayerNames = new Viewbox[10];
            TextBlocksPlayerNames = new TextBlock[10];
            ViewboxesPlayerStacks = new Viewbox[10];
            TextBlocksPlayerStacks = new TextBlock[10];
            ViewboxesPlayerBets = new Viewbox[10];
            TextBlocksPlayerBets = new TextBlock[10];
            for (int i = 0; i < 10; i++)
            {
                // player name
                var tb = new TextBlock
                {
                    Foreground = Brushes.White,
                };
                TextBlocksPlayerNames[i] = tb;
                var vb = new Viewbox
                {
                    Child = tb
                };
                ViewboxesPlayerNames[i] = vb;
                CanvasTable.Children.Add(vb);

                // player stack
                tb = new TextBlock
                {
                    Foreground = Brushes.GreenYellow,
                };
                TextBlocksPlayerStacks[i] = tb;
                vb = new Viewbox
                {
                    Child = tb
                };
                ViewboxesPlayerStacks[i] = vb;
                CanvasTable.Children.Add(vb);

            }

            // player bets
            int[] customZOrder = { 9, 0, 8, 1, 7, 2, 6, 3, 5, 4 };
            CanvasPlayerBets = new Canvas[10];
            for (int i = 0; i < 10; i++)
            {
                // chips
                Canvas c = new Canvas();
                CanvasPlayerBets[customZOrder[i]] = c;
                CanvasTable.Children.Add(c);

                // player bet
                var tb = new TextBlock
                {
                    Foreground = Brushes.Honeydew,
                };
                TextBlocksPlayerBets[customZOrder[i]] = tb;
                var vb = new Viewbox
                {
                    Child = tb
                };
                ViewboxesPlayerBets[customZOrder[i]] = vb;
                CanvasTable.Children.Add(vb);
            }

            // pot
            TextBlockPot = new TextBlock
            {
                Foreground = Brushes.GreenYellow,
            };
            ViewboxPot = new Viewbox
            {
                Child = TextBlockPot
            };
            CanvasTable.Children.Add(ViewboxPot);
        }

        //

        private void CleanTable()
        {
            for (int i = 0; i < 10; i++)
            {
                SetPlayerVisible(i, false);
                UcButton.Visibility = Visibility.Collapsed;
                SetPlayerBet(i, 0);
            }
        }

        public void ReplayHand(PokerHand pokerHand)
        {
            CleanTable();
            PokerHand = pokerHand;
            Table.LoadHand(pokerHand);
            Table.ToDoCommandsBeginning();
            VisualizeHandState();
        }

        private void VisualizeHandState()
        {
            for (int i = 0; i < PokerHand.Seats.Length; i++)
            {
                var player = PokerHand.Seats[i];
                if (player == null)
                {
                    SetPlayerVisible(MapToPreferredSeat(i), false);
                }
                else
                {
                    ButtonSeat = PokerHand.ButtonSeat;
                    UcButton.Visibility = ButtonSeat > 0 ? Visibility.Visible : Visibility.Collapsed;
                    SetPlayerVisible(MapToPreferredSeat(i), true);
                    TextBlocksPlayerNames[MapToPreferredSeat(i)].Text = player.PlayerName;
                    TextBlocksPlayerStacks[MapToPreferredSeat(i)].Text = string.Format("{0}", player.Stack);
                    SetPlayerBet(MapToPreferredSeat(i), player.Bet);
                    TextBlockPot.Text = string.Format("Pot: {0:0.##}", Table.TotalPot);
                }
            }

            Update();
        }

        private void SetPlayerVisible(int seat, bool isVisible)
        {
            RectanglesPlayers[seat].Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            ViewboxesPlayerNames[seat].Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            ViewboxesPlayerStacks[seat].Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            ViewboxesPlayerBets[seat].Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            CanvasPlayerBets[seat].Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public void SetPlayerBet(int seat, decimal value, int size = -1)
        {
            if (size == -1)
            {
                size = Converter.GetSize(ActualWidth);
            }
            CanvasPlayerBets[seat].Children.Clear();
            foreach (var ucChip in UcChip.GetUcChips(value, size))
            {
                CanvasPlayerBets[seat].Children.Add(ucChip);
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
            var neededOffset = PreferredSeat - hero.SeatNumber;
            return (seat + neededOffset) % 10;
        }

        //

        private void Update()
        {
            try
            {
                double width = ActualWidth, height = ActualHeight;
                UpdatePlayerRectangle(width, height);
                UpdatePlayerNameStack(width, height);
                UpdatePlayerBets(width, height);
                UpdateButton(width, height);
                UpdatePot(width, height);
            }
            catch
            {
            }
        }

        private void UpdatePlayerRectangle(double width, double height)
        {
            for (int i = 0; i < 10; i++)
            {
                var r = RectanglesPlayers[i];
                var playerSize = Converter.GetPlayerSize(width, height);
                r.Width = playerSize.X;
                r.Height = playerSize.Y;
                var playerXY = Converter.GetPlayerXY(width, height, i);
                Canvas.SetLeft(r, playerXY.X);
                Canvas.SetTop(r, playerXY.Y);
                r.RadiusX = Converter.GetPlayerRectangleRadiusX(width);
                r.RadiusY = Converter.GetPlayerRectangleRadiusY(width);
            }
        }

        private void UpdatePlayerNameStack(double width, double height)
        {
            for (int i = 0; i < 10; i++)
            {
                var vn = ViewboxesPlayerNames[i];
                var vs = ViewboxesPlayerStacks[i];
                var playerTextSize = Converter.GetPlayerTextSize(width, height);
                vn.Width = playerTextSize.X;
                vn.Height = playerTextSize.Y / 2;
                vs.Width = playerTextSize.X;
                vs.Height = playerTextSize.Y / 2;
                var playerXY = Converter.GetPlayerXY(width, height, i);
                Canvas.SetLeft(vn, playerXY.X);
                Canvas.SetTop(vn, playerXY.Y);
                Canvas.SetLeft(vs, playerXY.X);
                Canvas.SetTop(vs, playerXY.Y + vn.Height);
            }
        }

        private void UpdatePlayerBets(double width, double height)
        {
            int size = Converter.GetSize(width);
            double gap = Converter.GetChipsGap(size);

            for (int i = 0; i < 10; i++)
            {
                decimal totalAmount = CanvasPlayerBets[i].Children.OfType<UcChip>().Sum(a => a.Value);
                // ensure chips size
                if (CurrentChipsSize != size)
                {
                    SetPlayerBet(i, totalAmount, size);
                }
                // chips
                for (int j = 0; j < CanvasPlayerBets[i].Children.Count; j++)
                {
                    Canvas.SetLeft(CanvasPlayerBets[i].Children[j], 0);
                    Canvas.SetTop(CanvasPlayerBets[i].Children[j], (CanvasPlayerBets[i].Children.Count - j) * gap);
                }
                Point canvasSize = new Point(Converter.DEFAULT_CHIPS_SIZES[size].X, Converter.DEFAULT_CHIPS_SIZES[size].Y + (CanvasPlayerBets[i].Children.Count - 1) * gap);
                CanvasPlayerBets[i].Width = canvasSize.X;
                CanvasPlayerBets[i].Height = canvasSize.Y;
                Point betXy = Converter.GetBetXY(width, height, i);
                Point canvasXY = new Point(betXy.X - CanvasPlayerBets[i].Width / 2, betXy.Y - CanvasPlayerBets[i].Height);
                Canvas.SetLeft(CanvasPlayerBets[i], canvasXY.X);
                Canvas.SetTop(CanvasPlayerBets[i], canvasXY.Y);

                // text
                TextBlocksPlayerBets[i].Text = string.Format("{0:0.##}", totalAmount);
                ViewboxesPlayerBets[i].ToolTip = TextBlocksPlayerBets[i].Text;
                ViewboxesPlayerBets[i].Height = Converter.DEFAULT_CHIPS_SIZES[size].Y * 0.8;
                ViewboxesPlayerBets[i].UpdateLayout();
                Canvas.SetTop(ViewboxesPlayerBets[i], betXy.Y - Converter.DEFAULT_CHIPS_SIZES[size].Y * 0.65);
                if (i < 5)
                {
                    Canvas.SetLeft(ViewboxesPlayerBets[i], betXy.X - Converter.DEFAULT_CHIPS_SIZES[size].X * 0.7 - ViewboxesPlayerBets[i].ActualWidth);
                }
                else
                {
                    Canvas.SetLeft(ViewboxesPlayerBets[i], betXy.X + Converter.DEFAULT_CHIPS_SIZES[size].X * 0.7);
                }
                ViewboxesPlayerBets[i].Visibility = totalAmount > 0 ? Visibility.Visible : Visibility.Collapsed;
            }

            CurrentChipsSize = size;
        }

        private void UpdateButton(double width, double height)
        {
            int size = Converter.GetSize(width);
            if (UcButton.Size != size)
            {
                UcButton.Children.Clear();
                UcButton.Children.Add(UcButton.GetButtonImage(size));
                UcButton.Width = Converter.DEFAULT_CHIPS_SIZES[size].X;
                UcButton.Height = Converter.DEFAULT_CHIPS_SIZES[size].Y;
            }

            Point buttonXy = Converter.GetButtonXY(width, height, ButtonSeat);
            Canvas.SetLeft(UcButton, buttonXy.X - Converter.DEFAULT_CHIPS_SIZES[size].X / 2);
            Canvas.SetTop(UcButton, buttonXy.Y - Converter.DEFAULT_CHIPS_SIZES[size].Y / 2);
        }

        private void UpdatePot(double width, double height)
        {
            Point potXy = Converter.GetPotXY(width, height);
            ViewboxPot.Height = height * 0.04;
            ViewboxPot.UpdateLayout();
            Canvas.SetLeft(ViewboxPot, potXy.X - ViewboxPot.ActualWidth / 2);
            Canvas.SetTop(ViewboxPot, potXy.Y - ViewboxPot.ActualHeight / 2);
        }
        //

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Table.UnDoCommand();
            VisualizeHandState();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Table.ToDoCommand();
            VisualizeHandState();
        }
    }
}

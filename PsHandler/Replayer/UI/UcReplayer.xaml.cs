using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

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
        Rectangle[] RectanglesPlayers = new Rectangle[0];
        Viewbox[] ViewboxesPlayerNames = new Viewbox[0];
        TextBlock[] TextBlocksPlayerNames = new TextBlock[0];
        Viewbox[] ViewboxesPlayerStacks = new Viewbox[0];
        TextBlock[] TextBlocksPlayerStacks = new TextBlock[0];
        Canvas[] CanvasPlayerBets = new Canvas[0];
        int CurrentChipsSize = 0;

        //

        public UcReplayer()
        {
            InitializeComponent();
            Init();
            SizeChanged += (sender, args) => Update();
            Loaded += (sender, args) => Update();
        }

        private void Init()
        {
            // player rectangle
            RectanglesPlayers = new Rectangle[10];
            for (int i = 0; i < 10; i++)
            {

                Rectangle r = new Rectangle
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
            for (int i = 0; i < 10; i++)
            {
                // player name
                TextBlock tb = new TextBlock
                {
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.Bold,
                    Text = "Player " + i, //TODO
                };
                TextBlocksPlayerNames[i] = tb;
                Viewbox vb = new Viewbox
                {
                    Child = tb
                };
                ViewboxesPlayerNames[i] = vb;
                CanvasTable.Children.Add(vb);
                // player stack
                tb = new TextBlock
                {
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.Bold,
                    Text = 1500.ToString(), //TODO
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
            CanvasPlayerBets = new Canvas[10];
            for (int i = 0; i < 10; i++)
            {
                Canvas c = new Canvas();
                CanvasPlayerBets[i] = c;
                CanvasTable.Children.Add(c);
                SetBet(i, 25 * i + 25, 0); //TODO
            }
        }

        //

        public void SetBet(int seat, decimal value, int size)
        {
            CanvasPlayerBets[seat].Children.Clear();
            foreach (var ucChip in UcChip.GetUcChips(value, size))
            {
                CanvasPlayerBets[seat].Children.Add(ucChip);
            }
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
            EnsureChipsSize(width, height);

            double gap = Converter.GetChipsGap(size);

            for (int i = 0; i < 10; i++)
            {
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
            }
        }

        private void EnsureChipsSize(double width, double height)
        {
            int size = Converter.GetSize(width);
            if (CurrentChipsSize != size)
            {
                for (int i = 0; i < 10; i++)
                {
                    decimal totalAmount = CanvasPlayerBets[i].Children.OfType<UcChip>().Sum(a => a.Value);
                    SetBet(i, totalAmount, size);
                }
            }
        }
    }
}

using System.Drawing;
using Point = System.Windows.Point;

namespace PsHandler
{
    public interface PokerStarsTheme
    {
        Point ButtonTimer { get; }
        Point ButtonImBack { get; }
        Point ButtonReplay { get; }
        Bmp BmpButtonImBack { get; }
        double BmpButtonImBackRed { get; }
        double BmpButtonImBackGreen { get; }
        double BmpButtonImBackBlue { get; }
        double ButtonImBackX { get; }
        double ButtonImBackY { get; }
        double ButtonImBackWidth { get; }
        double ButtonImBackHeight { get; }
        double MaxDifferenceR { get; }
        double MaxDifferenceG { get; }
        double MaxDifferenceB { get; }
    }

    public class PokerStarsThemes
    {
        public const double WIDTH = 792;
        public const double HEIGHT = 546;

        public class Unknown : PokerStarsTheme
        {
            public Point ButtonTimer { get { return new Point(0 / WIDTH, 0 / HEIGHT); } }
            public Point ButtonImBack { get { return new Point(0 / WIDTH, 0 / HEIGHT); } }
            public Point ButtonReplay { get { return new Point(0 / WIDTH, 0 / HEIGHT); } }
            private readonly Bmp _bmpButtonImBack;
            private readonly double _bmpButtonImBackRed;
            private readonly double _bmpButtonImBackGreen;
            private readonly double _bmpButtonImBackBlue;
            public Bmp BmpButtonImBack { get { return _bmpButtonImBack; } }
            public double BmpButtonImBackRed { get { return _bmpButtonImBackRed; } }
            public double BmpButtonImBackGreen { get { return _bmpButtonImBackGreen; } }
            public double BmpButtonImBackBlue { get { return _bmpButtonImBackBlue; } }
            public double ButtonImBackX { get { return 0 / WIDTH; } }
            public double ButtonImBackY { get { return 0 / HEIGHT; } }
            public double ButtonImBackWidth { get { return 1; } }
            public double ButtonImBackHeight { get { return 1; } }
            public double MaxDifferenceR { get { return 0; } }
            public double MaxDifferenceG { get { return 0; } }
            public double MaxDifferenceB { get { return 0; } }

            public Unknown()
            {
                _bmpButtonImBack = new Bmp(10, 10);
                for (int y = 0; y < 10; y++)
                {
                    for (int x = 0; x < 10; x++)
                    {
                        _bmpButtonImBack.SetPixelA(x, y, 255);
                        _bmpButtonImBack.SetPixelR(x, y, 255);
                        _bmpButtonImBack.SetPixelG(x, y, 255);
                        _bmpButtonImBack.SetPixelB(x, y, 255);
                    }
                }
                Handler.AverageColor(_bmpButtonImBack, new System.Drawing.Rectangle(0, 0, _bmpButtonImBack.Width, _bmpButtonImBack.Height), out _bmpButtonImBackRed, out _bmpButtonImBackGreen, out _bmpButtonImBackBlue);
            }

            public override string ToString() { return "Unknown"; }
        }

        public class Nova : PokerStarsTheme
        {
            public Point ButtonTimer { get { return new Point(735 / WIDTH, 425 / HEIGHT); } }
            public Point ButtonImBack { get { return new Point(700 / WIDTH, 490 / HEIGHT); } }
            public Point ButtonReplay { get { return new Point(15 / WIDTH, 11 / HEIGHT); } }
            private readonly Bmp _bmpButtonImBack;
            private readonly double _bmpButtonImBackRed;
            private readonly double _bmpButtonImBackGreen;
            private readonly double _bmpButtonImBackBlue;
            public Bmp BmpButtonImBack { get { return _bmpButtonImBack; } }
            public double BmpButtonImBackRed { get { return _bmpButtonImBackRed; } }
            public double BmpButtonImBackGreen { get { return _bmpButtonImBackGreen; } }
            public double BmpButtonImBackBlue { get { return _bmpButtonImBackBlue; } }
            public double ButtonImBackX { get { return 611 / WIDTH; } }
            public double ButtonImBackY { get { return 469 / HEIGHT; } }
            public double ButtonImBackWidth { get { return _bmpButtonImBack.Width / WIDTH; } }
            public double ButtonImBackHeight { get { return _bmpButtonImBack.Height / HEIGHT; } }
            public double MaxDifferenceR { get { return 6; } }
            public double MaxDifferenceG { get { return 6; } }
            public double MaxDifferenceB { get { return 6; } }

            public Nova()
            {
                _bmpButtonImBack = new Bmp(new Bitmap(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream("PsHandler.Images.Themes.Nova.imback.png")));
                Handler.AverageColor(_bmpButtonImBack, new System.Drawing.Rectangle(0, 0, _bmpButtonImBack.Width, _bmpButtonImBack.Height), out _bmpButtonImBackRed, out _bmpButtonImBackGreen, out _bmpButtonImBackBlue);
            }

            public override string ToString() { return "Nova"; }
        }

        public class Classic : PokerStarsTheme
        {
            public Point ButtonTimer { get { return new Point(500 / WIDTH, 470 / HEIGHT); } }
            public Point ButtonImBack { get { return new Point(615 / WIDTH, 460 / HEIGHT); } }
            public Point ButtonReplay { get { return new Point(10 / WIDTH, 26 / HEIGHT); } }
            private readonly Bmp _bmpButtonImBack;
            private readonly double _bmpButtonImBackRed;
            private readonly double _bmpButtonImBackGreen;
            private readonly double _bmpButtonImBackBlue;
            public Bmp BmpButtonImBack { get { return _bmpButtonImBack; } }
            public double BmpButtonImBackRed { get { return _bmpButtonImBackRed; } }
            public double BmpButtonImBackGreen { get { return _bmpButtonImBackGreen; } }
            public double BmpButtonImBackBlue { get { return _bmpButtonImBackBlue; } }
            public double ButtonImBackX { get { return 553 / WIDTH; } }
            public double ButtonImBackY { get { return 452 / HEIGHT; } }
            public double ButtonImBackWidth { get { return _bmpButtonImBack.Width / WIDTH; } }
            public double ButtonImBackHeight { get { return _bmpButtonImBack.Height / HEIGHT; } }
            public double MaxDifferenceR { get { return 8; } }
            public double MaxDifferenceG { get { return 10; } }
            public double MaxDifferenceB { get { return 11; } }

            public Classic()
            {
                _bmpButtonImBack = new Bmp(new Bitmap(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream("PsHandler.Images.Themes.Classic.imback.png")));
                Handler.AverageColor(_bmpButtonImBack, new System.Drawing.Rectangle(0, 0, _bmpButtonImBack.Width, _bmpButtonImBack.Height), out _bmpButtonImBackRed, out _bmpButtonImBackGreen, out _bmpButtonImBackBlue);
            }

            public override string ToString() { return "Classic"; }
        }

        public class Black : PokerStarsTheme
        {
            public Point ButtonTimer { get { return new Point(442 / WIDTH, 450 / HEIGHT); } }
            public Point ButtonImBack { get { return new Point(600 / WIDTH, 455 / HEIGHT); } }
            public Point ButtonReplay { get { return new Point(117 / WIDTH, 13 / HEIGHT); } }
            private readonly Bmp _bmpButtonImBack;
            private readonly double _bmpButtonImBackRed;
            private readonly double _bmpButtonImBackGreen;
            private readonly double _bmpButtonImBackBlue;
            public Bmp BmpButtonImBack { get { return _bmpButtonImBack; } }
            public double BmpButtonImBackRed { get { return _bmpButtonImBackRed; } }
            public double BmpButtonImBackGreen { get { return _bmpButtonImBackGreen; } }
            public double BmpButtonImBackBlue { get { return _bmpButtonImBackBlue; } }
            public double ButtonImBackX { get { return 526 / WIDTH; } }
            public double ButtonImBackY { get { return 442 / HEIGHT; } }
            public double ButtonImBackWidth { get { return _bmpButtonImBack.Width / WIDTH; } }
            public double ButtonImBackHeight { get { return _bmpButtonImBack.Height / HEIGHT; } }
            public double MaxDifferenceR { get { return 4; } }
            public double MaxDifferenceG { get { return 4; } }
            public double MaxDifferenceB { get { return 4; } }

            public Black()
            {
                _bmpButtonImBack = new Bmp(new Bitmap(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream("PsHandler.Images.Themes.Black.imback.png")));
                Handler.AverageColor(_bmpButtonImBack, new System.Drawing.Rectangle(0, 0, _bmpButtonImBack.Width, _bmpButtonImBack.Height), out _bmpButtonImBackRed, out _bmpButtonImBackGreen, out _bmpButtonImBackBlue);
            }

            public override string ToString() { return "Black"; }
        }

        public class Slick : PokerStarsTheme
        {
            public Point ButtonTimer { get { return new Point(442 / WIDTH, 450 / HEIGHT); } }
            public Point ButtonImBack { get { return new Point(560 / WIDTH, 450 / HEIGHT); } }
            public Point ButtonReplay { get { return new Point(117 / WIDTH, 13 / HEIGHT); } }
            private readonly Bmp _bmpButtonImBack;
            private readonly double _bmpButtonImBackRed;
            private readonly double _bmpButtonImBackGreen;
            private readonly double _bmpButtonImBackBlue;
            public Bmp BmpButtonImBack { get { return _bmpButtonImBack; } }
            public double BmpButtonImBackRed { get { return _bmpButtonImBackRed; } }
            public double BmpButtonImBackGreen { get { return _bmpButtonImBackGreen; } }
            public double BmpButtonImBackBlue { get { return _bmpButtonImBackBlue; } }
            public double ButtonImBackX { get { return 528 / WIDTH; } }
            public double ButtonImBackY { get { return 442 / HEIGHT; } }
            public double ButtonImBackWidth { get { return _bmpButtonImBack.Width / WIDTH; } }
            public double ButtonImBackHeight { get { return _bmpButtonImBack.Height / HEIGHT; } }
            public double MaxDifferenceR { get { return 5; } }
            public double MaxDifferenceG { get { return 5; } }
            public double MaxDifferenceB { get { return 5; } }

            public Slick()
            {
                _bmpButtonImBack = new Bmp(new Bitmap(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream("PsHandler.Images.Themes.Slick.imback.png")));
                Handler.AverageColor(_bmpButtonImBack, new System.Drawing.Rectangle(0, 0, _bmpButtonImBack.Width, _bmpButtonImBack.Height), out _bmpButtonImBackRed, out _bmpButtonImBackGreen, out _bmpButtonImBackBlue);
            }

            public override string ToString() { return "Slick"; }
        }

        public class HyperSimple : PokerStarsTheme
        {
            public Point ButtonTimer { get { return new Point(500 / WIDTH, 455 / HEIGHT); } }
            public Point ButtonImBack { get { return new Point(583 / WIDTH, 456 / HEIGHT); } }
            public Point ButtonReplay { get { return new Point(10 / WIDTH, 26 / HEIGHT); } }
            private readonly Bmp _bmpButtonImBack;
            private readonly double _bmpButtonImBackRed;
            private readonly double _bmpButtonImBackGreen;
            private readonly double _bmpButtonImBackBlue;
            public Bmp BmpButtonImBack { get { return _bmpButtonImBack; } }
            public double BmpButtonImBackRed { get { return _bmpButtonImBackRed; } }
            public double BmpButtonImBackGreen { get { return _bmpButtonImBackGreen; } }
            public double BmpButtonImBackBlue { get { return _bmpButtonImBackBlue; } }
            public double ButtonImBackX { get { return 556 / WIDTH; } }
            public double ButtonImBackY { get { return 451 / HEIGHT; } }
            public double ButtonImBackWidth { get { return _bmpButtonImBack.Width / WIDTH; } }
            public double ButtonImBackHeight { get { return _bmpButtonImBack.Height / HEIGHT; } }
            public double MaxDifferenceR { get { return 6; } }
            public double MaxDifferenceG { get { return 6; } }
            public double MaxDifferenceB { get { return 6; } }

            public HyperSimple()
            {
                _bmpButtonImBack = new Bmp(new Bitmap(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream("PsHandler.Images.Themes.HyperSimple.imback.png")));
                Handler.AverageColor(_bmpButtonImBack, new System.Drawing.Rectangle(0, 0, _bmpButtonImBack.Width, _bmpButtonImBack.Height), out _bmpButtonImBackRed, out _bmpButtonImBackGreen, out _bmpButtonImBackBlue);
            }

            public override string ToString() { return "HyperSimple"; }
        }

        public class Stars : PokerStarsTheme
        {
            public Point ButtonTimer { get { return new Point(500 / WIDTH, 455 / HEIGHT); } }
            public Point ButtonImBack { get { return new Point(583 / WIDTH, 456 / HEIGHT); } }
            public Point ButtonReplay { get { return new Point(10 / WIDTH, 26 / HEIGHT); } }
            private readonly Bmp _bmpButtonImBack;
            private readonly double _bmpButtonImBackRed;
            private readonly double _bmpButtonImBackGreen;
            private readonly double _bmpButtonImBackBlue;
            public Bmp BmpButtonImBack { get { return _bmpButtonImBack; } }
            public double BmpButtonImBackRed { get { return _bmpButtonImBackRed; } }
            public double BmpButtonImBackGreen { get { return _bmpButtonImBackGreen; } }
            public double BmpButtonImBackBlue { get { return _bmpButtonImBackBlue; } }
            public double ButtonImBackX { get { return 558 / WIDTH; } }
            public double ButtonImBackY { get { return 451 / HEIGHT; } }
            public double ButtonImBackWidth { get { return _bmpButtonImBack.Width / WIDTH; } }
            public double ButtonImBackHeight { get { return _bmpButtonImBack.Height / HEIGHT; } }
            public double MaxDifferenceR { get { return 4; } }
            public double MaxDifferenceG { get { return 4; } }
            public double MaxDifferenceB { get { return 4; } }

            public Stars()
            {
                _bmpButtonImBack = new Bmp(new Bitmap(System.Reflection.Assembly.GetEntryAssembly().GetManifestResourceStream("PsHandler.Images.Themes.Stars.imback.png")));
                Handler.AverageColor(_bmpButtonImBack, new System.Drawing.Rectangle(0, 0, _bmpButtonImBack.Width, _bmpButtonImBack.Height), out _bmpButtonImBackRed, out _bmpButtonImBackGreen, out _bmpButtonImBackBlue);
            }

            public override string ToString() { return "Stars"; }
        }
    }
}

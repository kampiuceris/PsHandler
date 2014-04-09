using System.Drawing;

namespace PsHandler
{
    public abstract class PokerStarsTheme
    {
        public const double WIDTH = 792;
        public const double HEIGHT = 546;

        public PokerStarsButton ButtonImBack { get; set; }
        public PokerStarsButton ButtonTimebank { get; set; }
        public double ButtonHandReplayX;
        public double ButtonHandReplayY;
    }

    public class PokerStarsButton
    {
        public double ClickX { get; set; }
        public double ClickY { get; set; }
        public Bmp Bmp { get; set; }
        public double AvgRed { get; set; }
        public double AvgGreen { get; set; }
        public double AvgBlue { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double MaxDiffR { get; set; }
        public double MaxDiffG { get; set; }
        public double MaxDiffB { get; set; }
        public PokerStarsButton ButtonSecondaryCheck { get; set; }

        public PokerStarsButton(Bmp bmp, double clickX, double clickY, double x, double y, double maxDiffR, double maxDiffG, double maxDiffB)
        {
            Bmp = bmp;
            double r, g, b;
            Methods.AverageColor(Bmp, new Rectangle(0, 0, Bmp.Width, Bmp.Height), out r, out g, out b);
            AvgRed = r;
            AvgGreen = g;
            AvgBlue = b;
            ClickX = clickX / PokerStarsTheme.WIDTH;
            ClickY = clickY / PokerStarsTheme.HEIGHT;
            X = x / PokerStarsTheme.WIDTH;
            Y = y / PokerStarsTheme.HEIGHT;
            Width = bmp.Width / PokerStarsTheme.WIDTH;
            Height = bmp.Height / PokerStarsTheme.HEIGHT;
            MaxDiffR = maxDiffR;
            MaxDiffG = maxDiffG;
            MaxDiffB = maxDiffB;
        }
    }

    public class PokerStarsThemes
    {
        public class Unknown : PokerStarsTheme
        {
            public Unknown()
            {
                ButtonImBack = new PokerStarsButton(new Bmp(10, 10, Color.Transparent), 0, 0, 0, 0, 0, 0, 0);
                ButtonTimebank = new PokerStarsButton(new Bmp(10, 10, Color.Transparent), 0, 0, 0, 0, 0, 0, 0);
                ButtonHandReplayX = 0;
                ButtonHandReplayY = 0;
            }

            public override string ToString() { return "Unknown"; }
        }

        public class Azure : PokerStarsTheme
        {
            public Azure()
            {
                ButtonImBack = new PokerStarsButton(new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Azure.imback.png")), 562, 452, 531, 441, 5, 5, 5);
                ButtonTimebank = new PokerStarsButton(new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Azure.timebank.png")), 465, 458, 399, 438, 8, 8, 8);
                ButtonHandReplayX = 10 / WIDTH;
                ButtonHandReplayY = 26 / HEIGHT;
            }

            public override string ToString() { return "Azure"; }
        }

        public class Black : PokerStarsTheme
        {
            public Black()
            {
                ButtonImBack = new PokerStarsButton(new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Black.imback.png")), 600, 455, 526, 442, 4, 4, 4);
                ButtonTimebank = new PokerStarsButton(new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Black.timebank.png")), 445, 460, 387, 435, 11, 11, 7);
                ButtonHandReplayX = 177 / WIDTH;
                ButtonHandReplayY = 13 / HEIGHT;
            }

            public override string ToString() { return "Black"; }
        }

        public class Classic : PokerStarsTheme
        {
            public Classic()
            {
                ButtonImBack = new PokerStarsButton(new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Classic.imback.png")), 615, 460, 553, 452, 8, 10, 11);
                ButtonTimebank = new PokerStarsButton(new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Classic.timebank.png")), 510, 463, 451, 447, 7, 8, 6);
                ButtonHandReplayX = 10 / WIDTH;
                ButtonHandReplayY = 26 / HEIGHT;
            }

            public override string ToString() { return "Classic"; }
        }

        public class HyperSimple : PokerStarsTheme
        {
            public HyperSimple()
            {
                ButtonImBack = new PokerStarsButton(new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.HyperSimple.imback.png")), 583, 456, 556, 451, 6, 6, 6);
                ButtonTimebank = new PokerStarsButton(new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.HyperSimple.timebank.png")), 503, 459, 452, 446, 6, 6, 6);
                ButtonHandReplayX = 10 / WIDTH;
                ButtonHandReplayY = 26 / HEIGHT;
            }

            public override string ToString() { return "HyperSimple"; }
        }

        public class Nova : PokerStarsTheme
        {
            public Nova()
            {
                ButtonImBack = new PokerStarsButton(new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Nova.imback.png")), 700, 490, 611, 469, 6, 6, 6);
                ButtonTimebank = new PokerStarsButton(new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Nova.timebank.png")), 733, 423, 702, 411, 15, 15, 15);
                ButtonHandReplayX = 16 / WIDTH;
                ButtonHandReplayY = 11 / HEIGHT;
            }

            public override string ToString() { return "Nova"; }
        }

        public class Slick : PokerStarsTheme
        {
            public Slick()
            {
                ButtonImBack = new PokerStarsButton(new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Slick.imback.png")), 560, 450, 528, 442, 7, 7, 7);
                ButtonTimebank = new PokerStarsButton(new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Slick.timebank.png")), 446, 459, 386, 434, 13, 13, 13);
                ButtonHandReplayX = 177 / WIDTH;
                ButtonHandReplayY = 13 / HEIGHT;
                ButtonImBack.ButtonSecondaryCheck = new PokerStarsButton(new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Slick.imback2.png")), 560, 450, 579, 443, 13, 13, 13);
            }

            public override string ToString() { return "Slick"; }
        }

        public class Stars : PokerStarsTheme
        {
            public Stars()
            {
                ButtonImBack = new PokerStarsButton(new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Stars.imback.png")), 583, 456, 558, 451, 4, 4, 4);
                ButtonTimebank = new PokerStarsButton(new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Stars.timebank.png")), 500, 458, 452, 446, 6, 6, 7);
                ButtonHandReplayX = 10 / WIDTH;
                ButtonHandReplayY = 26 / HEIGHT;
            }

            public override string ToString() { return "Stars"; }
        }
    }
}

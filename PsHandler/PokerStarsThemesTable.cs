using System.Drawing;
using PsHandler.Custom;

namespace PsHandler
{
    public abstract class PokerStarsThemeTable
    {
        // client size for windows 7/8/8.1
        public const double WIDTH = 792;
        public const double HEIGHT = 546;

        public PokerStarsButton ButtonImBack { get; set; }
        public PokerStarsButton ButtonTimebank { get; set; }
        public double ButtonHandReplayX;
        public double ButtonHandReplayY;

        public static PokerStarsThemeTable Parse(string value)
        {
            PokerStarsThemeTable o;

            o = new PokerStarsThemesTable.Azure(); if (value.Equals(o.ToString())) return o;
            o = new PokerStarsThemesTable.Black(); if (value.Equals(o.ToString())) return o;
            o = new PokerStarsThemesTable.Classic(); if (value.Equals(o.ToString())) return o;
            o = new PokerStarsThemesTable.HyperSimple(); if (value.Equals(o.ToString())) return o;
            o = new PokerStarsThemesTable.Mercury(); if (value.Equals(o.ToString())) return o;
            o = new PokerStarsThemesTable.Nova(); if (value.Equals(o.ToString())) return o;
            o = new PokerStarsThemesTable.Slick(); if (value.Equals(o.ToString())) return o;
            o = new PokerStarsThemesTable.Stars(); if (value.Equals(o.ToString())) return o;
            o = new PokerStarsThemesTable.Unknown(); if (value.Equals(o.ToString())) return o;

            return new PokerStarsThemesTable.Unknown();
        }
    }

    public class PokerStarsButton
    {
        public Bmp Bmp { get; set; }
        public double AvgR { get; set; }
        public double AvgG { get; set; }
        public double AvgB { get; set; }
        public double LocationX { get; set; }
        public double LocationY { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double MaxDiffR { get; set; }
        public double MaxDiffG { get; set; }
        public double MaxDiffB { get; set; }
        public PokerStarsButton ButtonSecondaryCheck { get; set; }
        public double ClickX { get; set; }
        public double ClickY { get; set; }

        public PokerStarsButton(Bmp bmp, double clickX, double clickY, double locationX, double locationY, double maxDiffR, double maxDiffG, double maxDiffB)
        {
            Bmp = bmp;
            double r, g, b;
            Methods.AverageColor(Bmp, new Rectangle(0, 0, Bmp.Width, Bmp.Height), out r, out g, out b);
            AvgR = r;
            AvgG = g;
            AvgB = b;
            ClickX = clickX / PokerStarsThemeTable.WIDTH;
            ClickY = clickY / PokerStarsThemeTable.HEIGHT;
            LocationX = locationX / PokerStarsThemeTable.WIDTH;
            LocationY = locationY / PokerStarsThemeTable.HEIGHT;
            Width = bmp.Width / PokerStarsThemeTable.WIDTH;
            Height = bmp.Height / PokerStarsThemeTable.HEIGHT;
            MaxDiffR = maxDiffR;
            MaxDiffG = maxDiffG;
            MaxDiffB = maxDiffB;
        }
    }

    public class PokerStarsThemesTable
    {
        public class Unknown : PokerStarsThemeTable
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

        public class Azure : PokerStarsThemeTable
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

        public class Black : PokerStarsThemeTable
        {
            public Black()
            {
                ButtonImBack = new PokerStarsButton(new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Black.imback.png")), 600, 455, 526, 442, 4, 4, 4);
                ButtonTimebank = new PokerStarsButton(new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Black.timebank.png")), 445, 460, 387, 435, 11, 14, 14);
                ButtonHandReplayX = 177 / WIDTH;
                ButtonHandReplayY = 13 / HEIGHT;
            }

            public override string ToString() { return "Black"; }
        }

        public class Classic : PokerStarsThemeTable
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

        public class HyperSimple : PokerStarsThemeTable
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

        public class Mercury : PokerStarsThemeTable
        {
            public Mercury()
            {
                ButtonImBack = new PokerStarsButton(new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Mercury.imback.png")), 650, 490, 613, 471, 10, 10, 10);
                ButtonTimebank = new PokerStarsButton(new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Mercury.timebank.png")), 439, 469, 404, 463, 26, 26, 26);
                ButtonHandReplayX = 109 / WIDTH;
                ButtonHandReplayY = 12 / HEIGHT;
            }

            public override string ToString() { return "Mercury"; }
        }

        public class Nova : PokerStarsThemeTable
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

        public class Slick : PokerStarsThemeTable
        {
            public Slick()
            {
                ButtonImBack = new PokerStarsButton(new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Slick.imback.png")), 560, 450, 528, 442, 9, 9, 9);
                ButtonTimebank = new PokerStarsButton(new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Slick.timebank.png")), 446, 459, 386, 434, 13, 13, 13);
                ButtonHandReplayX = 177 / WIDTH;
                ButtonHandReplayY = 13 / HEIGHT;
                ButtonImBack.ButtonSecondaryCheck = new PokerStarsButton(new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Slick.imback2.png")), 560, 450, 579, 443, 13, 13, 13);
            }

            public override string ToString() { return "Slick"; }
        }

        public class Stars : PokerStarsThemeTable
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

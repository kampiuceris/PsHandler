using System;
using System.Diagnostics;
using System.Drawing;

namespace PsHandler
{
    public abstract class PokerStarsThemeTable
    {
        public const double WIDTH = 792;
        public const double HEIGHT = 546;
        public const double WIDTH_SMALL = 475;
        public const double HEIGHT_SMALL = 327;

        public PokerStarsButton ButtonImBack { get; set; }
        public PokerStarsButton ButtonTimebank { get; set; }
        public double ButtonHandReplayX;
        public double ButtonHandReplayY;
        public ___PokerStarsCheckBox CheckBoxFoldToAnyBet { get; set; }
        public ___PokerStarsCheckBox CheckBoxSitOutNextHand { get; set; }
        public ___PokerStarsCheckBox CheckBoxSitOutNextBigBlind { get; set; }

        public static PokerStarsThemeTable Parse(string value)
        {
            PokerStarsThemeTable o;

            o = new PokerStarsThemesTable.Azure(); if (value.Equals(o.ToString())) return o;
            o = new PokerStarsThemesTable.Black(); if (value.Equals(o.ToString())) return o;
            o = new PokerStarsThemesTable.Classic(); if (value.Equals(o.ToString())) return o;
            o = new PokerStarsThemesTable.HyperSimple(); if (value.Equals(o.ToString())) return o;
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

    public class ___PokerStarsCheckBox
    {
        public double LocationX { get; set; }
        public double LocationY { get; set; }
        public double LocationXSmall { get; set; }
        public double LocationYSmall { get; set; }

        public double Width { get; set; }
        public double Height { get; set; }
        public double WidthSmall { get; set; }
        public double HeightSmall { get; set; }

        public Bmp BmpUnchecked { get; set; }
        public Bmp BmpChecked { get; set; }
        public Bmp BmpUncheckedSmall { get; set; }
        public Bmp BmpCheckedSmall { get; set; }

        public double AvgRUnchecked { get; set; }
        public double AvgGUnchecked { get; set; }
        public double AvgBUnchecked { get; set; }
        public double AvgRChecked { get; set; }
        public double AvgGChecked { get; set; }
        public double AvgBChecked { get; set; }

        public double MaxDiffRUnchecked { get; set; }
        public double MaxDiffGUnchecked { get; set; }
        public double MaxDiffBUnchecked { get; set; }
        public double MaxDiffRChecked { get; set; }
        public double MaxDiffGChecked { get; set; }
        public double MaxDiffBChecked { get; set; }

        public double AvgRUncheckedSmall { get; set; }
        public double AvgGUncheckedSmall { get; set; }
        public double AvgBUncheckedSmall { get; set; }
        public double AvgRCheckedSmall { get; set; }
        public double AvgGCheckedSmall { get; set; }
        public double AvgBCheckedSmall { get; set; }

        public double MaxDiffRUncheckedSmall { get; set; }
        public double MaxDiffGUncheckedSmall { get; set; }
        public double MaxDiffBUncheckedSmall { get; set; }
        public double MaxDiffRCheckedSmall { get; set; }
        public double MaxDiffGCheckedSmall { get; set; }
        public double MaxDiffBCheckedSmall { get; set; }

        public double ClickX { get; set; }
        public double ClickY { get; set; }
        public double ClickXSmall { get; set; }
        public double ClickYSmall { get; set; }

        public ___PokerStarsCheckBox(Bmp bmpUnchecked, Bmp bmpChecked, Bmp bmpUncheckedSmall, Bmp bmpCheckedSmall,
            double locationX, double locationY, double locationXSmall, double locationYSmall,
            double maxDiffRUnchecked, double maxDiffGUnchecked, double maxDiffBUnchecked,
            double maxDiffRChecked, double maxDiffGChecked, double maxDiffBChecked,
            double maxDiffRUncheckedSmall, double maxDiffGUncheckedSmall, double maxDiffBUncheckedSmall,
            double maxDiffRCheckedSmall, double maxDiffGCheckedSmall, double maxDiffBCheckedSmall,
            double clickX, double clickY, double clickXSmall, double clickYSmall)
        {
            if (bmpUnchecked.Width != bmpChecked.Width || bmpUnchecked.Height != bmpChecked.Height ||
                bmpUncheckedSmall.Height != bmpCheckedSmall.Height || bmpUncheckedSmall.Height != bmpCheckedSmall.Height)
                throw new NotSupportedException("bmpCUnchecked(Small) & bmpChecked(Small) size does not match.");

            BmpUnchecked = bmpUnchecked;
            BmpChecked = bmpChecked;
            BmpUncheckedSmall = bmpUncheckedSmall;
            BmpCheckedSmall = bmpCheckedSmall;

            LocationX = locationX / PokerStarsThemeTable.WIDTH;
            LocationY = locationY / PokerStarsThemeTable.HEIGHT;
            LocationXSmall = locationXSmall / PokerStarsThemeTable.WIDTH_SMALL;
            LocationYSmall = locationYSmall / PokerStarsThemeTable.HEIGHT_SMALL;

            Width = BmpUnchecked.Width / PokerStarsThemeTable.WIDTH;
            Height = BmpUnchecked.Height / PokerStarsThemeTable.HEIGHT;
            WidthSmall = BmpUncheckedSmall.Width / PokerStarsThemeTable.WIDTH_SMALL;
            HeightSmall = BmpUncheckedSmall.Height / PokerStarsThemeTable.HEIGHT_SMALL;

            double r, g, b;
            Methods.AverageColor(BmpUnchecked, new Rectangle(0, 0, BmpUnchecked.Width, BmpUnchecked.Height), out r, out g, out b);
            AvgRUnchecked = r;
            AvgGUnchecked = g;
            AvgBUnchecked = b;
            Methods.AverageColor(BmpChecked, new Rectangle(0, 0, BmpChecked.Width, BmpChecked.Height), out r, out g, out b);
            AvgRChecked = r;
            AvgGChecked = g;
            AvgBChecked = b;
            Methods.AverageColor(BmpUncheckedSmall, new Rectangle(0, 0, BmpUncheckedSmall.Width, BmpUncheckedSmall.Height), out r, out g, out b);
            AvgRUncheckedSmall = r;
            AvgGUncheckedSmall = g;
            AvgBUncheckedSmall = b;
            Methods.AverageColor(BmpCheckedSmall, new Rectangle(0, 0, BmpCheckedSmall.Width, BmpCheckedSmall.Height), out r, out g, out b);
            AvgRCheckedSmall = r;
            AvgGCheckedSmall = g;
            AvgBCheckedSmall = b;

            MaxDiffRUnchecked = maxDiffRUnchecked;
            MaxDiffGUnchecked = maxDiffGUnchecked;
            MaxDiffBUnchecked = maxDiffBUnchecked;
            MaxDiffRChecked = maxDiffRChecked;
            MaxDiffGChecked = maxDiffGChecked;
            MaxDiffBChecked = maxDiffBChecked;
            MaxDiffRUncheckedSmall = maxDiffRUncheckedSmall;
            MaxDiffGUncheckedSmall = maxDiffGUncheckedSmall;
            MaxDiffBUncheckedSmall = maxDiffBUncheckedSmall;
            MaxDiffRCheckedSmall = maxDiffRCheckedSmall;
            MaxDiffGCheckedSmall = maxDiffGCheckedSmall;
            MaxDiffBCheckedSmall = maxDiffBCheckedSmall;

            ClickX = clickX / PokerStarsThemeTable.WIDTH;
            ClickY = clickY / PokerStarsThemeTable.HEIGHT;
            ClickXSmall = clickXSmall / PokerStarsThemeTable.WIDTH_SMALL;
            ClickYSmall = clickYSmall / PokerStarsThemeTable.HEIGHT_SMALL;
        }

        public enum PokerStarsCheckBoxState
        {
            Unchecked,
            Checked,
            NotFound,
            Both
        }

        //TODO later
        private PokerStarsCheckBoxState ___GetStateCheckBoxSitOutNextHand(Bmp bmp)
        {
            if (bmp.Width > PokerStarsThemeTable.WIDTH_SMALL + 50)
            {
                // big
                Rectangle rect = new Rectangle(
                    (int)Math.Round(LocationX * bmp.Width),
                    (int)Math.Round(LocationY * bmp.Height),
                    (int)Math.Round(Width * bmp.Width),
                    (int)Math.Round(Height * bmp.Height));
                double r, g, b;
                Methods.AverageColor(bmp, rect, out r, out g, out b);

                double diffRU = Math.Abs(r - AvgRUnchecked);
                double diffGU = Math.Abs(g - AvgGUnchecked);
                double diffBU = Math.Abs(b - AvgBUnchecked);
                double diffRC = Math.Abs(r - AvgRChecked);
                double diffGC = Math.Abs(g - AvgGChecked);
                double diffBC = Math.Abs(b - AvgBChecked);

                bool isUnchecked = false, isChecked = false;
                if (diffRU < MaxDiffRUnchecked && diffGU < MaxDiffGUnchecked && diffBU < MaxDiffBUnchecked)
                {
                    isUnchecked = true;
                }
                if (diffRC < MaxDiffRChecked && diffGC < MaxDiffGChecked && diffBC < MaxDiffBChecked)
                {
                    isChecked = true;
                }

                PokerStarsCheckBoxState state;
                if (isChecked && !isUnchecked) state = PokerStarsCheckBoxState.Checked;
                else if (!isChecked && isUnchecked) state = PokerStarsCheckBoxState.Unchecked;
                else if (isChecked && isUnchecked) state = PokerStarsCheckBoxState.Both;
                else state = PokerStarsCheckBoxState.NotFound;

                Debug.WriteLine(string.Format("BIG   [Unchecked] {0,-3:##0} {1,-3:##0} {2,-3:##0}     [Checked]  {3,-3:##0} {4,-3:##0} {5,-3:##0}     {6}",
                    diffRU, diffGU, diffBU, diffRC, diffGC, diffBC, state));

                return state;
            }
            else
            {
                // small
                Rectangle rect = new Rectangle(
                    (int)Math.Round(LocationXSmall * bmp.Width),
                    (int)Math.Round(LocationYSmall * bmp.Height),
                    (int)Math.Round(WidthSmall * bmp.Width),
                    (int)Math.Round(HeightSmall * bmp.Height));
                double r, g, b;
                Methods.AverageColor(bmp, rect, out r, out g, out b);

                double diffRU = Math.Abs(r - AvgRUncheckedSmall);
                double diffGU = Math.Abs(g - AvgGUncheckedSmall);
                double diffBU = Math.Abs(b - AvgBUncheckedSmall);
                double diffRC = Math.Abs(r - AvgRCheckedSmall);
                double diffGC = Math.Abs(g - AvgGCheckedSmall);
                double diffBC = Math.Abs(b - AvgBCheckedSmall);

                bool isUnchecked = false, isChecked = false;
                if (diffRU < MaxDiffRUncheckedSmall && diffGU < MaxDiffGUncheckedSmall && diffBU < MaxDiffBUncheckedSmall)
                {
                    isUnchecked = true;
                }
                if (diffRC < MaxDiffRCheckedSmall && diffGC < MaxDiffGCheckedSmall && diffBC < MaxDiffBCheckedSmall)
                {
                    isChecked = true;
                }

                PokerStarsCheckBoxState state;
                if (isChecked && !isUnchecked) state = PokerStarsCheckBoxState.Checked;
                else if (!isChecked && isUnchecked) state = PokerStarsCheckBoxState.Unchecked;
                else if (isChecked && isUnchecked) state = PokerStarsCheckBoxState.Both;
                else state = PokerStarsCheckBoxState.NotFound;

                Debug.WriteLine(string.Format("SMALL [Unchecked] {0,-3:##0} {1,-3:##0} {2,-3:##0}     [Checked]  {3,-3:##0} {4,-3:##0} {5,-3:##0}     {6}",
                    diffRU, diffGU, diffBU, diffRC, diffGC, diffBC, state));

                return state;
            }
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
                ButtonTimebank = new PokerStarsButton(new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Black.timebank.png")), 445, 460, 387, 435, 11, 11, 7);
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

        public class Nova : PokerStarsThemeTable
        {
            public Nova()
            {
                ButtonImBack = new PokerStarsButton(new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Nova.imback.png")), 700, 490, 611, 469, 6, 6, 6);
                ButtonTimebank = new PokerStarsButton(new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Nova.timebank.png")), 733, 423, 702, 411, 15, 15, 15);
                ButtonHandReplayX = 16 / WIDTH;
                ButtonHandReplayY = 11 / HEIGHT;

                if (false) //TODO later
                {
                    CheckBoxFoldToAnyBet = new ___PokerStarsCheckBox(
                    new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Nova.uncheckedfold.png")),
                    new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Nova.checked.png")),
                    new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Nova.uncheckedfoldsmall.png")),
                    new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Nova.checkedsmall.png")),
                    9, 386, 6, 232,
                    10, 10, 10, 10, 10, 10,
                    10, 10, 10, 10, 10, 10,
                    12, 389, 8, 233);
                    CheckBoxSitOutNextHand = new ___PokerStarsCheckBox(
                        new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Nova.unchecked.png")),
                        new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Nova.checked.png")),
                        new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Nova.uncheckedsmall.png")),
                        new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Nova.checkedsmall.png")),
                        9, 401, 6, 241,
                        40, 40, 40, 25, 25, 25,
                        40, 40, 40, 40, 20, 25,
                        12, 404, 8, 242);
                    CheckBoxSitOutNextBigBlind = new ___PokerStarsCheckBox(
                        new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Nova.unchecked.png")),
                        new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Nova.checked.png")),
                        new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Nova.uncheckedsmall.png")),
                        new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.Themes.Nova.checkedsmall.png")),
                        9, 416, 6, 250,
                        40, 40, 40, 25, 25, 25,
                        40, 40, 40, 40, 20, 25,
                        12, 419, 8, 251);
                }
            }

            public override string ToString() { return "Nova"; }
        }

        public class Slick : PokerStarsThemeTable
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

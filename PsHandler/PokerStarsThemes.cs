using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace PsHandler
{
    public interface PokerStarsTheme
    {
        Point ButtonTimer { get; }
        Point ButtonImBack { get; }
        Point ButtonReplay { get; }
    }

    public class PokerStarsThemes
    {
        public const int DEFAULT_TABLE_WIDTH = 792;
        public const int DEFAULT_TABLE_HEIGHT = 546;

        public class Unknown : PokerStarsTheme
        {
            public Point ButtonTimer
            {
                get { return new Point(0, 0); }
            }
            public Point ButtonImBack
            {
                get { return new Point(0, 0); }
            }
            public Point ButtonReplay
            {
                get { return new Point(0, 0); }
            }
            public override string ToString()
            {
                return "Unknown";
            }
        }

        public class Nova : PokerStarsTheme
        {
            public Point ButtonTimer
            {
                get { return new Point(735 / (double)DEFAULT_TABLE_WIDTH, 425 / (double)DEFAULT_TABLE_HEIGHT); }
            }
            public Point ButtonImBack
            {
                get { return new Point(700 / (double)DEFAULT_TABLE_WIDTH, 490 / (double)DEFAULT_TABLE_HEIGHT); }
            }
            public Point ButtonReplay
            {
                get { return new Point(15 / (double)DEFAULT_TABLE_WIDTH, 11 / (double)DEFAULT_TABLE_HEIGHT); }
            }
            public override string ToString()
            {
                return "Nova";
            }
        }

        public class Classic : PokerStarsTheme
        {
            public Point ButtonTimer
            {
                get { return new Point(500 / (double)DEFAULT_TABLE_WIDTH, 470 / (double)DEFAULT_TABLE_HEIGHT); }
            }
            public Point ButtonImBack
            {
                get { return new Point(615 / (double)DEFAULT_TABLE_WIDTH, 460 / (double)DEFAULT_TABLE_HEIGHT); }
            }
            public Point ButtonReplay
            {
                get { return new Point(10 / (double)DEFAULT_TABLE_WIDTH, 26 / (double)DEFAULT_TABLE_HEIGHT); }
            }
            public override string ToString()
            {
                return "Classic";
            }
        }

        public class Black : PokerStarsTheme
        {
            public Point ButtonTimer
            {
                get { return new Point(442 / (double)DEFAULT_TABLE_WIDTH, 450 / (double)DEFAULT_TABLE_HEIGHT); }
            }
            public Point ButtonImBack
            {
                get { return new Point(600 / (double)DEFAULT_TABLE_WIDTH, 455 / (double)DEFAULT_TABLE_HEIGHT); }
            }
            public Point ButtonReplay
            {
                get { return new Point(117 / (double)DEFAULT_TABLE_WIDTH, 13 / (double)DEFAULT_TABLE_HEIGHT); }
            }
            public override string ToString()
            {
                return "Black";
            }
        }

        public class Slick : PokerStarsTheme
        {
            public Point ButtonTimer
            {
                get { return new Point(442 / (double)DEFAULT_TABLE_WIDTH, 450 / (double)DEFAULT_TABLE_HEIGHT); }
            }
            public Point ButtonImBack
            {
                get { return new Point(560 / (double)DEFAULT_TABLE_WIDTH, 450 / (double)DEFAULT_TABLE_HEIGHT); }
            }
            public Point ButtonReplay
            {
                get { return new Point(117 / (double)DEFAULT_TABLE_WIDTH, 13 / (double)DEFAULT_TABLE_HEIGHT); }
            }
            public override string ToString()
            {
                return "Slick";
            }
        }

        public class HyperSimple : PokerStarsTheme
        {
            public Point ButtonTimer
            {
                get { return new Point(500 / (double)DEFAULT_TABLE_WIDTH, 455 / (double)DEFAULT_TABLE_HEIGHT); }
            }
            public Point ButtonImBack
            {
                get { return new Point(583 / (double)DEFAULT_TABLE_WIDTH, 456 / (double)DEFAULT_TABLE_HEIGHT); }
            }
            public Point ButtonReplay
            {
                get { return new Point(10 / (double)DEFAULT_TABLE_WIDTH, 26 / (double)DEFAULT_TABLE_HEIGHT); }
            }
            public override string ToString()
            {
                return "HyperSimple";
            }
        }

        public class Stars : PokerStarsTheme
        {
            public Point ButtonTimer
            {
                get { return new Point(500 / (double)DEFAULT_TABLE_WIDTH, 455 / (double)DEFAULT_TABLE_HEIGHT); }
            }
            public Point ButtonImBack
            {
                get { return new Point(583 / (double)DEFAULT_TABLE_WIDTH, 456 / (double)DEFAULT_TABLE_HEIGHT); }
            }
            public Point ButtonReplay
            {
                get { return new Point(10 / (double)DEFAULT_TABLE_WIDTH, 26 / (double)DEFAULT_TABLE_HEIGHT); }
            }
            public override string ToString()
            {
                return "Stars";
            }
        }
    }
}

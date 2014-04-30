using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PsHandler
{
    public abstract class PokerStarsThemeLobby
    {
        public static PokerStarsThemeLobby Parse(string value)
        {
            PokerStarsThemeLobby o;

            o = new PokerStarsThemesLobby.Black(); if (value.Equals(o.ToString())) return o;
            o = new PokerStarsThemesLobby.Classic(); if (value.Equals(o.ToString())) return o;

            return new PokerStarsThemesLobby.Unknown();
        }
    }

    public class PokerStarsThemesLobby
    {
        public class Unknown : PokerStarsThemeLobby
        {
            public override string ToString() { return "Unknown"; }
        }

        public class Black : PokerStarsThemeLobby
        {
            public override string ToString() { return "Black"; }
        }

        public class Classic : PokerStarsThemeLobby
        {
            public override string ToString() { return "Classic"; }
        }
    }
}

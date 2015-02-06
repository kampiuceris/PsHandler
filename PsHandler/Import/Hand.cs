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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Shapes;
using PsHandler.PokerMath;

namespace PsHandler.Import
{
    // Level

    public class Level
    {
        public decimal Ante;
        public decimal SmallBlind;
        public decimal BigBlind;
        public bool IsAnteDefined;
    }

    // Hand

    public class Hand : PokerHand
    {
        public Level Level;
        public string[] PlayerNames;
        public decimal[] StacksAfterHand;

        public Hand(string handHistoryText)
            : base(handHistoryText)
        {
            Level = new Level { SmallBlind = LevelSmallBlind, BigBlind = LevelBigBlind, Ante = LevelAnte, IsAnteDefined = true };
            var table = new PokerMath.Table();
            table.LoadHand(this);
            table.ToDoCommandsAll();
            PlayerNames = new string[table.PlayerCount];
            StacksAfterHand = new decimal[table.PlayerCount];
            for (int i = 0; i < table.PlayerCount; i++)
            {
                PlayerNames[i] = table.Players[i].PlayerName;
                StacksAfterHand[i] = table.Players[i].Stack;
            }
        }

        public new static Hand Parse(string handHistoryText)
        {
            try
            {
                return new Hand(handHistoryText);
            }
            catch
            {
                return null;
            }
        }
    }
}

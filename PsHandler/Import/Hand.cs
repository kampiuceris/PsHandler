// PsHandler - poker software helping tool.
// Copyright (C) 2014-2015  kampiuceris

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
        public decimal[] StacksBeforeHand;
        public decimal[] StacksAfterHand;
        public int HeroId = -1;

        public string[] HudPlayerNames;
        public decimal[] HudPlayerStacks;
        public PokerEnums.Position[] HudPlayerPositions;
        public int HudHeroSeat;

        public Hand(string handHistoryText)
            : base(handHistoryText)
        {
            Level = new Level { SmallBlind = LevelSmallBlind, BigBlind = LevelBigBlind, Ante = LevelAnte, IsAnteDefined = true };
            var table = new PokerMath.Table();
            table.LoadHand(this);

            PlayerNames = new string[table.PlayerCount];
            StacksBeforeHand = new decimal[table.PlayerCount];
            StacksAfterHand = new decimal[table.PlayerCount];
            for (int i = 0; i < table.PlayerCount; i++)
            {
                PlayerNames[i] = table.Players[i].PlayerName;
                StacksBeforeHand[i] = table.Players[i].Stack;
            }
            table.ToDoCommandsAll();
            for (int i = 0; i < table.PlayerCount; i++)
            {
                StacksAfterHand[i] = table.Players[i].Stack;
            }

            var hero = table.Players.FirstOrDefault(a => a.IsHero);
            if (hero != null)
            {
                HeroId = Array.IndexOf(PlayerNames, hero.PlayerName);
                HudHeroSeat = hero.SeatNumber;
            }

            HudPlayerNames = new string[table.Seats.Length];
            HudPlayerStacks = new decimal[table.Seats.Length];
            for (int i = 0; i < table.Seats.Length; i++)
            {
                if (table.Seats[i] != null && table.Seats[i].Stack > 0)
                {
                    HudPlayerNames[i] = table.Seats[i].PlayerName;
                    HudPlayerStacks[i] = table.Seats[i].Stack;
                }
                else
                {
                    HudPlayerNames[i] = null;
                    HudPlayerStacks[i] = 0;
                }
            }

            #region Get next hands Positions

            // set position for next hand (cut missing players and etc)

            var alivePlayers = table.Players;
            var players = new List<Player>();
            players.AddRange(alivePlayers);
            players.AddRange(alivePlayers);

            var playersFromButton = new List<Player>();
            int start = players.IndexOf(players.First(p => p.SeatNumber == ButtonSeat));
            bool buttonDropped = false;
            for (int i = start; i < start + alivePlayers.Length; i++)
            {
                if (players[i].Stack > 0)
                {
                    playersFromButton.Add(players[i]);
                }
                else
                {
                    if (players[i].SeatNumber == ButtonSeat)
                    {
                        buttonDropped = true;
                    }
                }
            }

            var playersFromButtonLive = new List<Player>(playersFromButton);
            if (!buttonDropped)
            {
                var playeUtg = playersFromButtonLive[0];
                playersFromButtonLive.RemoveAt(0);
                playersFromButtonLive.Add(playeUtg);
            }

            // dabar kai playersFromButtonLive yra nuo utg
            var positions = GetPositions(playersFromButtonLive.Count);
            HudPlayerPositions = new PokerEnums.Position[10];
            for (int i = 0; i < playersFromButtonLive.Count; i++)
            {
                HudPlayerPositions[Array.IndexOf(HudPlayerNames, playersFromButtonLive[i].PlayerName)] = positions[i];
            }

            #endregion
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

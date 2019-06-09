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
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using PsHandler.Custom;
using PsHandler.Import;
using PsHandler.PokerMath;
using PsHandler.PokerTypes;

namespace PsHandler.Hud
{
    public class TableHud : IDisposable
    {
        private static readonly Regex _regexTitle = new Regex(@"\A(?<table_name>.+) - (\$|€|£)?(?<sb>[\d\.]+)\/(\$|€|£)?(?<bb>[\d\.]+)( ante (\$|€|£)?(?<ante>[\d\.]+))? - Tournament (?<tournament_number>\d+).+Logged In as (?<hero>.+)\z");
        public enum OwnerState { Unknown, Unattached, Attached }

        private readonly Thread _thread;
        public PokerEnums.TableSize TableSize = PokerEnums.TableSize.Default;
        public PokerType PokerType;
        //
        public Table Table;
        public WindowTimer WindowTimer;
        public WindowBigBlind[] WindowsBigBlind = new WindowBigBlind[10];

        public TableHud(Table table)
        {
            #region Init Windows

            Methods.UiInvoke(() =>
            {
                Table = table;
                WindowTimer = new WindowTimer(Table);
                WindowTimer.Opacity = 0;
                WindowTimer.Show();
                for (int i = 0; i < 10; i++)
                {
                    WindowsBigBlind[i] = new WindowBigBlind(Table, i);
                    WindowsBigBlind[i].Opacity = 0;
                    WindowsBigBlind[i].Show();
                }
            });

            #endregion

            _thread = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        #region Calculate Timer / BigBlinds

                        string title = Table.Title, className = Table.ClassName;
                        long tournamentNumber;
                        decimal sb, bb, ante;

                        string timerValue = null;
                        bool timerVisibility = false;
                        decimal[] bbValue = new decimal[10];
                        bool[] bbVisibility = new bool[10];
                        bool[] bbIsHero = new bool[10];
                        string[] bbTooltip = new string[10];

                        var success = GetInfo(title, out sb, out bb, out ante, out tournamentNumber);
                        if (success)
                        {
                            var tournament = App.HandHistoryManager.GetTournament(tournamentNumber);
                            if (tournament != null)
                            {
                                TableSize = tournament.GetLastHandTableSize();
                                GetContentTimer(title, className, tournament, sb, bb, out timerValue, out timerVisibility);
                                GetContentBigBlind(tournament, sb, bb, ante, out bbValue, out bbVisibility, out bbIsHero, out bbTooltip);
                            }
                        }

                        #endregion

                        #region Update Timer / BigBlinds

                        Methods.UiInvoke(() =>
                        {
                            try // to eliminate that stupid UpdateView null reference problem..
                            {
                                // Update View
                                WindowTimer.UpdateView(timerValue, PokerType != null ? PokerType.Name : "Unknown");
                                for (int i = 0; i < 10; i++)
                                {
                                    WindowsBigBlind[i].UpdateView(i, bbValue[i], bbTooltip[i], bbIsHero[i]);
                                }

                                // Ensure visibility
                                WindowTimer.EnsureVisibility(timerVisibility);
                                for (int i = 0; i < 10; i++)
                                {
                                    WindowsBigBlind[i].EnsureVisibility(bbVisibility[i] && ((!bbIsHero[i] && Config.HudBigBlindShowForOpponents) || (bbIsHero[i] && Config.HudBigBlindShowForHero)));
                                }
                            }
                            catch
                            {
                            }
                        });

                        #endregion

                        Thread.Sleep(500);
                    }
                }
#if (DEBUG)
                catch (ThreadInterruptedException)
                {
                }
#else
                catch (Exception e)
                {
                    if (!(e is ThreadInterruptedException))
                    {
                        Methods.DisplayException(e, App.WindowMain, WindowStartupLocation.CenterOwner);
                    }
                }
#endif
                finally
                {
                    #region Dispose Windows

                    Methods.UiInvoke(() =>
                    {
                        WindowTimer.Close();
                        for (int i = 0; i < 10; i++)
                        {
                            WindowsBigBlind[i].Close();
                        }
                    });

                    #endregion
                }
            });
            _thread.Start();
        }

        public void Dispose()
        {
            if (_thread != null)
            {
                _thread.Interrupt();
            }
        }

        private static bool GetInfo(string title, out decimal sb, out decimal bb, out decimal ante, out long toutnamentNumber)
        {
            sb = -1;
            bb = -1;
            ante = -1;
            toutnamentNumber = -1;

            Match match = _regexTitle.Match(title);
            if (match.Success)
            {
                sb = decimal.Parse(match.Groups["sb"].Value);
                bb = decimal.Parse(match.Groups["bb"].Value);
                var groupAnte = match.Groups["ante"];
                if (groupAnte.Success)
                {
                    ante = decimal.Parse(groupAnte.Value);
                }
                toutnamentNumber = long.Parse(match.Groups["tournament_number"].Value);
                //heroName = match.Groups["hero"].Value;
                return true;
            }
            return false;
        }

        private void GetContentTimer(string title, string className, Tournament tournament, decimal sb, decimal bb, out string value, out bool visibility)
        {
            if (!Config.HudTimerShowHandCount)
            {
                int pokerTypeErrors = -1;
                if (PokerType == null)
                {
                    PokerType = App.PokerTypeManager.GetPokerType(title, className, out pokerTypeErrors);
                    if (pokerTypeErrors != 0)
                    {
                        PokerType = null;
                    }
                }
                if (PokerType != null)
                {
                    DateTime dateTimeUtcNow = DateTime.UtcNow.AddSeconds(-Config.HudTimerDiff);
                    DateTime dateTimeUtcNextLevel = tournament.GetFirstHandTimestampET();
                    while (dateTimeUtcNextLevel < dateTimeUtcNow)
                    {
                        dateTimeUtcNextLevel = dateTimeUtcNextLevel + PokerType.LevelLength;
                    }
                    TimeSpan timeSpan = dateTimeUtcNextLevel - dateTimeUtcNow;
                    value = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
                }
                else
                {
                    if (pokerTypeErrors == 1)
                    {
                        value = Config.HudTimerPokerTypeNotFound;
                    }
                    else if (pokerTypeErrors == 2)
                    {
                        value = Config.HudTimerMultiplePokerTypes;
                    }
                    else
                    {
                        value = string.Format("Unknown Error");
                    }
                }
            }
            else
            {
                value = string.Format("{0}", tournament.CountLevelHands(sb, bb) + 1);
            }

            visibility = true;
        }

        private void GetContentBigBlind(Tournament tournament, decimal sb, decimal bb, decimal ante, out decimal[] value, out bool[] visibility, out bool[] isHero, out string[] tooltip)
        {
            value = new decimal[10];
            visibility = new bool[10];
            isHero = new bool[10];
            tooltip = new string[10];

            var playerNames = tournament.GetLastHandHudPlayerNames();
            var stacks = tournament.GetLastHandHudPlayerStacks();
            var heroSeat = tournament.GetLastHandHudHeroSeat();

            var tableSize = tournament.GetLastHandTableSize();
            var playersAlive = stacks.Count(a => a > 0);
            int playerCount = playersAlive;
            if (Config.HudBigBlindMByPlayerCount) playerCount = playersAlive;
            if (Config.HudBigBlindMByTableSize) playerCount = (int)tableSize;

            for (int i = 0; i < stacks.Length; i++)
            {
                var mappedSeat = MapToPreferredSeat(TableSize, heroSeat, i);

                visibility[mappedSeat] = stacks[i] > 0;
                isHero[mappedSeat] = i == heroSeat;
                tooltip[mappedSeat] = playerNames[i];

                if (Config.HudBigBlindShowBB)
                {
                    value[mappedSeat] = stacks[i] / bb;
                    tooltip[mappedSeat] = string.Format("{0}\r\n{1:0.0} = {2} / {3}", playerNames[i], value[mappedSeat], stacks[i], bb);
                }
                else if (Config.HudBigBlindShowAdjustedBB)
                {
                    // Adjusted BB = stack / (2/3 * (SB + BB + (Ante * number of players)))
                    value[mappedSeat] = stacks[i] / ((decimal)(2.0 / 3.0) * (sb + bb + (ante * playerCount)));
                    tooltip[mappedSeat] = string.Format("{0}\r\n{1:0.0} = {2} / ((2 / 3) * ({3} + {4} + ({5} * {6})))", playerNames[i], value[mappedSeat], stacks[i], sb, bb, ante, playerCount);
                }
                else if (Config.HudBigBlindShowTournamentM)
                {
                    // Tournament M = stack / (SB + BB + (Ante * number of players))
                    value[mappedSeat] = stacks[i] / (sb + bb + (ante * playerCount));
                    tooltip[mappedSeat] = string.Format("{0}\r\n{1:0.0} = {2} / ({3} + {4} + ({5} * {6}))", playerNames[i], value[mappedSeat], stacks[i], sb, bb, ante, playerCount);
                }
            }
        }

        private int MapToPreferredSeat(PokerEnums.TableSize tableSize, int heroSeat, int seat)
        {
            var neededOffset = Config.PreferredSeat[(int)tableSize] + (int)tableSize - heroSeat;
            return (seat + neededOffset) % (int)tableSize;
        }
    }
}

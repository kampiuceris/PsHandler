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

namespace PsHandler.PokerMath
{
    public class Ev
    {
        public decimal[] IcmPayouts;
        public decimal PrizePool;
        public PokerEnums.Currency Currency;

        public decimal[] StacksHandStart;
        public decimal[] StacksHandEnd;
        public decimal[] StacksDiff;
        public double[] IcmsHandStart;
        public double[] IcmsHandEnd;
        public double[] IcmsDiff;

        public string[] PlayerNames;
        public int HeroId;
        public string[][] PocketCards;

        public Street EvRegularAllInStreet;
        public ExpectedValue EvRegular;
        public ExpectedValue EvRegularFullInfo;
        public ExpectedValueStreetByStreet EvStreetByStreet;
        public ExpectedValueStreetByStreet EvStreetByStreetFullInfo;

        public delegate EvaluationPocket Evaluate(string[][] pocketCardsStr, string[] boardCardsStr, string[] deadCardsStr);
        private readonly Evaluate _evaluate;
        private readonly PokerHand _pokerHand;
        private readonly Table _table;
        private string[] _communityCards;
        private string[] _communityCardsFlop;
        private string _communityCardsTurn;
        private string _communityCardsRiver;

        public Ev(PokerHand pokerHand, decimal[] icmPayouts, decimal prizePool, PokerEnums.Currency currency, Evaluate evaluate)
        {
            _pokerHand = pokerHand;
            IcmPayouts = icmPayouts;
            PrizePool = prizePool;
            Currency = currency;
            _evaluate = evaluate;

            _table = new Table();
            _table.LoadHand(pokerHand);

            InitInfo();

            if (pokerHand.Showdown)
            {
                CalculateEvStreetByStreet();

                EvRegularAllInStreet = GetAllInStreet(_table);
                if (EvRegularAllInStreet == Street.Preflop || EvRegularAllInStreet == Street.Flop || EvRegularAllInStreet == Street.Turn)
                {
                    CalculateEvRegular();
                }
            }
        }

        private void InitInfo()
        {
            _table.UnDoCommandsAll();
            StacksHandStart = new decimal[_table.PlayerCount];
            for (int i = 0; i < _table.PlayerCount; i++)
            {
                StacksHandStart[i] = _table.Players[i].Stack;
            }

            _table.ToDoCommandsAll();
            StacksHandEnd = new decimal[_table.PlayerCount];
            for (int i = 0; i < _table.PlayerCount; i++)
            {
                StacksHandEnd[i] = _table.Players[i].Stack;
            }

            IcmsHandStart = Icm.GetEquity(StacksHandStart, StacksHandStart, IcmPayouts);
            IcmsHandEnd = Icm.GetEquity(StacksHandEnd, StacksHandStart, IcmPayouts);

            StacksDiff = new decimal[_table.PlayerCount];
            IcmsDiff = new double[_table.PlayerCount];
            for (int i = 0; i < _table.PlayerCount; i++)
            {
                StacksDiff[i] = StacksHandEnd[i] - StacksHandStart[i];
                IcmsDiff[i] = IcmsHandEnd[i] - IcmsHandStart[i];
            }

            // set community cards
            List<string> communityCards = new List<string>();
            var flop = _pokerHand.PokerCommands.OfType<PokerCommands.Flop>().ToArray();
            if (flop.Any())
            {
                _communityCardsFlop = flop.ElementAt(0).FlopCards;
                communityCards.AddRange(_communityCardsFlop);
            }
            var turn = _pokerHand.PokerCommands.OfType<PokerCommands.Turn>().ToArray();
            if (turn.Any())
            {
                _communityCardsTurn = turn.ElementAt(0).TurnCard;
                communityCards.Add(_communityCardsTurn);
            }
            var river = _pokerHand.PokerCommands.OfType<PokerCommands.River>().ToArray();
            if (river.Any())
            {
                _communityCardsRiver = river.ElementAt(0).RiverCard;
                communityCards.Add(_communityCardsRiver);
            }
            _communityCards = communityCards.ToArray();

            // playernames + pockets + pockets visibility
            PlayerNames = new string[_table.PlayerCount];
            PocketCards = new string[_table.PlayerCount][];
            for (int i = 0; i < _table.PlayerCount; i++)
            {
                PlayerNames[i] = _table.Players[i].PlayerName;
                PocketCards[i] = _table.Players[i].PocketCards;
                if (_table.Players[i].IsHero) HeroId = i;
            }
        }

        private static Street GetAllInStreet(Table table)
        {
            table.UnDoCommandsAll();
            table.ToDoCommandsBeginning();

            Street street = Street.Unknown;
            while (table.ToDo.Any())
            {
                var currentCommand = table.ToDo[0];
                if (currentCommand is PokerCommands.Flop)
                {
                    if (table.Players.Where(o => o.IsInPlay).Count(o => o.Stack != 0) <= 1)
                    {
                        street = Street.Preflop;
                        break;
                    }
                }
                if (currentCommand is PokerCommands.Turn)
                {
                    if (table.Players.Where(o => o.IsInPlay).Count(o => o.Stack != 0) <= 1)
                    {
                        street = Street.Flop;
                        break;
                    }
                }
                if (currentCommand is PokerCommands.River)
                {
                    if (table.Players.Where(o => o.IsInPlay).Count(o => o.Stack != 0) <= 1)
                    {
                        street = Street.Turn;
                        break;
                    }
                }
                if (currentCommand is PokerCommands.FinalizePots)
                {
                    if (table.Players.Where(o => o.IsInPlay).Count(o => o.Stack != 0) <= 1)
                    {
                        street = Street.River;
                        break;
                    }
                }

                table.ToDoCommand();
            }

            return street;
        }

        private static List<Player> FindWinnersAmongPlayers(Scenario scenario, List<Player> players, Dictionary<int, Player> scenarioIdToPlayer)
        {
            List<Player> winners = new List<Player>();
            int currentWinner = 0;
            while (!winners.Any())
            {
                for (int i = 0; i < scenario.Places.Length; i++)
                {
                    if (scenario.Places[i] == currentWinner)
                    {
                        var player = scenarioIdToPlayer.First(o => o.Key == i).Value;
                        if (players.Contains(player))
                        {
                            winners.Add(player);
                        }
                    }
                }
                currentWinner++;
            }
            return winners;
        }

        private void CalculateEvStreetByStreet()
        {
            // init objects
            EvStreetByStreet = new ExpectedValueStreetByStreet();
            EvStreetByStreetFullInfo = new ExpectedValueStreetByStreet();

            // go to showdown & collect stacks for each street (before collection)
            decimal[][] stacksBeforeCollection = new decimal[4][];
            _table.UnDoCommandsAll();
            Street currentStreet = Street.Preflop;
            while (true)
            {
                var pokerCommand = _table.ToDo[0];
                if (pokerCommand is PokerCommands.Flop || pokerCommand is PokerCommands.Turn || pokerCommand is PokerCommands.River || pokerCommand is PokerCommands.FinalizePots)
                {
                    if (pokerCommand is PokerCommands.Flop) currentStreet = Street.Preflop;
                    if (pokerCommand is PokerCommands.Turn) currentStreet = Street.Flop;
                    if (pokerCommand is PokerCommands.River) currentStreet = Street.Turn;
                    if (pokerCommand is PokerCommands.FinalizePots) currentStreet = Street.River;

                    int street;
                    switch (currentStreet)
                    {
                        case Street.Preflop: street = 0; break;
                        case Street.Flop: street = 1; break;
                        case Street.Turn: street = 2; break;
                        case Street.River: street = 3; break;
                        default: street = int.MinValue; break;
                    }
                    stacksBeforeCollection[street] = new decimal[_table.PlayerCount];
                    for (int player = 0; player < _table.PlayerCount; player++) stacksBeforeCollection[street][player] = _table.Players[player].Stack;
                }

                _table.ToDoCommand();
                if (pokerCommand is PokerCommands.FinalizePots) break;
            }

            var pots = _table.PotsStreetByStreet;

            var potsPreflop = pots.Where(o => o.Street == Street.Preflop).ToList();
            var potsFlop = pots.Where(o => o.Street == Street.Preflop || o.Street == Street.Flop).ToList();
            var potsTurn = pots.Where(o => o.Street == Street.Preflop || o.Street == Street.Flop || o.Street == Street.Turn).ToList();
            var potsRiver = pots.Where(o => o.Street == Street.Preflop || o.Street == Street.Flop || o.Street == Street.Turn || o.Street == Street.River).ToList();

            if (potsPreflop.Any(o => o.Street == Street.Preflop))
            {
                Street street = Street.Preflop;
                ExpectedValue ev = new ExpectedValue { Street = street };
                ExpectedValue evFullInfo = new ExpectedValue { Street = street };
                CalculateEv(_evaluate, _table, street, _communityCards, potsPreflop, stacksBeforeCollection[0], StacksHandStart, IcmsHandEnd, IcmPayouts, ref ev, ref evFullInfo);
                EvStreetByStreet.Evs.Add(ev);
                EvStreetByStreetFullInfo.Evs.Add(evFullInfo);
            }
            if (potsFlop.Any(o => o.Street == Street.Flop))
            {
                Street street = Street.Flop;
                ExpectedValue ev = new ExpectedValue { Street = street };
                ExpectedValue evFullInfo = new ExpectedValue { Street = street };
                CalculateEv(_evaluate, _table, street, _communityCards, potsFlop, stacksBeforeCollection[1], StacksHandStart, IcmsHandEnd, IcmPayouts, ref ev, ref evFullInfo);
                EvStreetByStreet.Evs.Add(ev);
                EvStreetByStreetFullInfo.Evs.Add(evFullInfo);
            }
            if (potsTurn.Any(o => o.Street == Street.Turn))
            {
                Street street = Street.Turn;
                ExpectedValue ev = new ExpectedValue { Street = street };
                ExpectedValue evFullInfo = new ExpectedValue { Street = street };
                CalculateEv(_evaluate, _table, street, _communityCards, potsTurn, stacksBeforeCollection[2], StacksHandStart, IcmsHandEnd, IcmPayouts, ref ev, ref evFullInfo);
                EvStreetByStreet.Evs.Add(ev);
                EvStreetByStreetFullInfo.Evs.Add(evFullInfo);
            }
            if (potsRiver.Any(o => o.Street == Street.River))
            {
                Street street = Street.River;
                ExpectedValue ev = new ExpectedValue { Street = street };
                ExpectedValue evFullInfo = new ExpectedValue { Street = street };
                CalculateEv(_evaluate, _table, street, _communityCards, potsRiver, stacksBeforeCollection[3], StacksHandStart, IcmsHandEnd, IcmPayouts, ref ev, ref evFullInfo);
                EvStreetByStreet.Evs.Add(ev);
                EvStreetByStreetFullInfo.Evs.Add(evFullInfo);
            }

            // get weights
            var playersInvolved = pots.First(o => o.Players.Count == pots.Max(oo => oo.Players.Count)).Players.ToArray();
            double[] playersPots = new double[_table.PlayerCount];
            foreach (var ev in EvStreetByStreet.Evs)
            {
                ev.Weight = new double[_table.PlayerCount]; for (int i = 0; i < playersPots.Length; i++) if (!playersInvolved.Contains(_table.Players[i])) ev.Weight[i] = double.NaN;
                for (int i = 0; i < _table.PlayerCount; i++)
                {
                    foreach (var pot in pots.Where(o => o.Street == ev.Street && o.Players.Contains(_table.Players[i])))
                    {
                        ev.Weight[i] += (double)pot.Amount / pot.Players.Count;
                        playersPots[i] += (double)pot.Amount / pot.Players.Count;
                    }
                }
            }
            var totalPots = (double)pots.Sum(o => o.Amount);
            foreach (var ev in EvStreetByStreet.Evs)
            {
                var streetPots = ev.Weight.Where(o => !double.IsNaN(o)).Sum();
                for (int i = 0; i < _table.PlayerCount; i++)
                {
                    if (double.IsNaN(ev.Weight[i]))
                    {
                        ev.Weight[i] = streetPots / totalPots; // casual player
                    }
                    else
                    {
                        ev.Weight[i] = ev.Weight[i] / playersPots[i]; // involved player
                    }
                }
            }
            for (int i = 0; i < EvStreetByStreetFullInfo.Evs.Count; i++) EvStreetByStreetFullInfo.Evs[i].Weight = EvStreetByStreet.Evs[i].Weight.Clone() as double[];

            // get scenario weights
            foreach (var ev in EvStreetByStreet.Evs)
            {
                ev.ScenarioWeights = new double[ev.ScenarioPlaces.Length][];
                for (int s = 0; s < ev.ScenarioWeights.Length; s++)
                {
                    ev.ScenarioWeights[s] = new double[_table.PlayerCount];
                    for (int p = 0; p < _table.PlayerCount; p++)
                    {
                        ev.ScenarioWeights[s][p] = ev.ScenarioIcms[s][p] * ev.Weight[p];
                    }
                }
            }
            EvStreetByStreet.IcmsEv = new double[_table.PlayerCount];
            EvStreetByStreetFullInfo.IcmsEv = new double[_table.PlayerCount];
            EvStreetByStreet.IcmsDiffEv = new double[_table.PlayerCount];
            EvStreetByStreetFullInfo.IcmsDiffEv = new double[_table.PlayerCount];
            for (int p = 0; p < _table.PlayerCount; p++)
            {
                foreach (var ev in EvStreetByStreet.Evs)
                {
                    for (int s = 0; s < ev.ScenarioPlaces.GetLength(0); s++)
                    {
                        EvStreetByStreet.IcmsEv[p] += ev.ScenarioIcms[s][p] * ev.Weight[p] * ev.ScenarioProbabilities[s];
                    }
                }

                foreach (var ev in EvStreetByStreetFullInfo.Evs)
                {
                    for (int s = 0; s < ev.ScenarioPlaces.GetLength(0); s++)
                    {
                        EvStreetByStreetFullInfo.IcmsEv[p] += ev.ScenarioIcms[s][p] * ev.Weight[p] * ev.ScenarioProbabilities[s];
                    }
                }

                EvStreetByStreet.IcmsDiffEv[p] = EvStreetByStreet.IcmsEv[p] - IcmsHandEnd[p];
                EvStreetByStreetFullInfo.IcmsDiffEv[p] = EvStreetByStreetFullInfo.IcmsEv[p] - IcmsHandEnd[p];
            }

            // get chips ev
            EvStreetByStreet.ChipsEv = new decimal[_table.PlayerCount];
            EvStreetByStreetFullInfo.ChipsEv = new decimal[_table.PlayerCount];

            foreach (var ev in EvStreetByStreet.Evs)
            {
                for (int s = 0; s < ev.ScenarioProbabilities.Length; s++)
                {
                    for (int p = 0; p < _table.PlayerCount; p++)
                    {
                        EvStreetByStreet.ChipsEv[p] += ev.ScenarioStacks[s][p] * (decimal)ev.ScenarioProbabilities[s] * (decimal)ev.Weight[p];
                    }
                }
            }
            foreach (var ev in EvStreetByStreetFullInfo.Evs)
            {
                for (int s = 0; s < ev.ScenarioProbabilities.Length; s++)
                {
                    for (int p = 0; p < _table.PlayerCount; p++)
                    {
                        EvStreetByStreetFullInfo.ChipsEv[p] += ev.ScenarioStacks[s][p] * (decimal)ev.ScenarioProbabilities[s] * (decimal)ev.Weight[p];
                    }
                }
            }
            for (int p = 0; p < _table.PlayerCount; p++)
            {
                EvStreetByStreet.ChipsEv[p] = Math.Round(EvStreetByStreet.ChipsEv[p], 5);
                EvStreetByStreetFullInfo.ChipsEv[p] = Math.Round(EvStreetByStreetFullInfo.ChipsEv[p], 5);
            }
        }

        private void CalculateEvRegular()
        {
            // init objects
            EvRegular = new ExpectedValue { Street = EvRegularAllInStreet };
            EvRegularFullInfo = new ExpectedValue { Street = EvRegularAllInStreet };

            // go to showdown
            _table.UnDoCommandsAll(); while (!(_table.ToDo[0] is PokerCommands.FinalizePots)) _table.ToDoCommand(); _table.ToDoCommand();

            var pots = _table.Pots;

            // init stacks before showdown collection
            decimal[] stacksBeforeCollection = new decimal[_table.PlayerCount];
            for (int i = 0; i < _table.PlayerCount; i++) stacksBeforeCollection[i] = _table.Players[i].Stack;

            // calculate ev
            CalculateEv(_evaluate, _table, EvRegularAllInStreet, _communityCards, pots, stacksBeforeCollection, StacksHandStart, IcmsHandEnd, IcmPayouts, ref EvRegular, ref EvRegularFullInfo);

            // get weight
            EvRegular.Weight = new double[_table.PlayerCount];
            EvRegularFullInfo.Weight = new double[_table.PlayerCount];
            for (int i = 0; i < _table.PlayerCount; i++)
            {
                EvRegular.Weight[i] = 1;
                EvRegularFullInfo.Weight[i] = 1;
            }
        }

        private static void CalculateEv(Evaluate evaluate, Table table, Street street, string[] communityCards, List<Pot> pots,
            decimal[] stacksBeforeCollection, decimal[] stacksHandStart, double[] icmsHandEnd, decimal[] icmPayouts,
            ref ExpectedValue ev, ref ExpectedValue evFullInfo)
        {
            // get all possible players
            var players = pots.First(o => o.Players.Count == pots.Max(oo => oo.Players.Count)).Players;

            // create dictionary for { scenarioId; Player }
            Dictionary<int, Player> dicScenarioIndexToPlayer = new Dictionary<int, Player>();
            for (int i = 0; i < players.Count; i++) dicScenarioIndexToPlayer.Add(i, players[i]);

            // get pockets
            string[][] pocketCardsStr = new string[players.Count][];
            for (int i = 0; i < players.Count; i++) pocketCardsStr[i] = (players[i].PocketCards);

            // get all other known cards (dead cards)
            List<string> deadCards = new List<string>();
            foreach (var player in table.Players.Where(o => !players.Contains(o)).Where(player => player.PocketCards != null)) deadCards.AddRange(player.PocketCards.Where(o => o != null));

            // get community cards when on all-in street
            string[] communityCardsOnAllInStreet = new string[0];
            switch (street)
            {
                case Street.Flop: communityCardsOnAllInStreet = new[] { communityCards[0], communityCards[1], communityCards[2] }; break;
                case Street.Turn: communityCardsOnAllInStreet = new[] { communityCards[0], communityCards[1], communityCards[2], communityCards[3] }; break;
                case Street.River: communityCardsOnAllInStreet = new[] { communityCards[0], communityCards[1], communityCards[2], communityCards[3], communityCards[4] }; break;
            }

            // evaluate
            var evaluation = evaluate(pocketCardsStr, communityCardsOnAllInStreet, new string[0]);
            var evaluationFullInfo = evaluate(pocketCardsStr, communityCardsOnAllInStreet, deadCards.ToArray());

            // save scenarios
            ev.ScenarioPlaces = new int[evaluation.Scenarios.Length][];
            evFullInfo.ScenarioPlaces = new int[evaluationFullInfo.Scenarios.Length][];
            for (int i = 0; i < evaluation.Scenarios.Length; i++) ev.ScenarioPlaces[i] = evaluation.Scenarios[i].Places;
            for (int i = 0; i < evaluationFullInfo.Scenarios.Length; i++) evFullInfo.ScenarioPlaces[i] = evaluationFullInfo.Scenarios[i].Places;

            // set odds (visual purpose)
            ev.Odds = new double[table.PlayerCount];
            evFullInfo.Odds = new double[table.PlayerCount];
            for (int i = 0; i < table.PlayerCount; i++)
            {
                ev.Odds[i] = double.NaN;
                evFullInfo.Odds[i] = double.NaN;
            }
            for (int i = 0; i < evaluation.Odds.Length; i++)
            {
                ev.Odds[Array.IndexOf(table.Players, players[i])] = evaluation.Odds[i];
            }
            for (int i = 0; i < evaluationFullInfo.Odds.Length; i++)
            {
                evFullInfo.Odds[Array.IndexOf(table.Players, players[i])] = evaluationFullInfo.Odds[i];
            }

            // get probabilities/stacks/icms for scenarios
            ev.ScenarioProbabilities = new double[evaluation.Scenarios.Length];
            evFullInfo.ScenarioProbabilities = new double[evaluationFullInfo.Scenarios.Length];
            ev.ScenarioStacks = new decimal[evaluation.Scenarios.Length][];
            evFullInfo.ScenarioStacks = new decimal[evaluationFullInfo.Scenarios.Length][];
            ev.ScenarioIcms = new double[evaluation.Scenarios.Length][];
            evFullInfo.ScenarioIcms = new double[evaluationFullInfo.Scenarios.Length][];
            ev.IcmsEv = new double[table.PlayerCount];
            evFullInfo.IcmsEv = new double[table.PlayerCount];

            // regular
            for (int i = 0; i < evaluation.Scenarios.Length; i++)
            {
                ev.ScenarioProbabilities[i] = (double)evaluation.Scenarios[i].Count / evaluation.TotalBoardsEvaluated;
                ev.ScenarioStacks[i] = (decimal[])stacksBeforeCollection.Clone();

                foreach (var pot in pots)
                {
                    List<Player> playersInPot = FindWinnersAmongPlayers(evaluation.Scenarios[i], pot.Players, dicScenarioIndexToPlayer);
                    foreach (var player in playersInPot) ev.ScenarioStacks[i][Array.IndexOf(table.Players, player)] += pot.Amount / playersInPot.Count;
                }

                ev.ScenarioIcms[i] = Icm.GetEquity(ev.ScenarioStacks[i], stacksHandStart, icmPayouts);
                for (int j = 0; j < table.PlayerCount; j++)
                {
                    ev.IcmsEv[j] += ev.ScenarioIcms[i][j] * ev.ScenarioProbabilities[i];
                }
            }

            // regular full info
            for (int i = 0; i < evaluationFullInfo.Scenarios.Length; i++)
            {
                evFullInfo.ScenarioProbabilities[i] = (double)evaluationFullInfo.Scenarios[i].Count / evaluationFullInfo.TotalBoardsEvaluated;
                evFullInfo.ScenarioStacks[i] = (decimal[])stacksBeforeCollection.Clone();

                foreach (var pot in pots)
                {
                    List<Player> playersInPot = FindWinnersAmongPlayers(evaluationFullInfo.Scenarios[i], pot.Players, dicScenarioIndexToPlayer);
                    foreach (var player in playersInPot) evFullInfo.ScenarioStacks[i][Array.IndexOf(table.Players, player)] += pot.Amount / playersInPot.Count;
                }

                evFullInfo.ScenarioIcms[i] = Icm.GetEquity(evFullInfo.ScenarioStacks[i], stacksHandStart, icmPayouts);
                for (int j = 0; j < table.PlayerCount; j++)
                {
                    evFullInfo.IcmsEv[j] += evFullInfo.ScenarioIcms[i][j] * evFullInfo.ScenarioProbabilities[i];
                }
            }

            // get icm diff expected value
            ev.IcmsDiffEv = new double[table.PlayerCount];
            evFullInfo.IcmsDiffEv = new double[table.PlayerCount];
            for (int i = 0; i < table.PlayerCount; i++)
            {
                ev.IcmsDiffEv[i] = ev.IcmsEv[i] - icmsHandEnd[i];
                evFullInfo.IcmsDiffEv[i] = evFullInfo.IcmsEv[i] - icmsHandEnd[i];
            }

            // get chips ev
            ev.ChipsEv = new decimal[table.PlayerCount];
            evFullInfo.ChipsEv = new decimal[table.PlayerCount];
            for (int s = 0; s < evaluation.Scenarios.Length; s++)
            {
                for (int p = 0; p < table.PlayerCount; p++)
                {
                    ev.ChipsEv[p] += ev.ScenarioStacks[s][p] * (decimal)ev.ScenarioProbabilities[s];
                }
            }
            for (int s = 0; s < evaluationFullInfo.Scenarios.Length; s++)
            {
                for (int p = 0; p < table.PlayerCount; p++)
                {
                    evFullInfo.ChipsEv[p] += evFullInfo.ScenarioStacks[s][p] * (decimal)evFullInfo.ScenarioProbabilities[s];
                }
            }
            for (int p = 0; p < table.PlayerCount; p++)
            {
                ev.ChipsEv[p] = Math.Round(ev.ChipsEv[p], 5);
                evFullInfo.ChipsEv[p] = Math.Round(ev.ChipsEv[p], 5);
            }
        }

        //

        public class ExpectedValue
        {
            public Street Street;
            public double[] Odds;
            public int[][] ScenarioPlaces;
            public double[] ScenarioProbabilities;
            public decimal[][] ScenarioStacks;
            public double[][] ScenarioIcms;
            public double[] IcmsEv;
            public double[] IcmsDiffEv;
            public double[] Weight;
            public double[][] ScenarioWeights;
            public decimal[] ChipsEv;
        }

        public class ExpectedValueStreetByStreet
        {
            public List<ExpectedValue> Evs = new List<ExpectedValue>();
            public double[] IcmsEv;
            public double[] IcmsDiffEv;
            public decimal[] ChipsEv;
        }

        #region ToString()


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("---------------------------------------------------");
            sb.AppendLine();
            sb.AppendLine(string.Format("Total BuyIn: {0} {1:0.00}", _pokerHand.Currency, _pokerHand.TotalBuyIn));
            sb.AppendLine(string.Format("Prize Pool: {0} {1:0.00}", _pokerHand.Currency, PrizePool));
            StringBuilder sbPayouts = new StringBuilder(); foreach (var icmPayout in IcmPayouts) sbPayouts.Append(string.Format("{0:0.#####}, ", icmPayout)); sb.AppendLine(string.Format("Icm Payouts = {{ {0} }}", sbPayouts.ToString().TrimEnd(' ', ',')));
            sb.AppendLine();

            if (EvRegular != null)
            {
                sb.AppendLine("--------- Regular Ev ---------").AppendLine();
                sb.AppendLine(ToString(EvRegular));
            }

            if (EvRegularFullInfo != null)
            {
                sb.AppendLine("--------- Regular Ev (Full Info) ---------").AppendLine();
                sb.AppendLine(ToString(EvRegularFullInfo));
            }

            if (EvStreetByStreet != null)
            {
                sb.AppendLine("--------- Street By Street Ev ---------").AppendLine();
                sb.AppendLine(ToString(EvStreetByStreet));
            }

            if (EvStreetByStreetFullInfo != null)
            {
                sb.AppendLine("--------- Street By Street Ev (Full Info) ---------").AppendLine();
                sb.AppendLine(ToString(EvStreetByStreetFullInfo));
            }

            sb.AppendLine("---------------------------------------------------");
            return sb.ToString();
        }

        public string ToString(ExpectedValue ev)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < PlayerNames.Length; i++)
            {
                string pocketStr = ""; if (PocketCards[i] != null) pocketStr = PocketCards[i].Where(o => o != null).Aggregate(pocketStr, (a, b) => a + b);

                sb.AppendLine(string.Format("{0,-15} {3,-5} {4,-7} {1}{2:0.00} {5}",
                    PlayerNames[i],
                    EvRegular.IcmsDiffEv[i] > 0 ? "+" : "",
                    ev.IcmsDiffEv[i] * (double)PrizePool,
                    pocketStr,
                    double.IsNaN(EvRegular.Odds[i]) ? "" : string.Format("{0:0.00%}", ev.Odds[i]),
                    i == HeroId ? "(Hero)" : ""
                    ));
            }

            return sb.ToString();
        }

        public string ToString(ExpectedValueStreetByStreet evsbs)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < PlayerNames.Length; i++)
            {
                string pocketStr = ""; if (PocketCards[i] != null) pocketStr = PocketCards[i].Where(o => o != null).Aggregate(pocketStr, (a, b) => a + b);

                sb.AppendLine(string.Format("{0,-15} {3,-5} {4,-7} {1}{2:0.00} {5}",
                    PlayerNames[i],
                    EvStreetByStreet.IcmsDiffEv[i] > 0 ? "+" : "",
                    evsbs.IcmsDiffEv[i] * (double)PrizePool,
                    pocketStr,
                    "",
                    i == HeroId ? "(Hero)" : ""
                    ));
            }

            return sb.ToString();
        }

        #endregion
    }

    #region Icm

    public class Icm
    {
        class ComparerStacks : IComparer<int>
        {
            readonly decimal[] _startingStacks;

            public ComparerStacks(decimal[] startingStacks)
            {
                _startingStacks = startingStacks;
            }

            public int Compare(int x, int y)
            {
                decimal delta = _startingStacks[x] - _startingStacks[y];
                if (delta == 0)
                {
                    return 0;
                }
                if (delta < 0)
                {
                    return 1;
                }
                return -1;
            }
        }

        //Recursive method doing the actual calculation.
        private static double GetEquity(decimal[] payouts, decimal[] stacks, double total, int player, int depth)
        {
            double eq = (double)stacks[player] / total * (double)payouts[depth];
            if (depth + 1 < payouts.Length)
            {
                for (int i = 0; i < stacks.Length; i++)
                {
                    if (i != player && stacks[i] > 0)
                    {
                        double c = (double)stacks[i];
                        stacks[i] = 0;
                        eq += GetEquity(payouts, stacks, total - c, player, depth + 1) * c / total;
                        stacks[i] = (decimal)c;
                    }
                }
            }

            return eq;
        }

        /// <summary>
        /// Calculates ICM.
        /// </summary>
        /// <param name="stacks">Players stacks</param>
        /// <param name="startingStacks">Players starting stacks (needed in case few players busted ITM)</param>
        /// <param name="player">Index of selected player in the stack Array</param>
        /// <param name="payouts">Payout structure, eg.: new double[] {0.5, 0.3, 0.2}</param>
        /// <returns>ICM equity of selected player</returns>
        private static double GetEquity(decimal[] stacks, decimal[] startingStacks, int player, decimal[] payouts)
        {
            if (stacks[player] == 0)
            {
                List<int> deadStacks = new List<int>();
                List<int> aliveStacks = new List<int>();
                for (int i = 0; i < stacks.Length; i++)
                {
                    if (stacks[i] == 0) deadStacks.Add(i); else aliveStacks.Add(i);
                }
                deadStacks.Sort(new ComparerStacks(startingStacks));
                aliveStacks.Sort(new ComparerStacks(stacks));
                aliveStacks.AddRange(deadStacks);
                int place = aliveStacks.IndexOf(player);
                if (place >= payouts.Length)
                {
                    return 0;
                }
                return (double)payouts[place];
            }

            double total = stacks.Sum(t => (double)t);
            return GetEquity(payouts, stacks.Clone() as decimal[], total, player, 0);
        }

        public static double[] GetEquity(decimal[] stacks, decimal[] startingStacks, decimal[] payouts)
        {
            double[] icms = new double[stacks.Length];
            for (int i = 0; i < icms.Length; i++)
            {
                icms[i] = GetEquity(stacks, startingStacks, i, payouts);
            }
            return icms;
        }

    }

    #endregion
}

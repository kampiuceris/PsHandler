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

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PsHandler.PokerMath
{
    #region Evaluation / EvaluationPocket

    public class Evaluation
    {
        public int PlayerCount;
        public string[] BoardCardsStr;
        public string[] DeadCardsStr;
        public Scenario[] Scenarios;
        public long TotalBoardsEvaluated;
        public double[] Odds;

        public Evaluation(int playerCount, Scenario[] scenarios)
        {
            PlayerCount = playerCount;
            Scenarios = scenarios;
            TotalBoardsEvaluated = Scenarios.Sum(o => o.Count);
            CalculateOdds();
        }

        private void CalculateOdds()
        {
            Odds = new double[PlayerCount];
            foreach (var handEnding in Scenarios)
            {
                int winnerCount = handEnding.Places.Count(o => o == 0);
                for (int i = 0; i < handEnding.Places.Length; i++)
                {
                    if (handEnding.Places[i] == 0)
                    {
                        Odds[i] += (double)handEnding.Count / winnerCount;
                    }
                }
            }
            for (int i = 0; i < Odds.Length; i++)
            {
                Odds[i] = Odds[i] / TotalBoardsEvaluated;
            }
        }
    }

    public class EvaluationPocket : Evaluation
    {
        public string[][] PocketsStr;

        public EvaluationPocket(string[][] pocketsStr, string[] boardCardsStr, string[] deadCardsStr, Scenario[] scenarios)
            : base(pocketsStr.GetLength(0), scenarios)
        {
            PocketsStr = pocketsStr;
            BoardCardsStr = boardCardsStr;
            DeadCardsStr = deadCardsStr;
        }
    }

    #endregion

    #region Scenario / ScenarioNode

    public class Scenario
    {
        public int[] Places = null;
        public long Count = 0;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < Places.Length; i++)
            {
                sb.Append(string.Format("{0,-3}", Places[i]));
            }

            return string.Format("{0}, {1}", Count, sb);
        }
    }

    internal class ScenarioNode
    {
        private ScenarioNode[] _children;
        public int[] Places;
        public long Count = 0;

        public void Add(int[] places, int index, long count)
        {
            if (index == places.Length)
            {
                if (Places == null)
                {
                    Places = places.Clone() as int[];
                }
                Count += count;
            }
            else
            {
                if (_children == null)
                {
                    _children = new ScenarioNode[places.Length];
                }
                if (_children[places[index]] == null)
                {
                    _children[places[index]] = new ScenarioNode();
                }
                _children[places[index]].Add(places, index + 1, count);
            }
        }

        public void Add(int[] places, long count)
        {
            Add(places, 0, count);
        }

        public void Add(int[] places)
        {
            Add(places, 0, 1);
        }

        public List<ScenarioNode> CollectLeafs()
        {
            var leafs = new List<ScenarioNode>();
            CollectLeafs(ref leafs);
            //leafs.Sort((o0, o1) => o1.Count - o0.Count);
            leafs.Sort((o0, o1) => o1.Count > o0.Count ? 1 : o1.Count < o0.Count ? -1 : 0);
            return leafs;
        }

        private void CollectLeafs(ref List<ScenarioNode> leafs)
        {
            if (Count > 0)
            {
                leafs.Add(this);
            }
            else if (_children != null)
            {
                foreach (var child in _children)
                {
                    if (child != null)
                    {
                        child.CollectLeafs(ref leafs);
                    }
                }
            }
        }

        public static int[] Normalize(int[] values)
        {
            int[] places = new int[values.Length];
            int currentPlace = 0, lessThan = int.MaxValue, maxValue;
            bool wasFound = true;

            while (wasFound)
            {
                // find max value
                maxValue = 0;
                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i] > maxValue && values[i] < lessThan)
                    {
                        maxValue = values[i];
                    }
                }
                // check if not found to break loop
                if (maxValue == 0)
                {
                    wasFound = false;
                }
                // if found set places
                if (wasFound)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (values[i] == maxValue)
                        {
                            places[i] = currentPlace;
                        }
                    }
                    currentPlace++;
                    lessThan = maxValue;
                }
            }

            return places;
        }

        public Scenario[] CollectScenarios()
        {
            // collect leafs
            List<ScenarioNode> leafs = CollectLeafs();
            Scenario[] scenarios = new Scenario[leafs.Count];
            for (int i = 0; i < leafs.Count; i++)
            {
                scenarios[i] = new Scenario { Places = leafs[i].Places, Count = leafs[i].Count };
            }
            return scenarios;
        }
    }

    #endregion
}

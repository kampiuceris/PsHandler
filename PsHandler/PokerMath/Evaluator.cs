using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsHandler.PokerMath
{
    public class Evaluator : IDisposable
    {
        private int[] _HR;
        private static readonly string[] _HAND_RANKS = { "Hand Ranking Error!", "High Card", "Pair", "Two Pair", "Three of a Kind", "Straight", "Flush", "Full House", "Four of a Kind", "Straight Flush" };
        private static readonly string[] _MASKS = { null, "2c", "2d", "2h", "2s", "3c", "3d", "3h", "3s", "4c", "4d", "4h", "4s", "5c", "5d", "5h", "5s", "6c", "6d", "6h", "6s", "7c", "7d", "7h", "7s", "8c", "8d", "8h", "8s", "9c", "9d", "9h", "9s", "Tc", "Td", "Th", "Ts", "Jc", "Jd", "Jh", "Js", "Qc", "Qd", "Qh", "Qs", "Kc", "Kd", "Kh", "Ks", "Ac", "Ad", "Ah", "As" };
        private static readonly int[] _EMPTY_ARRAY_INT = new int[0];

        //

        /// <param name="handRankingDataFilePath">Path to hand ranking data file</param>
        public Evaluator(string handRankingDataFilePath)
        {
            _HR = new int[32487834];
            var fs = new FileStream(handRankingDataFilePath, FileMode.Open);
            var br = new BinaryReader(fs);

            for (int i = 0; i < 32487834; i++)
            {
                _HR[i] = br.ReadInt32();
            }

            fs.Close();
            br.Close();
        }

        /// <summary>
        /// Default hand ranking data file path = "hr.dat"
        /// </summary>
        public Evaluator()
            : this("hr.dat")
        {
        }

        public void Dispose()
        {
            _HR = null;
            GC.Collect();
        }

        //

        public static int GetMaskCard(char value, char suit)
        {
            int index;

            switch (value)
            {
                case '2':
                    index = 1;
                    break;
                case '3':
                    index = 5;
                    break;
                case '4':
                    index = 9;
                    break;
                case '5':
                    index = 13;
                    break;
                case '6':
                    index = 17;
                    break;
                case '7':
                    index = 21;
                    break;
                case '8':
                    index = 25;
                    break;
                case '9':
                    index = 29;
                    break;
                case 'T':
                    index = 33;
                    break;
                case 'J':
                    index = 37;
                    break;
                case 'Q':
                    index = 41;
                    break;
                case 'K':
                    index = 45;
                    break;
                case 'A':
                    index = 49;
                    break;
                default:
                    return int.MinValue;
            }

            switch (suit)
            {
                case 'c':
                    return index;
                case 'd':
                    return index + 1;
                case 'h':
                    return index + 2;
                case 's':
                    return index + 3;
                default:
                    return int.MinValue;
            }
        }

        public static int GetMaskCard(string cardStr)
        {
            return GetMaskCard(cardStr[0], cardStr[1]);
        }

        public static int[] GetMaskCards(string cardsStr)
        {
            int[] cards = new int[cardsStr.Length / 2];
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i] = GetMaskCard(cardsStr[i * 2], cardsStr[i * 2 + 1]);
            }
            return cards;
        }

        public static int[] GetMaskCards(string[] cardsStr)
        {
            int[] cards = new int[cardsStr.Length];
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i] = GetMaskCard(cardsStr[i]);
            }
            return cards;
        }

        public static string UnmaskCard(int card)
        {
            return _MASKS[card];
        }

        public static string UnmaskCards(int[] cards)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < cards.Length; i++)
            {
                sb.Append(UnmaskCard(i));
            }
            return sb.ToString();
        }

        //

        public static string GetHandRank(int hand)
        {
            return _HAND_RANKS[hand >> 12];
        }

        private static bool ArrayContains(int[] array, int target)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == target)
                {
                    return true;
                }
            }
            return false;
        }

        //

        public int LookUpHand7(int card0, int card1, int card2, int card3, int card4, int card5, int card6)
        {
            int pointer = _HR[53 + card0];
            pointer = _HR[pointer + card1];
            pointer = _HR[pointer + card2];
            pointer = _HR[pointer + card3];
            pointer = _HR[pointer + card4];
            pointer = _HR[pointer + card5];
            pointer = _HR[pointer + card6];
            return pointer;
        }

        public int LookUpHand5(int card0, int card1, int card2, int card3, int card4)
        {
            int pointer = _HR[53 + card0];
            pointer = _HR[pointer + card1];
            pointer = _HR[pointer + card2];
            pointer = _HR[pointer + card3];
            pointer = _HR[pointer + card4];
            return _HR[pointer];
        }

        //

        private void Evaluate5(ScenarioNode root, int[] possibleCards, int possibleCardCount, Pocket[] pockets)
        {
            int[] values = new int[pockets.Length];

            for (int c0 = 0; c0 < possibleCardCount - 4; c0++)
            {
                int card0 = possibleCards[c0];
                int u0 = _HR[53 + card0];
                for (int c1 = c0 + 1; c1 < possibleCardCount - 3; c1++)
                {
                    int card1 = possibleCards[c1];
                    int u1 = _HR[u0 + card1];
                    for (int c2 = c1 + 1; c2 < possibleCardCount - 2; c2++)
                    {
                        int card2 = possibleCards[c2];
                        int u2 = _HR[u1 + card2];
                        for (int c3 = c2 + 1; c3 < possibleCardCount - 1; c3++)
                        {
                            int card3 = possibleCards[c3];
                            int u3 = _HR[u2 + card3];
                            for (int c4 = c3 + 1; c4 < possibleCardCount; c4++)
                            {
                                int card4 = possibleCards[c4];
                                int u4 = _HR[u3 + card4];

                                for (int i = 0; i < pockets.Length; i++)
                                {
                                    int u5 = _HR[u4 + pockets[i].Card0];
                                    values[i] = _HR[u5 + pockets[i].Card1];
                                }
                                root.Add(ScenarioNode.Normalize(values), 1);
                            }
                        }
                    }
                }
            }
        }

        private void Evaluate4(ScenarioNode root, int[] possibleCards, int possibleCardCount, Pocket[] pockets, int[] boardCards)
        {
            int[] values = new int[pockets.Length];

            int card0 = boardCards[0];
            int u0 = _HR[53 + card0];

            for (int c1 = 0; c1 < possibleCardCount - 3; c1++)
            {
                int card1 = possibleCards[c1];
                int u1 = _HR[u0 + card1];
                for (int c2 = c1 + 1; c2 < possibleCardCount - 2; c2++)
                {
                    int card2 = possibleCards[c2];
                    int u2 = _HR[u1 + card2];
                    for (int c3 = c2 + 1; c3 < possibleCardCount - 1; c3++)
                    {
                        int card3 = possibleCards[c3];
                        int u3 = _HR[u2 + card3];
                        for (int c4 = c3 + 1; c4 < possibleCardCount; c4++)
                        {
                            int card4 = possibleCards[c4];
                            int u4 = _HR[u3 + card4];

                            for (int i = 0; i < pockets.Length; i++)
                            {
                                int u5 = _HR[u4 + pockets[i].Card0];
                                values[i] = _HR[u5 + pockets[i].Card1];
                            }
                            root.Add(ScenarioNode.Normalize(values), 1);
                        }
                    }
                }
            }
        }

        private void Evaluate3(ScenarioNode root, int[] possibleCards, int possibleCardCount, Pocket[] pockets, int[] boardCards)
        {
            int[] values = new int[pockets.Length];

            int card0 = boardCards[0];
            int u0 = _HR[53 + card0];

            int card1 = boardCards[1];
            int u1 = _HR[u0 + card1];

            for (int c2 = 0; c2 < possibleCardCount - 2; c2++)
            {
                int card2 = possibleCards[c2];
                int u2 = _HR[u1 + card2];
                for (int c3 = c2 + 1; c3 < possibleCardCount - 1; c3++)
                {
                    int card3 = possibleCards[c3];
                    int u3 = _HR[u2 + card3];
                    for (int c4 = c3 + 1; c4 < possibleCardCount; c4++)
                    {
                        int card4 = possibleCards[c4];
                        int u4 = _HR[u3 + card4];

                        for (int i = 0; i < pockets.Length; i++)
                        {
                            int u5 = _HR[u4 + pockets[i].Card0];
                            values[i] = _HR[u5 + pockets[i].Card1];
                        }
                        root.Add(ScenarioNode.Normalize(values), 1);
                    }
                }
            }
        }

        private void Evaluate2(ScenarioNode root, int[] possibleCards, int possibleCardCount, Pocket[] pockets, int[] boardCards)
        {
            int[] values = new int[pockets.Length];

            int card0 = boardCards[0];
            int u0 = _HR[53 + card0];

            int card1 = boardCards[1];
            int u1 = _HR[u0 + card1];


            int card2 = boardCards[2];
            int u2 = _HR[u1 + card2];

            for (int c3 = 0; c3 < possibleCardCount - 1; c3++)
            {
                int card3 = possibleCards[c3];
                int u3 = _HR[u2 + card3];
                for (int c4 = c3 + 1; c4 < possibleCardCount; c4++)
                {
                    int card4 = possibleCards[c4];
                    int u4 = _HR[u3 + card4];

                    for (int i = 0; i < pockets.Length; i++)
                    {
                        int u5 = _HR[u4 + pockets[i].Card0];
                        values[i] = _HR[u5 + pockets[i].Card1];
                    }
                    root.Add(ScenarioNode.Normalize(values), 1);
                }
            }
        }

        private void Evaluate1(ScenarioNode root, int[] possibleCards, int possibleCardCount, Pocket[] pockets, int[] boardCards)
        {
            int[] values = new int[pockets.Length];

            int card0 = boardCards[0];
            int u0 = _HR[53 + card0];

            int card1 = boardCards[1];
            int u1 = _HR[u0 + card1];

            int card2 = boardCards[2];
            int u2 = _HR[u1 + card2];

            int card3 = boardCards[3];
            int u3 = _HR[u2 + card3];

            for (int c4 = 0; c4 < possibleCardCount; c4++)
            {
                int card4 = possibleCards[c4];
                int u4 = _HR[u3 + card4];

                for (int i = 0; i < pockets.Length; i++)
                {
                    int u5 = _HR[u4 + pockets[i].Card0];
                    values[i] = _HR[u5 + pockets[i].Card1];
                }
                root.Add(ScenarioNode.Normalize(values), 1);
            }
        }

        private void Evaluate0(ScenarioNode root, Pocket[] pockets, int[] boardCards)
        {
            int[] values = new int[pockets.Length];

            int card0 = boardCards[0];
            int u0 = _HR[53 + card0];

            int card1 = boardCards[1];
            int u1 = _HR[u0 + card1];

            int card2 = boardCards[2];
            int u2 = _HR[u1 + card2];

            int card3 = boardCards[3];
            int u3 = _HR[u2 + card3];

            int card4 = boardCards[4];
            int u4 = _HR[u3 + card4];

            for (int i = 0; i < pockets.Length; i++)
            {
                int u5 = _HR[u4 + pockets[i].Card0];
                values[i] = _HR[u5 + pockets[i].Card1];
            }
            root.Add(ScenarioNode.Normalize(values), 1);
        }

        //

        private void EvaluateRange(ScenarioNode root, PocketRange[] pocketRanges, int playerId, Pocket[] pockets, int[] boardCards, int[] deadCards)
        {
            if (playerId < pocketRanges.Length)
            {
                foreach (var pocket in pocketRanges[playerId].Pockets)
                {
                    // check if pocket is possible (no duplicate cards)
                    bool pocketIsPossible = true;
                    for (int i = 0; i < playerId; i++) { if (pockets[i].Card0 == pocket.Card0 || pockets[i].Card0 == pocket.Card1 || pockets[i].Card1 == pocket.Card0 || pockets[i].Card1 == pocket.Card1) { pocketIsPossible = false; break; } }
                    foreach (var boardCard in boardCards) { if (boardCard == pocket.Card0 || boardCard == pocket.Card1) { pocketIsPossible = false; break; } }
                    foreach (var deadCard in deadCards) { if (deadCard == pocket.Card0 || deadCard == pocket.Card1) { pocketIsPossible = false; break; } }
                    // continue
                    if (pocketIsPossible)
                    {
                        pockets[playerId] = pocket;
                        EvaluateRange(root, pocketRanges, playerId + 1, pockets, boardCards, deadCards);
                    }
                }
            }
            else
            {
                foreach (var scenario in Evaluate(pockets, boardCards, deadCards).Scenarios)
                {
                    root.Add(scenario.Places, scenario.Count);
                }
            }
        }

        private void CollectNeedEvaluations(List<Pocket[]> neededEvaluations, PocketRange[] pocketRanges, int playerId, Pocket[] pockets, int[] boardCards, int[] deadCards)
        {
            if (playerId < pocketRanges.Length)
            {
                foreach (var pocket in pocketRanges[playerId].Pockets)
                {
                    // check if pocket is possible (no duplicate cards)
                    bool pocketIsPossible = true;
                    for (int i = 0; i < playerId; i++)
                    {
                        if (pockets[i].Card0 == pocket.Card0 || pockets[i].Card0 == pocket.Card1 || pockets[i].Card1 == pocket.Card0 || pockets[i].Card1 == pocket.Card1)
                        {
                            pocketIsPossible = false;
                            break;
                        }
                    }
                    foreach (var boardCard in boardCards)
                    {
                        if (boardCard == pocket.Card0 || boardCard == pocket.Card1)
                        {
                            pocketIsPossible = false;
                            break;
                        }
                    }
                    foreach (var deadCard in deadCards)
                    {
                        if (deadCard == pocket.Card0 || deadCard == pocket.Card1)
                        {
                            pocketIsPossible = false;
                            break;
                        }
                    }
                    // continue
                    if (pocketIsPossible)
                    {
                        pockets[playerId] = pocket;
                        CollectNeedEvaluations(neededEvaluations, pocketRanges, playerId + 1, pockets, boardCards, deadCards);
                    }
                }
            }
            else
            {
                neededEvaluations.Add(pockets.Clone() as Pocket[]);
            }
        }

        //

        public EvaluationPocket Evaluate(Pocket[] pockets, int[] boardCards, int[] deadCards)
        {
            int[] pocketCards = new int[pockets.Length * 2];
            for (int i = 0; i < pockets.Length; i++)
            {
                pocketCards[i * 2] = pockets[i].Card0;
                pocketCards[i * 2 + 1] = pockets[i].Card1;
            }
            int[] possibleCards = new int[52];
            int possibleCardCount = 0;

            int card;
            for (int i = 0; i < 52; i++)
            {
                card = i + 1;
                if (!ArrayContains(pocketCards, card) && !ArrayContains(boardCards, card) && !ArrayContains(deadCards, card))
                {
                    possibleCards[possibleCardCount] = card;
                    possibleCardCount++;
                }
            }

            ScenarioNode root = new ScenarioNode();

            switch (boardCards.Length)
            {
                case 0:
                    Evaluate5(root, possibleCards, possibleCardCount, pockets);
                    break;
                case 1:
                    Evaluate4(root, possibleCards, possibleCardCount, pockets, boardCards);
                    break;
                case 2:
                    Evaluate3(root, possibleCards, possibleCardCount, pockets, boardCards);
                    break;
                case 3:
                    Evaluate2(root, possibleCards, possibleCardCount, pockets, boardCards);
                    break;
                case 4:
                    Evaluate1(root, possibleCards, possibleCardCount, pockets, boardCards);
                    break;
                case 5:
                    Evaluate0(root, pockets, boardCards);
                    break;
                default:
                    throw new NotSupportedException("Board contains more than 5 cards.");
            }

            return new EvaluationPocket(pockets, boardCards, deadCards, root.CollectScenarios());
        }

        public EvaluationPocket Evaluate(Pocket[] pockets, int[] boardCards)
        {
            return Evaluate(pockets, boardCards, _EMPTY_ARRAY_INT);
        }

        public EvaluationPocket Evaluate(Pocket[] pockets)
        {
            return Evaluate(pockets, _EMPTY_ARRAY_INT, _EMPTY_ARRAY_INT);
        }

        //

        public EvaluationRange EvaluateSingleThread(PocketRange[] pocketRanges, int[] boardCards, int[] deadCards)
        {
            ScenarioNode root = new ScenarioNode();
            EvaluateRange(root, pocketRanges, 0, new Pocket[pocketRanges.Length], boardCards, deadCards);
            return new EvaluationRange(pocketRanges, boardCards, deadCards, root.CollectScenarios());
        }

        public EvaluationRange Evaluate(PocketRange[] pocketRanges, int[] boardCards, int[] deadCards)
        {
            List<Pocket[]> neededEvaluations = new List<Pocket[]>();
            Pocket[] temp = new Pocket[pocketRanges.Length];
            CollectNeedEvaluations(neededEvaluations, pocketRanges, 0, temp, boardCards, deadCards);

            object eLock = new object();
            ScenarioNode root = new ScenarioNode();
            Parallel.ForEach(neededEvaluations, pockets =>
            {
                var evaluation = Evaluate(pockets, boardCards, deadCards);
                lock (eLock)
                {
                    foreach (var scenario in evaluation.Scenarios)
                    {
                        root.Add(scenario.Places, scenario.Count);
                    }
                }
            });
            return new EvaluationRange(pocketRanges, boardCards, deadCards, root.CollectScenarios());
        }
    }

    #region Pocket / PocketRange

    public class Pocket
    {
        public int Card0;
        public int Card1;

        public Pocket(string pocketCardsStr)
        {
            Card0 = Evaluator.GetMaskCard(pocketCardsStr[0], pocketCardsStr[1]);
            Card1 = Evaluator.GetMaskCard(pocketCardsStr[2], pocketCardsStr[3]);
        }

        public Pocket(string[] pocketCardsStr)
        {
            Card0 = Evaluator.GetMaskCard(pocketCardsStr[0][0], pocketCardsStr[0][1]);
            Card1 = Evaluator.GetMaskCard(pocketCardsStr[1][0], pocketCardsStr[1][1]);
        }

        public override string ToString()
        {
            return string.Format("{0}{1}", Evaluator.UnmaskCard(Card0), Evaluator.UnmaskCard(Card1));
        }

        public static Pocket[] FromString(string pocketCardsStr)
        {
            pocketCardsStr = pocketCardsStr.Replace(" ", "");
            Pocket[] pockets = new Pocket[pocketCardsStr.Length / 4];
            for (int i = 0; i < pockets.Length; i++)
            {
                pockets[i] = new Pocket(pocketCardsStr.Substring(i * 4, 4));
            }
            return pockets;
        }
    }

    public class PocketRange
    {
        public Pocket[] Pockets;

        public PocketRange(Pocket[] pockets)
        {
            Pockets = pockets;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var pocket in Pockets)
            {
                sb.Append(Evaluator.UnmaskCard(pocket.Card0) + Evaluator.UnmaskCard(pocket.Card1) + " ");
            }
            return sb.ToString().TrimEnd(' ');
        }
    }

    #endregion

    #region Evaluation / EvaluationPocket / EvaluationRange

    public class Evaluation
    {
        public int PlayerCount;
        public int[] BoardCards;
        public int[] DeadCards;
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
        public Pocket[] Pockets;

        public EvaluationPocket(Pocket[] pockets, int[] boardCards, int[] deadCards, Scenario[] scenarios)
            : base(pockets.Length, scenarios)
        {
            Pockets = pockets;
            BoardCards = boardCards;
            DeadCards = deadCards;
        }
    }

    public class EvaluationRange : Evaluation
    {
        public PocketRange[] PocketRanges;

        public EvaluationRange(PocketRange[] pocketRanges, int[] boardCards, int[] deadCards, Scenario[] scenarios)
            : base(pocketRanges.Length, scenarios)
        {
            PocketRanges = pocketRanges;
            BoardCards = boardCards;
            DeadCards = deadCards;
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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsHandler.ChartsPreflop
{
    public class RangeConstructor
    {
        public readonly static string[] RANKING_STR = new[]
        {
            "AA", "AKs", "AKo", "AQs", "AQo", "AJs", "AJo", "ATs", "ATo", "A9s", "A9o", "A8s", "A8o", "A7s", "A7o", "A6s", "A6o", "A5s", "A5o", "A4s", "A4o", "A3s", "A3o", "A2s", "A2o", //25   0-24
            "KK", "KQs", "KQo", "KJs", "KJo", "KTs", "KTo", "K9s", "K9o", "K8s", "K8o", "K7s", "K7o", "K6s", "K6o", "K5s", "K5o", "K4s", "K4o", "K3s", "K3o", "K2s", "K2o", //23   25-47
            "QQ", "QJs", "QJo", "QTs", "QTo", "Q9s", "Q9o", "Q8s", "Q8o", "Q7s", "Q7o", "Q6s", "Q6o", "Q5s", "Q5o", "Q4s", "Q4o", "Q3s", "Q3o", "Q2s", "Q2o", //21   48-68
            "JJ", "JTs", "JTo", "J9s", "J9o", "J8s", "J8o", "J7s", "J7o", "J6s", "J6o", "J5s", "J5o", "J4s", "J4o", "J3s", "J3o", "J2s", "J2o", //19   69-87
            "TT", "T9s", "T9o", "T8s", "T8o", "T7s", "T7o", "T6s", "T6o", "T5s", "T5o", "T4s", "T4o", "T3s", "T3o", "T2s", "T2o", //17   88-104
            "99", "98s", "98o", "97s", "97o", "96s", "96o", "95s", "95o", "94s", "94o", "93s", "93o", "92s", "92o", //15   105-119
            "88", "87s", "87o", "86s", "86o", "85s", "85o", "84s", "84o", "83s", "83o", "82s", "82o", //13   120-132
            "77", "76s", "76o", "75s", "75o", "74s", "74o", "73s", "73o", "72s", "72o", //11   133-143
            "66", "65s", "65o", "64s", "64o", "63s", "63o", "62s", "62o", //9   144-152
            "55", "54s", "54o", "53s", "53o", "52s", "52o", //7   153-159
            "44", "43s", "43o", "42s", "42o", //5   160-164
            "33", "32s", "32o", //3   165-167
            "22" //1   168
        };
        public readonly static int[] RANKING = new[]
        {
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 
            30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 
            60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 
            90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 
            116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 
            140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 
            164, 165, 166, 167, 168
        };

        public readonly static string[] SUITES = { "c", "d", "h", "s" };
        public readonly static string[] VALUES = { "A", "K", "Q", "J", "T", "9", "8", "7", "6", "5", "4", "3", "2" };

        public static int IndexOfValue(char c)
        {
            switch (c)
            {
                case 'A': //Ace
                    return 0;
                case 'K': //King
                    return 1;
                case 'Q': //Queen
                    return 2;
                case 'J': //Jack
                    return 3;
                case 'T': //Ten
                    return 4;
                case '9': //Nine
                    return 5;
                case '8': //Eight
                    return 6;
                case '7': //Seven
                    return 7;
                case '6': //Six
                    return 8;
                case '5': //Five
                    return 9;
                case '4': //Four
                    return 10;
                case '3': //Three
                    return 11;
                case '2': //Two
                    return 12;
                default:
                    return Int32.MinValue;
            }
        }

        //

        public static IEnumerable<string> GetStr(string rangeExpression)
        {
            var split = rangeExpression.Split(new[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries);
            var range = new List<string>();

            // 77+ ATs+ AJo+ KQs
            // 22+ Qx+ J2s+ J5o+ T2s+ T6o+ 92s+ 96o+ 84s+ 86o+ 73s+ 75o+ 63s+ 65o 52s+ 54o 43s
            // 77+, A8s+, K9s+, QTs+, JTs, ATo+, KJo+

            foreach (var item in split)
            {
                if (item.Contains("-"))
                {
                    range.AddRangeUnique(GetJoined(item));
                }
                else if (!item.Contains("+"))
                {
                    // single addition
                    range.AddUnique(item);
                }
                else
                {
                    // multiple addition
                    if (item[0] == item[1])
                    {
                        // QQ+
                        range.AddRangeUnique(GetPocketPlus(item));
                    }
                    else if (item.ToLower()[1] == 'x')
                    {
                        // Qx+
                        range.AddRangeUnique(GetXPlus(item));
                    }
                    else
                    {
                        // Q5o+
                        range.AddRangeUnique(GetRegularPlus(item));
                    }
                }
            }

            return range;
        }

        public static IEnumerable<int> GetInt(string rangeExpression)
        {
            return GetStr(rangeExpression).Select(Parse).Where(pocketInt => pocketInt != -1);
        }

        public static string GetExpression(int[] range)
        {
            if (range.Length == 169) return "Any two";

            string[] rangeStr = new string[range.Length];
            for (int i = 0; i < range.Length; i++)
            {
                rangeStr[i] = UnParse(range[i]);
            }

            var expressions = new List<string>();

            expressions.AddRange(GetExpressions(rangeStr, true));

            foreach (var value in VALUES)
            {
                expressions.AddRange(GetExpressions(rangeStr, false, true, value[0]));
                expressions.AddRange(GetExpressions(rangeStr, false, false, value[0]));
            }

            var sb = new StringBuilder();
            foreach (var expression in expressions)
            {
                sb.Append(expression + ", ");
            }
            return sb.ToString().TrimEnd(',', ' ');
        }

        // Parse {string hand} to {int hand}
        public static int Parse(string handStr)
        {
            int hand = Array.IndexOf(RANKING_STR, handStr);
            if (hand < 0 && hand >= RANKING_STR.Length) throw new Exception("Error ParseHand(string '" + handStr + "')");
            return hand;
        }

        // Parse range {string[] range} to {int[] range}
        public static int[] Parse(string[] rangeStr)
        {
            int[] range = new int[rangeStr.Length];
            for (int i = 0; i < rangeStr.Length; i++)
            {
                range[i] = Parse(rangeStr[i]);
            }
            return range;
        }

        // UnParse {int hand} to {string hand}
        public static string UnParse(int hand)
        {
            if (hand < 0 && hand >= RANKING_STR.Length) throw new Exception("Error HandToString(int '" + hand + "')");
            return RANKING_STR[hand];
        }

        // UnParse range {int[] range} to {string[] range}
        public static string[] UnParse(int[] hands)
        {
            string[] unparsed = new string[hands.Length];
            for (int i = 0; i < hands.Length; i++)
            {
                unparsed[i] = UnParse(hands[i]);
            }
            return unparsed;
        }

        public static void GetRangeFrequency(int[] range, out double frequency, out int hands)
        {
            double sumFrequency = 0;
            int sumHands = 0;

            for (int hand = 0; hand < range.Length; hand++)
            {
                var handStr = UnParse(range[hand]);
                if (handStr[0] == handStr[1])
                {
                    sumFrequency += 6.0 / 1326.0;
                    sumHands += 6;
                }
                else if (handStr[2] == 'o')
                {
                    sumFrequency += 12.0 / 1326.0;
                    sumHands += 12;
                }
                else
                {
                    sumFrequency += 4.0 / 1326.0;
                    sumHands += 4;
                }
            }

            frequency = sumFrequency;
            hands = sumHands;
        }

        //

        private static string NormalizeItem(string item)
        {
            return IndexOfValue(item[0]) > IndexOfValue(item[1]) ? string.Format("{0}{1}{2}", item[1], item[0], (item.Length == 3 ? item[2].ToString(CultureInfo.InvariantCulture) : "")) : item;
        }

        private static IEnumerable<string> GetPocketPlus(string item)
        {
            List<string> range = new List<string>();

            string t = item.Replace("+", "");
            for (int i = 0; i <= IndexOfValue(item[0]); i++)
            {
                range.Add(NormalizeItem(VALUES[i] + VALUES[i]));
            }

            return range;
        }

        private static IEnumerable<string> GetXPlus(string item)
        {
            List<string> range = new List<string>();

            for (int i = 0; i <= IndexOfValue(item[0]); i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    if (i != j) // no pockets
                    {
                        range.Add(NormalizeItem(VALUES[i] + VALUES[j] + "s"));
                        range.Add(NormalizeItem(VALUES[i] + VALUES[j] + "o"));
                    }
                }
            }

            return range;
        }

        private static IEnumerable<string> GetJoined(string item)
        {
            List<string> range = new List<string>();

            if (item[0] == item[1])
            {
                // pockets
                for (int i = IndexOfValue(item[1]); i <= IndexOfValue(item[4]); i++)
                {
                    range.Add(NormalizeItem(VALUES[i] + VALUES[i]));
                }
            }
            else
            {
                // offsuited / suited
                for (int i = IndexOfValue(item[1]); i <= IndexOfValue(item[5]); i++)
                {
                    range.Add(NormalizeItem(item[0] + VALUES[i]) + item[2]);
                }
            }

            return range;
        }

        private static IEnumerable<string> GetRegularPlus(string item)
        {
            List<string> range = new List<string>();

            bool suited = item[2] == 's';
            int i0 = IndexOfValue(item[0]);

            for (int i = i0 + 1; i <= IndexOfValue(item[1]); i++)
            {
                range.Add(NormalizeItem(VALUES[i0] + VALUES[i] + (suited ? "s" : "o")));
            }

            return range;
        }

        private static IEnumerable<string> GetExpressions(IEnumerable<string> rangeStr, bool isPair, bool isSuited = false, char value = 'A')
        {
            var expressions = new List<string>();

            List<string> filteredRangeStr;
            if (isPair)
            {
                filteredRangeStr = rangeStr.Where(a => a.Length == 2).ToList();
            }
            else
            {
                filteredRangeStr = rangeStr.Where(a => a.Length == 3 && a[0] == value && a[2] == (isSuited ? 's' : 'o')).ToList();
            }

            if (filteredRangeStr.Any())
            {
                filteredRangeStr.Sort((a, b) => IndexOfValue(a[1]) - IndexOfValue(a[1]));

                // make parts
                var parts = new List<List<string>>();
                var newPart = new List<string> { filteredRangeStr[0] };
                for (int i = 1; i < filteredRangeStr.Count; i++)
                {
                    if (IndexOfValue(filteredRangeStr[i][1]) - IndexOfValue(newPart.Last()[1]) == 1)
                    {
                        // continue part
                        newPart.Add(filteredRangeStr[i]);
                    }
                    else
                    {
                        // break part
                        parts.Add(newPart);
                        newPart = new List<string> { filteredRangeStr[i] };
                    }
                }
                parts.Add(newPart);

                // add parts to expression list
                foreach (var part in parts)
                {
                    if (part.Count == 1)
                    {
                        expressions.Add(string.Format("{0}", part.First()));
                    }
                    else
                    {
                        if (IndexOfValue(part.First()[1]) == (isPair ? IndexOfValue('A') : IndexOfValue(value)) + (isPair ? 0 : 1))
                        {
                            expressions.Add(string.Format("{0}+", part.Last()));
                        }
                        else
                        {
                            expressions.Add(string.Format("{0}-{1}", part.First(), part.Last()));
                        }
                    }
                }
            }

            return expressions;
        }

        //

        public static string PocketFullToShort(string pocketFull)
        {
            string p = pocketFull;
            if (IndexOfValue(pocketFull[0]) > IndexOfValue(pocketFull[2]))
            {
                p = p.Substring(2, 2) + p.Substring(0, 2);
            }
            if (p[0] == p[2])
            {
                return string.Format("{0}{1}", p[0], p[2]);
            }
            if (p[1] != p[3])
            {
                return string.Format("{0}{1}o", p[0], p[2]);
            }
            return string.Format("{0}{1}s", p[0], p[2]);
        }
    }

    public static class RangeConstructorExtensionMethods
    {
        public static bool AddUnique<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
                return true;
            }
            return false;
        }

        public static void AddRangeUnique<T>(this List<T> list, IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                list.AddUnique(item);
            }
        }
    }
}

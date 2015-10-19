using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsHandler.ChartsPreflop
{
    public class ChartPreflop
    {
        private const string REGEX_TITLE_MTT = "";

        public enum PositionId
        {
            Any = -1,
            EP = 0,
            MP = 1,
            CO = 2,
            BU = 3,
            SB = 4,
            BB = 5,
        }

        public enum StackRelationshipToAverageStackId
        {
            SmallerThanAverage = 0,
            AroundAverage = 1,
            BiggerThanAverage = 2,
        }

        public string RangeExpression = "";
        public string RegexTitle = "";
        public int? Players;
        public PositionId? Position;
        public decimal? BigBlindsGreaterThanOrEqual;
        public decimal? BigBlindsLessThan;
        public decimal? TournamentMGreaterThanOrEqual;
        public decimal? TournamentMLessThan;

        public static IEnumerable<ChartPreflop> GetDefaultValues()
        {
            var chartsPreflopDefault = new List<ChartPreflop>();

            #region MTT

            #region MTT 0-6bb

            #region MTT 10 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "19.7%, 33+ A2s+ A8o+ K9s+ KJo+ Q9s+ J9s+ T9s 98s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 10,
                Position = PositionId.EP,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "29.7%, 22+ A2s+ A3o+ K6s+ KTo+ Q8s+ QTo+ J9s+ JTo T8s+ 98s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 10,
                Position = PositionId.MP,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "39.8%, 22+ Ax+ K2s+ K6o+ Q5s+ Q9o+ J7s+ J9o+ T7s+ 97s+ 87s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 10,
                Position = PositionId.CO,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "47.8%, 22+ Ax+ K2s+ K3o+ Q2s+ Q7o+ J6s+ J8o+ T7s+ T9o 97s+ 87s 76s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 10,
                Position = PositionId.BU,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "77.7%, 22+ Jx+ T2s+ T4o+ 92s+ 95o+ 84s+ 86o+ 74s+ 76o 63s+ 65o 53s+ 43s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 10,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            #endregion

            #region MTT 9 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "20.7%, 22+ A2s+ A8o+ K9s+ KTo+ Q9s+ J9s+ T9s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 9,
                Position = PositionId.EP,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "29.5%, 22+ A2s+ A3o+ K6s+ KTo+ Q8s+ QTo+ J9s+ JTo T8s+ 98s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 9,
                Position = PositionId.MP,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "39.4%, 22+ Ax+ K2s+ K6o+ Q6s+ Q9o+ J7s+ J9o+ T7s+ 97s+ 87s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 9,
                Position = PositionId.CO,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "46.4%, 22+ Ax+ K2s+ K3o+ Q3s+ Q8o+ J6s+ J8o+ T7s+ T9o 97s+ 87s 76s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 9,
                Position = PositionId.BU,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "76.4%, 22+ Jx+ T2s+ T4o+ 92s+ 96o+ 84s+ 86o+ 74s+ 76o 63s+ 65o 53s+ 43s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 9,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            #endregion

            #region MTT 8 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "21.8%, 22+ A2s+ A8o+ K9s+ KTo+ Q9s+ QJo J9s+ T9s 98s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 8,
                Position = PositionId.EP,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "29.2%, 22+ A2s+ A3o+ K6s+ KTo+ Q9s+ QTo+ J9s+ JTo T8s+ 98s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 8,
                Position = PositionId.MP,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "38.4%, 22+ Ax+ K2s+ K7o+ Q6s+ Q9o+ J7s+ J9o+ T7s+ 97s+ 87s 76s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 8,
                Position = PositionId.CO,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "46.3%, 22+ Ax+ K2s+ K3o+ Q3s+ Q8o+ J7s+ J8o+ T7s+ T9o 97s+ 86s+ 76s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 8,
                Position = PositionId.BU,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "76.8%, 22+ Jx+ T2s+ T4o+ 92s+ 96o+ 84s+ 86o+ 74s+ 76o 63s+ 65o 53s+ 43s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 8,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            #endregion

            #region MTT 7 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "24.3%, 22+ A2s+ A7o+ A5o K7s+ KTo+ Q9s+ QJo J9s+ T9s 98s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 7,
                Position = PositionId.EP,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "30.3%, 22+ Ax+ K6s+ K9o+ Q8s+ QTo+ J9s+ T9s 98s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 7,
                Position = PositionId.MP,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "37.5%, 22+ Ax+ K2s+ K7o+ Q6s+ Q9o+ J7s+ JTo T7s+ 97s+ 87s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 7,
                Position = PositionId.CO,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "45.7%, 22+ Ax+ K2s+ K3o+ Q3s+ Q8o+ J6s+ J9o+ T7s+ T9o 97s+ 86s+ 76s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 7,
                Position = PositionId.BU,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "76.8%, 22+ Jx+ T2s+ T4o+ 92s+ 96o+ 84s+ 86o+ 74s+ 76o 63s+ 65o 53s+ 43s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 7,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            #endregion

            #region MTT 6 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "27.7%, 22+ A2s+ A4o+ K6s+ KTo+ Q9s+ QTo+ J9s+ T8s+ 98s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 6,
                Position = PositionId.EP,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "32.3%, 22+ Ax+ K5s+ K9o+ Q8s+ QTo+ J8s+ JTo T8s+ 98s 87s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 6,
                Position = PositionId.MP,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "37.2%, 22+ Ax+ K2s+ K7o+ Q6s+ Q9o+ J8s+ JTo T8s+ 97s+ 87s 76s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 6,
                Position = PositionId.CO,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "45.3%, 22+ Ax+ K2s+ K3o+ Q3s+ Q8o+ J7s+ J9o+ T7s+ T9o 97s+ 86s+ 76s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 6,
                Position = PositionId.BU,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "75.4%, 22+ Jx+ T2s+ T5o+ 92s+ 96o+ 84s+ 86o+ 74s+ 76o 63s+ 53s+ 43s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 6,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            #endregion

            #region MTT 5 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "32.3%, 22+ Ax+ K5s+ K9o+ Q8s+ QTo+ J8s+ JTo T8s+ 98s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 5,
                Position = PositionId.MP,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "37.1%, 22+ Ax+ K2s+ K7o+ Q6s+ Q9o+ J8s+ JTo T8s+ 97s+ 87s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 5,
                Position = PositionId.CO,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "44.0%, 22+ Ax+ K2s+ K4o+ Q4s+ Q8o+ J7s+ J9o+ T7s+ T9o 97s+ 87s 76s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 5,
                Position = PositionId.BU,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "75.8%, 22+ Jx+ T2s+ T5o+ 92s+ 96o+ 84s+ 86o+ 74s+ 76o 63s+ 65o 53s+ 43s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 5,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            #endregion

            #region MTT 4 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "36.0%, 22+ Ax+ K2s+ K8o+ Q6s+ Q9o+ J8s+ JTo T8s+ 97s+ 87s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 4,
                Position = PositionId.CO,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "43.9%, 22+ Ax+ K2s+ K4o+ Q4s+ Q8o+ J7s+ J9o+ T7s+ T9o 97s+ 87s 76s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 4,
                Position = PositionId.BU,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "73.9%, 22+ Jx+ T2s+ T6o+ 93s+ 96o+ 84s+ 86o+ 74s+ 76o 63s+ 53s+ 43s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 4,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            #endregion

            #region MTT 3 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "43.9%, 22+ Ax+ K2s+ K4o+ Q4s+ Q8o+ J7s+ J9o+ T7s+ T9o 97s+ 87s 76s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 3,
                Position = PositionId.BU,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "73.2%, 22+ Jx+ T2s+ T6o+ 93s+ 96o+ 84s+ 86o+ 74s+ 76o 64s+ 53s+ ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 3,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            #endregion

            #region MTT 2 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "73.2%, 22+ Jx+ T2s+ T6o+ 93s+ 96o+ 84s+ 86o+ 74s+ 76o 64s+ 53s+ ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 2,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 0,
                BigBlindsLessThan = 6,
            });

            #endregion

            #endregion

            #region MTT 6-12bb

            #region MTT 10 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "15.2%, 33+ A7s+ A5s ATo+ K9s+ KQo Q9s+ J9s+ T9s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 10,
                Position = PositionId.EP,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "23.7%, 22+ A2s+ A7o+ K9s+ KTo+ Q9s+ QJo J8s+ T8s+ 98s 87s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 10,
                Position = PositionId.MP,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "34.2%, 22+ Ax+ K4s+ K9o+ Q8s+ QTo+ J8s+ JTo T7s+ 97s+ 86s+ 76s 65s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 10,
                Position = PositionId.CO,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "43.9%, 22+ Ax+ K2s+ K5o+ Q4s+ Q9o+ J7s+ J9o+ T6s+ T9o 96s+ 86s+ 75s+ 65s 54s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 10,
                Position = PositionId.BU,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "71.9%, 22+ Qx+ J2s+ J5o+ T2s+ T6o+ 93s+ 96o+ 84s+ 86o+ 74s+ 76o 63s+ 65o 53s+ 43s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 10,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            #endregion

            #region MTT 9 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "15.7%, 33+ A7s+ A5s-A4s ATo+ K9s+ KQo Q9s+ J9s+ T9s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 9,
                Position = PositionId.EP,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "22.6%, 22+ A2s+ A8o+ K8s+ KJo+ Q9s+ QJo J8s+ T8s+ 98s 87s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 9,
                Position = PositionId.MP,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "33.9%, 22+ Ax+ K5s+ K9o+ Q8s+ QTo+ J8s+ JTo T7s+ 97s+ 86s+ 76s 65s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 9,
                Position = PositionId.CO,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "42.7%, 22+ Ax+ K2s+ K6o+ Q5s+ Q9o+ J7s+ J9o+ T6s+ T9o 96s+ 86s+ 75s+ 65s 54s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 9,
                Position = PositionId.BU,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "71.0%, 22+ Qx+ J2s+ J5o+ T2s+ T7o+ 93s+ 96o+ 84s+ 86o+ 74s+ 76o 63s+ 65o 53s+ 43s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 9,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            #endregion

            #region MTT 8 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "16.2%, 33+ A7s+ A5s-A4s ATo+ K9s+ KJo+ Q9s+ J9s+ T9s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 8,
                Position = PositionId.EP,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "22.2%, 22+ A2s+ A8o+ K8s+ KJo+ Q9s+ QJo J8s+ T8s+ 98s 87s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 8,
                Position = PositionId.MP,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "32.8%, 22+ Ax+ K5s+ KTo+ Q8s+ QTo+ J8s+ JTo T7s+ 97s+ 86s+ 76s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 8,
                Position = PositionId.CO,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "41.3%, 22+ Ax+ K2s+ K7o+ Q5s+ Q9o+ J7s+ J9o+ T7s+ T9o 96s+ 86s+ 75s+ 65s 54s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 8,
                Position = PositionId.BU,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "69.2%, 22+ Qx+ J2s+ J6o+ T2s+ T7o+ 93s+ 97o+ 84s+ 86o+ 74s+ 76o 63s+ 65o 53s+ 43s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 8,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            #endregion

            #region MTT 7 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "18.0%, 22+ A3s+ ATo+ K9s+ KJo+ Q9s+ J9s+ T8s+ 98s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 7,
                Position = PositionId.EP,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "24.6%, 22+ A2s+ A8o+ K7s+ KTo+ Q8s+ QJo J8s+ JTo T8s+ 98s 87s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 7,
                Position = PositionId.MP,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "32.7%, 22+ Ax+ K5s+ KTo+ Q8s+ QTo+ J8s+ JTo T7s+ 97s+ 86s+ 76s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 7,
                Position = PositionId.CO,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "40.8%, 22+ Ax+ K2s+ K7o+ Q5s+ Q9o+ J7s+ JTo T7s+ T9o 96s+ 86s+ 75s+ 65s 54s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 7,
                Position = PositionId.BU,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "68.9%, 22+ Qx+ J2s+ J6o+ T2s+ T7o+ 93s+ 97o+ 84s+ 86o+ 74s+ 76o 63s+ 65o 53s+ 43s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 7,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            #endregion

            #region MTT 6 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "20.8%, 22+ A2s+ A9o+ K8s+ KJo+ Q9s+ QJo J8s+ T8s+ 98s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 6,
                Position = PositionId.EP,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "25.5%, 22+ A2s+ A7o+ A5o K9s+ KTo+ Q9s+ QJo J8s+ JTo T8s+ 98s 87s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 6,
                Position = PositionId.MP,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "32.4%, 22+ Ax+ K5s+ KTo+ Q8s+ QTo+ J8s+ JTo T7s+ 97s+ 87s 76s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 6,
                Position = PositionId.CO,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "39.5%, 22+ Ax+ K2s+ K8o+ Q5s+ Q9o+ J7s+ JTo T7s+ T9o 96s+ 86s+ 75s+ 65s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 6,
                Position = PositionId.BU,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "67.7%, 22+ Qx+ J2s+ J7o+ T3s+ T7o+ 94s+ 97o+ 84s+ 86o+ 74s+ 76o 63s+ 65o 53s+ 43s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 6,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            #endregion

            #region MTT 5 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "25.2%, 22+ A2s+ A7o+ A5o K9s+ KTo+ Q9s+ QJo J8s+ JTo T8s+ 98s 87s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 5,
                Position = PositionId.MP,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "31.8%, 22+ Ax+ K6s+ KTo+ Q8s+ QTo+ J8s+ JTo T8s+ 97s+ 87s 76s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 5,
                Position = PositionId.CO,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "38.3%, 22+ Ax+ K2s+ K8o+ Q6s+ QTo+ J7s+ JTo T7s+ T9o 96s+ 86s+ 75s+ 65s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 5,
                Position = PositionId.BU,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "65.6%, 22+ Kx+ Q2s+ Q3o+ J2s+ J7o+ T3s+ T7o+ 95s+ 97o+ 84s+ 86o+ 74s+ 76o 63s+ 53s+ 43s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 5,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            #endregion

            #region MTT 4 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "30.8%, 22+ A2s+ A3o+ K6s+ KTo+ Q8s+ QTo+ J8s+ JTo T8s+ 97s+ 87s 76s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 4,
                Position = PositionId.CO,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "36.7%, 22+ Ax+ K3s+ K9o+ Q6s+ QTo+ J7s+ JTo T7s+ T9o 96s+ 86s+ 76s 65s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 4,
                Position = PositionId.BU,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "63.5%, 22+ Kx+ Q2s+ Q4o+ J2s+ J7o+ T3s+ T7o+ 95s+ 97o+ 84s+ 87o 74s+ 76o 64s+ 53s+ 43s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 4,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            #endregion

            #region MTT 3 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "35.4%, 22+ Ax+ K4s+ K9o+ Q8s+ QTo+ J7s+ JTo T7s+ T9o 97s+ 86s+ 76s 65s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 3,
                Position = PositionId.BU,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "63.5%, 22+ Kx+ Q2s+ Q4o+ J2s+ J7o+ T4s+ T7o+ 95s+ 97o+ 84s+ 87o 74s+ 76o 63s+ 53s+ 43s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 3,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            #endregion

            #region MTT 2 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "61.7%, 22+ Kx+ Q2s+ Q5o+ J2s+ J8o+ T4s+ T7o+ 95s+ 97o+ 84s+ 87o 74s+ 76o 63s+ 53s+ 43s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 2,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 6,
                BigBlindsLessThan = 12,
            });

            #endregion

            #endregion

            #region MTT 12-18bb

            #region MTT 10 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "10.9%, 55+ A9s+ A5s AJo+ KTs+ QTs+ JTs ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 10,
                Position = PositionId.EP,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "18.4%, 33+ A3s+ ATo+ K9s+ KJo+ Q9s+ QJo J9s+ T9s 98s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 10,
                Position = PositionId.MP,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "29.7%, 22+ A2s+ A4o+ K7s+ KTo+ Q8s+ QTo+ J8s+ JTo T8s+ 97s+ 87s 76s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 10,
                Position = PositionId.CO,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "36.7%, 22+ Ax+ K4s+ K9o+ Q6s+ QTo+ J7s+ JTo T7s+ T9o 96s+ 86s+ 75s+ 65s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 10,
                Position = PositionId.BU,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "61.7%, 22+ Kx+ Q2s+ Q5o+ J2s+ J8o+ T4s+ T7o+ 95s+ 97o+ 84s+ 87o 74s+ 76o 63s+ 53s+ 43s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 10,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            #endregion

            #region MTT 9 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "11.5%, 55+ A9s+ AJo+ KTs+ KQo QTs+ JTs ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 9,
                Position = PositionId.EP,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "16.9%, 33+ A7s+ A5s-A4s ATo+ K9s+ KJo+ Q9s+ J9s+ T9s 98s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 9,
                Position = PositionId.MP,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "29.4%, 22+ A2s+ A4o+ K7s+ KTo+ Q8s+ QTo+ J8s+ JTo T8s+ 97s+ 87s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 9,
                Position = PositionId.CO,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "36.3%, 22+ Ax+ K4s+ K9o+ Q6s+ QTo+ J7s+ JTo T7s+ T9o 96s+ 86s+ 76s 65s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 9,
                Position = PositionId.BU,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "60.4%, 22+ Kx+ Q2s+ Q6o+ J3s+ J8o+ T4s+ T7o+ 95s+ 97o+ 84s+ 87o 74s+ 76o 63s+ 53s+ 43s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 9,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            #endregion

            #region MTT 8 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "11.8%, 55+ A9s+ A5s AJo+ KTs+ KQo QTs+ JTs ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 8,
                Position = PositionId.EP,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "16.7%, 33+ A7s+ A5s-A4s ATo+ K9s+ KJo+ Q9s+ J9s+ T9s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 8,
                Position = PositionId.MP,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "27.1%, 22+ A2s+ A7o+ A5o K8s+ KTo+ Q8s+ QTo+ J8s+ JTo T8s+ 98s 87s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 8,
                Position = PositionId.CO,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "35.3%, 22+ Ax+ K4s+ KTo+ Q8s+ QTo+ J7s+ JTo T7s+ T9o 96s+ 86s+ 76s 65s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 8,
                Position = PositionId.BU,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "60.2%, 22+ Kx+ Q2s+ Q6o+ J3s+ J8o+ T4s+ T7o+ 95s+ 97o+ 84s+ 87o 74s+ 76o 64s+ 53s+ 43s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 8,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            #endregion

            #region MTT 7 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "13.7%, 44+ A8s+ A5s AJo+ K9s+ KQo Q9s+ J9s+ T9s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 7,
                Position = PositionId.EP,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "17.9%, 22+ A3s+ ATo+ K9s+ KJo+ Q9s+ J9s+ T9s 98s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 7,
                Position = PositionId.MP,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "26.5%, 22+ A2s+ A7o+ A5o K7s+ KTo+ Q8s+ QJo J8s+ JTo T8s+ 98s 87s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 7,
                Position = PositionId.CO,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "34.4%, 22+ Ax+ K4s+ KTo+ Q8s+ QTo+ J7s+ JTo T7s+ T9o 97s+ 86s+ 76s 65s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 7,
                Position = PositionId.BU,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "59.3%, 22+ Kx+ Q2s+ Q7o+ J3s+ J8o+ T4s+ T7o+ 95s+ 97o+ 84s+ 87o 74s+ 76o 64s+ 53s+ 43s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 7,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            #endregion

            #region MTT 6 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "15.4%, 33+ A8s+ A5s ATo+ K9s+ KQo Q9s+ J9s+ T9s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 6,
                Position = PositionId.EP,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "19.4%, 22+ A3s+ ATo+ K8s+ KJo+ Q9s+ QJo J9s+ T8s+ 98s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 6,
                Position = PositionId.MP,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "25.6%, 22+ A2s+ A7o+ A5o K8s+ KTo+ Q8s+ QJo J8s+ JTo T8s+ 98s 87s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 6,
                Position = PositionId.CO,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "34.1%, 22+ Ax+ K5s+ KTo+ Q8s+ QTo+ J8s+ JTo T7s+ T9o 97s+ 86s+ 76s 65s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 6,
                Position = PositionId.BU,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "57.2%, 22+ Kx+ Q2s+ Q8o+ J3s+ J8o+ T4s+ T8o+ 95s+ 97o+ 84s+ 87o 74s+ 76o 64s+ 53s+ ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 6,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            #endregion

            #region MTT 5 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "19.1%, 22+ A3s+ ATo+ K9s+ KJo+ Q9s+ QJo J9s+ T8s+ 98s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 5,
                Position = PositionId.MP,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "24.0%, 22+ A2s+ A8o+ K7s+ KJo+ Q8s+ QJo J8s+ JTo T8s+ 98s 87s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 5,
                Position = PositionId.CO,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "32.7%, 22+ Ax+ K6s+ KTo+ Q8s+ QTo+ J8s+ JTo T7s+ 97s+ 86s+ 76s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 5,
                Position = PositionId.BU,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "55.4%, 22+ Kx+ Q2s+ Q8o+ J4s+ J8o+ T5s+ T8o+ 95s+ 98o 85s+ 87o 74s+ 76o 64s+ 53s+ ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 5,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            #endregion

            #region MTT 4 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "23.7%, 22+ A2s+ A8o+ K7s+ KJo+ Q8s+ QJo J8s+ JTo T8s+ 98s 87s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 4,
                Position = PositionId.CO,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "32.8%, 22+ Ax+ K5s+ KTo+ Q8s+ QTo+ J8s+ JTo T7s+ 97s+ 86s+ 76s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 4,
                Position = PositionId.BU,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "54.2%, 22+ Ax+ K2s+ K3o+ Q2s+ Q8o+ J4s+ J8o+ T5s+ T8o+ 95s+ 98o 85s+ 87o 74s+ 76o 64s+ 53s+ ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 4,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            #endregion

            #region MTT 3 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "32.5%, 22+ Ax+ K6s+ KTo+ Q8s+ QTo+ J8s+ JTo T7s+ 97s+ 87s 76s ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 3,
                Position = PositionId.BU,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "52.4%, 22+ Ax+ K2s+ K4o+ Q2s+ Q8o+ J4s+ J8o+ T6s+ T8o+ 95s+ 98o 85s+ 87o 74s+ 64s+ 53s+ ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 3,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            #endregion

            #region MTT 2 Players

            chartsPreflopDefault.Add(new ChartPreflop
            {
                RangeExpression = "50.1%, 22+ Ax+ K2s+ K5o+ Q3s+ Q9o+ J4s+ J8o+ T6s+ T8o+ 95s+ 98o 85s+ 87o 74s+ 64s+ 53s+ ",
                RegexTitle = REGEX_TITLE_MTT,
                Players = 2,
                Position = PositionId.SB,
                BigBlindsGreaterThanOrEqual = 12,
                BigBlindsLessThan = 18,
            });

            #endregion

            #endregion

            #endregion

            return chartsPreflopDefault;
        }
    }
}

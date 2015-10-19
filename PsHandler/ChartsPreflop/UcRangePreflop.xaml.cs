using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PsHandler.ChartsPreflop
{
    /// <summary>
    /// Interaction logic for UcRangePreflop.xaml
    /// </summary>
    public partial class UcRangePreflop : UserControl
    {
        public WindowRangePreflop WindowRangePreflop;
        public UcPocketPreflop[] UcPocketPreFlops = new UcPocketPreflop[169];
        public UcPocketPreflop UcPocketPreFlopPressed;
        public bool IsReadOnly;

        public UcRangePreflop(WindowRangePreflop windowRangePreflop, bool isReadOnly)
        {
            WindowRangePreflop = windowRangePreflop;
            IsReadOnly = isReadOnly;
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            InitPocket("AA", 0, 0);
            InitPocket("AKs", 0, 1);
            InitPocket("AQs", 0, 2);
            InitPocket("AJs", 0, 3);
            InitPocket("ATs", 0, 4);
            InitPocket("A9s", 0, 5);
            InitPocket("A8s", 0, 6);
            InitPocket("A7s", 0, 7);
            InitPocket("A6s", 0, 8);
            InitPocket("A5s", 0, 9);
            InitPocket("A4s", 0, 10);
            InitPocket("A3s", 0, 11);
            InitPocket("A2s", 0, 12);

            InitPocket("AKo", 1, 0);
            InitPocket("KK", 1, 1);
            InitPocket("KQs", 1, 2);
            InitPocket("KJs", 1, 3);
            InitPocket("KTs", 1, 4);
            InitPocket("K9s", 1, 5);
            InitPocket("K8s", 1, 6);
            InitPocket("K7s", 1, 7);
            InitPocket("K6s", 1, 8);
            InitPocket("K5s", 1, 9);
            InitPocket("K4s", 1, 10);
            InitPocket("K3s", 1, 11);
            InitPocket("K2s", 1, 12);

            InitPocket("AQo", 2, 0);
            InitPocket("KQo", 2, 1);
            InitPocket("QQ", 2, 2);
            InitPocket("QJs", 2, 3);
            InitPocket("QTs", 2, 4);
            InitPocket("Q9s", 2, 5);
            InitPocket("Q8s", 2, 6);
            InitPocket("Q7s", 2, 7);
            InitPocket("Q6s", 2, 8);
            InitPocket("Q5s", 2, 9);
            InitPocket("Q4s", 2, 10);
            InitPocket("Q3s", 2, 11);
            InitPocket("Q2s", 2, 12);

            InitPocket("AJo", 3, 0);
            InitPocket("KJo", 3, 1);
            InitPocket("QJo", 3, 2);
            InitPocket("JJ", 3, 3);
            InitPocket("JTs", 3, 4);
            InitPocket("J9s", 3, 5);
            InitPocket("J8s", 3, 6);
            InitPocket("J7s", 3, 7);
            InitPocket("J6s", 3, 8);
            InitPocket("J5s", 3, 9);
            InitPocket("J4s", 3, 10);
            InitPocket("J3s", 3, 11);
            InitPocket("J2s", 3, 12);

            InitPocket("ATo", 4, 0);
            InitPocket("KTo", 4, 1);
            InitPocket("QTo", 4, 2);
            InitPocket("JTo", 4, 3);
            InitPocket("TT", 4, 4);
            InitPocket("T9s", 4, 5);
            InitPocket("T8s", 4, 6);
            InitPocket("T7s", 4, 7);
            InitPocket("T6s", 4, 8);
            InitPocket("T5s", 4, 9);
            InitPocket("T4s", 4, 10);
            InitPocket("T3s", 4, 11);
            InitPocket("T2s", 4, 12);

            InitPocket("A9o", 5, 0);
            InitPocket("K9o", 5, 1);
            InitPocket("Q9o", 5, 2);
            InitPocket("J9o", 5, 3);
            InitPocket("T9o", 5, 4);
            InitPocket("99", 5, 5);
            InitPocket("98s", 5, 6);
            InitPocket("97s", 5, 7);
            InitPocket("96s", 5, 8);
            InitPocket("95s", 5, 9);
            InitPocket("94s", 5, 10);
            InitPocket("93s", 5, 11);
            InitPocket("92s", 5, 12);

            InitPocket("A8o", 6, 0);
            InitPocket("K8o", 6, 1);
            InitPocket("Q8o", 6, 2);
            InitPocket("J8o", 6, 3);
            InitPocket("T8o", 6, 4);
            InitPocket("98o", 6, 5);
            InitPocket("88", 6, 6);
            InitPocket("87s", 6, 7);
            InitPocket("86s", 6, 8);
            InitPocket("85s", 6, 9);
            InitPocket("84s", 6, 10);
            InitPocket("83s", 6, 11);
            InitPocket("82s", 6, 12);

            InitPocket("A7o", 7, 0);
            InitPocket("K7o", 7, 1);
            InitPocket("Q7o", 7, 2);
            InitPocket("J7o", 7, 3);
            InitPocket("T7o", 7, 4);
            InitPocket("97o", 7, 5);
            InitPocket("87o", 7, 6);
            InitPocket("77", 7, 7);
            InitPocket("76s", 7, 8);
            InitPocket("75s", 7, 9);
            InitPocket("74s", 7, 10);
            InitPocket("73s", 7, 11);
            InitPocket("72s", 7, 12);

            InitPocket("A6o", 8, 0);
            InitPocket("K6o", 8, 1);
            InitPocket("Q6o", 8, 2);
            InitPocket("J6o", 8, 3);
            InitPocket("T6o", 8, 4);
            InitPocket("96o", 8, 5);
            InitPocket("86o", 8, 6);
            InitPocket("76o", 8, 7);
            InitPocket("66", 8, 8);
            InitPocket("65s", 8, 9);
            InitPocket("64s", 8, 10);
            InitPocket("63s", 8, 11);
            InitPocket("62s", 8, 12);

            InitPocket("A5o", 9, 0);
            InitPocket("K5o", 9, 1);
            InitPocket("Q5o", 9, 2);
            InitPocket("J5o", 9, 3);
            InitPocket("T5o", 9, 4);
            InitPocket("95o", 9, 5);
            InitPocket("85o", 9, 6);
            InitPocket("75o", 9, 7);
            InitPocket("65o", 9, 8);
            InitPocket("55", 9, 9);
            InitPocket("54s", 9, 10);
            InitPocket("53s", 9, 11);
            InitPocket("52s", 9, 12);

            InitPocket("A4o", 10, 0);
            InitPocket("K4o", 10, 1);
            InitPocket("Q4o", 10, 2);
            InitPocket("J4o", 10, 3);
            InitPocket("T4o", 10, 4);
            InitPocket("94o", 10, 5);
            InitPocket("84o", 10, 6);
            InitPocket("74o", 10, 7);
            InitPocket("64o", 10, 8);
            InitPocket("54o", 10, 9);
            InitPocket("44", 10, 10);
            InitPocket("43s", 10, 11);
            InitPocket("42s", 10, 12);

            InitPocket("A3o", 11, 0);
            InitPocket("K3o", 11, 1);
            InitPocket("Q3o", 11, 2);
            InitPocket("J3o", 11, 3);
            InitPocket("T3o", 11, 4);
            InitPocket("93o", 11, 5);
            InitPocket("83o", 11, 6);
            InitPocket("73o", 11, 7);
            InitPocket("63o", 11, 8);
            InitPocket("53o", 11, 9);
            InitPocket("43o", 11, 10);
            InitPocket("33", 11, 11);
            InitPocket("32s", 11, 12);

            InitPocket("A2o", 12, 0);
            InitPocket("K2o", 12, 1);
            InitPocket("Q2o", 12, 2);
            InitPocket("J2o", 12, 3);
            InitPocket("T2o", 12, 4);
            InitPocket("92o", 12, 5);
            InitPocket("82o", 12, 6);
            InitPocket("72o", 12, 7);
            InitPocket("62o", 12, 8);
            InitPocket("52o", 12, 9);
            InitPocket("42o", 12, 10);
            InitPocket("32o", 12, 11);
            InitPocket("22", 12, 12);
        }

        private void InitPocket(string pocket, int row, int column)
        {
            var ucPocketPreflop = new UcPocketPreflop(this, pocket);
            ucPocketPreflop.Margin = new Thickness(1);
            Grid_Pockets.Children.Add(ucPocketPreflop);
            Grid.SetRow(ucPocketPreflop, row);
            Grid.SetColumn(ucPocketPreflop, column);
            UcPocketPreFlops[RangeConstructor.Parse(pocket)] = ucPocketPreflop;
        }

        public void MouseDownCtrl(string pocketStr, bool isSelected)
        {
            if (isSelected)
            {
                var rangeToDeselect = new List<int>();
                for (int v = RangeConstructor.IndexOfValue(pocketStr[1]); v < RangeConstructor.VALUES.Length; v++)
                {
                    string s;
                    if (pocketStr.Length == 2)
                    {
                        // pocket
                        s = string.Format("{0}{0}", RangeConstructor.VALUES[v]);
                    }
                    else
                    {
                        // suited, offsuited
                        s = string.Format("{0}{1}{2}", pocketStr[0], RangeConstructor.VALUES[v], pocketStr[2].ToString(CultureInfo.InvariantCulture));
                    }

                    rangeToDeselect.Add(RangeConstructor.Parse(s));
                }
                foreach (var pocketInt in rangeToDeselect)
                {
                    UcPocketPreFlops[pocketInt].IsSelected = false;
                }
            }
            else
            {
                foreach (var pocketInt in RangeConstructor.GetInt(pocketStr + "+"))
                {
                    UcPocketPreFlops[pocketInt].IsSelected = true;
                }
            }
        }
    }
}

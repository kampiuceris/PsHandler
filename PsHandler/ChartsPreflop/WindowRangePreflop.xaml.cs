using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using PsHandler.Custom;

namespace PsHandler.ChartsPreflop
{
    /// <summary>
    /// Interaction logic for WindowRangePreflop.xaml
    /// </summary>
    public partial class WindowRangePreflop : Window
    {
        private readonly UcRangePreflop _ucRangePreflop;
        public int[] Range;
        public bool IsReadOnly;

        public WindowRangePreflop(Window owner = null, int[] range = null, bool isReadOnly = true)
        {
            InitializeComponent();
            if (owner != null)
            {
                Owner = owner;
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            Range = range ?? new int[0];
            IsReadOnly = isReadOnly;
            _ucRangePreflop = new UcRangePreflop(this, IsReadOnly)
            {
                Margin = new Thickness(5),
                Focusable = true,
            };
            Grid_UcRangePrefFlop.Children.Add(_ucRangePreflop);
            SetRange(Range);
            if (IsReadOnly)
            {
                TextBox_RangeExpression.IsReadOnly = true;
            }
            TextBox_RangeExpression.TextChanged += (sender, args) =>
            {
                if (TextBox_RangeExpression.IsFocused)
                {
                    try
                    {
                        SetRange(RangeConstructor.GetInt(TextBox_RangeExpression.Text));
                    }
                    catch
                    {
                    }
                }
            };
            MouseDown += (sender, args) =>
            {
                _ucRangePreflop.Focus();
            };
        }

        public void SetRange(IEnumerable<int> range)
        {
            var notRange = RangeConstructor.RANKING.ToList();
            foreach (var pocket in range)
            {
                notRange.RemoveAll(a => a == pocket);
            }
            foreach (var pocket in range)
            {
                _ucRangePreflop.UcPocketPreFlops[pocket].IsSelected = true;
            }
            foreach (var pocket in notRange)
            {
                _ucRangePreflop.UcPocketPreFlops[pocket].IsSelected = false;
            }
        }

        public void UpdateInfo()
        {
            var ucPocketPreFlops = _ucRangePreflop.UcPocketPreFlops.Where(a => a.IsSelected).ToArray();
            Range = new int[ucPocketPreFlops.Length];
            for (int i = 0; i < ucPocketPreFlops.Length; i++)
            {
                Range[i] = ucPocketPreFlops[i].PocketInt;
            }

            double rangeFrequency;
            int rangeHands;
            RangeConstructor.GetRangeFrequency(Range, out rangeFrequency, out rangeHands);
            TextBlock_RangeInfo.Text = string.Format("Selected range contains {0}/1326 hands ({1:0.00%}).", rangeHands, rangeFrequency);

            if (!TextBox_RangeExpression.IsFocused)
            {
                Methods.UiInvoke(() =>
                {
                    var text = RangeConstructor.GetExpression(Range);
                    TextBox_RangeExpression.Text = text;
                }, true);
            }
        }
    }
}

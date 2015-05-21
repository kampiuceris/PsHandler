using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PsHandler.Custom;
using PsHandler.PokerMath;
using PsHandler.UI;

namespace PsHandler.Replayer.UI
{
    /// <summary>
    /// Interaction logic for WindowCalculateEv.xaml
    /// </summary>
    public partial class WindowCalculateEv : Window
    {
        private readonly string _handHistory;

        public WindowCalculateEv(Window owner, string handHistory)
        {
            InitializeComponent();

            if (owner != null)
            {
                Owner = owner;
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            _handHistory = handHistory;
            var pokerHand = PokerHand.Parse(_handHistory);
            TextBox_PrizePool.Text = string.Format("{0}", pokerHand.BuyIn * (int)pokerHand.TableSize);

            ComboBox_Payouts.Items.Add(new ComboBoxItemPayouts("Heads up", new[] { 1m }));
            ComboBox_Payouts.Items.Add(new ComboBoxItemPayouts("6 man", new[] { 0.65m, 0.35m }));
            ComboBox_Payouts.Items.Add(new ComboBoxItemPayouts("9 man", new[] { 0.5m, 0.3m, 0.2m }));
            ComboBox_Payouts.Items.Add(new ComboBoxItemPayouts("Fifty50 10 man", new[] { 0.6m, 0.1m, 0.1m, 0.1m, 0.1m }));
            ComboBox_Payouts.Items.Add(new ComboBoxItemPayouts("Fifty50 6 man", new[] { 0.8m, 0.1m, 0.1m }));
            ComboBox_Payouts.Items.Add(new ComboBoxItemPayouts("18 man FT", new[] { 0.4m, 0.3m, 0.2m, 0.1m }));
            ComboBox_Payouts.Items.Add(new ComboBoxItemPayouts("45 man FT", new[] { 0.31m, 0.215m, 0.165m, 0.125m, 0.09m, 0.06m, 0.035m }));
            ComboBox_Payouts.Items.Add(new ComboBoxItemPayouts("90 man FT", new[] { 0.2755m, 0.185m, 0.14m, 0.092m, 0.067m, 0.0495m, 0.04m, 0.0335m, 0.0275m }));
            ComboBox_Payouts.Items.Add(new ComboBoxItemPayouts("180 man FT", new[] { 0.3m, 0.2m, 0.114m, 0.074m, 0.058m, 0.043m, 0.03m, 0.022m, 0.015m }));
            ComboBox_Payouts.Items.Add(new ComboBoxItemPayouts("360 man FT", new[] { 0.2045m, 0.15m, 0.113m, 0.085m, 0.058m, 0.045m, 0.035m, 0.025m, 0.019m }));

            TextBox_Payouts.TextChanged += (sender, args) =>
            {
                var payouts = GetPayouts();
                if (payouts == null)
                {
                    TextBox_Payouts.Foreground = Brushes.Red;
                    TextBox_Payouts.ToolTip = "Error";
                }
                else
                {
                    TextBox_Payouts.Foreground = Brushes.Black;
                    TextBox_Payouts.ToolTip = payouts.Aggregate("", (a, b) => a + string.Format("{0:0.##########}{1}", b * 100, Environment.NewLine)).TrimEnd('\r', '\n');
                }
                ComboBox_Payouts.SelectedItem = null;
            };
            ComboBox_Payouts.SelectionChanged += (sender, args) =>
            {
                var selectedItem = ComboBox_Payouts.SelectedItem as ComboBoxItemPayouts;
                if (selectedItem != null)
                {
                    TextBox_Payouts.Text = selectedItem.Payouts.Aggregate("", (a, b) => a + string.Format("{0:0.##########}{1}", b, Environment.NewLine)).TrimEnd(' ', '\r', '\n');
                    TextBox_Payouts.ToolTip = selectedItem.ToolTip;
                }
            };

            CheckBox_StandartizePayouts.ToolTip = "If checked, payouts will be converted so their sum would be add up to 1.\n" +
                                                  "\n" +
                                                  "For example:\npayouts = { 300, 200 }\n" +
                                                  "will be converted to:\n" +
                                                  "payouts = { 0.6, 0.4 }\n" +
                                                  "\n" +
                                                  "Otherwise payouts will be converted so they stay in relationship to the prize pool.\n" +
                                                  "\n" +
                                                  "For example: payouts = { 300, 200 } with prize pool = 1000\n" +
                                                  "will be converted to:\n" +
                                                  "payouts = { 0.3, 0.2 }";
            ToolTipService.SetShowDuration(CheckBox_StandartizePayouts, 60000);
        }

        private decimal[] GetPayouts()
        {
            try
            {
                var payouts = new List<decimal>();
                var split = TextBox_Payouts.Text.Split(new[] { " ", ",", "\r", "\n", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                payouts.AddRange(split.Select(s => decimal.Parse(s)));
                if (CheckBox_StandartizePayouts.IsChecked == true)
                {
                    // normalize payotus to sum up to 1.000
                    var sum = payouts.Sum();
                    payouts = payouts.Select(payout => payout / sum).ToList();
                }
                else
                {
                    // normalize by prize pool
                    var prizePool = GetPrizePool();
                    if (prizePool == null) throw new NotSupportedException("Invalid prize pool.");
                    payouts = payouts.Select(payout => payout / (decimal)prizePool).ToList();
                }
                return payouts.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private decimal? GetPrizePool()
        {
            try
            {
                return decimal.Parse(TextBox_PrizePool.Text);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void Button_CalculateEv_Click(object sender, RoutedEventArgs e)
        {
            var prizePool = GetPrizePool();
            if (prizePool == null)
            {
                WindowMessage.ShowDialog("Invalid input: prize pool", "Error", WindowMessageButtons.OK, WindowMessageImage.Error, this, WindowStartupLocation.CenterOwner, WindowMessageTextType.TextBlock);
                return;
            }

            var payouts = GetPayouts();
            if (payouts == null || payouts.Length == 0)
            {
                WindowMessage.ShowDialog("Invalid input: payouts", "Error", WindowMessageButtons.OK, WindowMessageImage.Error, this, WindowStartupLocation.CenterOwner, WindowMessageTextType.TextBlock);
                return;
            }

            new Thread(() =>
            {
                var pokerHand = PokerHand.Parse(_handHistory);
                var ev = new Ev(pokerHand, payouts, (decimal)prizePool, pokerHand.Currency, PokerMath.Evaluator.Hand.Evaluate);
                Methods.UiInvoke(() => WindowMessage.Show(ev.ToString(), "Expected Value Analysis", WindowMessageButtons.OK, WindowMessageImage.None, this, WindowStartupLocation.CenterOwner, WindowMessageTextType.TextBox, new FontFamily("Consolas"), 10));
            }).Start();
        }

        internal class ComboBoxItemPayouts : ComboBoxItem
        {
            public string PayoutsName;
            public decimal[] Payouts;

            public ComboBoxItemPayouts(string payoutsName, decimal[] payouts)
            {
                PayoutsName = payoutsName;
                Payouts = payouts;
                Content = PayoutsName;
                ToolTip = Payouts.Aggregate("", (a, b) => a + string.Format("{0:0.##########}{1}", b * 100, Environment.NewLine)).TrimEnd('\r', '\n');
            }

            public override string ToString()
            {
                return PayoutsName;
            }
        }
    }
}

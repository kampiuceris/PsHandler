using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PsHandler.PokerTypes
{
    /// <summary>
    /// Interaction logic for WindowPokerTypeEdit.xaml
    /// </summary>
    public partial class WindowPokerTypeEdit : Window
    {
        public PokerType PokerType;
        public bool Saved = false;

        public WindowPokerTypeEdit(Window owner, PokerType pokerType = null)
        {
            InitializeComponent();
            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            PokerType = pokerType ?? new PokerType();

            TextBox_Name.Text = PokerType.Name;
            TextBox_LevelLengthInSeconds.Text = PokerType.LevelLengthInSeconds.ToString(CultureInfo.InvariantCulture);
            TextBox_IncludeAnd.Text = !PokerType.IncludeAnd.Any() ? "" : PokerType.IncludeAnd.Aggregate((s0, s1) => s0 + Environment.NewLine + s1);
            TextBox_IncludeOr.Text = !PokerType.IncludeOr.Any() ? "" : PokerType.IncludeOr.Aggregate((s0, s1) => s0 + Environment.NewLine + s1);
            TextBox_ExcludeAnd.Text = !PokerType.ExcludeAnd.Any() ? "" : PokerType.ExcludeAnd.Aggregate((s0, s1) => s0 + Environment.NewLine + s1);
            TextBox_ExcludeOr.Text = !PokerType.ExcludeOr.Any() ? "" : PokerType.ExcludeOr.Aggregate((s0, s1) => s0 + Environment.NewLine + s1);
            TextBox_BuyInAndRake.Text = !PokerType.BuyInAndRake.Any() ? "" : PokerType.BuyInAndRake.Aggregate((s0, s1) => s0 + Environment.NewLine + s1);
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Saved = false;
            Close();
        }

        private void Button_SaveAndClose_Click(object sender, RoutedEventArgs e)
        {
            if (TextBox_Name.Text.Length == 0)
            {
                MessageBox.Show(string.Format("Invalid '{0}' input.", Label_Name.Content), "Error saving", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            PokerType.Name = TextBox_Name.Text;
            if (!int.TryParse(TextBox_LevelLengthInSeconds.Text, out PokerType.LevelLengthInSeconds))
            {
                MessageBox.Show(string.Format("Invalid '{0}' input.", Label_LevelLengthInSeconds.Content), "Error saving", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            PokerType.IncludeAnd = TextBox_IncludeAnd.Text.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            PokerType.IncludeOr = TextBox_IncludeOr.Text.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            PokerType.ExcludeAnd = TextBox_ExcludeAnd.Text.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            PokerType.ExcludeOr = TextBox_ExcludeOr.Text.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            PokerType.BuyInAndRake = TextBox_BuyInAndRake.Text.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            Saved = true;
            Close();
        }

        private void TextBox_LevelLengthInSeconds_TextChanged(object sender, TextChangedEventArgs e)
        {
            int seconds;
            if (int.TryParse(TextBox_LevelLengthInSeconds.Text, out seconds))
            {
                Label_LevelLengthInMinutes.Content = string.Format("{0:0.#} minutes", (double)seconds / 60);
            }
            else
            {
                Label_LevelLengthInMinutes.Content = "LocationX minutes";
            }
        }
    }
}

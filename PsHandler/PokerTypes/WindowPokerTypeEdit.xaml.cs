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
using PsHandler.TableTiler;
using PsHandler.UI.ToolTips;

namespace PsHandler.PokerTypes
{
    /// <summary>
    /// Interaction logic for WindowPokerTypeEdit.xaml
    /// </summary>
    public partial class WindowPokerTypeEdit : Window
    {
        public PokerType PokerType;
        public bool Saved = false;
        private WindowWindowsInfo _windowWindowsInfo;

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

            TextBox_IncludeAnd.TextChanged += (sender, args) => CheckTextBoxFilter();
            TextBox_IncludeOr.TextChanged += (sender, args) => CheckTextBoxFilter();
            TextBox_ExcludeAnd.TextChanged += (sender, args) => CheckTextBoxFilter();
            TextBox_ExcludeOr.TextChanged += (sender, args) => CheckTextBoxFilter();
            TextBox_CheckFilter.TextChanged += (sender, args) => CheckTextBoxFilter();

            // ToolTips

            Image_TitleIncludeAllWords.ToolTip = new UCToolTipTitleIncludeAllWords();
            Image_TitleIncludeAnyWords.ToolTip = new UCToolTipTitleIncludeAnyWords();
            Image_TitleExcludeAllWords.ToolTip = new UCToolTipTitleExcludeAllWords();
            Image_TitleExcludeAnyWords.ToolTip = new UCToolTipTitleExcludeAnyWords();
            Image_FileNameBuyInRake.ToolTip = new UCToolTipFileNameBuyInRake();
            Image_CheckFilter.ToolTip = new UCToolTipTextBoxCheckFilter();
        }

        private void CheckTextBoxFilter()
        {
            string text = TextBox_CheckFilter.Text;

            if (string.IsNullOrEmpty(text))
            {
                TextBox_CheckFilter.Background = System.Windows.Media.Brushes.White;
            }
            else
            {
                var includeAnd = TextBox_IncludeAnd.Text.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                var includeOr = TextBox_IncludeOr.Text.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                var excludeAnd = TextBox_ExcludeAnd.Text.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                var excludeOr = TextBox_ExcludeOr.Text.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);



                var bIncludeAnd = includeAnd.Length == 0 || includeAnd.All(text.Contains);
                var bIncludeOr = includeOr.Length == 0 || includeOr.Any(text.Contains);
                var bExcludeAnd = excludeAnd.Length == 0 || !excludeAnd.All(text.Contains);
                var bExcludeOr = excludeOr.Length == 0 || !excludeOr.Any(text.Contains);
                if (bIncludeAnd && bIncludeOr && bExcludeAnd && bExcludeOr)
                {
                    TextBox_CheckFilter.Background = System.Windows.Media.Brushes.Honeydew;
                }
                else
                {
                    TextBox_CheckFilter.Background = System.Windows.Media.Brushes.MistyRose;
                }
            }
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

        private void Button_WindowsInfo_Click(object sender, RoutedEventArgs e)
        {
            if (_windowWindowsInfo != null) _windowWindowsInfo.Close();
            _windowWindowsInfo = new WindowWindowsInfo();
            _windowWindowsInfo.Show();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (_windowWindowsInfo != null)
            {
                _windowWindowsInfo.Close();
            }
            base.OnClosing(e);
        }
    }
}

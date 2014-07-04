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
using PsHandler.UI;
using PsHandler.UI.ToolTips;

namespace PsHandler.PokerTypes
{
    /// <summary>
    /// Interaction logic for WindowPokerTypeEdit.xaml
    /// </summary>
    public partial class WindowPokerTypeEdit : Window, IFilter
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

            // IFilter hook

            TextBox_IncludeAnd.TextChanged += (sender, args) => UpdateFilter();
            TextBox_IncludeOr.TextChanged += (sender, args) => UpdateFilter();
            TextBox_ExcludeAnd.TextChanged += (sender, args) => UpdateFilter();
            TextBox_ExcludeOr.TextChanged += (sender, args) => UpdateFilter();
            TextBox_WindowClass.TextChanged += (sender, args) => UpdateFilter();

            // seed values

            TextBox_Name.Text = PokerType.Name;
            TextBox_LevelLengthInSeconds.Text = PokerType.LevelLengthInSeconds.ToString(CultureInfo.InvariantCulture);
            TextBox_IncludeAnd.Text = !PokerType.IncludeAnd.Any() ? "" : PokerType.IncludeAnd.Aggregate((s0, s1) => s0 + Environment.NewLine + s1);
            TextBox_IncludeOr.Text = !PokerType.IncludeOr.Any() ? "" : PokerType.IncludeOr.Aggregate((s0, s1) => s0 + Environment.NewLine + s1);
            TextBox_ExcludeAnd.Text = !PokerType.ExcludeAnd.Any() ? "" : PokerType.ExcludeAnd.Aggregate((s0, s1) => s0 + Environment.NewLine + s1);
            TextBox_ExcludeOr.Text = !PokerType.ExcludeOr.Any() ? "" : PokerType.ExcludeOr.Aggregate((s0, s1) => s0 + Environment.NewLine + s1);
            TextBox_BuyInAndRake.Text = !PokerType.BuyInAndRake.Any() ? "" : PokerType.BuyInAndRake.Aggregate((s0, s1) => s0 + Environment.NewLine + s1);
            TextBox_WindowClass.Text = PokerType.WindowClass;

            // ToolTips

            Image_TitleIncludeAllWords.ToolTip = new UCToolTipTitleIncludeAllWords();
            Image_TitleIncludeAnyWords.ToolTip = new UCToolTipTitleIncludeAnyWords();
            Image_TitleExcludeAllWords.ToolTip = new UCToolTipTitleExcludeAllWords();
            Image_TitleExcludeAnyWords.ToolTip = new UCToolTipTitleExcludeAnyWords();
            Image_FileNameBuyInRake.ToolTip = new UCToolTipFileNameBuyInRake();
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

            if (PokerTypeManager.PokerTypes.ToArray().Any(o => o.Name.ToLowerInvariant().Equals(TextBox_Name.Text.ToLowerInvariant())))
            {
                MessageBox.Show("Cannot save. Duplicate names.", "Error saving", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(TextBox_LevelLengthInSeconds.Text, out PokerType.LevelLengthInSeconds))
            {
                MessageBox.Show(string.Format("Invalid '{0}' input.", Label_LevelLengthInSeconds.Content), "Error saving", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            PokerType.Name = TextBox_Name.Text;
            PokerType.IncludeAnd = TextBox_IncludeAnd.Text.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            PokerType.IncludeOr = TextBox_IncludeOr.Text.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            PokerType.ExcludeAnd = TextBox_ExcludeAnd.Text.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            PokerType.ExcludeOr = TextBox_ExcludeOr.Text.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            PokerType.BuyInAndRake = TextBox_BuyInAndRake.Text.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            PokerType.WindowClass = TextBox_WindowClass.Text;

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
            _windowWindowsInfo = new WindowWindowsInfo(this);
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

        // IFilter

        private readonly object _filterLock = new object();
        private readonly List<string> _filterIncludeAnd = new List<string>();
        private readonly List<string> _filterIncludeOr = new List<string>();
        private readonly List<string> _filterExcludeAnd = new List<string>();
        private readonly List<string> _filterExcludeOr = new List<string>();
        private string _filterWindowClass = "";
        public List<string> FilterIncludeAnd { get { lock (_filterLock) { return _filterIncludeAnd.ToList(); } } }
        public List<string> FilterIncludeOr { get { lock (_filterLock) { return _filterIncludeOr.ToList(); } } }
        public List<string> FilterExcludeAnd { get { lock (_filterLock) { return _filterExcludeAnd.ToList(); } } }
        public List<string> FilterExcludeOr { get { lock (_filterLock) { return _filterExcludeOr.ToList(); } } }
        public string WindowClass { get { lock (_filterLock) { return _filterWindowClass; } } }

        private void UpdateFilter()
        {
            lock (_filterLock)
            {
                _filterIncludeAnd.Clear();
                _filterIncludeOr.Clear();
                _filterExcludeAnd.Clear();
                _filterExcludeOr.Clear();
                _filterIncludeAnd.AddRange(TextBox_IncludeAnd.Text.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                _filterIncludeOr.AddRange(TextBox_IncludeOr.Text.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                _filterExcludeAnd.AddRange(TextBox_ExcludeAnd.Text.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                _filterExcludeOr.AddRange(TextBox_ExcludeOr.Text.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries));
                _filterWindowClass = TextBox_WindowClass.Text;
            }
        }
    }
}

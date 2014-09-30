using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PsHandler.UI;

namespace PsHandler.PokerTypes
{
    /// <summary>
    /// Interaction logic for WindowPokerTypeEdit.xaml
    /// </summary>
    public partial class WindowPokerTypeEdit : Window, IFilter
    {
        public PokerType PokerType;
        public bool Saved;
        private WindowWindowsInfo _windowWindowsInfo;

        public WindowPokerTypeEdit(Window owner, PokerType pokerType = null)
        {
            InitializeComponent();
            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            PokerType = pokerType ?? new PokerType();

            // Hook

            Closing += (sender, args) => { if (_windowWindowsInfo != null) { _windowWindowsInfo.Close(); } };

            TextBox_RegexWindowTitle.TextChanged += (sender, args) => UpdateIFilter();
            TextBox_RegexWindowClass.TextChanged += (sender, args) => UpdateIFilter();

            TextBox_LevelLength.TextChanged += (sender, args) =>
            {
                try
                {
                    TimeSpan.Parse(TextBox_LevelLength.Text);
                    TextBox_LevelLength.Background = Brushes.Honeydew;
                }
                catch
                {
                    TextBox_LevelLength.Background = Brushes.MistyRose;
                }
            };
            Loaded += (sender, args) =>
            {
                string text = TextBox_RegexWindowTitle.Text; TextBox_RegexWindowTitle.Text = text + "1"; TextBox_RegexWindowTitle.Text = text;
                text = TextBox_RegexWindowClass.Text; TextBox_RegexWindowClass.Text = text + "1"; TextBox_RegexWindowClass.Text = text;
            };

            // Seed

            TextBox_Name.Text = PokerType.Name;
            TextBox_LevelLength.Text = PokerType.LevelLength.ToString();
            TextBox_RegexWindowTitle.Text = PokerType.RegexWindowTitle == null ? "" : PokerType.RegexWindowTitle.ToString();
            TextBox_RegexWindowClass.Text = PokerType.RegexWindowClass == null ? "" : PokerType.RegexWindowClass.ToString();

            // ToolTips

            Label_RegexWindowTitle.ToolTip = "Regular Expression (Regex) for window's title. You can learn and test regex at: http://rubular.com/ or any other similar site.";
            ToolTipService.SetShowDuration(Label_RegexWindowTitle, 60000);

            Label_RegexWindowClass.ToolTip = "Regular Expression (Regex) for window's class name. You can learn and test regex at: http://rubular.com/ or any other similar site.";
            ToolTipService.SetShowDuration(Label_RegexWindowClass, 60000);
        }

        private void Button_WindowsInfo_Click(object sender, RoutedEventArgs e)
        {
            if (_windowWindowsInfo != null) _windowWindowsInfo.Close();
            _windowWindowsInfo = new WindowWindowsInfo(this);
            _windowWindowsInfo.Show();
        }

        private void Button_SaveAndClose_Click(object sender, RoutedEventArgs e)
        {
            if (TextBox_Name.Text.Length == 0)
            {
                WindowMessage.ShowDialog(string.Format("Invalid '{0}' input.", Label_Name.Content), "Error saving", WindowMessageButtons.OK, WindowMessageImage.Error, this);
                return;
            }
            if (PokerTypeManager.GetPokerTypesCopy().Any(o => o.Name.ToLowerInvariant().Equals(TextBox_Name.Text.ToLowerInvariant())))
            {
                WindowMessage.ShowDialog("Cannot save. Duplicate names.", "Error saving", WindowMessageButtons.OK, WindowMessageImage.Error, this);
                return;
            }
            try
            {
                PokerType.LevelLength = TimeSpan.Parse(TextBox_LevelLength.Text);
            }
            catch
            {
                WindowMessage.ShowDialog(string.Format("Invalid '{0}' input.", Label_LevelLength.Content), "Error saving", WindowMessageButtons.OK, WindowMessageImage.Error, this);
                return;
            }
            try
            {
                new Regex(TextBox_RegexWindowTitle.Text);
            }
            catch
            {
                WindowMessage.ShowDialog(string.Format("Invalid '{0}' input.", Label_RegexWindowTitle.Content), "Error saving", WindowMessageButtons.OK, WindowMessageImage.Error, this);
                return;
            }
            try
            {
                new Regex(TextBox_RegexWindowClass.Text);
            }
            catch
            {
                WindowMessage.ShowDialog(string.Format("Invalid '{0}' input.", Label_RegexWindowClass.Content), "Error saving", WindowMessageButtons.OK, WindowMessageImage.Error, this);
                return;
            }

            PokerType.Name = TextBox_Name.Text;
            //PokerType.LevelLength = TimeSpan.Parse(TextBox_LevelLength.Text);
            PokerType.RegexWindowTitle = new Regex(TextBox_RegexWindowTitle.Text);
            PokerType.RegexWindowClass = new Regex(TextBox_RegexWindowClass.Text);

            Saved = true;
            Close();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Saved = false;
            Close();
        }

        // IFilter

        private readonly object _iFilterLock = new object();
        public Regex _regexWindowTitle = new Regex("");
        public Regex _regexWindowClass = new Regex("");
        public Regex RegexWindowTitle
        {
            get
            {
                lock (_iFilterLock)
                {
                    return _regexWindowTitle;
                }
            }
        }
        public Regex RegexWindowClass
        {
            get
            {
                lock (_iFilterLock)
                {
                    return _regexWindowClass;
                }
            }
        }

        private void UpdateIFilter()
        {
            lock (_iFilterLock)
            {
                try
                {
                    _regexWindowTitle = new Regex(TextBox_RegexWindowTitle.Text);
                    TextBox_RegexWindowTitle.Background = Brushes.Honeydew;
                }
                catch
                {
                    _regexWindowTitle = new Regex("");
                    TextBox_RegexWindowTitle.Background = Brushes.MistyRose;
                }
                try
                {
                    _regexWindowClass = new Regex(TextBox_RegexWindowClass.Text);
                    TextBox_RegexWindowClass.Background = Brushes.Honeydew;
                }
                catch (Exception)
                {
                    _regexWindowClass = new Regex("");
                    TextBox_RegexWindowClass.Background = Brushes.MistyRose;
                }
            }
        }
    }
}

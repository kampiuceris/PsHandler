using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PsHandler
{
    /// <summary>
    /// Interaction logic for TextBoxHotkey.xaml
    /// </summary>
    public partial class TextBoxHotkey : UserControl
    {
        private readonly Brush _borderBrush;
        private readonly Thickness _borderThickness;
        private KeyCombination _keyCombination = new KeyCombination(Key.None, false, false, false);
        public KeyCombination KeyCombination
        {
            set
            {
                _keyCombination = value;
                TextBox_Hotkey.Text = GetString();
                TextBox_Hotkey.Foreground = TextBox_Hotkey.Text.Equals("None") ? Brushes.DarkGray : Brushes.Black;
            }
            get
            {
                return _keyCombination;
            }
        }

        public TextBoxHotkey()
        {
            InitializeComponent();
            _borderBrush = TextBox_Hotkey.BorderBrush;
            _borderThickness = TextBox_Hotkey.BorderThickness;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox_Hotkey.BorderBrush = new SolidColorBrush(Colors.Red);
            TextBox_Hotkey.BorderThickness = new Thickness(1.5);
            App.KeyboardHook.KeyCombinationDownMethods.Add(TextBoxKeyDown);
        }

        private void TextBox_Hotkey_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox_Hotkey.BorderBrush = _borderBrush;
            TextBox_Hotkey.BorderThickness = _borderThickness;
            App.KeyboardHook.KeyCombinationDownMethods.Remove(TextBoxKeyDown);
        }

        private void TextBoxKeyDown(KeyCombination keyCombination)
        {
            KeyCombination = keyCombination;
        }

        private string GetString()
        {
            List<string> modifiers = new List<string>();
            if (KeyCombination.Ctrl)
            {
                modifiers.Add("Ctrl");
                modifiers.Add(" + ");
            }
            if (KeyCombination.Alt)
            {
                modifiers.Add("Alt");
                modifiers.Add(" + ");
            }
            if (KeyCombination.Shift)
            {
                modifiers.Add("Shift");
                modifiers.Add(" + ");
            }

            StringBuilder sb = new StringBuilder();

            foreach (string s in modifiers)
                sb.Append(s);
            sb.Append(KeyCombination.Key);

            return sb.ToString();
        }

        private void TextBox_Hotkey_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            KeyCombination = new KeyCombination(Key.None, false, false, false);
        }
    }
}

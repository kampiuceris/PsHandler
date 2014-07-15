using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace PsHandler.UI
{
    public class TextBoxHotkey : TextBox
    {
        private KeyCombination _keyCombination;

        public KeyCombination KeyCombination
        {
            set
            {
                if (value == null) return;
                _keyCombination = value;
                Text = GetString(KeyCombination);
                Foreground = Text.Equals("None") ? Brushes.DarkGray : Brushes.Black;
            }
            get
            {
                return _keyCombination;
            }
        }

        public TextBoxHotkey()
        {
            KeyCombination = new KeyCombination(Key.None, false, false, false);

            BorderThickness = new Thickness(1.2);
            BorderBrush = Brushes.DarkGray;
            IsReadOnly = true;
            Background = Brushes.AliceBlue;
            SelectionBrush = Brushes.Transparent;
            VerticalContentAlignment = VerticalAlignment.Center;
            ToolTip = "Double click mouse to remove hotkey";

            GotFocus += (sender, args) =>
            {
                BorderBrush = Brushes.Red;
                App.KeyboardHook.KeyCombinationDownMethods.Add(TextBoxKeyDown);
            };

            LostFocus += (sender, args) =>
            {
                BorderBrush = Brushes.DarkGray;
                App.KeyboardHook.KeyCombinationDownMethods.Remove(TextBoxKeyDown);
            };

            MouseDoubleClick += (sender, args) =>
            {
                KeyCombination = new KeyCombination(Key.None, false, false, false);
            };

            TextWrapping = TextWrapping.NoWrap;
        }

        private void TextBoxKeyDown(KeyCombination keyCombination)
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null && parentWindow.WindowState != WindowState.Minimized)
            {
                if (new WindowInteropHelper(parentWindow).Handle == WinApi.GetActiveWindow())
                {
                    KeyCombination = keyCombination;
                }
            }
        }

        public static string GetString(KeyCombination keyCombination)
        {
            List<string> modifiers = new List<string>();
            if (keyCombination.Ctrl)
            {
                modifiers.Add("Ctrl");
                modifiers.Add(" + ");
            }
            if (keyCombination.Alt)
            {
                modifiers.Add("Alt");
                modifiers.Add(" + ");
            }
            if (keyCombination.Shift)
            {
                modifiers.Add("Shift");
                modifiers.Add(" + ");
            }

            StringBuilder sb = new StringBuilder();

            foreach (string s in modifiers)
                sb.Append(s);
            sb.Append(keyCombination.Key);

            return sb.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using PsHandler.Hook;

namespace PsHandler.UI
{
    public class TextBoxHotkey : TextBox
    {
        private KeyCombination _keyCombination;
        public bool RestrictedToSingeKeys;

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

        public TextBoxHotkey(bool restrictedToSingeKeys)
        {
            RestrictedToSingeKeys = restrictedToSingeKeys;

            KeyCombination = new KeyCombination(Key.None, false, false, false);

            BorderThickness = new Thickness(1.2);
            BorderBrush = Brushes.DarkGray;
            IsReadOnly = true;
            Background = Brushes.AliceBlue;
            SelectionBrush = Brushes.Transparent;
            VerticalContentAlignment = VerticalAlignment.Center;
            ToolTip = "Double click mouse to remove hotkey";
            Padding = new Thickness(0);

            GotFocus += (sender, args) =>
            {
                BorderBrush = Brushes.Red;
                if (App.KeyboardHook != null)
                {
                    App.KeyboardHook.KeyDownMethods.Add(TextBoxKeyDown);
                }
            };

            LostFocus += (sender, args) =>
            {
                BorderBrush = Brushes.DarkGray;
                if (App.KeyboardHook != null)
                {
                    App.KeyboardHook.KeyDownMethods.Remove(TextBoxKeyDown);
                }
            };

            MouseDoubleClick += (sender, args) =>
            {
                KeyCombination = new KeyCombination(Key.None, false, false, false);
            };

            TextWrapping = TextWrapping.NoWrap;
        }

        public TextBoxHotkey()
            : this(false)
        {
        }

        private void TextBoxKeyDown(KeyCombination keyCombination)
        {
            Window parentWindow = Window.GetWindow(this);
            if (parentWindow != null && parentWindow.WindowState != WindowState.Minimized)
            {
                if (new WindowInteropHelper(parentWindow).Handle == WinApi.GetActiveWindow())
                {
                    if (RestrictedToSingeKeys)
                    {
                        keyCombination.Alt = false;
                        keyCombination.Shift = false;
                        keyCombination.Ctrl = false;
                    }
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

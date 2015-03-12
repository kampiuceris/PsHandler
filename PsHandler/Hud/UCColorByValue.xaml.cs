// PsHandler - poker software helping tool.
// Copyright (C) 2014-2015  kampiuceris

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
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

namespace PsHandler.Hud
{
    /// <summary>
    /// Interaction logic for UCColorByValue.xaml
    /// </summary>
    public partial class UCColorByValue : UserControl
    {
        private readonly StackPanel _owner;

        public UCColorByValue(StackPanel owner, ColorByValue colorByValue = null)
        {
            _owner = owner;
            InitializeComponent();

            foreach (var item in typeof(System.Drawing.Color).GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                ComboBox_Foreground.Items.Add(new ComboBoxItemColor(item.Name));
            }

            TextBox_GreaterOrEqual.GotFocus += (sender, args) =>
            {
                if (TextBox_GreaterOrEqual.Text.Equals("-inf"))
                {
                    TextBox_GreaterOrEqual.Text = "";
                }
            };
            TextBox_Less.GotFocus += (sender, args) =>
            {
                if (TextBox_Less.Text.Equals("+inf"))
                {
                    TextBox_Less.Text = "";
                }
            };
            TextBox_GreaterOrEqual.LostFocus += (sender, args) =>
            {
                if (TextBox_GreaterOrEqual.Text.Equals("") || TextBox_GreaterOrEqual.Text.Equals("-inf"))
                {
                    TextBox_GreaterOrEqual.Text = "-inf";
                }
            };
            TextBox_Less.LostFocus += (sender, args) =>
            {
                if (TextBox_Less.Text.Equals("") || TextBox_Less.Text.Equals("+inf"))
                {
                    TextBox_Less.Text = "+inf";
                }
            };

            TextBox_GreaterOrEqual.TextChanged += (sender, args) => Validate();
            TextBox_Less.TextChanged += (sender, args) => Validate();

            if (colorByValue != null)
            {
                TextBox_GreaterOrEqual.Text = colorByValue.GreaterOrEqual == decimal.MinValue ? "-inf" : colorByValue.GreaterOrEqual.ToString(CultureInfo.InvariantCulture);
                TextBox_Less.Text = colorByValue.Less == decimal.MaxValue ? "+inf" : colorByValue.Less.ToString(CultureInfo.InvariantCulture);
                foreach (ComboBoxItemColor item in ComboBox_Foreground.Items.Cast<object>().OfType<ComboBoxItemColor>().Where(item => item.ColorMedia.Equals(colorByValue.Color)))
                {
                    ComboBox_Foreground.SelectedItem = item;
                    break;
                }
            }

            Validate();
        }

        private void Button_Remove_Click(object sender, RoutedEventArgs e)
        {
            _owner.Children.Remove(this);
        }

        private bool Validate()
        {
            decimal greaterOrEqual, less;
            return GetValues(out greaterOrEqual, out less);
        }

        private bool GetValues(out decimal greaterOrEqual, out decimal less)
        {
            bool successGreaterOrEqual = decimal.TryParse(TextBox_GreaterOrEqual.Text, out greaterOrEqual);
            if (!successGreaterOrEqual)
            {
                greaterOrEqual = decimal.MinValue;
                if (TextBox_GreaterOrEqual.Text.Equals("-inf"))
                {
                    successGreaterOrEqual = true;
                }
            }

            bool successLess = decimal.TryParse(TextBox_Less.Text, out less);
            if (!successLess)
            {
                less = decimal.MaxValue;
                if (TextBox_Less.Text.Equals("+inf"))
                {
                    successLess = true;
                }
            }

            TextBox_GreaterOrEqual.Background = successGreaterOrEqual ? Brushes.Honeydew : Brushes.MistyRose;
            TextBox_Less.Background = successLess ? Brushes.Honeydew : Brushes.MistyRose;

            return successGreaterOrEqual && successLess;
        }

        public ColorByValue GetColorByValue()
        {
            var ComboBox_BackgroundSelectedItem = (ComboBoxItemColor)ComboBox_Foreground.SelectedItem;
            if (ComboBox_BackgroundSelectedItem == null) return null;
            Color color = ComboBox_BackgroundSelectedItem.ColorMedia;
            decimal greaterOrEqual, less;
            return GetValues(out greaterOrEqual, out less) ? new ColorByValue { Color = color, GreaterOrEqual = greaterOrEqual, Less = less } : null;
        }
    }
}

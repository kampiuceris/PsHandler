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

using System.Windows;
using System.Windows.Media;

namespace PsHandler.ColorPicker
{
    /// <summary>
    /// Interaction logic for WindowColorPicker.xaml
    /// </summary>
    public partial class WindowColorPicker : Window
    {
        public bool Saved { get; set; }
        public Color Color { get; set; }

        public WindowColorPicker(Window owner, Color color)
        {
            InitializeComponent();

            if (owner != null)
            {
                Owner = owner;
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

            Color = color;
            UcColorPicker_Main.SetColor(Color);
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            Color = UcColorPicker_Main.ColorARGBHSV.ColorMedia;
            UcColorPicker.AddRecentColor(Color);
            Saved = true;
            Close();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Saved = false;
            Close();
        }
    }
}

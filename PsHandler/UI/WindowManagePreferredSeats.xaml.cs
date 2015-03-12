// PsHandler - poker software helping tool.
// Copyright (C) 2014  kampiuceris

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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PsHandler.UI
{
    /// <summary>
    /// Interaction logic for WindowManagePreferredSeats.xaml
    /// </summary>
    public partial class WindowManagePreferredSeats : Window
    {
        public static int[] PreferredSeat = new int[11]
        {
            0, // Default
            0, // 1-max
            0, // 2-max
            1, // 3-max
            1, // 4-max
            2, // 5-max
            2, // 6-max
            3, // 7-max
            3, // 8-max
            4, // 9-max
            4, // 10-max
        };

        public WindowManagePreferredSeats(Window owner)
        {
            InitializeComponent();
            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            for (int i = 0; i < 11; i++)
            {
                PreferredSeat[i] = Config.PreferredSeat[i];
            }

            // Init

            ComboBox_PreferredSeat2.Items.Add("1");
            ComboBox_PreferredSeat2.Items.Add("2");

            ComboBox_PreferredSeat3.Items.Add("1");
            ComboBox_PreferredSeat3.Items.Add("2");
            ComboBox_PreferredSeat3.Items.Add("3");

            ComboBox_PreferredSeat4.Items.Add("1");
            ComboBox_PreferredSeat4.Items.Add("2");
            ComboBox_PreferredSeat4.Items.Add("3");
            ComboBox_PreferredSeat4.Items.Add("4");

            ComboBox_PreferredSeat5.Items.Add("1");
            ComboBox_PreferredSeat5.Items.Add("2");
            ComboBox_PreferredSeat5.Items.Add("3");
            ComboBox_PreferredSeat5.Items.Add("4");
            ComboBox_PreferredSeat5.Items.Add("5");

            ComboBox_PreferredSeat6.Items.Add("1");
            ComboBox_PreferredSeat6.Items.Add("2");
            ComboBox_PreferredSeat6.Items.Add("3");
            ComboBox_PreferredSeat6.Items.Add("4");
            ComboBox_PreferredSeat6.Items.Add("5");
            ComboBox_PreferredSeat6.Items.Add("6");

            ComboBox_PreferredSeat7.Items.Add("1");
            ComboBox_PreferredSeat7.Items.Add("2");
            ComboBox_PreferredSeat7.Items.Add("3");
            ComboBox_PreferredSeat7.Items.Add("4");
            ComboBox_PreferredSeat7.Items.Add("5");
            ComboBox_PreferredSeat7.Items.Add("6");
            ComboBox_PreferredSeat7.Items.Add("7");

            ComboBox_PreferredSeat8.Items.Add("1");
            ComboBox_PreferredSeat8.Items.Add("2");
            ComboBox_PreferredSeat8.Items.Add("3");
            ComboBox_PreferredSeat8.Items.Add("4");
            ComboBox_PreferredSeat8.Items.Add("5");
            ComboBox_PreferredSeat8.Items.Add("6");
            ComboBox_PreferredSeat8.Items.Add("7");
            ComboBox_PreferredSeat8.Items.Add("8");

            ComboBox_PreferredSeat9.Items.Add("1");
            ComboBox_PreferredSeat9.Items.Add("2");
            ComboBox_PreferredSeat9.Items.Add("3");
            ComboBox_PreferredSeat9.Items.Add("4");
            ComboBox_PreferredSeat9.Items.Add("5");
            ComboBox_PreferredSeat9.Items.Add("6");
            ComboBox_PreferredSeat9.Items.Add("7");
            ComboBox_PreferredSeat9.Items.Add("8");
            ComboBox_PreferredSeat9.Items.Add("9");

            ComboBox_PreferredSeat10.Items.Add("1");
            ComboBox_PreferredSeat10.Items.Add("2");
            ComboBox_PreferredSeat10.Items.Add("3");
            ComboBox_PreferredSeat10.Items.Add("4");
            ComboBox_PreferredSeat10.Items.Add("5");
            ComboBox_PreferredSeat10.Items.Add("6");
            ComboBox_PreferredSeat10.Items.Add("7");
            ComboBox_PreferredSeat10.Items.Add("8");
            ComboBox_PreferredSeat10.Items.Add("9");
            ComboBox_PreferredSeat10.Items.Add("10");

            // Seed

            ComboBox_PreferredSeat2.SelectedIndex = PreferredSeat[2];
            ComboBox_PreferredSeat3.SelectedIndex = PreferredSeat[3];
            ComboBox_PreferredSeat4.SelectedIndex = PreferredSeat[4];
            ComboBox_PreferredSeat5.SelectedIndex = PreferredSeat[5];
            ComboBox_PreferredSeat6.SelectedIndex = PreferredSeat[6];
            ComboBox_PreferredSeat7.SelectedIndex = PreferredSeat[7];
            ComboBox_PreferredSeat8.SelectedIndex = PreferredSeat[8];
            ComboBox_PreferredSeat9.SelectedIndex = PreferredSeat[9];
            ComboBox_PreferredSeat10.SelectedIndex = PreferredSeat[10];

            // Hook

            ComboBox_PreferredSeat2.SelectionChanged += (sender, args) => PreferredSeat[2] = ComboBox_PreferredSeat2.SelectedIndex;
            ComboBox_PreferredSeat3.SelectionChanged += (sender, args) => PreferredSeat[3] = ComboBox_PreferredSeat3.SelectedIndex;
            ComboBox_PreferredSeat4.SelectionChanged += (sender, args) => PreferredSeat[4] = ComboBox_PreferredSeat4.SelectedIndex;
            ComboBox_PreferredSeat5.SelectionChanged += (sender, args) => PreferredSeat[5] = ComboBox_PreferredSeat5.SelectedIndex;
            ComboBox_PreferredSeat6.SelectionChanged += (sender, args) => PreferredSeat[6] = ComboBox_PreferredSeat6.SelectedIndex;
            ComboBox_PreferredSeat7.SelectionChanged += (sender, args) => PreferredSeat[7] = ComboBox_PreferredSeat7.SelectedIndex;
            ComboBox_PreferredSeat8.SelectionChanged += (sender, args) => PreferredSeat[8] = ComboBox_PreferredSeat8.SelectedIndex;
            ComboBox_PreferredSeat9.SelectionChanged += (sender, args) => PreferredSeat[9] = ComboBox_PreferredSeat9.SelectedIndex;
            ComboBox_PreferredSeat10.SelectionChanged += (sender, args) => PreferredSeat[10] = ComboBox_PreferredSeat10.SelectedIndex;
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            Save();
            Close();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Apply_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void Save()
        {
            for (int i = 0; i < 11; i++)
            {
                Config.PreferredSeat[i] = PreferredSeat[i];
            }
        }
    }
}

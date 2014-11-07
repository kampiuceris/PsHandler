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

using System.Drawing;
using System.Windows;
using System.Windows.Input;
using PsHandler.Custom;

namespace PsHandler.Hud
{
    /// <summary>
    /// Interaction logic for WindowBigBlind.xaml
    /// </summary>
    public partial class WindowBigBlind : Window
    {
        public Table Table;

        public WindowBigBlind(Table table)
        {
            Table = table;
            InitializeComponent();

            Loaded += (sender, args) => WinApi.SetWindowLong(this.GetHandle(), -8, Table.Handle.ToInt32());

            // drag by right mouse click
            System.Windows.Point startPosition = new System.Windows.Point();
            PreviewMouseRightButtonDown += (sender, e) =>
            {
                startPosition = e.GetPosition(this);
            };
            PreviewMouseMove += (sender, e) =>
            {
                if (!TableManager.HudBigBlindLocationLocked && e.RightButton == MouseButtonState.Pressed)
                {
                    System.Windows.Point endPosition = e.GetPosition(this);
                    Vector vector = endPosition - startPosition;
                    Left += vector.X;
                    Top += vector.Y;

                    Rectangle cr = WinApi.GetClientRectangle(Table.Handle);
                    double x = (Left - cr.Left) / cr.Width;
                    double y = (Top - cr.Top) / cr.Height;
                    TableManager.SetHudBigBlindLocationX(Table.TableHud.TableSize, (float)x, this);
                    TableManager.SetHudBigBlindLocationY(Table.TableHud.TableSize, (float)y, this);
                }
            };
        }
    }
}

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
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using PsHandler.Custom;
using PsHandler.PokerMath;

namespace PsHandler.Hud
{
    /// <summary>
    /// Interaction logic for WindowTimer.xaml
    /// </summary>
    public partial class WindowTimer : Window
    {
        public Table Table;
        public TableHud.OwnerState OwnerState;

        public WindowTimer(Table table)
        {
            Table = table;
            InitializeComponent();

            // drag by right mouse click
            System.Windows.Point startPosition = new System.Windows.Point();
            PreviewMouseRightButtonDown += (sender, e) =>
            {
                startPosition = e.GetPosition(this);
            };
            PreviewMouseMove += (sender, e) =>
            {
                if (!Config.HudTimerLocationLocked && e.RightButton == MouseButtonState.Pressed)
                {
                    System.Windows.Point endPosition = e.GetPosition(this);
                    Vector vector = endPosition - startPosition;
                    Left += vector.X;
                    Top += vector.Y;

                    Rectangle cr = WinApi.GetClientRectangle(Table.Handle);
                    double x = (Left - cr.Left) / cr.Width;
                    double y = (Top - cr.Top) / cr.Height;
                    Config.HudTimerLocationsX[(int)Table.TableHud.TableSize] = (float)x;
                    Config.HudTimerLocationsY[(int)Table.TableHud.TableSize] = (float)y;
                }
            };
        }

        public void UpdateView(string value, string toolTip)
        {
            UCLabel_Main.SetText(value);
            ToolTip = toolTip;

            Left = Table.RectangleClient.X + Table.RectangleClient.Width * Config.HudTimerLocationsX[(int)Table.TableHud.TableSize];
            Top = Table.RectangleClient.Y + Table.RectangleClient.Height * Config.HudTimerLocationsY[(int)Table.TableHud.TableSize];

            UCLabel_Main.SetBackground(Config.HudTimerBackground);
            UCLabel_Main.SetForeground(Config.HudTimerForeground);
            UCLabel_Main.SetFontFamily(Config.HudTimerFontFamily);
            UCLabel_Main.SetFontWeight(Config.HudTimerFontWeight);
            UCLabel_Main.SetFontStyle(Config.HudTimerFontStyle);
            UCLabel_Main.SetMargin(Config.HudTimerMargin);

            //UCLabel_Main.SetFontSize(Config.HudTimerFontSize);
            Viewbox_Main.Height = ((Config.HudTimerFontSize + Config.HudTimerMargin.Top + Config.HudTimerMargin.Bottom) / 546.0) * Table.RectangleClient.Height;
        }

        public void EnsureVisibility(bool isVisible)
        {
            if (Config.HudTimerEnable && isVisible)
            {
                if (OwnerState != TableHud.OwnerState.Attached)
                {
                    Visibility = Visibility.Visible;
                    this.SetOwner(Table.Handle);
                    OwnerState = TableHud.OwnerState.Attached;

                    Opacity = 1;
                    // ensure correct size
                    SizeToContent = SizeToContent.Manual;
                    SizeToContent = SizeToContent.WidthAndHeight;
                }
            }
            else
            {
                if (OwnerState != TableHud.OwnerState.Unattached)
                {
                    WinApi.SetWindowLong(this.GetHandle(), -8, 0); //const int GWL_HWNDPARENT = -8;
                    Visibility = Visibility.Collapsed;
                    OwnerState = TableHud.OwnerState.Unattached;

                    Opacity = 0;
                    // ensure correct size
                    SizeToContent = SizeToContent.Manual;
                    SizeToContent = SizeToContent.WidthAndHeight;
                }
            }
        }
    }
}

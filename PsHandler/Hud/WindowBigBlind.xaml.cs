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
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using PsHandler.Custom;
using PsHandler.PokerMath;

namespace PsHandler.Hud
{
    /// <summary>
    /// Interaction logic for WindowBigBlind.xaml
    /// </summary>
    public partial class WindowBigBlind : Window
    {
        public Table Table;
        public int Index;
        public TableHud.OwnerState OwnerState;

        public WindowBigBlind(Table table, int index)
        {
            Table = table;
            Index = index;
            InitializeComponent();

            // drag by right mouse click
            System.Windows.Point startPosition = new System.Windows.Point();
            PreviewMouseRightButtonDown += (sender, e) =>
            {
                startPosition = e.GetPosition(this);
            };
            PreviewMouseMove += (sender, e) =>
            {
                if (!Config.HudBigBlindLocationLocked && e.RightButton == MouseButtonState.Pressed)
                {
                    System.Windows.Point endPosition = e.GetPosition(this);
                    Vector vector = endPosition - startPosition;
                    Left += vector.X;
                    Top += vector.Y;

                    Rectangle cr = WinApi.GetClientRectangle(Table.Handle);
                    double x = (Left - cr.Left) / cr.Width;
                    double y = (Top - cr.Top) / cr.Height;

                    Config.HudBigBlindLocationsX[(int)Table.TableHud.TableSize][index] = (float)x;
                    Config.HudBigBlindLocationsY[(int)Table.TableHud.TableSize][index] = (float)y;
                }
            };
        }

        public void UpdateView(int index, decimal value, string toolTip, bool isHero)
        {
            if (Config.HudBigBlindDecimals > 0)
            {
                var f = ""; for (int i = 0; i < Config.HudBigBlindDecimals; i++) { f += "0"; }
                UCLabel_Main.SetText(string.Format("{1}{0:0." + f + "}{2}", value, Config.HudBigBlindPrefix, Config.HudBigBlindPostfix));
            }
            else
            {
                UCLabel_Main.SetText(string.Format("{1}{0:0}{2}", value, Config.HudBigBlindPrefix, Config.HudBigBlindPostfix));
            }

            ToolTip = toolTip ?? "";

            Left = Table.RectangleClient.X + Table.RectangleClient.Width * Config.HudBigBlindLocationsX[(int)Table.TableHud.TableSize][index];
            Top = Table.RectangleClient.Y + Table.RectangleClient.Height * Config.HudBigBlindLocationsY[(int)Table.TableHud.TableSize][index];

            if (!isHero)
            {
                UCLabel_Main.SetBackground(Config.HudBigBlindOpponentsBackground);
                UCLabel_Main.SetForeground(ColorByValue.GetColorByValue(Config.HudBigBlindOpponentsForeground, value, Config.HudBigBlindOpponentsColorsByValue));
                UCLabel_Main.SetFontFamily(Config.HudBigBlindOpponentsFontFamily);
                UCLabel_Main.SetFontWeight(Config.HudBigBlindOpponentsFontWeight);
                UCLabel_Main.SetFontStyle(Config.HudBigBlindOpponentsFontStyle);
                UCLabel_Main.SetMargin(Config.HudBigBlindOpponentsMargin);

                //UCLabel_Main.SetFontSize(Config.HudBigBlindOpponentsFontSize);
                Viewbox_Main.Height = ((Config.HudBigBlindOpponentsFontSize + Config.HudBigBlindOpponentsMargin.Top + Config.HudBigBlindOpponentsMargin.Bottom) / 546.0) * Table.RectangleClient.Height;
            }
            else
            {
                UCLabel_Main.SetBackground(Config.HudBigBlindHeroBackground);
                UCLabel_Main.SetForeground(ColorByValue.GetColorByValue(Config.HudBigBlindHeroForeground, value, Config.HudBigBlindHeroColorsByValue));
                UCLabel_Main.SetFontFamily(Config.HudBigBlindHeroFontFamily);
                UCLabel_Main.SetFontWeight(Config.HudBigBlindHeroFontWeight);
                UCLabel_Main.SetFontStyle(Config.HudBigBlindHeroFontStyle);
                UCLabel_Main.SetMargin(Config.HudBigBlindHeroMargin);

                //UCLabel_Main.SetFontSize(Config.HudBigBlindHeroFontSize);
                Viewbox_Main.Height = ((Config.HudBigBlindHeroFontSize + Config.HudBigBlindHeroMargin.Top + Config.HudBigBlindHeroMargin.Bottom) / 546.0) * Table.RectangleClient.Height;
            }
        }

        public void EnsureVisibility(bool isVisible)
        {
            if (Config.HudBigBlindEnable && isVisible)
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

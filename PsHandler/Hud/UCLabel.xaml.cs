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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Converters;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PsHandler.Hud
{
    /// <summary>
    /// Interaction logic for UCLabel.xaml
    /// </summary>
    public partial class UCLabel : UserControl
    {
        public UCLabel()
        {
            InitializeComponent();
        }

        public void SetText(string value)
        {
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
            {
                Label_Main.Content = value;
            }));
            UpdateSize();
        }

        public void SetBackground(Color value)
        {
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
            {
                Border_Main.Background = new SolidColorBrush(value);
            }));
        }

        public void SetForeground(Color value)
        {
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
            {
                Label_Main.Foreground = new SolidColorBrush(value);
            }));
        }

        public void SetFontFamily(FontFamily value)
        {
            if (value != null)
            {
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                {
                    Label_Main.FontFamily = value;
                }));
                UpdateSize();
            }
        }

        public void SetFontWeight(FontWeight? value)
        {
            if (value != null)
            {
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                {
                    Label_Main.FontWeight = (FontWeight)value;
                }));
                UpdateSize();
            }
        }

        public void SetFontStyle(FontStyle? value)
        {
            if (value != null)
            {
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                {
                    Label_Main.FontStyle = (FontStyle)value;
                }));
                UpdateSize();
            }
        }

        public void SetFontSize(double value, double scale = 1.0)
        {
            value *= scale;

            if (value < 1) value = 1;
            if (value > 72) value = 72;
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
            {
                Label_Main.FontSize = value;
            }));
            UpdateSize();
        }

        public void SetMargin(Thickness thickness, double scale = 1.0)
        {
            thickness.Left *= scale;
            thickness.Top *= scale;
            thickness.Right *= scale;
            thickness.Bottom *= scale;

            if (thickness.Left < 0) thickness.Left = 0;
            if (thickness.Left > 50) thickness.Left = 50;
            if (thickness.Top < 0) thickness.Top = 0;
            if (thickness.Top > 50) thickness.Top = 50;
            if (thickness.Right < 0) thickness.Right = 0;
            if (thickness.Right > 50) thickness.Right = 50;
            if (thickness.Bottom < 0) thickness.Bottom = 0;
            if (thickness.Bottom > 50) thickness.Bottom = 50;
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
            {
                Label_Main.Margin = thickness;
            }));
            UpdateSize();
        }

        public void SetBorderBrush(Color value)
        {
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
            {
                Border_Main.BorderBrush = new SolidColorBrush(value);
            }));
        }

        public void SetBorderThickness(Thickness thickness, double scale = 1.0)
        {
            thickness.Left *= scale;
            thickness.Top *= scale;
            thickness.Right *= scale;
            thickness.Bottom *= scale;

            if (thickness.Left < 0) thickness.Left = 0;
            if (thickness.Left > 50) thickness.Left = 50;
            if (thickness.Top < 0) thickness.Top = 0;
            if (thickness.Top > 50) thickness.Top = 50;
            if (thickness.Right < 0) thickness.Right = 0;
            if (thickness.Right > 50) thickness.Right = 50;
            if (thickness.Bottom < 0) thickness.Bottom = 0;
            if (thickness.Bottom > 50) thickness.Bottom = 50;

            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
            {
                Border_Main.BorderThickness = thickness;
            }));
            UpdateSize();
        }

        public void SetCornerRadius(CornerRadius cornerRadius, double scale = 1.0)
        {
            cornerRadius.TopLeft *= scale;
            cornerRadius.TopRight *= scale;
            cornerRadius.BottomRight *= scale;
            cornerRadius.BottomLeft *= scale;

            if (cornerRadius.TopLeft < 0) cornerRadius.TopLeft = 0;
            if (cornerRadius.TopLeft > 50) cornerRadius.TopLeft = 50;
            if (cornerRadius.TopRight < 0) cornerRadius.TopRight = 0;
            if (cornerRadius.TopRight > 50) cornerRadius.TopRight = 50;
            if (cornerRadius.BottomRight < 0) cornerRadius.BottomRight = 0;
            if (cornerRadius.BottomRight > 50) cornerRadius.BottomRight = 50;
            if (cornerRadius.BottomLeft < 0) cornerRadius.BottomLeft = 0;
            if (cornerRadius.BottomLeft > 50) cornerRadius.BottomLeft = 50;

            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
            {
                Border_Main.CornerRadius = cornerRadius;
            }));
        }

        public void UpdateSize()
        {
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
            {
                Width = Label_Main.Width + Label_Main.Margin.Left + Label_Main.Margin.Right + Border_Main.BorderThickness.Left + Border_Main.BorderThickness.Right;
                Height = Label_Main.Height + Label_Main.Margin.Top + Label_Main.Margin.Bottom + Border_Main.BorderThickness.Top + Border_Main.BorderThickness.Bottom;
            }));
        }

        public void SetToolTip(string text)
        {
            ToolTip = text;
        }
    }
}

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
using System.Windows.Controls;
using System.Windows.Media;

namespace PsHandler.ColorPicker
{
    /// <summary>
    /// Interaction logic for UcColorPreview.xaml
    /// </summary>
    public partial class UcColorPreview : UserControl
    {
        private Color _color;
        public Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                BorderColor_Main.Color = _color;
                Label_Main.Content = string.Format("#{0}", ColorARGBHSV.ToStringHex(_color));
                RaiseColorChangedEvent();
            }
        }
        public Window Owner { set; get; }

        public UcColorPreview()
            : this(Colors.White)
        {
        }

        public UcColorPreview(Color color)
        {
            InitializeComponent();

            Color = color;

            BorderColor_Main.MouseDown += (sender, args) =>
            {
                var dialog = new WindowColorPicker(Owner, Color);
                dialog.ShowDialog();
                if (dialog.Saved)
                {
                    Color = dialog.Color;
                }
            };
        }

        //

        public static readonly RoutedEvent ColorChangedEvent = EventManager.RegisterRoutedEvent("ColorChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UcColorPreview));

        public event RoutedEventHandler ColorChanged
        {
            add { AddHandler(ColorChangedEvent, value); }
            remove { RemoveHandler(ColorChangedEvent, value); }
        }

        void RaiseColorChangedEvent()
        {
            RaiseEvent(new RoutedEventArgs(UcColorPreview.ColorChangedEvent));
        }
    }
}

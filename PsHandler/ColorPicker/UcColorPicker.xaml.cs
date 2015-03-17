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
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace PsHandler.ColorPicker
{
    /// <summary>
    /// Interaction logic for UcColorPicker.xaml
    /// </summary>
    public partial class UcColorPicker : UserControl
    {
        readonly System.Drawing.Bitmap _bitmapBar = new System.Drawing.Bitmap(16, 256);
        readonly System.Drawing.Bitmap _bitmapPalette = new System.Drawing.Bitmap(256, 256);
        private bool _mouseDownOnBar = false;
        private bool _mouseDownOnPalette = false;

        public ColorARGBHSV ColorARGBHSV = new ColorARGBHSV(255, 255, 255, 255);
        private int _yBar = 0;
        private int _xPalette = 0;
        private int _yPalette = 0;

        public static List<Color> RecentColors = new List<Color>();

        public UcColorPicker()
        {
            InitializeComponent();
            // draw hue bar
            DrawBarByHue();
            // draw palette
            DrawPaletteByHue(0);
            // init combobox with colors
            foreach (var item in typeof(System.Drawing.Color).GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                ComboBox_ColorPrefabs.Items.Add(new ComboBoxItemColor(item.Name));
            }
            // init recent colors
            InitRecentColors();
            // draw transparent background for color preview rectangle
            for (int i = 0; i < 8; i++)
            {
                Grid_ColorTransparentBackground.RowDefinitions.Add(new RowDefinition());
                Grid_ColorTransparentBackground.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int column = 0; column < 8; column++)
            {
                for (int row = 0; row < 8; row++)
                {
                    var g = new Grid { Background = ((row + column) % 2 == 0) ? System.Windows.Media.Brushes.LightGray : System.Windows.Media.Brushes.White };
                    Grid_ColorTransparentBackground.Children.Add(g);
                    Grid.SetRow(g, row);
                    Grid.SetColumn(g, column);
                }
            }

            // hooks

            #region Grid Hooks

            Canvas_Main.MouseDown += (sender, args) => Canvas_Main.Focus();
            MouseUp += (sender, args) =>
            {
                _mouseDownOnBar = false;
                _mouseDownOnPalette = false;
            };
            Grid_HookBar.MouseDown += (sender, args) =>
            {
                Grid_HookBar.Focus();
                _mouseDownOnBar = true;
                _mouseDownOnPalette = false;

                MousePickBar(args);
                SyncState(Grid_HookBar);
            };
            Grid_HookPalette.MouseDown += (sender, args) =>
            {
                Grid_HookPalette.Focus();
                _mouseDownOnBar = false;
                _mouseDownOnPalette = true;

                MousePickPalette(args);
                SyncState(Grid_HookPalette);
            };
            MouseMove += (sender, args) =>
            {
                if (args.LeftButton == MouseButtonState.Pressed && _mouseDownOnBar)
                {
                    MousePickBar(args);
                    SyncState(Grid_HookBar);
                }
            };
            MouseMove += (sender, args) =>
            {
                if (args.LeftButton == MouseButtonState.Pressed && _mouseDownOnPalette)
                {
                    MousePickPalette(args);
                    SyncState(Grid_HookPalette);
                }
            };

            #endregion

            #region TextBox Hooks

            TextBox_ColorHex.TextChanged += (sender, args) =>
            {
                var textBox = TextBox_ColorHex;
                if (!ColorARGBHSV.RegexColorHex.Match(textBox.Text).Success)
                {
                    textBox.Background = System.Windows.Media.Brushes.MistyRose;
                }
                else
                {
                    textBox.Background = System.Windows.Media.Brushes.White;
                    if (!textBox.IsFocused) return;
                    SyncState(sender);
                }
            };
            TextBox_ColorAlpha.TextChanged += (sender, args) =>
            {
                var textBox = TextBox_ColorAlpha;
                int value;
                if (!int.TryParse(textBox.Text, out value) || value < 0 || value > 255)
                {
                    textBox.Background = System.Windows.Media.Brushes.MistyRose;
                }
                else
                {
                    textBox.Background = System.Windows.Media.Brushes.White;
                    if (!textBox.IsFocused) return;
                    SyncState(sender);
                }
            };
            TextBox_ColorRed.TextChanged += (sender, args) =>
            {
                var textBox = TextBox_ColorRed;
                int value;
                if (!int.TryParse(textBox.Text, out value) || value < 0 || value > 255)
                {
                    textBox.Background = System.Windows.Media.Brushes.MistyRose;
                }
                else
                {
                    textBox.Background = System.Windows.Media.Brushes.White;
                    if (!textBox.IsFocused) return;
                    SyncState(sender);
                }
            };
            TextBox_ColorGreen.TextChanged += (sender, args) =>
            {
                var textBox = TextBox_ColorGreen;
                int value;
                if (!int.TryParse(textBox.Text, out value) || value < 0 || value > 255)
                {
                    textBox.Background = System.Windows.Media.Brushes.MistyRose;
                }
                else
                {
                    textBox.Background = System.Windows.Media.Brushes.White;
                    if (!textBox.IsFocused) return;
                    SyncState(sender);
                }
            };
            TextBox_ColorBlue.TextChanged += (sender, args) =>
            {
                var textBox = TextBox_ColorBlue;
                int value;
                if (!int.TryParse(textBox.Text, out value) || value < 0 || value > 255)
                {
                    textBox.Background = System.Windows.Media.Brushes.MistyRose;
                }
                else
                {
                    textBox.Background = System.Windows.Media.Brushes.White;
                    if (!textBox.IsFocused) return;
                    SyncState(sender);
                }
            };

            //

            TextBox_ColorHue.TextChanged += (sender, args) =>
            {
                var textBox = TextBox_ColorHue;
                int value;
                if (!int.TryParse(textBox.Text, out value) || value < 0 || value > 360)
                {
                    textBox.Background = System.Windows.Media.Brushes.MistyRose;
                }
                else
                {
                    textBox.Background = System.Windows.Media.Brushes.White;
                    if (!textBox.IsFocused) return;
                    SyncState(sender);
                }
            };
            TextBox_ColorSaturation.TextChanged += (sender, args) =>
            {
                var textBox = TextBox_ColorSaturation;
                int value;
                if (!int.TryParse(textBox.Text, out value) || value < 0 || value > 100)
                {
                    textBox.Background = System.Windows.Media.Brushes.MistyRose;
                }
                else
                {
                    textBox.Background = System.Windows.Media.Brushes.White;
                    if (!textBox.IsFocused) return;
                    SyncState(sender);
                }
            };
            TextBox_ColorValue.TextChanged += (sender, args) =>
            {
                var textBox = TextBox_ColorValue;
                int value;
                if (!int.TryParse(textBox.Text, out value) || value < 0 || value > 100)
                {
                    textBox.Background = System.Windows.Media.Brushes.MistyRose;
                }
                else
                {
                    textBox.Background = System.Windows.Media.Brushes.White;
                    if (!textBox.IsFocused) return;
                    SyncState(sender);
                }
            };

            #endregion

            #region TextBox ScrollWheel Hooks

            TextBox_ColorAlpha.MouseWheel += (sender, args) =>
            {
                TextBox_ColorAlpha.Text = string.Format("{0}", Fit(GetTextBoxAlpha() + (args.Delta > 0 ? 1 : -1), 0, 255));
                SyncState(TextBox_ColorAlpha);
            };
            TextBox_ColorRed.MouseWheel += (sender, args) =>
            {
                TextBox_ColorRed.Text = string.Format("{0}", Fit(GetTextBoxRed() + (args.Delta > 0 ? 1 : -1), 0, 255));
                SyncState(TextBox_ColorRed);
            };
            TextBox_ColorGreen.MouseWheel += (sender, args) =>
            {
                TextBox_ColorGreen.Text = string.Format("{0}", Fit(GetTextBoxGreen() + (args.Delta > 0 ? 1 : -1), 0, 255));
                SyncState(TextBox_ColorGreen);
            };
            TextBox_ColorBlue.MouseWheel += (sender, args) =>
            {
                TextBox_ColorBlue.Text = string.Format("{0}", Fit(GetTextBoxBlue() + (args.Delta > 0 ? 1 : -1), 0, 255));
                SyncState(TextBox_ColorBlue);
            };
            TextBox_ColorHue.MouseWheel += (sender, args) =>
            {
                TextBox_ColorHue.Text = string.Format("{0}", Fit(GetTextBoxHue() * 359 + (args.Delta > 0 ? 1 : -1), 0, 359));
                SyncState(TextBox_ColorHue);
            };
            TextBox_ColorSaturation.MouseWheel += (sender, args) =>
            {
                TextBox_ColorSaturation.Text = string.Format("{0}", Fit(GetTextBoxSaturation() * 100 + (args.Delta > 0 ? 1 : -1), 0, 100));
                SyncState(TextBox_ColorSaturation);
            };
            TextBox_ColorValue.MouseWheel += (sender, args) =>
            {
                TextBox_ColorValue.Text = string.Format("{0}", Fit(GetTextBoxValue() * 100 + (args.Delta > 0 ? 1 : -1), 0, 100));
                SyncState(TextBox_ColorValue);
            };

            #endregion

            #region Combobox Hook

            ComboBox_ColorPrefabs.SelectionChanged += (sender, args) =>
            {
                var comboBoxItemColor = ComboBox_ColorPrefabs.SelectedItem as ComboBoxItemColor;
                if (comboBoxItemColor != null)
                {
                    SetColor(comboBoxItemColor.ColorMedia);
                }
            };

            #endregion

            TextBox_ColorAlpha.MouseDown += (sender, args) =>
            {
                _mouseDownOnBar = false;
                _mouseDownOnPalette = false;
            };

            // seed default value
            SetColor(Colors.White);
        }

        public void SetColor(Color color)
        {
            ColorARGBHSV.ColorMedia = color;
            SyncState(null);
        }

        public static void AddRecentColor(Color color)
        {
            RecentColors.RemoveAll(a => a.A == color.A && a.R == color.R && a.G == color.G && a.B == color.B);
            while (RecentColors.Count > 14)
            {
                RecentColors.RemoveAt(0);
            }
            RecentColors.Add(color);
        }

        public void InitRecentColors()
        {
            StackPanel_RecentColors.Children.Clear();

            for (int i = RecentColors.Count - 1; i >= 0; i--)
            {
                var color = RecentColors[i];
                var borderColor = new BorderColor(color)
                {
                    Margin = new Thickness(0, 0, 5, 0),
                    Width = 18,
                    Height = 18,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                };
                borderColor.MouseDown += (sender, args) => SetColor(borderColor.Color);
                StackPanel_RecentColors.Children.Add(borderColor);
            }
        }

        //

        private static int Fit(int value, int min, int max)
        {
            if (value < min) value = min;
            if (value > max) value = max;
            return value;
        }

        private static double Fit(double value, double min, double max)
        {
            if (value < min) value = min;
            if (value > max) value = max;
            return value;
        }

        private void SetColor(byte alpha, byte red, byte green, byte blue)
        {
            Grid_ColorSolid.Background = new SolidColorBrush(Color.FromArgb(255, red, green, blue));
            Grid_ColorAlpha.Background = new SolidColorBrush(Color.FromArgb(alpha, red, green, blue));
        }

        private void MousePickBar(MouseEventArgs args)
        {
            var position = args.GetPosition(Grid_HookBar);
            var y = position.Y;
            if (y < 0) y = 0;
            if (y > 255) y = 255;
            _yBar = (int)Math.Round(y);
        }

        private void MousePickPalette(MouseEventArgs args)
        {
            var position = args.GetPosition(Grid_HookPalette);
            var x = position.X;
            var y = position.Y;
            if (x < 0) x = 0;
            if (x > 255) x = 255;
            if (y < 0) y = 0;
            if (y > 255) y = 255;
            _xPalette = (int)Math.Round(x);
            _yPalette = (int)Math.Round(y);
        }

        //

        private void SyncState(object sender)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    _SyncState(sender);
                }, DispatcherPriority.Render);
            }
            catch
            {
            }
        }

        private void _SyncState(object sender)
        {
            if (Equals(TextBox_ColorHex, sender)) ColorARGBHSV = ColorARGBHSV.FromStringHex(GetTextBoxHex());
            if (Equals(TextBox_ColorAlpha, sender)) ColorARGBHSV.A = GetTextBoxAlpha();

            if (Equals(TextBox_ColorRed, sender)) ColorARGBHSV.R = GetTextBoxRed();
            if (Equals(TextBox_ColorGreen, sender)) ColorARGBHSV.G = GetTextBoxGreen();
            if (Equals(TextBox_ColorBlue, sender)) ColorARGBHSV.B = GetTextBoxBlue();
            if (Equals(TextBox_ColorHue, sender)) ColorARGBHSV.H = GetTextBoxHue();
            if (Equals(TextBox_ColorSaturation, sender)) ColorARGBHSV.S = GetTextBoxSaturation();
            if (Equals(TextBox_ColorValue, sender)) ColorARGBHSV.V = GetTextBoxValue();

            if (Equals(Grid_HookBar, sender)) ColorARGBHSV.H = _yBar / 255d;
            if (Equals(Grid_HookPalette, sender))
            {
                ColorARGBHSV.S = _xPalette / 255d;
                ColorARGBHSV.V = 1 - _yPalette / 255d;
            }

            TextBox_ColorHex.Text = string.Format("{0}", ColorARGBHSV.StringHex);
            TextBox_ColorAlpha.Text = string.Format("{0}", ColorARGBHSV.A);

            TextBox_ColorRed.Text = string.Format("{0}", ColorARGBHSV.R);
            TextBox_ColorGreen.Text = string.Format("{0}", ColorARGBHSV.G);
            TextBox_ColorBlue.Text = string.Format("{0}", ColorARGBHSV.B);
            TextBox_ColorHue.Text = string.Format("{0}", (int)Math.Round(ColorARGBHSV.H * 359));
            TextBox_ColorSaturation.Text = string.Format("{0}", (int)Math.Round(ColorARGBHSV.S * 100));
            TextBox_ColorValue.Text = string.Format("{0}", (int)Math.Round(ColorARGBHSV.V * 100));

            if (Equals(Grid_HookBar, sender) || Equals(TextBox_ColorHex, sender) || Equals(TextBox_ColorRed, sender) || Equals(TextBox_ColorGreen, sender) || Equals(TextBox_ColorBlue, sender) || Equals(TextBox_ColorHue, sender) || sender == null)
            {
                SetPointerBar((int)Math.Round(ColorARGBHSV.H * 255));
                DrawPaletteByHue(ColorARGBHSV.H);
            }
            SetPointerPalette((int)Math.Round(ColorARGBHSV.S * 255), 255 - (int)Math.Round(ColorARGBHSV.V * 255));

            SetColor(ColorARGBHSV.A, ColorARGBHSV.R, ColorARGBHSV.G, ColorARGBHSV.B);
        }

        //

        #region Get TextBox Values = Hex/Alpha/Red/Green/Blue/Hue/Saturation/Value

        private string GetTextBoxHex()
        {
            var match = ColorARGBHSV.RegexColorHex.Match(TextBox_ColorHex.Text);
            if (match.Success)
            {
                return TextBox_ColorHex.Text;
            }
            return ColorARGBHSV.StringHex;
        }

        private byte GetTextBoxAlpha()
        {
            int value;
            var success = int.TryParse(TextBox_ColorAlpha.Text, out value);
            if (!success) value = ColorARGBHSV.A;
            if (value < 0) value = 0;
            if (value > 255) value = 255;
            return (byte)value;
        }

        private byte GetTextBoxRed()
        {
            int value;
            var success = int.TryParse(TextBox_ColorRed.Text, out value);
            if (!success) value = ColorARGBHSV.R;
            if (value < 0) value = 0;
            if (value > 255) value = 255;
            return (byte)value;
        }

        private byte GetTextBoxGreen()
        {
            int value;
            var success = int.TryParse(TextBox_ColorGreen.Text, out value);
            if (!success) value = ColorARGBHSV.G;
            if (value < 0) value = 0;
            if (value > 255) value = 255;
            return (byte)value;
        }

        private byte GetTextBoxBlue()
        {
            int value;
            var success = int.TryParse(TextBox_ColorBlue.Text, out value);
            if (!success) value = ColorARGBHSV.B;
            if (value < 0) value = 0;
            if (value > 255) value = 255;
            return (byte)value;
        }

        private double GetTextBoxHue()
        {
            double value;
            var success = double.TryParse(TextBox_ColorHue.Text, out value);
            if (!success) value = Math.Round(ColorARGBHSV.H * 359);
            if (value < 0) value = 0;
            if (value > 359) value = 359;
            return value / 359;
        }

        private double GetTextBoxSaturation()
        {
            double value;
            var success = double.TryParse(TextBox_ColorSaturation.Text, out value);
            if (!success) value = ColorARGBHSV.S * 100;
            if (value < 0) value = 0;
            if (value > 100) value = 100;
            return value / 100;
        }

        private double GetTextBoxValue()
        {
            double value;
            var success = double.TryParse(TextBox_ColorValue.Text, out value);
            if (!success) value = ColorARGBHSV.V * 100;
            if (value < 0) value = 0;
            if (value > 100) value = 100;
            return value / 100;
        }

        #endregion

        private void SetPointerBar(int valueY)
        {
            var canvasLeft = Canvas.GetLeft(Grid_HookBar);
            var canvasTop = Canvas.GetTop(Grid_HookBar);

            double x = canvasLeft;
            double y = canvasTop + valueY;

            Canvas.SetLeft(Image_PointerBar, x - 4);
            Canvas.SetTop(Image_PointerBar, y - 3);
        }

        private void SetPointerPalette(int valueX, int valueY)
        {
            var canvasLeft = Canvas.GetLeft(Grid_HookPalette);
            var canvasTop = Canvas.GetTop(Grid_HookPalette);

            double x = canvasLeft + valueX;
            double y = canvasTop + valueY;

            Canvas.SetLeft(Image_PointerPalette, x - 7);
            Canvas.SetTop(Image_PointerPalette, y - 7);
        }

        //

        private void DrawBarByHue()
        {
            byte r = 0, g = 0, b = 0;
            System.Drawing.Color color;
            for (int y = 0; y < 256; y++)
            {
                ColorARGBHSV.HSVtoRGB((float)(y / 256.0), 1, 1, out r, out g, out b);
                color = System.Drawing.Color.FromArgb(255, r, g, b);
                for (int x = 0; x < 16; x++)
                {
                    _bitmapBar.SetPixel(x, y, color);
                }
            }
            Image_Bar.Source = _bitmapBar.ToImageSource();
        }

        private void DrawPaletteByHue(double hue)
        {
            byte r = 0, g = 0, b = 0;
            System.Drawing.Color color;
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 256; y++)
                {
                    ColorARGBHSV.HSVtoRGB(hue, (float)(x / 256.0), (float)((256 - y) / 256.0), out r, out g, out b);
                    color = System.Drawing.Color.FromArgb(255, r, g, b);
                    _bitmapPalette.SetPixel(x, y, color);
                }
            }
            Image_Palette.Source = _bitmapPalette.ToImageSource();
        }
    }

    public static class Extensions
    {
        public static System.Windows.Media.ImageSource ToImageSource(this System.Drawing.Image image)
        {
            return ((System.Drawing.Bitmap)image).ToImageSource();
        }

        public static System.Windows.Media.ImageSource ToImageSource(this System.Drawing.Bitmap bitmap)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
        }
    }

    public class ComboBoxItemColor : ComboBoxItem
    {
        public string ColorName;
        public Color ColorMedia;
        public System.Drawing.Color ColorDrawing;

        public ComboBoxItemColor(string colorName)
        {
            ColorName = colorName;
            System.Drawing.Color color = System.Drawing.Color.FromName(ColorName);
            ColorDrawing = color;
            ColorMedia = Color.FromArgb(color.A, color.R, color.G, color.B);

            var r = new System.Windows.Shapes.Rectangle
            {
                Width = 14,
                Height = 14,
                VerticalAlignment = VerticalAlignment.Center,
                Fill = new SolidColorBrush(ColorMedia),
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1
            };

            if (color.A == 0)
            {
                r.Fill = new LinearGradientBrush
                {
                    GradientStops = new GradientStopCollection(1)
                };
            }

            var l = new Label
            {
                Height = 16,
                VerticalAlignment = VerticalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(0),
                Content = ColorName,
                Margin = new Thickness(5, 0, 0, 0)
            };

            var sp = new StackPanel
            {
                Height = 16,
                Orientation = Orientation.Horizontal
            };
            sp.Children.Add(r);
            sp.Children.Add(l);

            Content = sp;
        }
    }

    public class BorderColor : System.Windows.Controls.Border
    {
        private const int TRANSPARENT_BACKGROUND_DENSE = 4;
        private System.Windows.Controls.Grid _gridColor;
        private System.Windows.Media.Color _color;
        public System.Windows.Media.Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                _gridColor.Background = new SolidColorBrush(_color);
            }
        }

        public BorderColor()
            : this(Colors.White)
        {
        }

        public BorderColor(System.Windows.Media.Color color)
        {
            BorderBrush = System.Windows.Media.Brushes.DimGray;
            BorderThickness = new System.Windows.Thickness(1);

            // craete main grid (container) for border (this)
            var grid = new System.Windows.Controls.Grid();

            // create transparent background (white/gray blocks)
            var gridTransparentBackground = new System.Windows.Controls.Grid();
            for (int i = 0; i < TRANSPARENT_BACKGROUND_DENSE; i++)
            {
                gridTransparentBackground.RowDefinitions.Add(new RowDefinition());
                gridTransparentBackground.ColumnDefinitions.Add(new ColumnDefinition());
            }
            for (int column = 0; column < TRANSPARENT_BACKGROUND_DENSE; column++)
            {
                for (int row = 0; row < TRANSPARENT_BACKGROUND_DENSE; row++)
                {
                    var g = new System.Windows.Controls.Grid
                    {
                        Background = ((row + column) % 2 == 0) ? System.Windows.Media.Brushes.LightGray : System.Windows.Media.Brushes.White
                    };
                    gridTransparentBackground.Children.Add(g);
                    System.Windows.Controls.Grid.SetRow(g, row);
                    System.Windows.Controls.Grid.SetColumn(g, column);
                }
            }
            grid.Children.Add(gridTransparentBackground);

            // create grid for actual color
            _gridColor = new System.Windows.Controls.Grid
            {
                Background = System.Windows.Media.Brushes.Transparent,
            };
            grid.Children.Add(_gridColor);

            Child = grid;

            // init color
            Color = color;
        }
    }
}

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
using System.Windows.Shapes;

namespace PsHandler.Hud
{
    public class HudCustomizeParams
    {
        public enum HudCustomizeType { HudTimer, HudBigBlindOpponents, HudBigBlindHero }

        private readonly HudCustomizeType _hudCustomizeType;
        public string PreviewText;
        public Color Background;
        public Color Foreground;
        public FontFamily FontFamily;
        public FontWeight FontWeight;
        public FontStyle FontStyle;
        public double FontSize;
        public Thickness Margin;

        public HudCustomizeParams(HudCustomizeType hudCustomizeType)
        {
            _hudCustomizeType = hudCustomizeType;

            string previewTextTimer = "01:23";

            string decimalsFormal = "";
            for (int i = 0; i < Config.HudBigBlindDecimals; i++) decimalsFormal += "0";
            if (decimalsFormal.Length > 0) decimalsFormal = "." + decimalsFormal;
            string previewTextBB = string.Format("{0:0" + decimalsFormal + "}", 12.3456); // 12.3456 test data

            switch (_hudCustomizeType)
            {
                case HudCustomizeType.HudTimer:
                    PreviewText = previewTextTimer;
                    Background = Config.HudTimerBackground;
                    Foreground = Config.HudTimerForeground;
                    FontFamily = Config.HudTimerFontFamily;
                    FontWeight = Config.HudTimerFontWeight;
                    FontStyle = Config.HudTimerFontStyle;
                    FontSize = Config.HudTimerFontSize;
                    Margin = Config.HudTimerMargin;
                    break;
                case HudCustomizeType.HudBigBlindOpponents:
                    PreviewText = previewTextBB;
                    Background = Config.HudBigBlindOpponentsBackground;
                    Foreground = Config.HudBigBlindOpponentsForeground;
                    FontFamily = Config.HudBigBlindOpponentsFontFamily;
                    FontWeight = Config.HudBigBlindOpponentsFontWeight;
                    FontStyle = Config.HudBigBlindOpponentsFontStyle;
                    FontSize = Config.HudBigBlindOpponentsFontSize;
                    Margin = Config.HudBigBlindOpponentsMargin;
                    break;
                case HudCustomizeType.HudBigBlindHero:
                    PreviewText = previewTextBB;
                    Background = Config.HudBigBlindHeroBackground;
                    Foreground = Config.HudBigBlindHeroForeground;
                    FontFamily = Config.HudBigBlindHeroFontFamily;
                    FontWeight = Config.HudBigBlindHeroFontWeight;
                    FontStyle = Config.HudBigBlindHeroFontStyle;
                    FontSize = Config.HudBigBlindHeroFontSize;
                    Margin = Config.HudBigBlindHeroMargin;
                    break;
            }
        }

        public void Save()
        {
            switch (_hudCustomizeType)
            {
                case HudCustomizeType.HudTimer:
                    Config.HudTimerBackground = Background;
                    Config.HudTimerForeground = Foreground;
                    Config.HudTimerFontFamily = FontFamily;
                    Config.HudTimerFontWeight = FontWeight;
                    Config.HudTimerFontStyle = FontStyle;
                    Config.HudTimerFontSize = FontSize;
                    Config.HudTimerMargin = Margin;
                    break;
                case HudCustomizeType.HudBigBlindOpponents:
                    Config.HudBigBlindOpponentsBackground = Background;
                    Config.HudBigBlindOpponentsForeground = Foreground;
                    Config.HudBigBlindOpponentsFontFamily = FontFamily;
                    Config.HudBigBlindOpponentsFontWeight = FontWeight;
                    Config.HudBigBlindOpponentsFontStyle = FontStyle;
                    Config.HudBigBlindOpponentsFontSize = FontSize;
                    Config.HudBigBlindOpponentsMargin = Margin;
                    break;
                case HudCustomizeType.HudBigBlindHero:
                    Config.HudBigBlindHeroBackground = Background;
                    Config.HudBigBlindHeroForeground = Foreground;
                    Config.HudBigBlindHeroFontFamily = FontFamily;
                    Config.HudBigBlindHeroFontWeight = FontWeight;
                    Config.HudBigBlindHeroFontStyle = FontStyle;
                    Config.HudBigBlindHeroFontSize = FontSize;
                    Config.HudBigBlindHeroMargin = Margin;
                    break;
            }
        }
    }

    /// <summary>
    /// Interaction logic for WindowHudDesign.xaml
    /// </summary>
    public partial class WindowHudDesign : Window
    {
        public HudCustomizeParams HudCustomizeParams;

        public WindowHudDesign(Window owner, HudCustomizeParams.HudCustomizeType hudCustomizeType)
        {
            InitializeComponent();
            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            HudCustomizeParams = new HudCustomizeParams(hudCustomizeType);

            // init values

            foreach (var item in typeof(System.Drawing.Color).GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                ComboBox_Background.Items.Add(new ComboBoxItemColor(item.Name));
                ComboBox_Foreground.Items.Add(new ComboBoxItemColor(item.Name));
            }

            foreach (var item in Fonts.SystemFontFamilies)
            {
                ComboBox_FontFamily.Items.Add(new ComboBoxItemFontFamilyFontWeightFontStyle(item));
            }

            ComboBox_FontWeight.Items.Add(new ComboBoxItemFontFamilyFontWeightFontStyle(FontWeights.Black));
            ComboBox_FontWeight.Items.Add(new ComboBoxItemFontFamilyFontWeightFontStyle(FontWeights.Bold));
            ComboBox_FontWeight.Items.Add(new ComboBoxItemFontFamilyFontWeightFontStyle(FontWeights.DemiBold));
            ComboBox_FontWeight.Items.Add(new ComboBoxItemFontFamilyFontWeightFontStyle(FontWeights.ExtraBlack));
            ComboBox_FontWeight.Items.Add(new ComboBoxItemFontFamilyFontWeightFontStyle(FontWeights.ExtraBold));
            ComboBox_FontWeight.Items.Add(new ComboBoxItemFontFamilyFontWeightFontStyle(FontWeights.ExtraLight));
            ComboBox_FontWeight.Items.Add(new ComboBoxItemFontFamilyFontWeightFontStyle(FontWeights.Heavy));
            ComboBox_FontWeight.Items.Add(new ComboBoxItemFontFamilyFontWeightFontStyle(FontWeights.Light));
            ComboBox_FontWeight.Items.Add(new ComboBoxItemFontFamilyFontWeightFontStyle(FontWeights.Medium));
            ComboBox_FontWeight.Items.Add(new ComboBoxItemFontFamilyFontWeightFontStyle(FontWeights.Normal));
            ComboBox_FontWeight.Items.Add(new ComboBoxItemFontFamilyFontWeightFontStyle(FontWeights.Regular));
            ComboBox_FontWeight.Items.Add(new ComboBoxItemFontFamilyFontWeightFontStyle(FontWeights.SemiBold));
            ComboBox_FontWeight.Items.Add(new ComboBoxItemFontFamilyFontWeightFontStyle(FontWeights.Thin));
            ComboBox_FontWeight.Items.Add(new ComboBoxItemFontFamilyFontWeightFontStyle(FontWeights.UltraBlack));
            ComboBox_FontWeight.Items.Add(new ComboBoxItemFontFamilyFontWeightFontStyle(FontWeights.UltraBold));
            ComboBox_FontWeight.Items.Add(new ComboBoxItemFontFamilyFontWeightFontStyle(FontWeights.UltraLight));

            ComboBox_FontStyle.Items.Add(new ComboBoxItemFontFamilyFontWeightFontStyle(FontStyles.Italic));
            ComboBox_FontStyle.Items.Add(new ComboBoxItemFontFamilyFontWeightFontStyle(FontStyles.Normal));
            ComboBox_FontStyle.Items.Add(new ComboBoxItemFontFamilyFontWeightFontStyle(FontStyles.Oblique));

            // hook

            ComboBox_Background.SelectionChanged += (o, args) => UCLabel_Preview.SetBackground(((ComboBoxItemColor)ComboBox_Background.SelectedItem).ColorMedia);
            ComboBox_Foreground.SelectionChanged += (o, args) => UCLabel_Preview.SetForeground(((ComboBoxItemColor)ComboBox_Foreground.SelectedItem).ColorMedia);
            ComboBox_FontFamily.SelectionChanged += (o, args) => UCLabel_Preview.SetFontFamily(((ComboBoxItemFontFamilyFontWeightFontStyle)ComboBox_FontFamily.SelectedItem).SystemFontFamily);
            ComboBox_FontWeight.SelectionChanged += (o, args) => UCLabel_Preview.SetFontWeight(((ComboBoxItemFontFamilyFontWeightFontStyle)ComboBox_FontWeight.SelectedItem).SystemFontWeight);
            ComboBox_FontStyle.SelectionChanged += (o, args) => UCLabel_Preview.SetFontStyle(((ComboBoxItemFontFamilyFontWeightFontStyle)ComboBox_FontStyle.SelectedItem).SystemFontStyle);
            TextBox_FontSize.TextChanged += (sender, args) => { double value; if (double.TryParse(TextBox_FontSize.Text, out value)) UCLabel_Preview.SetFontSize(value); };
            TextBox_Margin.TextChanged += (sender, args) =>
            {
                try
                {
                    string[] split = TextBox_Margin.Text.Split(new[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                    UCLabel_Preview.SetMargin(new Thickness(double.Parse(split[0]), double.Parse(split[1]), double.Parse(split[2]), double.Parse(split[3])));
                    TextBox_Margin.Background = Brushes.White;
                }
                catch
                {
                    TextBox_Margin.Background = Brushes.MistyRose;
                }
            };

            // seed values

            SeedValues();
        }

        private void SeedValues()
        {
            UCLabel_Preview.SetText(HudCustomizeParams.PreviewText);

            foreach (ComboBoxItemColor item in ComboBox_Background.Items.Cast<object>().OfType<ComboBoxItemColor>().Where(item => item.ColorMedia.Equals(HudCustomizeParams.Background)))
            {
                ComboBox_Background.SelectedItem = item;
                break;
            }

            foreach (ComboBoxItemColor item in ComboBox_Foreground.Items.Cast<object>().OfType<ComboBoxItemColor>().Where(item => item.ColorMedia.Equals(HudCustomizeParams.Foreground)))
            {
                ComboBox_Foreground.SelectedItem = item;
                break;
            }

            foreach (ComboBoxItemFontFamilyFontWeightFontStyle item in ComboBox_FontFamily.Items.Cast<object>().OfType<ComboBoxItemFontFamilyFontWeightFontStyle>().Where(item => item.SystemFontFamily.Equals(HudCustomizeParams.FontFamily)))
            {
                ComboBox_FontFamily.SelectedItem = item;
                break;
            }

            foreach (ComboBoxItemFontFamilyFontWeightFontStyle item in ComboBox_FontWeight.Items.Cast<object>().OfType<ComboBoxItemFontFamilyFontWeightFontStyle>().Where(item => item.SystemFontWeight.Equals(HudCustomizeParams.FontWeight)))
            {
                ComboBox_FontWeight.SelectedItem = item;
                break;
            }

            foreach (ComboBoxItemFontFamilyFontWeightFontStyle item in ComboBox_FontStyle.Items.Cast<object>().OfType<ComboBoxItemFontFamilyFontWeightFontStyle>().Where(item => item.SystemFontStyle.Equals(HudCustomizeParams.FontStyle)))
            {
                ComboBox_FontStyle.SelectedItem = item;
                break;
            }

            TextBox_FontSize.Text = HudCustomizeParams.FontSize.ToString(CultureInfo.InvariantCulture);

            TextBox_Margin.Text = HudCustomizeParams.Margin.Left + " " + HudCustomizeParams.Margin.Top + " " + HudCustomizeParams.Margin.Right + " " + HudCustomizeParams.Margin.Bottom;
        }

        private bool CollectParams()
        {
            var ComboBox_BackgroundSelectedItem = (ComboBoxItemColor)ComboBox_Background.SelectedItem;
            if (ComboBox_BackgroundSelectedItem != null)
            {
                HudCustomizeParams.Background = ComboBox_BackgroundSelectedItem.ColorMedia;
            }

            var ComboBox_ForegroundSelectedItem = (ComboBoxItemColor)ComboBox_Foreground.SelectedItem;
            if (ComboBox_ForegroundSelectedItem != null)
            {
                HudCustomizeParams.Foreground = ComboBox_ForegroundSelectedItem.ColorMedia;
            }

            var ComboBox_FontFamilySelectedItem = (ComboBoxItemFontFamilyFontWeightFontStyle)ComboBox_FontFamily.SelectedItem;
            if (ComboBox_FontFamilySelectedItem != null)
            {
                HudCustomizeParams.FontFamily = ComboBox_FontFamilySelectedItem.SystemFontFamily;
            }

            var ComboBox_FontWeightSelectedItem = (ComboBoxItemFontFamilyFontWeightFontStyle)ComboBox_FontWeight.SelectedItem;
            if (ComboBox_FontWeightSelectedItem != null)
            {
                if (ComboBox_FontWeightSelectedItem.SystemFontWeight != null)
                {
                    HudCustomizeParams.FontWeight = (FontWeight)ComboBox_FontWeightSelectedItem.SystemFontWeight;
                }
            }

            var ComboBox_FontStyleSelectedItem = (ComboBoxItemFontFamilyFontWeightFontStyle)ComboBox_FontStyle.SelectedItem;
            if (ComboBox_FontStyleSelectedItem != null)
            {
                if (ComboBox_FontStyleSelectedItem.SystemFontStyle != null)
                {
                    HudCustomizeParams.FontStyle = (FontStyle)ComboBox_FontStyleSelectedItem.SystemFontStyle;
                }
            }

            double fontSize;
            if (double.TryParse(TextBox_FontSize.Text, out fontSize))
            {
                HudCustomizeParams.FontSize = fontSize;
            }

            try
            {
                string[] split = TextBox_Margin.Text.Split(new[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                HudCustomizeParams.Margin = new Thickness(double.Parse(split[0]), double.Parse(split[1]), double.Parse(split[2]), double.Parse(split[3]));
            }
            catch
            {
                HudCustomizeParams.Margin = new Thickness(2, 2, 2, 2);
            }

            return true;
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            if (CollectParams())
            {
                HudCustomizeParams.Save();
                Close();
            }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Apply_Click(object sender, RoutedEventArgs e)
        {
            if (CollectParams())
            {
                HudCustomizeParams.Save();
            }
        }
    }

    //

    public class ComboBoxItemFontFamilyFontWeightFontStyle : ComboBoxItem
    {
        public FontFamily SystemFontFamily;
        public FontWeight? SystemFontWeight;
        public FontStyle? SystemFontStyle;

        public ComboBoxItemFontFamilyFontWeightFontStyle(FontFamily systemFontFamily)
            : this(systemFontFamily, null, null)
        {
        }

        public ComboBoxItemFontFamilyFontWeightFontStyle(FontWeight systemFontWeight)
            : this(null, systemFontWeight, null)
        {
        }

        public ComboBoxItemFontFamilyFontWeightFontStyle(FontStyle systemFontStyle)
            : this(null, null, systemFontStyle)
        {
        }

        public ComboBoxItemFontFamilyFontWeightFontStyle(FontFamily systemFontFamily, FontWeight? systemFontWeight, FontStyle? systemFontStyle)
        {
            Label l = new Label
            {
                Height = 16,
                VerticalAlignment = VerticalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(0),
                Foreground = Brushes.Black,
                Content = "Text"
            };

            if (systemFontFamily != null)
            {
                SystemFontFamily = systemFontFamily;
                l.FontFamily = SystemFontFamily;
            }
            if (systemFontWeight != null)
            {
                SystemFontWeight = systemFontWeight;
                l.FontWeight = (FontWeight)SystemFontWeight;
            }
            if (systemFontStyle != null)
            {
                SystemFontStyle = systemFontStyle;
                l.FontStyle = (FontStyle)systemFontStyle;
            }

            if (systemFontFamily != null) l.Content = SystemFontFamily.ToString();
            if (systemFontWeight != null) l.Content = SystemFontWeight.ToString();
            if (systemFontStyle != null) l.Content = SystemFontStyle.ToString();


            StackPanel sp = new StackPanel
            {
                Height = 16,
                Orientation = Orientation.Horizontal
            };
            sp.Children.Add(l);

            Content = sp;
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

            Rectangle r = new Rectangle
            {
                Width = 14,
                Height = 14,
                VerticalAlignment = VerticalAlignment.Center,
                Fill = new SolidColorBrush(ColorMedia),
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1
            };

            Label l = new Label
            {
                Height = 16,
                VerticalAlignment = VerticalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Padding = new Thickness(0),
                Content = ColorName,
                Margin = new Thickness(5, 0, 0, 0)
            };

            StackPanel sp = new StackPanel
            {
                Height = 16,
                Orientation = Orientation.Horizontal
            };
            sp.Children.Add(r);
            sp.Children.Add(l);

            Content = sp;
        }
    }
}

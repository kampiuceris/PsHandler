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
    /// <summary>
    /// Interaction logic for WindowHudDesign.xaml
    /// </summary>
    public partial class WindowHudDesign : Window
    {
        public bool Saved = false;
        public string PreviewText = "01:23";

        public Color HudBackground = Colors.Black;
        public Color HudForeground = Colors.White;
        public FontFamily HudFontFamily = new FontFamily("Consolas");
        public FontWeight HudFontWeight = FontWeights.Bold;
        public FontStyle HudFontStyle = FontStyles.Normal;
        public double HudFontSize = 10;
        public Thickness HudMargin = new Thickness(2, 2, 2, 2);

        public WindowHudDesign(Window owner, Color hudBackground, Color hudForeground, FontFamily hudFontFamily, FontWeight hudFontWeight, FontStyle hudFontStyle, double hudFontSize, Thickness hudMargin, string previewText)
        {
            InitializeComponent();
            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            HudBackground = hudBackground;
            HudForeground = hudForeground;
            HudFontFamily = hudFontFamily;
            HudFontWeight = hudFontWeight;
            HudFontStyle = hudFontStyle;
            HudFontSize = hudFontSize;
            HudMargin = hudMargin;
            PreviewText = previewText;

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
            UCLabel_Preview.SetText(PreviewText);

            foreach (ComboBoxItemColor item in ComboBox_Background.Items.Cast<object>().OfType<ComboBoxItemColor>().Where(item => item.ColorMedia.Equals(HudBackground)))
            {
                ComboBox_Background.SelectedItem = item;
                break;
            }

            foreach (ComboBoxItemColor item in ComboBox_Foreground.Items.Cast<object>().OfType<ComboBoxItemColor>().Where(item => item.ColorMedia.Equals(HudForeground)))
            {
                ComboBox_Foreground.SelectedItem = item;
                break;
            }

            foreach (ComboBoxItemFontFamilyFontWeightFontStyle item in ComboBox_FontFamily.Items.Cast<object>().OfType<ComboBoxItemFontFamilyFontWeightFontStyle>().Where(item => item.SystemFontFamily.Equals(HudFontFamily)))
            {
                ComboBox_FontFamily.SelectedItem = item;
                break;
            }

            foreach (ComboBoxItemFontFamilyFontWeightFontStyle item in ComboBox_FontWeight.Items.Cast<object>().OfType<ComboBoxItemFontFamilyFontWeightFontStyle>().Where(item => item.SystemFontWeight.Equals(HudFontWeight)))
            {
                ComboBox_FontWeight.SelectedItem = item;
                break;
            }

            foreach (ComboBoxItemFontFamilyFontWeightFontStyle item in ComboBox_FontStyle.Items.Cast<object>().OfType<ComboBoxItemFontFamilyFontWeightFontStyle>().Where(item => item.SystemFontStyle.Equals(HudFontStyle)))
            {
                ComboBox_FontStyle.SelectedItem = item;
                break;
            }

            TextBox_FontSize.Text = HudFontSize.ToString(CultureInfo.InvariantCulture);

            TextBox_Margin.Text = HudMargin.Left + " " + HudMargin.Top + " " + HudMargin.Right + " " + HudMargin.Bottom;
        }

        private void Button_SaveAndClose_Click(object sender, RoutedEventArgs e)
        {
            var ComboBox_BackgroundSelectedItem = (ComboBoxItemColor)ComboBox_Background.SelectedItem;
            if (ComboBox_BackgroundSelectedItem != null)
            {
                HudBackground = ComboBox_BackgroundSelectedItem.ColorMedia;
            }

            var ComboBox_ForegroundSelectedItem = (ComboBoxItemColor)ComboBox_Foreground.SelectedItem;
            if (ComboBox_ForegroundSelectedItem != null)
            {
                HudForeground = ComboBox_ForegroundSelectedItem.ColorMedia;
            }

            var ComboBox_FontFamilySelectedItem = (ComboBoxItemFontFamilyFontWeightFontStyle)ComboBox_FontFamily.SelectedItem;
            if (ComboBox_FontFamilySelectedItem != null)
            {
                HudFontFamily = ComboBox_FontFamilySelectedItem.SystemFontFamily;
            }

            var ComboBox_FontWeightSelectedItem = (ComboBoxItemFontFamilyFontWeightFontStyle)ComboBox_FontWeight.SelectedItem;
            if (ComboBox_FontWeightSelectedItem != null)
            {
                if (ComboBox_FontWeightSelectedItem.SystemFontWeight != null)
                {
                    HudFontWeight = (FontWeight)ComboBox_FontWeightSelectedItem.SystemFontWeight;
                }
            }

            var ComboBox_FontStyleSelectedItem = (ComboBoxItemFontFamilyFontWeightFontStyle)ComboBox_FontStyle.SelectedItem;
            if (ComboBox_FontStyleSelectedItem != null)
            {
                if (ComboBox_FontStyleSelectedItem.SystemFontStyle != null)
                {
                    HudFontStyle = (FontStyle)ComboBox_FontStyleSelectedItem.SystemFontStyle;
                }
            }

            double fontSize;
            if (double.TryParse(TextBox_FontSize.Text, out fontSize))
            {
                HudFontSize = fontSize;
            }

            try
            {
                string[] split = TextBox_Margin.Text.Split(new[] { ",", " " }, StringSplitOptions.RemoveEmptyEntries);
                HudMargin = new Thickness(double.Parse(split[0]), double.Parse(split[1]), double.Parse(split[2]), double.Parse(split[3]));
            }
            catch
            {
                HudMargin = new Thickness(2, 2, 2, 2);
            }

            Saved = true;
            Close();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Saved = false;
            Close();
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

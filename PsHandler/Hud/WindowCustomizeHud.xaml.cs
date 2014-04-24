using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;


namespace PsHandler.Hud
{
    /// <summary>
    /// Interaction logic for WindowCustomizeHud.xaml
    /// </summary>
    public partial class WindowCustomizeHud : Window
    {
        public WindowCustomizeHud()
        {
            InitializeComponent();

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

            ComboBox_Background.SelectionChanged += (o, args) => Timer_Main.SetBackground(((ComboBoxItemColor)ComboBox_Background.SelectedItem).ColorMedia);
            ComboBox_Foreground.SelectionChanged += (o, args) => Timer_Main.SetForeground(((ComboBoxItemColor)ComboBox_Foreground.SelectedItem).ColorMedia);
            ComboBox_FontFamily.SelectionChanged += (o, args) => Timer_Main.SetFontFamily(((ComboBoxItemFontFamilyFontWeightFontStyle)ComboBox_FontFamily.SelectedItem).SystemFontFamily);
            ComboBox_FontWeight.SelectionChanged += (o, args) => Timer_Main.SetFontWeight(((ComboBoxItemFontFamilyFontWeightFontStyle)ComboBox_FontWeight.SelectedItem).SystemFontWeight);
            ComboBox_FontStyle.SelectionChanged += (o, args) => Timer_Main.SetFontStyle(((ComboBoxItemFontFamilyFontWeightFontStyle)ComboBox_FontStyle.SelectedItem).SystemFontStyle);
            TextBox_FontSize.TextChanged += (sender, args) => { double value; if (double.TryParse(TextBox_FontSize.Text, out value)) Timer_Main.SetFontSize(value); };

            // config values

            ConfigValues();
        }

        private void ConfigValues()
        {
            CheckBox_Locked.IsChecked = HudManager.TimerHudLocationLocked;

            foreach (ComboBoxItemColor item in ComboBox_Background.Items.Cast<object>().OfType<ComboBoxItemColor>().Where(item => item.ColorMedia.Equals(HudManager.TimerHudBackground)))
            {
                ComboBox_Background.SelectedItem = item;
                break;
            }

            foreach (ComboBoxItemColor item in ComboBox_Foreground.Items.Cast<object>().OfType<ComboBoxItemColor>().Where(item => item.ColorMedia.Equals(HudManager.TimerHudForeground)))
            {
                ComboBox_Foreground.SelectedItem = item;
                break;
            }

            foreach (ComboBoxItemFontFamilyFontWeightFontStyle item in ComboBox_FontFamily.Items.Cast<object>().OfType<ComboBoxItemFontFamilyFontWeightFontStyle>().Where(item => item.SystemFontFamily.Equals(HudManager.TimerHudFontFamily)))
            {
                ComboBox_FontFamily.SelectedItem = item;
                break;
            }

            foreach (ComboBoxItemFontFamilyFontWeightFontStyle item in ComboBox_FontWeight.Items.Cast<object>().OfType<ComboBoxItemFontFamilyFontWeightFontStyle>().Where(item => item.SystemFontWeight.Equals(HudManager.TimerHudFontWeight)))
            {
                ComboBox_FontWeight.SelectedItem = item;
                break;
            }

            foreach (ComboBoxItemFontFamilyFontWeightFontStyle item in ComboBox_FontStyle.Items.Cast<object>().OfType<ComboBoxItemFontFamilyFontWeightFontStyle>().Where(item => item.SystemFontStyle.Equals(HudManager.TimerHudFontStyle)))
            {
                ComboBox_FontStyle.SelectedItem = item;
                break;
            }

            TextBox_FontSize.Text = HudManager.TimerHudFontSize.ToString(CultureInfo.InvariantCulture);
        }

        private void Button_SaveAndClose_Click(object sender, RoutedEventArgs e)
        {
            HudManager.TimerHudLocationLocked = CheckBox_Locked.IsChecked == true;

            var ComboBox_BackgroundSelectedItem = (ComboBoxItemColor)ComboBox_Background.SelectedItem;
            if (ComboBox_BackgroundSelectedItem != null)
            {
                HudManager.TimerHudBackground = ComboBox_BackgroundSelectedItem.ColorMedia;
            }

            var ComboBox_ForegroundSelectedItem = (ComboBoxItemColor)ComboBox_Foreground.SelectedItem;
            if (ComboBox_ForegroundSelectedItem != null)
            {
                HudManager.TimerHudForeground = ComboBox_ForegroundSelectedItem.ColorMedia;
            }

            var ComboBox_FontFamilySelectedItem = (ComboBoxItemFontFamilyFontWeightFontStyle)ComboBox_FontFamily.SelectedItem;
            if (ComboBox_FontFamilySelectedItem != null)
            {
                HudManager.TimerHudFontFamily = ComboBox_FontFamilySelectedItem.SystemFontFamily;
            }

            var ComboBox_FontWeightSelectedItem = (ComboBoxItemFontFamilyFontWeightFontStyle)ComboBox_FontWeight.SelectedItem;
            if (ComboBox_FontWeightSelectedItem != null)
            {
                if (ComboBox_FontWeightSelectedItem.SystemFontWeight != null)
                {
                    HudManager.TimerHudFontWeight = (FontWeight)ComboBox_FontWeightSelectedItem.SystemFontWeight;
                }
            }

            var ComboBox_FontStyleSelectedItem = (ComboBoxItemFontFamilyFontWeightFontStyle)ComboBox_FontStyle.SelectedItem;
            if (ComboBox_FontStyleSelectedItem != null)
            {
                if (ComboBox_FontStyleSelectedItem.SystemFontStyle != null)
                {
                    HudManager.TimerHudFontStyle = (FontStyle)ComboBox_FontStyleSelectedItem.SystemFontStyle;
                }
            }

            double fontSize;
            if (double.TryParse(TextBox_FontSize.Text, out fontSize))
            {
                HudManager.TimerHudFontSize = fontSize;
            }

            Close();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_ResetLocation_Click(object sender, RoutedEventArgs e)
        {
            HudManager.TimerHudLocationX = 0;
            HudManager.TimerHudLocationY = 0;
        }
    }

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

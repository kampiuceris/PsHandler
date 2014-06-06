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

namespace PsHandler.Hud
{
    /// <summary>
    /// Interaction logic for WindowCustomizeColorsByValue.xaml
    /// </summary>
    public partial class WindowCustomizeColorsByValue : Window
    {
        public bool Saved = false;
        public List<ColorByValue> ColorsByValue;

        public WindowCustomizeColorsByValue(Window owner, Color defaultColor, List<ColorByValue> ColorsByValue = null)
        {
            InitializeComponent();
            Owner = owner;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            Rectangle_Default.Fill = new SolidColorBrush(defaultColor);
            Label_Default.Content = defaultColor.ToString();

            if (ColorsByValue == null) ColorsByValue = new List<ColorByValue>();
            foreach (var colorByValue in ColorsByValue)
            {
                StackPanel_ColorsByValue.Children.Add(new UCColorByValue(StackPanel_ColorsByValue, colorByValue));
            }
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            StackPanel_ColorsByValue.Children.Add(new UCColorByValue(StackPanel_ColorsByValue));
        }

        private void Button_SaveAndClose_Click(object sender, RoutedEventArgs e)
        {
            ColorsByValue = new List<ColorByValue>();

            foreach (var item in StackPanel_ColorsByValue.Children.OfType<UCColorByValue>())
            {
                ColorByValue colorByValue = item.GetColorByValue();
                if (colorByValue == null)
                {
                    MessageBox.Show(string.Format("Invalid one or more 'Color By Value' input."), "Error saving", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                ColorsByValue.Add(colorByValue);
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
}

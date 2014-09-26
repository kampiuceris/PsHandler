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
                Background = new SolidColorBrush(value);
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

        public void UpdateSize()
        {
            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
            {
                Width = Label_Main.Width + Label_Main.Margin.Left + Label_Main.Margin.Right;
                Height = Label_Main.Height + Label_Main.Margin.Top + Label_Main.Margin.Bottom;
            }));
        }

        public void SetToolTip(string text)
        {
            ToolTip = text;
        }
    }
}

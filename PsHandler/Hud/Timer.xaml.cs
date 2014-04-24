using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PsHandler.Hud
{
    /// <summary>
    /// Interaction logic for Timer.xaml
    /// </summary>
    public partial class Timer : UserControl
    {
        public Timer()
        {
            InitializeComponent();
        }

        public void SetText(string value)
        {
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
            {
                Label_Timer.Content = value;
            }));
            UpdateSize();
        }

        public void SetBackground(Color value)
        {
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
            {
                Background = new SolidColorBrush(value);
            }));
        }

        public void SetForeground(Color value)
        {
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
            {
                Label_Timer.Foreground = new SolidColorBrush(value);
            }));
        }

        public void SetFontFamily(FontFamily value)
        {
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
            {
                Label_Timer.FontFamily = value;
            }));
            UpdateSize();
        }

        public void SetFontWeight(FontWeight? value)
        {
            if (value != null)
            {
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                {
                    Label_Timer.FontWeight = (FontWeight)value;
                }));
                UpdateSize();
            }
        }

        public void SetFontStyle(FontStyle? value)
        {
            if (value != null)
            {
                Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                {
                    Label_Timer.FontStyle = (FontStyle)value;
                }));
                UpdateSize();
            }
        }

        public void SetFontSize(double value)
        {
            if (value < 1) value = 1;
            if (value > 72) value = 72;
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
            {
                Label_Timer.FontSize = value;
            }));
            UpdateSize();
        }

        public void UpdateSize()
        {
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
            {
                Width = Label_Timer.Width + Label_Timer.Margin.Left + +Label_Timer.Margin.Right;
                Height = Label_Timer.Height + Label_Timer.Margin.Top + +Label_Timer.Margin.Bottom;
            }));
        }
    }
}

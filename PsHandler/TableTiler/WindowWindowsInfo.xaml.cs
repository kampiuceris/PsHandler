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
using System.Windows.Shapes;

namespace PsHandler.TableTiler
{
    /// <summary>
    /// Interaction logic for WindowWindowsInfo.xaml
    /// </summary>
    public partial class WindowWindowsInfo : Window
    {
        struct WindowInfo
        {
            public IntPtr Handle;
            public string Class;
            public string Title;
            public System.Drawing.Rectangle Rectangle;
        }

        private List<WindowInfo> _windowInfos = new List<WindowInfo>();

        public WindowWindowsInfo()
        {
            InitializeComponent();
            Refresh();
            UpdateInfo();
        }

        private void Refresh()
        {
            _windowInfos.Clear();

            foreach (IntPtr handle in WinApi.GetWindowHWndAll().Where(handle => !Methods.IsMinimized(handle)))
            {
                _windowInfos.Add(new WindowInfo { Handle = handle, Class = WinApi.GetClassName(handle), Title = WinApi.GetWindowTitle(handle), Rectangle = WinApi.GetWindowRectangle(handle) });
            }
        }

        private void UpdateInfo()
        {
            StringBuilder sb = new StringBuilder();
            foreach (WindowInfo windowInfo in _windowInfos)
            {
                bool passesFilters = true;
                if (!string.IsNullOrEmpty(TextBox_SearchPattern.Text))
                {
                    if (!windowInfo.Title.ToLowerInvariant().Contains(TextBox_SearchPattern.Text.ToLowerInvariant()))
                    {
                        passesFilters = false;
                    }
                }
                if (CheckBox_PokerStarsTableWindowsOnly.IsChecked == true)
                {
                    if (!windowInfo.Class.Equals("PokerStarsTableFrameClass"))
                    {
                        passesFilters = false;
                    }
                }
                if (passesFilters)
                {
                    sb.AppendLine(string.Format("{0,-20} \"{1}\"", string.Format("{0} {1} {2} {3}", windowInfo.Rectangle.X, windowInfo.Rectangle.Y, windowInfo.Rectangle.Width, windowInfo.Rectangle.Height), windowInfo.Title));
                }
            }
            TextBox_Info.Text = sb.ToString();
        }

        private void Button_Refresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
            UpdateInfo();
        }

        private void TextBox_SearchPattern_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateInfo();
        }

        private void CheckBox_PokerStarsTableWindowsOnly_Checked(object sender, RoutedEventArgs e)
        {
            UpdateInfo();
        }

        private void CheckBox_PokerStarsTableWindowsOnly_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateInfo();
        }
    }
}

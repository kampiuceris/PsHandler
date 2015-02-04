// PsHandler - poker software helping tool.
// Copyright (C) 2014  kampiuceris

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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PsHandler.UI
{
    public enum WindowMessageImage
    {
        None,
        Error,
        Information,
        Question,
        Warning
    }

    public enum WindowMessageButtons
    {
        OK,
        OKCancel,
        YesNo,
        YesNoCancel
    }

    public enum WindowMessageResult
    {
        Cancel,
        No,
        None,
        OK,
        Yes
    }

    /// <summary>
    /// Interaction logic for WindowMessage.xaml
    /// </summary>
    public partial class WindowMessage : Window
    {
        public WindowMessageResult Result = WindowMessageResult.None;

        public WindowMessage(string message, string title, WindowMessageButtons buttons, WindowMessageImage image, Window owner, WindowStartupLocation windowStartupLocation, FontFamily fontFamily = null)
        {
            InitializeComponent();

            if (owner != null)
            {
                Owner = owner;
            }
            WindowStartupLocation = windowStartupLocation;

            Title = title;
            TextBlock_Message.Text = message;

            Image_Error.Visibility = Visibility.Collapsed;
            Image_Information.Visibility = Visibility.Collapsed;
            Image_Question.Visibility = Visibility.Collapsed;
            Image_Warning.Visibility = Visibility.Collapsed;
            switch (image)
            {
                case WindowMessageImage.Error:
                    Image_Error.Visibility = Visibility.Visible;
                    break;
                case WindowMessageImage.Information:
                    Image_Information.Visibility = Visibility.Visible;
                    break;
                case WindowMessageImage.Question:
                    Image_Question.Visibility = Visibility.Visible;
                    break;
                case WindowMessageImage.Warning:
                    Image_Warning.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }

            Button_OK.Visibility = Visibility.Collapsed;
            Button_Yes.Visibility = Visibility.Collapsed;
            Button_No.Visibility = Visibility.Collapsed;
            Button_Cancel.Visibility = Visibility.Collapsed;
            switch (buttons)
            {
                case WindowMessageButtons.OK:
                    Button_OK.Visibility = Visibility.Visible;
                    break;
                case WindowMessageButtons.OKCancel:
                    Button_OK.Visibility = Visibility.Visible;
                    Button_OK.Margin = new Thickness(0, 0, 5, 0);
                    Button_Cancel.Visibility = Visibility.Visible;
                    break;
                case WindowMessageButtons.YesNo:
                    Button_Yes.Visibility = Visibility.Visible;
                    Button_Yes.Margin = new Thickness(0, 0, 5, 0);
                    Button_No.Visibility = Visibility.Visible;
                    break;
                case WindowMessageButtons.YesNoCancel:
                    Button_Yes.Visibility = Visibility.Visible;
                    Button_Yes.Margin = new Thickness(0, 0, 5, 0);
                    Button_No.Visibility = Visibility.Visible;
                    Button_No.Margin = new Thickness(0, 0, 5, 0);
                    Button_Cancel.Visibility = Visibility.Visible;
                    break;
                default:
                    Button_OK.Visibility = Visibility.Visible;
                    break;
            }

            if (fontFamily != null)
            {
                TextBlock_Message.FontFamily = fontFamily;
            }
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            Result = WindowMessageResult.OK;
            Close();
        }

        private void Button_Yes_Click(object sender, RoutedEventArgs e)
        {
            Result = WindowMessageResult.Yes;
            Close();
        }

        private void Button_No_Click(object sender, RoutedEventArgs e)
        {
            Result = WindowMessageResult.No;
            Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Result = WindowMessageResult.Cancel;
            Close();
        }

        //

        public static WindowMessageResult Show(string message, string title, WindowMessageButtons buttons, WindowMessageImage image, Window owner, WindowStartupLocation windowStartupLocation = WindowStartupLocation.CenterOwner, FontFamily fontFamily = null)
        {
            WindowMessage windowMessage = new WindowMessage(message, title, buttons, image, owner, windowStartupLocation, fontFamily);
            ((Window)windowMessage).Show();
            return windowMessage.Result;
        }

        public static WindowMessageResult ShowDialog(string message, string title, WindowMessageButtons buttons, WindowMessageImage image, Window owner, WindowStartupLocation windowStartupLocation = WindowStartupLocation.CenterOwner, FontFamily fontFamily = null)
        {
            WindowMessage windowMessage = new WindowMessage(message, title, buttons, image, owner, windowStartupLocation, fontFamily);
            ((Window)windowMessage).ShowDialog();
            return windowMessage.Result;
        }
    }
}

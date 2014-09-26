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

        public WindowMessage(string message, string title, WindowMessageButtons buttons, WindowMessageImage image, Window owner)
        {
            InitializeComponent();

            if (owner != null)
            {
                Owner = owner;
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }

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

        public static WindowMessageResult Show(string message, string title, WindowMessageButtons buttons, WindowMessageImage image, Window owner)
        {
            WindowMessage windowMessage = new WindowMessage(message, title, buttons, image, owner);
            ((Window)windowMessage).Show();
            return windowMessage.Result;
        }

        public static WindowMessageResult ShowDialog(string message, string title, WindowMessageButtons buttons, WindowMessageImage image, Window owner)
        {
            WindowMessage windowMessage = new WindowMessage(message, title, buttons, image, owner);
            ((Window)windowMessage).ShowDialog();
            return windowMessage.Result;
        }
    }
}

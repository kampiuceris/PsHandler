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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PsHandler.UI
{
    /// <summary>
    /// Interaction logic for ButtonImage.xaml
    /// </summary>
    public partial class ButtonImage : Button
    {
        private ImageSource _imageSource;
        public ImageSource ImageSource
        {
            get
            {
                return _imageSource;
            }
            set
            {
                _imageSource = value;
                Image_Main.Source = _imageSource;
            }
        }
        private bool _imageVisible;
        public bool ImageVisible
        {
            get
            {
                return _imageVisible;
            }
            set
            {
                _imageVisible = value;
                Image_Main.Visibility = _imageVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                Label_Main.Content = _text;
            }
        }

        public ButtonImage()
        {
            InitializeComponent();
        }
    }
}

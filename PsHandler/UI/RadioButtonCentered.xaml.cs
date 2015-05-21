using System.Windows.Controls;

namespace PsHandler.UI
{
    /// <summary>
    /// Interaction logic for RadioButtonCentered.xaml
    /// </summary>
    public partial class RadioButtonCentered : RadioButton
    {
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

        public RadioButtonCentered()
        {
            InitializeComponent();
        }
    }
}

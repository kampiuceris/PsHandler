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
    /// Interaction logic for UCHud.xaml
    /// </summary>
    public partial class UCHud : UserControl
    {
        public UCHud()
        {
            InitializeComponent();

            // Seed

            CheckBox_EnableHud.IsChecked = Config.EnableHud;

            // Hook

            CheckBox_EnableHud.Checked += (sender, args) =>
            {
                Config.EnableHud = true;
            };
            CheckBox_EnableHud.Unchecked += (sender, args) =>
            {
                Config.EnableHud = false;
            };


        }
    }
}

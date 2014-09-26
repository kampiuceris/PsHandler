using System;
using System.Collections.Generic;
using System.Globalization;
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
using PsHandler.Custom;
using PsHandler.Import;

namespace PsHandler.UI
{
    /// <summary>
    /// Interaction logic for UCStatusBar.xaml
    /// </summary>
    public partial class UCStatusBar : UserControl, IObserverHandHistoryManager, IObserverTableManagerTableCount
    {
        public UCStatusBar()
        {
            InitializeComponent();
        }

        public void SetImportedTournaments(int value)
        {
            Methods.UiInvoke(() =>
            {
                Label_ImportedTournaments.Content = value;
            });
        }

        public void SetImportedHands(int value)
        {
            Methods.UiInvoke(() =>
            {
                Label_ImportedHands.Content = value;
            });
        }

        public void SetImportedErrors(int value)
        {
            Methods.UiInvoke(() =>
            {
                Label_ImportedErrors.Content = value;
                if (value <= 0)
                {
                    Image_ImportedErrors.Visibility = Visibility.Collapsed;
                    Label_ErrorsCaption.Visibility = Visibility.Collapsed;
                    Label_ImportedErrors.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Image_ImportedErrors.Visibility = Visibility.Visible;
                    Label_ErrorsCaption.Visibility = Visibility.Visible;
                    Label_ImportedErrors.Visibility = Visibility.Visible;
                }
            });
        }

        public void SetTableCount(int value)
        {
            Methods.UiInvoke(() =>
            {
                Label_Tables.Content = value;
            });
        }
    }
}

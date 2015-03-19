// PsHandler - poker software helping tool.
// Copyright (C) 2014-2015  kampiuceris

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

using System.Windows;
using System.Windows.Controls;
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

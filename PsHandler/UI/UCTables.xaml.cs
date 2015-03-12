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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using PsHandler.Annotations;
using PsHandler.Custom;

namespace PsHandler.UI
{
    /// <summary>
    /// Interaction logic for UCTables.xaml
    /// </summary>
    public partial class UCTables : UserControl, IObserverTableManagerTableList
    {
        private readonly ObservableCollection<TableInfo> _tablesInfo = new ObservableCollection<TableInfo>();

        public UCTables()
        {
            InitializeComponent();
            ListView_TablesInfo.ItemsSource = _tablesInfo;
        }

        public void UpdateView(List<Table> tables)
        {
            Methods.UiInvoke(() =>
            {
                // remove non-existing
                foreach (var tableInfo in _tablesInfo.Where(ti => tables.All(t => t != ti.Table)).ToArray())
                {
                    _tablesInfo.Remove(tableInfo);
                }
                // find of add new
                foreach (var table in tables)
                {
                    TableInfo tableInfo = _tablesInfo.FirstOrDefault(ti => ti.Table == table);
                    if (tableInfo == null)
                    {
                        tableInfo = new TableInfo(table);
                        _tablesInfo.Add(tableInfo);
                    }
                    tableInfo.Update();
                }
                // fit grid view
                GridView_TablesInfo.ResetColumnWidths();
            });
        }
    }

    public sealed class TableInfo : INotifyPropertyChanged
    {
        public Table Table;
        private readonly ImageSource _imageSource;
        //
        public ImageSource ImageSource { get { return _imageSource; } }
        public string Title { get { return Table.Title; } }

        public TableInfo(Table table)
        {
            Table = table;
            _imageSource = new BitmapImage(new Uri(string.Format(@"pack://application:,,,/Images/Resources/Size16x16/poker.png"), UriKind.Absolute));
        }

        public void Update()
        {
            //NotifyPropertyChanged("ImageSource");
            NotifyPropertyChanged("Title");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

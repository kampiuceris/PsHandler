using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using PsHandler.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using PsHandler.Custom;

namespace PsHandler.UI
{
    public interface IFilter
    {
        Regex RegexWindowTitle { get; }
        Regex RegexWindowClass { get; }
    }

    /// <summary>
    /// Interaction logic for WindowWindowsInfo.xaml
    /// </summary>
    public partial class WindowWindowsInfo : Window
    {
        private IFilter _iFilter;
        private readonly Thread _thread;
        private readonly object _currentInfoLock = new object();
        private readonly ObservableCollection<Info> _currentInfo = new ObservableCollection<Info>();

        public WindowWindowsInfo(IFilter iFilter)
        {
            _iFilter = iFilter;
            InitializeComponent();

            ListView_Main.ItemsSource = _currentInfo;

            // Context Menu
            ContextMenu contextMenu = new ContextMenu();
            ListView_Main.ContextMenu = contextMenu;
            MenuItem menuItem = new MenuItem { Header = "Copy to clipboard: X Y Width Height" };
            menuItem.Click += (sender, args) =>
            {
                Info selectedItem = ListView_Main.SelectedItem as Info;
                if (selectedItem != null) Clipboard.SetText(string.Format("{0} {1} {2} {3}", selectedItem.X, selectedItem.Y, selectedItem.Width, selectedItem.Height));
            };
            contextMenu.Items.Add(menuItem);
            menuItem = new MenuItem { Header = "Copy to clipboard: Title" };
            menuItem.Click += (sender, args) =>
            {
                Info selectedItem = ListView_Main.SelectedItem as Info;
                if (selectedItem != null) Clipboard.SetText(String.IsNullOrEmpty(selectedItem.Title) ? "" : selectedItem.Title);
            };
            contextMenu.Items.Add(menuItem);
            menuItem = new MenuItem { Header = "Copy to clipboard: WindowClass" };
            menuItem.Click += (sender, args) =>
            {
                Info selectedItem = ListView_Main.SelectedItem as Info;
                if (selectedItem != null) Clipboard.SetText(String.IsNullOrEmpty(selectedItem.WindowClass) ? "" : selectedItem.WindowClass);
            };
            contextMenu.Items.Add(menuItem);

            // Update Thread
            _thread = new Thread(() =>
            {
                while (true)
                {
                    UpdateList();
                    Thread.Sleep(250);
                }
            });
            _thread.Start();
        }

        private void UpdateList()
        {
            List<Info> latestInfo = (from handle in WinApi.GetWindowHWndAll().Where(handle => !Methods.IsMinimized(handle))
                                     let rect = WinApi.GetWindowRectangle(handle)
                                     select new Info
                                     {
                                         Handle = handle,
                                         Title = WinApi.GetWindowTitle(handle),
                                         X = rect.X,
                                         Y = rect.Y,
                                         Width = rect.Width,
                                         Height = rect.Height,
                                         WindowClass = WinApi.GetClassName(handle)
                                     }).ToList();
            latestInfo.Sort((o1, o2) => o1.Y - o2.Y);
            latestInfo.Sort((o1, o2) => o1.X - o2.X);

            Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
            {
                lock (_currentInfoLock)
                {
                    // remove
                    var toRemove = _currentInfo.Where(item => !latestInfo.Any(o => o.Handle.Equals(item.Handle))).ToList();
                    foreach (var item in toRemove)
                    {
                        _currentInfo.Remove(item);
                    }

                    // update / add
                    foreach (var item in latestInfo)
                    {
                        if (_iFilter.RegexWindowTitle.IsMatch(item.Title) && _iFilter.RegexWindowClass.IsMatch(item.WindowClass)) { item.PassesFilter = true; } else { item.PassesFilter = false; }

                        var firstOrDefault = _currentInfo.FirstOrDefault(o => o.Handle.Equals(item.Handle));
                        if (firstOrDefault != null)
                        {
                            firstOrDefault.X = item.X;
                            firstOrDefault.Y = item.Y;
                            firstOrDefault.Width = item.Width;
                            firstOrDefault.Height = item.Height;
                            firstOrDefault.Title = item.Title;
                            firstOrDefault.WindowClass = item.WindowClass;
                            firstOrDefault.PassesFilter = item.PassesFilter;
                        }
                        else
                        {
                            _currentInfo.Add(item);
                        }
                    }
                }

                // fit grid view
                //GridView_Main.ResetColumnWidths();
            }));
        }

        private GridViewColumnHeader _lastHeaderClicked;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;

        private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    string header = headerClicked.Column.Header as string;
                    Sort(header, direction);

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate = Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate = Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    // Remove arrow from previously sorted header 
                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }


                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(ListView_Main.ItemsSource ?? ListView_Main.Items);
            dataView.SortDescriptions.Clear();

            switch (sortBy)
            {
                case "Passes Filter?":
                    sortBy = "PassesFilter";
                    break;
                case "X":
                    sortBy = "X";
                    break;
                case "Y":
                    sortBy = "Y";
                    break;
                case "Width":
                    sortBy = "Width";
                    break;
                case "Height":
                    sortBy = "Height";
                    break;
                case "Title":
                    sortBy = "Title";
                    break;
            }

            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _thread.Abort();
            base.OnClosing(e);
        }
    }

    public sealed class Info : INotifyPropertyChanged
    {
        private IntPtr _handle;
        private int _x;
        private int _y;
        private int _width;
        private int _height;
        private string _title;
        private string _windowClass;
        private bool _passesFilter;

        public IntPtr Handle { get { return _handle; } set { _handle = value; NotifyPropertyChanged(); } }
        public int X { get { return _x; } set { _x = value; NotifyPropertyChanged(); } }
        public int Y { get { return _y; } set { _y = value; NotifyPropertyChanged(); } }
        public int Width { get { return _width; } set { _width = value; NotifyPropertyChanged(); } }
        public int Height { get { return _height; } set { _height = value; NotifyPropertyChanged(); } }
        public string Title { get { return _title; } set { _title = value; NotifyPropertyChanged(); } }
        public string WindowClass { get { return _windowClass; } set { _windowClass = value; NotifyPropertyChanged(); } }
        public bool PassesFilter { get { return _passesFilter; } set { _passesFilter = value; NotifyPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return Title;
        }
    }
}

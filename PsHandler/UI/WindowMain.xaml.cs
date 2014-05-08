using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using PsHandler.PokerTypes;
using ContextMenu = System.Windows.Controls.ContextMenu;
using MenuItem = System.Windows.Controls.MenuItem;
using Point = System.Drawing.Point;

namespace PsHandler.UI
{
    /// <summary>
    /// Interaction logic for WindowMain.xaml
    /// </summary>
    public partial class WindowMain : Window
    {
        public bool IsClosing;

        public bool Importing { set { Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate { Grid_Import.Visibility = value ? Visibility.Visible : Visibility.Hidden; })); } }

        public WindowMain()
        {
            InitializeComponent();

            Title = Config.NAME + " v1." + Config.VERSION;

            TaskbarIcon_NotifyIcon.ContextMenu = GetNotifyIconContextMenu();
            TaskbarIcon_NotifyIcon.ToolTipText = Config.NAME + " v1." + Config.VERSION;
            TaskbarIcon_NotifyIcon.TrayMouseDoubleClick += (sender, args) =>
            {
                Show();
                WindowState = WindowState.Normal;
                //MyNotifyIcon.Visibility = Visibility.Hidden;
            };
            Loaded += (sender, args) =>
            {
                if (Config.StartMinimized)
                {
                    WindowState = WindowState.Minimized;
                }
                UCTableTiler.UpdateListView();
                UCPokerTypes.UpdateListView();
            };

            if (CheckAndFixStartingLocation(new Rectangle(Config.GuiLocationX, Config.GuiLocationY, Config.GuiWidth, Config.GuiHeight)))
            {
                if (Config.SaveGuiSize)
                {
                    Width = Config.GuiWidth;
                    Height = Config.GuiHeight;
                }
                if (Config.SaveGuiLocation)
                {
                    Left = Config.GuiLocationX;
                    Top = Config.GuiLocationY;
                }
            }
        }

        private bool CheckAndFixStartingLocation(Rectangle rectangle)
        {
            var corners = new List<Point>
            {
                new Point(rectangle.Left, rectangle.Top),
                new Point(rectangle.Right, rectangle.Top),
                new Point(rectangle.Right, rectangle.Bottom),
                new Point(rectangle.Left, rectangle.Bottom),
            };
            return Screen.AllScreens.Select(screen => corners.Any(p => Methods.CheckIfPointIsInArea(p, screen.WorkingArea))).Any(containsMatchingCorner => containsMatchingCorner);
        }

        private ContextMenu GetNotifyIconContextMenu()
        {
            MenuItem miExit = new MenuItem { Header = "Exit" };
            miExit.Click += (sender, args) => Close();

            ContextMenu cm = new ContextMenu();
            cm.Items.Add(miExit);

            return cm;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                if (Config.MinimizeToSystemTray)
                {
                    Hide();
                }
                UCSettings.CheckBox_MinimizeToSystemTray.IsEnabled = false;
            }
            else
            {
                UCSettings.CheckBox_MinimizeToSystemTray.IsEnabled = true;
            }
            base.OnStateChanged(e);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!IsClosing)
            {
                e.Cancel = true;
                Config.GuiLocationX = (int)Left;
                Config.GuiLocationY = (int)Top;
                Config.GuiWidth = (int)Width;
                Config.GuiHeight = (int)Height;
                App.Quit();
            }
            else
            {
                TaskbarIcon_NotifyIcon.Dispose();
                base.OnClosing(e);
            }
        }
    }
}

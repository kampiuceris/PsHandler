﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using PsHandler.Custom;
using Image = System.Windows.Controls.Image;

namespace PsHandler.UI
{
    /// <summary>
    /// Interaction logic for WindowMain.xaml
    /// </summary>
    public partial class WindowMain : Window
    {
        public bool IsClosing;

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
                Topmost = true;
                Topmost = false;
            };
            Loaded += (sender, args) =>
            {
                if (Config.StartMinimized)
                {
                    WindowState = WindowState.Minimized;
                }
                UCTableTiler.UpdateListView();
                UCHud.UCHudPokerTypes.UpdateListView();
            };

            if (CheckAndFixStartingLocation(new System.Drawing.Rectangle(Config.GuiLocationX, Config.GuiLocationY, Config.GuiWidth, Config.GuiHeight)))
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

        private bool CheckAndFixStartingLocation(System.Drawing.Rectangle rectangle)
        {
            var corners = new List<System.Drawing.Point>
            {
                new System.Drawing.Point(rectangle.Left, rectangle.Top),
                new System.Drawing.Point(rectangle.Right, rectangle.Top),
                new System.Drawing.Point(rectangle.Right, rectangle.Bottom),
                new System.Drawing.Point(rectangle.Left, rectangle.Bottom),
            };
            return System.Windows.Forms.Screen.AllScreens.Select(screen => corners.Any(p => Methods.CheckIfPointIsInArea(p, screen.WorkingArea))).Any(containsMatchingCorner => containsMatchingCorner);
        }

        private ContextMenu GetNotifyIconContextMenu()
        {
            MenuItem miExit = new MenuItem { Header = "Exit" };
            miExit.Click += (sender, args) => Close();
            miExit.Icon = new Image
            {
                Source = Methods.GetEmbeddedResourceBitmap(string.Format("PsHandler.Images.EmbeddedResources.Size16x16.door_out.png")).ToBitmapSource(),
                Width = 16,
                Height = 16,
                Margin = new Thickness(5, 0, 0, 0)
            };
            ContextMenu cm = new ContextMenu();
            cm.Items.Add(miExit);
            cm.UseLayoutRounding = true;

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
            }

            base.OnStateChanged(e);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!IsClosing)
            {
                e.Cancel = true;
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

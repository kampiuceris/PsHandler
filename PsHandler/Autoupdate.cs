using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Hardcodet.Wpf.TaskbarNotification;
using System.Windows.Controls;
using Color = System.Windows.Media.Color;
using Image = System.Windows.Controls.Image;

namespace PsHandler
{
    public class Autoupdate
    {
        private static Thread _threadUpdate;

        private static string GetMd5(string path, bool upperCaseHex = false)
        {
            using (MD5 md5 = MD5.Create())
            {
                using (FileStream fileStream = File.OpenRead(path))
                {
                    byte[] bytes = md5.ComputeHash(fileStream); // get bytes
                    StringBuilder result = new StringBuilder(bytes.Length * 2); // to hex string
                    for (int i = 0; i < bytes.Length; i++) result.Append(bytes[i].ToString(upperCaseHex ? "X2" : "x2"));
                    return result.ToString();
                }
            }
        }

        public static void CheckForUpdates(string hrefPhp, string hrefXml, string applicationName, string exeName, string exeDir, Window owner, Action quitAction, BitmapSource iconWindow, BitmapSource iconButtonCancel, BitmapSource iconButtonUpdate)
        {
            _threadUpdate = new Thread(() =>
            {
                try
                {
                    bool trayBalloonTipClickedRoutedEventHandlerAdded = false;
                    while (true)
                    {
                        string updateFileHref, updateFileNameFullPath, version, releaseNotes;
                        if (CheckForUpdatesAndDeleteFiles(hrefPhp, out updateFileHref, out updateFileNameFullPath, out version, out releaseNotes))
                        {
                            // needs update
                            App.TaskbarIcon.ShowBalloonTip(string.Format("{0} Update", applicationName), string.Format("Latest {0} version is available. Click here to update.", version), BalloonIcon.Info);
                            if (!trayBalloonTipClickedRoutedEventHandlerAdded)
                            {
                                App.TaskbarIcon.TrayBalloonTipClicked += (sender, args) => ShowWindowUpdateDialog(hrefXml, applicationName, exeName, exeDir,
                                    owner, quitAction, updateFileHref, updateFileNameFullPath, version, releaseNotes, iconWindow, iconButtonCancel, iconButtonUpdate);
                                trayBalloonTipClickedRoutedEventHandlerAdded = true;
                            }
                        }
                        Thread.Sleep(7200000); // 2 hours
                    }
                }
                catch
                {
                }
            });
            _threadUpdate.Start();
        }

        private static bool CheckForUpdatesAndDeleteFiles(string href, out string autoupdateHref, out string autoupdateFileNameFullPath, out string version, out string releaseNotes)
        {
            using (WebClient webClient = new WebClient { Proxy = null })
            {
                // download xml file
                string xml = webClient.DownloadString(href);

                // read and create XElement from xml text
                XElement root = XDocument.Parse(xml).Root;
                if (root == null) throw new Exception(string.Format("Invalid '{0}' xml file.", href));

                var exeDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                var tempDir = new DirectoryInfo(Path.GetTempPath());

                // delete files
                foreach (var file in root.Elements().Where(e => e.Name.LocalName.Equals("delete")).Select(xElement => xElement.Attribute("name").Value))
                {
                    if (exeDir.GetFiles().Any(o => o.Name.Equals(file)))
                    {
                        File.Delete(file);
                    }
                }

                // version
                version = root.Attribute("version").Value;

                // update file
                autoupdateHref = root.Elements().First(e => e.Name.LocalName.Equals("update")).Attribute("href").Value;
                autoupdateFileNameFullPath = tempDir.FullName + root.Elements().First(e => e.Name.LocalName.Equals("update")).Attribute("name").Value;
                string autoupdateFileName = root.Elements().First(e => e.Name.LocalName.Equals("update")).Attribute("name").Value;

                // release notes
                releaseNotes = "";
                try
                {
                    releaseNotes = root.Elements().First(e => e.Name.LocalName.Equals("updateinfo")).Value;
                }
                catch
                {
                }

                // delete update file
                var exeDirUpdateFile = exeDir.GetFiles().FirstOrDefault(o => o.Name.Equals(autoupdateFileName));
                if (exeDirUpdateFile != null) exeDirUpdateFile.Delete();
                var tempDirUpdateFile = tempDir.GetFiles().FirstOrDefault(o => o.Name.Equals(autoupdateFileName));
                if (tempDirUpdateFile != null) tempDirUpdateFile.Delete();

                // check files
                string[][] files = root.Elements().Where(e => e.Name.LocalName.Equals("file")).Select(e => new[] { e.Attribute("name").Value, e.Attribute("md5").Value }).ToArray();

                foreach (string[] f in files)
                {
                    string fileName = f[0];
                    string md5 = f[1];

                    if (!File.Exists(fileName)) return true;
                    if (!GetMd5(fileName).Equals(md5)) return true;
                }

                return false;
            }
        }

        private static void ShowWindowUpdateDialog(string hrefXml, string applicationName, string exeName, string exeDir, Window owner, Action quitAction,
            string updateFileHref, string updateFileNameFullPath, string version, string releaseNotes, BitmapSource iconWindow, BitmapSource iconButtonCancel, BitmapSource iconButtonUpdate)
        {
            // Window

            Window window = new Window
            {
                Owner = owner,
                WindowStartupLocation = owner.WindowState == WindowState.Minimized ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner,
                UseLayoutRounding = true,
                Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xF0, 0xF0, 0xF0)),
                MaxWidth = 1280,
                MaxHeight = 720,
                MinWidth = 320,
                MinHeight = 200,
                Width = 480,
                Height = 360,
                ResizeMode = ResizeMode.CanResize,
                Icon = iconWindow,
                Title = string.Format("{0} Update", applicationName)
            };

            #region Window Content

            // Grid

            Grid grid = new Grid
            {
                Margin = new Thickness(10)
            };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100, GridUnitType.Pixel) });
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(175, GridUnitType.Pixel) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(24, GridUnitType.Pixel) });
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(5, GridUnitType.Pixel) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(24, GridUnitType.Pixel) });

            // Label

            Label label = new Label
            {
                Content = string.Format("Release notes:"),
                Padding = new Thickness(0),
                VerticalAlignment = VerticalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontWeight = FontWeights.Bold
            };
            Grid.SetRow(label, 0);
            Grid.SetColumn(label, 0);
            Grid.SetColumnSpan(label, 3);

            // TextBox

            TextBox textBox = new TextBox
            {
                IsReadOnly = true,
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
                Text = releaseNotes,
                Padding = new Thickness(5)
            };
            Grid.SetRow(textBox, 1);
            Grid.SetColumn(textBox, 0);
            Grid.SetColumnSpan(textBox, 3);

            // Button Cancel

            Button buttonCancel = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            Grid.SetRow(buttonCancel, 3);
            Grid.SetColumn(buttonCancel, 0);
            StackPanel stackPanelCancel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(10, 2, 10, 2)
            };
            Image imageCancel = new Image
            {
                Width = 16,
                Height = 16,
                Source = iconButtonCancel
            };
            TextBlock textBlockCancel = new TextBlock
            {
                Text = "Cancel",
                Margin = new Thickness(5, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            stackPanelCancel.Children.Add(imageCancel);
            stackPanelCancel.Children.Add(textBlockCancel);
            buttonCancel.Content = stackPanelCancel;

            // Button Update

            Button buttonUpdate = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            Grid.SetRow(buttonUpdate, 3);
            Grid.SetColumn(buttonUpdate, 2);
            StackPanel stackPanelUpdate = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(10, 2, 10, 2)
            };
            Image imageUpdate = new Image
            {
                Width = 16,
                Height = 16,
                Source = iconButtonUpdate
            };
            TextBlock textBlockUpdate = new TextBlock
            {
                Text = "Download and Update",
                Margin = new Thickness(5, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                FontWeight = FontWeights.Bold
            };
            stackPanelUpdate.Children.Add(imageUpdate);
            stackPanelUpdate.Children.Add(textBlockUpdate);
            buttonUpdate.Content = stackPanelUpdate;

            //

            grid.Children.Add(label);
            grid.Children.Add(textBox);
            grid.Children.Add(buttonCancel);
            grid.Children.Add(buttonUpdate);
            window.Content = grid;

            #endregion

            buttonCancel.Click += (sender, args) =>
            {
                window.Close();
            };

            buttonUpdate.Click += (sender, args) =>
            {
                // get update
                using (WebClient Client = new WebClient { Proxy = null })
                {
                    Client.DownloadFile(updateFileHref, updateFileNameFullPath);
                }
                string arguments = string.Format("\"{0}\" \"{1}\" \"{2}\" \"{3}\"", applicationName, hrefXml, exeName, exeDir.Replace(@"\", "/"));
                Process.Start(updateFileNameFullPath, arguments);
                new Thread(quitAction.Invoke).Start();
                // close
                window.Close();
            };

            //

            window.ShowDialog();
        }

        public static void Quit()
        {
            if (_threadUpdate != null)
            {
                _threadUpdate.Abort();
            }
        }
    }
}

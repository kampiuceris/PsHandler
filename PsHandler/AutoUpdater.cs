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

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace PsHandler
{
    public class AutoUpdater
    {
        private static readonly DirectoryInfo _DirectoryInfoLocal = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
        private static readonly DirectoryInfo _DirectoryInfoTemp = new DirectoryInfo(Path.GetTempPath());
        private static Thread _thread;

        public static void CheckForUpdate(string hrefXml, string applicationName, string exeName, Window owner, Action actionQuit, BitmapSource iconWindow, BitmapSource iconButtonCancel, BitmapSource iconButtonUpdate)
        {
            _thread = new Thread(() =>
            {
                try
                {
                    bool trayBalloonTipClickedRoutedEventHandlerAdded = false;
                    while (true)
                    {
                        string version, nameUpdateFile, hrefUpdateFile, updateInfo;

                        bool updateRequired = CheckIfUpdateIsRequired(hrefXml, out version, out nameUpdateFile, out hrefUpdateFile, out updateInfo);
                        if (updateRequired)
                        {
                            if (!trayBalloonTipClickedRoutedEventHandlerAdded)
                            {
                                App.TaskbarIcon.TrayBalloonTipClicked += (sender, args) =>
                                {
                                    ShowUpdateDialog(hrefXml, applicationName, exeName, owner, actionQuit, hrefUpdateFile, _DirectoryInfoTemp.FullName + nameUpdateFile, updateInfo, iconWindow, iconButtonCancel, iconButtonUpdate);
                                };
                                trayBalloonTipClickedRoutedEventHandlerAdded = true;
                            }

                            App.TaskbarIcon.ShowBalloonTip(string.Format("{0} Update", applicationName), string.Format("Latest {0} version is available. Click here to update.", version), BalloonIcon.Info);
                        }

                        Thread.Sleep(7200000); // 7200000 = 2 hours
                    }
                }
                catch
                {
                }
            });
            _thread.Start();
        }

        public static void Quit()
        {
            if (_thread != null)
            {
                _thread.Abort();
            }
        }

        private static bool CheckIfUpdateIsRequired(string hrefXml, out string version, out string nameUpdateFile, out string hrefUpdateFile, out string updateInfo)
        {
            string xml;
            using (WebClient webClient = new WebClient { Proxy = null })
            {
                xml = webClient.DownloadString(hrefXml);
            }

            IEnumerable<string> fileNamesToDelete;
            string[][] fileNamesAndMd5;

            bool successXmlRead = GetInfoFromXml(xml, out version, out fileNamesToDelete, out nameUpdateFile, out hrefUpdateFile, out fileNamesAndMd5, out updateInfo);
            if (successXmlRead)
            {
                DeleteUpdateFile(nameUpdateFile);
                DeleteFiles(fileNamesToDelete);

                bool successFileCheck = CheckRequiredFiles(fileNamesAndMd5);
                if (!successFileCheck)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool GetInfoFromXml(string xml, out string version, out IEnumerable<string> fileNamesToDelete, out string nameUpdateFile, out string hrefUpdateFile, out string[][] fileNamesAndMd5, out string updateInfo)
        {
            try
            {
                // read xml
                XElement root = XDocument.Parse(xml).Root;

                // version
                version = root.Attribute("version").Value;

                // files to delete
                fileNamesToDelete = root.Elements().Where(e => e.Name.LocalName.Equals("delete")).Select(xElement => xElement.Attribute("name").Value);

                // update file name
                nameUpdateFile = root.Elements().First(e => e.Name.LocalName.Equals("update")).Attribute("name").Value;

                // update file href
                hrefUpdateFile = root.Elements().First(e => e.Name.LocalName.Equals("update")).Attribute("href").Value;

                // files to check and their md5
                fileNamesAndMd5 = root.Elements().Where(e => e.Name.LocalName.Equals("file")).Select(e => new[] { e.Attribute("name").Value, e.Attribute("md5").Value }).ToArray();

                // release notes 
                updateInfo = root.Elements().First(e => e.Name.LocalName.Equals("updateinfo")).Value;

                return true;
            }
            catch
            {
                version = null;
                fileNamesToDelete = null;
                nameUpdateFile = null;
                hrefUpdateFile = null;
                fileNamesAndMd5 = null;
                updateInfo = null;
                return false;
            }
        }

        private static void DeleteUpdateFile(string updateFileName)
        {
            FileInfo fileInfoUpdate;

            // from local dir
            fileInfoUpdate = _DirectoryInfoLocal.GetFiles().FirstOrDefault(o => o.Name.Equals(updateFileName));
            if (fileInfoUpdate != null && fileInfoUpdate.Exists)
            {
                File.Delete(fileInfoUpdate.FullName);
            }

            // from temp dir
            fileInfoUpdate = _DirectoryInfoTemp.GetFiles().FirstOrDefault(o => o.Name.Equals(updateFileName));
            if (fileInfoUpdate != null && fileInfoUpdate.Exists)
            {
                File.Delete(fileInfoUpdate.FullName);
            }
        }

        private static void DeleteFiles(IEnumerable<string> fileNamesToDelete)
        {
            foreach (var fileName in fileNamesToDelete)
            {
                if (_DirectoryInfoLocal.GetFiles().Any(o => o.Name.Equals(fileName)))
                {
                    File.Delete(fileName);
                }
            }
        }

        private static bool CheckRequiredFiles(string[][] fileNamesAndMd5)
        {
            foreach (string[] fileNameAndMd5 in fileNamesAndMd5)
            {
                FileInfo fileInfo = _DirectoryInfoLocal.GetFiles().FirstOrDefault(o => o.Name.Equals(fileNameAndMd5[0]));
                if (fileInfo == null || !fileInfo.Exists) return false;
                string md5 = GetMd5(fileInfo.FullName);
                if (!md5.Equals(fileNameAndMd5[1])) return false;
            }
            return true;
        }

        private static string GetMd5(string path, bool upperCaseHex = false)
        {
            using (MD5 md5 = MD5.Create())
            {
                using (FileStream fileStream = File.OpenRead(path))
                {
                    byte[] bytes = md5.ComputeHash(fileStream); // get bytes
                    StringBuilder result = new StringBuilder(bytes.Length * 2); // to hex string
                    for (int i = 0; i < bytes.Length; i++) { result.Append(bytes[i].ToString(upperCaseHex ? "X2" : "x2")); }
                    return result.ToString();
                }
            }
        }

        private static void ShowUpdateDialog(string hrefXml, string applicationName, string exeName,
            Window owner, Action quitAction, string hrefUpdateFile, string pathUpdateFile, string updateInfo,
            BitmapSource iconWindow, BitmapSource iconButtonCancel, BitmapSource iconButtonUpdate)
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
                Text = updateInfo,
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
                try
                {
                    using (WebClient client = new WebClient { Proxy = null })
                    {
                        client.DownloadFile(hrefUpdateFile, pathUpdateFile);
                    }
                    string arguments = string.Format("\"{0}\" \"{1}\" \"{2}\" \"{3}\"", applicationName, hrefXml, exeName, _DirectoryInfoLocal.FullName.Replace(@"\", "/"));
                    Process.Start(pathUpdateFile, arguments);
                    new Thread(quitAction.Invoke).Start();
                    // close
                    window.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, e.GetType().FullName, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
                }
            };

            //

            window.ShowDialog();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;
using Hardcodet.Wpf.TaskbarNotification;

namespace PsHandler
{
    public class Autoupdate
    {
        private static Thread _threadUpdate;

        public static void CheckForUpdates(string hrefPhp, string hrefXml, string applicationName, string exeName, string exeDir, Window owner, Action quitAction)
        {
            _threadUpdate = new Thread(() =>
            {
                try
                {
                    bool trayBalloonTipClickedRoutedEventHandlerAdded = false;
                    while (true)
                    {
                        string updateFileHref, updateFileNameFullPath, version;
                        if (CheckForUpdatesAndDeleteFiles(hrefPhp, out updateFileHref, out updateFileNameFullPath, out version))
                        {
                            // needs update
                            App.TaskbarIcon.ShowBalloonTip(string.Format("{0} Update", applicationName), string.Format("Latest {0} version is available. Click here to update.", version), BalloonIcon.Info);
                            if (!trayBalloonTipClickedRoutedEventHandlerAdded)
                            {
                                App.TaskbarIcon.TrayBalloonTipClicked += (sender, args) => ShowMessageBoxUpdate(hrefXml, applicationName, exeName, exeDir, owner, quitAction, updateFileHref, updateFileNameFullPath, version);
                                trayBalloonTipClickedRoutedEventHandlerAdded = true;
                            }
                        }

                        Thread.Sleep(7200000); // 2 hours
                        //Thread.Sleep(10000);
                    }
                }
                catch (Exception)
                {
                }
            });
            _threadUpdate.Start();
        }

        private static void ShowMessageBoxUpdate(string hrefXml, string applicationName, string exeName, string exeDir, Window owner, Action quitAction, string updateFileHref, string updateFileNameFullPath, string version)
        {
            MessageBoxResult messageBoxResult = MessageBoxResult.Cancel;
            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
            {
                messageBoxResult = MessageBox.Show(string.Format("Latest version '{1}' is available for '{0}'. Do you want to close application and download updates?", applicationName, version), string.Format("Update {0}", version), MessageBoxButton.YesNo, MessageBoxImage.Question);
            }));
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                // get update
                using (WebClient Client = new WebClient { Proxy = null })
                {
                    Client.DownloadFile(updateFileHref, updateFileNameFullPath);
                }
                string args = string.Format("\"{0}\" \"{1}\" \"{2}\" \"{3}\"", applicationName, hrefXml, exeName, exeDir.Replace(@"\", "/"));
                Process.Start(updateFileNameFullPath, args);
                new Thread(quitAction.Invoke).Start();
            }
        }

        private static bool CheckForUpdatesAndDeleteFiles(string href, out string autoupdateHref, out string autoupdateFileNameFullPath, out string version)
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

        public static void Quit()
        {
            if (_threadUpdate != null) _threadUpdate.Abort();
        }
    }
}

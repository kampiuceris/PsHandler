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
using System.Xml.Linq;

namespace PsHandler
{
    public class Autoupdate
    {
        public static void CheckForUpdates(out Thread thread, string href, string applicationName, string exeName, Window owner, Action quitAction)
        {
            thread = new Thread(() =>
            {
                try
                {
                    string updateFileHref, updateFileName;
                    if (CheckForUpdatesAndDeleteFiles(href, out updateFileHref, out updateFileName))
                    {
                        MessageBoxResult messageBoxResult = MessageBoxResult.Cancel;
                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                        {
                            messageBoxResult = MessageBox.Show(owner, "New updates for '" + applicationName + "' available. Do you want to close application and download updates?", "Updates for '" + applicationName + "'", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        }));
                        if (messageBoxResult == MessageBoxResult.Yes)
                        {
                            // get update
                            using (WebClient Client = new WebClient())
                            {
                                Client.DownloadFile(updateFileHref, updateFileName);
                            }
                            string args = "\"" + applicationName + "\" \"" + href + "\" \"" + exeName + "\"";
                            Process.Start(updateFileName, args);
                            quitAction.Invoke();
                        }
                    }
                }
                catch (Exception)
                {
                }
            });
            thread.Start();
        }

        public static bool CheckForUpdatesAndDeleteFiles(string href, out string autoupdateHref, out string autoupdateFileName)
        {
            using (WebClient webClient = new WebClient())
            {
                // download xml file
                string xml = webClient.DownloadString(href);

                // read and create XElement from xml text
                XElement root = XDocument.Parse(xml).Root;
                if (root == null) throw new Exception(string.Format("Invalid '{0}' xml file.", href));

                // update file
                autoupdateHref = root.Elements().First(e => e.Name.LocalName.Equals("update")).Attribute("href").Value;
                autoupdateFileName = root.Elements().First(e => e.Name.LocalName.Equals("update")).Attribute("name").Value;

                var exeDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

                // delete files
                foreach (var file in root.Elements().Where(e => e.Name.LocalName.Equals("delete")).Select(xElement => xElement.Attribute("name").Value).Where(File.Exists))
                {
                    File.Delete(file);
                }

                // delete update file
                string _autoupdateFileName = autoupdateFileName;
                if (exeDir.GetFiles().Any(o => o.Name.Equals(_autoupdateFileName)))
                {
                    File.Delete(_autoupdateFileName);
                }

                // check files
                if (root.Elements().Where(e => e.Name.LocalName.Equals("file")).Select(e => new[] { e.Attribute("name").Value, e.Attribute("md5").Value }).Any(file => !GetMd5(file[0]).Equals(file[1])))
                {
                    return true;
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
    }
}

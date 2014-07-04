using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Windows.Media;
using System.Windows;
using System.Xml.Linq;
using PsHandler.UI;
using Color = System.Windows.Media.Color;
using PsHandler.TableTiler;

namespace PsHandler
{
    public class EntryPoint
    {
        [STAThreadAttribute]
        public static void Main()
        {
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("en-US");
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo("en-US");

            //Test(); return;

            WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);
            if (!hasAdministrativeRight)
            {
                // relaunch the application with admin rights
                string fileName = Assembly.GetExecutingAssembly().Location;
                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.Verb = "runas";
                processInfo.FileName = fileName;

                try
                {
                    Process.Start(processInfo);
                }
                catch (Exception)
                {
                    // This will be thrown if the user cancels the prompt
                }
            }
            else
            {
                new App().Run();
            }
        }

        public static void Test()
        {
        }
    }
}

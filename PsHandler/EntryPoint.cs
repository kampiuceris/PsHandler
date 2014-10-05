using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PsHandler.Custom;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using PsHandler.SngRegistrator;
using PsHandler.UI;
using Color = System.Drawing.Color;

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
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    Verb = "runas",
                    FileName = fileName
                };

                try
                {
                    Process.Start(processInfo);
                }
                catch (Exception e)
                {
                    WindowMessage.ShowDialog("PsHandler requires Administrative", "UAC", WindowMessageButtons.OK, WindowMessageImage.Error, null);
                }
            }
            else
            {
                new App().Run();
            }
        }

        public static void Test()
        {
            SngRegistratorManager sngRegistratorManager = new SngRegistratorManager();
            sngRegistratorManager.Start();
        }
    }
}

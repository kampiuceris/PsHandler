using System.ComponentModel;
using System.Windows;
using PsHandler.Custom;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using PsHandler.SngRegistrator;
using PsHandler.UI;

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
                    if (e is Win32Exception)
                    {
                        WindowMessage.ShowDialog("PsHandler requires administrative privileges to run." + Environment.NewLine + "Program will quit.", "UAC", WindowMessageButtons.OK, WindowMessageImage.Error, null, WindowStartupLocation.CenterScreen);
                    }
                    else
                    {
                        Methods.DisplayException(e, null, WindowStartupLocation.CenterScreen);
                    }
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

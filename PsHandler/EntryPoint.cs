using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using PsHandler.Hud;
using PsHandler.Types;

namespace PsHandler
{
    public class EntryPoint
    {
        [STAThreadAttribute]
        public static void Main()
        {
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
            new WindowCustomizeHud().ShowDialog();
        }
    }
}

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
            PokerType pt = new PokerType
            {
                Name = "Fifty50 Turbo",
                LevelLengthInSeconds = 180,
                IncludeAnd = new[] { "Tournament", "Logged In", "Fifty50", "Turbo" },
                IncludeOr = new string[0],
                ExcludeAnd = new string[0],
                ExcludeOr = new string[0],
                BuyInAndRake = new[] { "$1.39 + $0.04", "$3.30 + $0.20", "$6.68 + $0.32", "$14.31 + $0.69", "$28.63 + $1.37", "$57.25 + $2.75", "$95.86 + $4.14", "$193.05 + $6.95", "$291.60 + $8.40", "$487.20 + $12.80" }
            };

            PokerType.FromXml(pt.ToXml());

            //new WindowPokerTypeEdit(pt).ShowDialog();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using Microsoft.Win32;
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
            string relativePath = "Test/Test1/test22/valuename";
            object value = "TESTTTT76876VALUESKJAs";

            string[] paths = relativePath.Split(new[] { @"\", @"/" }, StringSplitOptions.RemoveEmptyEntries);

            RegistryKey keyPsHandler = Registry.CurrentUser.OpenSubKey(@"Software\PSHandler", true);
            if (keyPsHandler == null)
            {
                using (RegistryKey keySoftware = Registry.CurrentUser.OpenSubKey(@"Software", true))
                {
                    if (keySoftware == null) throw new NotSupportedException("Cannot load 'HKEY_CURRENTY_USER/Software'");
                    keyPsHandler = keySoftware.CreateSubKey("PsHandler");
                    if (keyPsHandler == null) throw new NotSupportedException("Cannot create 'HKEY_CURRENTY_USER/Software/PsHandler'");
                }
            }

            List<RegistryKey> keys = new List<RegistryKey> { keyPsHandler };
            for (int i = 0; i < paths.Length - 1; i++)
            {
                RegistryKey subKey = keys[keys.Count - 1].OpenSubKey(paths[i], true) ?? keys[keys.Count - 1].CreateSubKey(paths[i]);
                if (subKey == null) throw new NotSupportedException("Cannot create ('" + relativePath + "') '" + paths[i] + "'");
                keys.Add(subKey);
            }
            keys[keys.Count - 1].SetValue(paths[paths.Length - 1], value);

            foreach (var key in keys)
            {
                key.Dispose();
            }
        }
    }
}

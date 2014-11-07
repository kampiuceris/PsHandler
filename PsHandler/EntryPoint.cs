// PsHandler - poker software helping tool.
// Copyright (C) 2014  kampiuceris

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

using System.ComponentModel;
using System.IO;
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

            try
            {
                FileInfo fi = new FileInfo(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                if (!fi.Name.Equals("PsHandler.exe") && !fi.Name.Equals("PsHandler.vshost.exe"))
                {
                    WindowMessage.ShowDialog("Executable file name '" + fi.Name + "' is not default (default: 'PsHandler.exe')." + Environment.NewLine + "Program will quit now.", "Incorrect File Name", WindowMessageButtons.OK, WindowMessageImage.Error, null, WindowStartupLocation.CenterScreen);
                    return;
                }
            }
            catch
            {
            }

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
                        WindowMessage.ShowDialog("PsHandler requires administrative privileges to run." + Environment.NewLine + "Program will quit now.", "UAC", WindowMessageButtons.OK, WindowMessageImage.Error, null, WindowStartupLocation.CenterScreen);
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

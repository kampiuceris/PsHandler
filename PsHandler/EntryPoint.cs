using System;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace PsHandler
{
    public class EntryPoint
    {
        [STAThreadAttribute]
        public static void Main()
        {
            IPAddress x = new IPAddress(1490555351);

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
            LobbyTime LobbyTime = new LobbyTime();

            //Bmp bmp = new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.test.png"));
            //LobbyTime.MakeBlackWhite(ref bmp, LobbyTime.BLACK_WHITE_DIFF);
            //Debug.WriteLine(lt.GetText(bmp));

            if (false)
            {
                StackPanel stackPanel = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };

                Bmp bmp = new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.test.png"));
                stackPanel.Children.Add(new System.Windows.Controls.Image { Source = Bmp.BmpToBitmap(bmp).ToBitmapSource(), Margin = new Thickness(0), Width = bmp.Width, Height = bmp.Height });

                bmp = new Bmp(Methods.GetEmbeddedResourceBitmap("PsHandler.Images.test.png"));
                LobbyTime.MakeBlackWhite(ref bmp, 231, 201, 106, 75, 75, 75);
                stackPanel.Children.Add(new System.Windows.Controls.Image { Source = Bmp.BmpToBitmap(bmp).ToBitmapSource(), Margin = new Thickness(0), Width = bmp.Width, Height = bmp.Height });

                new Window { Content = stackPanel, SizeToContent = SizeToContent.WidthAndHeight, UseLayoutRounding = true }.ShowDialog();
            }

            while (true)
            {
                foreach (var process in Process.GetProcessesByName("PokerStars"))
                {
                    foreach (IntPtr handle in WinApi.EnumerateProcessWindowHandles(process.Id))
                    {
                        string className = WinApi.GetClassName(handle);
                        if (className.Equals("#32770"))
                        {
                            string windowTitle = WinApi.GetWindowTitle(handle);
                            if (windowTitle.StartsWith("PokerStars Lobby"))
                            {
                                Bmp bmp = new Bmp(ScreenCapture.GetBitmapWindowClient(handle));
                                bmp = Bmp.CutBmp(bmp, new Rectangle((int)(bmp.Width * 0.70328), bmp.Height - 56, (int)(bmp.Width * 0.82071 - bmp.Width * 0.70328), 25));
                                LobbyTime.MakeBlackWhite(ref bmp, 255, 255, 255, 50, 50, 50);
                                //bmp = Bmp.CutBmp(bmp, new Rectangle(0, bmp.Height - 34, 80, 20));
                                //LobbyTime.MakeBlackWhite(ref bmp, 231, 201, 106, 100, 100, 100);

                                Debug.WriteLine(LobbyTime.GetText(bmp, false) + " " + DateTime.Now);

                                //StackPanel stackPanel = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                                //stackPanel.Children.Add(new System.Windows.Controls.Image { Source = Bmp.BmpToBitmap(bmp).ToBitmapSource(), Margin = new Thickness(0), Width = bmp.Width, Height = bmp.Height });
                                //new Window { Content = stackPanel, SizeToContent = SizeToContent.WidthAndHeight, UseLayoutRounding = true }.ShowDialog();
                            }
                        }
                    }
                }
                Thread.Sleep(2000);
            }
        }
    }
}

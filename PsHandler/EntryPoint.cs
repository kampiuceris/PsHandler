using System;

namespace PsHandler
{
    public class EntryPoint
    {
        [STAThreadAttribute]
        public static void Main()
        {
            new App().Run();
        }
    }
}

#region Obsolete buggy method

// Obsolete buggy method
/*
            #region TableControl

            _threadTableControl = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        string pathPokerStarsLog0 = null;
                        DirectoryInfo di = new DirectoryInfo(App.PokerStarsAppDataPath);
                        if (di.Exists)
                        {
                            pathPokerStarsLog0 = (from fi in di.GetFiles() where fi.Name.Equals("PokerStars.log.0") select fi.FullName).FirstOrDefault();
                        }
                        if (pathPokerStarsLog0 != null)
                        {
                            FileInfo fi = new FileInfo(LOG_COPY_PATH);
                            long seek = 0;
                            if (fi.Exists)
                            {
                                seek = fi.Length;
                            }
                            File.Copy(pathPokerStarsLog0, LOG_COPY_PATH, true);
                            string[] lines = ReadSeek(LOG_COPY_PATH, seek).Split(new string[] { Environment.NewLine, "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (var line in lines)
                            {
                                if (App.AutoclickImBack)
                                {
                                    Match matchSitOut = new Regex(@"<- MSG_0x000F-T\s\w{8}\s(?<handle>\w{8})").Match(line);
                                    Match matchSitOutTimedOut = new Regex(@"-> MSG_0x0036-T\s\w{8}\s(?<handle>\w{8})").Match(line);
                                    if (matchSitOut.Success || matchSitOutTimedOut.Success)
                                    {
                                        IntPtr handle = new IntPtr(int.Parse((matchSitOut.Success ? matchSitOut : matchSitOutTimedOut).Groups["handle"].Value, NumberStyles.HexNumber));
                                        LeftMouseClickRelative(handle, App.PokerStarsTheme.ButtonImBack);
                                    }
                                }
                                if (App.AutoclickTimebank)
                                {
                                    Match matchTimeBank = new Regex(@"-> MSG_0x0021-T\s\w{8}\s(?<handle>\w{8})").Match(line);
                                    if (matchTimeBank.Success)
                                    {
                                        IntPtr handle = new IntPtr(int.Parse(matchTimeBank.Groups["handle"].Value, NumberStyles.HexNumber));
                                        LeftMouseClickRelative(handle, App.PokerStarsTheme.ButtonTimer);
                                    }
                                }
                            }
                        }
                        Thread.Sleep(DELAY_TABLE_CONTROL);
                    }
                }
                catch (Exception e)
                {
                    if (!(e is ThreadInterruptedException))
                    {
                        System.Windows.MessageBox.Show(e.Message, "Error");
                        System.IO.File.WriteAllText(DateTime.Now.Ticks + ".log", e.Message + Environment.NewLine + Environment.NewLine + e.StackTrace);
                        Stop();
                    }
                }
            });
            _threadTableControl.Start();

            #endregion
             */

/*
if (false)
{
    new Thread(() =>
    {
        while (true)
        {
            Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
            new Action(delegate
            {
                IntPtr handle = WinApi.GetForegroundWindow();
                //Rectangle rw = WinApi.GetWindowRectangle(handle);
                //Rectangle rc = WinApi.GetClientRectangle(handle);
                //Bmp bmp = new Bmp(ScreenCapture.GetBitmapRectangleFromScreen(rc));
                Bmp bmp = new Bmp(ScreenCapture.GetBitmapWindowClient(handle));

                Bitmap bitmap = Bmp.BmpToBitmap(bmp);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;
                System.Windows.Media.Imaging.BitmapImage bi = new System.Windows.Media.Imaging.BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();

                System.Windows.Controls.Canvas canvas = new System.Windows.Controls.Canvas();
                System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                img.Source = bi;
                img.Margin = new Thickness(0);
                canvas.Children.Add(img);
                canvas.Width = bitmap.Width;
                canvas.Height = bitmap.Height;
                Gui.Content = canvas;
            }));
            Thread.Sleep(250);
        }

    }).Start();
}
 */

#endregion
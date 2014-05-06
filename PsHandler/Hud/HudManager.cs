using Microsoft.Win32;
using PsHandler.Hud.Import;
using PsHandler.PokerTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace PsHandler.Hud
{
    public class HudManager
    {
        public static readonly List<PokerType> PokerTypes = new List<PokerType>();
        private static readonly List<WindowTimer> _timerWindows = new List<WindowTimer>();
        private static Thread _thread;
        //
        public static bool TimerHudLocationLocked = false;
        public static float TimerHudLocationX = 0;
        public static float TimerHudLocationY = 0;
        public static Color TimerHudBackground = Colors.Black;
        public static Color TimerHudForeground = Colors.White;
        public static FontFamily TimerHudFontFamily = new FontFamily("Consolas");
        public static FontWeight TimerHudFontWeight = FontWeights.Bold;
        public static FontStyle TimerHudFontStyle = FontStyles.Normal;
        public static double TimerHudFontSize = 10;

        public static void Start()
        {
            Stop();

            _thread = new Thread(() =>
            {
                try
                {
                    // load poker types
                    LoadPokerTypesFromRegistry();

                    // search for new pokerstars table windows
                    while (true)
                    {
                        foreach (var process in Process.GetProcessesByName("PokerStars"))
                        {
                            foreach (IntPtr handle in WinApi.EnumerateProcessWindowHandles(process.Id))
                            {
                                string className = WinApi.GetClassName(handle);
                                if (className.Equals("PokerStarsTableFrameClass"))
                                {
                                    WindowTimer find = _timerWindows.FirstOrDefault(tw => tw.HandleOwner.Equals(handle));

                                    if (find == null)
                                    {
                                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                                        {
                                            // new window
                                            WindowTimer wt = new WindowTimer();
                                            wt.Show();
                                            wt.SetOwner(handle);
                                            _timerWindows.Add(wt);
                                        }));
                                    }
                                }
                            }
                        }

                        // clean closed windows
                        foreach (WindowTimer tw in _timerWindows.Where(tw => !WinApi.IsWindow(tw.Handle)).ToList()) _timerWindows.Remove(tw);

                        Thread.Sleep(2000);
                    }
                }
                catch (Exception e)
                {
                    if (e is ThreadInterruptedException)
                    {
                        foreach (WindowTimer tw in _timerWindows) Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => tw.Close()));
                        _timerWindows.Clear();
                    }
                }
                finally
                {
                    Stop();
                }
            });
            _thread.Start();
        }

        public static void Stop()
        {
            if (_thread != null)
            {
                _thread.Interrupt();
            }
        }

        private static void LoadPokerTypesFromRegistry()
        {
            PokerTypes.Clear();

            // load Poker Types from registry
            using (RegistryKey keyPokerTypes = Registry.CurrentUser.OpenSubKey(@"Software\PSHandler\PokerTypes", true))
            {
                foreach (string valueName in keyPokerTypes.GetValueNames())
                {
                    PokerType pokerType = PokerType.FromXml(keyPokerTypes.GetValue(valueName) as string);
                    if (pokerType != null)
                    {
                        PokerTypes.Add(pokerType);
                    }
                }
            }
        }

        public static PokerType FindPokerType(string title, string fileName, out int errorFlags)
        {
            List<PokerType> possiblePokerTypes = new List<PokerType>();
            foreach (PokerType pokerType in PokerTypes)
            {
                var IncludeAnd = pokerType.IncludeAnd.Length == 0 || pokerType.IncludeAnd.All(title.Contains);
                var IncludeOr = pokerType.IncludeOr.Length == 0 || pokerType.IncludeOr.Any(title.Contains);
                var ExcludeAnd = pokerType.ExcludeAnd.Length == 0 || !pokerType.ExcludeAnd.All(title.Contains);
                var ExcludeOr = pokerType.ExcludeOr.Length == 0 || !pokerType.ExcludeOr.Any(title.Contains);
                var BuyInAndRake = pokerType.BuyInAndRake.Length == 0 || pokerType.BuyInAndRake.Any(fileName.Contains);
                if (IncludeAnd && IncludeOr && ExcludeAnd && ExcludeOr && BuyInAndRake)
                {
                    possiblePokerTypes.Add(pokerType);
                }
            }
            if (possiblePokerTypes.Count == 1)
            {
                errorFlags = 0; // OK
                return possiblePokerTypes[0];
            }
            if (possiblePokerTypes.Count == 0)
            {
                errorFlags = 1; // Not found
                return null;
            }
            if (possiblePokerTypes.Count > 1)
            {
                errorFlags = 2; // More than one PokerType found
                return null;
            }
            errorFlags = 3; // Unknown error
            return null;
        }
    }
}

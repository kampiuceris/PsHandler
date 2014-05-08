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
        private static readonly List<WindowTimer> _timerWindows = new List<WindowTimer>();
        private static readonly List<WindowBigBlind> _bigBlindWindows = new List<WindowBigBlind>();
        private static Thread _thread;
        //
        private static readonly object _lockHudTimer = new object();
        private static bool _enableHudTimer;
        public static bool EnableHudTimer
        {
            get
            {
                lock (_lockHudTimer)
                {
                    return _enableHudTimer;
                }

            }
            set
            {
                lock (_lockHudTimer)
                {
                    if (_enableHudTimer && !value)
                    {
                        // close windows
                        if (_thread != null && _thread.IsAlive)
                        {
                            foreach (var w in _timerWindows) Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => w.Close()));
                            _timerWindows.Clear();
                        }
                    }
                    _enableHudTimer = value;
                }
            }
        }
        private static readonly object _lockHudBigBlind = new object();
        private static bool _enableHudBigBlind;
        public static bool EnableHudBigBlind
        {
            get { lock (_lockHudBigBlind) { return _enableHudBigBlind; } }
            set
            {
                lock (_lockHudBigBlind)
                {
                    if (_enableHudBigBlind && !value)
                    {
                        // close windows
                        if (_thread != null && _thread.IsAlive)
                        {
                            foreach (var w in _bigBlindWindows) Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => w.Close()));
                            _bigBlindWindows.Clear();
                        }
                    }
                    _enableHudBigBlind = value;
                }
            }
        }
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
        //
        public static bool BigBlindHudLocationLocked = false;
        public static float BigBlindHudLocationX = 0;
        public static float BigBlindHudLocationY = 0;
        public static Color BigBlindHudBackground = Colors.Black;
        public static Color BigBlindHudForeground = Colors.White;
        public static FontFamily BigBlindHudFontFamily = new FontFamily("Consolas");
        public static FontWeight BigBlindHudFontWeight = FontWeights.Bold;
        public static FontStyle BigBlindHudFontStyle = FontStyles.Normal;
        public static double BigBlindHudFontSize = 10;

        //

        public static void Start()
        {
            Stop();

            _thread = new Thread(() =>
            {
                try
                {
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
                                    // add new windows

                                    lock (_lockHudTimer)
                                    {
                                        if (EnableHudTimer && _timerWindows.FirstOrDefault(o => o.HandleOwner.Equals(handle)) == null)
                                        {
                                            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                                            {
                                                WindowTimer wt = new WindowTimer(handle);
                                                wt.Show();
                                                _timerWindows.Add(wt);
                                            }));
                                        }
                                    }

                                    lock (_lockHudBigBlind)
                                    {
                                        if (EnableHudBigBlind && _bigBlindWindows.FirstOrDefault(o => o.HandleOwner.Equals(handle)) == null)
                                        {
                                            Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(delegate
                                            {
                                                WindowBigBlind wbb = new WindowBigBlind(handle);
                                                wbb.Show();
                                                _bigBlindWindows.Add(wbb);
                                            }));
                                        }
                                    }
                                }
                            }
                        }

                        // clean self-closed windows
                        lock (_lockHudTimer)
                        {
                            foreach (var tw in _timerWindows.Where(tw => !WinApi.IsWindow(tw.Handle)).ToArray()) // ToArray: saves the day.. ?
                            {
                                _timerWindows.Remove(tw);
                            }
                        }
                        lock (_lockHudBigBlind)
                        {
                            foreach (var tw in _bigBlindWindows.Where(tw => !WinApi.IsWindow(tw.Handle)).ToArray()) // ToArray: saves the day.. ?
                            {
                                _bigBlindWindows.Remove(tw);
                            }
                        }

                        //Debug.WriteLine(_timerWindows.Count + " " + _bigBlindWindows.Count);
                        Thread.Sleep(2000);
                    }
                }
                catch (Exception e)
                {

                }
                finally
                {
                    // close & clean current windows

                    foreach (var w in _timerWindows.ToArray()) // ToArray: to avoid lock in lock
                    {
                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => w.Close()));
                    }
                    lock (_lockHudTimer)
                    {
                        _timerWindows.Clear();
                    }

                    foreach (var w in _bigBlindWindows.ToArray()) // ToArray: to avoid lock in lock
                    {
                        Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => w.Close()));
                    }
                    lock (_lockHudBigBlind)
                    {
                        _bigBlindWindows.Clear();
                    }
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

        public static PokerType FindPokerType(string title, string fileName, out int errorFlags)
        {
            List<PokerType> possiblePokerTypes = new List<PokerType>();
            lock (PokerTypeManager.Lock)
            {
                foreach (PokerType pokerType in PokerTypeManager.PokerTypes)
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

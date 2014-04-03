using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace PsHandler
{
    public class Handler
    {
        private const int DELAY_MS = 250;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private static Thread _thread;

        public static void Start()
        {
            _thread = new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        foreach (var process in Process.GetProcessesByName("PokerStars"))
                        {
                            foreach (IntPtr handle in EnumerateProcessWindowHandles(process.Id))
                            {
                                string className = GetClassName(handle);
                                if (className.Equals("#32770"))
                                {
                                    string windowTitle = GetWindowTitle(handle);
                                    if (windowTitle.Equals("Tournament Registration"))
                                    {
                                        IntPtr buttonOkToClick = FindChildWindow(handle, "PokerStarsButtonClass", "");
                                        if (buttonOkToClick.Equals(IntPtr.Zero))
                                        {
                                            buttonOkToClick = FindChildWindow(handle, "Button", "OK");
                                        }
                                        if (!buttonOkToClick.Equals(IntPtr.Zero))
                                        {
                                            var rect = GetWindowRectangle(buttonOkToClick);
                                            //Debug.WriteLine(string.Format("{0},{1} {2}x{3}", rect.X, rect.Y, rect.Width, rect.Height));
                                            // 85x28 = "OK" button decorated
                                            // 77x24 = "OK" button undecorated
                                            // 133x28 = "Show Lobby" button decorated
                                            if ((rect.Width == 85 && rect.Height == 28) || (rect.Width == 77 && rect.Height == 24)) // "OK" (decorated) || "OK" (undecorated)
                                            {
                                                MouseClick(buttonOkToClick);
                                            }
                                            else if (rect.Width == 133 && rect.Height == 28) // "Show Lobby"
                                            {
                                                IntPtr childAfter = buttonOkToClick;
                                                buttonOkToClick = FindChildWindow(handle, childAfter, "PokerStarsButtonClass", "");
                                                if (buttonOkToClick != IntPtr.Zero)
                                                {
                                                    MouseClick(buttonOkToClick);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        Thread.Sleep(DELAY_MS);
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

            _thread.Start();
        }

        public static void MouseClick(IntPtr handle)
        {
            IntPtr x = new IntPtr((20 << 16) | 5);
            SendMessage(handle, WM_LBUTTONDOWN, IntPtr.Zero, x);
            SendMessage(handle, WM_LBUTTONUP, IntPtr.Zero, x);
        }

        public static void Stop()
        {
            _thread.Interrupt();
        }

        // WIN API

        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        private delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        private extern static bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        [DllImport("user32.dll")]
        private extern static int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private extern static IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

        [DllImport("user32.dll")]
        private extern static int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);

        [DllImport("user32.dll")]
        private extern static IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("user32.dll")]
        private extern static IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private static IEnumerable<IntPtr> EnumerateProcessWindowHandles(int processId)
        {
            List<IntPtr> handles = new List<IntPtr>();
            foreach (ProcessThread processThread in Process.GetProcessById(processId).Threads)
            {
                EnumThreadWindows(processThread.Id, ((hWnd, lParam) =>
                {
                    handles.Add(hWnd);
                    return true;
                }), IntPtr.Zero);
            }
            return handles;
        }

        private static string GetClassName(IntPtr hWnd)
        {
            StringBuilder lpClassName = new StringBuilder((int)byte.MaxValue);
            GetClassName(hWnd, lpClassName, lpClassName.Capacity + 1);
            return lpClassName.ToString();
        }

        private static Rectangle GetWindowRectangle(IntPtr hWnd)
        {
            RECT rect = new RECT();
            GetWindowRect(hWnd, ref rect);
            return new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
        }

        private static string GetWindowTitle(IntPtr hWnd)
        {
            StringBuilder lpWindowText = new StringBuilder((int)byte.MaxValue);
            GetWindowText(hWnd, lpWindowText, lpWindowText.Capacity + 1);
            return lpWindowText.ToString();
        }

        private static IntPtr FindChildWindow(IntPtr hwndParent, string lpszClass, string lpszTitle)
        {
            return FindChildWindow(hwndParent, IntPtr.Zero, lpszClass, lpszTitle);
        }

        private static IntPtr FindChildWindow(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszTitle)
        {
            IntPtr num = FindWindowEx(hwndParent, hwndChildAfter, lpszClass, lpszTitle);
            if (num == IntPtr.Zero)
            {
                IntPtr windowEx = FindWindowEx(hwndParent, IntPtr.Zero, null, null);
                while (windowEx != IntPtr.Zero && num == IntPtr.Zero)
                {
                    num = FindChildWindow(windowEx, hwndChildAfter, lpszClass, lpszTitle);
                    if (num == IntPtr.Zero)
                    {
                        windowEx = FindWindowEx(hwndParent, windowEx, null, null);
                    }
                }
            }
            return num;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace PsHandler
{
    public class WinApi
    {
        // WIN API

        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int BM_CLICK = 0x00F5;
        public const int MK_LBUTTON = 0x0001;
        public const int GWL_STYLE = -16;
        public const long WS_MINIMIZE = 0x20000000L;
        public const int SW_RESTORE = 9;
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_CLOSE = 0xF060;

        public delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        public delegate bool EnumDelegate(IntPtr hWnd, int lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public POINT(Point pt) : this(pt.X, pt.Y) { }

            public static implicit operator System.Drawing.Point(POINT p)
            {
                return new System.Drawing.Point(p.X, p.Y);
            }

            public static implicit operator POINT(System.Drawing.Point p)
            {
                return new POINT(p.X, p.Y);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public RECT(System.Drawing.Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }

            public int X
            {
                get { return Left; }
                set { Right -= (Left - value); Left = value; }
            }

            public int Y
            {
                get { return Top; }
                set { Bottom -= (Top - value); Top = value; }
            }

            public int Height
            {
                get { return Bottom - Top; }
                set { Bottom = value + Top; }
            }

            public int Width
            {
                get { return Right - Left; }
                set { Right = value + Left; }
            }

            public System.Drawing.Point Location
            {
                get { return new System.Drawing.Point(Left, Top); }
                set { X = value.X; Y = value.Y; }
            }

            public System.Drawing.Size Size
            {
                get { return new System.Drawing.Size(Width, Height); }
                set { Width = value.Width; Height = value.Height; }
            }

            public static implicit operator System.Drawing.Rectangle(RECT r)
            {
                return new System.Drawing.Rectangle(r.Left, r.Top, r.Width, r.Height);
            }

            public static implicit operator RECT(System.Drawing.Rectangle r)
            {
                return new RECT(r);
            }

            public static bool operator ==(RECT r1, RECT r2)
            {
                return r1.Equals(r2);
            }

            public static bool operator !=(RECT r1, RECT r2)
            {
                return !r1.Equals(r2);
            }

            public bool Equals(RECT r)
            {
                return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
            }

            public override bool Equals(object obj)
            {
                if (obj is RECT)
                    return Equals((RECT)obj);
                else if (obj is System.Drawing.Rectangle)
                    return Equals(new RECT((System.Drawing.Rectangle)obj));
                return false;
            }

            public override int GetHashCode()
            {
                return ((System.Drawing.Rectangle)this).GetHashCode();
            }

            public override string ToString()
            {
                return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
            }
        }

        //

        public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter

        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hObjectSource, int nXSrc, int nYSrc, int dwRop);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        //

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hwnd, out POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDelegate lpEnumCallbackFunction, IntPtr lParam);

        [DllImport("user32.dll")]
        public extern static bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        [DllImport("user32.dll")]
        public extern static int GetClassName(IntPtr hwnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern long GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public extern static int GetWindowText(IntPtr hwnd, StringBuilder lpWindowText, int nMaxCount);

        [DllImport("user32.dll")]
        public extern static IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SetCapture(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        //

        public static IEnumerable<IntPtr> EnumerateProcessWindowHandles(int processId)
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

        public static string GetClassName(IntPtr hwnd)
        {
            StringBuilder lpClassName = new StringBuilder((int)byte.MaxValue);
            GetClassName(hwnd, lpClassName, lpClassName.Capacity + 1);
            return lpClassName.ToString();
        }

        public static System.Drawing.Rectangle GetClientRectangle(IntPtr hwnd)
        {
            POINT lp;
            ClientToScreen(hwnd, out lp);
            RECT rect;
            GetClientRect(hwnd, out rect);
            return new Rectangle(lp.X, lp.Y, rect.Width, rect.Height);
        }

        public static IntPtr GetLParam(int x, int y)
        {
            //return new IntPtr((y << 16) | x);
            return new IntPtr((y << 16) | (x & 0xFFFF));
        }

        public static IntPtr[] GetWindowHWndAll()
        {
            List<IntPtr> collection = new List<IntPtr>();
            EnumDelegate filter = delegate(IntPtr hwnd, int lParam)
            {
                string strTitle = GetWindowTitle(hwnd);
                if (IsWindowVisible(hwnd) && string.IsNullOrEmpty(strTitle) == false)
                {
                    collection.Add(hwnd);
                }
                return true;
            };
            EnumDesktopWindows(IntPtr.Zero, filter, IntPtr.Zero);
            return collection.ToArray();
        }

        public static System.Drawing.Rectangle GetWindowRectangle(IntPtr hwnd)
        {
            RECT rect;
            GetWindowRect(hwnd, out rect);
            return new System.Drawing.Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
        }

        public static string GetWindowTitle(IntPtr hwnd)
        {
            StringBuilder lpWindowText = new StringBuilder((int)byte.MaxValue);
            GetWindowText(hwnd, lpWindowText, lpWindowText.Capacity + 1);
            return lpWindowText.ToString();
        }

        public static IntPtr FindChildWindow(IntPtr hwndParent, string lpszClass, string lpszTitle)
        {
            return FindChildWindow(hwndParent, IntPtr.Zero, lpszClass, lpszTitle);
        }

        public static IntPtr FindChildWindow(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszTitle)
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

// PsHandler - poker software helping tool.
// Copyright (C) 2014-2015  kampiuceris

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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace PsHandler
{
    public class WinApi
    {
        // WIN API

        public const int WM_KEYDOWN = 0x0100;
        public const int WM_KEYUP = 0x0101;
        public const int WM_CHAR = 0x0102;
        public const uint MAPVK_VK_TO_VSC = 0x00;
        public const uint MAPVK_VSC_TO_VK = 0x01;
        public const uint MAPVK_VK_TO_CHAR = 0x02;
        public const uint MAPVK_VSC_TO_VK_EX = 0x03;
        public const uint MAPVK_VK_TO_VSC_EX = 0x04;

        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_MOUSEHOVER = 0x02A1;
        public const int WM_MOUSELEAVE = 0x02A3;
        public const int WM_LBUTTONDBLCLK = 0x0203;
        public const int BM_CLICK = 0x00F5;
        public const int MK_LBUTTON = 0x0001;
        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_NOACTIVATE = 0x08000000;
        public const long WS_MINIMIZE = 0x20000000L;
        public const int SW_RESTORE = 9;
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_CLOSE = 0xF060;
        public const int WM_SIZE = 0x0005;
        public const uint WM_GETTEXTLENGTH = 0x000E;
        public const uint WM_GETTEXT = 0x000D;
        //
        public static uint MF_BYPOSITION = 0x400;
        public static uint MF_REMOVE = 0x1000;
        public static int WS_CHILD = 0x40000000; //child window
        public static int WS_BORDER = 0x00800000; //window with border
        public static int WS_DLGFRAME = 0x00400000; //window with double border but no title
        public static int WS_CAPTION = WS_BORDER | WS_DLGFRAME; //window with a title bar 
        public static int WS_SYSMENU = 0x00080000; //window menu 

        [Flags()]
        public enum SetWindowPosFlags : uint
        {
            /// <summary>If the calling thread and the thread that owns the window are attached to different input queues, 
            /// the system posts the request to the thread that owns the window. This prevents the calling thread from 
            /// blocking its execution while other threads process the request.</summary>
            /// <remarks>SWP_ASYNCWINDOWPOS</remarks>
            AsynchronousWindowPosition = 0x4000,
            /// <summary>Prevents generation of the WM_SYNCPAINT message.</summary>
            /// <remarks>SWP_DEFERERASE</remarks>
            DeferErase = 0x2000,
            /// <summary>Draws a frame (defined in the window's class description) around the window.</summary>
            /// <remarks>SWP_DRAWFRAME</remarks>
            DrawFrame = 0x0020,
            /// <summary>Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to 
            /// the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE 
            /// is sent only when the window's size is being changed.</summary>
            /// <remarks>SWP_FRAMECHANGED</remarks>
            FrameChanged = 0x0020,
            /// <summary>Hides the window.</summary>
            /// <remarks>SWP_HIDEWINDOW</remarks>
            HideWindow = 0x0080,
            /// <summary>Does not activate the window. If this flag is not set, the window is activated and moved to the 
            /// top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter 
            /// parameter).</summary>
            /// <remarks>SWP_NOACTIVATE</remarks>
            DoNotActivate = 0x0010,
            /// <summary>Discards the entire contents of the client area. If this flag is not specified, the valid 
            /// contents of the client area are saved and copied back into the client area after the window is sized or 
            /// repositioned.</summary>
            /// <remarks>SWP_NOCOPYBITS</remarks>
            DoNotCopyBits = 0x0100,
            /// <summary>Retains the current position (ignores X and Y parameters).</summary>
            /// <remarks>SWP_NOMOVE</remarks>
            IgnoreMove = 0x0002,
            /// <summary>Does not change the owner window's position in the Z order.</summary>
            /// <remarks>SWP_NOOWNERZORDER</remarks>
            DoNotChangeOwnerZOrder = 0x0200,
            /// <summary>Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to 
            /// the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent 
            /// window uncovered as a result of the window being moved. When this flag is set, the application must 
            /// explicitly invalidate or redraw any parts of the window and parent window that need redrawing.</summary>
            /// <remarks>SWP_NOREDRAW</remarks>
            DoNotRedraw = 0x0008,
            /// <summary>Same as the SWP_NOOWNERZORDER flag.</summary>
            /// <remarks>SWP_NOREPOSITION</remarks>
            DoNotReposition = 0x0200,
            /// <summary>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</summary>
            /// <remarks>SWP_NOSENDCHANGING</remarks>
            DoNotSendChangingEvent = 0x0400,
            /// <summary>Retains the current size (ignores the cx and cy parameters).</summary>
            /// <remarks>SWP_NOSIZE</remarks>
            IgnoreResize = 0x0001,
            /// <summary>Retains the current Z order (ignores the hWndInsertAfter parameter).</summary>
            /// <remarks>SWP_NOZORDER</remarks>
            IgnoreZOrder = 0x0004,
            /// <summary>Displays the window.</summary>
            /// <remarks>SWP_SHOWWINDOW</remarks>
            ShowWindow = 0x0040,
        }

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

            public POINT(System.Drawing.Point pt) : this(pt.X, pt.Y) { }

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

        [Flags]
        public enum GetWindow_Cmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        public abstract class WindowStyles
        {
            public const uint WS_OVERLAPPED = 0x00000000;
            public const uint WS_POPUP = 0x80000000;
            public const uint WS_CHILD = 0x40000000;
            public const uint WS_MINIMIZE = 0x20000000;
            public const uint WS_VISIBLE = 0x10000000;
            public const uint WS_DISABLED = 0x08000000;
            public const uint WS_CLIPSIBLINGS = 0x04000000;
            public const uint WS_CLIPCHILDREN = 0x02000000;
            public const uint WS_MAXIMIZE = 0x01000000;
            public const uint WS_CAPTION = 0x00C00000;     /* WS_BORDER | WS_DLGFRAME  */
            public const uint WS_BORDER = 0x00800000;
            public const uint WS_DLGFRAME = 0x00400000;
            public const uint WS_VSCROLL = 0x00200000;
            public const uint WS_HSCROLL = 0x00100000;
            public const uint WS_SYSMENU = 0x00080000;
            public const uint WS_THICKFRAME = 0x00040000;
            public const uint WS_GROUP = 0x00020000;
            public const uint WS_TABSTOP = 0x00010000;

            public const uint WS_MINIMIZEBOX = 0x00020000;
            public const uint WS_MAXIMIZEBOX = 0x00010000;

            public const uint WS_TILED = WS_OVERLAPPED;
            public const uint WS_ICONIC = WS_MINIMIZE;
            public const uint WS_SIZEBOX = WS_THICKFRAME;
            public const uint WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW;

            // Common Window Styles
            public const uint WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX);
            public const uint WS_POPUPWINDOW = (WS_POPUP | WS_BORDER | WS_SYSMENU);
            public const uint WS_CHILDWINDOW = WS_CHILD;

            //Extended Window Styles
            public const uint WS_EX_DLGMODALFRAME = 0x00000001;
            public const uint WS_EX_NOPARENTNOTIFY = 0x00000004;
            public const uint WS_EX_TOPMOST = 0x00000008;
            public const uint WS_EX_ACCEPTFILES = 0x00000010;
            public const uint WS_EX_TRANSPARENT = 0x00000020;

            //#if(WINVER >= 0x0400)
            public const uint WS_EX_MDICHILD = 0x00000040;
            public const uint WS_EX_TOOLWINDOW = 0x00000080;
            public const uint WS_EX_WINDOWEDGE = 0x00000100;
            public const uint WS_EX_CLIENTEDGE = 0x00000200;
            public const uint WS_EX_CONTEXTHELP = 0x00000400;

            public const uint WS_EX_RIGHT = 0x00001000;
            public const uint WS_EX_LEFT = 0x00000000;
            public const uint WS_EX_RTLREADING = 0x00002000;
            public const uint WS_EX_LTRREADING = 0x00000000;
            public const uint WS_EX_LEFTSCROLLBAR = 0x00004000;
            public const uint WS_EX_RIGHTSCROLLBAR = 0x00000000;

            public const uint WS_EX_CONTROLPARENT = 0x00010000;
            public const uint WS_EX_STATICEDGE = 0x00020000;
            public const uint WS_EX_APPWINDOW = 0x00040000;
            public const uint WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE);
            public const uint WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);
            //#endif /* WINVER >= 0x0400 */

            //#if(_WIN32_WINNT >= 0x0500)
            public const uint WS_EX_LAYERED = 0x00080000;
            //#endif /* _WIN32_WINNT >= 0x0500 */

            //#if(WINVER >= 0x0500)
            public const uint WS_EX_NOINHERITLAYOUT = 0x00100000; // Disable inheritence of mirroring by children
            public const uint WS_EX_LAYOUTRTL = 0x00400000; // Right to left mirroring
            //#endif /* WINVER >= 0x0500 */

            //#if(_WIN32_WINNT >= 0x0500)
            public const uint WS_EX_COMPOSITED = 0x02000000;
            public const uint WS_EX_NOACTIVATE = 0x08000000;
            //#endif /* _WIN32_WINNT >= 0x0500 */
        }

        public enum DeferWindowPosCommands : uint
        {
            ///<Summary>Draws a frame (defined in the window's class description) around the window.</Summary>
            SWP_DRAWFRAME = 0x0020,

            ///<Summary>Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.</Summary>
            SWP_FRAMECHANGED = 0x0020,

            ///<Summary>Hides the window.</Summary>
            SWP_HIDEWINDOW = 0x0080,

            ///<Summary>Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).</Summary>
            SWP_NOACTIVATE = 0x0010,

            ///<Summary>Discards the entire contents of the client area. If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.</Summary>
            SWP_NOCOPYBITS = 0x0100,

            ///<Summary>Retains the current position (ignores the x and y parameters).</Summary>
            SWP_NOMOVE = 0x0002,

            ///<Summary>Does not change the owner window's position in the Z order.</Summary>
            SWP_NOOWNERZORDER = 0x0200,

            ///<Summary>Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved. When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.</Summary>
            SWP_NOREDRAW = 0x0008,

            ///<Summary>Same as the SWP_NOOWNERZORDER flag.</Summary>
            SWP_NOREPOSITION = 0x0200,

            ///<Summary>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</Summary>
            SWP_NOSENDCHANGING = 0x0400,

            ///<Summary>Retains the current size (ignores the cx and cy parameters).</Summary>
            SWP_NOSIZE = 0x0001,

            ///<Summary>Retains the current Z order (ignores the hWndInsertAfter parameter).</Summary>
            SWP_NOZORDER = 0x0004,

            ///<Summary>Displays the window.</Summary>
            SWP_SHOWWINDOW = 0x0040
        };

        #region SystemMetric

        /// <summary>
        /// Flags used with the Windows API (User32.dll):GetSystemMetrics(SystemMetric smIndex)
        /// 
        /// This Enum and declaration signature was written by Gabriel T. Sharp
        /// ai_productions@verizon.net or osirisgothra@hotmail.com
        /// Obtained on pinvoke.net, please contribute your code to support the wiki!
        /// </summary>
        public enum SystemMetric : int
        {
            /// <summary>
            /// The flags that specify how the system arranged minimized windows. For more information, see the Remarks section in this topic.
            /// </summary>
            SM_ARRANGE = 56,

            /// <summary>
            /// The value that specifies how the system is started: 
            /// 0 Normal boot
            /// 1 Fail-safe boot
            /// 2 Fail-safe with network boot
            /// A fail-safe boot (also called SafeBoot, Safe Mode, or Clean Boot) bypasses the user startup files.
            /// </summary>
            SM_CLEANBOOT = 67,

            /// <summary>
            /// The number of display monitors on a desktop. For more information, see the Remarks section in this topic.
            /// </summary>
            SM_CMONITORS = 80,

            /// <summary>
            /// The number of buttons on a mouse, or zero if no mouse is installed.
            /// </summary>
            SM_CMOUSEBUTTONS = 43,

            /// <summary>
            /// The width of a window border, in pixels. This is equivalent to the SM_CXEDGE value for windows with the 3-D look.
            /// </summary>
            SM_CXBORDER = 5,

            /// <summary>
            /// The width of a cursor, in pixels. The system cannot create cursors of other sizes.
            /// </summary>
            SM_CXCURSOR = 13,

            /// <summary>
            /// This value is the same as SM_CXFIXEDFRAME.
            /// </summary>
            SM_CXDLGFRAME = 7,

            /// <summary>
            /// The width of the rectangle around the location of a first click in a double-click sequence, in pixels. ,
            /// The second click must occur within the rectangle that is defined by SM_CXDOUBLECLK and SM_CYDOUBLECLK for the system
            /// to consider the two clicks a double-click. The two clicks must also occur within a specified time.
            /// To set the width of the double-click rectangle, call SystemParametersInfo with SPI_SETDOUBLECLKWIDTH.
            /// </summary>
            SM_CXDOUBLECLK = 36,

            /// <summary>
            /// The number of pixels on either side of a mouse-down point that the mouse pointer can move before a drag operation begins. 
            /// This allows the user to click and release the mouse button easily without unintentionally starting a drag operation. 
            /// If this value is negative, it is subtracted from the left of the mouse-down point and added to the right of it.
            /// </summary>
            SM_CXDRAG = 68,

            /// <summary>
            /// The width of a 3-D border, in pixels. This metric is the 3-D counterpart of SM_CXBORDER.
            /// </summary>
            SM_CXEDGE = 45,

            /// <summary>
            /// The thickness of the frame around the perimeter of a window that has a caption but is not sizable, in pixels.
            /// SM_CXFIXEDFRAME is the height of the horizontal border, and SM_CYFIXEDFRAME is the width of the vertical border.
            /// This value is the same as SM_CXDLGFRAME.
            /// </summary>
            SM_CXFIXEDFRAME = 7,

            /// <summary>
            /// The width of the left and right edges of the focus rectangle that the DrawFocusRectdraws. 
            /// This value is in pixels. 
            /// Windows 2000: This value is not supported.
            /// </summary>
            SM_CXFOCUSBORDER = 83,

            /// <summary>
            /// This value is the same as SM_CXSIZEFRAME.
            /// </summary>
            SM_CXFRAME = 32,

            /// <summary>
            /// The width of the client area for a full-screen window on the primary display monitor, in pixels. 
            /// To get the coordinates of the portion of the screen that is not obscured by the system taskbar or by application desktop toolbars, 
            /// call the SystemParametersInfofunction with the SPI_GETWORKAREA value.
            /// </summary>
            SM_CXFULLSCREEN = 16,

            /// <summary>
            /// The width of the arrow bitmap on a horizontal scroll bar, in pixels.
            /// </summary>
            SM_CXHSCROLL = 21,

            /// <summary>
            /// The width of the thumb box in a horizontal scroll bar, in pixels.
            /// </summary>
            SM_CXHTHUMB = 10,

            /// <summary>
            /// The default width of an icon, in pixels. The LoadIcon function can load only icons with the dimensions 
            /// that SM_CXICON and SM_CYICON specifies.
            /// </summary>
            SM_CXICON = 11,

            /// <summary>
            /// The width of a grid cell for items in large icon view, in pixels. Each item fits into a rectangle of size 
            /// SM_CXICONSPACING by SM_CYICONSPACING when arranged. This value is always greater than or equal to SM_CXICON.
            /// </summary>
            SM_CXICONSPACING = 38,

            /// <summary>
            /// The default width, in pixels, of a maximized top-level window on the primary display monitor.
            /// </summary>
            SM_CXMAXIMIZED = 61,

            /// <summary>
            /// The default maximum width of a window that has a caption and sizing borders, in pixels. 
            /// This metric refers to the entire desktop. The user cannot drag the window frame to a size larger than these dimensions. 
            /// A window can override this value by processing the WM_GETMINMAXINFO message.
            /// </summary>
            SM_CXMAXTRACK = 59,

            /// <summary>
            /// The width of the default menu check-mark bitmap, in pixels.
            /// </summary>
            SM_CXMENUCHECK = 71,

            /// <summary>
            /// The width of menu bar buttons, such as the child window close button that is used in the multiple document interface, in pixels.
            /// </summary>
            SM_CXMENUSIZE = 54,

            /// <summary>
            /// The minimum width of a window, in pixels.
            /// </summary>
            SM_CXMIN = 28,

            /// <summary>
            /// The width of a minimized window, in pixels.
            /// </summary>
            SM_CXMINIMIZED = 57,

            /// <summary>
            /// The width of a grid cell for a minimized window, in pixels. Each minimized window fits into a rectangle this size when arranged. 
            /// This value is always greater than or equal to SM_CXMINIMIZED.
            /// </summary>
            SM_CXMINSPACING = 47,

            /// <summary>
            /// The minimum tracking width of a window, in pixels. The user cannot drag the window frame to a size smaller than these dimensions. 
            /// A window can override this value by processing the WM_GETMINMAXINFO message.
            /// </summary>
            SM_CXMINTRACK = 34,

            /// <summary>
            /// The amount of border padding for captioned windows, in pixels. Windows XP/2000: This value is not supported.
            /// </summary>
            SM_CXPADDEDBORDER = 92,

            /// <summary>
            /// The width of the screen of the primary display monitor, in pixels. This is the same value obtained by calling 
            /// GetDeviceCaps as follows: GetDeviceCaps( hdcPrimaryMonitor, HORZRES).
            /// </summary>
            SM_CXSCREEN = 0,

            /// <summary>
            /// The width of a button in a window caption or title bar, in pixels.
            /// </summary>
            SM_CXSIZE = 30,

            /// <summary>
            /// The thickness of the sizing border around the perimeter of a window that can be resized, in pixels. 
            /// SM_CXSIZEFRAME is the width of the horizontal border, and SM_CYSIZEFRAME is the height of the vertical border. 
            /// This value is the same as SM_CXFRAME.
            /// </summary>
            SM_CXSIZEFRAME = 32,

            /// <summary>
            /// The recommended width of a small icon, in pixels. Small icons typically appear in window captions and in small icon view.
            /// </summary>
            SM_CXSMICON = 49,

            /// <summary>
            /// The width of small caption buttons, in pixels.
            /// </summary>
            SM_CXSMSIZE = 52,

            /// <summary>
            /// The width of the virtual screen, in pixels. The virtual screen is the bounding rectangle of all display monitors. 
            /// The SM_XVIRTUALSCREEN metric is the coordinates for the left side of the virtual screen.
            /// </summary>
            SM_CXVIRTUALSCREEN = 78,

            /// <summary>
            /// The width of a vertical scroll bar, in pixels.
            /// </summary>
            SM_CXVSCROLL = 2,

            /// <summary>
            /// The height of a window border, in pixels. This is equivalent to the SM_CYEDGE value for windows with the 3-D look.
            /// </summary>
            SM_CYBORDER = 6,

            /// <summary>
            /// The height of a caption area, in pixels.
            /// </summary>
            SM_CYCAPTION = 4,

            /// <summary>
            /// The height of a cursor, in pixels. The system cannot create cursors of other sizes.
            /// </summary>
            SM_CYCURSOR = 14,

            /// <summary>
            /// This value is the same as SM_CYFIXEDFRAME.
            /// </summary>
            SM_CYDLGFRAME = 8,

            /// <summary>
            /// The height of the rectangle around the location of a first click in a double-click sequence, in pixels. 
            /// The second click must occur within the rectangle defined by SM_CXDOUBLECLK and SM_CYDOUBLECLK for the system to consider 
            /// the two clicks a double-click. The two clicks must also occur within a specified time. To set the height of the double-click 
            /// rectangle, call SystemParametersInfo with SPI_SETDOUBLECLKHEIGHT.
            /// </summary>
            SM_CYDOUBLECLK = 37,

            /// <summary>
            /// The number of pixels above and below a mouse-down point that the mouse pointer can move before a drag operation begins. 
            /// This allows the user to click and release the mouse button easily without unintentionally starting a drag operation. 
            /// If this value is negative, it is subtracted from above the mouse-down point and added below it.
            /// </summary>
            SM_CYDRAG = 69,

            /// <summary>
            /// The height of a 3-D border, in pixels. This is the 3-D counterpart of SM_CYBORDER.
            /// </summary>
            SM_CYEDGE = 46,

            /// <summary>
            /// The thickness of the frame around the perimeter of a window that has a caption but is not sizable, in pixels. 
            /// SM_CXFIXEDFRAME is the height of the horizontal border, and SM_CYFIXEDFRAME is the width of the vertical border. 
            /// This value is the same as SM_CYDLGFRAME.
            /// </summary>
            SM_CYFIXEDFRAME = 8,

            /// <summary>
            /// The height of the top and bottom edges of the focus rectangle drawn byDrawFocusRect. 
            /// This value is in pixels. 
            /// Windows 2000: This value is not supported.
            /// </summary>
            SM_CYFOCUSBORDER = 84,

            /// <summary>
            /// This value is the same as SM_CYSIZEFRAME.
            /// </summary>
            SM_CYFRAME = 33,

            /// <summary>
            /// The height of the client area for a full-screen window on the primary display monitor, in pixels. 
            /// To get the coordinates of the portion of the screen not obscured by the system taskbar or by application desktop toolbars,
            /// call the SystemParametersInfo function with the SPI_GETWORKAREA value.
            /// </summary>
            SM_CYFULLSCREEN = 17,

            /// <summary>
            /// The height of a horizontal scroll bar, in pixels.
            /// </summary>
            SM_CYHSCROLL = 3,

            /// <summary>
            /// The default height of an icon, in pixels. The LoadIcon function can load only icons with the dimensions SM_CXICON and SM_CYICON.
            /// </summary>
            SM_CYICON = 12,

            /// <summary>
            /// The height of a grid cell for items in large icon view, in pixels. Each item fits into a rectangle of size 
            /// SM_CXICONSPACING by SM_CYICONSPACING when arranged. This value is always greater than or equal to SM_CYICON.
            /// </summary>
            SM_CYICONSPACING = 39,

            /// <summary>
            /// For double byte character set versions of the system, this is the height of the Kanji window at the bottom of the screen, in pixels.
            /// </summary>
            SM_CYKANJIWINDOW = 18,

            /// <summary>
            /// The default height, in pixels, of a maximized top-level window on the primary display monitor.
            /// </summary>
            SM_CYMAXIMIZED = 62,

            /// <summary>
            /// The default maximum height of a window that has a caption and sizing borders, in pixels. This metric refers to the entire desktop. 
            /// The user cannot drag the window frame to a size larger than these dimensions. A window can override this value by processing 
            /// the WM_GETMINMAXINFO message.
            /// </summary>
            SM_CYMAXTRACK = 60,

            /// <summary>
            /// The height of a single-line menu bar, in pixels.
            /// </summary>
            SM_CYMENU = 15,

            /// <summary>
            /// The height of the default menu check-mark bitmap, in pixels.
            /// </summary>
            SM_CYMENUCHECK = 72,

            /// <summary>
            /// The height of menu bar buttons, such as the child window close button that is used in the multiple document interface, in pixels.
            /// </summary>
            SM_CYMENUSIZE = 55,

            /// <summary>
            /// The minimum height of a window, in pixels.
            /// </summary>
            SM_CYMIN = 29,

            /// <summary>
            /// The height of a minimized window, in pixels.
            /// </summary>
            SM_CYMINIMIZED = 58,

            /// <summary>
            /// The height of a grid cell for a minimized window, in pixels. Each minimized window fits into a rectangle this size when arranged. 
            /// This value is always greater than or equal to SM_CYMINIMIZED.
            /// </summary>
            SM_CYMINSPACING = 48,

            /// <summary>
            /// The minimum tracking height of a window, in pixels. The user cannot drag the window frame to a size smaller than these dimensions. 
            /// A window can override this value by processing the WM_GETMINMAXINFO message.
            /// </summary>
            SM_CYMINTRACK = 35,

            /// <summary>
            /// The height of the screen of the primary display monitor, in pixels. This is the same value obtained by calling 
            /// GetDeviceCaps as follows: GetDeviceCaps( hdcPrimaryMonitor, VERTRES).
            /// </summary>
            SM_CYSCREEN = 1,

            /// <summary>
            /// The height of a button in a window caption or title bar, in pixels.
            /// </summary>
            SM_CYSIZE = 31,

            /// <summary>
            /// The thickness of the sizing border around the perimeter of a window that can be resized, in pixels. 
            /// SM_CXSIZEFRAME is the width of the horizontal border, and SM_CYSIZEFRAME is the height of the vertical border. 
            /// This value is the same as SM_CYFRAME.
            /// </summary>
            SM_CYSIZEFRAME = 33,

            /// <summary>
            /// The height of a small caption, in pixels.
            /// </summary>
            SM_CYSMCAPTION = 51,

            /// <summary>
            /// The recommended height of a small icon, in pixels. Small icons typically appear in window captions and in small icon view.
            /// </summary>
            SM_CYSMICON = 50,

            /// <summary>
            /// The height of small caption buttons, in pixels.
            /// </summary>
            SM_CYSMSIZE = 53,

            /// <summary>
            /// The height of the virtual screen, in pixels. The virtual screen is the bounding rectangle of all display monitors. 
            /// The SM_YVIRTUALSCREEN metric is the coordinates for the top of the virtual screen.
            /// </summary>
            SM_CYVIRTUALSCREEN = 79,

            /// <summary>
            /// The height of the arrow bitmap on a vertical scroll bar, in pixels.
            /// </summary>
            SM_CYVSCROLL = 20,

            /// <summary>
            /// The height of the thumb box in a vertical scroll bar, in pixels.
            /// </summary>
            SM_CYVTHUMB = 9,

            /// <summary>
            /// Nonzero if User32.dll supports DBCS; otherwise, 0.
            /// </summary>
            SM_DBCSENABLED = 42,

            /// <summary>
            /// Nonzero if the debug version of User.exe is installed; otherwise, 0.
            /// </summary>
            SM_DEBUG = 22,

            /// <summary>
            /// Nonzero if the current operating system is Windows 7 or Windows Server 2008 R2 and the Tablet PC Input 
            /// service is started; otherwise, 0. The return value is a bitmask that specifies the type of digitizer input supported by the device. 
            /// For more information, see Remarks. 
            /// Windows Server 2008, Windows Vista, and Windows XP/2000: This value is not supported.
            /// </summary>
            SM_DIGITIZER = 94,

            /// <summary>
            /// Nonzero if Input Method Manager/Input Method Editor features are enabled; otherwise, 0. 
            /// SM_IMMENABLED indicates whether the system is ready to use a Unicode-based IME on a Unicode application. 
            /// To ensure that a language-dependent IME works, check SM_DBCSENABLED and the system ANSI code page.
            /// Otherwise the ANSI-to-Unicode conversion may not be performed correctly, or some components like fonts
            /// or registry settings may not be present.
            /// </summary>
            SM_IMMENABLED = 82,

            /// <summary>
            /// Nonzero if there are digitizers in the system; otherwise, 0. SM_MAXIMUMTOUCHES returns the aggregate maximum of the 
            /// maximum number of contacts supported by every digitizer in the system. If the system has only single-touch digitizers, 
            /// the return value is 1. If the system has multi-touch digitizers, the return value is the number of simultaneous contacts 
            /// the hardware can provide. Windows Server 2008, Windows Vista, and Windows XP/2000: This value is not supported.
            /// </summary>
            SM_MAXIMUMTOUCHES = 95,

            /// <summary>
            /// Nonzero if the current operating system is the Windows XP, Media Center Edition, 0 if not.
            /// </summary>
            SM_MEDIACENTER = 87,

            /// <summary>
            /// Nonzero if drop-down menus are right-aligned with the corresponding menu-bar item; 0 if the menus are left-aligned.
            /// </summary>
            SM_MENUDROPALIGNMENT = 40,

            /// <summary>
            /// Nonzero if the system is enabled for Hebrew and Arabic languages, 0 if not.
            /// </summary>
            SM_MIDEASTENABLED = 74,

            /// <summary>
            /// Nonzero if a mouse is installed; otherwise, 0. This value is rarely zero, because of support for virtual mice and because 
            /// some systems detect the presence of the port instead of the presence of a mouse.
            /// </summary>
            SM_MOUSEPRESENT = 19,

            /// <summary>
            /// Nonzero if a mouse with a horizontal scroll wheel is installed; otherwise 0.
            /// </summary>
            SM_MOUSEHORIZONTALWHEELPRESENT = 91,

            /// <summary>
            /// Nonzero if a mouse with a vertical scroll wheel is installed; otherwise 0.
            /// </summary>
            SM_MOUSEWHEELPRESENT = 75,

            /// <summary>
            /// The least significant bit is set if a network is present; otherwise, it is cleared. The other bits are reserved for future use.
            /// </summary>
            SM_NETWORK = 63,

            /// <summary>
            /// Nonzero if the Microsoft Windows for Pen computing extensions are installed; zero otherwise.
            /// </summary>
            SM_PENWINDOWS = 41,

            /// <summary>
            /// This system metric is used in a Terminal Services environment to determine if the current Terminal Server session is 
            /// being remotely controlled. Its value is nonzero if the current session is remotely controlled; otherwise, 0. 
            /// You can use terminal services management tools such as Terminal Services Manager (tsadmin.msc) and shadow.exe to 
            /// control a remote session. When a session is being remotely controlled, another user can view the contents of that session 
            /// and potentially interact with it.
            /// </summary>
            SM_REMOTECONTROL = 0x2001,

            /// <summary>
            /// This system metric is used in a Terminal Services environment. If the calling process is associated with a Terminal Services 
            /// client session, the return value is nonzero. If the calling process is associated with the Terminal Services console session, 
            /// the return value is 0. 
            /// Windows Server 2003 and Windows XP: The console session is not necessarily the physical console. 
            /// For more information, seeWTSGetActiveConsoleSessionId.
            /// </summary>
            SM_REMOTESESSION = 0x1000,

            /// <summary>
            /// Nonzero if all the display monitors have the same color format, otherwise, 0. Two displays can have the same bit depth, 
            /// but different color formats. For example, the red, green, and blue pixels can be encoded with different numbers of bits, 
            /// or those bits can be located in different places in a pixel color value.
            /// </summary>
            SM_SAMEDISPLAYFORMAT = 81,

            /// <summary>
            /// This system metric should be ignored; it always returns 0.
            /// </summary>
            SM_SECURE = 44,

            /// <summary>
            /// The build number if the system is Windows Server 2003 R2; otherwise, 0.
            /// </summary>
            SM_SERVERR2 = 89,

            /// <summary>
            /// Nonzero if the user requires an application to present information visually in situations where it would otherwise present 
            /// the information only in audible form; otherwise, 0.
            /// </summary>
            SM_SHOWSOUNDS = 70,

            /// <summary>
            /// Nonzero if the current session is shutting down; otherwise, 0. Windows 2000: This value is not supported.
            /// </summary>
            SM_SHUTTINGDOWN = 0x2000,

            /// <summary>
            /// Nonzero if the computer has a low-end (slow) processor; otherwise, 0.
            /// </summary>
            SM_SLOWMACHINE = 73,

            /// <summary>
            /// Nonzero if the current operating system is Windows 7 Starter Edition, Windows Vista Starter, or Windows XP Starter Edition; otherwise, 0.
            /// </summary>
            SM_STARTER = 88,

            /// <summary>
            /// Nonzero if the meanings of the left and right mouse buttons are swapped; otherwise, 0.
            /// </summary>
            SM_SWAPBUTTON = 23,

            /// <summary>
            /// Nonzero if the current operating system is the Windows XP Tablet PC edition or if the current operating system is Windows Vista
            /// or Windows 7 and the Tablet PC Input service is started; otherwise, 0. The SM_DIGITIZER setting indicates the type of digitizer 
            /// input supported by a device running Windows 7 or Windows Server 2008 R2. For more information, see Remarks.
            /// </summary>
            SM_TABLETPC = 86,

            /// <summary>
            /// The coordinates for the left side of the virtual screen. The virtual screen is the bounding rectangle of all display monitors. 
            /// The SM_CXVIRTUALSCREEN metric is the width of the virtual screen.
            /// </summary>
            SM_XVIRTUALSCREEN = 76,

            /// <summary>
            /// The coordinates for the top of the virtual screen. The virtual screen is the bounding rectangle of all display monitors. 
            /// The SM_CYVIRTUALSCREEN metric is the height of the virtual screen.
            /// </summary>
            SM_YVIRTUALSCREEN = 77,
        }

        #endregion

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

        [DllImport("user32.dll", SetLastError = true)]
        public static extern Boolean AdjustWindowRect(ref RECT lpRect, UIntPtr dwStyle, bool bMenu);

        [DllImport("user32.dll")]
        public static extern IntPtr BeginDeferWindowPos(int nNumWindows);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool BringWindowToTop(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, out POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern IntPtr DeferWindowPos(IntPtr hWinPosInfo, IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, [MarshalAs(UnmanagedType.U4)]DeferWindowPosCommands uFlags);

        [DllImport("user32.dll")]
        public static extern bool DrawMenuBar(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool EndDeferWindowPos(IntPtr hWinPosInfo);

        [DllImport("user32.dll")]
        public static extern bool EnumDesktopWindows(IntPtr hDesktop, EnumDelegate lpEnumCallbackFunction, IntPtr lParam);

        [DllImport("user32.dll")]
        public extern static bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        public extern static int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetMenu(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int GetMenuItemCount(IntPtr hMenu);

        [DllImport("user32.dll")]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(SystemMetric smIndex);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindow_Cmd uCmd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowEnabled(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool redraw);

        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        public static extern bool RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, [Out] StringBuilder lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SetCapture(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("user32.dll")]
        public static extern bool SetWindowText(IntPtr hwnd, String lpString);

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
            return new System.Drawing.Rectangle(lp.X, lp.Y, rect.Width, rect.Height);
        }

        public static System.Drawing.Rectangle GetClientRectangleRelativeTo(IntPtr hwndChild, IntPtr hwndParent)
        {
            Rectangle crChild = GetClientRectangle(hwndChild);
            Rectangle crParent = GetClientRectangle(hwndParent);
            return new Rectangle(crChild.X - crParent.X, crChild.Y - crParent.Y, crChild.Width, crChild.Height);
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

        public static IntPtr[] GetWindowHandlesByClassName(string className)
        {
            List<IntPtr> collection = new List<IntPtr>();
            EnumDelegate filter = delegate(IntPtr hwnd, int lParam)
            {
                if (GetClassName(hwnd).Equals(className) && IsWindowVisible(hwnd))
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

        public static string GetWindowTextRaw(IntPtr hwnd)
        {
            // Allocate correct string length first
            int length = (int)SendMessage(hwnd, WM_GETTEXTLENGTH, IntPtr.Zero, IntPtr.Zero);
            StringBuilder sb = new StringBuilder(length + 1);
            SendMessage(hwnd, WM_GETTEXT, (IntPtr)sb.Capacity, sb);
            return sb.ToString();
        }

        public static string GetWindowTitle(IntPtr hwnd)
        {
            StringBuilder lpWindowText = new StringBuilder(byte.MaxValue);
            GetWindowText(hwnd, lpWindowText, lpWindowText.Capacity + 1);
            return lpWindowText.ToString();
        }

        public static IntPtr ___FindChildWindow(IntPtr hwndParent, string lpszClass, string lpszTitle)
        {
            return FindChildWindow(hwndParent, IntPtr.Zero, lpszClass, lpszTitle);
        }

        public static IntPtr FindChildWindow(IntPtr hwndParent, string lpszClass, string lpszTitle)
        {
            IntPtr child = IntPtr.Zero;
            while (true)
            {
                IntPtr newChild = FindWindowEx(hwndParent, child, lpszClass, "");
                if (newChild != IntPtr.Zero)
                {
                    string rawText = GetWindowTextRaw(newChild);
                    if (lpszTitle.Equals(rawText))
                    {
                        return newChild;
                    }
                    child = newChild;
                }
                else
                {
                    break;
                }
            }
            return IntPtr.Zero;
        }

        public static IntPtr FindChildWindow(IntPtr hwndParent, string lpszClass, IEnumerable<string> possibleTitles)
        {
            IntPtr child = IntPtr.Zero;
            while (true)
            {
                IntPtr newChild = FindWindowEx(hwndParent, child, lpszClass, "");
                if (newChild != IntPtr.Zero)
                {
                    string rawText = GetWindowTextRaw(newChild);
                    if (possibleTitles.Contains(rawText))
                    {
                        return newChild;
                    }
                    child = newChild;
                }
                else
                {
                    break;
                }
            }
            return IntPtr.Zero;
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

        public static List<IntPtr> FindAllChildWindowByClass(IntPtr hwndParent, string lpszClass)
        {
            List<IntPtr> children = new List<IntPtr>();
            IntPtr child = IntPtr.Zero;
            while (true)
            {
                IntPtr newChild = FindWindowEx(hwndParent, child, lpszClass, "");
                if (newChild != IntPtr.Zero)
                {
                    children.Add(newChild);
                    child = newChild;
                }
                else
                {
                    break;
                }
            }
            return children;
        }

        public static List<IntPtr> FindAllChildWindow(IntPtr hwndParent, string lpszClass, string lpszTitle)
        {
            List<IntPtr> children = new List<IntPtr>();
            IntPtr child = IntPtr.Zero;
            while (true)
            {
                IntPtr newChild = FindWindowEx(hwndParent, child, lpszClass, "");
                if (newChild != IntPtr.Zero)
                {
                    if (lpszTitle.Equals(GetWindowTextRaw(newChild)))
                    {
                        children.Add(newChild);
                    }
                    child = newChild;
                }
                else
                {
                    break;
                }
            }
            return children;
        }

        public static void SetClientRectangle(IntPtr hwnd, UIntPtr dwStyle, Rectangle r, bool bMenu)
        {
            RECT rect = new RECT(r);
            AdjustWindowRect(ref rect, dwStyle, bMenu);
            MoveWindow(hwnd, rect.X, rect.Y, rect.Width, rect.Height, true);
        }

        // BORDERS: left + top + right + bottom = 5 + 27 + 5 + 5  
        // CLIENT: 792, 546
        // WINDOW WS_OVERLAPPEDWINDOW: 802, 578
        // CLIENT WS_THICKFRAME: 802, 556

        public const uint WINDOW_STYLE_NORMAL = WindowStyles.WS_CAPTION | WindowStyles.WS_VISIBLE | WindowStyles.WS_CLIPSIBLINGS | WindowStyles.WS_CLIPCHILDREN
            | WindowStyles.WS_SYSMENU | WindowStyles.WS_THICKFRAME | WindowStyles.WS_OVERLAPPED | WindowStyles.WS_MINIMIZEBOX | WindowStyles.WS_MAXIMIZEBOX; // 0x16cf0000;
        public const uint WINDOW_EX_STYLE_NORMAL = WindowStyles.WS_EX_LEFT | WindowStyles.WS_EX_LTRREADING | WindowStyles.WS_EX_RIGHTSCROLLBAR | WindowStyles.WS_EX_WINDOWEDGE; //0x00000100;
        public const uint WINDOW_STYLE_NOCAPTION = WindowStyles.WS_SIZEBOX | WindowStyles.WS_THICKFRAME | WindowStyles.WS_TABSTOP | WindowStyles.WS_VISIBLE | WindowStyles.WS_CLIPSIBLINGS | WindowStyles.WS_CLIPCHILDREN;
        public const uint WINDOW_STYLE_BORDERLESS = WindowStyles.WS_TABSTOP | WindowStyles.WS_VISIBLE | WindowStyles.WS_CLIPSIBLINGS | WindowStyles.WS_CLIPCHILDREN;

        public static void SetWindowStyleNormal(IntPtr handle)
        {
            EnsureWindowGwlStyleGwlExStyle(handle, WINDOW_STYLE_NORMAL, WINDOW_EX_STYLE_NORMAL);
        }

        public static void SetWindowStyleNoCaption(IntPtr handle)
        {
            EnsureWindowGwlStyleGwlExStyle(handle, WINDOW_STYLE_NOCAPTION, WINDOW_EX_STYLE_NORMAL);
        }

        public static void SetWindowStyleNone(IntPtr handle)
        {
            EnsureWindowGwlStyleGwlExStyle(handle, WINDOW_STYLE_BORDERLESS, WINDOW_EX_STYLE_NORMAL);
        }

        public static void EnsureWindowGwlStyleGwlExStyle(IntPtr handle, uint gwlStyle, uint gwExlStyle)
        {
            if (GetWindowLong(handle, GWL_STYLE) != gwlStyle || GetWindowLong(handle, GWL_EXSTYLE) != gwExlStyle)
            {
                Rectangle clientRectangle = GetClientRectangle(handle);
                RECT rect = new RECT(clientRectangle);
                AdjustWindowRect(ref rect, (UIntPtr)gwlStyle, false);

                SetWindowLong(handle, GWL_STYLE, (int)gwlStyle);
                SetWindowLong(handle, GWL_EXSTYLE, (int)gwExlStyle);
                SetWindowPos(handle, IntPtr.Zero, 0, 0, rect.Width, rect.Height, SetWindowPosFlags.FrameChanged | SetWindowPosFlags.IgnoreMove | SetWindowPosFlags.IgnoreZOrder | SetWindowPosFlags.DoNotChangeOwnerZOrder);
            }
        }

        //

        public static void KeyType(IntPtr handle, System.Windows.Input.Key key)
        {
            PostMessage(handle, WM_CHAR, new IntPtr(System.Windows.Input.KeyInterop.VirtualKeyFromKey(key)), IntPtr.Zero);
        }

        public static void KeyDown(IntPtr handle, System.Windows.Input.Key key)
        {
            PostMessage(handle, WM_KEYDOWN, new IntPtr(System.Windows.Input.KeyInterop.VirtualKeyFromKey(key)), IntPtr.Zero);
        }

        public static void KeyUp(IntPtr handle, System.Windows.Input.Key key)
        {
            PostMessage(handle, WM_KEYUP, new IntPtr(System.Windows.Input.KeyInterop.VirtualKeyFromKey(key)), IntPtr.Zero);
        }
    }
}

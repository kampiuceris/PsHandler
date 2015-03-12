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
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace PsHandler.UI
{
    [StructLayout(LayoutKind.Sequential)]
    public struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    };

    public class GlassEffect
    {
        [DllImport("DwmApi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        static extern bool DwmIsCompositionEnabled();

        public static readonly DependencyProperty IsEnabledProperty =
                DependencyProperty.RegisterAttached("IsEnabled",
                typeof(Boolean),
                typeof(GlassEffect),
                new FrameworkPropertyMetadata(OnIsEnabledChanged));

        public static void SetIsEnabled(DependencyObject element, Boolean value)
        {
            element.SetValue(IsEnabledProperty, value);
        }
        public static Boolean GetIsEnabled(DependencyObject element)
        {
            return (Boolean)element.GetValue(IsEnabledProperty);
        }

        public static void OnIsEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if ((bool)args.NewValue == true)
            {
                try
                {
                    Window wnd = (Window)obj;
                    wnd.Loaded += new RoutedEventHandler(wnd_Loaded);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error(GlassEffect): " + e.Message + "\nStackTrace: " + e.StackTrace);
                }
            }
        }

        static void wnd_Loaded(object sender, RoutedEventArgs e)
        {
            Window wnd = (Window)sender;
            Brush originalBackground = wnd.Background;
            wnd.Background = Brushes.Transparent;
            try
            {
                IntPtr mainWindowPtr = new WindowInteropHelper(wnd).Handle;
                HwndSource mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
                mainWindowSrc.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

                //System.Drawing.Graphics desktop = System.Drawing.Graphics.FromHwnd(mainWindowPtr);
                //float DesktopDpiX = desktop.DpiX;
                //float DesktopDpiY = desktop.DpiY;

                MARGINS margins = new MARGINS();
                margins.cxLeftWidth = -1;
                margins.cxRightWidth = -1;
                margins.cyTopHeight = -1;
                margins.cyBottomHeight = -1;

                DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref margins);
            }
            catch (DllNotFoundException)
            {
                wnd.Background = originalBackground;
            }
        }
    }
}

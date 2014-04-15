using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace PsHandler
{
    public static class ExtensionMethods
    {
        public static void SetProperty(this object o, string propertyName, object property, bool setForChildren = false)
        {
            if (o == null) return;

            PropertyInfo pi = o.GetType().GetProperty(propertyName);
            if (pi != null)
            {
                pi.SetValue(o, property, null);

                if (setForChildren)
                {
                    if (o is GroupBox)
                    {
                        (o as GroupBox).Content.SetProperty(propertyName, property, true);
                    }
                    if (o is Grid)
                    {
                        foreach (var child in (o as Grid).Children)
                        {
                            child.SetProperty(propertyName, property, true);
                        }
                    }
                    if (o is StackPanel)
                    {
                        foreach (var child in (o as StackPanel).Children)
                        {
                            child.SetProperty(propertyName, property, true);
                        }
                    }
                    if (o is UserControl)
                    {
                        (o as UserControl).Content.SetProperty(propertyName, property, true);
                    }
                }
            }
        }

        /// <summary>
        /// Converts a <see cref="System.Drawing.Image"/> into a WPF <see cref="BitmapSource"/>.
        /// </summary>
        /// <param name="source">The source image.</param>
        /// <returns>A BitmapSource</returns>
        public static BitmapSource ToBitmapSource(this System.Drawing.Image source)
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(source);

            var bitSrc = bitmap.ToBitmapSource();

            bitmap.Dispose();
            bitmap = null;

            return bitSrc;
        }

        /// <summary>
        /// Converts a <see cref="System.Drawing.Bitmap"/> into a WPF <see cref="BitmapSource"/>.
        /// </summary>
        /// <remarks>Uses GDI to do the conversion. Hence the call to the marshalled DeleteObject.
        /// </remarks>
        /// <param name="source">The source bitmap.</param>
        /// <returns>A BitmapSource</returns>
        public static BitmapSource ToBitmapSource(this System.Drawing.Bitmap source)
        {
            BitmapSource bitSrc = null;

            var hBitmap = source.GetHbitmap();

            try
            {
                bitSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Win32Exception)
            {
                bitSrc = null;
            }
            finally
            {
                WinApi.DeleteObject(hBitmap);
            }

            return bitSrc;
        }
    }
}

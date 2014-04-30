using System.Runtime.InteropServices;

namespace Hardcodet.Wpf.TaskbarNotification.Interop
{
    /// <summary>
    /// Win API struct providing coordinates for a single point.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        /// <summary>
        /// LocationX coordinate.
        /// </summary>
        public int X;
        /// <summary>
        /// LocationY coordinate.
        /// </summary>
        public int Y;
    }
}
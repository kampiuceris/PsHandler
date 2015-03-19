using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using PsHandler.Custom;

namespace PsHandler
{
    public class Test
    {
        // windows borders ~ 8 31 8 8
        static double RATIO = 792d / 546d;
        static Size SMALLEST = new Size(475, 327);
        static Size DEFAULT = new Size(792, 546);
        static Size BIGGEST = new Size(1320, 910);
        //static Rectangle THEME_MERCURY = new Rectangle(615, 473, 130, 30);
        //static Rectangle THEME_NOVA = new Rectangle(615, 473, 130, 30);
        //static Rectangle THEME_CLASSIC = new Rectangle(552, 458, 97, 19);
        //static Rectangle THEME_BLACK = new Rectangle(530, 446, 141, 44);
        //static Rectangle THEME_SLICK = new Rectangle(532, 446, 136, 44);
        //static Rectangle THEME_STARS = new Rectangle(555, 456, 94, 25);
        //static Rectangle THEME_HYPERSIMPLE = new Rectangle(555, 456, 94, 25);
        //static Rectangle THEME_AZURE = new Rectangle(534, 445, 132, 49);
        static Rectangle THEME = new Rectangle(534, 445, 132, 49);

        public static void Do()
        {
            var readAllLines = File.ReadAllLines(@"C:\Users\WinWork\Desktop\classic.txt");
            var array = new TableSizeAverageColors[readAllLines.Length];
            for (int i = 0; i < readAllLines.Length; i++)
            {
                var split = readAllLines[i].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                array[i] = new TableSizeAverageColors
                {
                    TableSize = new Size(int.Parse(split[0]), int.Parse(split[1])),
                    ScanRectangle = new Rectangle(int.Parse(split[2]), int.Parse(split[3]), int.Parse(split[4]), int.Parse(split[5])),
                    R = double.Parse(split[6]),
                    G = double.Parse(split[7]),
                    B = double.Parse(split[8])
                };
            }



            var sb = new StringBuilder();
            foreach (var item in array)
            {
                sb.AppendLine(string.Format("new TableSizeAverageColors{0} TableSize = new Size({1}, {2}), ScanRectangle = new Rectangle({3},{4},{5},{6}), R = {7}, G = {8}, B = {9} {10},",
                    "{", item.TableSize.Width, item.TableSize.Height, item.ScanRectangle.X, item.ScanRectangle.Y, item.ScanRectangle.Width, item.ScanRectangle.Height, item.R, item.G, item.B, "}"
                    ));
            }
            File.WriteAllText(@"C:\Users\WinWork\Desktop\output.txt", sb.ToString());
        }

        public static void TestLive()
        {
            var readAllLines = File.ReadAllLines(@"C:\Users\WinWork\Desktop\azure.txt");
            var array = new TableSizeAverageColors[readAllLines.Length];
            for (int i = 0; i < readAllLines.Length; i++)
            {
                var split = readAllLines[i].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                array[i] = new TableSizeAverageColors
                {
                    TableSize = new Size(int.Parse(split[0]), int.Parse(split[1])),
                    ScanRectangle = new Rectangle(int.Parse(split[2]), int.Parse(split[3]), int.Parse(split[4]), int.Parse(split[5])),
                    R = double.Parse(split[6]),
                    G = double.Parse(split[7]),
                    B = double.Parse(split[8])
                };
            }

            while (true)
            {
                var handles = WinApi.GetWindowHandlesByClassName("PokerStarsTableFrameClass");
                if (!handles.Any()) continue;

                var handle = handles.First();
                var wr = WinApi.GetWindowRectangle(handle);
                var cr = WinApi.GetClientRectangle(handle);

                var bmp = new Bmp(ScreenCapture.GetBitmapWindowClient(handle));

                var closest = array.FirstOrDefault(a => a.TableSize.Width == cr.Width && a.TableSize.Height == cr.Height);
                if (closest == null) continue;

                var ratioWidth = (double)cr.Width / DEFAULT.Width;
                var ratioHeight = (double)cr.Height / DEFAULT.Height;
                var theme = THEME;

                //var rectangle = new Rectangle(
                //            (int)Math.Round(theme.X * ratioWidth),
                //            (int)Math.Round(theme.Y * ratioHeight),
                //            (int)Math.Round(theme.Width * ratioWidth),
                //            (int)Math.Round(theme.Height * ratioHeight)
                //            );
                double r, g, b;
                Methods.AverageColor(bmp, closest.ScanRectangle, out r, out g, out b);

                Debug.WriteLine(string.Format("{0,-3:0.00} {1,-3:0.00} {2,-3:0.00}", Math.Abs(closest.R - r), Math.Abs(closest.G - g), Math.Abs(closest.B - b)));
                Console.WriteLine(string.Format("{0,-3:0.00} {1,-3:0.00} {2,-3:0.00}", Math.Abs(closest.R - r), Math.Abs(closest.G - g), Math.Abs(closest.B - b)));

                Thread.Sleep(1000);
            }
        }

        public static void Scrape()
        {
            Thread.Sleep(3000);

            var handle = WinApi.GetWindowHandlesByClassName("PokerStarsTableFrameClass").First();
            var wr = WinApi.GetWindowRectangle(handle);

            WinApi.MoveWindow(handle, wr.Left, wr.Top, SMALLEST.Width + 8 + 8, SMALLEST.Height + 31 + 8, true);
            wr = WinApi.GetWindowRectangle(handle);
            Thread.Sleep(250);
            MouseOperations.SetCursorPosition(wr.Right - 4, wr.Top + 100);
            Thread.Sleep(250);
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftDown);

            var sb = new StringBuilder();
            for (int i = 0; i <= BIGGEST.Width - SMALLEST.Width; i++)
            {
                MouseOperations.SetCursorPosition(wr.Right - 4 + i, wr.Top + 100);
                Thread.Sleep(200);

                var cr = WinApi.GetClientRectangle(handle);
                var bmp = new Bmp(ScreenCapture.GetBitmapWindowClient(handle));
                var ratioWidth = (double)cr.Width / DEFAULT.Width;
                var ratioHeight = (double)cr.Height / DEFAULT.Height;
                var theme = THEME;

                var rectangle = new Rectangle(
                            (int)Math.Round(theme.X * ratioWidth),
                            (int)Math.Round(theme.Y * ratioHeight),
                            (int)Math.Round(theme.Width * ratioWidth),
                            (int)Math.Round(theme.Height * ratioHeight)
                            );
                double r, g, b;
                Methods.AverageColor(bmp, rectangle, out r, out g, out b);
                Debug.WriteLine(string.Format("{0} {1} {2} {3} {4} {5} {6,3:0.00} {7,3:0.00} {8,3:0.00}", cr.Width, cr.Height, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, r, g, b));
                sb.AppendLine(string.Format("{0} {1} {2} {3} {4} {5} {6,3:0.00} {7,3:0.00} {8,3:0.00}", cr.Width, cr.Height, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, r, g, b));
            }
            MouseOperations.MouseEvent(MouseOperations.MouseEventFlags.LeftUp);
            Thread.Sleep(250);

            File.WriteAllText(@"C:\Users\WinWork\Desktop\azure1.txt", sb.ToString());
        }
    }

    public class MouseOperations
    {
        [Flags]
        public enum MouseEventFlags
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        public static void SetCursorPosition(int X, int Y)
        {
            SetCursorPos(X, Y);
        }

        public static void SetCursorPosition(MousePoint point)
        {
            SetCursorPos(point.X, point.Y);
        }

        public static MousePoint GetCursorPosition()
        {
            MousePoint currentMousePoint;
            var gotPoint = GetCursorPos(out currentMousePoint);
            if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
            return currentMousePoint;
        }

        public static void MouseEvent(MouseEventFlags value)
        {
            MousePoint position = GetCursorPosition();

            mouse_event
                ((int)value,
                 position.X,
                 position.Y,
                 0,
                 0)
                ;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint
        {
            public int X;
            public int Y;

            public MousePoint(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
    }
}

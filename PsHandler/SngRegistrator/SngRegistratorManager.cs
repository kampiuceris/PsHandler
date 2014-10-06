using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hardcodet.Wpf.TaskbarNotification;
using PsHandler.Custom;

namespace PsHandler.SngRegistrator
{
    public class SngRegistratorManager
    {
        private Thread _thread;
        private readonly Bmp _bmpMainLobbyOn;
        private readonly Bmp _bmpMainLobbyOff;
        private readonly Bmp _bmpMainLobbyOffHover;
        private readonly Bmp _bmpMainLobbyPressed;
        private IntPtr _handleWindowPokerStarsLobby;
        private IntPtr _handleSelectorLobbySelector;
        private IntPtr _handleSelectorMainLobbySelector;

        public SngRegistratorManager()
        {
            _handleWindowPokerStarsLobby = WinApi.FindWindow("#32770", "PokerStars Lobby");
            if (_handleWindowPokerStarsLobby.Equals(IntPtr.Zero)) throw new NotSupportedException("Cannot find Window 'PokerStars Lobby'.");
            _handleSelectorLobbySelector = WinApi.FindChildWindow(_handleWindowPokerStarsLobby, "PokerStarsSelectorClass", "lobby-selector");
            if (_handleSelectorLobbySelector.Equals(IntPtr.Zero)) throw new NotSupportedException("Cannot find Selector 'Lobby Selector'.");
            _handleSelectorMainLobbySelector = WinApi.FindChildWindow(_handleWindowPokerStarsLobby, "PokerStarsSelectorClass", "main-lobby-selector");
            if (_handleSelectorMainLobbySelector.Equals(IntPtr.Zero)) throw new NotSupportedException("Cannot find Selector 'Main Lobby Selector'.");

            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.main_lobby_on.png"))
            {
                _bmpMainLobbyOn = new Bmp(bitmap);
            }
            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.main_lobby_off.png"))
            {
                _bmpMainLobbyOff = new Bmp(bitmap);
            }
            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.main_lobby_off_hover.png"))
            {
                _bmpMainLobbyOffHover = new Bmp(bitmap);
            }
            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.main_lobby_pressed.png"))
            {
                _bmpMainLobbyPressed = new Bmp(bitmap);
            }
        }

        public void Stop()
        {
            if (_thread != null)
            {
                _thread.Interrupt();
            }
        }

        public void Start()
        {
            Stop();

            //_thread = new Thread(() =>
            //{
            while (true)
            {
                EnsureSelectorLobbySelectorMainLobbyOpened();
                EnsureSelectorMainLobbySelectorSitAndGoOpened();

                Thread.Sleep(100);
                // break;
            }
            //});
            //_thread.Start();
        }

        private Bmp GetBmpWindowPokerStarsMainLobby()
        {
            Bmp bmpPokerStarsLobby;
            using (Bitmap bitmapPokerStarsLobby = ScreenCapture.GetBitmapWindowClient(_handleWindowPokerStarsLobby))
            {
                bmpPokerStarsLobby = new Bmp(bitmapPokerStarsLobby);
            }
            return bmpPokerStarsLobby;
        }

        private void EnsureSelectorLobbySelectorMainLobbyOpened()
        {
            DateTime started = DateTime.Now;
            while (true)
            {
                Bmp bmpSelectorLobbySelector = GetBmpWindowPokerStarsMainLobby().CutRectangle(WinApi.GetClientRectangleRelativeTo(_handleSelectorLobbySelector, _handleWindowPokerStarsLobby));
                Point point;
                // check if is on
                point = FindBmp(bmpSelectorLobbySelector, _bmpMainLobbyOn);
                if (point.IsValid())
                {
                    break;
                }
                // check if is off
                point = FindBmp(bmpSelectorLobbySelector, _bmpMainLobbyOff);
                if (point.IsValid())
                {
                    Methods.MouseEnterLeftMouseClickMouseLeave(_handleSelectorLobbySelector, point.X, point.Y);
                    continue;
                }
                // check if is hover
                point = FindBmp(bmpSelectorLobbySelector, _bmpMainLobbyOffHover);
                if (point.IsValid())
                {
                    Methods.MouseEnterLeftMouseClickMouseLeave(_handleSelectorLobbySelector, point.X, point.Y);
                    continue;
                }
                // check if is pressed
                point = FindBmp(bmpSelectorLobbySelector, _bmpMainLobbyPressed);
                if (point.IsValid())
                {
                    Methods.MouseEnterLeftMouseClickMouseLeave(_handleSelectorLobbySelector, point.X, point.Y);
                    continue;
                }

                if ((DateTime.Now - started).TotalSeconds < 5) throw new NotSupportedException("Cannot open 'Main Lobby'.");
            }
        }

        private void EnsureSelectorMainLobbySelectorSitAndGoOpened()
        {
            DateTime started = DateTime.Now;
            while (true)
            {
                Bmp bmpSelectorMainLobbySelector = GetBmpWindowPokerStarsMainLobby().CutRectangle(WinApi.GetClientRectangleRelativeTo(_handleSelectorMainLobbySelector, _handleWindowPokerStarsLobby));

                Methods.DisplayBitmap(bmpSelectorMainLobbySelector.ToBitmap(), true);

                Point point;
                // check if is on
                point = FindBmp(bmpSelectorMainLobbySelector, _bmpMainLobbyOn);
                if (point.IsValid())
                {
                    break;
                }
                // check if is off
                point = FindBmp(bmpSelectorMainLobbySelector, _bmpMainLobbyOff);
                if (point.IsValid())
                {
                    Methods.MouseEnterLeftMouseClickMouseLeave(_handleSelectorMainLobbySelector, point.X, point.Y);
                    continue;
                }
                // check if is hover
                point = FindBmp(bmpSelectorMainLobbySelector, _bmpMainLobbyOffHover);
                if (point.IsValid())
                {
                    Methods.MouseEnterLeftMouseClickMouseLeave(_handleSelectorMainLobbySelector, point.X, point.Y);
                    continue;
                }
                // check if is pressed
                point = FindBmp(bmpSelectorMainLobbySelector, _bmpMainLobbyPressed);
                if (point.IsValid())
                {
                    Methods.MouseEnterLeftMouseClickMouseLeave(_handleSelectorMainLobbySelector, point.X, point.Y);
                    continue;
                }

                if ((DateTime.Now - started).TotalSeconds < 5) throw new NotSupportedException("Cannot open 'Main Lobby'.");
            }
        }


        //

        public static Point FindBmp(Bmp bmpSource, Bmp bmpTarget, double accuracy = 1.0, int xSourceStarting = 0, int ySourceStarting = 0)
        {
            int totalPixelsToCheck = bmpTarget.Width * bmpTarget.Height;
            int minPixelsToMatch = (int)Math.Round(totalPixelsToCheck * accuracy);
            int maxPixelsToMismatch = (int)Math.Round(totalPixelsToCheck * (1.0 - accuracy));
            for (int ySource = xSourceStarting; ySource < bmpSource.Height; ySource++)
            {
                for (int xSource = ySourceStarting; xSource < bmpSource.Width; xSource++)
                {
                    if (BmpSourceFitsBmpTarget(bmpSource, xSource, ySource, bmpTarget, minPixelsToMatch, maxPixelsToMismatch))
                    {
                        return new Point(xSource, ySource);
                    }
                }
            }
            return new Point(int.MinValue, int.MinValue);
        }

        private static bool BmpSourceFitsBmpTarget(Bmp bmpSource, int xSource, int ySource, Bmp bmpTarget, int minPixelsToMatch, int maxPixelsToMismatch)
        {
            if (xSource + bmpTarget.Width > bmpSource.Width || ySource + bmpTarget.Height > bmpSource.Height) return false;

            int pixelsMatched = 0, pixelsMismatched = 0;
            for (int yTarget = 0; yTarget < bmpTarget.Height; yTarget++)
            {
                for (int xTarget = 0; xTarget < bmpTarget.Width; xTarget++)
                {
                    if (bmpSource.GetPixelR(xSource + xTarget, ySource + yTarget) == bmpTarget.GetPixelR(xTarget, yTarget)
                        && bmpSource.GetPixelG(xSource + xTarget, ySource + yTarget) == bmpTarget.GetPixelG(xTarget, yTarget)
                        && bmpSource.GetPixelB(xSource + xTarget, ySource + yTarget) == bmpTarget.GetPixelB(xTarget, yTarget))
                    {
                        pixelsMatched++;
                    }
                    else
                    {
                        pixelsMismatched++;
                    }

                    if (pixelsMismatched > maxPixelsToMismatch)
                    {
                        return false;
                    }
                    if (pixelsMatched >= minPixelsToMatch)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //

    }
}

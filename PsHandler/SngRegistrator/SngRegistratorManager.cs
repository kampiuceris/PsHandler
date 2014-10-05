using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PsHandler.Custom;

namespace PsHandler.SngRegistrator
{
    public class SngRegistratorManager
    {
        private Thread _thread;
        private readonly Bmp _bmpMainLobbyOffHighConrast;
        private readonly Bmp _bmpMainLobbyOnHighConrast;

        public SngRegistratorManager()
        {
            using (Bitmap bitmapMainLobbyOff = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.main_lobby_off.png"))
            {
                _bmpMainLobbyOffHighConrast = new Bmp(bitmapMainLobbyOff).ToHighContrast();
            }
            using (Bitmap bitmapMainLobbyOn = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.main_lobby_on.png"))
            {
                _bmpMainLobbyOnHighConrast = new Bmp(bitmapMainLobbyOn).ToHighContrast();
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
                IntPtr handlePokerStarsLobby = WinApi.FindWindow("#32770", "PokerStars Lobby");
                Bmp bmpPokerStarsLobby, bmpPokerStarsLobbyHighContrast;
                using (Bitmap bitmapPokerStarsLobby = ScreenCapture.GetBitmapWindowClient(handlePokerStarsLobby))
                {
                    bmpPokerStarsLobby = new Bmp(bitmapPokerStarsLobby);
                    bmpPokerStarsLobbyHighContrast = bmpPokerStarsLobby.ToHighContrastClone();
                }
                IntPtr handleLobbySelector = WinApi.FindChildWindow(handlePokerStarsLobby, "PokerStarsSelectorClass", new[] { "lobby-selector", "Quick Seat", "Main Lobby", "Favorites", "News" });
                if (handleLobbySelector == IntPtr.Zero) throw new NotSupportedException("Cannot find 'Lobby Slector' handle.");

                Rectangle crrLobbySelector = WinApi.GetClientRectangleRelativeTo(handleLobbySelector, handlePokerStarsLobby);
                Bmp bmpLobbySelectorHighContrast = bmpPokerStarsLobbyHighContrast.CutRectangle(crrLobbySelector);

                Point pointMainLobbyOnHighConrast = FindBmp(bmpLobbySelectorHighContrast, _bmpMainLobbyOnHighConrast, 1);
                if (!pointMainLobbyOnHighConrast.IsValid())
                {
                    Point pointMainLobbyOffHighConrast = FindBmp(bmpLobbySelectorHighContrast, _bmpMainLobbyOffHighConrast, 1);
                    if (pointMainLobbyOffHighConrast.IsValid())
                    {
                        Methods.MouseEnterLeftMouseClickMouseLeave(handleLobbySelector,
                            pointMainLobbyOffHighConrast.X + _bmpMainLobbyOffHighConrast.Width / 2,
                            pointMainLobbyOffHighConrast.Y + _bmpMainLobbyOffHighConrast.Height / 2);
                    }
                    else
                    {
                        throw new NotSupportedException("Cannot find 'Main Lobby' button.");
                    }
                }

                Thread.Sleep(100);
                // break;
            }
            //});
            //_thread.Start();
        }

        //

        public static Point FindBmp(Bmp bmpSource, Bmp bmpTarget, double accuracy, int xSourceStarting = 0, int ySourceStarting = 0)
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

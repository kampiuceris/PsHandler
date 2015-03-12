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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;
using PsHandler.Custom;
using PsHandler.Import;

namespace PsHandler.SngRegistrator
{
    public class SngRegistratorManager
    {
        private Thread _thread;
        private IntPtr _handleWindowPokerStarsLobby;
        private IntPtr _handleSelectorLobbySelector;
        private IntPtr _handleSelectorMainLobbySelector;
        private IntPtr _handleSelectorMainLobbyTabSelectorSitgo;
        private IntPtr _handleButtonGamesView;
        private IntPtr _handleButtonSngTournamentFilter;
        private readonly Bmp _bmpMainLobbyOn;
        private readonly Bmp _bmpMainLobbyOff;
        private readonly Bmp _bmpMainLobbyOffHover;
        private readonly Bmp _bmpMainLobbyPressed;
        private readonly Bmp _bmpSngOn;
        private readonly Bmp _bmpSngOff;
        private readonly Bmp _bmpSngOffHover;
        private readonly Bmp _bmpSngOffPressed;
        private readonly Bmp _bmpSngAllOn;
        private readonly Bmp _bmpSngAllOff;
        private readonly Bmp _bmpSngAllOffHover;
        private readonly Bmp _bmpSngAllOffPressed;
        private readonly Bmp _bmpTransparentGamesViewOn;
        private readonly Bmp _bmpTransparentGamesViewOnPressed;
        private readonly Bmp _bmpTransparentGamesViewOff;
        private readonly Bmp _bmpTransparentGamesViewOffHover;
        private readonly Bmp _bmpTransparentGamesViewOffPressed;
        private readonly SngTournamentFilterManager SngTournamentFilterManager = new SngTournamentFilterManager();

        public SngRegistratorManager()
        {
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

            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.sng_on.png"))
            {
                _bmpSngOn = new Bmp(bitmap);
            }
            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.sng_off.png"))
            {
                _bmpSngOff = new Bmp(bitmap);
            }
            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.sng_off_hover.png"))
            {
                _bmpSngOffHover = new Bmp(bitmap);
            }
            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.sng_off_pressed.png"))
            {
                _bmpSngOffPressed = new Bmp(bitmap);
            }

            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.sng_all_on.png"))
            {
                _bmpSngAllOn = new Bmp(bitmap);
            }
            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.sng_all_off.png"))
            {
                _bmpSngAllOff = new Bmp(bitmap);
            }
            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.sng_all_off_hover.png"))
            {
                _bmpSngAllOffHover = new Bmp(bitmap);
            }
            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.sng_all_off_pressed.png"))
            {
                _bmpSngAllOffPressed = new Bmp(bitmap);
            }

            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.t_games_view_on.png"))
            {
                _bmpTransparentGamesViewOn = new Bmp(bitmap);
            }
            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.t_games_view_on_pressed.png"))
            {
                _bmpTransparentGamesViewOnPressed = new Bmp(bitmap);
            }
            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.t_games_view_off.png"))
            {
                _bmpTransparentGamesViewOff = new Bmp(bitmap);
            }
            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.t_games_view_off_hover.png"))
            {
                _bmpTransparentGamesViewOffHover = new Bmp(bitmap);
            }
            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.t_games_view_off_pressed.png"))
            {
                _bmpTransparentGamesViewOffPressed = new Bmp(bitmap);
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

            SetHandles();

            //_thread = new Thread(() =>
            //{
            while (true)
            {
                EnsureGamesViewOn();
                SngTournamentFilterManager.EnsureSngTournamentFilterOn(_handleWindowPokerStarsLobby, _handleButtonSngTournamentFilter, EnsureSitAndGoAllOn);
                SngTournamentFilterManager.Test();

                //Methods.DisplayBitmap(GetBmpWindowPokerStarsLobby().CutRectangle(WinApi.GetClientRectangleRelativeTo(_handleQuickFilter, _handleWindowPokerStarsLobby)).ToBitmap(), true);

                Thread.Sleep(100);
            }
            //});
            //_thread.Start();
        }

        //

        private void SetHandles()
        {
            _handleWindowPokerStarsLobby = WinApi.FindWindow("#32770", "PokerStars Lobby");
            if (_handleWindowPokerStarsLobby.Equals(IntPtr.Zero)) throw new NotSupportedException("Cannot find Window 'PokerStars Lobby'.");
            _handleSelectorLobbySelector = WinApi.FindChildWindow(_handleWindowPokerStarsLobby, "PokerStarsSelectorClass", "lobby-selector");
            if (_handleSelectorLobbySelector.Equals(IntPtr.Zero)) throw new NotSupportedException("Cannot find Selector 'Lobby Selector'.");
            _handleSelectorMainLobbySelector = WinApi.FindChildWindow(_handleWindowPokerStarsLobby, "PokerStarsSelectorClass", "main-lobby-selector");
            if (_handleSelectorMainLobbySelector.Equals(IntPtr.Zero)) throw new NotSupportedException("Cannot find Selector 'Main Lobby Selector'.");

            EnsureMainLobbyOn();
            EnsureSitAndGoOn();

            IntPtr[] possibleHandles = WinApi.FindAllChildWindow(_handleWindowPokerStarsLobby, "PokerStarsSelectorClass", "main-lobby-tab-selector-sitgo").Where(o => WinApi.IsWindowVisible(o)).ToArray();
            if (possibleHandles.Length != 1)
                throw new NotSupportedException("Cannot find Selector 'Main Lobby Tab Selector Sitgo'.");
            else
                _handleSelectorMainLobbyTabSelectorSitgo = possibleHandles[0];

            EnsureSitAndGoAllOn();
            SetHandleButtonGamesView();
            EnsureGamesViewOn();

            _handleButtonSngTournamentFilter = WinApi.FindChildWindow(_handleWindowPokerStarsLobby, "PokerStarsButtonClass", "Sit & Go Tournament Filter");
            if (_handleButtonSngTournamentFilter.Equals(IntPtr.Zero)) throw new NotSupportedException("Cannot find Button 'Sit & Go Tournament Filter'.");

            SngTournamentFilterManager.EnsureSngTournamentFilterOn(_handleWindowPokerStarsLobby, _handleButtonSngTournamentFilter, EnsureSitAndGoAllOn);
        }

        private void EnsureMainLobbyOn()
        {
            DateTime started = DateTime.Now;
            while (true)
            {
                if ((DateTime.Now - started).TotalSeconds > 5) throw new NotSupportedException("Cannot open 'Main Lobby'.");
                Thread.Sleep(100);

                if (Methods.IsMinimized(_handleWindowPokerStarsLobby))
                {
                    WinApi.ShowWindow(_handleWindowPokerStarsLobby, WinApi.SW_RESTORE);
                }

                Bmp bmp = GetBmpWindowPokerStarsLobby().CutRectangle(WinApi.GetClientRectangleRelativeTo(_handleSelectorLobbySelector, _handleWindowPokerStarsLobby));

                Point point;
                point = FindBmp(bmp, _bmpMainLobbyOn);
                if (point.IsValid())
                {
                    return;
                }
                point = FindBmp(bmp, _bmpMainLobbyOff);
                if (point.IsValid())
                {
                    Methods.MouseEnterLeftMouseClickMouseLeave(_handleSelectorLobbySelector, point.X, point.Y);
                    continue;
                }
                point = FindBmp(bmp, _bmpMainLobbyOffHover);
                if (point.IsValid())
                {
                    Methods.MouseEnterLeftMouseClickMouseLeave(_handleSelectorLobbySelector, point.X, point.Y);
                    continue;
                }
                point = FindBmp(bmp, _bmpMainLobbyPressed);
                if (point.IsValid())
                {
                    Methods.MouseEnterLeftMouseClickMouseLeave(_handleSelectorLobbySelector, point.X, point.Y);
                    continue;
                }
            }
        }

        private void EnsureSitAndGoOn()
        {
            DateTime started = DateTime.Now;
            while (true)
            {
                if ((DateTime.Now - started).TotalSeconds > 5) throw new NotSupportedException("Cannot open 'Sit & Go'.");
                Thread.Sleep(100);

                if (!WinApi.IsWindowVisible(_handleSelectorMainLobbySelector))
                {
                    EnsureMainLobbyOn();
                    continue;
                }

                Bmp bmp = GetBmpWindowPokerStarsLobby().CutRectangle(WinApi.GetClientRectangleRelativeTo(_handleSelectorMainLobbySelector, _handleWindowPokerStarsLobby));

                Point point;
                point = FindBmp(bmp, _bmpSngOn, 0.95, 0.95);
                if (point.IsValid())
                {
                    return;
                }
                point = FindBmp(bmp, _bmpSngOff, 0.95, 0.95);
                if (point.IsValid())
                {
                    Methods.MouseEnterLeftMouseClickMouseLeave(_handleSelectorMainLobbySelector, point.X, point.Y);
                    continue;
                }
                point = FindBmp(bmp, _bmpSngOffHover, 0.95, 0.95);
                if (point.IsValid())
                {
                    Methods.MouseEnterLeftMouseClickMouseLeave(_handleSelectorMainLobbySelector, point.X, point.Y);
                    continue;
                }
                point = FindBmp(bmp, _bmpSngOffPressed, 0.95, 0.95);
                if (point.IsValid())
                {
                    Methods.MouseEnterLeftMouseClickMouseLeave(_handleSelectorMainLobbySelector, point.X, point.Y);
                    continue;
                }
            }
        }

        private void EnsureSitAndGoAllOn()
        {
            DateTime started = DateTime.Now;
            while (true)
            {
                if ((DateTime.Now - started).TotalSeconds > 5) throw new NotSupportedException("Cannot open 'Sit & Go -> All' tab.");
                Thread.Sleep(100);

                if (!WinApi.IsWindowVisible(_handleSelectorMainLobbyTabSelectorSitgo))
                {
                    EnsureSitAndGoOn();
                    continue;
                }

                Bmp bmp = GetBmpWindowPokerStarsLobby().CutRectangle(WinApi.GetClientRectangleRelativeTo(_handleSelectorMainLobbyTabSelectorSitgo, _handleWindowPokerStarsLobby));

                Point point;
                point = FindBmp(bmp, _bmpSngAllOn, 0.95, 0.95);
                if (point.IsValid())
                {
                    return;
                }
                point = FindBmp(bmp, _bmpSngAllOff, 0.95, 0.95);
                if (point.IsValid())
                {
                    Methods.MouseEnterLeftMouseClickMouseLeave(_handleSelectorMainLobbyTabSelectorSitgo, point.X, point.Y);
                    continue;
                }
                point = FindBmp(bmp, _bmpSngAllOffHover, 0.95, 0.95);
                if (point.IsValid())
                {
                    Methods.MouseEnterLeftMouseClickMouseLeave(_handleSelectorMainLobbyTabSelectorSitgo, point.X, point.Y);
                    continue;
                }
                point = FindBmp(bmp, _bmpSngAllOffPressed, 0.95, 0.95);
                if (point.IsValid())
                {
                    Methods.MouseEnterLeftMouseClickMouseLeave(_handleSelectorMainLobbyTabSelectorSitgo, point.X, point.Y);
                    continue;
                }
            }
        }

        private void SetHandleButtonGamesView()
        {
            if (_handleButtonGamesView == IntPtr.Zero)
            {
                Thread.Sleep(100);
                _handleButtonGamesView = FindElementInPokerStarsLobby("PokerStarsButtonClass", "", true,
                       new Size(_bmpTransparentGamesViewOn.Width, _bmpTransparentGamesViewOn.Height), 1.0,
                       new Rectangle(35, 160, 70, 50), 1,
                       new List<Bmp> { _bmpTransparentGamesViewOn, _bmpTransparentGamesViewOnPressed, _bmpTransparentGamesViewOff, _bmpTransparentGamesViewOffHover, _bmpTransparentGamesViewOffPressed }, 0.99, 0.99);
            }
            if (_handleButtonGamesView == IntPtr.Zero)
            {
                throw new NotSupportedException("Cannot find 'Games View' button.");
            }
        }

        private void EnsureGamesViewOn()
        {
            DateTime started = DateTime.Now;
            while (true)
            {
                if ((DateTime.Now - started).TotalSeconds > 5) throw new NotSupportedException("Ensure 'Games View' timeout.");
                Thread.Sleep(100);

                if (!WinApi.IsWindowVisible(_handleButtonGamesView))
                {
                    EnsureSitAndGoAllOn();
                    continue;
                }

                Bmp bmp = GetBmpWindowPokerStarsLobby().CutRectangle(WinApi.GetClientRectangleRelativeTo(_handleButtonGamesView, _handleWindowPokerStarsLobby));

                if (FindBmp(bmp, _bmpTransparentGamesViewOn, 0.99, 0.99).IsValid())
                {
                    return;
                }
                if (FindBmp(bmp, _bmpTransparentGamesViewOnPressed, 0.99, 0.99).IsValid())
                {
                    Methods.MouseEnterLeftMouseClickMiddleMouseLeave(_handleButtonGamesView);
                }
                else if (FindBmp(bmp, _bmpTransparentGamesViewOff, 0.99, 0.99).IsValid())
                {
                    Methods.MouseEnterLeftMouseClickMiddleMouseLeave(_handleButtonGamesView);
                }
                else if (FindBmp(bmp, _bmpTransparentGamesViewOffHover, 0.99, 0.99).IsValid())
                {
                    Methods.MouseEnterLeftMouseClickMiddleMouseLeave(_handleButtonGamesView);
                }
                else if (FindBmp(bmp, _bmpTransparentGamesViewOffPressed, 0.99, 0.99).IsValid())
                {
                    Methods.MouseEnterLeftMouseClickMiddleMouseLeave(_handleButtonGamesView);
                }
            }
        }

        //

        private Bmp GetBmpWindowPokerStarsLobby()
        {
            Bmp bmpPokerStarsLobby;
            using (Bitmap bitmap = ScreenCapture.GetBitmapWindowClient(_handleWindowPokerStarsLobby))
            {
                bmpPokerStarsLobby = new Bmp(bitmap);
            }
            return bmpPokerStarsLobby;
        }


        private IntPtr FindElementInPokerStarsLobby(string className, string title, bool isVisible, Size size, double sizeAccuracy, Rectangle areaToSearch, double areaToSearchAccuracy, List<Bmp> bmps, double bmpsMatchAccuracy, double bmpsColorAccuracy)
        {
            IEnumerable<IntPtr> findComponentsInPokerStarsLobby = FindElementsInPokerStarsLobby(className, title, isVisible, size, sizeAccuracy, areaToSearch, areaToSearchAccuracy, bmps, bmpsMatchAccuracy, bmpsColorAccuracy);
            if (findComponentsInPokerStarsLobby.Count() == 1)
            {
                return findComponentsInPokerStarsLobby.ElementAt(0);
            }
            return IntPtr.Zero;
            //else if (!findComponentsInPokerStarsLobby.Any())
            //{
            //    return IntPtr.Zero;
            //}
            //else
            //{
            //    return IntPtr.Zero;
            //}
        }

        private IEnumerable<IntPtr> FindElementsInPokerStarsLobby(string className, string title, bool isVisible, Size size, double sizeAccuracy, Rectangle areaToSearch, double areaToSearchAccuracy, List<Bmp> bmps, double bmpsMatchAccuracy, double bmpsColorAccuracy)
        {
            List<IntPtr> possibleComponents = new List<IntPtr>();
            Bmp bmpWindowPokerStarsLobby = GetBmpWindowPokerStarsLobby();

            foreach (IntPtr handle in WinApi.FindAllChildWindowByClass(_handleWindowPokerStarsLobby, className).Where(o => WinApi.IsWindowVisible(o) == isVisible))
            {
                string rawTitle = WinApi.GetWindowTextRaw(handle);
                if ((string.IsNullOrEmpty(title) && string.IsNullOrEmpty(rawTitle)) || (!string.IsNullOrEmpty(title) && rawTitle.Equals(title)))
                {
                    Rectangle r = WinApi.GetClientRectangleRelativeTo(handle, _handleWindowPokerStarsLobby);
                    if (Math.Abs(size.Width - r.Width) <= size.Width - size.Width * sizeAccuracy && Math.Abs(size.Height - r.Height) <= size.Height - size.Height * sizeAccuracy)
                    {
                        if (areaToSearch.Left - (bmpWindowPokerStarsLobby.Width - bmpWindowPokerStarsLobby.Width * areaToSearchAccuracy) <= r.Left
                            && areaToSearch.Top - (bmpWindowPokerStarsLobby.Height - bmpWindowPokerStarsLobby.Height * areaToSearchAccuracy) <= r.Top
                            && areaToSearch.Right + (bmpWindowPokerStarsLobby.Width - bmpWindowPokerStarsLobby.Width * areaToSearchAccuracy) >= r.Right
                            && areaToSearch.Bottom + (bmpWindowPokerStarsLobby.Height - bmpWindowPokerStarsLobby.Height * areaToSearchAccuracy) >= r.Bottom)
                        {
                            Bmp cutBmp = bmpWindowPokerStarsLobby.CutRectangle(r);
                            foreach (Bmp bmp in bmps)
                            {
                                Point point = FindBmp(cutBmp, bmp, bmpsMatchAccuracy, bmpsColorAccuracy);
                                if (point.X == 0 && point.Y == 0)
                                {
                                    possibleComponents.Add(handle);
                                }
                            }
                        }
                    }
                }
            }

            return possibleComponents.Distinct();
        }

        private static Point FindBmp(Bmp bmpSource, Bmp bmpTarget, double matchAccuracy = 1.0, double colorAccuracy = 1.0, int xSourceStarting = 0, int ySourceStarting = 0)
        {
            int totalPixelsToCheck = bmpTarget.Width * bmpTarget.Height;
            int minPixelsToMatch = (int)Math.Round(totalPixelsToCheck * matchAccuracy);
            int maxPixelsToMismatch = (int)Math.Round(totalPixelsToCheck * (1.0 - matchAccuracy));
            int maxColorError = (int)Math.Round(byte.MaxValue - (byte.MaxValue * colorAccuracy));

            for (int ySource = xSourceStarting; ySource < bmpSource.Height; ySource++)
            {
                for (int xSource = ySourceStarting; xSource < bmpSource.Width; xSource++)
                {
                    if (BmpSourceFitsBmpTarget(bmpSource, xSource, ySource, bmpTarget, minPixelsToMatch, maxPixelsToMismatch, maxColorError))
                    {
                        return new Point(xSource, ySource);
                    }
                }
            }
            return new Point(int.MinValue, int.MinValue);
        }

        private static bool BmpSourceFitsBmpTarget(Bmp bmpSource, int xSource, int ySource, Bmp bmpTarget, int minPixelsToMatch, int maxPixelsToMismatch, int maxColorError)
        {
            if (xSource + bmpTarget.Width > bmpSource.Width || ySource + bmpTarget.Height > bmpSource.Height) return false;


            int transparent = 0, pixelsMatched = 0, pixelsMismatched = 0;
            for (int yTarget = 0; yTarget < bmpTarget.Height; yTarget++)
            {
                for (int xTarget = 0; xTarget < bmpTarget.Width; xTarget++)
                {
                    int colorSourceR = bmpSource.GetPixelR(xSource + xTarget, ySource + yTarget);
                    int colorSourceG = bmpSource.GetPixelG(xSource + xTarget, ySource + yTarget);
                    int colorSourceB = bmpSource.GetPixelB(xSource + xTarget, ySource + yTarget);
                    int colorTargetA = bmpTarget.GetPixelA(xTarget, yTarget);
                    int colorTargetR = bmpTarget.GetPixelR(xTarget, yTarget);
                    int colorTargetG = bmpTarget.GetPixelG(xTarget, yTarget);
                    int colorTargetB = bmpTarget.GetPixelB(xTarget, yTarget);

                    if (colorTargetA == 0)
                    {
                        transparent++;
                    }
                    else if (colorSourceR - maxColorError <= colorTargetR && colorSourceR + maxColorError >= colorTargetR
                       && colorSourceG - maxColorError <= colorTargetG && colorSourceG + maxColorError >= colorTargetG
                       && colorSourceB - maxColorError <= colorTargetB && colorSourceB + maxColorError >= colorTargetB)
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
                    if (pixelsMatched + transparent >= minPixelsToMatch)
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

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
        private IntPtr _handleButtonExpand;
        private IntPtr _handleButtonCollapse;
        private IntPtr _handleButtonGamesView;
        private IntPtr _handleButtonFilter;
        private IntPtr _handleQuickFilter;
        private readonly Bmp _bmpMainLobbyOn;
        private readonly Bmp _bmpMainLobbyOff;
        private readonly Bmp _bmpMainLobbyOffHover;
        private readonly Bmp _bmpMainLobbyPressed;
        private readonly Bmp _bmpSngOn;
        private readonly Bmp _bmpSngOff;
        private readonly Bmp _bmpSngOffHover;
        private readonly Bmp _bmpSngOffPressed;
        private readonly Bmp _bmpTransparentExpandOn;
        private readonly Bmp _bmpTransparentExpandOnHover;
        private readonly Bmp _bmpTransparentExpandOnPressed;
        private readonly Bmp _bmpTransparentCollapseOn;
        private readonly Bmp _bmpTransparentCollapseOnHover;
        private readonly Bmp _bmpTransparentCollapseOnPressed;
        private readonly Bmp _bmpTransparentGamesViewOn;
        private readonly Bmp _bmpTransparentGamesViewOnPressed;
        private readonly Bmp _bmpTransparentGamesViewOff;
        private readonly Bmp _bmpTransparentGamesViewOffHover;
        private readonly Bmp _bmpTransparentGamesViewOffPressed;
        private readonly Bmp _bmpTransparentFilterOn;
        private readonly Bmp _bmpTransparentFilterOnHover;
        private readonly Bmp _bmpTransparentFilterOnPressed;
        private readonly Bmp _bmpTransparentFilterOff;
        private readonly Bmp _bmpTransparentFilterOffHover;
        private readonly Bmp _bmpTransparentFilterOffPressed;


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

            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.t_button_collapse_on.png"))
            {
                _bmpTransparentCollapseOn = new Bmp(bitmap);
            }
            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.t_button_collapse_on_hover.png"))
            {
                _bmpTransparentCollapseOnHover = new Bmp(bitmap);
            }
            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.t_button_collapse_on_pressed.png"))
            {
                _bmpTransparentCollapseOnPressed = new Bmp(bitmap);
            }
            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.t_button_expand_on.png"))
            {
                _bmpTransparentExpandOn = new Bmp(bitmap);
            }
            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.t_button_expand_on_hover.png"))
            {
                _bmpTransparentExpandOnHover = new Bmp(bitmap);
            }
            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.t_button_expand_on_pressed.png"))
            {
                _bmpTransparentExpandOnPressed = new Bmp(bitmap);
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

            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.t_filters_on.png"))
            {
                _bmpTransparentFilterOn = new Bmp(bitmap);
            }
            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.t_filters_on_hover.png"))
            {
                _bmpTransparentFilterOnHover = new Bmp(bitmap);
            }
            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.t_filters_on_pressed.png"))
            {
                _bmpTransparentFilterOnPressed = new Bmp(bitmap);
            }
            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.t_filters_off.png"))
            {
                _bmpTransparentFilterOff = new Bmp(bitmap);
            }
            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.t_filters_off_hover.png"))
            {
                _bmpTransparentFilterOffHover = new Bmp(bitmap);
            }
            using (Bitmap bitmap = Methods.GetEmbeddedResourceBitmap("PsHandler.Images.SngRegistrator.t_filters_off_pressed.png"))
            {
                _bmpTransparentFilterOffPressed = new Bmp(bitmap);
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
                //EnsureGamesView();
                //EnsureFiltersOff();

                //Methods.DisplayBitmap(GetBmpWindowPokerStarsLobby().CutRectangle(WinApi.GetClientRectangleRelativeTo(_handleQuickFilter, _handleWindowPokerStarsLobby)).ToBitmap(), true);
                EnsureMainLobby();
                EnsureSitAndGo();
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

            EnsureMainLobby();
            EnsureSitAndGo();
            //SetHandlesButtonsExpandColapse();
            //EnsureCollapsed();
            //SetHandleButtonGamesView();
            //SetHandleButtonFilters();
            //SetHandleQuickFilter();
        }

        private void EnsureMainLobby()
        {
            DateTime started = DateTime.Now;
            while (true)
            {
                Thread.Sleep(100);

                Bmp bmpSelectorLobbySelector = GetBmpWindowPokerStarsLobby().CutRectangle(WinApi.GetClientRectangleRelativeTo(_handleSelectorLobbySelector, _handleWindowPokerStarsLobby));
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

                if ((DateTime.Now - started).TotalSeconds > 5) throw new NotSupportedException("Cannot open 'Main Lobby'.");
            }
        }

        private void EnsureSitAndGo()
        {
            DateTime started = DateTime.Now;
            while (true)
            {
                Thread.Sleep(100);

                Bmp bmpSelectorMainLobbySelector = GetBmpWindowPokerStarsLobby().CutRectangle(WinApi.GetClientRectangleRelativeTo(_handleSelectorMainLobbySelector, _handleWindowPokerStarsLobby));

                Point point;
                // check if is on
                point = FindBmp(bmpSelectorMainLobbySelector, _bmpSngOn, 0.95, 0.95);
                if (point.IsValid())
                {
                    break;
                }
                // check if is off
                point = FindBmp(bmpSelectorMainLobbySelector, _bmpSngOff, 0.95, 0.95);
                if (point.IsValid())
                {
                    Methods.MouseEnterLeftMouseClickMouseLeave(_handleSelectorMainLobbySelector, point.X, point.Y);
                    continue;
                }
                // check if is hover
                point = FindBmp(bmpSelectorMainLobbySelector, _bmpSngOffHover, 0.95, 0.95);
                if (point.IsValid())
                {
                    Methods.MouseEnterLeftMouseClickMouseLeave(_handleSelectorMainLobbySelector, point.X, point.Y);
                    continue;
                }
                // check if is pressed
                point = FindBmp(bmpSelectorMainLobbySelector, _bmpSngOffPressed, 0.95, 0.95);
                if (point.IsValid())
                {
                    Methods.MouseEnterLeftMouseClickMouseLeave(_handleSelectorMainLobbySelector, point.X, point.Y);
                    continue;
                }

                if ((DateTime.Now - started).TotalSeconds > 5) throw new NotSupportedException("Cannot open 'Sit & Go'.");
            }
        }

        private void EnsureCollapsed()
        {
            DateTime started = DateTime.Now;

            while (!WinApi.IsWindowVisible(_handleButtonExpand))
            {
                Thread.Sleep(100);

                if (!WinApi.IsWindowVisible(_handleButtonCollapse))
                {
                    EnsureMainLobby();
                    EnsureSitAndGo();
                }

                if (WinApi.IsWindowVisible(_handleButtonCollapse))
                {
                    Methods.MouseEnterLeftMouseClickMiddleMouseLeave(_handleButtonCollapse);
                }
                else if (WinApi.IsWindowVisible(_handleButtonExpand))
                {
                    return;
                }
                else
                {
                    throw new NotSupportedException("Cannot ensure 'Collapsed' state.");
                }

                if ((DateTime.Now - started).TotalSeconds > 5) throw new NotSupportedException("Ensure 'Collapsed' state timeout.");
            }
        }

        private void EnsureMainLobbySitAndGoCollapsed()
        {
            EnsureMainLobby();
            EnsureSitAndGo();
            EnsureCollapsed();
        }


        //

        private void SetHandlesButtonsExpandColapse()
        {
            Thread.Sleep(100);

            _handleButtonExpand = FindButtonExpand();
            _handleButtonCollapse = FindButtonCollapse();
            if (_handleButtonExpand == IntPtr.Zero && _handleButtonCollapse == IntPtr.Zero)
            {
                throw new NotSupportedException("Cannot find buttons 'Expand'/'Collapse'.");
            }
            if (_handleButtonExpand != IntPtr.Zero)
            {
                Methods.MouseEnterLeftMouseClickMiddleMouseLeave(_handleButtonExpand);
                Thread.Sleep(100);
                _handleButtonCollapse = FindButtonCollapse();
                if (_handleButtonCollapse == IntPtr.Zero)
                {
                    throw new NotSupportedException("Cannot find button 'Collapse'.");
                }
                else
                {
                    Methods.MouseEnterLeftMouseClickMiddleMouseLeave(_handleButtonCollapse);
                }
            }
            else
            {
                Methods.MouseEnterLeftMouseClickMiddleMouseLeave(_handleButtonCollapse);
                Thread.Sleep(100);
                _handleButtonExpand = FindButtonExpand();
                if (_handleButtonExpand == IntPtr.Zero)
                {
                    throw new NotSupportedException("Cannot find button 'Expand'.");
                }
            }
        }

        private IntPtr FindButtonExpand()
        {
            return FindElementInPokerStarsLobby("PokerStarsButtonClass", "", true,
                       new Size(_bmpTransparentExpandOn.Width, _bmpTransparentExpandOn.Height), 1.0,
                       new Rectangle((WinApi.GetClientRectangle(_handleWindowPokerStarsLobby).Width - 60 - _bmpTransparentExpandOn.Width) / 2 - 20, 150, 140, 35), 1,
                       new List<Bmp> { _bmpTransparentExpandOn, _bmpTransparentExpandOnHover, _bmpTransparentExpandOnPressed }, 0.99, 0.99);
        }

        private IntPtr FindButtonCollapse()
        {
            return FindElementInPokerStarsLobby("PokerStarsButtonClass", "", true,
                       new Size(_bmpTransparentCollapseOn.Width, _bmpTransparentCollapseOn.Height), 1.0,
                       new Rectangle((WinApi.GetClientRectangle(_handleWindowPokerStarsLobby).Width - 60 - _bmpTransparentExpandOn.Width) / 2 - 20, 270, 140, 35), 1,
                       new List<Bmp> { _bmpTransparentCollapseOn, _bmpTransparentCollapseOnHover, _bmpTransparentCollapseOnPressed }, 0.99, 0.99);
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

        private void SetHandleButtonFilters()
        {
            if (_handleButtonFilter == IntPtr.Zero)
            {
                Thread.Sleep(100);
                _handleButtonFilter = FindElementInPokerStarsLobby("PokerStarsButtonClass", "", true,
                       new Size(_bmpTransparentFilterOn.Width, _bmpTransparentFilterOn.Height), 1.0,
                       new Rectangle(WinApi.GetClientRectangle(_handleWindowPokerStarsLobby).Width - 125, 120, 60, 50), 1,
                       new List<Bmp> { _bmpTransparentFilterOn, _bmpTransparentFilterOnHover, _bmpTransparentFilterOnPressed, _bmpTransparentFilterOff, _bmpTransparentFilterOffHover, _bmpTransparentFilterOffPressed }, 0.99, 0.99);
                if (_handleButtonFilter == IntPtr.Zero)
                {
                    throw new NotSupportedException("Cannot find 'Filters' button");
                }
            }
        }

        private void SetHandleQuickFilter()
        {
            if (_handleQuickFilter == IntPtr.Zero)
            {
                Thread.Sleep(100);
                var handles = WinApi.FindAllChildWindowByClass(_handleWindowPokerStarsLobby, "PokerStarsFilterClass").Where(o => WinApi.IsWindowVisible(o) && WinApi.GetClientRectangle(o).Size.Equals(new Size(105, 15))).ToArray();
                if (handles.Length == 1)
                {
                    _handleQuickFilter = handles[0];
                }
                else
                {
                    throw new NotSupportedException("Cannot find 'Quick Filter' textbox.");
                }
            }
        }

        //

        private void EnsureGamesView()
        {
            DateTime started = DateTime.Now;
            while (true)
            {
                Thread.Sleep(100);
                if (!WinApi.IsWindowVisible(_handleButtonGamesView))
                {
                    EnsureMainLobbySitAndGoCollapsed();
                    continue;
                }

                Bmp bmp = GetBmpWindowPokerStarsLobby().CutRectangle(WinApi.GetClientRectangleRelativeTo(_handleButtonGamesView, _handleWindowPokerStarsLobby));

                if (FindBmp(bmp, _bmpTransparentGamesViewOn, 0.99, 0.99).IsValid())
                {
                    return;
                }
                else if (FindBmp(bmp, _bmpTransparentGamesViewOnPressed, 0.99, 0.99).IsValid())
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

                if ((DateTime.Now - started).TotalSeconds > 5) throw new NotSupportedException("Ensure 'Games View' timeout.");
            }
        }

        private void EnsureFiltersOff()
        {
            DateTime started = DateTime.Now;
            while (true)
            {
                Thread.Sleep(100);
                if (!WinApi.IsWindowVisible(_handleButtonFilter))
                {
                    EnsureMainLobbySitAndGoCollapsed();
                    continue;
                }

                Bmp bmp = GetBmpWindowPokerStarsLobby().CutRectangle(WinApi.GetClientRectangleRelativeTo(_handleButtonFilter, _handleWindowPokerStarsLobby));

                if (FindBmp(bmp, _bmpTransparentFilterOff, 0.99, 0.99).IsValid())
                {
                    return;
                }
                else if (FindBmp(bmp, _bmpTransparentFilterOffHover, 0.99, 0.99).IsValid())
                {
                    return;
                }
                else if (FindBmp(bmp, _bmpTransparentFilterOffPressed, 0.99, 0.99).IsValid())
                {
                    return;
                }
                else if (FindBmp(bmp, _bmpTransparentFilterOn, 0.99, 0.99).IsValid())
                {
                    Methods.MouseEnterLeftMouseClickMiddleMouseLeave(_handleButtonFilter);
                }
                else if (FindBmp(bmp, _bmpTransparentFilterOnHover, 0.99, 0.99).IsValid())
                {
                    Methods.MouseEnterLeftMouseClickMiddleMouseLeave(_handleButtonFilter);
                }
                else if (FindBmp(bmp, _bmpTransparentFilterOnPressed, 0.99, 0.99).IsValid())
                {
                    Methods.MouseEnterLeftMouseClickMiddleMouseLeave(_handleButtonFilter);
                }

                if ((DateTime.Now - started).TotalSeconds > 5) throw new NotSupportedException("Ensure 'Filters off' timeout.");
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

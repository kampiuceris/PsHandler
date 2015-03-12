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
using System.Windows.Forms;
using System.Windows.Input;
using PsHandler.Custom;
using PsHandler.Import;

namespace PsHandler.SngRegistrator
{
    public class SngTournamentFilterManager
    {
        public IntPtr HandleWindowSngTournamentFilter;
        public bool HandlesOfChildren;

        #region Handles

        public IntPtr ButtonGameHoldem;
        public IntPtr ButtonGameOmaha;
        public IntPtr ButtonGameOmahaHiLo;
        public IntPtr ButtonGame5CardOmaha;
        public IntPtr ButtonGame5CardOmahaHiLo;
        public IntPtr ButtonGameCourchevel;
        public IntPtr ButtonGameCourchevelHiLo;
        public IntPtr ButtonGameStud;
        public IntPtr ButtonGameStudHiLo;
        public IntPtr ButtonGameRazz;
        public IntPtr ButtonGame5CardDraw;
        public IntPtr ButtonGame27TripleDraw;
        public IntPtr ButtonGame27SingleDraw;
        public IntPtr ButtonGameBadugi;

        public IntPtr ButtonMixedHorse;
        public IntPtr ButtonMixedHose;
        public IntPtr ButtonMixedMixedHoldem;
        public IntPtr ButtonMixedMixedOmahaHiLo;
        public IntPtr ButtonMixedMixedOmaha;
        public IntPtr ButtonMixed8Game;
        public IntPtr ButtonMixedHoldemOmaha;
        public IntPtr ButtonMixedTripleStud;

        public IntPtr ButtonLimitNoLimit;
        public IntPtr ButtonLimitPotLimit;
        public IntPtr ButtonLimitFixedLimit;

        public IntPtr ButtonBuyInMin;
        public IntPtr ButtonBuyInMax;
        public IntPtr ButtonBuyInCash;
        public IntPtr ButtonBuyInFpp;
        public IntPtr ButtonBuyInPlayMoney;

        public IntPtr ButtonSpeedRegular;
        public IntPtr ButtonSpeedTurbo;
        public IntPtr ButtonSpeedHyperTurbo;
        public IntPtr ButtonSpeedSlow;

        public IntPtr ButtonCurrencyUsDollars;
        public IntPtr ButtonCurrencyEuro;
        public IntPtr ButtonCurrencyBritishPounds;
        public IntPtr ButtonCurrencyCanadianDollars;

        public IntPtr ButtonFormatVariantRegular;
        public IntPtr ButtonFormatVariantRebuy;
        public IntPtr ButtonFormatVariantKnockout;
        public IntPtr ButtonFormatVariantShootout;
        public IntPtr ButtonFormatVariantFifty50;
        public IntPtr ButtonFormatVariantSatellite;
        public IntPtr ButtonFormatVariantSteps;

        public IntPtr ButtonPlayersPerTable21On1;
        public IntPtr ButtonPlayersPerTable4Max;
        public IntPtr ButtonPlayersPerTable6Max;
        public IntPtr ButtonPlayersPerTable7To9;
        public IntPtr ButtonPlayersPerTable10;

        public IntPtr ButtonStateMinPlayersEnroller;
        public IntPtr ButtonStateMinTotalEntrants;
        public IntPtr ButtonStateMaxTotalEntrants;
        public IntPtr ButtonStateShowAllRegistering;
        public IntPtr ButtonStateShowRunning;
        public IntPtr ButtonStateShowCompleted;

        #endregion

        public IntPtr ComboBoxBuyInMin;
        public IntPtr EditBuyInMin;
        public IntPtr ComboBoxBuyInMax;
        public IntPtr EditBuyInMax;
        public IntPtr ComboBoxStateMinPlayersEnrolled;
        public IntPtr EditStateMinPlayersEnrolled;
        public IntPtr ComboBoxStateMinTotalEntrants;
        public IntPtr EditStateMinTotalEntrants;
        public IntPtr ComboBoxStateMaxTotalEntrants;
        public IntPtr EditStateMaxTotalEntrants;

        #region States

        public bool StateButtonGameHoldem;
        public bool StateButtonGameOmaha;
        public bool StateButtonGameOmahaHiLo;
        public bool StateButtonGame5CardOmaha;
        public bool StateButtonGame5CardOmahaHiLo;
        public bool StateButtonGameCourchevel;
        public bool StateButtonGameCourchevelHiLo;
        public bool StateButtonGameStud;
        public bool StateButtonGameStudHiLo;
        public bool StateButtonGameRazz;
        public bool StateButtonGame5CardDraw;
        public bool StateButtonGame27TripleDraw;
        public bool StateButtonGame27SingleDraw;
        public bool StateButtonGameBadugi;

        public bool StateButtonMixedHorse;
        public bool StateButtonMixedHose;
        public bool StateButtonMixedMixedHoldem;
        public bool StateButtonMixedMixedOmahaHiLo;
        public bool StateButtonMixedMixedOmaha;
        public bool StateButtonMixed8Game;
        public bool StateButtonMixedHoldemOmaha;
        public bool StateButtonMixedTripleStud;

        public bool StateButtonLimitNoLimit;
        public bool StateButtonLimitPotLimit;
        public bool StateButtonLimitFixedLimit;

        public bool StateButtonBuyInMin;
        public bool StateButtonBuyInMax;
        public bool StateButtonBuyInCash;
        public bool StateButtonBuyInFpp;
        public bool StateButtonBuyInPlayMoney;

        public bool StateButtonSpeedRegular;
        public bool StateButtonSpeedTurbo;
        public bool StateButtonSpeedHyperTurbo;
        public bool StateButtonSpeedSlow;

        public bool StateButtonCurrencyUsDollars;
        public bool StateButtonCurrencyEuro;
        public bool StateButtonCurrencyBritishPounds;
        public bool StateButtonCurrencyCanadianDollars;

        public bool StateButtonFormatVariantRegular;
        public bool StateButtonFormatVariantRebuy;
        public bool StateButtonFormatVariantKnockout;
        public bool StateButtonFormatVariantShootout;
        public bool StateButtonFormatVariantFifty50;
        public bool StateButtonFormatVariantSatellite;
        public bool StateButtonFormatVariantSteps;

        public bool StateButtonPlayersPerTable21On1;
        public bool StateButtonPlayersPerTable4Max;
        public bool StateButtonPlayersPerTable6Max;
        public bool StateButtonPlayersPerTable7To9;
        public bool StateButtonPlayersPerTable10;

        public bool StateButtonStateMinPlayersEnroller;
        public bool StateButtonStateMinTotalEntrants;
        public bool StateButtonStateMaxTotalEntrants;
        public bool StateButtonStateShowAllRegistering;
        public bool StateButtonStateShowRunning;
        public bool StateButtonStateShowCompleted;

        #endregion

        public static IntPtr GetWindowSngTournamentFilter(IntPtr handleWindowPokerStarsLobby)
        {
            uint processId;
            WinApi.GetWindowThreadProcessId(handleWindowPokerStarsLobby, out processId);
            foreach (IntPtr handle in WinApi.EnumerateProcessWindowHandles((int)processId).Where(o => WinApi.GetClassName(o).Equals("#32770") && WinApi.IsWindowVisible(o) && WinApi.GetWindowTitle(o).Equals("")))
            {
                if (!WinApi.FindChildWindow(handle, "PokerStarsButtonClass", "Fifty50").Equals(IntPtr.Zero)
                    && !WinApi.FindChildWindow(handle, "PokerStarsButtonClass", "6-Max").Equals(IntPtr.Zero)
                    && !WinApi.FindChildWindow(handle, "PokerStarsButtonClass", "FPP").Equals(IntPtr.Zero)
                    && !WinApi.FindChildWindow(handle, "PokerStarsButtonClass", "Turbo").Equals(IntPtr.Zero))
                {
                    return handle;
                }
            }
            return IntPtr.Zero;
        }

        public void EnsureSngTournamentFilterOn(IntPtr handleWindowPokerStarsLobby, IntPtr handleButtonSngTournamentFilter, Action ensureSitAndGoAllOn)
        {
            DateTime started = DateTime.Now;
            while (true)
            {
                if (!WinApi.IsWindowVisible(HandleWindowSngTournamentFilter))
                {
                    HandleWindowSngTournamentFilter = IntPtr.Zero;
                    HandlesOfChildren = false;
                }
                if (!HandleWindowSngTournamentFilter.Equals(IntPtr.Zero))
                {
                    if (!HandlesOfChildren)
                    {
                        SetHandles();
                        HandlesOfChildren = true;
                    }
                    return;
                }

                if ((DateTime.Now - started).TotalSeconds > 5) throw new NotSupportedException("Cannot find/open Window 'Sit & Go Tournament Filter'.");
                Thread.Sleep(100);

                if (!WinApi.IsWindowVisible(handleButtonSngTournamentFilter))
                {
                    ensureSitAndGoAllOn.Invoke();
                    continue;
                }

                HandleWindowSngTournamentFilter = GetWindowSngTournamentFilter(handleWindowPokerStarsLobby);
                if (HandleWindowSngTournamentFilter.Equals(IntPtr.Zero))
                {
                    Methods.MouseEnterLeftMouseClickMiddleMouseLeave(handleButtonSngTournamentFilter);
                    Thread.Sleep(100);
                    HandleWindowSngTournamentFilter = GetWindowSngTournamentFilter(handleWindowPokerStarsLobby);
                    continue;
                }
            }
        }

        public void Reset()
        {
            Methods.LeftMouseClick(HandleWindowSngTournamentFilter, WinApi.GetClientRectangle(HandleWindowSngTournamentFilter).Width - 75, 20);
        }

        private void SetHandles()
        {
            foreach (IntPtr handle in WinApi.FindAllChildWindowByClass(HandleWindowSngTournamentFilter, "PokerStarsButtonClass"))
            {
                string text = WinApi.GetWindowTextRaw(handle);

                #region by text

                // State
                if (text.Equals("Show completed"))
                {
                    ButtonStateShowCompleted = handle;
                }
                else if (text.Equals("Show running"))
                {
                    ButtonStateShowRunning = handle;
                }
                else if (text.Equals("Show all registering"))
                {
                    ButtonStateShowAllRegistering = handle;
                }
                // Players Per Table
                else if (text.Equals("10"))
                {
                    ButtonPlayersPerTable10 = handle;
                }
                else if (text.Equals("7 to 9"))
                {
                    ButtonPlayersPerTable7To9 = handle;
                }
                else if (text.Equals("6-Max"))
                {
                    ButtonPlayersPerTable6Max = handle;
                }
                else if (text.Equals("4-Max"))
                {
                    ButtonPlayersPerTable4Max = handle;
                }
                else if (text.Equals("2 (1-on-1)"))
                {
                    ButtonPlayersPerTable21On1 = handle;
                }
                // Format / Variant
                else if (text.Equals("Steps"))
                {
                    ButtonFormatVariantSteps = handle;
                }
                else if (text.Equals("Satellite"))
                {
                    ButtonFormatVariantSatellite = handle;
                }
                else if (text.Equals("Fifty50"))
                {
                    ButtonFormatVariantFifty50 = handle;
                }
                else if (text.Equals("Shootout"))
                {
                    ButtonFormatVariantShootout = handle;
                }
                else if (text.Equals("Knockout"))
                {
                    ButtonFormatVariantKnockout = handle;
                }
                else if (text.Equals("Rebuy"))
                {
                    ButtonFormatVariantRebuy = handle;
                }
                //else if (text.Equals("Regular"))
                //{
                //    ButtonFormatVariantRegular = handle;
                //}

                // Currency
                else if (text.Equals("Canadian Dollars"))
                {
                    ButtonCurrencyCanadianDollars = handle;
                }
                else if (text.Equals("British Pounds"))
                {
                    ButtonCurrencyBritishPounds = handle;
                }
                else if (text.Equals("Euro"))
                {
                    ButtonCurrencyEuro = handle;
                }
                else if (text.Equals("US Dollars"))
                {
                    ButtonCurrencyUsDollars = handle;
                }
                // Speed
                else if (text.Equals("Slow"))
                {
                    ButtonSpeedSlow = handle;
                }
                else if (text.Equals("Hyper-Turbo"))
                {
                    ButtonSpeedHyperTurbo = handle;
                }
                else if (text.Equals("Turbo"))
                {
                    ButtonSpeedTurbo = handle;
                }
                //else if (text.Equals("Regular"))
                //{
                //    ButtonSpeedRegular = handle;
                //}

                // Buy-In
                else if (text.Equals("Play Money"))
                {
                    ButtonBuyInPlayMoney = handle;
                }
                else if (text.Equals("FPP"))
                {
                    ButtonBuyInFpp = handle;
                }
                else if (text.Equals("Cash"))
                {
                    ButtonBuyInCash = handle;
                }
                // Limit
                else if (text.Equals("Fixed Limit"))
                {
                    ButtonLimitFixedLimit = handle;
                }
                else if (text.Equals("Pot Limit"))
                {
                    ButtonLimitPotLimit = handle;
                }
                else if (text.Equals("No Limit"))
                {
                    ButtonLimitNoLimit = handle;
                }
                // Mixed
                else if (text.Equals("Triple Stud"))
                {
                    ButtonMixedTripleStud = handle;
                }
                else if (text.Equals("Hold'em/Omaha"))
                {
                    ButtonMixedHoldemOmaha = handle;
                }
                else if (text.Equals("8-Game"))
                {
                    ButtonMixed8Game = handle;
                }
                else if (text.Equals("Mixed Omaha"))
                {
                    ButtonMixedMixedOmaha = handle;
                }
                else if (text.Equals("Mixed Omaha Hi/Lo"))
                {
                    ButtonMixedMixedOmahaHiLo = handle;
                }
                else if (text.Equals("Mixed Hold'em"))
                {
                    ButtonMixedMixedHoldem = handle;
                }
                else if (text.Equals("HOSE"))
                {
                    ButtonMixedHose = handle;
                }
                else if (text.Equals("HORSE"))
                {
                    ButtonMixedHorse = handle;
                }
                // Game
                else if (text.Equals("Badugi"))
                {
                    ButtonGameBadugi = handle;
                }
                else if (text.Equals("2-7 Single Draw"))
                {
                    ButtonGame27SingleDraw = handle;
                }
                else if (text.Equals("2-7 Triple Draw"))
                {
                    ButtonGame27TripleDraw = handle;
                }
                else if (text.Equals("5 Card Draw"))
                {
                    ButtonGame5CardDraw = handle;
                }
                else if (text.Equals("Razz"))
                {
                    ButtonGameRazz = handle;
                }
                else if (text.Equals("Stud Hi/Lo"))
                {
                    ButtonGameStudHiLo = handle;
                }
                else if (text.Equals("Stud"))
                {
                    ButtonGameStud = handle;
                }
                else if (text.Equals("Courchevel Hi/Lo"))
                {
                    ButtonGameCourchevelHiLo = handle;
                }
                else if (text.Equals("Courchevel"))
                {
                    ButtonGameCourchevel = handle;
                }
                else if (text.Equals("5 Card Omaha Hi/Lo"))
                {
                    ButtonGame5CardOmahaHiLo = handle;
                }
                else if (text.Equals("5 Card Omaha"))
                {
                    ButtonGame5CardOmaha = handle;
                }
                else if (text.Equals("Omaha Hi/Lo"))
                {
                    ButtonGameOmahaHiLo = handle;
                }
                else if (text.Equals("Omaha"))
                {
                    ButtonGameOmaha = handle;
                }
                else if (text.Equals("Hold'em"))
                {
                    ButtonGameHoldem = handle;
                }

                #endregion
            }

            // Speed.Regular + Format/Variant.Regular lef
            // textless buyin buttons left
            // buyin comboboxes + state comboboxes left

            // both "Regular"

            IntPtr nextHandle = WinApi.FindChildWindow(HandleWindowSngTournamentFilter, ButtonSpeedTurbo, "PokerStarsButtonClass", "");
            if (nextHandle.Equals(IntPtr.Zero) || !WinApi.GetWindowTextRaw(nextHandle).Equals("Regular"))
            {
                throw new NotSupportedException("Cannot find 'Speed' 'Regular' button.");
            }
            ButtonSpeedRegular = nextHandle;

            nextHandle = WinApi.FindChildWindow(HandleWindowSngTournamentFilter, ButtonFormatVariantRebuy, "PokerStarsButtonClass", "");
            if (nextHandle.Equals(IntPtr.Zero) || !WinApi.GetWindowTextRaw(nextHandle).Equals("Regular"))
            {
                throw new NotSupportedException("Cannot find 'Format / Variant' 'Regular' button.");
            }
            ButtonFormatVariantRegular = nextHandle;

            // min/max

            nextHandle = WinApi.FindChildWindow(HandleWindowSngTournamentFilter, ButtonBuyInCash, "PokerStarsButtonClass", "");
            if (nextHandle.Equals(IntPtr.Zero) || !WinApi.GetWindowTextRaw(nextHandle).Equals(""))
            {
                throw new NotSupportedException("Cannot find 'Buy-In' 'Max:' button.");
            }
            ButtonBuyInMax = nextHandle;

            nextHandle = WinApi.FindChildWindow(HandleWindowSngTournamentFilter, ButtonBuyInMax, "PokerStarsButtonClass", "");
            if (nextHandle.Equals(IntPtr.Zero) || !WinApi.GetWindowTextRaw(nextHandle).Equals(""))
            {
                throw new NotSupportedException("Cannot find 'Buy-In' 'Min:' button.");
            }
            ButtonBuyInMin = nextHandle;

            // checkboxes

            nextHandle = WinApi.FindChildWindow(HandleWindowSngTournamentFilter, ButtonGameHoldem, "ComboBox", "");
            if (nextHandle.Equals(IntPtr.Zero))
            {
                throw new NotSupportedException("Cannot find 'Buy-In' 'Min:' ComboBox.");
            }
            ComboBoxBuyInMin = nextHandle;

            nextHandle = WinApi.FindChildWindow(HandleWindowSngTournamentFilter, ComboBoxBuyInMin, "ComboBox", "");
            if (nextHandle.Equals(IntPtr.Zero))
            {
                throw new NotSupportedException("Cannot find 'Buy-In' 'Max:' ComboBox.");
            }
            ComboBoxBuyInMax = nextHandle;

            nextHandle = WinApi.FindChildWindow(HandleWindowSngTournamentFilter, ComboBoxBuyInMax, "ComboBox", "");
            if (nextHandle.Equals(IntPtr.Zero))
            {
                throw new NotSupportedException("Cannot find 'State' 'Min. players enrolled:' ComboBox.");
            }
            ComboBoxStateMinPlayersEnrolled = nextHandle;

            nextHandle = WinApi.FindChildWindow(HandleWindowSngTournamentFilter, ComboBoxStateMinPlayersEnrolled, "ComboBox", "");
            if (nextHandle.Equals(IntPtr.Zero))
            {
                throw new NotSupportedException("Cannot find 'State' 'Min. total entrants:' ComboBox.");
            }
            ComboBoxStateMinTotalEntrants = nextHandle;

            nextHandle = WinApi.FindChildWindow(HandleWindowSngTournamentFilter, ComboBoxStateMinTotalEntrants, "ComboBox", "");
            if (nextHandle.Equals(IntPtr.Zero))
            {
                throw new NotSupportedException("Cannot find 'State' 'Max. total entrants:' ComboBox.");
            }
            ComboBoxStateMaxTotalEntrants = nextHandle;

            // edit

            nextHandle = WinApi.FindChildWindow(ComboBoxBuyInMin, IntPtr.Zero, "Edit", "");
            if (nextHandle.Equals(IntPtr.Zero))
            {
                throw new NotSupportedException("Cannot find 'Buy-In' 'Min:' Edit.");
            }
            EditBuyInMin = nextHandle;

            nextHandle = WinApi.FindChildWindow(ComboBoxBuyInMax, IntPtr.Zero, "Edit", "");
            if (nextHandle.Equals(IntPtr.Zero))
            {
                throw new NotSupportedException("Cannot find 'Buy-In' 'Max:' Edit.");
            }
            EditBuyInMax = nextHandle;

            nextHandle = WinApi.FindChildWindow(ComboBoxStateMinPlayersEnrolled, IntPtr.Zero, "Edit", "");
            if (nextHandle.Equals(IntPtr.Zero))
            {
                throw new NotSupportedException("Cannot find 'State' 'Min. players enrolled:' Edit.");
            }
            EditStateMinPlayersEnrolled = nextHandle;

            nextHandle = WinApi.FindChildWindow(ComboBoxStateMinTotalEntrants, IntPtr.Zero, "Edit", "");
            if (nextHandle.Equals(IntPtr.Zero))
            {
                throw new NotSupportedException("Cannot find 'State' 'Min. total entrants:' Edit.");
            }
            EditStateMinTotalEntrants = nextHandle;

            nextHandle = WinApi.FindChildWindow(ComboBoxStateMaxTotalEntrants, IntPtr.Zero, "Edit", "");
            if (nextHandle.Equals(IntPtr.Zero))
            {
                throw new NotSupportedException("Cannot find 'State' 'Max. total entrants:' Edit.");
            }
            EditStateMaxTotalEntrants = nextHandle;
        }

        private void ClickCheckBox(IntPtr handle)
        {
            Methods.LeftMouseClick(handle, 4, 4);
        }

        private Bmp GetBmpWindowWindowSngTournamentFilter()
        {
            Bmp bmpPokerStarsLobby;
            using (Bitmap bitmap = ScreenCapture.GetBitmapWindowClient(HandleWindowSngTournamentFilter))
            {
                bmpPokerStarsLobby = new Bmp(bitmap);
            }
            return bmpPokerStarsLobby;
        }

        public void Test()
        {
            //Methods.DisplayBitmap(GetBmpWindowWindowSngTournamentFilter().CutRectangle(WinApi.GetClientRectangleRelativeTo(ButtonBuyInMax, HandleWindowSngTournamentFilter)).ToBitmap(), true);
            CheckButton(GetBmpWindowWindowSngTournamentFilter(), HandleWindowSngTournamentFilter, ButtonGameHoldem, false);
            //Reset();
            //Thread.Sleep(25);
            //ClickCheckBox(ButtonBuyInMax);
            //Thread.Sleep(25);
            //Methods.LeftMouseClick(EditBuyInMax, WinApi.GetClientRectangle(EditBuyInMax).Width - 10, 5);
            //Thread.Sleep(25);
            //WinApi.KeyType(EditBuyInMax, Key.D1);
            //WinApi.KeyType(EditBuyInMax, Key.D0);
            //WinApi.KeyType(EditBuyInMax, Key.D0);
        }

        private void CheckButton(Bmp bmp, IntPtr window, IntPtr button, bool isTextless)
        {
            Thread.Sleep(1000);

            Rectangle rectangleToCheck = WinApi.GetClientRectangleRelativeTo(button, window);
            rectangleToCheck = isTextless ? new Rectangle(rectangleToCheck.X + 2, rectangleToCheck.Y + 3, 10, 10) : new Rectangle(rectangleToCheck.X + 5, rectangleToCheck.Y + 4, 10, 10);

            double avgR, avgG, avgB;
            Methods.AverageColor(bmp, rectangleToCheck, out avgR, out avgG, out avgB);
            Debug.WriteLine(avgR + " " + avgG + " " + avgB);
        }
    }
}

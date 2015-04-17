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
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Interop;
using System.Xml.Linq;
using PsHandler.Custom;
using PsHandler.Import;

namespace PsHandler.TableTiler
{
    public class TableTileManager
    {
        // Table Tiles list controls

        private static readonly Regex _regexTournamentNumber = new Regex(@".+Tournament (?<tournament_number>\d+) .+");

        private readonly object _lockTableTiles = new object();
        private readonly List<TableTile> _tableTiles = new List<TableTile>();

        public TableTileManager()
        {
            Start();
        }

        public TableTile[] GetTableTilesCopy()
        {
            return _tableTiles.ToArray();
        }

        public void Add(TableTile tableTile)
        {
            lock (_lockTableTiles)
            {
                if (!_tableTiles.Any(o => o.Name.Equals(tableTile.Name)))
                {
                    _tableTiles.Add(tableTile);
                }
                _tableTiles.Sort((o0, o1) => string.CompareOrdinal(o0.Name, o1.Name));
            }
        }

        public void Add(IEnumerable<TableTile> tableTiles)
        {
            lock (_lockTableTiles)
            {
                foreach (var tableTile in tableTiles)
                {
                    if (!_tableTiles.Any(o => o.Name.Equals(tableTile.Name)))
                    {
                        _tableTiles.Add(tableTile);
                    }
                }
                _tableTiles.Sort((o0, o1) => string.CompareOrdinal(o0.Name, o1.Name));
            }
        }

        public void Remove(TableTile tableTile)
        {
            lock (_lockTableTiles)
            {
                _tableTiles.Remove(tableTile);
            }
        }

        public void RemoveAll()
        {
            lock (_lockTableTiles)
            {
                _tableTiles.Clear();
            }
        }

        public void SeedDefaultValues()
        {
            if (!_tableTiles.Any())
            {
                Add(TableTile.GetDefaultValues());
            }
        }

        //
        private bool _busy;
        private Thread _thread;
        // tile
        private KeyCombination _keyCombinationPressed;
        private readonly object _lockKeyCombination = new object();
        // auto tile
        private struct AutoTileWithTimestamp
        {
            public Table TableToAutoTile;
            public DateTime Added;
        }
        private readonly List<AutoTileWithTimestamp> _tablesToAutoTile = new List<AutoTileWithTimestamp>();
        private readonly object _lockAutoTile = new object();
        private struct HandleTitleClass
        {
            public IntPtr Handle;
            public string Title;
            public string Class;
        }

        public void SetKeyCombination(KeyCombination keyCombination)
        {
            if (!Config.EnableTableTiler) return;

            if (!_busy)
            {
                lock (_lockKeyCombination)
                {
                    _keyCombinationPressed = keyCombination;
                }
            }
        }

        public void AddAutoTileTable(Table newTable)
        {
            if (!Config.EnableTableTiler) return;

            lock (_lockAutoTile)
            {
                _tablesToAutoTile.RemoveAll(o => o.TableToAutoTile.Handle.Equals(newTable.Handle));
                _tablesToAutoTile.Add(new AutoTileWithTimestamp
                {
                    TableToAutoTile = newTable,
                    Added = DateTime.Now
                });
            }
        }

        public void RemoveAutoTileTable(Table newTable)
        {
            lock (_lockAutoTile)
            {
                _tablesToAutoTile.RemoveAll(o => o.TableToAutoTile.Handle.Equals(newTable.Handle));
            }
        }

        public void Start()
        {
            Stop();

            _thread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        Thread.Sleep(100);
                        lock (_lockKeyCombination)
                        {
                            if (_keyCombinationPressed != null)
                            {
                                _busy = true;
                                Tile(_keyCombinationPressed);
                                _keyCombinationPressed = null;
                                _busy = false;
                            }
                        }
                        lock (_lockAutoTile)
                        {
                            if (_tablesToAutoTile.Any())
                            {
                                List<AutoTileWithTimestamp> toRemove = new List<AutoTileWithTimestamp>();
                                foreach (var item in _tablesToAutoTile)
                                {
                                    bool autoTileSucessful = AutoTile(item.TableToAutoTile);
                                    if (autoTileSucessful)
                                    {
                                        toRemove.Add(item);
                                    }
                                }
                                toRemove.AddRange(_tablesToAutoTile.Where(o => o.Added.AddMilliseconds(Config.AutoTileCheckingTimeMs) < DateTime.Now));
                                foreach (var item in toRemove) _tablesToAutoTile.Remove(item);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (e is ThreadInterruptedException)
                        {
                            break;
                        }
                    }
                }
            });
            _thread.Start();
        }

        public void Stop()
        {
            if (_thread != null)
            {
                _thread.Interrupt();
            }
        }

        // private Tile mech



        private void Tile(KeyCombination kc)
        {
            // collect info

            List<TableTileAndTableInfos> ttatis = _tableTiles.Where(o => o.IsEnabled && o.KeyCombination.Equals(kc)).Select(tableTile => new TableTileAndTableInfos { TableTile = tableTile, TableInfos = new List<TableInfo>() }).ToList();

            if (ttatis.Any())
            {
                // get all windows info
                var windowsInfo = new List<HandleTitleClass>();
                foreach (var handle in WinApi.GetWindowHWndAll().Where(o => !Methods.IsMinimized(o)))
                {
                    windowsInfo.Add(new HandleTitleClass
                    {
                        Handle = handle,
                        Title = WinApi.GetWindowTitle(handle),
                        Class = WinApi.GetClassName(handle)
                    });
                }

                // checkif and tile
                foreach (var ttati in ttatis)
                {
                    // check how many tables match regextitle/regexclass
                    var windowsMatchRegexes = windowsInfo.Where(a => ttati.TableTile.RegexWindowClass.IsMatch(a.Class) && ttati.TableTile.RegexWindowTitle.IsMatch(a.Title)).ToArray();
                    if (windowsMatchRegexes.Any() && ttati.TableTile.TableCountEqualOrGreaterThan <= windowsMatchRegexes.Length && windowsMatchRegexes.Length <= ttati.TableTile.TableCountEqualOrLessThan)
                    {
                        foreach (var windowInfo in windowsMatchRegexes)
                        {
                            // table tile config has valid targets
                            TableInfo ti = new TableInfo { Handle = windowInfo.Handle, Title = windowInfo.Title, CurrentRectangle = WinApi.GetWindowRectangle(windowInfo.Handle) };
                            Match match = _regexTournamentNumber.Match(windowInfo.Title);
                            if (match.Success)
                            {
                                ti.IsTournament = long.TryParse(match.Groups["tournament_number"].Value, out ti.TournamentNumber);
                                Tournament tournament = App.HandHistoryManager.GetTournament(ti.TournamentNumber);
                                if (tournament != null)
                                {
                                    ti.FirstHandTimestamp = tournament.GetFirstHandTimestampET();
                                }
                                else
                                {
                                    ti.FirstHandTimestamp = DateTime.MaxValue;
                                }
                            }
                            else
                            {
                                ti.IsTournament = false;
                                ti.FirstHandTimestamp = DateTime.MaxValue;
                            }
                            ttati.TableInfos.Add(ti);
                        }
                    }
                }

                // obsolete
                //foreach (IntPtr hwnd in WinApi.GetWindowHWndAll().Where(o => !Methods.IsMinimized(o)))
                //{
                //    string title = WinApi.GetWindowTitle(hwnd);
                //    string windowClass = WinApi.GetClassName(hwnd);
                //    //Debug.WriteLine("Window: " + title);
                //    foreach (var ttati in ttatis)
                //    {
                //        if (ttati.TableTile.RegexWindowClass.IsMatch(windowClass) && ttati.TableTile.RegexWindowTitle.IsMatch(title))
                //        {
                //            TableInfo ti = new TableInfo { Handle = hwnd, Title = title, CurrentRectangle = WinApi.GetWindowRectangle(hwnd) };
                //            Match match = _regexTournamentNumber.Match(title);
                //            if (match.Success)
                //            {
                //                ti.IsTournament = long.TryParse(match.Groups["tournament_number"].Value, out ti.TournamentNumber);
                //                Tournament tournament = App.HandHistoryManager.GetTournament(ti.TournamentNumber);
                //                if (tournament != null)
                //                {
                //                    ti.FirstHandTimestamp = tournament.GetFirstHandTimestampET();
                //                }
                //                else
                //                {
                //                    ti.FirstHandTimestamp = DateTime.MaxValue;
                //                }
                //            }
                //            else
                //            {
                //                ti.IsTournament = false;
                //                ti.FirstHandTimestamp = DateTime.MaxValue;
                //            }
                //            ttati.TableInfos.Add(ti);
                //        }
                //    }
                //}

                // tile

                foreach (var ttati in ttatis)
                {
                    Tile(ttati);
                }
            }

        }

        private static void Tile(TableTileAndTableInfos ttati)
        {
            List<HandleRectangleWindow> infoForWindowsToMove = new List<HandleRectangleWindow>();

            if (ttati.TableTile.SortByStartingHand)
            {
                // sort
                List<Rectangle> availablePositions = ttati.TableTile.XYWHs.ToList();
                List<TableInfo> tournamentWindows = ttati.TableInfos.Where(o => o.FirstHandTimestamp < DateTime.MaxValue).ToList();
                List<TableInfo> otherWindows = ttati.TableInfos.Where(o => o.FirstHandTimestamp == DateTime.MaxValue).ToList();
                tournamentWindows.Sort((o0, o1) => DateTime.Compare(o0.FirstHandTimestamp, o1.FirstHandTimestamp));

                // sort first

                while (availablePositions.Any() && tournamentWindows.Any())
                {
                    Rectangle availablePosition = availablePositions[0];
                    TableInfo tournamentWindow = tournamentWindows[0];
                    availablePositions.Remove(availablePosition);
                    tournamentWindows.Remove(tournamentWindow);
                    //MoveAndResize(tournamentWindow.Handle, availablePosition);
                    infoForWindowsToMove.Add(new HandleRectangleWindow
                    {
                        Handle = tournamentWindow.Handle,
                        RectangleWindow = availablePosition,
                    });
                }

                // leftovers to the main window pool

                if (tournamentWindows.Any())
                {
                    otherWindows.AddRange(tournamentWindows);
                }

                // closest then

                int max = availablePositions.Count;
                if (max > otherWindows.Count) max = otherWindows.Count;

                infoForWindowsToMove.AddRange(MoveClosest(availablePositions.GetRange(0, max), otherWindows.GetRange(0, max)));
            }
            else
            {
                infoForWindowsToMove.AddRange(MoveClosest(ttati.TableTile.XYWHs.ToList(), ttati.TableInfos));
            }

            // move and resize windows
            //foreach (HandleRectangleWindow info in infoForWindowsToMove) { MoveWindowBringWindowToTop(info.Handle, info.RectangleWindow); }
            BeginDeferWindowPosDeferWindowPosEndDeferWindowPos(infoForWindowsToMove.ToArray(), IntPtr.Zero);
        }

        private static IEnumerable<HandleRectangleWindow> MoveClosest(List<Rectangle> availablePositions, List<TableInfo> availableWindows)
        {
            List<HandleRectangleWindow> infoForWindowsToMove = new List<HandleRectangleWindow>();

            while (availablePositions.Any() && availableWindows.Any())
            {
                List<Distances> distances = new List<Distances>();
                foreach (Rectangle availablePosition in availablePositions)
                {
                    double distance;
                    TableInfo closestWindow = GetClosestWindowWidhDistance(availablePosition, availableWindows, out distance);
                    distances.Add(new Distances { AvailablePosition = availablePosition, ClosestWindow = closestWindow, Distance = distance });
                }
                distances = distances.OrderBy(o => o.Distance).ToList();

                availablePositions.Remove(distances[0].AvailablePosition);
                availableWindows.Remove(distances[0].ClosestWindow);
                //MoveAndResize(distances[0].ClosestWindow.Handle, distances[0].AvailablePosition);
                infoForWindowsToMove.Add(new HandleRectangleWindow
                {
                    Handle = distances[0].ClosestWindow.Handle,
                    RectangleWindow = distances[0].AvailablePosition,
                });
            }

            return infoForWindowsToMove;
        }

        private static void MoveWindowBringWindowToTop(IntPtr handle, Rectangle rectangle)
        {
            WinApi.MoveWindow(handle, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, true);
            WinApi.BringWindowToTop(handle);
        }

        private static void BeginDeferWindowPosDeferWindowPosEndDeferWindowPos(HandleRectangleWindow[] infos, IntPtr hWndInsertAfter)
        {
            IntPtr pointerToMultipleWindowPositionStructure = WinApi.BeginDeferWindowPos(infos.Length);
            foreach (HandleRectangleWindow info in infos)
            {
                WinApi.DeferWindowPos(
                    pointerToMultipleWindowPositionStructure,
                    info.Handle,
                    hWndInsertAfter,
                    info.RectangleWindow.X,
                    info.RectangleWindow.Y,
                    info.RectangleWindow.Width,
                    info.RectangleWindow.Height,
                    WinApi.DeferWindowPosCommands.SWP_NOACTIVATE | WinApi.DeferWindowPosCommands.SWP_NOZORDER
                    );
            }
            WinApi.EndDeferWindowPos(pointerToMultipleWindowPositionStructure);
        }

        // auto tile
        private struct AutoTileSlot
        {
            public Rectangle Slot;
            public int Id;
            public double DistanceToTheAvailableSlot;
        }

        private bool AutoTile(Table newTable)
        {
            List<Table> oldTables = App.TableManager.GetTablesCopy();
            oldTables.RemoveAll(o => o.Handle.Equals(newTable.Handle));

            bool autoTileSuccessful = false;
            // filter only enabled and autotile tabletiles
            foreach (TableTile tableTile in GetTableTilesCopy().Where(o => o.IsEnabled && o.AutoTile).Where(tableTile => tableTile.RegexWindowClass.IsMatch(newTable.ClassName) && tableTile.RegexWindowTitle.IsMatch(newTable.Title)))
            {
                // check if matches from-to table count rule
                var count = oldTables.Count(a => tableTile.RegexWindowClass.IsMatch(a.ClassName) && tableTile.RegexWindowTitle.IsMatch(a.Title));
                if (tableTile.TableCountEqualOrGreaterThan <= count + 1 && count + 1 <= tableTile.TableCountEqualOrLessThan)
                {
                    // get available slots
                    var tables = oldTables.ToList();
                    var availableAutoTileSlots = new List<AutoTileSlot>();
                    for (int i = 0; i < tableTile.XYWHs.Length; i++)
                    {
                        availableAutoTileSlots.Add(new AutoTileSlot
                        {
                            Slot = tableTile.XYWHs[i],
                            Id = i,
                            DistanceToTheAvailableSlot = GetDistanceBetweenPoints(GetCenterPointOfWindow(tableTile.XYWHs[i]), GetCenterPointOfWindow(newTable.RectangleWindows))
                        });
                    }

                    for (int i = 0; i < tableTile.XYWHs.Length; i++)
                    {
                        var tableTakenThisSlot = tables.FirstOrDefault(o => GetDistanceBetweenPoints(GetCenterPointOfWindow(tableTile.XYWHs[i]), GetCenterPointOfWindow(o.RectangleWindows)) < 5);
                        if (tableTakenThisSlot != null)
                        {
                            tables.Remove(tableTakenThisSlot);
                            availableAutoTileSlots.Remove(availableAutoTileSlots.First(a => a.Id == i));
                        }
                    }

                    // autotile if available slots found
                    if (availableAutoTileSlots.Any())
                    {
                        Rectangle slot = new Rectangle(0, 0, 0, 0);
                        if (tableTile.AutoTileMethod == AutoTileMethod.ToTheClosestSlot)
                        {
                            availableAutoTileSlots.Sort((o1, o2) =>
                            {
                                double d = o1.DistanceToTheAvailableSlot - o2.DistanceToTheAvailableSlot;
                                if (d > 0) return 1;
                                return -1;
                            });

                            slot = availableAutoTileSlots[0].Slot;
                        }
                        if (tableTile.AutoTileMethod == AutoTileMethod.ToTheTopSlot)
                        {
                            availableAutoTileSlots.Sort((o1, o2) =>
                            {
                                double d = o1.Id - o2.Id;
                                if (d > 0) return 1;
                                return -1;
                            });

                            slot = availableAutoTileSlots[0].Slot;
                        }

                        if (!slot.IsEmpty)
                        {
                            WinApi.SetWindowPos(newTable.Handle, (IntPtr)(1), slot.X, slot.Y, slot.Width, slot.Height, WinApi.SetWindowPosFlags.DoNotActivate);
                            //BeginDeferWindowPosDeferWindowPosEndDeferWindowPos(new HandleRectangleWindow[1]
                            //{
                            //    new HandleRectangleWindow
                            //    {
                            //        Handle = newTable.Handle,
                            //        RectangleWindow = slot,
                            //    }
                            //}, (IntPtr)(1));

                            autoTileSuccessful = true;
                        }
                    }
                }
            }

            return autoTileSuccessful;
        }

        //

        private static TableInfo GetClosestWindowWidhDistance(Rectangle availablePosition, List<TableInfo> availableWindows, out double distance)
        {
            Point centerAvailablePosition = GetCenterPointOfWindow(availablePosition);
            double minDistance = double.MaxValue;
            TableInfo? closestWindow = null;
            distance = double.MaxValue;

            foreach (TableInfo availableWindow in availableWindows)
            {
                Point centerAvailableWindow = GetCenterPointOfWindow(availableWindow.CurrentRectangle);
                distance = GetDistanceBetweenPoints(centerAvailablePosition, centerAvailableWindow);
                if (minDistance > distance)
                {
                    minDistance = distance;
                    closestWindow = availableWindow;
                }
            }

            return closestWindow ?? availableWindows[0];
        }

        private static Point GetCenterPointOfWindow(Rectangle r)
        {
            return new Point(r.X + r.Width / 2, r.Y + r.Height / 2);
        }

        private static double GetDistanceBetweenPoints(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        private struct TableTileAndTableInfos
        {
            public TableTile TableTile;
            public List<TableInfo> TableInfos;
        }

        private struct TableInfo
        {
            public IntPtr Handle;
            public string Title;
            public Rectangle CurrentRectangle;
            public bool IsTournament;
            public long TournamentNumber;
            public DateTime FirstHandTimestamp;
        }

        private struct Distances
        {
            public Rectangle AvailablePosition;
            public TableInfo ClosestWindow;
            public double Distance;
        }

        private struct HandleRectangleWindow
        {
            public IntPtr Handle;
            public Rectangle RectangleWindow;
        }

        //

        public XElement ToXElement()
        {
            var xElement = new XElement("TableTiles");
            foreach (TableTile tableTile in GetTableTilesCopy())
            {
                xElement.Add(tableTile.ToXElement());
            }
            return xElement;
        }

        public void FromXElement(XElement xElement, ref List<ExceptionPsHandler> exceptions, string exceptionHeader)
        {
            foreach (XElement xTableTile in xElement.Elements("TableTile"))
            {
                TableTile tableTile = TableTile.FromXElement(xTableTile, ref exceptions, exceptionHeader);
                if (tableTile != null)
                {
                    Add(tableTile);
                }
            }
        }
    }
}

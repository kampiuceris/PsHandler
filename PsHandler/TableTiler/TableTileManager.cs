using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using PsHandler.Custom;
using PsHandler.Hook;
using PsHandler.Import;
using PsHandler.UI;

namespace PsHandler.TableTiler
{
    public class TableTileManager
    {
        // Table Tiles list controls

        private static readonly Regex _regexTournamentNumber = new Regex(@".+Tournament (?<tournament_number>\d+) .+");

        private static readonly object _lockTableTiles = new object();
        private static readonly List<TableTile> _tableTiles = new List<TableTile>();

        public static TableTile[] GetTableTilesCopy()
        {
            return _tableTiles.ToArray();
        }

        public static void Add(TableTile tableTile)
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

        public static void Add(IEnumerable<TableTile> tableTiles)
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

        public static void Remove(TableTile tableTile)
        {
            lock (_lockTableTiles)
            {
                _tableTiles.Remove(tableTile);
            }
        }

        public static void RemoveAll()
        {
            lock (_lockTableTiles)
            {
                _tableTiles.Clear();
            }
        }

        public static void SeedDefaultValues()
        {
            if (!_tableTiles.Any())
            {
                Add(TableTile.GetDefaultValues());
            }
        }

        // Tile mech

        private static bool _busy;
        private static Thread _thread;
        private static KeyCombination _keyCombinationPressed;
        private static readonly object _lockKeyCombination = new object();
        private static Table _newTable;
        private static IEnumerable<Table> _oldTables;
        private static readonly object _lockAutoTile = new object();

        public static void SetKeyCombination(KeyCombination keyCombination)
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

        public static void SetAutoTileTable(Table newTable, IEnumerable<Table> oldTables)
        {
            if (!Config.EnableTableTiler) return;

            lock (_lockAutoTile)
            {
                _newTable = newTable;
                _oldTables = oldTables;
            }
        }

        public static void Start()
        {
            Stop();
            _thread = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
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
                            if (_newTable != null && _oldTables != null)
                            {
                                _busy = true;
                                AutoTile(_newTable, _oldTables);
                                _newTable = null;
                                _oldTables = null;
                                _busy = false;
                            }
                        }
                        Thread.Sleep(100);
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

        public static void Stop()
        {
            if (_thread != null)
            {
                _thread.Interrupt();
            }
        }

        // private Tile mech

        private static void Tile(KeyCombination kc)
        {
            // collect info

            List<TableTileAndTableInfos> ttatis = _tableTiles.Where(o => o.IsEnabled && o.KeyCombination.Equals(kc)).Select(tableTile => new TableTileAndTableInfos { TableTile = tableTile, TableInfos = new List<TableInfo>() }).ToList();

            if (ttatis.Any())
            {
                foreach (IntPtr hwnd in WinApi.GetWindowHWndAll().Where(o => !Methods.IsMinimized(o)))
                {
                    string title = WinApi.GetWindowTitle(hwnd);
                    string windowClass = WinApi.GetClassName(hwnd);
                    //Debug.WriteLine("Window: " + title);
                    foreach (var ttati in ttatis)
                    {
                        if (ttati.TableTile.RegexWindowClass.IsMatch(windowClass) && ttati.TableTile.RegexWindowTitle.IsMatch(title))
                        {
                            TableInfo ti = new TableInfo { Handle = hwnd, Title = title, CurrentRectangle = WinApi.GetWindowRectangle(hwnd) };
                            Match match = _regexTournamentNumber.Match(title);
                            if (match.Success)
                            {
                                ti.IsTournament = long.TryParse(match.Groups["tournament_number"].Value, out ti.TournamentNumber);
                                Tournament tournament = App.HandHistoryManager.GetTournament(ti.TournamentNumber);
                                if (tournament != null)
                                {
                                    ti.FirstHandTimestamp = tournament.GetFirstHandTimestamp();
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
            BeginDeferWindowPosDeferWindowPosEndDeferWindowPos(infoForWindowsToMove.ToArray());
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

        private static void BeginDeferWindowPosDeferWindowPosEndDeferWindowPos(HandleRectangleWindow[] infos)
        {
            IntPtr pointerToMultipleWindowPositionStructure = WinApi.BeginDeferWindowPos(infos.Length);
            foreach (HandleRectangleWindow info in infos)
            {
                WinApi.DeferWindowPos(pointerToMultipleWindowPositionStructure, info.Handle, IntPtr.Zero, info.RectangleWindow.X, info.RectangleWindow.Y, info.RectangleWindow.Width, info.RectangleWindow.Height, 0);
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

        private static void AutoTile(Table newTable, IEnumerable<Table> oldTables)
        {
            // filter only enabled and autotile tabletiles
            foreach (TableTile tableTile in GetTableTilesCopy().Where(o => o.IsEnabled && o.AutoTile).Where(tableTile => tableTile.RegexWindowClass.IsMatch(newTable.ClassName) && tableTile.RegexWindowTitle.IsMatch(newTable.Title)))
            {
                // get available slots
                List<AutoTileSlot> availableAutoTileSlots = new List<AutoTileSlot>();
                for (int i = 0; i < tableTile.XYWHs.Length; i++)
                {
                    Rectangle slot = tableTile.XYWHs[i];
                    bool isFreeSlot = oldTables.All(o => (GetDistanceBetweenPoints(GetCenterPointOfWindow(slot), GetCenterPointOfWindow(o.RectangleWindows)) > 5));
                    if (isFreeSlot)
                    {
                        availableAutoTileSlots.Add(new AutoTileSlot { Slot = slot, Id = i, DistanceToTheAvailableSlot = GetDistanceBetweenPoints(GetCenterPointOfWindow(slot), GetCenterPointOfWindow(newTable.RectangleWindows)) });
                    }
                }
                if (availableAutoTileSlots.Any())
                {
                    if (tableTile.AutoTileMethod == AutoTileMethod.ToTheClosestSlot)
                    {
                        availableAutoTileSlots.Sort((o1, o2) =>
                        {
                            double d = o1.DistanceToTheAvailableSlot - o2.DistanceToTheAvailableSlot;
                            if (d > 0) return 1;
                            return -1;
                        });
                        Rectangle r = availableAutoTileSlots[0].Slot;
                        WinApi.MoveWindow(newTable.Handle, r.X, r.Y, r.Width, r.Height, true);
                    }
                    if (tableTile.AutoTileMethod == AutoTileMethod.ToTheTopSlot)
                    {
                        availableAutoTileSlots.Sort((o1, o2) =>
                        {
                            double d = o1.Id - o2.Id;
                            if (d > 0) return 1;
                            return -1;
                        });
                        Rectangle r = availableAutoTileSlots[0].Slot;
                        WinApi.MoveWindow(newTable.Handle, r.X, r.Y, r.Width, r.Height, true);
                    }
                }
            }
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

        public static XElement ToXElement()
        {
            var xElement = new XElement("TableTiles");
            foreach (TableTile tableTile in GetTableTilesCopy())
            {
                xElement.Add(tableTile.ToXElement());
            }
            return xElement;
        }

        public static void FromXElement(XElement xElement)
        {
            foreach (XElement xTableTile in xElement.Elements("TableTile"))
            {
                TableTile tableTile = TableTile.FromXElement(xTableTile);
                if (tableTile != null)
                {
                    Add(tableTile);
                }
            }
        }
    }
}

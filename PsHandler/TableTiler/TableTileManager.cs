using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using PsHandler.Hud.Import;
using PsHandler.UI;
using System.Threading;

namespace PsHandler.TableTiler
{
    public class TableTileManager
    {
        private static readonly Regex _regexTournamentNumber = new Regex(@".+Tournament (?<tournament_number>\d+) .+");
        public static readonly List<TableTile> TableTiles = new List<TableTile>();
        public static readonly object Lock = new object();
        private static bool _busy;
        private static Thread _thread;

        public static void Add(TableTile tableTile)
        {
            lock (Lock)
            {
                TableTiles.Add(tableTile);
                TableTiles.Sort((o0, o1) => string.CompareOrdinal(o0.Name, o1.Name));
            }
        }

        public static void Remove(TableTile tableTile)
        {
            lock (Lock)
            {
                TableTiles.Remove(tableTile);
            }
        }

        public static void Save()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\PSHandler\TableTiles", true))
                {
                    if (key == null)
                    {
                        using (RegistryKey keyPsHandler = Registry.CurrentUser.OpenSubKey(@"Software\PSHandler", true))
                        {
                            keyPsHandler.CreateSubKey("TableTiles");
                        }
                    }
                }

                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\PSHandler\TableTiles", true))
                {
                    if (key == null) return;

                    foreach (string valueName in key.GetValueNames())
                    {
                        key.DeleteValue(valueName);
                    }

                    lock (Lock)
                    {
                        foreach (var tableTile in TableTiles)
                        {
                            key.SetValue(tableTile.Name, tableTile.ToXml());
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public static void Load()
        {
            lock (Lock)
            {
                TableTiles.Clear();
            }

            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\PSHandler\TableTiles", true))
                {
                    if (key == null) return;

                    foreach (string valueName in key.GetValueNames())
                    {
                        TableTile tableTile = TableTile.FromXml(key.GetValue(valueName) as string);
                        if (tableTile != null)
                        {
                            Add(tableTile);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        //

        struct TableTileAndTableInfos
        {
            public TableTile TableTile;
            public List<TableInfo> TableInfos;
        }

        struct TableInfo
        {
            public IntPtr Handle;
            public string Title;
            public Rectangle CurrentRectangle;
            public bool IsTournament;
            public long TournamentNumber;
            public DateTime FirstHandTimestamp;
        }

        struct Distances
        {
            public Rectangle AvailablePosition;
            public TableInfo ClosestWindow;
            public double Distance;
        }

        public static void Tile(KeyCombination kc)
        {
            if (!Config.EnableTableTiler)
            {
                return;
            }

            if (!_busy)
            {
                _busy = true;
                _thread = new Thread(() =>
                {
                    try
                    {
                        // collect info

                        List<TableTileAndTableInfos> ttatis = TableTiles.Where(o => o.IsEnabled && o.KeyCombination.Equals(kc)).Select(tableTile => new TableTileAndTableInfos { TableTile = tableTile, TableInfos = new List<TableInfo>() }).ToList();

                        if (ttatis.Any())
                        {
                            foreach (IntPtr hwnd in WinApi.GetWindowHWndAll().Where(o => !Methods.IsMinimized(o)))
                            {
                                string title = WinApi.GetWindowTitle(hwnd);
                                //Debug.WriteLine("Window: " + title);
                                foreach (var ttati in ttatis)
                                {
                                    var IncludeAnd = ttati.TableTile.IncludeAnd.Length == 0 || ttati.TableTile.IncludeAnd.All(title.Contains);
                                    var IncludeOr = ttati.TableTile.IncludeOr.Length == 0 || ttati.TableTile.IncludeOr.Any(title.Contains);
                                    var ExcludeAnd = ttati.TableTile.ExcludeAnd.Length == 0 || !ttati.TableTile.ExcludeAnd.All(title.Contains);
                                    var ExcludeOr = ttati.TableTile.ExcludeOr.Length == 0 || !ttati.TableTile.ExcludeOr.Any(title.Contains);
                                    if (IncludeAnd && IncludeOr && ExcludeAnd && ExcludeOr)
                                    {
                                        //Debug.WriteLine("Move: " + title);

                                        TableInfo ti = new TableInfo { Handle = hwnd, Title = title, CurrentRectangle = WinApi.GetWindowRectangle(hwnd) };
                                        Match match = _regexTournamentNumber.Match(title);
                                        if (match.Success)
                                        {
                                            ti.IsTournament = long.TryParse(match.Groups["tournament_number"].Value, out ti.TournamentNumber);
                                            Tournament tournament = App.Import.GetTournament(ti.TournamentNumber);
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
                    catch
                    {
                    }
                    finally
                    {
                        _busy = false;
                    }
                });
                _thread.Start();
            }
        }

        private static void Tile(TableTileAndTableInfos ttati)
        {
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
                    WinApi.MoveWindow(tournamentWindow.Handle, availablePosition.X, availablePosition.Y, availablePosition.Width, availablePosition.Height, true);
                    WinApi.BringWindowToTop(tournamentWindow.Handle);
                }

                // leftovers to the main window pool

                if (tournamentWindows.Any())
                {
                    otherWindows.AddRange(tournamentWindows);
                }

                // closest then

                MoveClosest(availablePositions, otherWindows);
            }
            else
            {
                MoveClosest(ttati.TableTile.XYWHs.ToList(), ttati.TableInfos);
            }
        }

        private static void MoveClosest(List<Rectangle> availablePositions, List<TableInfo> availableWindows)
        {
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
                MoveAndResize(distances[0].ClosestWindow.Handle, distances[0].AvailablePosition);
            }
        }

        private static void MoveAndResize(IntPtr handle, Rectangle rectangle)
        {
            WinApi.MoveWindow(handle, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, true);
            WinApi.BringWindowToTop(handle);
        }

        private static TableInfo GetClosestWindow(Rectangle availablePosition, List<TableInfo> availableWindows)
        {
            Point centerAvailablePosition = GetCenterPointOfWindow(availablePosition);
            double minDistance = double.MaxValue;
            TableInfo? closestWindow = null;

            foreach (TableInfo availableWindow in availableWindows)
            {
                Point centerAvailableWindow = GetCenterPointOfWindow(availableWindow.CurrentRectangle);
                double distance = GetDistanceBetweenPoints(centerAvailablePosition, centerAvailableWindow);
                if (minDistance > distance)
                {
                    minDistance = distance;
                    closestWindow = availableWindow;
                }
            }

            return closestWindow ?? availableWindows[0];
        }

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
    }
}

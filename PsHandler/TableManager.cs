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
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using PsHandler.Custom;
using PsHandler.Hud;
using PsHandler.Import;
using PsHandler.PokerMath;

namespace PsHandler
{
    public interface IObserverTableManagerTableList
    {
        void UpdateView(List<Table> tables);
    }

    public interface IObserverTableManagerTableCount
    {
        void SetTableCount(int value);
    }

    public class Table : IDisposable
    {
        // WinApi
        public IntPtr Handle;
        public string Title;
        public string ClassName;
        public Rectangle RectangleWindows;
        public Rectangle RectangleClient;
        // PsHandler
        public TableHud TableHud;

        public Table(IntPtr handle)
        {
            Handle = handle;
        }

        public void Dispose()
        {
            if (TableHud != null)
            {
                TableHud.Dispose();
                TableHud = null;
            }
        }

        public void EnsureHud()
        {
            if (Config.HudEnable)
            {
                if (TableHud == null)
                {
                    TableHud = new TableHud(this);
                }
            }
            else
            {
                if (TableHud != null)
                {
                    TableHud.Dispose();
                    TableHud = null;
                }
            }
        }
    }

    public class TableManager
    {
        private Thread _thread;
        private const int DELAY_MS = 100;
        private readonly List<Table> _tables = new List<Table>();
        private readonly object _tablesLock = new object();
        public IObserverTableManagerTableList ObserverTableManagerTableList;
        public IObserverTableManagerTableCount ObserverTableManagerTableCount;

        public TableManager()
        {
            Start();
        }

        public void Start()
        {
            Stop();

            _thread = new Thread(() =>
            {
                try
                {
                    int timer = 0;
                    bool firstCycle = true;
                    while (true)
                    {
                        lock (_tablesLock)
                        {
                            IntPtr[] handles = WinApi.GetWindowHandlesByClassName("PokerStarsTableFrameClass");

                            // tables
                            // remove closed tables
                            foreach (Table t in _tables.Where(o => !handles.Contains(o.Handle)).ToArray())
                            {
                                t.Dispose();
                                _tables.Remove(t);
                            }
                            // update tables
                            foreach (IntPtr handle in handles)
                            {
                                bool isNewTable = false;
                                // find or craete new table
                                Table table = _tables.FirstOrDefault(o => o.Handle == handle);
                                if (table == null)
                                {
                                    table = new Table(handle) { ClassName = "PokerStarsTableFrameClass" };
                                    _tables.Add(table);
                                    isNewTable = true;
                                }
                                // update table
                                table.Title = WinApi.GetWindowTitle(handle);
                                table.RectangleWindows = WinApi.GetWindowRectangle(handle);
                                table.RectangleClient = WinApi.GetClientRectangle(handle);
                                table.EnsureHud();
                                // autotile
                                List<Table> tablesWithoutNewTable = _tables.ToList();
                                tablesWithoutNewTable.Remove(table);
                                if (isNewTable && !firstCycle)
                                {
                                    App.TableTileManager.AddAutoTileTable(table);
                                }
                            }

                            // controller
                            if (Config.AutoclickImBack || Config.AutoclickTimebank)
                            {
                                timer += DELAY_MS;
                                if (timer > 2000)
                                {
                                    timer = 0;
                                    foreach (IntPtr handle in handles)
                                    {
                                        Bitmap bitmap = ScreenCapture.GetBitmapWindowClient(handle);
                                        if (bitmap != null)
                                        {
                                            Bmp bmp = new Bmp(bitmap);
                                            if (Config.AutoclickImBack)
                                            {
                                                if (!Config.AutoclickImBackDisableDuringBreaks || (Config.AutoclickImBackDisableDuringBreaks && DateTime.Now.Minute < 55))
                                                {
                                                    Methods.CheckButtonAndClick(bmp, Config.PokerStarsThemeTable.ButtonImBack, handle);
                                                }
                                            }
                                            if (Config.AutoclickTimebank) Methods.CheckButtonAndClick(bmp, Config.PokerStarsThemeTable.ButtonTimebank, handle);
                                        }
                                    }
                                }
                            }

                            // UI update
                            if (ObserverTableManagerTableList != null) ObserverTableManagerTableList.UpdateView(_tables);
                            if (ObserverTableManagerTableCount != null) ObserverTableManagerTableCount.SetTableCount(_tables.Count);
                        }
                        if (firstCycle) firstCycle = false;
                        Thread.Sleep(DELAY_MS);
                    }
                }
#if (DEBUG)
                catch (ThreadInterruptedException)
                {
                }
#else
                catch (Exception e)
                {
                    if (!(e is ThreadInterruptedException))
                    {
                        Methods.DisplayException(e, App.WindowMain, WindowStartupLocation.CenterOwner);
                    }
                }
#endif
                finally
                {
                    lock (_tablesLock)
                    {
                        foreach (Table t in _tables)
                        {
                            t.Dispose();
                        }
                        _tables.Clear();
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

        public List<Table> GetTablesCopy()
        {
            return _tables.ToList();
        }
    }
}

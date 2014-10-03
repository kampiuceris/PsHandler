using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using PsHandler.Annotations;
using PsHandler.Custom;
using PsHandler.Hud;
using PsHandler.TableTiler;
using PsHandler.UI;
using PsHandler.Import;

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
            if (Config.EnableHud)
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
                                    EnsureTableStyle(table);
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
                                    TableTileManager.AddAutoTileTable(table);
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
                                            if (Config.AutoclickImBack) Methods.CheckButtonAndClick(bmp, Config.PokerStarsThemeTable.ButtonImBack, handle);
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
                        Methods.DisplayException(e);
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

        public void EnsureTablesStyle()
        {
            foreach (Table table in _tables.ToArray())
            {
                EnsureTableStyle(table);
            }
        }

        private void EnsureTableStyle(Table table)
        {
            if (Config.EnableCustomTablesWindowStyle)
            {
                switch (Config.CustomTablesWindowStyle)
                {
                    case Config.TableWindowStyle.NoCaption:
                        WinApi.SetWindowStyleNoCaption(table.Handle);
                        break;
                    case Config.TableWindowStyle.Borderless:
                        WinApi.SetWindowStyleNone(table.Handle);
                        break;
                }
            }
            else
            {
                WinApi.SetWindowStyleNormal(table.Handle);
            }
        }

        //

        public static bool EnableHudTimer { set; get; }
        public static bool EnableHudBigBlind { set; get; }
        public static bool HudTimerLocationLocked { set; get; }
        public static bool HudBigBlindLocationLocked { set; get; }

        // Default, Max10, Max9, Max8, Max7,  Max6, Max4, Max2
        public static float[] HudTimerLocationX = new float[Enum.GetNames(typeof(TableSize)).Length];
        public static float[] HudTimerLocationY = new float[Enum.GetNames(typeof(TableSize)).Length];
        public static float[] HudBigBlindLocationX = new float[Enum.GetNames(typeof(TableSize)).Length];
        public static float[] HudBigBlindLocationY = new float[Enum.GetNames(typeof(TableSize)).Length];

        // Timer

        public static void SetHudTimerLocationX(TableSize tableSize, float value, object sender)
        {
            if (value > 5) value = 5; if (value < -5) value = -5;
            HudTimerLocationX[(int)tableSize] = value;
            if (App.WindowMain != null && App.WindowMain.UCHud != null && App.WindowMain.UCHud.UCHudTimer != null && App.WindowMain.UCHud.UCHudTimer.TextBoxesLocationX.All(o => !o.Equals(sender)))
            {
                App.WindowMain.UCHud.UCHudTimer.TextBoxesLocationX[(int)tableSize].Text = HudTimerLocationX[(int)tableSize].ToString(CultureInfo.InvariantCulture);
            }
        }
        public static void SetHudTimerLocationY(TableSize tableSize, float value, object sender)
        {
            if (value > 5) value = 5; if (value < -5) value = -5;
            HudTimerLocationY[(int)tableSize] = value;
            if (App.WindowMain != null && App.WindowMain.UCHud != null && App.WindowMain.UCHud.UCHudTimer != null && App.WindowMain.UCHud.UCHudTimer.TextBoxesLocationY.All(o => !o.Equals(sender)))
            {
                App.WindowMain.UCHud.UCHudTimer.TextBoxesLocationY[(int)tableSize].Text = HudTimerLocationY[(int)tableSize].ToString(CultureInfo.InvariantCulture);
            }
        }
        public static float GetHudTimerLocationX(TableSize tableSize, object sender)
        {
            return HudTimerLocationX[(int)tableSize];
        }
        public static float GetHudTimerLocationY(TableSize tableSize, object sender)
        {
            return HudTimerLocationY[(int)tableSize];
        }

        // BigBlind

        public static void SetHudBigBlindLocationX(TableSize tableSize, float value, object sender)
        {
            if (value > 5) value = 5; if (value < -5) value = -5;
            HudBigBlindLocationX[(int)tableSize] = value;
            if (App.WindowMain != null && App.WindowMain.UCHud != null && App.WindowMain.UCHud.UCHudBigBlind != null && App.WindowMain.UCHud.UCHudBigBlind.TextBoxesLocationX.All(o => !o.Equals(sender)))
            {
                App.WindowMain.UCHud.UCHudBigBlind.TextBoxesLocationX[(int)tableSize].Text = HudBigBlindLocationX[(int)tableSize].ToString(CultureInfo.InvariantCulture);
            }
        }
        public static void SetHudBigBlindLocationY(TableSize tableSize, float value, object sender)
        {
            if (value > 5) value = 5; if (value < -5) value = -5;
            HudBigBlindLocationY[(int)tableSize] = value;
            if (App.WindowMain != null && App.WindowMain.UCHud != null && App.WindowMain.UCHud.UCHudBigBlind != null && App.WindowMain.UCHud.UCHudBigBlind.TextBoxesLocationY.All(o => !o.Equals(sender)))
            {
                App.WindowMain.UCHud.UCHudBigBlind.TextBoxesLocationY[(int)tableSize].Text = HudBigBlindLocationY[(int)tableSize].ToString(CultureInfo.InvariantCulture);
            }
        }
        public static float GetHudBigBlindLocationX(TableSize tableSize, object sender)
        {
            return HudBigBlindLocationX[(int)tableSize];
        }
        public static float GetHudBigBlindLocationY(TableSize tableSize, object sender)
        {
            return HudBigBlindLocationY[(int)tableSize];
        }

        // 

        #region XML Save/Lead Locations

        public static XElement ToXElementHudTimerLocations()
        {
            var xElement = new XElement("HudTimerLocations");

            foreach (TableSize tableSize in Enum.GetValues(typeof(TableSize)))
            {
                xElement.Add(new XElement("Location",
                    new XElement("TableSize", tableSize),
                    new XElement("LocationX", GetHudTimerLocationX(tableSize, null)),
                    new XElement("LocationY", GetHudTimerLocationY(tableSize, null))
                    ));
            }

            return xElement;
        }

        public static XElement ToXElementHudBigBlindLocations()
        {
            var xElement = new XElement("HudBigBlindLocations");

            foreach (TableSize tableSize in Enum.GetValues(typeof(TableSize)))
            {
                xElement.Add(new XElement("Location",
                    new XElement("TableSize", tableSize),
                    new XElement("LocationX", GetHudBigBlindLocationX(tableSize, null)),
                    new XElement("LocationY", GetHudBigBlindLocationY(tableSize, null))
                    ));
            }

            return xElement;
        }

        public static void FromXElementHudTimerLocations(XElement xElement, ref int errors)
        {
            foreach (XElement xlocation in xElement.Elements("Location"))
            {
                try
                {
                    TableSize tableSize = Enum.GetValues(typeof(TableSize)).Cast<TableSize>().FirstOrDefault(ts => ts.ToString().Equals(xlocation.Element("TableSize").Value));
                    SetHudTimerLocationX(tableSize, float.Parse(xlocation.Element("LocationX").Value), null);
                    SetHudTimerLocationY(tableSize, float.Parse(xlocation.Element("LocationY").Value), null);
                }
                catch
                {
                    errors++;
                }
            }
        }

        public static void FromXElementHudBigBlindLocations(XElement xElement, ref int errors)
        {
            foreach (XElement xlocation in xElement.Elements("Location"))
            {
                try
                {
                    TableSize tableSize = Enum.GetValues(typeof(TableSize)).Cast<TableSize>().FirstOrDefault(ts => ts.ToString().Equals(xlocation.Element("TableSize").Value));
                    SetHudBigBlindLocationX(tableSize, float.Parse(xlocation.Element("LocationX").Value), null);
                    SetHudBigBlindLocationY(tableSize, float.Parse(xlocation.Element("LocationY").Value), null);
                }
                catch
                {
                    errors++;
                }
            }
        }

        #endregion
    }
}

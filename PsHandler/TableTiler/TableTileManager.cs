using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using PsHandler.UI;

namespace PsHandler.TableTiler
{
    public class TableTileManager
    {
        public static readonly List<TableTile> TableTiles = new List<TableTile>();
        public static readonly object Lock = new object();

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
                    if (key == null) return;

                    lock (Lock)
                    {
                        foreach (string valueName in key.GetValueNames())
                        {
                            key.DeleteValue(valueName);
                        }

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
            TableTiles.Clear();

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
                            TableTiles.Add(tableTile);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public static void Tile(KeyCombination kc)
        {
            //TODO check for keycombinations and execute tiles
        }
    }
}

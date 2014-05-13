using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace PsHandler.PokerTypes
{
    public class PokerTypeManager
    {
        public static readonly List<PokerType> PokerTypes = new List<PokerType>();
        public static readonly object Lock = new object();

        public static void Add(PokerType pokerType)
        {
            lock (Lock)
            {
                PokerTypes.Add(pokerType);
                PokerTypes.Sort((o0, o1) => string.CompareOrdinal(o0.Name, o1.Name));
            }
        }

        public static void Add(IEnumerable<PokerType> pokerTypes)
        {
            lock (Lock)
            {
                PokerTypes.AddRange(pokerTypes);
                PokerTypes.Sort((o0, o1) => string.CompareOrdinal(o0.Name, o1.Name));
            }
        }

        public static void Remove(PokerType pokerType)
        {
            lock (Lock)
            {
                PokerTypes.Remove(pokerType);
            }
        }

        public static void SeedDefaultValues()
        {
            if (!PokerTypes.Any())
            {
                Add(PokerType.GetDefaultValues());
            }
        }

        public static void Save()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\PSHandler\PokerTypes", true))
                {
                    if (key == null)
                    {
                        using (RegistryKey keyPsHandler = Registry.CurrentUser.OpenSubKey(@"Software\PSHandler", true))
                        {
                            keyPsHandler.CreateSubKey("PokerTypes");
                        }
                    }
                }

                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\PSHandler\PokerTypes", true))
                {
                    foreach (string valueName in key.GetValueNames())
                    {
                        key.DeleteValue(valueName);
                    }

                    lock (Lock)
                    {
                        foreach (PokerType pokerType in PokerTypes)
                        {
                            key.SetValue(pokerType.Name, pokerType.ToXml());
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
                PokerTypes.Clear();
            }

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\PSHandler\PokerTypes", true))
            {
                if (key == null) return;

                foreach (string valueName in key.GetValueNames())
                {
                    PokerType pokerType = PokerType.FromXml(key.GetValue(valueName) as string);
                    if (pokerType != null)
                    {
                        Add(pokerType);
                    }
                }
            }
        }
    }
}

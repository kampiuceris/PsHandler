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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PsHandler.Randomizer
{
    public class RandomizerManager
    {
        private WindowRandomizer _windowRandomizer;

        public void CheckKeyCombination(KeyCombination kc)
        {
            if (!Config.EnableRandomizer) return;

            if (kc.Equals(Config.HotkeyRandomizerChance10)) { ShowRandomizer(Config.RandomizerChance10); }
            if (kc.Equals(Config.HotkeyRandomizerChance20)) { ShowRandomizer(Config.RandomizerChance20); }
            if (kc.Equals(Config.HotkeyRandomizerChance30)) { ShowRandomizer(Config.RandomizerChance30); }
            if (kc.Equals(Config.HotkeyRandomizerChance40)) { ShowRandomizer(Config.RandomizerChance40); }
            if (kc.Equals(Config.HotkeyRandomizerChance50)) { ShowRandomizer(Config.RandomizerChance50); }
            if (kc.Equals(Config.HotkeyRandomizerChance60)) { ShowRandomizer(Config.RandomizerChance60); }
            if (kc.Equals(Config.HotkeyRandomizerChance70)) { ShowRandomizer(Config.RandomizerChance70); }
            if (kc.Equals(Config.HotkeyRandomizerChance80)) { ShowRandomizer(Config.RandomizerChance80); }
            if (kc.Equals(Config.HotkeyRandomizerChance90)) { ShowRandomizer(Config.RandomizerChance90); }
        }

        public void ShowRandomizer(double value)
        {
            if (_windowRandomizer != null)
            {
                _windowRandomizer.Close();
            }
            _windowRandomizer = new WindowRandomizer(value / 100.0);
            _windowRandomizer.Show();
        }

        public void ___SeedDefaultValues_Obsolete()
        {
            if (Config.RandomizerChance10 == 0
                && Config.RandomizerChance20 == 0
                && Config.RandomizerChance30 == 0
                && Config.RandomizerChance40 == 0
                && Config.RandomizerChance50 == 0
                && Config.RandomizerChance60 == 0
                && Config.RandomizerChance70 == 0
                && Config.RandomizerChance80 == 0
                && Config.RandomizerChance90 == 0
                && Config.HotkeyRandomizerChance10.Equals(new KeyCombination(Key.None, false, false, false))
                && Config.HotkeyRandomizerChance20.Equals(new KeyCombination(Key.None, false, false, false))
                && Config.HotkeyRandomizerChance30.Equals(new KeyCombination(Key.None, false, false, false))
                && Config.HotkeyRandomizerChance40.Equals(new KeyCombination(Key.None, false, false, false))
                && Config.HotkeyRandomizerChance50.Equals(new KeyCombination(Key.None, false, false, false))
                && Config.HotkeyRandomizerChance60.Equals(new KeyCombination(Key.None, false, false, false))
                && Config.HotkeyRandomizerChance70.Equals(new KeyCombination(Key.None, false, false, false))
                && Config.HotkeyRandomizerChance80.Equals(new KeyCombination(Key.None, false, false, false))
                && Config.HotkeyRandomizerChance90.Equals(new KeyCombination(Key.None, false, false, false)))
            {
                Config.RandomizerChance10 = 10;
                Config.RandomizerChance20 = 20;
                Config.RandomizerChance30 = 30;
                Config.RandomizerChance40 = 40;
                Config.RandomizerChance50 = 50;
                Config.RandomizerChance60 = 60;
                Config.RandomizerChance70 = 70;
                Config.RandomizerChance80 = 80;
                Config.RandomizerChance90 = 90;

                Config.HotkeyRandomizerChance10 = new KeyCombination(Key.NumPad1, true, false, false);
                Config.HotkeyRandomizerChance20 = new KeyCombination(Key.NumPad2, true, false, false);
                Config.HotkeyRandomizerChance30 = new KeyCombination(Key.NumPad3, true, false, false);
                Config.HotkeyRandomizerChance40 = new KeyCombination(Key.NumPad4, true, false, false);
                Config.HotkeyRandomizerChance50 = new KeyCombination(Key.NumPad5, true, false, false);
                Config.HotkeyRandomizerChance60 = new KeyCombination(Key.NumPad6, true, false, false);
                Config.HotkeyRandomizerChance70 = new KeyCombination(Key.NumPad7, true, false, false);
                Config.HotkeyRandomizerChance80 = new KeyCombination(Key.NumPad8, true, false, false);
                Config.HotkeyRandomizerChance90 = new KeyCombination(Key.NumPad9, true, false, false);
            }
        }
    }
}

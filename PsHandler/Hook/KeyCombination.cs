using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PsHandler.Hook
{
    public class KeyCombination
    {
        public Key Key { get; set; }
        public bool Ctrl = false;
        public bool Alt = false;
        public bool Shift = false;
        public event EventHandler KeyCombinationPressed;
        public string CommandName;

        public KeyCombination(Key selectedKey, bool ctrl, bool alt, bool shift, string commandName, params EventHandler[] eventHandlers)
        {
            CommandName = commandName;
            Key = selectedKey;
            SetModifiers(ctrl, alt, shift);
            foreach (EventHandler eventHandler in eventHandlers)
            {
                KeyCombinationPressed += eventHandler;
            }
        }

        public KeyCombination(Key selectedKey)
            : this(selectedKey, false, false, false, "")
        {
        }

        public KeyCombination(Key selectedKey, bool ctrl, bool alt, bool shift, params EventHandler[] eventHandlers)
            : this(selectedKey, ctrl, alt, shift, "", eventHandlers)
        {
        }

        public void Set(Key selectedKey, bool ctrl, bool alt, bool shift)
        {
            Key = selectedKey;
            Alt = alt;
            Ctrl = ctrl;
            Shift = shift;
        }

        public void SetModifiers(bool ctrl, bool alt, bool shift)
        {
            Alt = alt;
            Ctrl = ctrl;
            Shift = shift;
        }

        public void OnKeyCombinationPressed(EventArgs e)
        {
            EventHandler handler = KeyCombinationPressed;
            if (handler != null) handler(null, e);
        }

        public void OnKeyCombinationReleased(EventArgs e)
        {
            //throw new NotSupportedException();
        }

        public static KeyCombination Parse(string text)
        {
            try
            {
                string[] split = text.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                Key key;
                bool tryParse = Enum.TryParse(split[3], true, out key);
                if (!tryParse) throw new Exception("Cannot parse Key");
                return new KeyCombination(key, bool.Parse(split[0]), bool.Parse(split[1]), bool.Parse(split[2]));
            }
            catch
            {
                return new KeyCombination(Key.None, false, false, false);
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", Ctrl, Alt, Shift, Key);
        }

        public bool Equals(KeyCombination kc)
        {
            return kc.Key == Key && kc.Ctrl == Ctrl && kc.Alt == Alt && kc.Shift == Shift;
        }
    }
}

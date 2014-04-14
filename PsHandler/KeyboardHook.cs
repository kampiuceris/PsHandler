using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Collections.Generic;

namespace PsHandler
{
    public delegate void CallbackKeyCombination(KeyCombination keyCombination);

    public class KeyboardHook : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;
        private readonly LowLevelKeyboardProc _keyboardProc;
        private readonly IntPtr _hookId = IntPtr.Zero;

        public List<KeyCombination> KeyCombinationsDown = new List<KeyCombination>();
        public List<KeyCombination> KeyCombinationsUp = new List<KeyCombination>();
        public List<CallbackKeyCombination> KeyCombinationDownMethods = new List<CallbackKeyCombination>();
        public List<CallbackKeyCombination> KeyCombinationUpMethods = new List<CallbackKeyCombination>();

        public KeyboardHook()
        {
            _keyboardProc = HookCallback;
            _hookId = SetHook(_keyboardProc);
        }

        public void Dispose()
        {
            UnhookWindowsHookEx(_hookId);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            //Trace.WriteLine(nCode + " " + wParam.ToString("X") + " " + lParam);
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Key key = KeyInterop.KeyFromVirtualKey(vkCode);
                //Trace.WriteLine(keyPressed);
                foreach (CallbackKeyCombination method in KeyCombinationDownMethods)
                {
                    method(new KeyCombination(key, (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control, (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt, (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift));
                }
                foreach (KeyCombination kc in KeyCombinationsDown)
                {
                    if ((key == kc.Key)
                        && (((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt) == kc.Alt)
                        && (((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) == kc.Ctrl)
                        && (((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) == kc.Shift))
                    {
                        kc.OnKeyCombinationPressed(new EventArgs());
                    }
                }
            }
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Key key = KeyInterop.KeyFromVirtualKey(vkCode);
                //Trace.WriteLine(keyPressed);
                foreach (CallbackKeyCombination method in KeyCombinationUpMethods)
                {
                    method(new KeyCombination(key, (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control, (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt, (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift));
                }
                foreach (KeyCombination kc in KeyCombinationsUp)
                {
                    if ((key == kc.Key)
                        && (((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt) == kc.Alt)
                        && (((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) == kc.Ctrl)
                        && (((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) == kc.Shift))
                    {
                        kc.OnKeyCombinationReleased(new EventArgs());
                    }
                }
            }
            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }

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
            catch (Exception)
            {
                return new KeyCombination(Key.None, false, false, false);
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", Ctrl, Alt, Shift, Key);
        }
    }
}
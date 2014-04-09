using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Collections.Generic;

namespace PsHandler.Hooks
{
    public delegate void CallbackKeyDown(Key keyPressed);

    public class KeyboardHook : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private LowLevelKeyboardProc keyboardProc;
        private IntPtr hookId = IntPtr.Zero;

        public List<KeyCombination> KeyCombinationsDown;
        public List<KeyCombination> KeyCombinationsUp;
        public List<CallbackKeyDown> CallbacksKeyDown;

        public KeyboardHook()
        {
            keyboardProc = HookCallback;
            hookId = SetHook(keyboardProc);
            KeyCombinationsDown = new List<KeyCombination>();
            KeyCombinationsUp = new List<KeyCombination>();
            CallbacksKeyDown = new List<CallbackKeyDown>();
        }

        public void Dispose()
        {
            UnhookWindowsHookEx(hookId);
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
            //Trace.WriteLine(nCode + " " + wParam + " " + lParam);
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                var keyPressed = KeyInterop.KeyFromVirtualKey(vkCode);
                //Trace.WriteLine(keyPressed);
                foreach (CallbackKeyDown method in CallbacksKeyDown)
                {
                    method(keyPressed);
                }
                foreach (KeyCombination kc in KeyCombinationsDown)
                {
                    if ((keyPressed == kc.SelectedKey)
                        && (((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt) == kc.Alt)
                        && (((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) == kc.Control)
                        && (((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) == kc.Shift))
                    {
                        kc.OnKeyCombinationPressed(new EventArgs());
                    }
                }
            }
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                var keyPressed = KeyInterop.KeyFromVirtualKey(vkCode);
                //Trace.WriteLine(keyPressed);
                foreach (KeyCombination kc in KeyCombinationsUp)
                {
                    if ((keyPressed == kc.SelectedKey)
                        && (((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt) == kc.Alt)
                        && (((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) == kc.Control)
                        && (((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) == kc.Shift))
                    {
                        kc.OnKeyCombinationPressed(new EventArgs());
                    }
                }

            }
            return CallNextHookEx(hookId, nCode, wParam, lParam);
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
        public Key SelectedKey { get; set; }
        public bool Alt = false;
        public bool Control = false;
        public bool Shift = false;
        public event EventHandler KeyCombinationPressed;
        public string CommandName;

        public KeyCombination(Key selectedKey, bool alt, bool control, bool shift, string commandName, params EventHandler[] eventHandlers)
        {
            CommandName = commandName;
            SelectedKey = selectedKey;
            SetModifiers(alt, control, shift);
            foreach (EventHandler eventHandler in eventHandlers)
            {
                KeyCombinationPressed += eventHandler;
            }
        }

        public KeyCombination(Key selectedKey, bool alt, bool control, bool shift, params EventHandler[] eventHandlers)
            : this(selectedKey, alt, control, shift, "", eventHandlers)
        {
        }

        public void SetModifiers(bool alt, bool control, bool shift)
        {
            Alt = alt;
            Control = control;
            Shift = shift;
        }

        public void OnKeyCombinationPressed(EventArgs e)
        {
            EventHandler handler = KeyCombinationPressed;
            if (handler != null) handler(null, e);
        }
    }
}
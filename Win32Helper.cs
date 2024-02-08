using System;

namespace AAgIO.Views
{
	public class Win32Helper
    {
        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern bool SetForegroundWindow(IntPtr handle);

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern bool ShowWindow(IntPtr hWind, int nCmdShow);

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        public static extern bool IsIconic(IntPtr handle);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);
    }
}    

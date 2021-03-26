using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDaily.Models.Win32
{
    public static class NativeMethods
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        public static Process GetActiveProcess()
        {
            try
            {
                IntPtr hWnd = GetForegroundWindow();

                int procId;
                GetWindowThreadProcessId(hWnd, out procId);

                return Process.GetProcessById(procId);
            }
            catch (Win32Exception)
            {
                return null;
            }
        }
    }
}

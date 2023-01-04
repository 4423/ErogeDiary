using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ErogeDiary.Models.Win32;

public static class ActiveProcess
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

    public static Process GetActiveProcess()
    {
        IntPtr hWnd = GetForegroundWindow();

        GetWindowThreadProcessId(hWnd, out int procId);

        return Process.GetProcessById(procId);
    }
}

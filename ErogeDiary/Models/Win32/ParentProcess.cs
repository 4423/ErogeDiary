using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ErogeDiary.Models.Win32;

// https://stackoverflow.com/questions/394816/how-to-get-parent-process-in-net-in-managed-way
// https://learn.microsoft.com/ja-jp/windows/win32/api/winternl/nf-winternl-ntqueryinformationprocess
public static class ParentProcess
{
    [StructLayout(LayoutKind.Sequential)]
    private struct ProcessBasicInformation
    {
        public IntPtr ExitStatus;
        public IntPtr PebBaseAddress;
        public IntPtr AffinityMask;
        public IntPtr BasePriority;
        public IntPtr UniqueProcessId;
        public IntPtr InheritedFromUniqueProcessId;
    }

    private enum ProcessInformationClass
    {
        ProcessBasicInformation = 0,
        // other...
    }

    // 今後の Windows ではサポートされない可能性があるため非推奨らしいが、他にいい方法がなさそう
    // そのうち .NET Standard で managed な API が実装されるかも
    // https://github.com/dotnet/runtime/issues/24423
    [DllImport("ntdll.dll")]
    private static extern int NtQueryInformationProcess(
        IntPtr processHandle,
        ProcessInformationClass processInformationClass, 
        ref ProcessBasicInformation processInformation, 
        int processInformationLength, 
        out int returnLength
    );

    public static Process GetParentProcess(Process process)
    {
        var pbi = new ProcessBasicInformation();

        var ntStatus = NtQueryInformationProcess(
            process.Handle,
            ProcessInformationClass.ProcessBasicInformation,
            ref pbi,
            Marshal.SizeOf(pbi),
            out int _
        );
        if (ntStatus != 0)
        {
            throw new Win32Exception($"NT status is {ntStatus}, not SUCCESS (0).");
        }

        var parentProcess = Process.GetProcessById(pbi.InheritedFromUniqueProcessId.ToInt32());
        if (parentProcess.StartTime > process.StartTime)
        {
            // PID が再利用されており、本当の親ではない。本当の親はもう死んでいる
            throw new Win32Exception("real parent process was killed.");
        }

        return parentProcess;
    }
}

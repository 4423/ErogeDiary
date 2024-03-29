﻿using ErogeDiary.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ErogeDiary.Models
{
    public delegate void ActiveProcessChanged(Process activeProcess);

    public sealed class ProcessMonitor
    {
        public static ProcessMonitor Instance { get; } = new ProcessMonitor();
        public event ActiveProcessChanged OnActiveProcessChanged;

        private DispatcherTimer timer;
        private Process previousProcess;

        private ProcessMonitor()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerTick;
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            timer.Stop();

            Update();

            timer.Start();
        }

        private void Update()
        {
            Process activeProcess;
            try
            {
                activeProcess = ActiveProcess.GetActiveProcess();
            }
            catch (Exception)
            {
                return;
            }

            if (previousProcess == null || previousProcess.ProcessName != activeProcess.ProcessName)
            {
                previousProcess = activeProcess;
                OnActiveProcessChanged?.Invoke(activeProcess);
            }
        }
    }
}

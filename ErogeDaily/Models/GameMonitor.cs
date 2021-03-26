using ErogeDaily.Models.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ErogeDaily.Models
{
    public delegate void GameStarted(Game game);
    public delegate void GameEnded(Game game, TimeSpan playTime);
    public delegate void ProgressChanged(Game game, TimeSpan currentPlayTime, TimeSpan totalPlayTime);

    public sealed class GameMonitor
    {
        public event GameStarted GameStarted;
        public event GameEnded GameEnded;
        public event ProgressChanged ProgressChanged;

        private Game previousGame;
        private DateTime previousGameStartDate;
        private IDatabaseAccess database;
        private DispatcherTimer timer;

        public GameMonitor(IDatabaseAccess database)
        {
            this.database = database;
            ProcessMonitor.Instance.OnActiveProcessChanged += OnActiveProcessChanged;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerTick;
        }

        private async void OnActiveProcessChanged(Process activeProcess)
        {
            if (previousGame != null)
            {
                timer.Stop();
                var playTime = DateTime.Now - previousGameStartDate;
                GameEnded?.Invoke(previousGame, playTime);
                previousGame = null;
            }

            var activeGame = await FindRegisteredGame(activeProcess);
            if (activeGame != null)
            {
                GameStarted?.Invoke(activeGame);
                previousGame = activeGame;
                previousGameStartDate = DateTime.Now;
                timer.Start();
            }
        }

        private async Task<Game> FindRegisteredGame(Process process)
        {
            try
            {
                return await database.FindGameByFileNameAsync(process.MainModule.FileName);
            }
            catch (Win32Exception)
            {
                return null;
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            var currentPlayTime = DateTime.Now - previousGameStartDate;
            var totalPlayTime = currentPlayTime + previousGame.TotalPlayTime;
            ProgressChanged?.Invoke(previousGame, currentPlayTime, totalPlayTime);
        }
    }
}

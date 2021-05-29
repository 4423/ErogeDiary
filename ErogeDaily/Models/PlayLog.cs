using Microsoft.EntityFrameworkCore;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDaily.Models
{
    public class PlayLog : BindableBase
    {
        public PlayLog() { }

        public PlayLog(int gameId, TimeSpan playTime)
        {
            GameId = gameId;
            PlayTime = playTime;
            var now = DateTime.Now;
            StartDateTime = now - playTime;
            EndDateTime = now;
        }


        private int id;
        public int Id
        {
            get => id;
            set { SetProperty(ref id, value); }
        }

        private int gameId;
        public int GameId
        {
            get => gameId;
            set { SetProperty(ref gameId, value); }
        }

        private DateTime startDateTime;
        public DateTime StartDateTime
        {
            get => startDateTime;
            set { SetProperty(ref startDateTime, value); }
        }

        private DateTime endDateTime;
        public DateTime EndDateTime
        {
            get => endDateTime;
            set { SetProperty(ref endDateTime, value); }
        }

        private TimeSpan playTime;
        public TimeSpan PlayTime
        {
            get => playTime;
            set { SetProperty(ref playTime, value); }
        }
    }
}

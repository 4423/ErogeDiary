using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDaily.Models
{
    public class Game : BindableBase
    {
        public Game()
        {
            RegistrationDate = DateTime.Now;
            LatestDate = null;
        }

        private int id;
        public int Id
        {
            get => id;
            set { SetProperty(ref id, value); }
        }

        private string title;
        public string Title
        {
            get => title;
            set { SetProperty(ref title, value); }
        }

        private string brand;
        public string Brand
        {
            get => brand;
            set { SetProperty(ref brand, value); }
        }

        private DateTime releaseDate;
        public DateTime ReleaseDate
        {
            get => releaseDate;
            set { SetProperty(ref releaseDate, value); }
        }

        private string imageUri;
        public string ImageUri
        {
            get => imageUri;
            set { SetProperty(ref imageUri, value); }
        }

        private string erogameScapeGameId;
        public string ErogameScapeGameId
        {
            get => erogameScapeGameId;
            set { SetProperty(ref erogameScapeGameId, value); }
        }

        private string fileName;
        public string FileName
        {
            get => fileName;
            set { SetProperty(ref fileName, value); }
        }

        private DateTime registrationDate;
        public DateTime RegistrationDate
        {
            get => registrationDate;
            set { SetProperty(ref registrationDate, value); }
        }

        private DateTime? latestDate;
        public DateTime? LatestDate
        {
            get => latestDate;
            set { SetProperty(ref latestDate, value); }
        }

        private TimeSpan totalPlayTime;
        public TimeSpan TotalPlayTime
        {
            get => totalPlayTime;
            set { SetProperty(ref totalPlayTime, value); }
        }

        private bool isCleared;
        public bool IsCleared
        {
            get => isCleared;
            set { SetProperty(ref isCleared, value); }
        }


        public bool IsValid()
        {
            return !String.IsNullOrWhiteSpace(title)
                && !String.IsNullOrWhiteSpace(brand)
                && !String.IsNullOrWhiteSpace(imageUri)
                && File.Exists(FileName);
        }

        public Game Clone()
        {
            var game = (Game)MemberwiseClone();
            return game;
        }

        public void CopyFrom(Game game)
        {
            Title = game.Title;
            Brand = game.Brand;
            ReleaseDate = game.ReleaseDate;
            ImageUri = game.ImageUri;
            FileName = game.FileName;
        }
    }

    public static class GameExtensions
    {
        public static Game ToGame(this ErogameScape.GameInfo gameInfo)
        {
            return new Game()
            {
                ErogameScapeGameId = gameInfo.Id,
                Title = gameInfo.Title,
                Brand = gameInfo.Brand,
                ReleaseDate = gameInfo.ReleaseDate,
                ImageUri = gameInfo.ImageUri
            };
        }
    }
}

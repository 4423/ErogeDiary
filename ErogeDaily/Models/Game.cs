using ErogeDaily.Models.DataAnnotations;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDaily.Models
{
    public class Game : VerifiableBindableBase
    {
        public Game()
        {
            ReleaseDate = DateTime.Now;
            RegistrationDate = DateTime.Now;
            LatestDate = null;
            Roots = new List<RootData>();
        }

        private int id;
        public int Id
        {
            get => id;
            set { SetProperty(ref id, value); }
        }

        private string title;
        [Required(ErrorMessage = "タイトルを入力してください。")]
        public string Title
        {
            get => title;
            set
            {
                SetProperty(ref title, value);
                ValidateProperty(value);
            }
        }

        private string brand;
        [Required(ErrorMessage = "ブランドを入力してください。")]
        public string Brand
        {
            get => brand;
            set
            {
                SetProperty(ref brand, value);
                ValidateProperty(value);
            }
        }

        private DateTime? releaseDate;
        [Required(ErrorMessage = "発売日を入力してください。")]
        public DateTime? ReleaseDate
        {
            get => releaseDate;
            set
            {
                SetProperty(ref releaseDate, value);
                ValidateProperty(value);
            }
        }

        private string imageUri;
        [Required(ErrorMessage = "サムネイル画像の場所を入力してください。")]
        [ValidExtensionRequired(ValidExtensions = new string[] { ".jpg", ".jpeg", ".png", ".bmp" })]
        public string ImageUri
        {
            get => imageUri;
            set
            {
                SetProperty(ref imageUri, value);
                ValidateProperty(value);
            }
        }

        private string erogameScapeGameId;
        public string ErogameScapeGameId
        {
            get => erogameScapeGameId;
            set { SetProperty(ref erogameScapeGameId, value); }
        }

        private string fileName;
        [Required(ErrorMessage = "実行ファイルの場所を入力してください。")]
        [FileExistRequired]
        public string FileName
        {
            get => fileName;
            set
            {
                SetProperty(ref fileName, value);
                ValidateProperty(value);
            }
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

        private List<RootData> roots;
        public List<RootData> Roots
        {
            get => roots;
            set { SetProperty(ref roots, value); }
        }

        private bool isCleared;
        public bool IsCleared
        {
            get => isCleared;
            set { SetProperty(ref isCleared, value); }
        }

        private DateTime? clearedAt;
        public DateTime? ClearedAt
        {
            get => clearedAt;
            set { SetProperty(ref clearedAt, value); }
        }

        public bool HasNullOrWhiteSpaceProperties()
        {
            return String.IsNullOrWhiteSpace(Title)
                || String.IsNullOrWhiteSpace(Brand)
                || String.IsNullOrWhiteSpace(ImageUri)
                || String.IsNullOrWhiteSpace(FileName);
        }

        public Game Clone()
        {
            var game = (Game)MemberwiseClone();
            if (Roots != null)
            {
                game.Roots = new List<RootData>(Roots.Clone());
            }
            return game;
        }

        public void CopyFrom(Game game)
        {
            Title = game.Title;
            Brand = game.Brand;
            ReleaseDate = game.ReleaseDate;
            ImageUri = game.ImageUri;
            FileName = game.FileName;
            IsCleared = game.IsCleared;
            ClearedAt = game.ClearedAt;
        }

        public TimeSpan GetUnallocatedTime()
        {
            if (Roots == null || Roots.Count == 0)
            {
                return TotalPlayTime;
            }
            var sumOfRootMilliseconds = Roots.Sum(r => r.PlayTime.TotalMilliseconds);
            var unallocatedMilliseconds = Math.Max(0, TotalPlayTime.TotalMilliseconds - sumOfRootMilliseconds);
            return TimeSpan.FromMilliseconds(unallocatedMilliseconds);
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

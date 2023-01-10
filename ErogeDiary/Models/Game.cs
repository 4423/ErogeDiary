using ErogeDiary.Models.DataAnnotations;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDiary.Models
{
    public class Game : VerifiableBindableBase
    {
        public Game()
        {
            var now = DateTime.Now;
            ReleaseDate = DateOnly.FromDateTime(now);
            RegistrationDate = now;
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

        private DateOnly? releaseDate;
        [Required(ErrorMessage = "発売日を入力してください。")]
        public DateOnly? ReleaseDate
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

        private string? erogameScapeGameId;
        public string? ErogameScapeGameId
        {
            get => erogameScapeGameId;
            set { SetProperty(ref erogameScapeGameId, value); }
        }

        private InstallationType installationType;
        public InstallationType InstallationType
        {
            get => installationType;
            set
            {
                SetProperty(ref installationType, value);

                // InstallationType に依存する validation を再評価
                switch (installationType)
                {
                    case InstallationType.Default:
                        ValidateProperty(fileName, nameof(FileName));
                        ClearErrors(nameof(WindowTitle));
                        break;
                    case InstallationType.DmmGamePlayer:
                        ValidateProperty(windowTitle, nameof(WindowTitle));
                        ClearErrors(nameof(FileName));
                        break;
                };
            }
        }

        private string? windowTitle;
        [RequiredIf(nameof(InstallationType), InstallationType.DmmGamePlayer, ErrorMessage = "ウィンドウタイトルを入力してください。")]
        public string? WindowTitle
        {
            get => windowTitle;
            set
            {
                SetProperty(ref windowTitle, value);
                ValidateProperty(value);
            }
        }

        private string? fileName;
        [RequiredIf(nameof(InstallationType), InstallationType.Default, ErrorMessage = "実行ファイルの場所を入力してください。")]
        [FileExistRequiredIf(nameof(InstallationType), InstallationType.Default)]
        public string? FileName
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
            set
            {
                SetProperty(ref isCleared, value);
                // IsCleared に依存する validation を再評価
                ValidateProperty(clearedAt, nameof(ClearedAt));
            }
        }

        private DateTime? clearedAt;
        [RequiredIf(nameof(IsCleared), true, ErrorMessage = "攻略日を入力してください。")]
        public DateTime? ClearedAt
        {
            get => clearedAt;
            set
            {
                SetProperty(ref clearedAt, value);
                ValidateProperty(value);
            }
        }

        public bool Valid()
        {
            var hasNullOrWhiteSpaceProperties = 
                String.IsNullOrWhiteSpace(Title)
                || String.IsNullOrWhiteSpace(Brand)
                || String.IsNullOrWhiteSpace(ImageUri)
                || InstallationType switch
                {
                    InstallationType.Default => String.IsNullOrWhiteSpace(FileName),
                    InstallationType.DmmGamePlayer => String.IsNullOrWhiteSpace(WindowTitle),
                };

            return !hasNullOrWhiteSpaceProperties && !HasErrors;
        }

        public void Pretty()
        {
            // 整合性を取る
            switch (InstallationType)
            {
                case InstallationType.Default:
                    WindowTitle = null;
                    break;
                case InstallationType.DmmGamePlayer:
                    FileName = null;
                    break;
            }

            if (!IsCleared)
            {
                ClearedAt = null;
            }
        }

        public Game Clone()
        {
            // MemberwiseClone を使うと constructor が呼ばれず ErrorsChanged が正しく発火しない
            var game = new Game();
            game.CopyFrom(this);
            return game;
        }

        public void CopyFrom(Game game)
        {
            Id = game.Id;
            Title = (string)game.Title.Clone();
            Brand = (string)game.Brand.Clone();
            ReleaseDate = game.ReleaseDate;
            ImageUri = (string)game.ImageUri.Clone();
            ErogameScapeGameId = (string?)game.ErogameScapeGameId?.Clone();
            InstallationType = game.InstallationType;
            WindowTitle = (string?)game.WindowTitle?.Clone();
            FileName = (string?)game.FileName?.Clone();
            RegistrationDate = game.RegistrationDate;
            LatestDate = game.LatestDate;
            TotalPlayTime = game.TotalPlayTime;
            Roots = new List<RootData>(game.Roots.Clone());
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

        public bool CanLaunch
        {
            get => InstallationType == InstallationType.Default;
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

    public enum InstallationType
    {
        Default,
        DmmGamePlayer,
    }
}

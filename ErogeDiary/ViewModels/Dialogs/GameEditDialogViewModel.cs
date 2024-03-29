﻿using ErogeDiary.Dialogs;
using ErogeDiary.Models;
using ErogeDiary.Models.Database;
using ErogeDiary.Models.Database.Entities;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Threading.Tasks;

namespace ErogeDiary.ViewModels.Dialogs
{
    public class GameEditDialogViewModel : BindableDialogBase
    {
        public DelegateCommand SelectThumbnailFileNameCommand { get; private set; }
        public DelegateCommand SelectExecutionFileNameCommand { get; private set; }
        public DelegateCommand CloseCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }

        private ErogeDiaryDbContext database;
        private IMessageDialog messageDialog;
        private IOpenFileDialog openFileDialog;
        private Game? originalGame;


        public GameEditDialogViewModel(
            ErogeDiaryDbContext database, 
            IMessageDialog messageDialog, 
            IOpenFileDialog openFileDialog)
        {
            this.database = database;
            this.messageDialog = messageDialog;
            this.openFileDialog = openFileDialog;

            SelectThumbnailFileNameCommand = new DelegateCommand(SelectThumbnailFileName);
            SelectExecutionFileNameCommand = new DelegateCommand(SelectExecutionFileName);
            CloseCommand = new DelegateCommand(CloseDialogCancel);
            UpdateCommand = new DelegateCommand(UpdateGame, CanExecuteUpdateGame);
        }


        public override void OnDialogOpened(IDialogParameters parameters)
        {
            originalGame = parameters.GetValue<Game>("game");
            VerifiableGame = new VerifiableGame()
            {
                Title = originalGame.Title,
                Brand = originalGame.Brand,
                ReleaseDate = originalGame.ReleaseDate,
                ImageUri = ThumbnailHelper.CombineThumbnailDir(originalGame.ImageFileName),
                ErogameScapeGameId = originalGame.ErogameScapeGameId,
                InstallationType = originalGame.InstallationType,
                WindowTitle = originalGame.WindowTitle,
                ExecutableFilePath = originalGame.ExecutableFilePath,
                IsCleared= originalGame.IsCleared,
                ClearedAt= originalGame.ClearedAt,
            };
            VerifiableGame.PropertyChanged += (_, __) => UpdateCommand.RaiseCanExecuteChanged();
            UpdateCommand.RaiseCanExecuteChanged();
        }

        public override void OnDialogClosed()
        {
            VerifiableGame!.ClearAllErrors();
            VerifiableGame = null;
        }


        private VerifiableGame? verifiableGame;
        public VerifiableGame? VerifiableGame
        {
            get { return verifiableGame; }
            set { SetProperty(ref verifiableGame, value); }
        }

        private bool isUpdating;
        public bool IsUpdating
        {
            get { return isUpdating; }
            set { SetProperty(ref isUpdating, value); }
        }

        private void SelectThumbnailFileName()
        {
            var imageUri = openFileDialog.Show(
                "サムネイル画像を選択してください",
                "画像ファイル(*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp");
            if (imageUri != null)
            {
                VerifiableGame!.ImageUri = imageUri;
            }
        }

        private void SelectExecutionFileName()
        {
            var fileName = openFileDialog.Show(
                "ゲームの実行ファイルを選択してください", "実行ファイル(*.exe)|*.exe");
            if (fileName != null)
            {
                VerifiableGame!.ExecutableFilePath = fileName;
            }
        }

        private bool CanExecuteUpdateGame()
            => VerifiableGame?.Valid() == true;

        private async void UpdateGame()
        {
            IsUpdating = true;
            try
            {
                await UpdateGameCore();
            } 
            finally
            {
                IsUpdating = false;
            }
        }

        private async Task UpdateGameCore()
        {
            if (VerifiableGame is null || originalGame is null)
            {
                throw new ArgumentNullException();
            }

            if (await HasConflictingGame(VerifiableGame, originalGame.GameId))
            {
                await messageDialog.ShowErrorAsync("既に同じゲームが登録されています。");
                return;
            }

            if (VerifiableGame.ImageUri != ThumbnailHelper.CombineThumbnailDir(originalGame.ImageFileName))
            {
                try
                {
                    VerifiableGame.ImageUri = new Uri(VerifiableGame.ImageUri!).IsFile ?
                        await ThumbnailHelper.CopyAndResize(VerifiableGame.ImageUri!) :
                        await ThumbnailHelper.DownloadAndResizeAsync(VerifiableGame.ImageUri!);
                }
                catch (Exception ex)
                {
                    await messageDialog.ShowErrorAsync($"サムネイル画像の取得に失敗しました。\n{ex.Message}");
                    return;
                }
            }

            VerifiableGame.Pretty();

            VerifiableGame.CopyTo(ref originalGame);

            await database.UpdateAsync(originalGame);

            CloseDialogOK();
        }

        private async Task<bool> HasConflictingGame(VerifiableGame verifiableGame, int gameId)
        {
            var gameWithSameTitleAndBrand = await database.FindGameByTitleAndBrandAsync(verifiableGame.Title!, verifiableGame.Brand!);
            var conflictTitleAndBrand = gameWithSameTitleAndBrand != null && gameWithSameTitleAndBrand.GameId != gameId;
            if (conflictTitleAndBrand)
            {
                return true;
            }

            if (verifiableGame.ExecutableFilePath == null)
            {
                return false;
            }

            var gameWithSameExecutableFilePath = await database.FindGameByFileNameAsync(verifiableGame.ExecutableFilePath);
            var conflictExecutableFilePath = gameWithSameExecutableFilePath != null && gameWithSameExecutableFilePath.GameId != gameId;
            return conflictExecutableFilePath;
        }
    }
}

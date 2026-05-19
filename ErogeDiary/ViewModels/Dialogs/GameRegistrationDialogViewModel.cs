using ErogeDiary.Dialogs;
using ErogeDiary.ErogameScape;
using ErogeDiary.Models;
using ErogeDiary.Models.Database;
using ErogeDiary.Models.Database.Entities;
using ErogeDiary.Properties;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ErogeDiary.ViewModels.Dialogs
{
    public partial class GameRegistrationDialogViewModel : BindableDialogBase
    {
        public Action? HideFlyout { get; set; }

        private ErogeDiaryDbContext database;
        private ErogameScapeClient erogameScapeClient;
        private IMessageDialog messageDialog;
        private IOpenFileDialog openFileDialog;


        public GameRegistrationDialogViewModel(
            ErogeDiaryDbContext database,
            ErogameScapeClient erogameScapeClient,
            IMessageDialog messageDialog,
            IOpenFileDialog openFileDialog)
        {
            VerifiableGame = new VerifiableGame();
            IsOpen = false;

            this.database = database;
            this.erogameScapeClient = erogameScapeClient;
            this.messageDialog = messageDialog;
            this.openFileDialog = openFileDialog;
        }


        private bool CanExecuteRegisterGame()
            => VerifiableGame.Valid();

        [RelayCommand(CanExecute = nameof(CanExecuteRegisterGame))]
        private async Task RegisterAsync()
        {
            try
            {
                IsRegistering = true;
                await RegisterGameCore();
            }
            finally
            {
                IsRegistering = false;
            }
        }

        private async Task RegisterGameCore()
        {
            if (await HasConflictingGame(VerifiableGame))
            {
                await messageDialog.ShowErrorAsync(Strings.GameRegistration_DuplicateGame);
                return;
            }

            VerifiableGame.ImageUri = new Uri(VerifiableGame.ImageUri!).IsFile ?
                await ThumbnailHelper.CopyAndResize(VerifiableGame.ImageUri!) :
                await ThumbnailHelper.DownloadAndResizeAsync(VerifiableGame.ImageUri!);

            VerifiableGame.Pretty();

            var game = new Game() { Title = "", Brand = "", ImageFileName = "" }; // 上書きするので値は何でもいい
            VerifiableGame.CopyTo(ref game);
            game.RegisteredAt = DateTime.Now; 

            await database.AddGameAsync(game);
            CloseDialogOK();
        }

        private async Task<bool> HasConflictingGame(VerifiableGame verifiableGame)
        {
            var conflictTitleAndBrand = await database.FindGameByTitleAndBrandAsync(verifiableGame.Title!, verifiableGame.Brand!) != null;
            if (conflictTitleAndBrand)
            {
                return true;
            }

            if (verifiableGame.ExecutableFilePath == null)
            {
                return false;
            }

            var conflictExecutableFilePath = await database.FindGameByFileNameAsync(verifiableGame.ExecutableFilePath) != null;
            return conflictExecutableFilePath;
        }

        [RelayCommand]
        private async Task FlyoutCompleteAsync()
        {
            IsWorking = true;

            try
            {
                var gameInfo = await erogameScapeClient.FetchGameInfoAsync(ErogameScapeUrl!);
                VerifiableGame = new VerifiableGame(gameInfo);
            }
            catch (Exception)
            {
                IsInvalidErogameScapeUrl = true;
                IsWorking = false;
                return;
            }

            HideFlyout?.Invoke();
            IsInvalidErogameScapeUrl = false;
            IsWorking = false;
        }

        [RelayCommand]
        private void SelectThumbnailFileName()
        {
            var imageUri = openFileDialog.Show(
                Strings.OpenFile_SelectThumbnail,
                Strings.FileFilter_Image);
            if (imageUri != null)
            {
                VerifiableGame.ImageUri = imageUri;
            }
        }

        [RelayCommand]
        private void SelectExecutionFileName()
        {
            var filePath = openFileDialog.Show(
                Strings.OpenFile_SelectExecutable,
                Strings.FileFilter_Executable);
            if (filePath != null)
            {
                VerifiableGame.ExecutableFilePath = filePath;
            }
        }

        private void VerifiableGameChanged(object? sender, EventArgs e)
            => RegisterCommand.NotifyCanExecuteChanged();


        [ObservableProperty]
        private VerifiableGame verifiableGame = null!;

        partial void OnVerifiableGameChanging(VerifiableGame value)
        {
            if (verifiableGame != null)
            {
                verifiableGame.PropertyChanged -= VerifiableGameChanged;
                verifiableGame.ErrorsChanged -= VerifiableGameChanged;
            }
        }

        partial void OnVerifiableGameChanged(VerifiableGame value)
        {
            value.PropertyChanged += VerifiableGameChanged;
            value.ErrorsChanged += VerifiableGameChanged;
            RegisterCommand.NotifyCanExecuteChanged();
        }

        [ObservableProperty]
        private bool isInvalidErogameScapeUrl;

        [ObservableProperty]
        private bool isWorking;

        [ObservableProperty]
        private bool isOpen;

        [ObservableProperty]
        private string? erogameScapeUrl;

        [ObservableProperty]
        private bool isRegistering;
    }
}

using ErogeDiary.Dialogs;
using ErogeDiary.ErogameScape;
using ErogeDiary.Models;
using ErogeDiary.Models.Database;
using ErogeDiary.Models.Database.Entities;
using Prism.Commands;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ErogeDiary.ViewModels.Dialogs
{
    public class GameRegistrationDialogViewModel : BindableDialogBase
    {
        public DelegateCommand FlyoutCompleteCommand { get; private set; }
        public DelegateCommand SelectThumbnailFileNameCommand { get; private set; }
        public DelegateCommand SelectExecutionFileNameCommand { get; private set; }
        public DelegateCommand RegisterCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }
        public Action HideFlyout { get; set; }

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
            FlyoutCompleteCommand = new DelegateCommand(FlyoutComplete);
            SelectThumbnailFileNameCommand = new DelegateCommand(SelectThumbnailFileName);
            SelectExecutionFileNameCommand = new DelegateCommand(SelectExecutionFileName);
            RegisterCommand = new DelegateCommand(RegisterGame, CanExecuteRegisterGame);
            CancelCommand = new DelegateCommand(CloseDialogCancel);
            
            VerifiableGame = new VerifiableGame();
            VerifiableGame.PropertyChanged += (_, __) => RegisterCommand.RaiseCanExecuteChanged();

            IsOpen = false;

            this.database = database;
            this.erogameScapeClient = erogameScapeClient;
            this.messageDialog = messageDialog;
            this.openFileDialog = openFileDialog;
        }


        private bool CanExecuteRegisterGame()
            => VerifiableGame.Valid();

        private async void RegisterGame()
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
                await messageDialog.ShowErrorAsync("既に同じゲームが登録されています。");
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

        private async void FlyoutComplete()
        {
            IsWorking = true;

            try
            {
                var gameInfo = await erogameScapeClient.FetchGameInfoAsync(ErogameScapeUrl);
                VerifiableGame = new VerifiableGame(gameInfo);
                VerifiableGame.PropertyChanged += (_, __) => RegisterCommand.RaiseCanExecuteChanged();
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

        private void SelectThumbnailFileName()
        {
            var imageUri = openFileDialog.Show(
                "サムネイル画像を選択してください", 
                "画像ファイル(*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp");
            if (imageUri != null)
            {
                VerifiableGame.ImageUri = imageUri;
            }
        }

        private void SelectExecutionFileName()
        {
            var filePath = openFileDialog.Show(
                "ゲームの実行ファイルを選択してください", "実行ファイル(*.exe)|*.exe");
            if (filePath != null)
            {
                VerifiableGame.ExecutableFilePath = filePath;
            }
        }


        private VerifiableGame verifiableGame;
        public VerifiableGame VerifiableGame
        {
            get { return verifiableGame; }
            set { SetProperty(ref verifiableGame, value); }
        }

        private bool isInvalidErogameScapeUrl;
        public bool IsInvalidErogameScapeUrl
        {
            get { return isInvalidErogameScapeUrl; }
            set { SetProperty(ref isInvalidErogameScapeUrl, value); }
        }

        private bool isWorking;
        public bool IsWorking
        {
            get { return isWorking; }
            set { SetProperty(ref isWorking, value); }
        }

        private bool isFlyoutOpen;
        public bool IsOpen
        {
            get { return isFlyoutOpen; }
            set { SetProperty(ref isFlyoutOpen, value); }
        }

        public string erogameScapeUrl;
        public string ErogameScapeUrl
        {
            get { return erogameScapeUrl; }
            set { SetProperty(ref erogameScapeUrl, value); }
        }

        private bool isRegistering;
        public bool IsRegistering
        {
            get { return isRegistering; }
            set { SetProperty(ref isRegistering, value); }
        }
    }
}
